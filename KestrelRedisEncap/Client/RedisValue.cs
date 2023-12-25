using System.Text;

namespace KestrelRedisEncap;

sealed class RedisValue(ReadOnlyMemory<byte> value)
{
    public ReadOnlyMemory<byte> Value { get; } = value;

    public override string ToString()
    {
        return Encoding.UTF8.GetString(this.Value.Span);
    }
}