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
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using CastleClub.BusinessLogic.Data;

namespace CastleClub.BackEnd.Controllers
{
    //  [Authorize(Roles = "Super Admin, Admin, Customer Service")]
    public class CustomersController : Controller
    {
        //
        // GET: /Customers/
        public ActionResult Index()
        {
            ViewBag.CustomersCount = CustomersManager.GetCustomersCount().ToString();
            ViewBag.SitesName = SitesManager.GetSites().GroupBy(x => x.Name).Select(x => x.Key);

            return View();
        }

        [HttpPost]
        public ActionResult GetListCustomersFilter(CustomerFilterVM filter)
        {
            if (ModelState.IsValid)
            {
                filter.Letter = filter.Letter == "%" ? string.Empty : filter.Letter;
                int totalPage = 0;
                int pageSize = CastleClub.BusinessLogic.Data.GlobalParameters.PageSize;
                List<CustomerDT> customersDT = CustomersManager.GetFilterCustomers(filter.Letter, filter.Page, pageSize, filter.Word, filter.All, filter.OnlyActive, filter.SiteName, out totalPage);

                ViewBag.PageCount = totalPage / pageSize + ((totalPage % pageSize) == 0 ? 0 : 1);
                ViewBag.CurrentPage = filter.All ? 0 : filter.Page;
                customersDT = customersDT != null ? customersDT : new List<CustomerDT>();
                return View(customersDT);
            }

            return View(new List<CustomerDT>());
        }

        [HttpPost]
        public bool EnabledDisabledCustomer(CustomerEnabled model)
        {
            if (ModelState.IsValid && model.IdCustomer > 0)
            {
                bool correctProcess = false;
                try
                {
                    CustomersManager.ActivateCustomer(model.IdCustomer);
                    correctProcess = true;
                }
                catch { }

                return correctProcess;
            }

            return false;
        }

