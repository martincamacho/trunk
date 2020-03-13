using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Member
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error()
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

            CastleClub.BusinessLogic.Utils.EventViewer.Writte("CastleClub", "Member", msg+"\n"+exc.ToString(), System.Diagnostics.EventLogEntryType.Error);
        }
    }
}