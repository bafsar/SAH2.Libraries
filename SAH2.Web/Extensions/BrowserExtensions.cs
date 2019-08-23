#if (NET472 || NET48)

using System.Web;

namespace SAH2.Web.Extensions
{
    public static class BrowserExtensions
    {
        public static string GetIPAddress(this HttpRequestBase requestBase)
        {
            return requestBase.ServerVariables["REMOTE_ADDR"];
        }

        public static string GetClientPageName(this HttpRequestBase requestBase)
        {
            return requestBase.ServerVariables["PATH_INFO"];
        }

        public static string GetClientReferrerPageName(this HttpRequestBase requestBase)
        {
            return requestBase.ServerVariables["HTTP_REFERER"];
        }

        public static string GetBrowserName(this HttpRequestBase requestBase)
        {
            return requestBase.Browser.Browser;
        }

        public static string GetBrowserVersion(this HttpRequestBase requestBase)
        {
            return requestBase.Browser.Version;
        }

        public static string GetClientLanguage(this HttpRequestBase requestBase)
        {
            return requestBase.ServerVariables["HTTP_ACCEPT_LANGUAGE"];
        }

        public static bool IsFromLocalhost(this HttpRequestBase request)
        {
            return Functions.BrowserFunctions.IsFromLocalhost(request.GetIPAddress());
        }


        public static string GetIPAddress(this HttpRequest request)
        {
            return request.ServerVariables["REMOTE_ADDR"];
        }

        public static string GetClientPageName(this HttpRequest request)
        {
            return request.ServerVariables["PATH_INFO"];
        }

        public static string GetClientReferrerPageName(this HttpRequest request)
        {
            return request.ServerVariables["HTTP_REFERER"];
        }

        public static string GetBrowserName(this HttpRequest request)
        {
            return request.Browser.Browser;
        }

        public static string GetBrowserVersion(this HttpRequest request)
        {
            return request.Browser.Version;
        }

        public static string GetClientLanguage(this HttpRequest request)
        {
            return request.ServerVariables["HTTP_ACCEPT_LANGUAGE"];
        }

        public static bool IsFromLocalhost(this HttpRequest request)
        {
            return Functions.BrowserFunctions.IsFromLocalhost(request.GetIPAddress());
        }
    }
}
#endif