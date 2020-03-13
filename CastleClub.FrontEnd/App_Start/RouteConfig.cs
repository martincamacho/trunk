using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CastleClub.FrontEnd
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "FullForm",
                url: "optin.aspx",
                defaults: new { controller = "Home", action = "FullForm" }
            );

            routes.MapRoute(
                name: "EmailForm",
                url: "signup.aspx",
                defaults: new { controller = "Home", action = "EmailForm" }
            );

            routes.MapRoute(
                name: "Css",
                url: "style.css",
                defaults: new { controller = "Home", action = "Css" }
            );

            routes.MapRoute(
                name: "Welcome",
                url: "welcome",
                defaults: new { controller = "Home", action = "Welcome" }
            );

            routes.MapRoute(
                name: "TermsOfMembership",
                url: "termsofmembership",
                defaults: new { controller = "Home", action = "TermsOfMembership" }
            );

            routes.MapRoute(
                name: "SalesPrivacy",
                url: "salesprivacy",
                defaults: new { controller = "Home", action = "SalesPrivacy" }
            );

            routes.MapRoute(
                name: "SalesClaim",
                url: "salesclaim",
                defaults: new { controller = "Home", action = "SalesClaim" }
            );

            routes.MapRoute(
                name: "SalesTerms",
                url: "salesterms",
                defaults: new { controller = "Home", action = "SalesTerms" }
            );

            
        }
    }
}
