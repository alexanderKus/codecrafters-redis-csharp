namespace codecrafters_redis.Commands;

public sealed record GetCommand(string Key) : RedisCommand;