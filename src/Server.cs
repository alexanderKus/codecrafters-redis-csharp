using System.Net;
using System.Net.Sockets;

TcpListener server = new(IPAddress.Any, 6379);
server.Start();

Socket clientSocket = server.AcceptSocket();
while (clientSocket.Connected)
{
    Span<byte> buffer = new byte[1024];
    clientSocket.Receive(buffer);
    clientSocket.Send("+PONG\r\n"u8.ToArray());
}