namespace KestrelRedisEncap;

sealed class QuitHandler : IRedisCmdHandler
{
    public RedisCmd Cmd => RedisCmd.Quit;

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="context"></param> 
    /// <returns></returns>
    public ValueTask HandleAsync(RedisContext context)
    {
        context.Client.Close();
        return ValueTask.CompletedTask;
    }
}