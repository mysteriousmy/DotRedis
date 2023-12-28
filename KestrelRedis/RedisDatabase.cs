using System.Collections.Concurrent;

namespace KestrelRedis;

public class RedisDatabase
{
    private readonly ConcurrentDictionary<string, string> _data;

    public RedisDatabase()
    {
        _data = [];
    }

    public void Set(string key, string value)
    {
        _data[key] = value;
    }

    public string? Get(string key)
    {
        if (_data.TryGetValue(key, out var value))
        {
            return value;
        }
        return null;
    }

    public int Delete(params string[] keys)
    {
        var count = 0;
        foreach (var key in keys)
        {
            _data.Remove(key, out _);
        }
        return count;
    }
}