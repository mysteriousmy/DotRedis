using Microsoft.AspNetCore.Connections;

namespace KestrelRedisEncap;

public sealed class RedisConnectionHandler(IServiceProvider appServices, ILogger<RedisConnectionHandler> logger) : ConnectionHandler
{
    private readonly ILogger<RedisConnectionHandler> logger = logger;
    private readonly RedisDelegate<RedisContext> redisServer = new RedisApplicationBuilder<RedisContext>(appServices)
                .Use<AuthMiddleware>()
                .Use<CmdMiddleware>()
                .Use<FallbackMiddleware>()
                .Build();


    public async override Task OnConnectedAsync(ConnectionContext connection)
    {
        try
        {
            await this.HandleRequestsAsync(connection);
        }
        catch (Exception ex)
        {
            this.logger.LogDebug(ex.Message);
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }
    private async Task HandleRequestsAsync(ConnectionContext context)
    {
        var input = context.Transport.Input;
        var client = new RedisClient(context);
        var response = new RedisResponse(context.Transport.Output);
        var database = new RedisDatabase();

        while (context.ConnectionClosed.IsCancellationRequested == false)
        {
            var result = await input.ReadAsync();
            if (result.IsCanceled)
            {
                break;
            }

            var requests = RedisRequest.Parse(result.Buffer, out var consumed);
            if (requests.Count > 0)
            {
                foreach (var request in requests)
                {
                    var redisContext = new RedisContext(client, request, response, database, context.Features);
                    await this.redisServer.Invoke(redisContext);
                }
                input.AdvanceTo(consumed);
            }
            else
            {
                input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
            }

            if (result.IsCompleted)
            {
                break;
            }
        }
    }
}