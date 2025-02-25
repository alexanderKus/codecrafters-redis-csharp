using System.Net;
using System.Net.Sockets;

TcpListener server = new(IPAddress.Any, 6379);
server.Start();

Socket clientSocket = server.AcceptSocket();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
HandleConnection(clientSocket);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

async Task HandleConnection(Socket socket)
{
    while (clientSocket.Connected)
    {
        var buffer = new byte[1024];
        await clientSocket.ReceiveAsync(buffer);
        await clientSocket.SendAsync("+PONG\r\n"u8.ToArray());
    }
}