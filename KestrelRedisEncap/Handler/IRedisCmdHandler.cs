namespace KestrelRedisEncap;

interface IRedisCmdHandler
{
    RedisCmd Cmd {get;}

    ValueTask HandleAsync(RedisContext context);
}