using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PortScan
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 1)
            {
                byte[] ip;
                try
                {
                    ip = args[0].Split('.').Select(x => byte.Parse(x)).ToArray();
                }
                catch
                {
                    Console.WriteLine("Wrong args. Expected ipV4 xxx.xxx.xxx.xxx");
                    return;
                }

                var ports = Enumerable.Range(1, 65535).ToArray();

                var scanner = new AsyncTcpScanner();
                var openPorts = await scanner.Scan(new IPAddress(ip), ports);
                foreach (var openPort in openPorts)
                {
                    Console.WriteLine(openPort);
                }
            }
            else
            {
                Console.WriteLine("Wrong args. Expected ipV4 xxx.xxx.xxx.xxx");
            }
        }
    }
}