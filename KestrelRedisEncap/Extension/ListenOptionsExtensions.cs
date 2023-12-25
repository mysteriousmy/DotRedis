using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace KestrelRedisEncap;
public static partial class ListenOptionsExtensions
{
    /// <summary>
    /// 使用RedisConnectionHandler
    /// </summary>
    /// <param name="listen"></param>
    public static void UseRedis(this ListenOptions listen)
    {
        listen.UseConnectionHandler<RedisConnectionHandler>();
    }
}