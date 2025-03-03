namespace codecrafters_redis.Commands;

public sealed record PongCommand(string Args = "PONG") : RedisCommand;