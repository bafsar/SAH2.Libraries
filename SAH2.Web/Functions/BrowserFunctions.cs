using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;

#if (NET472 || NET48)
namespace SAH2.Web.Functions
{
    public static class BrowserFunctions
    {
        public static string GetIPAddress()
        {
            return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        public static string GetClientPageName()
        {
            return HttpContext.Current.Request.ServerVariables["PATH_INFO"];
        }

        public static string GetClientReferrerPageName()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
        }

        public static string GetBrowserName()
        {
            return HttpContext.Current.Request.Browser.Browser;
        }

        public static string GetBrowserVersion()
        {
            return HttpContext.Current.Request.Browser.Version;
        }

        public static string GetClientLanguage()
        {
            return HttpContext.Current.Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"];
        }

        public static bool IsRequestFromLocalhost()
        {
            return IsFromLocalhost(GetIPAddress());
        }

        internal static bool IsFromLocalhost(string remoteAddress)
        {
            // if unknown, assume not local
            if (string.IsNullOrEmpty(remoteAddress))
                return false;

            // check if localhost
            if (remoteAddress == "127.0.0.1" || remoteAddress == "::1")
                return true;

            // compare with local address
            if (remoteAddress == GetLocalAddress() || remoteAddress == GetLocalAddress(IPProtocol.IPv6))
                return true;

            return false;
        }

        private enum IPProtocol
        {
            IPv4,
            IPv6
        }

        // https://stackoverflow.com/a/50706466/2374053
        private static string GetLocalAddress(IPProtocol ipProtocol = IPProtocol.IPv4)
        {
            var firstAddress = (from address in NetworkInterface.GetAllNetworkInterfaces().Select(x => x.GetIPProperties()).SelectMany(x => x.UnicastAddresses).Select(x => x.Address)
                                where !IPAddress.IsLoopback(address) && address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                                select address).FirstOrDefault();

            return (ipProtocol == IPProtocol.IPv4)
                ? firstAddress?.MapToIPv4().ToString()
                : firstAddress?.MapToIPv6().ToString();
        }
    }
}
#endif
