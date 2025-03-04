using codecrafters_redis.Commands;

namespace codecrafters_redis;

internal sealed record RedisDictValue(string Value, DateTimeOffset? ExpireDate = default);

public sealed record RedisEngine
{
    private readonly Dictionary<string, RedisDictValue> _dict = new ();
    
    public string Run(RedisCommand command)
    {
        switch (command)
        {
            case EchoCommand echoCommand: return $"${echoCommand.Args.Length}\r\n{echoCommand.Args}\r\n";
            case PongCommand pongCommand: return $"${pongCommand.Args.Length}\r\n{pongCommand.Args}\r\n";
            case SetCommand setCommand: return SetRedisDict(setCommand.Key, setCommand.Value, setCommand.Ttl);
            case GetCommand getCommand: return GetRedisDict(getCommand.Key);
            default: throw new Exception($"Does not know how to run command: {command}");
        }
    }

    private string SetRedisDict(string key, string value, TimeSpan? ttl = default)
    {
        if (ttl is not null)
            return _dict.TryAdd(key, new RedisDictValue(value, DateTimeOffset.UtcNow + ttl)) ? $"+OK\r\n" : $"$-1\r\n";
        return _dict.TryAdd(key, new RedisDictValue(value)) ? $"+OK\r\n" : $"$-1\r\n";
    }

    private string GetRedisDict(string key)
    {
        if (_dict.TryGetValue(key, out var value))
        {
            if (value.ExpireDate is null) 
                return $"${value.Value.Length}\r\n{value.Value}\r\n";
            if (value.ExpireDate >= DateTimeOffset.UtcNow)
                return $"${value.Value.Length}\r\n{value.Value}\r\n";
            _dict.Remove(key);
        }
        return $"$-1\r\n";
    }
}