        [HttpPost]
        public ActionResult ExportToXslx(CustomerFilterVM filter)
        {
            if (ModelState.IsValid)
            {
                filter.Letter = filter.Letter == "%" ? string.Empty : filter.Letter;
                int pageSize = CastleClub.BusinessLogic.Data.GlobalParameters.PageSize;
                try
                {
                    string path = CustomersManager.GetFilterCustomersToExcel(filter.Letter, filter.Page, pageSize, filter.Word, filter.All, filter.OnlyActive, filter.SiteName);

                    return Json(new { success = true, fName = path }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        public FileResult ExportToXslx(string fName)
        {
            if (!System.IO.File.Exists(Server.MapPath("/tmp/" + fName)))
            {
                throw new Exception("The file not exists.");
            }
            return File(System.IO.File.ReadAllBytes(Server.MapPath("/tmp/" + fName)), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fName);
        }

        private static Stream GetResourceFileStream(string fileName)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            // Get all embedded resources
            var arrResources = currentAssembly.GetManifestResourceNames();

            return (from resourceName in arrResources where resourceName.Contains(fileName) select currentAssembly.GetManifestResourceStream(resourceName)).FirstOrDefault();
        }

        [Authorize(Roles = "Super Admin, Admin")]
        public ActionResult ActiveReport()
        {
            var model = new ActiveReportVM();

            var sites = SitesManager.GetSites();
            foreach (var site in sites)
            {
                var siteVm = new SiteVM { Id = site.Id, Name = site.Name, ProcessCsv = false };
                model.sites.Add(siteVm);
            }

            return View(model);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        [HttpPost]
        public ActionResult ActiveReport(ActiveReportVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var today = DateTime.Today;
            if (model.AccessList)
            {

                var bytes = SitesManager.GetAccessCustomersCsvBytes(model.sites.Where(x => x.ProcessCsv).Select(x => x.Id).ToList());
                var name = today.ToString("yyyy-MM-dd") + "_access-format.csv";
                return File(bytes, "text/csv", name);
            }
            else
            {
                var bytes = SitesManager.GetActiveUsersAccessListFile(model.sites.Where(x => x.ProcessCsv == true).Select(x => x.Id).ToList());
                var name = today.ToString("yyyy-MM-dd") + "_simple-format.csv";
                return File(bytes, "text/csv", name);
            }
        }

        [Authorize(Roles = "Super Admin, Admin")]
        public ActionResult MembershipAgeCount()
        {
            MembershipAgeCountVM model = new MembershipAgeCountVM();
            return View(model);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        [HttpPost]
        public ActionResult MembershipAgeCount(MembershipAgeCountVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Show = true;

            DateTime today = DateTime.Now.Date;
            DateTime ThreeMonth = today.AddMonths(-3);
            DateTime SixMonth = today.AddMonths(-6);
            DateTime OneYear = today.AddYears(-1);
            DateTime TwoYears = today.AddYears(-2);
            DateTime ThreeYears = today.AddYears(-3);

            int countThreeMonthActive = 0;
            int countThreeMonthUnactive = 0;
            int countSixMonthActive = 0;
            int countSixMonthUnactive = 0;
            int countOneYearsActive = 0;
            int countOneYearsUnactive = 0;
            int countTwoYearsActive = 0;
            int countTwoYearsUnactive = 0;
            int countThreeYearsActive = 0;
            int countThreeYearsUnactive = 0;
            int countOldActive = 0;
            int countOldUnactive = 0;
            int total = 0;

            CastleClub.BusinessLogic.Managers.CustomersManager.MembershipAgeCountRange(ThreeMonth, today, model.SiteID, out countThreeMonthActive, out countThreeMonthUnactive, out total);
            CastleClub.BusinessLogic.Managers.CustomersManager.MembershipAgeCountRange(SixMonth, ThreeMonth, model.SiteID, out countSixMonthActive, out countSixMonthUnactive, out total);
            CastleClub.BusinessLogic.Managers.CustomersManager.MembershipAgeCountRange(OneYear, SixMonth, model.SiteID, out countOneYearsActive, out countOneYearsUnactive, out total);
            CastleClub.BusinessLogic.Managers.CustomersManager.MembershipAgeCountRange(TwoYears, OneYear, model.SiteID, out countTwoYearsActive, out countTwoYearsUnactive, out total);
            CastleClub.BusinessLogic.Managers.CustomersManager.MembershipAgeCountRange(ThreeYears, TwoYears, model.SiteID, out countThreeYearsActive, out countThreeYearsUnactive, out total);
            CastleClub.BusinessLogic.Managers.CustomersManager.MembershipAgeCountRange(new DateTime(2000, 1, 1), ThreeYears, model.SiteID, out countOldActive, out countOldUnactive, out total);

            model.Range_0_to_3_Months_Count_Active = countThreeMonthActive;
            model.Range_0_to_3_Months_Count_Unactive = countThreeMonthUnactive;
            model.Range_3_to_6_Months_Count_Active = countSixMonthActive;
            model.Range_3_to_6_Months_Count_Unactive = countSixMonthUnactive;
            model.Range_6_to_12_Months_Count_Active = countOneYearsActive;
            model.Range_6_to_12_Months_Count_Unactive = countOneYearsUnactive;
            model.Range_1_to_2_Years_Count_Active = countTwoYearsActive;
            model.Range_1_to_2_Years_Count_Unactive = countTwoYearsUnactive;
            model.Range_2_to_3_Years_Count_Active = countThreeYearsActive;
            model.Range_2_to_3_Years_Count_Unactive = countThreeYearsUnactive;
            model.Range_Old_Count_Active = countOldActive;
            model.Range_Old_Count_Unactive = countOldUnactive;

            model.Total = total;

            return View(model);
        }

        public ActionResult Debtors()
        {
            ViewBag.CustomersCount = CustomersManager.GetDebtorsCount().ToString();
            ViewBag.SitesName = SitesManager.GetSites().GroupBy(x => x.Name).Select(x => x.Key);

            return View();
        }



        public ActionResult GetListDebtorCustomersFilter(CustomerFilterVM filter)
        {
            if (ModelState.IsValid)
            {
                filter.Letter = filter.Letter == "%" ? string.Empty : filter.Letter;
                int totalPage = 0;
                int pageSize = CastleClub.BusinessLogic.Data.GlobalParameters.PageSize;
                List<CustomerDT> customersDT = CustomersManager.GetDebtorsCustomers(filter.Letter, filter.Page, pageSize, filter.Word, filter.All, filter.OnlyActive, filter.SiteName, out totalPage);

                ViewBag.PageCount = totalPage / pageSize + ((totalPage % pageSize) == 0 ? 0 : 1);
                ViewBag.CurrentPage = filter.All ? 0 : filter.Page;
                customersDT = customersDT != null ? customersDT : new List<CustomerDT>();
                return View(customersDT);
            }

            return View(new List<CustomerDT>());
        }

        public ActionResult DebtorsCount()
        {
            ViewBag.CustomersCount = CustomersManager.GetDebtorsCount().ToString();
            ViewBag.SitesName = SitesManager.GetSites().GroupBy(x => x.Name).Select(x => x.Key);

            return View();
        }

        /**/
        public ActionResult Access()
        {
            AccessVM model = new AccessVM();
            return View(model);
        }


        [HttpPost]

        public ActionResult Access(AccessVM model)
        {


            model.response = CustomersManager.NewActivateMember(SitesManager.GetSite(13), CustomersManager.GetCustomer(2239433), "OPEN");
            //////responseFromServer;
            /////SHA1 sha1 = SHA1Managed.Create();

            ////ASCIIEncoding encoding = new ASCIIEncoding();
            ////byte[] stream = null;
            ////StringBuilder sb = new StringBuilder();
            ////stream = sha1.ComputeHash(encoding.GetBytes("WEj5g-101036-2239433"));
            ////for (int i = 0; i < stream.Length; i++) 
            ////    sb.AppendFormat("{0:x2}", stream[i]);
            ////string cvt = sb.ToString();


            ////System.Diagnostics.Process.Start("https://foodvalueshop.enjoymydeals.com/home?CVT=" + cvt);

            return View(model);
        }
        /**/

        public string CancelDebtorsCustomers()
        {
            //ViewBag.CustomersCount = CustomersManager.GetDebtorsCount().ToString();
            //ViewBag.SitesName = SitesManager.GetSites().GroupBy(x => x.Name).Select(x => x.Key);

            //return DateTime.Now.ToLongDateString();
            return CustomersManager.CancelDebtorsCustomers().ToString();
        }

        public string RemoveEmailInAccess()
        {
            //ViewBag.CustomersCount = CustomersManager.GetDebtorsCount().ToString();
            //ViewBag.SitesName = SitesManager.GetSites().GroupBy(x => x.Name).Select(x => x.Key);

            //return DateTime.Now.ToLongDateString();


            return CustomersManager.RemoveEmailInAccess().ToString();
        }
        public ActionResult ActivateAllActiveCustomers()
        {
            AccessVM model = new AccessVM();
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                //SiteDT site = entities.Sites.Where(s=> s.Active == true).ToList().Select(s => s.GetDT()).FirstOrDefault();
                //var today = DateTime.Today.AddDays(-2);
                SiteDT site = SitesManager.GetSite(3);
               // List<CustomerDT> customers = entities.Customers.Where(c => c.StatusId == "ACTIVE" && c.SiteId == site.Id).ToList().Select(c => c.GetDT(false)).ToList();
                List<CustomerDT> customers = entities.Customers.Where(c => c.SiteId == site.Id).ToList().Select(c => c.GetDT(false)).ToList();              
                //List<CustomerDT> customers = entities.Customers.ToList().Select(c => c.GetDT(false)).ToList();             
                /*List<int> idList = new List<int>()
                {
                };*/
                string customerAutoPriceDiscount = "";
                foreach (CustomerDT custom in customers)
                {                  
                    try
                    {
                        /*if (idList.Contains(custom.Id) && custom.Status == DataTypes.Enums.CustomerStatus.ACTIVE)
                        {
                              //string response = CustomersManager.NewActivateMember(SitesManager.GetSite(custom.SiteId), custom, "CLOSED");
                              string response = CustomersManager.NewActivateMember(SitesManager.GetSite(custom.SiteId), custom, "OPEN");
                        }
                        else 
                        if (idList.Contains(custom.Id) && custom.Status != DataTypes.Enums.CustomerStatus.ACTIVE)
                        {
                            string response = CustomersManager.NewActivateMember(SitesManager.GetSite(custom.SiteId), custom, "SUSPEND");
                            customerAutoPriceDiscount +=  custom.Id.ToString() + " \n ,";                           
                        }             
                        */
                    }
                    catch (Exception e)
                    {
                    }

                    // RootObject oMyclass = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(response);

                }
                return View(model);
            }

        }
    }
    public class RootObject
    {
        [JsonProperty("data")]
        public List<item> item { get; set; }
    }
    public class item
    {
        [JsonProperty("id")]
        public long id;
        [JsonProperty("created_at")]
        public Nullable<DateTime> created_at;
        [JsonProperty("status")]
        public string status;
        [JsonProperty("valid_members_count")]
        public Nullable<long> valid_members_count;
        [JsonProperty("invalid_members_count")]
        public Nullable<int> invalid_members_count;
        [JsonProperty("imported_at")]
        public Nullable<DateTime> imported_at;
        [JsonProperty("links")]
        public Links links;
    }

    public class Links
    {
        [JsonProperty("show_import")]
        public string show_import;
        [JsonProperty("valid_members_csv")]
        public string valid_members_csv;
        [JsonProperty("invalid_members_csv")]
        public string invalid_members_csv;
    }
}

