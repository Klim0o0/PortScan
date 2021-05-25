using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PortScan
{
    public class AsyncTcpScanner : IPScanner
    {
        public virtual async Task<int[]> Scan(IPAddress ipAddr, int[] ports)
        {
            if (await PingAddr(ipAddr) == IPStatus.Success)
            {
                var partTasks = ports.Select(port => CheckPort(ipAddr, port)).ToArray();
                return (await Task.WhenAll(partTasks))
                    .Where(x => x.portStatus == PortStatus.OPEN)
                    .Select(x => x.port)
                    .ToArray();
            }

            return Array.Empty<int>();
        }

        private async Task<IPStatus> PingAddr(IPAddress ipAddr, int timeout = 3000)
        {
            using (var ping = new Ping())
            {
                var status = await ping.SendPingAsync(ipAddr, timeout);
                return status.Status;
            }
        }

        private async Task<(int port, PortStatus portStatus)> CheckPort(IPAddress ipAddr, int port,
            int timeout = 3000)
        {
            try
            {
                using (var tcpClient = new TcpClient())
                {
                    var connectTask = await tcpClient.ConnectWithTimeoutAsync(ipAddr, port, timeout);
                    PortStatus portStatus;
                    switch (connectTask.Status)
                    {
                        case TaskStatus.RanToCompletion:
                            portStatus = PortStatus.OPEN;
                            break;
                        case TaskStatus.Faulted:
                            portStatus = PortStatus.CLOSED;
                            break;
                        default:
                            portStatus = PortStatus.FILTERED;
                            break;
                    }

                    return (port, portStatus);
                }
            }
            catch
            {
                await Task.Delay(3000);
                return await CheckPort(ipAddr, port);
            }
        }
    }
}