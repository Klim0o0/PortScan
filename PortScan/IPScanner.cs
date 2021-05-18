using System.Net;
using System.Threading.Tasks;

namespace PortScan
{
    public interface IPScanner
    {
        Task<int[]> Scan(IPAddress ipAdrr, int[] ports);
    }
}