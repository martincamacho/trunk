using CastleClub.BackEnd.Models;
using CastleClub.BusinessLogic.Managers;
using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Management;
using System.Configuration;

namespace CastleClub.BackEnd.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Default()
        {
            string drive = ConfigurationManager.AppSettings["Drive"];
            int limitInGB = int.Parse(ConfigurationManager.AppSettings["LimitInGB"]);
            //drive
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"" + drive + ":\"");
            disk.Get();
            //Get the size
            double size = double.Parse(disk["Size"].ToString());
            //convert the size from bytes to GB 
            size = Math.Round(size / 1000000000, 2);
            //Get the freeSpace
            double freeSpace = double.Parse(disk["FreeSpace"].ToString());
            //convert the size from bytes to GB 
            freeSpace = Math.Round(freeSpace / 1000000000, 2);
            if (limitInGB >= freeSpace)
            {
                ViewBag.AlertDiskSpace = true;
            }
            ViewBag.FreeDiskSpace = freeSpace;
            ViewBag.DiskSpace = size;

            return View();
        }

        [HttpPost]
        public ActionResult GetReferrersInfo()
        {
            UserDT user = UsersManager.GetUserByAspNetId(User.Identity.GetUserId());

            List<ReferrerInfoDT> sitesInfo = SitesManager.GetReferrersInfo();
            ViewBag.SitesInfo = sitesInfo;

            List<CustomerVM> model = new List<CustomerVM>();
            var todayDate = DateTime.Now.Date;
            foreach (var customer in ReferrersManagers.GetCustomers(0, true).Where(x => x.CreatedAt >= todayDate))
            {
                string type = string.Empty;
                CustomersManager.GetLastFourDigitCreditCards(customer.Id, out type);

                model.Add(CustomerVM.TransformFromCustomerDT(customer, type));
            }

            return View(model);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        public ActionResult SitesInfo()
        {
            return View(new SiteInfoFilterVM());
        }

        [Authorize(Roles = "Super Admin, Admin")]
        [HttpPost]
        public ActionResult SitesInfo(SiteInfoFilterVM model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception();
            }

            model.SitesInfo = SitesManager.GetSitesInfo(model.From, model.To);

            return View(model);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        public ActionResult MagnamentActiveSite()
        {
            var sites = SitesManager.GetSites();
            var model = new CastleClub.BackEnd.Models.MagnementActivesSitesVM()
            {
                MasterEmailFormProcentage = sites[0].MasterCardEmailFormPercentage,
                VisaEmailFormPorcentage = sites[0].VisaEmailFormPercentage,
                DiscoverEmailFormPorcentage = sites[0].DiscoverEmailFormPercentage
            };
            model.Sites = new List<MagnementActiveSiteVM>();
            foreach (var site in sites.OrderBy(x => x.Id))
            {
                var siteRepeat = model.Sites.FirstOrDefault(x => x.Site.Name == site.Name);
                if (siteRepeat != null)
                {
                    model.Sites.Remove(siteRepeat);
                }
                model.Sites.Add(new CastleClub.BackEnd.Models.MagnementActiveSiteVM()
                {
                    Active = site.Active,
                    SiteId = site.Id,
                    Site = site,
                    Url = (site.OfferDomain.Contains("https://www.")) ?
                            site.SignupDomain
                            : "https://www." + site.SignupDomain.Replace("www", string.Empty).Replace("http://", string.Empty).Replace("https://", string.Empty) + "/optin.aspx"
                });
            }

            model.Sites = model.Sites.OrderBy(x => x.Site.Name).ToList();

            return View(model);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        [HttpPost]
        public bool MagnamentActiveSite(CastleClub.BackEnd.Models.MagnementActivesSitesVM model)
        {
            if (!ModelState.IsValid)
            {
                return false;
            }

            if (model.SiteID > 0)
            {
                return SitesManager.SetActiveSite(model.SiteID);
            }

            return false;
        }

        [Authorize(Roles = "Super Admin, Admin")]
        [HttpPost]
        public bool ChangePercentageCreditCard(Models.CreditCardPercentageVM model)
        {
            if (ModelState.IsValid && (model.EmailForm + model.FullForm == 100))
            {
                CastleClub.DataTypes.Enums.CCType creditCardType = (CastleClub.DataTypes.Enums.CCType)Enum.Parse(typeof(CastleClub.DataTypes.Enums.CCType), model.Type.ToUpper());

                try
                {
                    CastleClub.BusinessLogic.Managers.SitesManager.ChangeCreditCardPercentageAllSites(creditCardType, model.EmailForm, model.FullForm);
                    return true;
                }
                catch (Exception) { }
            }

            return false;
        }


        [Authorize(Roles = "Super Admin, Admin")]
        public ActionResult GetDiskSpaceInfo()
        {
            return View(new SiteInfoFilterVM());
        }
    }
}