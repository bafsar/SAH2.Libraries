using System;
using System.Web;

#if (NET472 || NET48)
namespace SAH2.Web.Functions
{

    public static class CookieFunctions
    {
        internal static HttpCookie WriteCookie(string name, string value, uint expiresDays,
            HttpResponseBase responseBase = null, HttpResponse response = null)
        {
            var c = HttpContext.Current.Request.Cookies[name];
            if (c != null && c.Expires >= DateTime.Now) return c;

            c = new HttpCookie(name, value) { Expires = DateTime.Now.AddDays(expiresDays) };

            if (responseBase != null)
                responseBase.Cookies.Set(c);
            else if (response != null)
                response.Cookies.Set(c);
            else
                HttpContext.Current.Response.Cookies.Set(c);
            return c;
        }

        public static HttpCookie WriteCookie(string name, string value, uint expiresDays = 90)
        {
            return WriteCookie(name, value, expiresDays, null);
        }


        internal static string ReadCookie(string name, HttpRequestBase requestBase = null, HttpRequest request = null)
        {
            HttpCookie c;
            if (requestBase != null)
                c = requestBase.Cookies[name];
            else if (request != null)
                c = request.Cookies[name];
            else
                c = HttpContext.Current.Request.Cookies[name];
            return c?.Value;
        }

        public static string ReadCookie(this HttpRequest request, string name)
        {
            return ReadCookie(name, request: request);
        }


        internal static void RemoveCookie(string name, HttpResponseBase responseBase = null,
            HttpResponse response = null)
        {
            var c = HttpContext.Current.Request.Cookies[name];
            if (c == null) return;

            c.Expires = DateTime.Now.AddYears(-1);

            if (responseBase != null)
                responseBase.Cookies.Set(c);
            else if (response != null)
                response.Cookies.Set(c);
            else
                HttpContext.Current.Response.Cookies.Set(c);
        }

        public static void RemoveCookie(string name)
        {
            RemoveCookie(name, null);
        }
    }
}
#endif
