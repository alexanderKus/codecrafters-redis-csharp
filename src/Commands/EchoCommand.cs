namespace codecrafters_redis.Commands;

public sealed record EchoCommand(string Args) : RedisCommand;
