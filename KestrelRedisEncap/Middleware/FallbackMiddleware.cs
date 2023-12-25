
namespace KestrelRedisEncap;

sealed class FallbackMiddleware(ILogger<FallbackMiddleware> logger) : IRedisMiddleware
{
    private readonly ILogger<FallbackMiddleware> logger = logger;


    public async Task InvokeAsync(RedisDelegate<RedisContext> next, RedisContext context)
    {
        this.logger.LogWarning($"无法处理{context.Reqeust}");
        await context.Response.WriteAsync(ResponseContent.Err);
    }
}