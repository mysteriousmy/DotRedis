namespace KestrelRedis;
class Program
{
    static async Task Main(string[] args)
    {
        var port = 6379;
        if (args.Length > 0 && int.TryParse(args[0], out var p))
        {
            port = p;
        }
        var server = new RedisServer(port);
        Console.CancelKeyPress += async (sender, e) =>
        {
            e.Cancel = true;
            await server.StopAsync();
            Environment.Exit(0);
        };

        await server.StartAsync();
        Console.WriteLine("Press Ctrl+C to exit");
        await Task.Delay(-1);
    }

}
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add any services you need here
        services.AddSingleton(a => new RedisServer(6379));
        services.AddSingleton<RedisConnectionHandler>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Add any middleware you need here
    }
}