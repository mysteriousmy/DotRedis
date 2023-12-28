
using Microsoft.Extensions.Options;

namespace KestrelRedisEncap;

sealed class AuthMiddleware(ILogger<AuthMiddleware> logger, IOptionsMonitor<RedisOptions> options) : IRedisMiddleware
{
    private readonly ILogger<AuthMiddleware> logger = logger;
    private readonly IOptionsMonitor<RedisOptions> options = options;

    public async Task InvokeAsync(RedisDelegate<RedisContext> next, RedisContext context)
    {
        if (context.Client.IsAuthed == false)
        {
            await context.Response.WriteAsync(ResponseContent.Err(" auth failed"));
        }
        else if (context.Client.IsAuthed == true)
        {
            await next(context);
        }
        else if (context.Reqeust.Cmd != RedisCmd.Auth)
        {
            if (string.IsNullOrEmpty(options.CurrentValue.Auth))
            {
                context.Client.IsAuthed = true;
                await next(context);
            }
            else
            {
                this.logger.LogWarning("需要客户端Auth");
                await context.Response.WriteAsync(ResponseContent.Err(" need client auth"));
            }
        }
        else
        {
            await next(context);
        }
    }
}