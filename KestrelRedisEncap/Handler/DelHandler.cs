
namespace KestrelRedisEncap;

class DeleteHandler : IRedisCmdHandler
{
    public RedisCmd Cmd => RedisCmd.Del;

    public async ValueTask HandleAsync(RedisContext context)
    {
        if (context.Reqeust.ArgumentCount >= 1)
        {
            var result = context.Database.Delete(context.Reqeust.Arguments);
            if (result == 0)
            {
                await context.Response.WriteAsync(ResponseContent.Null);
            }
            else
            {
                await context.Response.WriteAsync(ResponseContent.Option(result));
            }

        }
        else
        {
            await context.Response.WriteAsync(ResponseContent.GenErr(Cmd.ToString()));
        }
    }
}