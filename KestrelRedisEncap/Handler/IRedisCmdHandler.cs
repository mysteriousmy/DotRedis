namespace KestrelRedisEncap;

interface IRedisCmdHanler
{
    RedisCmd Cmd {get;}

    ValueTask HandleAsync(RedisContext context);
}