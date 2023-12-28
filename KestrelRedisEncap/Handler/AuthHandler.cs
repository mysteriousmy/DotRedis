
using System.Text;
using Microsoft.Extensions.Options;

namespace KestrelRedisEncap;

/// <summary>
/// Auth处理者
/// </summary>
/// <param name="options"></param>
sealed class AuthHandler(IOptionsMonitor<RedisOptions> options) : IRedisCmdHandler
{
    private readonly IOptionsMonitor<RedisOptions> options = options;

    public RedisCmd Cmd => RedisCmd.Auth;

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="context"></param> 
    /// <returns></returns>
    public async ValueTask HandleAsync(RedisContext context)
    {
        var client = context.Client;
        var auth = this.options.CurrentValue.Auth;

        if (string.IsNullOrEmpty(auth))
        {
            client.IsAuthed = true;
        }
        else if (context.Reqeust.ArgumentCount > 0)
        {
            var password = context.Reqeust.Argument(0).Value;
            client.IsAuthed = password.Span.SequenceEqual(Encoding.UTF8.GetBytes(auth));
        }
        else
        {
            client.IsAuthed = false;
        }

        if (client.IsAuthed == true)
        {
            await context.Response.WriteAsync(ResponseContent.OK);
        }
        else
        {
            await context.Response.WriteAsync(ResponseContent.Err(""));
        }
    }
}