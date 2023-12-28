namespace KestrelRedisEncap;

enum RedisCmd
{
    UnKnown,

    Auth,

    Ping,

    Quit,

    Echo,

    Info,

    Select,
    Set,
    Get,
    Del,
}