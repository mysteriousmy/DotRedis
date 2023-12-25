using Microsoft.AspNetCore.Http.Features;

namespace KestrelRedisEncap;

public abstract class RedisApplicationContext(IFeatureCollection features)
{
    public IFeatureCollection Features { get; } = new FeatureCollection(features);
}