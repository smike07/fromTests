using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using MvcApplication1.Controllers;

namespace MvcApplication1
{
    public class CustomRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new CustomHttpHandler();
        }
    }

    public class CustomHttpHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var hController = new HomeController();
            context.Response.Write(hController.ProccessUri(context.Request.Url.AbsoluteUri, context.Request.Url.Authority) );
        }
    }
}