using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PortScan
{
    public class AsyncUdpScanner
    {
        private byte[][] pakeges;

        public AsyncUdpScanner()
        {
            pakeges = new[] {Package.GetDns(), Package.GetNtp()}.Reverse().ToArray();
        }

        public async Task<IEnumerable<(int, Protocol)>> Scan(IPAddress ipAddr, int[] ports)
        {
            var partTasks = ports.Select(port => CheckPort(ipAddr, port)).ToArray();
            return (await Task.WhenAll(partTasks))
                .Where(x => x.portStatus == PortStatus.OPEN)
                .Select(x => (x.port, x.protocol));
        }

        private async Task<(int port, PortStatus portStatus, Protocol protocol)> CheckPort(IPAddress ipAddr,
            int port, int timeout = 3000)
        {
            while (true)
            {
                try
                {
                    using var udpClient = new UdpClient();
                    udpClient.Connect(ipAddr, port);

                    for (var i = 0; i < 2; i++)
                    {
                        try
                        {
                            await udpClient.SendAsync(pakeges[i], pakeges[i].Length);
                            var count = await udpClient.ReceiveWithTimeoutAsync(timeout);
                            if (count.Status == TaskStatus.RanToCompletion)
                            {
                                foreach (var v in count.Result.Buffer)
                                {
                                    Console.Write(v);
                                }
                                Console.WriteLine();
                                Console.WriteLine(port);
                                foreach (var v in pakeges[i])
                                {
                                    Console.Write(v);
                                }
                                
                                switch (i)
                                {
                                    case 0:
                                        return (port, PortStatus.OPEN, Protocol.DNS);

                                    case 1:
                                        return (port, PortStatus.OPEN, Protocol.NTP);
                                }
                            }
                           
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    return (port, PortStatus.CLOSED, Protocol.UNDEFINED);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}