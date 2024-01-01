using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
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
                options.Limits.MaxConcurrentConnections = int.MaxValue;
                options.Limits.MaxConcurrentUpgradedConnections = int.MaxValue;
            })
            .UseStartup<Startup>()
            .Build();
    private readonly RedisCommandDelegate _commandDelegate = new(new RedisDatabase());

    public async ValueTask StartAsync()
    {
        await _host.StartAsync();
        Console.WriteLine("Redis server started on port {0}", _host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First());
    }

    public async ValueTask StopAsync()
    {
        await _host.StopAsync();
        Console.WriteLine("Redis server stopped");
    }
    public RedisCommandDelegate GetCommandDelegate()
    {
        return _commandDelegate;
    }

}