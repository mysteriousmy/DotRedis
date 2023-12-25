
namespace KestrelRedisEncap;

sealed class CmdMiddleware : IRedisMiddleware
{
    private readonly Dictionary<RedisCmd, IRedisCmdHanler> cmdHandlers;

    public CmdMiddleware(IEnumerable<IRedisCmdHanler> cmdHanlers)
    {
        this.cmdHandlers = cmdHanlers.ToDictionary(item => item.Cmd, item => item);
    }

    public async Task InvokeAsync(RedisDelegate<RedisContext> next, RedisContext context)
    {
        if (this.cmdHandlers.TryGetValue(context.Reqeust.Cmd, out var hanler))
        {
            await hanler.HandleAsync(context);
        }
        else
        {
            await next(context);
        }
    }
}