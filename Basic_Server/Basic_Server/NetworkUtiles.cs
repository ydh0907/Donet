using System.Net;
using System.Net.Sockets;

namespace MyFramework
{
    public class NetworkUtiles
    {
        public static IPAddress GetLocalIPAddress()
        {
            string hostName = Dns.GetHostName();
            return Dns.GetHostAddresses(hostName, AddressFamily.InterNetwork)[0];
        }
    }
}
