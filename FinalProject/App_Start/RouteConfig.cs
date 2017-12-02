using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FinalProject
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                //Here I changed the controller from "Home" to "tickets". Eventually we can probably
                //just delete the Home controller and the files associated with it but we should not do that
                //yet. It could have some unknown consequences. As an example of how this works, if you change
                // 'controler = "tickets"' to 'controller = "Home"' you will see the program behave differently
                //becaues different controllers, and therefore views, are being called.
                defaults: new { controller = "tickets", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
