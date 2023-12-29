namespace KestrelRedis;

public class RedisCommandDelegate
{
    public delegate string CommandHandler(string[] command);
    private readonly RedisDatabase _db;
    private readonly Dictionary<string, CommandHandler> commandHandlers = [];

    public RedisCommandDelegate(RedisDatabase db)
    {
        this._db = db;
        commandHandlers.Add("PING", Ping);
        commandHandlers.Add("SET", Set);
        commandHandlers.Add("GET", Get);
        commandHandlers.Add("DEL", Del);
    }

    public Dictionary<string, CommandHandler> GetCommandHandlers()
    {
        return commandHandlers;
    }
    string Ping(string[] command)
    {
        return "+PONG\r\n";
    }

    string Set(string[] command)
    {
        if (command.Length >= 3)
        {
            _db.Set(command[4], command[6]);
            return "+OK\r\n";
        }
        else
        {
            return "-ERR wrong number of arguments for 'set' command\r\n";
        }
    }

    string Get(string[] command)
    {
        if (command.Length >= 2)
        {
            var value = _db.Get(command[4]);
            if (value != null)
            {
                return "" + value.Length + "\r\n" + value + "\r\n";
            }
            else
            {
                return "-1\r\n";
            }
        }
        else
        {
            return "-ERR wrong number of arguments for 'get' command\r\n";
        }
    }

    string Del(string[] command)
    {
        if (command.Length > 1)
        {
            var count = _db.Delete(command[1..]);
            return ":" + count + "\r\n";
        }
        else
        {
            return "-ERR wrong number of arguments for 'del' command\r\n";
        }
    }
}
