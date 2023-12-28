
namespace KestrelRedisEncap;
class GetHandler : IRedisCmdHandler
{
    public RedisCmd Cmd => RedisCmd.Get;

    public async ValueTask HandleAsync(RedisContext context)
    {
        if (context.Reqeust.ArgumentCount >= 1)
        {
            var result = context.Database.Get(context.Reqeust.Argument(0).ToString());
            if (result == null)
            {
                await context.Response.WriteAsync(ResponseContent.Null);
            }
            else
            {
                await context.Response.WriteAsync(ResponseContent.Value(result));
            }
        }
        else
        {
            await context.Response.WriteAsync(ResponseContent.GenErr(Cmd.ToString()));
        }
    }
}