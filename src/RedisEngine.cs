using codecrafters_redis.Commands;

namespace codecrafters_redis;

public sealed record RedisEngine
{
    public string Run(RedisCommand command)
    {
        switch (command)
        {
            case EchoCommand echoCommand: return $"${echoCommand.Args.Length}\r\n{echoCommand.Args}\r\n";
            case PongCommand pongCommand: return $"${pongCommand.Args.Length}\r\n{pongCommand.Args}\r\n";
            default: throw new Exception($"Does not know how to run command: {command}");
        }
    }
}