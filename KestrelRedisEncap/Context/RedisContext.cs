using Microsoft.AspNetCore.Http.Features;

namespace KestrelRedisEncap;

/// <summary>
/// redis上下文
/// </summary>
/// <param name="client"></param>
/// <param name="request"></param>
/// <param name="response"></param>
/// <param name="features"></param> 
sealed class RedisContext(RedisClient client, RedisRequest request, RedisResponse response, IFeatureCollection features) : RedisApplicationContext(features)
{
    /// <summary>
    /// 获取redis客户端
    /// </summary>
    public RedisClient Client { get; } = client;

    /// <summary>
    /// 获取redis请求
    /// </summary>
    public RedisRequest Reqeust { get; } = request;

    /// <summary>
    /// 获取redis响应
    /// </summary>
    public RedisResponse Response { get; } = response;

    public override string ToString()
    {
        return $"{this.Client} {this.Reqeust}";
    }
}