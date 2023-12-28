namespace KestrelRedisEncap;

sealed class InfoHandler : IRedisCmdHandler
{
    public RedisCmd Cmd => RedisCmd.Info;

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="context"></param> 
    /// <returns></returns>
    public async ValueTask HandleAsync(RedisContext context)
    {
        //$935
        //redis_version: 2.4.6

        const string info = "redis_version: 9.9.9";
        await context.Response
            .Write('$').Write(info.Length.ToString()).WriteLine()
            .Write(info).WriteLine()
            .FlushAsync();
    }
}