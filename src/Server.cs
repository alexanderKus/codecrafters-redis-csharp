using System.Net;
using System.Net.Sockets;
using System.Text;

var server = new TcpListener(IPAddress.Any, 6379);
server.Start();
var clientSocket = server.AcceptSocket();
clientSocket.Send(Encoding.UTF8.GetBytes("+PONG\r\n"), SocketFlags.None);