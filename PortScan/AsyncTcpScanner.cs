using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PortScan
{
    public class AsyncTcpScanner
    {
        public async Task<IEnumerable<(int, Protocol)>> Scan(IPAddress ipAddr, int[] ports)
        {
            if (await PingAddr(ipAddr) == IPStatus.Success)
            {
                var partTasks = ports.Select(port => CheckPort(ipAddr, port)).ToArray();
                return (await Task.WhenAll(partTasks))
                    .Where(x => x.portStatus == PortStatus.OPEN)
                    .Select(x => (x.port, x.protocol));
            }

            return Array.Empty<(int, Protocol)>();
        }

        private async Task<IPStatus> PingAddr(IPAddress ipAddr, int timeout = 3000)
        {
            using var ping = new Ping();
            var status = await ping.SendPingAsync(ipAddr, timeout);
            return status.Status;
        }

        private async Task<(int port, PortStatus portStatus, Protocol protocol)> CheckPort(IPAddress ipAddr, int port,
            int timeout = 3000)
        {
            while (true)
            {
                try
                {
                    using var tcpClient = new TcpClient {ReceiveTimeout = timeout};

                    try
                    {
                        await tcpClient.ConnectAsync(ipAddr, port);
                        if (tcpClient.Connected)
                            return (port, PortStatus.OPEN, await CheckProtocol(tcpClient));
                    }
                    catch
                    {
                        // ignored
                    }

                    return (port, PortStatus.CLOSED, Protocol.UNDEFINED);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private async Task<Protocol> CheckProtocol(TcpClient tcpClient)
        {
            try
            {
                var buffer = new byte[1024];
                var stream = tcpClient.GetStream();
                int count;
                try
                {
                    count = await stream.ReadAsync(buffer, 0, 1024);
                }
                catch
                {
                    //b'\x13' + b'\x00' * 39 + b'\x6f\x89\xe9\x1a\xb6\xd5\x3b\xd3'
                    await stream.WriteAsync(Encoding.UTF8.GetBytes("GET / HTTP/1.1\n\n"));
                    count = await stream.ReadAsync(buffer, 0, 1024);
                }

                if (count != 0)
                {
                    var s = Encoding.UTF8.GetString(buffer.Take(count).ToArray());
                    Console.WriteLine(s);
                    if (s.Contains("HTTP/1.1"))
                        return Protocol.HTTP;
                    if (s.Contains("smtp"))
                        return Protocol.SMTP;
                    if (s.Contains("IMAP"))
                        return Protocol.IMAP;
                    if (s.Contains("OK"))
                        return Protocol.POP3;
                }
            }
            catch
            {
                // ignored
            }

            return Protocol.UNDEFINED;
        }
    }
}