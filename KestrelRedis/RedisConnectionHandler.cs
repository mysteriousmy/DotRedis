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

    private string[]? ParseCommand(ref ReadOnlySequence<byte> buffer)
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
        var db = _server.GetDatabase();
        //System.Console.WriteLine(command[2]);
        switch (command[2].ToUpper())
        {
            case "PING":
                return "+PONG\r\n";
            case "SET":
                if (command.Length >= 3)
                {
                    db.Set(command[4], command[6]);
                    //System.Console.WriteLine(command[4]);
                    return "+OK\r\n";
                }
                else
                {
                    return "-ERR wrong number of arguments for 'set' command\r\n";
                }
            case "GET":
                if (command.Length >= 2)
                {
                    var value = db.Get(command[4]);
                    if (value != null)
                    {
                        return "$" + value.Length + "\r\n" + value + "\r\n";
                    }
                    else
                    {
                        return "$-1\r\n";
                    }
                }
                else
                {
                    return "-ERR wrong number of arguments for 'get' command\r\n";
                }
            case "DEL":
                if (command.Length > 1)
                {
                    var count = db.Delete(command[1..]);
                    return ":" + count + "\r\n";
                }
                else
                {
                    return "-ERR wrong number of arguments for 'del' command\r\n";
                }
            default:
                return "-ERR unknown command '" + command[0] + "'\r\n";
        }
    }

    private async Task WriteResponseAsync(PipeWriter pipeWriter, string response)
    {
        var bytes = Encoding.UTF8.GetBytes(response);
        await pipeWriter.WriteAsync(bytes);
    }
}
