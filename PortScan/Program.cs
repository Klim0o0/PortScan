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

        [Option('p', "ports", Min = 2, Max = 2, Required = false,Default = new[] {0, 65535}, HelpText = "Port range.")]
        public IEnumerable<int> Ports { get; set; } 
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var address = "";
            var tcp = false;
            var udp = false;
            var portsRange = new[] {0, 65535};
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                address = o.Address;
                portsRange = o.Ports.ToArray();
                tcp = o.Tcp;
                udp = o.Udp;
            });

            byte[] ip;
            try
            {
                ip = address.Split('.').Select(byte.Parse).ToArray();
            }
            catch
            {
                Console.WriteLine("Wrong args. Expected ipV4 xxx.xxx.xxx.xxx");
                return;
            }

            var ports = Enumerable.Range(portsRange[0], portsRange[1]).ToArray();
            if (tcp)
            {
                var scanner = new AsyncTcpScanner();
                var openPorts = await scanner.Scan(new IPAddress(ip), ports);
                foreach (var openPort in openPorts)
                {
                    Console.WriteLine("TCP:" + openPort);
                }
            }
        }
    }
}