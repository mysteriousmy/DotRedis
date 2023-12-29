using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace KestrelRedis;

public class RedisServer(int port)
{
    private readonly IWebHost _host = new WebHostBuilder()
            .UseKestrel(options =>
            {
                options.Listen(IPAddress.Any, port, listenOptions =>
                {
                    listenOptions.UseConnectionHandler<RedisConnectionHandler>();
                });
            })
            .UseStartup<Startup>()
            .Build();
    private RedisCommandDelegate _commandDelegate = new(new RedisDatabase());

    public async Task StartAsync()
    {
        await _host.StartAsync();
        Console.WriteLine("Redis server started on port {0}", _host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First());
    }

    public async Task StopAsync()
    {
        await _host.StopAsync();
        Console.WriteLine("Redis server stopped");
    }
    public RedisCommandDelegate GetCommandDelegate()
    {
        return _commandDelegate;
    }

}