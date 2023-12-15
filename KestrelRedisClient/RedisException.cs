namespace KestrelRedisClient;

public class RedisException : Exception
{
    // 定义一个构造函数，接受一个字符串作为参数，表示异常的消息
    public RedisException(string message) : base(message)
    {
        // 调用基类的构造函数，传入消息作为参数
    }

    // 定义一个构造函数，接受两个参数，一个字符串表示异常的消息，一个 Exception 对象表示异常的原因
    public RedisException(string message, Exception innerException) : base(message, innerException)
    {
        // 调用基类的构造函数，传入消息和原因作为参数
    }
}