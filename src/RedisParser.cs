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

        return ParseCommand(strings);
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
    
    private RedisCommand ParseCommand(List<string> data) 
        => data[0] switch
    {
        "PING" => new PongCommand(),
        "ECHO" => new EchoCommand(data[1]),
        "GET"  => new GetCommand(data[1]),
        "SET"  => ParseSerCommand(data),
        _ => throw new Exception($"Unsupported command ")
    };

    private RedisCommand ParseSerCommand(List<string> data)
    {
        if (data.Count == 3)
            return new SetCommand(data[1], data[2]);
        if (data.Count == 5)
        {
            var commandCommand = data[3].ToUpper();
            switch (commandCommand)
            {
                case "PX":
                {
                    var ms = int.Parse(data[4]);
                    return new SetCommand(data[1], data[2], TimeSpan.FromMilliseconds(ms));
                }
                case "EX":
                {
                    var s = int.Parse(data[4]);
                    return new SetCommand(data[1], data[2], TimeSpan.FromSeconds(s));
                }
            }
        }

        Console.WriteLine("Unsupported SetCommand Type");
        throw new Exception("Unsupported SetCommand Type");
    }
}