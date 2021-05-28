using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CommandLine;

namespace PortScan
{
    class Options
    {
        [Option('a', "address", Required = true, HelpText = "Set ipAddress to scan.")]
        public string Address { get; set; }

        [Option('t', "tcp", Required = false, HelpText = "Tcp scan.")]
        public bool Tcp { get; set; }

        [Option('u', "udp", Required = false, HelpText = "Udp scan.")]
        public bool Udp { get; set; }

        [Option('p', "ports", Min = 2, Max = 2, Required = false, Default = new[] {1, 65535}, HelpText = "Port range.")]
        public IEnumerable<int> Ports { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var address = "";
            var tcp = false;
            var udp = false;
            var portsRange = new[] {1, 65535};
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                address = o.Address;
                portsRange = o.Ports.ToArray();
                tcp = o.Tcp;
                udp = o.Udp;
            });
            var addresses = await Dns.GetHostAddressesAsync(address);
            if (addresses.Length == 0)
            {
                Console.WriteLine("Wrong hostName");
                return;
            }

            Console.WriteLine(addresses[0]);


            var ports = Enumerable.Range(portsRange[0], portsRange[1]).ToArray();
            if (tcp)
            {
                var scanner = new AsyncTcpScanner();
                var openPorts = await scanner.Scan(addresses[0], ports);
                foreach (var openPort in openPorts)
                {
                    Console.WriteLine("TCP: " + openPort.Item1 + " " +
                                      (openPort.Item2 == Protocol.UNDEFINED ? "" : openPort.Item2));
                }
            }

            if (udp)
            {
                var scanner = new AsyncUdpScanner();
                var openPorts = await scanner.Scan(addresses[0], ports);
                foreach (var openPort in openPorts)
                {
                    Console.WriteLine("UDP: " + openPort.Item1 + " " +
                                      (openPort.Item2 == Protocol.UNDEFINED ? "" : openPort.Item2));
                }
            }
        }
    }
}