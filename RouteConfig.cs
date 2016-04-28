using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcApplication1
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Add(new Route("{controller}/{*catchcall}", new CustomRouteHandler()));
            routes.MapRoute("Default",
                      url: "",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}