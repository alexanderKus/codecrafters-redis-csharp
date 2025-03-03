using System.Text;
using codecrafters_redis.Commands;

namespace codecrafters_redis;

public class RedisParser
{
    public RedisCommand Parse(ReadOnlySpan<byte> stream)
    {
        var index = 0;
        if (stream[index++] != '*')
            throw new Exception($"Stream does not start with *. {stream.ToString()}");
        var e = index+1;
        while (char.IsDigit((char)stream[e]))
            e++;
        var lines = int.Parse(stream[index..e]);
        index = e + 2;
        List<string> strings = new();
        for (var i = 0; i < lines; i++)
        {
            var (result, len) = ParseBulkString(stream[index..]);
            strings.Add(result);
            index += len;
        }

        if (strings.Count != 2) throw new Exception("Too much commands");
        return ParseCommand(strings[0], strings[1]);
    }

    private (string, int) ParseBulkString(ReadOnlySpan<byte> buffer)
    {
        var index = 0;
        if (buffer[index++] != '$')
            throw new Exception("Length must start with $");
        var e = index+1;
        while (buffer[e] != '\r' && buffer[e+1] != '\n')
            e++;
        var len = int.Parse(buffer[index..e]);
        index = e + 2;
        return (Encoding.UTF8.GetString(buffer[index..(index+len)]), index+len+2);
    }
    
    private RedisCommand ParseCommand(ReadOnlySpan<char> commandName, string args) 
        => commandName switch
    {
        "ECHO" => new EchoCommand(args),
        _ => throw new Exception($"Unsupported command {commandName}")
    };
}