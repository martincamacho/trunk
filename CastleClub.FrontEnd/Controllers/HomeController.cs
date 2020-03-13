using CastleClub.BusinessLogic.Managers;
using CastleClub.BusinessLogic.Utils;
using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;
using CastleClub.FrontEnd.Models.Home;
using CastleClub.FrontEnd.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CastleClub.FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        
        
        public ActionResult EmailForm()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            #region Debug
            //if (true)
            //{
            //    var site = ((SiteDT)ViewBag.Site);
            //    double newPercentaje = 0;
            //    CastleClub.DataTypes.Enums.CCType ccType = CCType.VISA;
            //    bool cont = false;
            //    switch ("visa")
            //    {
            //        case "visa":
            //            newPercentaje = (((double)(site.VisaEmailFormCount + 1) / (double)(site.VisaEmailFormTotal + 1))) * 100;
            //            cont = newPercentaje <= site.VisaEmailFormPercentage;
            //            break;
            //        case "mastercard":
            //            newPercentaje = (((double)(site.MasterCardEmailFormCount + 1) / (double)(site.MasterCardEmailFormTotal + 1))) * 100;
            //            ccType = CCType.MASTERCARD;
            //            cont = newPercentaje <= site.MasterCardEmailFormPercentage;
            //            break;
            //        case "discover":
            //            newPercentaje = (((double)(site.DiscoverEmailFormCount + 1) / (double)(site.DiscoverEmailFormTotal + 1))) * 100;
            //            ccType = CCType.DISCOVER;
            //            cont = newPercentaje <= site.DiscoverEmailFormPercentage;
            //            break;
            //        default:
            //            break;
            //    }

            //    if (cont)
            //    {
            //        CastleClub.BusinessLogic.Managers.SitesManager.UpdateCreditCardDataAllSites(ccType, true);
            //        var model = new EmailFormVM();
            //        model.SiteId = ((SiteDT)ViewBag.Site).Id;
            //        AddVisit(((SiteDT)ViewBag.Site).Id);
            //        return View(model);
            //    }

            //    CastleClub.BusinessLogic.Managers.SitesManager.UpdateCreditCardDataAllSites(ccType, false);

            //    return RedirectToAction("FullForm");
            //}
            #endregion

            ////var site = ((SiteDT)ViewBag.Site);
            var queryString = Request.QueryString.ToString();
            ////if (Request.QueryString.AllKeys.Contains("s") && Request.QueryString.AllKeys.Contains("t") &&
            ////    (site.MasterCardEmailFormPercentage>0 || site.VisaEmailFormPercentage>0 || site.DiscoverEmailFormPercentage>0))
            ////{
            ////    var referrerDT = ReferrersManagers.GetReferrer(Request.QueryString["s"]).GetDT();
            ////    Hashtable ht = new Hashtable();

            ////    try
            ////    {
            ////        ht = EncryptionHelper.Unpack(EncryptionHelper.Decrypt(Request.QueryString["t"], referrerDT.SiteKey));
            ////    }
            ////    catch(Exception ex)
            ////    {
            ////        SendExceptionEmail(ex.ToString());
            ////    }
            ////    string customerId = String.Empty;

            ////    if (ht.ContainsKey("CustomerID"))
            ////    {
            ////        customerId = ht["CustomerID"].ToString();
            ////        string data = string.Empty;
            ////        try
            ////        {
            ////            data = CustomersManager.RequestToPartsGeek(referrerDT, customerId);
            ////        }
            ////        catch(Exception ex)
            ////        {
            ////            SendExceptionEmail(ex.ToString());
            ////        }
            ////        if (!string.IsNullOrEmpty(data))
            ////        {
            ////            var customerHash = EncryptionHelper.Unpack(EncryptionHelper.Decrypt(data, referrerDT.SiteKey));
                        
                        
            ////            //string creditCardType = CreditCardHelper.GetCardType(customerHash["CardNumber"].ToString());
            ////            //if ((creditCardType.ToLower() == CCType.MASTERCARD.ToString().ToLower() && site.MasterCardEmailFormPercentage>0)
            ////            //    || (creditCardType.ToLower() == CCType.VISA.ToString().ToLower() && site.VisaEmailFormPercentage>0)
            ////            //    || (creditCardType.ToLower() == CCType.DISCOVER.ToString().ToLower() && site.DiscoverEmailFormPercentage > 0))
            ////            //{
            ////            //    double newPercentaje = 0;
            ////            //    CastleClub.DataTypes.Enums.CCType ccType = (CastleClub.DataTypes.Enums.CCType)Enum.Parse(typeof(CastleClub.DataTypes.Enums.CCType), creditCardType.ToUpper());
            ////            //    bool showEmailForm = false;
            ////            //    switch (ccType)
            ////            //    {
            ////            //        case CCType.VISA:
            ////            //            newPercentaje = (((double)(site.VisaEmailFormCount + 1) / (double)(site.VisaEmailFormTotal + 1))) * 100;
            ////            //            showEmailForm = newPercentaje <= site.VisaEmailFormPercentage;
            ////            //            break;
            ////            //        case CCType.MASTERCARD:
            ////            //            newPercentaje = (((double)(site.MasterCardEmailFormCount + 1) / (double)(site.MasterCardEmailFormTotal + 1))) * 100;
            ////            //            showEmailForm = newPercentaje <= site.MasterCardEmailFormPercentage;
            ////            //            break;
            ////            //        case CCType.DISCOVER:
            ////            //            newPercentaje = (((double)(site.DiscoverEmailFormCount + 1) / (double)(site.DiscoverEmailFormTotal + 1))) * 100;
            ////            //            showEmailForm = newPercentaje <= site.DiscoverEmailFormPercentage;
            ////            //            break;
            ////            //        default:
            ////            //            break;
            ////            //    }

            ////            //    if (showEmailForm)
            ////            //    {
            ////            //        CastleClub.BusinessLogic.Managers.SitesManager.UpdateCreditCardDataAllSites(ccType,true);
            ////            //        var model = new EmailFormVM();
            ////            //        model.SiteId = ((SiteDT)ViewBag.Site).Id;
            ////            //        AddVisit(((SiteDT)ViewBag.Site).Id, queryString, ccType.ToString());
            ////            //        return View(model);
            ////            //    }

            ////            //    CastleClub.BusinessLogic.Managers.SitesManager.UpdateCreditCardDataAllSites(ccType, false);
            ////            //}
            ////        }
            ////    }
            ////}

            //return RedirectToAction("FullForm");
            Response.Redirect("optin.aspx?" + queryString);
            return View();
        }

        private static void SendExceptionEmail(string message)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if (System.Configuration.ConfigurationManager.AppSettings["SendEmailException"].ToLower() == "true")
            {
                string[] separator = new string[] { ";" };
                List<string> emailList = System.Configuration.ConfigurationManager.AppSettings["EmailException"].Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> ccList = System.Configuration.ConfigurationManager.AppSettings["CCEmailException"].Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();
                CastleClub.BusinessLogic.Utils.Email.SendEmailWithCC(System.Configuration.ConfigurationManager.AppSettings["EmailAccount"], System.Configuration.ConfigurationManager.AppSettings["EmailPassword"], System.Configuration.ConfigurationManager.AppSettings["Smtp"], "Castle Club frontend", message, emailList, ccList, false);
            }
            
        }

        [HttpPost]
        public ActionResult EmailForm(EmailFormVM model)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (Request.QueryString.AllKeys.Contains("s") && Request.QueryString.AllKeys.Contains("t"))
            {
                string siteIdentifier = Request.QueryString.GetValues("s").FirstOrDefault();
                var referrer = ReferrersManagers.GetReferrer(siteIdentifier);
                int ncid = GetNcId();
                int visitId = GetVisitId();
                string email = model.Email;
                string token = Request.QueryString.GetValues("t").FirstOrDefault();
                CustomerDT customer = null;
                try
                {
                    customer = CustomersManager.NewCustomer(((SiteDT)ViewBag.Site), email, token, ncid, visitId, referrer.GetDT());
                    ViewBag.Customer = customer;
                }
                catch(Exception ex)
                {
                    SendExceptionEmail(ex.ToString());
                }

                if (customer!=null)
                {
                    return View("Welcome");
                }
                else
                {
                    return View("Error");
                }
            }

            throw new Exception();
        }

        public ActionResult FullForm()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var model = new FullFormVM();
            model.SiteId = ((SiteDT)ViewBag.Site).Id;
            model.States = LocationsManager.GetStates();

            var referrerDT = ReferrersManagers.GetReferrer(Request.QueryString["s"] != null ? Request.QueryString["s"] : "LF113B3N").GetDT();
            Hashtable ht = new Hashtable();

            try
            {
                ht = EncryptionHelper.Unpack(EncryptionHelper.Decrypt(Request.QueryString["t"].ToString(), referrerDT.SiteKey));
            }
            catch(Exception ex)
            {
                SendExceptionEmail(ex.ToString());
            }
             string customerId = String.Empty;
            string cctype = string.Empty;
             if (ht.ContainsKey("CustomerID"))
             {
                customerId = ht["CustomerID"].ToString();
                string data = string.Empty;
             
                data = CustomersManager.RequestToPartsGeek(referrerDT, customerId);
                if (!(string.IsNullOrEmpty(data) || (data.ToLower()=="not found")))
                {
                    var customerHash = EncryptionHelper.Unpack(EncryptionHelper.Decrypt(data, referrerDT.SiteKey));
                    if(customerHash != null)
                    {
                        model.SiteId = ((SiteDT)ViewBag.Site).Id;
                        model.Email =  customerHash["Email"] != null ? customerHash["Email"].ToString() : "";
                        model.FirstName = customerHash["FirstName"]!=null ? customerHash["FirstName"].ToString():"";
                        model.LastName =customerHash["LastName"]!=null ? customerHash["LastName"].ToString():"";
                        //model. .Phone = this.customerHash["Phone"].ToString();
                        model.Address = customerHash["Address1"]!=null ? customerHash["Address1"].ToString():"";
                        model.City = customerHash["City"]!=null ? customerHash["City"].ToString():"";
                        model.StateId = customerHash["State"]!=null ? customerHash["State"].ToString():"";
                        model.ZipCode =  customerHash["Zip"] != null ? customerHash["Zip"].ToString() : "";
                        cctype =  customerHash["CCType"] != null ? customerHash["CCType"].ToString() : "";
                        model.SelCCType = cctype;
                    }
                }
             }
             AddVisit(((SiteDT)ViewBag.Site).Id, Request.QueryString.ToString(), cctype);
            model.Referrer = string.IsNullOrEmpty(Request.QueryString["s"]) ? "LF113B3N" : Request.QueryString["s"];
            return View(model);
        }

        [HttpPost]
        public ActionResult FullForm(FullFormVM model)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if (!ModelState.IsValid)
            {
                model.States = LocationsManager.GetStates();
                return View(model);
            }
            
            int ncid = GetNcId();
            int visitId = GetVisitId();

            CustomerDT customer = new CustomerDT();
            customer.SiteId = model.SiteId;
            customer.NcId = ncid;
            customer.Email = model.Email;
            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Address = model.Address;
            customer.City = model.City;
            customer.StateId = model.StateId;
            customer.ZipCode = model.ZipCode;

            CreditCardDT cc = new CreditCardDT();
            cc.CardNumber = model.CardNumber;
            cc.CVV = model.CVV;
            cc.ExpDate = CreditCardHelper.GetExpDate(model.ExpMonth, model.ExpYear);
            CCType cardType= CCType.VISA;
            Enum.TryParse(model.CardType.ToString(), out cardType);
            cc.Type = cardType;

            var referrer = ReferrersManagers.GetReferrer(model.Referrer);
            try
            {
                customer = CustomersManager.NewCustomer(((SiteDT)ViewBag.Site), customer, cc, visitId, referrer.Id, false);
            }
            catch(Exception ex)
            {
                SendExceptionEmail(ex.ToString());
                CastleClub.BusinessLogic.Utils.EventViewer.Writte("CastleClub", "CastleClubFrontend", ex.Message + "\n" + ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }

            if (customer != null)
            {
                ViewBag.Customer = customer;
            }
            else
            {
                return View("Error");
            }

            return View("Welcome");
        }

        public ActionResult Welcome()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if (CastleClub.BusinessLogic.Data.GlobalParameters.EmailWelcome)
            {
                SiteDT site = ViewBag.Site;
                CustomerDT customer = ViewBag.Customer;

                CastleClub.BusinessLogic.Utils.Email.SendEmail(site.Email, CastleClub.BusinessLogic.Data.GlobalParameters.EmailPassword,
                    CastleClub.BusinessLogic.Data.GlobalParameters.Smtp, string.Format(Properties.Resources.EmailWelcomeSubjet,site.Name),
                    string.Format(Properties.Resources.EmailWelcomeBody, customer.FirstName+" "+customer.LastName, customer.Email, customer.ClearPassword,
                    site.Name, site.ShortName, "PartsGeek", site.Price, site.PricePerQuarter, site.Phone, site.Email, site.OfferDomain), new List<string>() { customer.Email }, true);
            }
            return View("Welcome");
        }

        public ActionResult TermsOfMembership()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            return View("TermsOfMembership");
        }

        public ActionResult SalesPrivacy()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            return View("SalesPrivacy");
        }

        public ActionResult SalesClaim()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            return View("SalesClaim");
        }

        public ActionResult SalesTerms()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            return View("SalesTerms");
        }

        public ActionResult Css()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            return new CssViewResult(((SiteDT)ViewBag.Site));
        }

        private void AddVisit(int siteId, string queryString, string CCinfo)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            int visitId = SitesManager.NewVisit(Request.UserHostAddress, Request.UserAgent, siteId, queryString, CCinfo);
            HttpCookie cookie = new HttpCookie("visitId");
            cookie.Value = visitId.ToString();
            cookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(cookie);
        }

        private int GetVisitId()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            int visitId = 0;
            if (Request.Cookies.AllKeys.Contains("visitId"))
            {
                Int32.TryParse(Request.Cookies["visitId"].Value, out visitId);
            }
            return visitId;
        }

        private int GetNcId()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            int ncId = 0;
            if (Request.QueryString.AllKeys.Contains("ncid"))
            {
                Int32.TryParse(Request.QueryString.GetValues("ncid").FirstOrDefault(), out ncId);
            }
            return ncId;
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            throw new Exception();
        }
	}
}