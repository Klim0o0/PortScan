using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PortScan
{
    public static class TcpClientExtensions
    {
            public static async Task<Task> ConnectWithTimeoutAsync(this TcpClient tcpClient, IPAddress ipAddr, int port, int timeout = 3000)
        {
            var connectTask = tcpClient.ConnectAsync(ipAddr, port);
            await Task.WhenAny(connectTask, Task.Delay(timeout));
            return connectTask;
        }
            
            public static async Task<Task<UdpReceiveResult>> ReceiveWithTimeoutAsync(this UdpClient tcpClient ,int timeout = 3000)
            {
                var connectTask = tcpClient.ReceiveAsync();
                await Task.WhenAny(connectTask, Task.Delay(timeout));
                return connectTask;
            }
    }
}