using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCShoppingCart
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Cart",
                url: "Cart/{action}/{id}",
                defaults: new { controller = "Cart", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MVCShoppingCart.Controllers" }
                );

            routes.MapRoute(
                name: "Shop",
                url: "Shop/{action}/{slug}",
                defaults: new { controller = "Shop", action = "Index", slug = UrlParameter.Optional },
                namespaces: new[] { "MVCShoppingCart.Controllers" }
                );

            routes.MapRoute(
                name: "SidebarPartial",
                url: "Pages/SidebarPartial",
                defaults: new { controller = "Pages", action = "SidebarPartial" },
                namespaces: new[] { "MVCShoppingCart.Controllers" }
                );

            routes.MapRoute(
                name: "PagesMenuPartial",
                url: "Pages/PagesMenuPartial",
                defaults: new { controller = "Pages", action = "PagesMenuPartial" },
                namespaces: new[] { "MVCShoppingCart.Controllers" }
                );

            routes.MapRoute(
                name: "Pages",
                url: "{slug}",
                defaults: new { controller = "Pages", action = "Index" },
                namespaces: new[] { "MVCShoppingCart.Controllers" }
                );

            routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new { controller = "Pages", action = "Index" },
                namespaces: new[] { "MVCShoppingCart.Controllers" }
                );

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
