
namespace KestrelRedisEncap;
class SetHandler : IRedisCmdHandler
{
    public RedisCmd Cmd => RedisCmd.Set;

    public async ValueTask HandleAsync(RedisContext context)
    {
        if (context.Reqeust.ArgumentCount >= 2)
        {
            context.Database.Set(context.Reqeust.Argument(0).ToString(), context.Reqeust.Argument(1).ToString());
            await context.Response.WriteAsync(ResponseContent.OK);
        }
        else
        {
            await context.Response.WriteAsync(ResponseContent.GenErr(Cmd.ToString()));
        }
    }
}