using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CastleClub.BusinessLogic;
using CastleClub.DataTypes;

namespace Member.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            var siteActive = CastleClub.BusinessLogic.Managers.SitesManager.GetSiteActive();

            string page = string.Empty;

            foreach (string item in Request.QueryString.Keys)
            {
                page = string.IsNullOrEmpty(page) ? page + "?" : page + "&";
                page = page + item + "=" + Request.QueryString[item];
            }

            if (Request.QueryString["t"] != null)
            {
                page = "signup.aspx" + page;
            }
            else
            {
                page = "optin.aspx" + page;
            }

            //Response.Redirect("https://www." + siteActive.SignupDomain + "/" + page);
            Response.Redirect("http://castleclub/" + page);
            return View();
        }
    }
}
