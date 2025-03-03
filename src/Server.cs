using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_redis;

TcpListener server = new(IPAddress.Any, 6379);
server.Start();

while (true)
{
    Socket clientSocket = server.AcceptSocket();
    #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    HandleConnection(clientSocket);
    #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
}

async Task HandleConnection(Socket socket)
{
    while (socket.Connected)
    {
        var buffer = new byte[1024];
        await socket.ReceiveAsync(buffer);
        var parser = new RedisParser();
        var command = parser.Parse(buffer.AsSpan());
        var engine = new RedisEngine();
        var result = engine.Run(command.First());
        await socket.SendAsync(Encoding.UTF8.GetBytes(result));
    }
}