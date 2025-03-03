using codecrafters_redis.Commands;

namespace codecrafters_redis;

public sealed record RedisEngine
{
    private readonly Dictionary<string, string> _dict = new ();
    
    public string Run(RedisCommand command)
    {
        switch (command)
        {
            case EchoCommand echoCommand: return $"${echoCommand.Args.Length}\r\n{echoCommand.Args}\r\n";
            case PongCommand pongCommand: return $"${pongCommand.Args.Length}\r\n{pongCommand.Args}\r\n";
            case SetCommand setCommand: return _dict.TryAdd(setCommand.Key, setCommand.Value) ? @"+OK\r\n" : @"$-1\r\n";
            case GetCommand getCommand: return _dict.TryGetValue(getCommand.Key, out var value) ? $"${value.Length}\r\n{value}\r\n" : @"$-1\r\n";
            default: throw new Exception($"Does not know how to run command: {command}");
        }
    }
}