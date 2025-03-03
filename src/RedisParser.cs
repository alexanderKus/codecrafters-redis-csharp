using codecrafters_redis.Commands;

namespace codecrafters_redis;

public class RedisParser
{
    public List<RedisCommand> Parse(ReadOnlySpan<byte> stream)
    {
        List<RedisCommand> commands = new();
        var index = 0;
        if (stream[index++] != '*')
            throw new Exception($"Stream does not start with *. {stream.ToString()}");
        var e = index+1;
        while (char.IsDigit((char)stream[e]))
            e++;
        var lines = int.Parse(stream[index..e]);
        index = e + 2;
        for (var i = 0; i < lines; i++)
        {
            if (stream[index++] != '$')
                throw new Exception("Length must start with $");
            e = index+1;
            while (char.IsDigit((char)stream[e]))
                e++;
            var commandName = stream[index..e].ToString();
            index = e + 2;
            while (char.IsDigit((char)stream[e]))
                e++;
            var args = stream[index..e].ToString();
            index = e + 2;
            commands.Add(ParseCommand(commandName, args));
        }

        return commands;
    }

    private RedisCommand ParseCommand(ReadOnlySpan<char> commandName, string args) 
        => commandName switch
    {
        "ECHO" => new EchoCommand(args),
        _ => throw new Exception($"Unsupported command {commandName}")
    };
}