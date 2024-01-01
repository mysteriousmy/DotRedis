using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using Microsoft.AspNetCore.Connections;

namespace KestrelRedis;

public class RedisConnectionHandler(RedisServer server) : ConnectionHandler
{

    private readonly RedisServer _server = server;

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        //System.Console.WriteLine("Client connected: {0}", connection.ConnectionId);
        var pipeReader = connection.Transport.Input;
        var pipeWriter = connection.Transport.Output;
        try
        {
            while (true)
            {
                var result = await pipeReader.ReadAsync();
                var buffer = result.Buffer;
                var command = ParseCommand(ref buffer);
                if (command != null)
                {
                    var response = ExcuteCommand(command);
                    await WriteResponseAsync(pipeWriter, response);
                }
                //System.Console.WriteLine(buffer);
                pipeReader.AdvanceTo(buffer.End);
                if (result.IsCompleted || result.IsCanceled)
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("Client error: {0}", ex.Message);
        }
        finally
        {
            //System.Console.WriteLine("Client disconnected: {0}", connection.ConnectionId);
            await pipeWriter.CompleteAsync();
            await pipeReader.CompleteAsync();
        }
    }

    private static string[]? ParseCommand(ref ReadOnlySequence<byte> buffer)
    {
        if (buffer.IsSingleSegment)
        {
            var line = Encoding.UTF8.GetString(buffer.FirstSpan);
            if (line.EndsWith("\r\n"))
            {
                return line.Split("\r\n");
            }
        }
        return null;
    }

    private string ExcuteCommand(string[] command)
    {
        var commandHandler = _server.GetCommandDelegate().GetCommandHandlers();
        var commandType = command[2].ToUpper();
        if (commandHandler.TryGetValue(commandType, out RedisCommandDelegate.CommandHandler? value))
        {
            var handler = value;
            return handler(command);
        }
        else
        {
            return "-ERR unknown command '" + command[0] + "'\r\n";
        }

    }

    private static async ValueTask WriteResponseAsync(PipeWriter pipeWriter, string response)
    {
        var bytes = Encoding.UTF8.GetBytes(response);
        await pipeWriter.WriteAsync(bytes);
    }
}
