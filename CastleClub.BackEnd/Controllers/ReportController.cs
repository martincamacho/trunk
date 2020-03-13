using CastleClub.BackEnd.Models;
using CastleClub.BusinessLogic.Managers;
using CastleClub.BusinessLogic.Utils;
using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Globalization;
using CastleClub.BackEnd.Models.Report;
using System.IO;
using CastleClub.BusinessLogic.Properties;
using CastleClub.BusinessLogic.Data;

namespace CastleClub.BackEnd.Controllers
{
    [Authorize(Roles = "Super Admin, Admin, Customer Service")]
    public class ReportController : Controller
    {
        [Authorize(Roles="Super Admin, Admin")]
        public ActionResult SalesReport()
        {
            var model = new SalesReportVM();
            model.Referrers = SitesManager.GetReferrers();
            model.Sites = SitesManager.GetSites();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles="Super Admin, Admin")]
        public ActionResult SalesReport(SalesReportVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Referrers = SitesManager.GetReferrers();
                model.Sites = SitesManager.GetSites();
                return View(model);
            }

            List<KeyValuePair<string, List<ReferrerSalesInfoDT>>> data = new List<KeyValuePair<string, List<ReferrerSalesInfoDT>>>();

            if (model.ReferrerId != 0)
            {
                if (model.SiteId != 0)
                {
                    ReferrerDT referrer = SitesManager.GetReferrer(model.ReferrerId);
                    List<ReferrerSalesInfoDT> referrerData = SitesManager.GetReferrerSiteSalesInfo(model.StartDate, model.EndDate, (DateGroupBy)Enum.Parse(typeof(DateGroupBy), model.GroupBy), model.ReferrerId, model.SiteId);
                    data.Add(new KeyValuePair<string, List<ReferrerSalesInfoDT>>(referrer.Name + " - " + SitesManager.GetSite(model.SiteId).Name, referrerData));
                }
                else
                {
                    ReferrerDT referrer = SitesManager.GetReferrer(model.ReferrerId);
                    List<ReferrerSalesInfoDT> referrerData = SitesManager.GetReferrerSiteSalesInfo(model.StartDate, model.EndDate, (DateGroupBy)Enum.Parse(typeof(DateGroupBy), model.GroupBy), model.ReferrerId, model.SiteId);
                    data.Add(new KeyValuePair<string, List<ReferrerSalesInfoDT>>(referrer.Name + " - All Sites", referrerData));

                    foreach (var site in SitesManager.GetSites())
                    {
                        referrerData = SitesManager.GetReferrerSiteSalesInfo(model.StartDate, model.EndDate, (DateGroupBy)Enum.Parse(typeof(DateGroupBy), model.GroupBy), model.ReferrerId, site.Id);
                        data.Add(new KeyValuePair<string, List<ReferrerSalesInfoDT>>(referrer.Name + " - " + site.Name, referrerData));
                    }
                }
                
            }
            else
            {
                if (model.SiteId != 0)
                {
                    string sitenName = SitesManager.GetSite(model.SiteId).Name;
                    List<ReferrerSalesInfoDT> referrerData = SitesManager.GetReferrerSiteSalesInfo(model.StartDate, model.EndDate, (DateGroupBy)Enum.Parse(typeof(DateGroupBy), model.GroupBy), 0, model.SiteId);
                    data.Add(new KeyValuePair<string, List<ReferrerSalesInfoDT>>("All Websites - " + sitenName, referrerData));

                    foreach (ReferrerDT referrer in SitesManager.GetReferrers())
                    {
                        referrerData = SitesManager.GetReferrerSiteSalesInfo(model.StartDate, model.EndDate, (DateGroupBy)Enum.Parse(typeof(DateGroupBy), model.GroupBy), referrer.Id, model.SiteId);
                        data.Add(new KeyValuePair<string, List<ReferrerSalesInfoDT>>(referrer.Name + " - " + sitenName, referrerData));
                    }
                }
                else
                {
                    List<ReferrerSalesInfoDT> referrerData = SitesManager.GetReferrerSiteSalesInfo(model.StartDate, model.EndDate, (DateGroupBy)Enum.Parse(typeof(DateGroupBy), model.GroupBy), 0, model.SiteId);
                    data.Add(new KeyValuePair<string, List<ReferrerSalesInfoDT>>("All Websites - All Sites", referrerData));

                    foreach (var site in SitesManager.GetSites())
                    {
                        foreach (ReferrerDT referrer in SitesManager.GetReferrers())
                        {
                            referrerData = SitesManager.GetReferrerSiteSalesInfo(model.StartDate, model.EndDate, (DateGroupBy)Enum.Parse(typeof(DateGroupBy), model.GroupBy), referrer.Id, site.Id);
                            data.Add(new KeyValuePair<string, List<ReferrerSalesInfoDT>>(referrer.Name + " - " + site.Name, referrerData));
                        }
                    }
                }
            }
            ViewBag.Data = data;

