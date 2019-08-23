using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SAH2.Web.MVC.Utilities.EmbeddedResourcePack;
using SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Controllers;

[assembly: PreApplicationStartMethod(typeof(RouteConfig), nameof(RouteConfig.RegisterRoutes))]

namespace SAH2.Web.MVC.Utilities.EmbeddedResourcePack
{
    public class RouteConfig
    {
        public static void RegisterRoutes()
        {
            var routes = RouteTable.Routes;

            var embeddedControllerNS = typeof(EmbeddedController).Namespace;

            routes.MapRoute(
                "EmbeddedResourceRoute_WithGet",
                "Embedded/Get/{assembly}/{resource}/{folders}",
                new
                {
                    controller = "Embedded",
                    action = "Get",
                    assembly = UrlParameter.Optional,
                    resource = UrlParameter.Optional,
                    folders = UrlParameter.Optional
                },
                new[] { embeddedControllerNS }
            );

            routes.MapRoute(
                "EmbeddedResourceRoute_WithoutGet",
                "Embedded/{assembly}/{resource}/{folders}",
                new
                {
                    controller = "Embedded",
                    action = "Get",
                    assembly = UrlParameter.Optional,
                    resource = UrlParameter.Optional,
                    folders = UrlParameter.Optional
                },
                new[] { embeddedControllerNS }
            );
        }
    }
}