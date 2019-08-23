#if (NET472 || NET48)

using System.Web;
using SAH2.Web.Functions;

namespace SAH2.Web.Extensions
{
    public static class CookieExtensions
    {
        public static void WriteCookie(this HttpResponseBase responseBase, string name, string value,
            uint expiresDays = 90)
        {
            CookieFunctions.WriteCookie(name, value, expiresDays, responseBase);
        }

        public static void WriteCookie(this HttpResponse response, string name, string value, uint expiresDays = 90)
        {
            CookieFunctions.WriteCookie(name, value, expiresDays, response: response);
        }


        public static string ReadCookie(string name)
        {
            return CookieFunctions.ReadCookie(name);
        }

        public static string ReadCookie(this HttpRequestBase requestBase, string name)
        {
            return CookieFunctions.ReadCookie(name, requestBase);
        }


        public static void RemoveCookie(this HttpResponseBase responseBase, string name)
        {
            CookieFunctions.RemoveCookie(name, responseBase);
        }

        public static void RemoveCookie(this HttpResponse response, string name)
        {
            CookieFunctions.RemoveCookie(name, response: response);
        }
    }
}
#endif