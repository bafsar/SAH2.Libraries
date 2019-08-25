using System;
using System.Web.Mvc;
using SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Base;

namespace Test.EmbeddedResourcePack
{
    public class jQuery : CustomEmbeddedBundle<jQuery>
    {
        public static MvcHtmlString JsOnLocal => AutoRender("Test.EmbeddedResourcePack/libs/js/jquery-3.4.1.min.js");
        public static MvcHtmlString JsOnCDN => AutoRender("//code.jquery.com/jquery-3.4.1.min.js");
    }
}
