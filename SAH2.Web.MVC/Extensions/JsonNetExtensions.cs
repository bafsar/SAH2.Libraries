using Newtonsoft.Json;
using System.Web.Mvc;

namespace SAH2.Web.MVC.Extensions
{
    public static class JsonNetExtensions
    {
        public static ContentResult JsonNetContent(this object obj)
        {
            var content = new ContentResult
            {
                Content = JsonConvert.SerializeObject(obj),
                ContentType = "application/json",
                ContentEncoding = null
            };

            return content;
        }
    }
}