            model.Referrers = SitesManager.GetReferrers();
            model.Sites = SitesManager.GetSites();
            return View(model);
        }

        public ActionResult EmailReport()
        {
            EmailReportVM model = new EmailReportVM()
            {
                Referrers = SitesManager.GetReferrers(),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult EmailReport(EmailReportVM model)
        {
            if (ModelState.IsValid)
            {
                List<EmailReportDT> emailReport = new List<EmailReportDT>();
                ViewBag.data = SitesManager.GetEmailReport(model.ReferrerId, model.ReportType == "ACTIVE", model.StartDate, model.EndDate);
            }

            model.Referrers = SitesManager.GetReferrers();

            return View(model);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        public ActionResult Chart()
        {
            var sites = new List<SiteDT>();
            sites.Add(new SiteDT() { Name = "All Web Sites", Id = 0 });
            sites.AddRange(SitesManager.GetSites());
            ChartReportVM model = new ChartReportVM()
            {
                Sites = sites.GroupBy(x => x.Name).Select(x => new SiteDT() { Name = x.Key, Id = x.Max(y => y.Id) }).ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        [HttpPost]
        public ActionResult Chart(ChartReportVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            List<ReportData> list = new List<ReportData>(4);
            string siteName = model.SiteID == 0 ? "All web sites" : SitesManager.GetSite(model.SiteID).Name;

            #region
            var data = new ReportData();

            data.Colors = new List<string>();
            data.Colors.Add("#00FF00");
            data.Colors.Add("#FF0000");
            data.Colors.Add("#0000FF");

            data.Type = "column";
            data.YAxisTitle = "Values (USD)";
            data.TooltipValueSuffix = "USD";
            data.Text = "The last four " + CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.AddDays(-1).DayOfWeek).ToLower() + " (" + siteName + ")";

            var reportDay = SitesManager.GetSiteReportTheLastFourDay(model.SiteID);

            data.Categories = new List<string>();
            foreach (var item in reportDay)
            {
                data.Categories.Add(item.Key.ToString("MM-dd-yyyy"));
            }

            data.Series = new List<Data>();

            Data revenueDay = new Data() { name = "Revenue", data = new List<double>() };
            foreach (var item in reportDay)
            {
                revenueDay.data.Add(item.Value[0]);
            }
            Data costDay = new Data() { name = "Cost", data = new List<double>() };
            foreach (var item in reportDay)
            {
                costDay.data.Add(item.Value[1]);
            }
            Data profitDay = new Data() { name = "Profit", data = new List<double>() };
            foreach (var item in reportDay)
            {
                profitDay.data.Add(item.Value[0] - item.Value[1]);
            }

            data.Series.Add(revenueDay);
            data.Series.Add(costDay);
            data.Series.Add(profitDay);
            data.Id = Guid.NewGuid().ToString();

            #region Table

            data.Table = new ChartTable();
            data.Table.Cols = new List<string>();
            data.Table.Rows = new List<string>();
            data.Table.Data = new string[reportDay.Count][];

            data.Table.Rows.Add("Revenue");
            data.Table.Rows.Add("Cost");
            data.Table.Rows.Add("Profit");

            int index=0;
            foreach (var item in reportDay)
            {
                data.Table.Cols.Add(item.Key.ToString("MM/dd/yyyy"));

                data.Table.Data[index] = new string[3];
                data.Table.Data[index][0] = item.Value[0].ToString();
                data.Table.Data[index][1] = item.Value[1].ToString();
                data.Table.Data[index][2] = (item.Value[0]- item.Value[1]).ToString();

                index = index + 1;
            }

            #endregion

            list.Add(data);
            #endregion

            #region
            var data2 = new ReportData();

            data2.Colors = new List<string>();
            data2.Colors.Add("#00FF00");
            data2.Colors.Add("#FF0000");
            data2.Colors.Add("#0000FF");

            data2.Text = "The last four months" + " (" + siteName + ")";
            data2.Type = "column";
            data2.YAxisTitle = "Values (USD)";
            data2.TooltipValueSuffix = "USD";
            var reportMonth = SitesManager.GetSiteReportTheLastFourMonth(model.SiteID);

            data2.Categories = new List<string>();
            foreach (var item in reportMonth)
            {
                data2.Categories.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key.Month) + " " + item.Key.Year);
            }

            data2.Series = new List<Data>();

            Data revenueMonth = new Data() { name = "Revenue", data = new List<double>() };
            foreach (var item in reportMonth)
            {
                revenueMonth.data.Add(item.Value[0]);
            }
            Data costMonth = new Data() { name = "Cost", data = new List<double>() };
            foreach (var item in reportMonth)
            {
                costMonth.data.Add(item.Value[1]);
            }
            Data profitMonth = new Data() { name = "Profit", data = new List<double>() };
            foreach (var item in reportMonth)
            {
                profitMonth.data.Add(item.Value[0] - item.Value[1]);
            }

            data2.Series.Add(revenueMonth);
            data2.Series.Add(costMonth);
            data2.Series.Add(profitMonth);
            data2.Id = Guid.NewGuid().ToString();

            #region Table

            data2.Table = new ChartTable();
            data2.Table.Cols = new List<string>();
            data2.Table.Rows = new List<string>();
            data2.Table.Data = new string[reportMonth.Count][];

            data2.Table.Rows.Add("Revenue");
            data2.Table.Rows.Add("Cost");
            data2.Table.Rows.Add("Profit");

            index = 0;
            foreach (var item in reportMonth)
            {
                data2.Table.Cols.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key.Month) + " " + item.Key.Year);

                data2.Table.Data[index] = new string[3];
                data2.Table.Data[index][0] = item.Value[0].ToString();
                data2.Table.Data[index][1] = item.Value[1].ToString();
                data2.Table.Data[index][2] = (item.Value[0] - item.Value[1]).ToString();

                index = index + 1;
            }

            #endregion

            list.Add(data2);
            #endregion

            #region
            var data3 = new ReportData();

            data3.Colors = new List<string>();
            data3.Colors.Add("#00FF00");
            data3.Colors.Add("#FF0000");
            data3.Colors.Add("#0000FF");

            data3.Text = "The last fifteen days" + " (" + siteName + ")";
            data3.Type = "column";
            data3.YAxisTitle = "Values (USD)";
            data3.TooltipValueSuffix = "USD";
            var reportFifteenDays = SitesManager.GetSiteReportTheLastFifteenDays(model.SiteID);

            data3.Categories = new List<string>();
            foreach (var item in reportFifteenDays)
            {
                data3.Categories.Add(item.Key.ToString("MM-dd-yyyy"));
            }

            data3.Series = new List<Data>();

            Data revenueFifteenDays = new Data() { name = "Revenue", data = new List<double>() };
            foreach (var item in reportFifteenDays)
            {
                revenueFifteenDays.data.Add(item.Value[0]);
            }
            Data costFifteenDays = new Data() { name = "Cost", data = new List<double>() };
            foreach (var item in reportFifteenDays)
            {
                costFifteenDays.data.Add(item.Value[1]);
            }
            Data profitFifteenDays = new Data() { name = "Profit", data = new List<double>() };
            foreach (var item in reportFifteenDays)
            {
                profitFifteenDays.data.Add(item.Value[0] - item.Value[1]);
            }

            data3.Series.Add(revenueFifteenDays);
            data3.Series.Add(costFifteenDays);
            data3.Series.Add(profitFifteenDays);
            data3.Id = Guid.NewGuid().ToString();

            #region Table

            data3.Table = new ChartTable();
            data3.Table.Cols = new List<string>();
            data3.Table.Rows = new List<string>();
            data3.Table.Data = new string[reportFifteenDays.Count][];

            data3.Table.Rows.Add("Revenue");
            data3.Table.Rows.Add("Cost");
            data3.Table.Rows.Add("Profit");

            index = 0;
            foreach (var item in reportFifteenDays)
            {
                data3.Table.Cols.Add(item.Key.ToString("MM-dd-yyyy"));

                data3.Table.Data[index] = new string[3];
                data3.Table.Data[index][0] = item.Value[0].ToString();
                data3.Table.Data[index][1] = item.Value[1].ToString();
                data3.Table.Data[index][2] = (item.Value[0] - item.Value[1]).ToString();

                index = index + 1;
            }

            #endregion

            list.Add(data3);
            #endregion


            #region
            var data4 = new ReportData();

            data4.Colors = new List<string>();
            data4.Colors.Add("#00FF00");
            data4.Colors.Add("#FF0000");
            data4.Colors.Add("#0000FF");

            data4.Text = "The last sixty days" + " (" + siteName + ")";
            data4.Type = "line";
            data4.YAxisTitle = "Values";
            data4.TooltipValueSuffix = "";
            var reportSixtyDays = SitesManager.GetSiteReportTheLastSixtyDays(model.SiteID);
           
           data4.Categories = new List<string>();
           foreach (var item in reportSixtyDays)
           {
               data4.Categories.Add(item.Key.ToString("MM-dd-yyyy"));
           }
           
            data4.Series = new List<Data>();

            Data signupsSixtyDays = new Data() { name = "Sign Up", data = new List<double>() };
            foreach (var item in reportSixtyDays)
            {
                signupsSixtyDays.data.Add(item.Value[0]);
            }
            Data visitsSixtyDays = new Data() { name = "Billeds", data = new List<double>() };
            foreach (var item in reportSixtyDays)
            {
                visitsSixtyDays.data.Add(item.Value[1]);
            }
            Data cancellationsSixtyDays = new Data() { name = "Cancellations", data = new List<double>() };
            foreach (var item in reportSixtyDays)
            {
                cancellationsSixtyDays.data.Add(item.Value[2]);
            }

            data4.Series.Add(signupsSixtyDays);
            data4.Series.Add(visitsSixtyDays);
            data4.Series.Add(cancellationsSixtyDays);
            data4.Id = Guid.NewGuid().ToString();

            #region Table

            data4.Table = new ChartTable();
            /**/
            data4.Table.Cols = new List<string>();
            data4.Table.Rows = new List<string>();
            /**
           data4.Table.Data = new string[reportSixtyDays.Count][];

           
           data4.Table.Rows.Add("Sign Ups");
           data4.Table.Rows.Add("Billeds");
           data4.Table.Rows.Add("Cancellations");
            
           index = 0;
           foreach (var item in reportSixtyDays)
           {
               data4.Table.Cols.Add(item.Key.ToString("MM-dd-yyyy"));

               data4.Table.Data[index] = new string[3];
               data4.Table.Data[index][0] = item.Value[0].ToString();
               data4.Table.Data[index][1] = item.Value[1].ToString();
               data4.Table.Data[index][2] = item.Value[2].ToString();

               index = index + 1;
           }
                       **/
            data4.Table.Data = new string[3][];
            data4.Table.Cols.Add("Sign Ups");
            data4.Table.Cols.Add("Billeds");
            data4.Table.Cols.Add("Cancellations");

            data4.Table.Data[0] = new string[reportSixtyDays.Count];
            data4.Table.Data[1] = new string[reportSixtyDays.Count];
            data4.Table.Data[2] = new string[reportSixtyDays.Count];
            index = 0;
            foreach (var item in reportSixtyDays)
            {
                data4.Table.Rows.Add(item.Key.ToString("MM-dd-yyyy"));

                //data4.Table.Data[index] = new string[3];
                data4.Table.Data[0][index] = item.Value[0].ToString();
                data4.Table.Data[1][index] = item.Value[1].ToString();
                data4.Table.Data[2][index] = item.Value[2].ToString();

                index = index + 1;
            }

            #endregion

            list.Add(data4);
            #endregion   


            return Json(list);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        public ActionResult AuthorizeTransactionCompare()
        {
            AuthorizeCompareTransactionVM model = new AuthorizeCompareTransactionVM()
            {
                From = DateTime.Today.Date,
                To = DateTime.Today.Date,
                Sites = new List<SiteDT>()
            };

            var sites = SitesManager.GetSites();
            model.Sites.Add(new SiteDT()
                {
                    Name = "All Web Sites",
                    Id = 0
                });

            foreach (var site in sites.OrderBy(x => x.Name))
            {
                if (!model.Sites.Any(x => x.Name == site.Name))
                {
                    model.Sites.Add(site);
                }
            }

            return View(model);
        }

        [Authorize(Roles="Super Admin, Admin")]
        [HttpPost]
        public ActionResult AuthorizeTransactionCompare(AuthorizeCompareTransactionVM model)
        {
            if (!ModelState.IsValid && CastleClub.BusinessLogic.Data.GlobalParameters.AuthorizeTransactionCompare)
            {
                return View(model);
            }

            Dictionary<DateTime, decimal[]> data = new Dictionary<DateTime, decimal[]>();

            for (DateTime i = model.From; i <= model.To; i = i.AddDays(1))
            {
                data.Add(i, new decimal[3]);
                var report = SitesManager.GetInvoiceSitesForDateRange(model.SitedID, i, i);

                ((decimal[])data[i])[0] = report.Sum(x => x.Amount);
                ((decimal[])data[i])[1] = report.Where(x => x.BilledDate.Date==i).Sum(x => x.Amount);
                ((decimal[])data[i])[2] = report.Where(x => x.BilledDate.Date!=i).Sum(x => x.Amount);
            }

            var chart = new ReportData();

            chart.Colors = new List<string>();
            chart.Colors.Add("#0000FF");
            chart.Colors.Add("#00FF00");
            chart.Colors.Add("#FF0000");

            chart.Text = "Transactions compare with Authorize";

            chart.Categories = new List<string>();
            chart.Series = new List<Data>();

            Data system = new Data() { name = "System", data = new List<double>() };
            Data authorizeSuccessfull = new Data() { name = "Authorize Successfull", data = new List<double>() };
            Data authorizeFail = new Data() { name = "Authorize Failed", data = new List<double>() };
            for (DateTime i = model.From; i <= model.To; i = i.AddDays(1))
            {
                chart.Categories.Add(i.ToString("MM/dd/yyyy"));

                system.data.Add((double)((decimal[])data[i])[0]);
                authorizeSuccessfull.data.Add((double)((decimal[])data[i])[1]);
                authorizeFail.data.Add((double)((decimal[])data[i])[2]);
            }

            chart.Series.Add(system);
            chart.Series.Add(authorizeSuccessfull);
            chart.Series.Add(authorizeFail);
            chart.Id = Guid.NewGuid().ToString();

            #region Table

            chart.Table = new ChartTable();
            chart.Table.Cols = new List<string>();
            chart.Table.Rows = new List<string>();
            chart.Table.Data = new string[data.Keys.Count][];

            chart.Table.Rows.Add("System");
            chart.Table.Rows.Add("Authorize Successfull");
            chart.Table.Rows.Add("Authorize Failed");

            int index = 0;
            foreach (var item in data.Keys)
            {
                chart.Table.Cols.Add(item.ToString("MM/dd/yyyy"));

                chart.Table.Data[index] = new string[3];
                chart.Table.Data[index][0] = ((decimal[])data[item])[0].ToString();
                chart.Table.Data[index][1] = ((decimal[])data[item])[1].ToString();
                chart.Table.Data[index][2] = ((decimal[])data[item])[2].ToString();

                index = index + 1;
            }

            #endregion

            return Json(chart);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        [HttpPost]
        public ActionResult GetInvoicesTypeForSiteAndDate(AuthorizeCompareTableRequestVM model)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }

            DateTime date = new DateTime();
            if (DateTime.TryParse(model.Date, out date) && !string.IsNullOrEmpty(model.ViewType))
            {
                var report = SitesManager.GetInvoiceSitesForDateRange(model.WebSite, date, date);
                if (model.ViewType == "System")
                {
                    report = report.ToList();
                }
                else if (model.ViewType == "Authorize Successfull")
                {
                    report = report.Where(x => x.BilledDate.Date==date).ToList();
                }
                else if (model.ViewType == "Authorize Failed")
                {
                    report = report.Where(x => x.BilledDate.Date!=date).ToList();
                }

                List<string> headers = new List<string>()
                {
                    "CustomerID",
                    "Status",
                    "Date",
                    "View"
                };

                List<List<string>> data = new List<List<string>>();
                foreach (var item in report)
                {
                    data.Add(new List<string>()
                        {
                            item.CustomerId.ToString(),
                            item.Status.ToString(),
                            item.CreatedAt.ToString(),
                            "<a href='/Subscription/ManageSubscriptions?customerEmail="+CustomersManager.GetCustomer(item.CustomerId).Email+"'>View</a>"
                        });
                }

                return Json(new { Header = headers, Data = data });
            }
            return null;
        }

        [Authorize(Roles="Super Admin, Admin")]
        public ActionResult TestPage()
        {
            var model = new CastleClub.BackEnd.Models.TestVM();

            model.Sites = new List<SiteDT>();
            foreach (var siteDT in SitesManager.GetSites().OrderBy(x => x.Name))
            {
                if (!model.Sites.Any(x => x.Name.ToLower()==siteDT.Name.ToLower()))
                {
                    model.Sites.Add(siteDT);
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles="Super Admin, Admin")]
        public ActionResult TestPage(TestVM model)
        {
            model.Sites = new List<SiteDT>();
            foreach (var siteDT in SitesManager.GetSites().OrderBy(x => x.Name))
            {
                if (!model.Sites.Any(x => x.Name.ToLower() == siteDT.Name.ToLower()))
                {
                    model.Sites.Add(siteDT);
                }
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Emails = CustomersManager.GetAllEmails(model.SiteID);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles="Super Admin, Admin")]
        public void SendEmail(TestVM model)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(model.EmailToSend) && !string.IsNullOrEmpty(model.Name))
            {
                
                string body = "";
               
                
                #region Generate random string (result in result variable)

               // var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                //var random = new Random();
                var result = "";// new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());

                #endregion
                
                SiteDT siteDT = SitesManager.GetSite(model.SiteID);
                using (CastleClubEntities entities = new CastleClubEntities())
                {
                    Customer customer = entities.Customers.Where(c => c.Email == model.EmailToSend  && c.SiteId == siteDT.Id).FirstOrDefault();
                    if (customer != null)
                    {
                        result = customer.UnEncryptPass;

                        if (result == null)
                        {
                            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                            var random = new Random();
                            result =  new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());

                            Password p = new Password(result);
                            customer.Password = p.SaltedPassword;
                            customer.UnEncryptPass = p.ClearPassword;
                            //customer.
                            customer.SaltKey = p.SaltKey;

                            entities.SaveChanges();
                        }
                    }

                }
                body = string.Format(Email.WelcomeBodyEmail(), model.Name,
                                                    model.EmailToSend, result, siteDT.Name, siteDT.Name, "PartsGeek",
                                                    siteDT.Price, siteDT.PricePerQuarter, siteDT.Phone, siteDT.Email, siteDT.OfferDomain);

                string subject = string.Format(Email.WelcomeSubjectEmail(), siteDT.Name);
                CastleClub.BusinessLogic.Utils.Email.SendEmailWithBCC(siteDT.Email, siteDT.PasswordEmail, siteDT.SmtpAddress, subject, body
                    , new List<string>() { model.EmailToSend }, new List<string>() { siteDT.WelcomeEmailBCC }, true);
            }
        }

        [Authorize(Roles="Super Admin, Admin")]
        public ActionResult ReportSiteCreditCards()
        {
            var model = new ReportSiteCreditCardsVM()
                {
                    From=DateTime.Now.Date,
                    To=DateTime.Now.Date
                };
            return View(model);
        }

        [Authorize(Roles = "Super Admin, Admin")]
        [HttpPost]
        public ActionResult ReportSiteCreditCards(ReportSiteCreditCardsVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Data = new Dictionary<SiteDT, List<ReportSiteCreditCardDataVM>>();
            if (model.SiteID==0)
            {
                var sites = CastleClub.BusinessLogic.Managers.SitesManager.GetSites();

                foreach (var site in sites)
                {
                    model.Data.Add(site, new List<ReportSiteCreditCardDataVM>());
                }
            }
            else
            {
                model.Data.Add(CastleClub.BusinessLogic.Managers.SitesManager.GetSite(model.SiteID), new List<ReportSiteCreditCardDataVM>());
            }

            foreach (var data in model.Data)
            {
                int signup = 0;
                int active = 0;

                CastleClub.BusinessLogic.Managers.SitesManager.ReportSiteCreditCards(model.From, model.To, data.Key.Id, CCType.VISA, true, out signup, out active);
                data.Value.Add(new ReportSiteCreditCardDataVM()
                    {
                        Name = "Visa – Email Only",
                        TotalActive=active,
                        TotalSignup=signup
                    });

                CastleClub.BusinessLogic.Managers.SitesManager.ReportSiteCreditCards(model.From, model.To, data.Key.Id, CCType.VISA, false, out signup, out active);
                data.Value.Add(new ReportSiteCreditCardDataVM()
                {
                    Name = "Visa – Full Data",
                    TotalActive = active,
                    TotalSignup = signup
                });

                CastleClub.BusinessLogic.Managers.SitesManager.ReportSiteCreditCards(model.From, model.To, data.Key.Id, CCType.MASTERCARD, true, out signup, out active);
                data.Value.Add(new ReportSiteCreditCardDataVM()
                {
                    Name = "Mastercard – Email Only",
                    TotalActive = active,
                    TotalSignup = signup
                });

                CastleClub.BusinessLogic.Managers.SitesManager.ReportSiteCreditCards(model.From, model.To, data.Key.Id, CCType.MASTERCARD, false, out signup, out active);
                data.Value.Add(new ReportSiteCreditCardDataVM()
                {
                    Name = "Mastercard – Full Data",
                    TotalActive = active,
                    TotalSignup = signup
                });

                CastleClub.BusinessLogic.Managers.SitesManager.ReportSiteCreditCards(model.From, model.To, data.Key.Id, CCType.DISCOVER, true, out signup, out active);
                data.Value.Add(new ReportSiteCreditCardDataVM()
                {
                    Name = "Discover – Email Only",
                    TotalActive = active,
                    TotalSignup = signup
                });

                CastleClub.BusinessLogic.Managers.SitesManager.ReportSiteCreditCards(model.From, model.To, data.Key.Id, CCType.DISCOVER, false, out signup, out active);
                data.Value.Add(new ReportSiteCreditCardDataVM()
                {
                    Name = "Discover – Full Data",
                    TotalActive = active,
                    TotalSignup = signup
                });
            }

            model.Total = CastleClub.BusinessLogic.Managers.SitesManager.CustomersCountRegisterRange(model.From, model.To);

            return View(model);
        }

        public ActionResult ReportIisReset()
        {
            ViewBag.Log = SitesManager.GetIISResetLog();
            return View();
        }


        public ActionResult ReportWelcomeEmail()
        {
            ReportEmailVM model = new ReportEmailVM()
            {
                Referrers = SitesManager.GetReferrers(),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                Sites = SitesManager.GetSites()
            }; 
            return View(model);
        }

        [HttpPost]
        public ActionResult ReportWelcomeEmail(ReportEmailVM model)
        {
            int siteID = model.SiteId;
            model.Referrers = SitesManager.GetReferrers();
            model.Sites = SitesManager.GetSites();
            //using (CastleClub.BusinessLogic.Data.CastleClubEntities entities = new BusinessLogic.Data.CastleClubEntities()) 
            //{ i
            //int i = 0;
            model.delayDef = new int[] {0,1,2,3,7};
            model.delayEmailSent = new int[model.delayDef.Max()+1, SitesManager.GetMaxID()+1];
            model.delayActiveUsers = new int[model.delayDef.Max() + 1, SitesManager.GetMaxID() + 1];
            model.delayCancelledUsers = new int[model.delayDef.Max() + 1, SitesManager.GetMaxID() + 1];
            foreach (var item in model.Sites)
            {
                //get total of email sent
                foreach (var delay in model.delayDef.ToList())
                {
                    if ((model.SiteId == item.Id) || model.SiteId == 0) 
                    {
                        model.delayEmailSent[delay, item.Id] = SitesManager.WelcomeEmailsSent(model.StartDate, model.EndDate, item.Id, model.ReferrerId, delay);
                        model.delayActiveUsers[delay, item.Id] = SitesManager.WelcomeEmailsSentActive(model.StartDate, model.EndDate, item.Id, model.ReferrerId, delay);
                        model.delayCancelledUsers[delay, item.Id] = SitesManager.WelcomeEmailsSentCancelled(model.StartDate, model.EndDate, item.Id, model.ReferrerId, delay);
                    }
                }
                    
            }

            //}


            return View(model);
        }
    }
}