namespace KestrelRedisEncap;

sealed class SelectHandler : IRedisCmdHandler
{
    public RedisCmd Cmd => RedisCmd.Select;

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="context"></param> 
    /// <returns></returns>
    public async ValueTask HandleAsync(RedisContext context)
    {
        await context.Response.WriteAsync(ResponseContent.OK);
    }
}