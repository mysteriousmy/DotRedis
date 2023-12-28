using System.Text;

namespace KestrelRedisEncap;

abstract class ResponseContent
{
    /// <summary>
    /// OK
    /// </summary>
    public static ResponseContent OK { get; } = new StringContent("+OK\r\n");

    /// <summary>
    /// Error
    /// </summary>

    /// <summary>
    /// pong
    /// </summary>
    public static ResponseContent Pong { get; } = new StringContent("+PONG\r\n");

    public static ResponseContent Null { get; } = new StringContent("$-1\r\n");

    internal static ResponseContent Err(string message)
    {
        return new StringContent($"-ERR{message}\r\n");
    }

    internal static ResponseContent GenErr(string method)
    {
        return Err($" wrong number of arguments for {method} command");
    }
    internal static ResponseContent Value(string result)
    {
        return new StringContent($"${result.Length}\r\n{result}\r\n");
    }
    
    internal static ResponseContent Option(int result)
    {
        return new StringContent($":{result}\r\n");
    }

    public abstract ReadOnlyMemory<byte> ToMemory();

    /// <summary>
    /// 文本响应内容
    /// </summary>
    private class StringContent : ResponseContent
    {
        private readonly ReadOnlyMemory<byte> value;

        public StringContent(string value)
        {
            this.value = Encoding.UTF8.GetBytes(value);
        }

        public override ReadOnlyMemory<byte> ToMemory()
        {
            return this.value;
        }
    }
}