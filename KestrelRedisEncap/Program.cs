using KestrelRedisEncap;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRedis();
builder.WebHost.ConfigureKestrel((context, kestrel) =>
{
    var section = context.Configuration.GetSection("Kestrel");
    kestrel.Configure(section).Endpoint("Redis", endpoint =>
    {
        endpoint.ListenOptions.UseRedis();
    });
});
var app = builder.Build();
app.UseRouting();
app.Map("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync("hello world");
});

app.Run();
