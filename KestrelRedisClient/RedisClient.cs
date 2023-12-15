using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace KestrelRedisClient;

public class RedisClient
{
    // 定义一个 TcpClient 对象，表示连接到 redis server 的套接字
    private TcpClient client;
    // 定义一个 NetworkStream 对象，表示套接字的数据流
    private NetworkStream stream;
    // 定义一个字符串，表示 redis server 的 IP 地址
    private string host;
    // 定义一个整数，表示 redis server 的端口号
    private int port;

    // 定义一个构造函数，接受一个 IP 地址和一个端口号作为参数
    public RedisClient(string host, int port)
    {
        // 初始化 host 和 port
        this.host = host;
        this.port = port;
    }

    // 定义一个方法，用来连接到 redis server
    public async Task Connect()
    {
        try
        {
            // 创建一个 TcpClient 对象，连接到指定的 IP 地址和端口号
            client = new TcpClient();
            await client.ConnectAsync(host, port);
            // 获取套接字的数据流
            stream = client.GetStream();
            // 打印连接成功的信息
            //Console.WriteLine("Connected to redis server at {0}:{1}", host, port);
        }
        catch (Exception e)
        {
            // 如果发生异常，抛出一个 RedisException 对象
            throw new RedisException("Connect error", e);
        }
    }

    // 定义一个方法，用来发送 RESP 格式的数据给 server
    public async Task Send(string data)
    {
        try
        {
            // 将数据转换为字节串
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            // 将字节串写入数据流
            await stream.WriteAsync(bytes, 0, bytes.Length);
            // 刷新数据流
            await stream.FlushAsync();
        }
        catch (Exception e)
        {
            // 如果发生异常，抛出一个 RedisException 对象
            throw new RedisException("Send error", e);
        }
    }

    // 定义一个方法，用来接收 server 返回的 RESP 格式的数据
    public async Task<string> Receive()
    {
        try
        {
            // 定义一个字节数组，用来存储接收到的数据
            byte[] bytes = new byte[1024];
            // 定义一个字符串，用来存储转换后的数据
            string data = "";
            // 定义一个整数，用来存储读取到的字节数
            int count = 0;
            // 从数据流中读取数据，直到读到换行符或数据流结束
            do
            {
                // 读取数据流中的数据，返回读取到的字节数
                count = await stream.ReadAsync(bytes, 0, bytes.Length);
                // 如果读取到的字节数大于 0，将字节串转换为字符串，并追加到 data 中
                if (count > 0)
                {
                    data += Encoding.UTF8.GetString(bytes, 0, count);
                }
                // 如果 data 以换行符结尾，跳出循环
                if (data.EndsWith("\r\n"))
                {
                    break;
                }
            } while (count > 0);
            // 返回 data
            return data;
        }
        catch (Exception e)
        {
            // 如果发生异常，抛出一个 RedisException 对象
            throw new RedisException("Receive error", e);
        }
    }

    // 定义一个方法，用来执行任意的 redis 命令，并返回结果或错误信息
    public async Task<string> Execute(string command, params string[] args)
    {
        try
        {
            // 定义一个字符串，用来存储 RESP 格式的数据
            string data = "";
            // 将命令转换为大写
            command = command.ToUpper();
            // 计算请求的数组长度，即命令和参数的个数
            int length = args.Length + 1;
            // 将数组长度转换为 RESP 格式，即 *length\r\n
            data += $"*{length}\r\n";
            // 将命令转换为 RESP 格式，即 $command.Length\r\ncommand\r\n
            data += $"${command.Length}\r\n{command}\r\n";
            // 遍历参数，将每个参数转换为 RESP 格式，即 $arg.Length\r\narg\r\n
            foreach (string arg in args)
            {
                data += $"${arg.Length}\r\n{arg}\r\n";
            }
            // 发送数据给 server

            await Send(data);

            // 接收 server 返回的数据
            data = await Receive();

            // 返回 data
            return data;
        }
        catch (Exception e)
        {
            // 如果发生异常，抛出一个 RedisException 对象
            throw new RedisException("Execute error", e);
        }
    }

    // 定义一个方法，用来关闭连接
    public void Close()
    {
        try
        {
            // 关闭数据流
            stream.Close();
            // 关闭套接字
            client.Close();
            // 打印关闭成功的信息
            //Console.WriteLine("Closed connection to redis server");
        }
        catch (Exception e)
        {
            // 如果发生异常，抛出一个 RedisException 对象
            throw new RedisException("Close error", e);
        }
    }
}