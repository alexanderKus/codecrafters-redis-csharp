using codecrafters_redis.Commands;

namespace codecrafters_redis;

public sealed record RedisEngine
{
    public string Run(RedisCommand command)
    {
        switch (command)
        {
            case EchoCommand echoCommand: return echoCommand.Args;
            default: throw new Exception($"Does not know how to run command: {command}");
        }
    }
}