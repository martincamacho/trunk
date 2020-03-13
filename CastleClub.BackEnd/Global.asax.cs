using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using CastleClub.DataTypes;
using CastleClub.BusinessLogic.Managers;
using System.Web.Configuration;
using System.Diagnostics;
using System.Configuration;
using Elmah;

namespace CastleClub.BackEnd
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalFilters.Filters.Add(new CustomerFilter(), 1);
            GlobalFilters.Filters.Add(new HandleErrorAttribute()
                {
                    View = "Error"
                }, 2);

            LoggingUtilities.Logger.LoggingEnabled = true;
            LoggingUtilities.Logger.AddFileLog(ConfigurationManager.AppSettings["LogFilePath"], 1);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError();

            Exception ex = new Exception("BACKEND: " + exc.Message, exc.InnerException);

            Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(ex));

            Response.Redirect("/Error/Index");
        }

       
    }

    public class CustomerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userIdAsp = filterContext.HttpContext.User.Identity.GetUserId();
            if (userIdAsp != null)
            {
                filterContext.Controller.ViewBag.User = UsersManager.GetUserByAspNetId(userIdAsp);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
