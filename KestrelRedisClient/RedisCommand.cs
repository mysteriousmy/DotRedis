namespace KestrelRedisClient;

public class RedisCommand
{
    // 定义一个 RedisClient 对象，表示与 redis server 的连接
    private RedisClient client;

    // 定义一个构造函数，接受一个 RedisClient 对象作为参数
    public RedisCommand(RedisClient client)
    {
        // 初始化 client
        this.client = client;
    }

    // 定义一个方法，用来设置键值对，调用 Execute 方法，并解析返回的数据
    public async Task<bool> Set(string key, string value)
    {

        // 执行 SET 命令，传入键和值作为参数
        string data = await client.Execute("SET", key, value);
        // 如果返回的数据是 "+OK\r\n"，表示设置成功，返回 true
        //System.Console.WriteLine(data);
        if (data == "+OK\r\n")
        {
            return true;
        }
        // 否则，表示设置失败，抛出一个 RedisException 对象
        else
        {
            throw new RedisException(data);
        }
    }

    // 定义一个方法，用来获取键对应的值，调用 Execute 方法，并解析返回的数据
    public async Task<string> Get(string key)
    {
        // 执行 GET 命令，传入键作为参数
        string data = await client.Execute("GET", key);
        // 如果返回的数据是 "$-1\r\n"，表示键不存在，返回 null
        if (data == "$-1\r\n")
        {
            return null;
        }
        // 否则，表示键存在，解析返回的数据，获取值的长度和值
        else
        {
            // 去掉返回的数据的前两个字符和最后两个字符，即 "$" 和 "\r\n"
            data = data.Substring(1, data.Length - 3);
            // 以 "\r\n" 为分隔符，将数据分割为两部分，第一部分是值的长度，第二部分是值
            string[] parts = data.Split("\r\n");
            // 获取值的长度，转换为整数
            int length = int.Parse(parts[0]);
            // 获取值，截取指定的长度
            string value = parts[1].Substring(0, length);
            // 返回值
            return value;
        }
    }

    // 定义一个方法，用来删除键，调用 Execute 方法，并解析返回的数据
    public async Task<bool> Del(string key)
    {
        // 执行 DEL 命令，传入键作为参数
        string data = await client.Execute("DEL", key);
        // 如果返回的数据是 ":1\r\n"，表示删除成功，返回 true
        if (data == ":1\r\n")
        {
            return true;
        }
        // 否则，表示删除失败，抛出一个 RedisException 对象
        else
        {
            throw new RedisException(data);
        }
    }

    // 定义一个方法，用来获取所有的键，调用 Execute 方法，并解析返回的数据
    public async Task<string[]> Keys(string pattern)
    {
        // 执行 KEYS 命令，传入匹配模式作为参数
        string data = await client.Execute("KEYS", pattern);
        // 如果返回的数据是 "*0\r\n"，表示没有匹配的键，返回空数组
        if (data == "*0\r\n")
        {
            return new string[0];
        }
        // 否则，表示有匹配的键，解析返回的数据，获取数组的长度和元素
        else
        {
            // 去掉返回的数据的前两个字符和最后两个字符，即 "*" 和 "\r\n"
            data = data.Substring(1, data.Length - 3);
            // 以 "\r\n" 为分隔符，将数据分割为多个部分，第一部分是数组的长度，剩余的部分是数组的元素
            string[] parts = data.Split("\r\n");
            // 获取数组的长度，转换为整数
            int length = int.Parse(parts[0]);
            // 定义一个字符串数组，用来存储数组的元素
            string[] keys = new string[length];
            // 遍历数组的元素，每个元素都是一个 RESP 批量字符串，需要去掉前两个字符和最后两个字符，即 "$" 和 "\r\n"
            for (int i = 0; i < length; i++)
            {
                // 获取元素的索引，即 i * 2 + 1
                int index = i * 2 + 1;
                // 获取元素的内容，去掉前两个字符和最后两个字符
                string key = parts[index].Substring(1, parts[index].Length - 3);
                // 将元素存储到字符串数组中
                keys[i] = key;
            }
            // 返回字符串数组
            return keys;
        }
    }
}