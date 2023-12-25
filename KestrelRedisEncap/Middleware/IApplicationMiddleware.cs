namespace KestrelRedisEncap;

public interface IApplicationMiddleware<TContext>
{

    Task InvokeAsync(RedisDelegate<TContext> next, TContext context);
}