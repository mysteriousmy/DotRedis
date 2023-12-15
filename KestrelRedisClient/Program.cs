namespace KestrelRedisClient;
class Program
{
    static async Task Main(string[] args)
    {
        await dos(1);
    }
    public static async Task dos(int index)
    {
        RedisClient client = new RedisClient("127.0.0.1", 6379);
        RedisCommand command = new RedisCommand(client);
        await client.Connect();

        await command.Set($"name{index}", $"GPT4"); // 使用 command 调用 Set 方法
        await command.Get($"name{index}"); // 使用 command 调用 Get 方法
        Console.WriteLine(command.Del("name")); // 使用 command 调用 Del 方法
        Console.WriteLine(command.Get("name")); // 使用 command 调用 Get 方法
        client.Close();
    }
}