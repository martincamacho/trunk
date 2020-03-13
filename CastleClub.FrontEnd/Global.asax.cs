using CastleClub.BusinessLogic.Managers;
using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CastleClub.FrontEnd
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalFilters.Filters.Add(new CustomFilter(),1);
            GlobalFilters.Filters.Add(new HandleErrorAttribute()
                {
                    View="Error"
                }, 2);

            LoggingUtilities.Logger.LoggingEnabled = true;
            LoggingUtilities.Logger.AddFileLog(ConfigurationManager.AppSettings["LogPath"], 1);
        }

        public class CustomFilter: ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                SiteDT site = SitesManager.GetSiteFromHostname(filterContext.HttpContext.Request.Url.Host);
                filterContext.Controller.ViewBag.Site = site;

                if (filterContext.HttpContext.Request.QueryString.AllKeys.Contains("s"))
                    filterContext.Controller.ViewBag.Referrer = ReferrersManagers.GetReferrer(filterContext.HttpContext.Request.QueryString["s"]).Name;
                else
                {
                    filterContext.Controller.ViewBag.Referrer = "PartsGeek.com";
                }

                base.OnActionExecuting(filterContext);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError();

            string msg = "Url: " + Request.Url.ToString() + "\n";

            if (Request.Form.HasKeys())
            {
                string postVariables = "";
                foreach (string key in Request.Form.AllKeys)
                {
                    postVariables += "[" + key + "] = " + Request.Form[key] + " | ";
                }
                postVariables = postVariables.Substring(0, postVariables.Length - 3);
                msg += "Post Variables: " + postVariables + "\n";
            }

            if (!string.IsNullOrEmpty(Request.QueryString.ToString()))
            {
                msg += "Query String: " + Request.QueryString + "\n";
            }

            string headers = "";
            foreach (string key in Request.Headers.AllKeys)
            {
                headers += "[" + key + "] = " + Request.Headers[key] + " | ";
            }
            headers = headers.Substring(0, headers.Length - 3);
            msg += "Headers:" + headers;

            //CastleClub.BusinessLogic.Utils.EventViewer.Writte("CastleClub", "CastleClubFrontend", msg+"\n"+exc.ToString(), System.Diagnostics.EventLogEntryType.Error);

            Exception ex = new Exception("FRONTEND: " + exc.Message , exc.InnerException);

            Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(ex));
        }
    }
}
