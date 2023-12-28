namespace KestrelRedisEncap;
sealed class PingHandler : IRedisCmdHandler
{
    public RedisCmd Cmd => RedisCmd.Ping;

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="context"></param> 
    /// <returns></returns>
    public async ValueTask HandleAsync(RedisContext context)
    {
        await context.Response.WriteAsync(ResponseContent.Pong);
    }
}