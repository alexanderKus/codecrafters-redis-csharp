namespace codecrafters_redis.Commands;

public sealed record SetCommand(string Key, string Value) : RedisCommand;