using CastleClub.BackEnd.Models;
using CastleClub.BusinessLogic.Managers;
using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Web.Configuration;
using System.Configuration;
using System.IO;
using System.Reflection;
using CastleClub.BackEnd.Models.Report;
using System.Globalization;
using CastleClub.BusinessLogic.Data;
using CastleClub.BusinessLogic.Utils;

namespace CastleClub.BackEnd.Controllers
{
    [Authorize(Roles = "Super Admin, Admin, Customer Service")]    
    public class SitesController : Controller
    {
        // GET: Sites
        public ActionResult WelcomeEmail()
        {
            var model = new WelcomeEmailVM();
            model.sites = SitesManager.GetSites();
            model.CapEmail = GlobalParameters.CapEmail;
            return View(model);
        }

        // GET: Sites
       
        [HttpPost]
        public ActionResult WelcomeEmail(CastleClub.BackEnd.Models.WelcomeEmailVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //bool sel = model.sites[0].SendWelcomeEmail;
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                foreach (var item in model.sites)
                {
                
                    var site = entities.Sites.Where(x => x.Id == item.Id).FirstOrDefault();
                    site.SendWelcomeEmail = item.SendWelcomeEmail;
                    site.WelcomeEmailDelay = item.WelcomeEmailDelay;
                    site.Email = item.Email;
                    site.SmtpAddress = item.SmtpAddress;
                    if (!string.IsNullOrEmpty(item.PasswordEmail))
                    {
                        site.PasswordEmail = item.PasswordEmail;
                    }

                }

                var parameter = entities.Parameters.FirstOrDefault();
                parameter.CapEmail = model.CapEmail;
                entities.SaveChanges();
            }
           
            return View(model);
        }

        public ActionResult PreviewEmail(int ID)
        {
            var model = new PreviewEmailVM();
            //int ID = 14;
            SiteDT siteDT = SitesManager.GetSite(ID);
            string name = "Alexander Broderick";
            string email = "email@email.com";
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
            model.HtmlEmail = string.Format(Email.WelcomeBodyEmail(), name,
                                                    email, result, siteDT.Name, siteDT.Name, "PartsGeek",
                                                    siteDT.Price, siteDT.PricePerQuarter, siteDT.Phone, siteDT.Email, siteDT.OfferDomain);
            return View(model);
        }


        
    }
}