using CastleClub.BusinessLogic.AuthorizeService;
using CastleClub.BusinessLogic.Data;
using CastleClub.BusinessLogic.Utils;
using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;
using CastleClub.DataTypes.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace CastleClub.BusinessLogic.Managers
{
    public static class SitesManager
    {
        public static SiteDT GetSiteFromHostname(string host)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                return entities.Sites.FirstOrDefault(x => x.Active).GetDT();
            }
        }

        public static SiteDT GetSiteActive()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                var siteOffer = entities.Sites.FirstOrDefault(x => x.Active);
                if (siteOffer == null)
                {
                    throw new Exception();
                }
                return siteOffer.GetDT();
            }
        }

        public static int NewVisit(string IPAddress, string userAgent, int siteId, string queryString, string CCinfo)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                Visit visit = new Visit();
                visit.SiteId = siteId;
                visit.IPAddress = IPAddress;
                visit.UserAgent = userAgent;
                visit.CreatedAt = DateTime.Now;
                visit.QueryString = queryString;
                visit.CCinfo = CCinfo;
                entities.Visits.Add(visit);
                var site = entities.Sites.FirstOrDefault(x => x.Id == siteId);
                if (site != null)
                {
                    site.TotalVisits++;
                }
                entities.SaveChanges();
                return visit.Id;
            }
        }

        public static List<SiteDT> GetSites()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                return entities.Sites.OrderBy(s => s.Name).ToList().Select(s => s.GetDT()).ToList();
            }
        }

        public static SiteDT GetSite(int id)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                return entities.Sites.FirstOrDefault(s => s.Id == id).GetDT();
            }
        }

        public static List<SitesInfoDT> GetSitesInfo(DateTime from, DateTime to)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                List<SitesInfoDT> sitesInfoDT = new List<SitesInfoDT>();

                DateTime? min = from;
                DateTime? max = to.AddDays(1);
                var todayDate = DateTime.Now.Date;
                foreach (var site in entities.Sites)
                {
                    int signup = site.Customers.Where(x => x.CreatedAt >= min && x.CreatedAt <= max).Count();
                    int activeCustomer = site.Customers.Where(x => !x.CancelledDate.HasValue).Count();
                    int totalCustomer = site.Customers.Count;
                    int signupToday = site.Customers.Count(x => x.CreatedAt.Date == todayDate);
                    double refund = totalCustomer > 0 ? ((double)(site.Customers.Where(x => x.Refunded).Count() * 100) / (double)totalCustomer) : 0;

                    int? siteId = site.Id;
                    var visits = entities.VisitsPerSiteDateRange(siteId, min, max).FirstOrDefault();
                    var visitsToday = entities.VisitsPerSiteDateRange(siteId, todayDate, todayDate.AddDays(1).AddSeconds(-1)).FirstOrDefault();

                    if (visits != null)
                    {
                        double porcentage = visits > 0 ? ((double)signup * 100) / (double)visits.Value : 0.00;

                        double porcentageToday = (visitsToday != null && visitsToday.HasValue && visitsToday.Value > 0) ? ((double)signupToday * 100) / (double)visitsToday.Value : 0;

                        var siteBefore = sitesInfoDT.FirstOrDefault(x => x.Name.ToLower() == site.Name.ToLower());
                        if (siteBefore != null)
                        {
                            siteBefore.Visits = siteBefore.Visits + visits.Value;
                            siteBefore.Signup = siteBefore.Signup + signup;

                            siteBefore.Percentage = siteBefore.Visits > 0 ? ((double)siteBefore.Signup * 100) / (double)siteBefore.Visits : 0.00;

                            siteBefore.ActiveCustomers = siteBefore.ActiveCustomers + activeCustomer;
                            totalCustomer = siteBefore.TotalCustomers + totalCustomer;
                            siteBefore.Refund = ((double)siteBefore.ActiveCustomers * 100) / (double)totalCustomer;
                            siteBefore.Id = siteBefore.Id >= site.Id ? siteBefore.Id : site.Id;
                        }
                        else
                        {
                            sitesInfoDT.Add(new SitesInfoDT()
                            {
                                Id = site.Id,
                                Name = site.Name,
                                Percentage = porcentage,
                                PercentageSignupToday = porcentageToday,
                                Signup = signup,
                                Visits = visits.Value,
                                ActiveCustomers = activeCustomer,
                                Refund = refund,
                                TotalCustomers = totalCustomer
                            });
                        }
                    }
                }

                return sitesInfoDT;
            }
        }

        public static List<ReferrerDT> GetReferrers()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                return entities.Referrers.OrderBy(r => r.Name).ToList().Select(r => r.GetDT()).ToList();
            }
        }

        public static ReferrerDT GetReferrer(int id)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                Referrer referrer = entities.Referrers.Where(r => r.Id == id).FirstOrDefault();
                if (referrer == null)
                {
                    throw new InvalidReferrerException();
                }
                return referrer.GetDT(); 
            }
        }

        public static List<ReferrerInfoDT> GetReferrersInfo()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                List<ReferrerInfoDT> result = new List<ReferrerInfoDT>();
                foreach (Referrer referrer in entities.Referrers)
                {
                    ReferrerInfoDT referrerInfo = new ReferrerInfoDT();
                    referrerInfo.Id = referrer.Id;
                    referrerInfo.Name = referrer.Name;
                    referrerInfo.TotalVisits = 0;                    
                    referrerInfo.TotalSignups = referrer.Customers.Count;                    
                    referrerInfo.SignupPercentage = 0;

                    referrerInfo.FreeTrial = referrer.Customers.Where(c => c.LastBillDate == null).Count();
                    referrerInfo.Active = referrer.Customers.Where(c => c.Status == CustomerStatus.ACTIVE).Count();
                    referrerInfo.Cancels = referrer.Customers.Where(c => c.Status == CustomerStatus.CANCELLED).Count();
                    referrerInfo.Refunds = referrer.Customers.Where(c => c.Refunded == true).Count();

                    referrerInfo.CancellationPercentage = referrerInfo.TotalSignups > 0 ? referrerInfo.Cancels * 100 / referrerInfo.TotalSignups : 0;
                    referrerInfo.CancellationsRefundedPercentage = referrerInfo.Cancels > 0 ? referrerInfo.Refunds * 100 / referrerInfo.Cancels : 0;
                    referrerInfo.RefundPercentage = referrerInfo.TotalSignups > 0 ? referrerInfo.Refunds * 100 / referrerInfo.TotalSignups : 0;

                    referrerInfo.RevenueAmount = (double)referrer.RevenueAmount;//(double)referrer.Sites.Select(s => s.Customers.Select(c => c.Invoices.Where(i => i.Status == InvoiceStatus.BILLED || i.Status == InvoiceStatus.REFUNDED || i.Status == InvoiceStatus.REFUNDEDFAIL).Sum(i => i.Amount)).Sum()).Sum();
                    referrerInfo.RefundAmount = (double)referrer.RefundAmount;//(double)referrer.Sites.Select(s => s.Customers.Select(c => c.Invoices.Where(i => i.Status == InvoiceStatus.REFUNDED).Sum(i => i.Amount)).Sum()).Sum();
                    referrerInfo.BalanceAmount = referrerInfo.RevenueAmount - referrerInfo.RefundAmount;

                    List<ReferrerSalesInfoDT> today = GetReferrerSalesInfo(DateTime.Today, DateTime.Today, DateGroupBy.DAY, referrer.Id);
                    if (today.Count == 1)
                    {
                        referrerInfo.VisitsToday = today[0].Visits;
                        referrerInfo.SignupsToday = today[0].Signups;
                        referrerInfo.BilledToday = today[0].Billed;
                        referrerInfo.BilledTodayAmount = today[0].Profit;
                    }

                    List<ReferrerSalesInfoDT> yesterday = GetReferrerSalesInfo(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1), DateGroupBy.DAY, referrer.Id);
                    if (yesterday.Count == 1)
                    {
                        referrerInfo.VisitsYesterday = yesterday[0].Visits;
                        referrerInfo.SignupsYesterday = yesterday[0].Signups;
                        referrerInfo.BilledYesterday = yesterday[0].Billed;
                        referrerInfo.BilledYesterdayAmount = yesterday[0].Profit;
                    }

                    List<ReferrerSalesInfoDT> thisWeek = GetReferrerSalesInfo(DateTime.Today.AddDays(0 - (int)DateTime.Now.DayOfWeek), DateTime.Today, DateGroupBy.DAY, referrer.Id);
                    foreach (ReferrerSalesInfoDT item in thisWeek)
                    {
                        referrerInfo.VisitsThisWeek += item.Visits;
                        referrerInfo.SignupsThisWeek += item.Signups;
                        referrerInfo.BilledThisWeek += item.Billed;
                        referrerInfo.BilledThisWeekAmount += item.Profit;
                    }

                    List<ReferrerSalesInfoDT> thisMonth = GetReferrerSalesInfo(DateTime.Today.AddDays(1 - DateTime.Today.Day), DateTime.Today, DateGroupBy.MONTH, referrer.Id);
                    if (thisMonth.Count == 1)
                    {
                        referrerInfo.VisitsThisMonth += thisMonth[0].Visits;
                        referrerInfo.SignupsThisMonth = thisMonth[0].Signups;
                        referrerInfo.BilledThisMonth = thisMonth[0].Billed;
                        referrerInfo.BilledThisMonthAmount = thisMonth[0].Profit;
                    }

                    List<ReferrerSalesInfoDT> thisYear = GetReferrerSalesInfo(DateTime.Today.AddDays(1 - DateTime.Today.DayOfYear), DateTime.Today, DateGroupBy.YEAR, referrer.Id);
                    if (thisYear.Count == 1)
                    {
                        referrerInfo.VisitsThisYear = thisYear[0].Visits;
                        referrerInfo.SignupsThisYear = thisYear[0].Signups;
                        referrerInfo.BilledThisYear = thisYear[0].Billed;
                        referrerInfo.BilledThisYearAmount = thisYear[0].Profit;
                    }

                    referrerInfo.BilledTotal = referrer.BilledTotal; //referrer.Sites.Select(s => s.Customers.Select(c => c.Invoices.Where(i => i.Status == InvoiceStatus.BILLED || i.Status == InvoiceStatus.REFUNDED || i.Status == InvoiceStatus.REFUNDEDFAIL).Count()).Sum()).Sum();

                    referrerInfo.Cancellations_0_30 = entities.CancellationsViews.Where(c => c.ReferrerId == referrer.Id && c.CancelledDate.HasValue && c.CancelledDate >= c.CreatedAt && c.CancelledDate < c.CreatedAt30).Count();
                    referrerInfo.Cancellations_30_120 = entities.CancellationsViews.Where(c => c.ReferrerId == referrer.Id && c.CancelledDate.HasValue && c.CancelledDate >= c.CreatedAt30 && c.CancelledDate < c.CreatedAt120).Count();
                    referrerInfo.Cancellations_120_210 = entities.CancellationsViews.Where(c => c.ReferrerId == referrer.Id && c.CancelledDate.HasValue && c.CancelledDate >= c.CreatedAt120 && c.CancelledDate < c.CreatedAt210).Count();
                    referrerInfo.Cancellations_210 = entities.CancellationsViews.Where(c => c.ReferrerId == referrer.Id && c.CancelledDate.HasValue && c.CancelledDate >= c.CreatedAt210).Count();

                    referrerInfo.CancellationsPercentage_0_30 = referrerInfo.Cancels > 0 ? referrerInfo.Cancellations_0_30 * 100 / referrerInfo.Cancels : 0;
                    referrerInfo.CancellationsPercentage_30_120 = referrerInfo.Cancels > 0 ? referrerInfo.Cancellations_30_120 * 100 / referrerInfo.Cancels : 0;
                    referrerInfo.CancellationsPercentage_120_210 = referrerInfo.Cancels > 0 ? referrerInfo.Cancellations_120_210 * 100 / referrerInfo.Cancels : 0;
                    referrerInfo.CancellationsPercentage_210 = referrerInfo.Cancels > 0 ? referrerInfo.Cancellations_210 * 100 / referrerInfo.Cancels : 0;

                    referrerInfo.Refunds_30_120 = entities.RefundsViews.Where(c => c.ReferrerId == referrer.Id && c.RefundedDate.HasValue && c.RefundedDate >= c.CreatedAt30 && c.RefundedDate < c.CreatedAt120).Count();
                    referrerInfo.Refunds_120_210 = entities.RefundsViews.Where(c => c.ReferrerId == referrer.Id && c.RefundedDate.HasValue && c.RefundedDate >= c.CreatedAt120 && c.RefundedDate < c.CreatedAt210).Count();
                    referrerInfo.Refunds_210 = entities.RefundsViews.Where(c => c.ReferrerId == referrer.Id && c.RefundedDate.HasValue && c.RefundedDate >= c.CreatedAt210).Count();

                    var refundsAmount_30_120 = entities.RefundsViews.Where(c => c.ReferrerId == referrer.Id && c.RefundedDate.HasValue && c.RefundedDate >= c.CreatedAt30 && c.RefundedDate < c.CreatedAt120);
                    referrerInfo.RefundsAmount_30_120 = refundsAmount_30_120.FirstOrDefault() != null ? refundsAmount_30_120.Sum(i => i.Amount) : 0;
                    var refundsAmount_120_210 = entities.RefundsViews.Where(c => c.ReferrerId == referrer.Id && c.RefundedDate.HasValue && c.RefundedDate >= c.CreatedAt120 && c.RefundedDate < c.CreatedAt210);
                    referrerInfo.RefundsAmount_120_210 = refundsAmount_120_210.FirstOrDefault() != null ? refundsAmount_120_210.Sum(i => i.Amount) : 0;
                    var refundsAmount_210 = entities.RefundsViews.Where(c => c.ReferrerId == referrer.Id && c.RefundedDate.HasValue && c.RefundedDate >= c.CreatedAt210);
                    referrerInfo.RefundsAmount_210 = refundsAmount_210.FirstOrDefault() != null ? refundsAmount_210.Sum(i => i.Amount) : 0;

                    referrerInfo.RefundsPercentage_30_120 = referrerInfo.Refunds > 0 ? referrerInfo.Refunds_30_120 * 100 / referrerInfo.Refunds : 0;
                    referrerInfo.RefundsPercentage_120_210 = referrerInfo.Refunds > 0 ? referrerInfo.Refunds_120_210 * 100 / referrerInfo.Refunds : 0;
                    referrerInfo.RefundsPercentage_210 = referrerInfo.Refunds > 0 ? referrerInfo.Refunds_210 * 100 / referrerInfo.Refunds : 0;

                    result.Add(referrerInfo);
                }

                result.Add(GetSumReferrersInfo(result));

                result.Reverse();

                return result;
            }
        }

        public static ReferrerInfoDT GetSumReferrersInfo(List<ReferrerInfoDT> list)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                ReferrerInfoDT referrerInfo = new ReferrerInfoDT();

                referrerInfo.Id = 0;
                referrerInfo.Name = "All Websites";
                referrerInfo.TotalVisits = entities.Sites.Sum(x => x.TotalVisits);
                referrerInfo.TotalSignups = list.Sum(x => x.TotalSignups);
                referrerInfo.SignupPercentage = referrerInfo.TotalVisits > 0 ? ((double)referrerInfo.TotalSignups * 100) / ((double)referrerInfo.TotalVisits) : 0;

                referrerInfo.FreeTrial = list.Sum(i => i.FreeTrial);
                referrerInfo.Active = list.Sum(i => i.Active);
                referrerInfo.Cancels = list.Sum(i => i.Cancels);
                referrerInfo.Refunds = list.Sum(i => i.Refunds);

                referrerInfo.CancellationPercentage = referrerInfo.TotalSignups > 0 ? ((double)referrerInfo.Cancels * 100) / ((double)referrerInfo.TotalSignups) : 0;
                referrerInfo.CancellationsRefundedPercentage = referrerInfo.Cancels > 0 ? ((double)referrerInfo.Refunds * 100) / ((double)referrerInfo.Cancels) : 0;
                referrerInfo.RefundPercentage = referrerInfo.TotalSignups > 0 ? ((double)referrerInfo.Refunds * 100) / ((double)referrerInfo.TotalSignups) : 0;

                referrerInfo.RevenueAmount = list.Sum(i => i.RevenueAmount);
                referrerInfo.RefundAmount = list.Sum(i => i.RefundAmount);
                referrerInfo.BalanceAmount = referrerInfo.RevenueAmount - referrerInfo.RefundAmount;

                referrerInfo.SignupsToday = list.Sum(i => i.SignupsToday);
                referrerInfo.BilledToday = list.Sum(i => i.BilledToday);
                referrerInfo.BilledTodayAmount = list.Sum(i => i.BilledTodayAmount);

                referrerInfo.SignupsYesterday = list.Sum(i => i.SignupsYesterday);
                referrerInfo.BilledYesterday = list.Sum(i => i.BilledYesterday);
                referrerInfo.BilledYesterdayAmount = list.Sum(i => i.BilledYesterdayAmount);

                referrerInfo.SignupsThisWeek = list.Sum(i => i.SignupsThisWeek);
                referrerInfo.BilledThisWeek = list.Sum(i => i.BilledThisWeek);
                referrerInfo.BilledThisWeekAmount = list.Sum(i => i.BilledThisWeekAmount);

                referrerInfo.SignupsThisMonth = list.Sum(i => i.SignupsThisMonth);
                referrerInfo.BilledThisMonth = list.Sum(i => i.BilledThisMonth);
                referrerInfo.BilledThisMonthAmount = list.Sum(i => i.BilledThisMonthAmount);

                referrerInfo.SignupsThisYear = list.Sum(i => i.SignupsThisYear);
                referrerInfo.BilledThisYear = list.Sum(i => i.BilledThisYear);
                referrerInfo.BilledThisYearAmount = list.Sum(i => i.BilledThisYearAmount);

                referrerInfo.BilledTotal = list.Sum(i => i.BilledTotal);

                referrerInfo.Cancellations_0_30 = list.Sum(i => i.Cancellations_0_30);
                referrerInfo.Cancellations_30_120 = list.Sum(i => i.Cancellations_30_120);
                referrerInfo.Cancellations_120_210 = list.Sum(i => i.Cancellations_120_210);
                referrerInfo.Cancellations_210 = list.Sum(i => i.Cancellations_210);

                referrerInfo.CancellationsPercentage_0_30 = referrerInfo.Cancels > 0 ? ((decimal)referrerInfo.Cancellations_0_30 * 100) / ((decimal)referrerInfo.Cancels) : 0;
                referrerInfo.CancellationsPercentage_30_120 = referrerInfo.Cancels > 0 ? ((double)referrerInfo.Cancellations_30_120 * 100) / ((double)referrerInfo.Cancels) : 0;
                referrerInfo.CancellationsPercentage_120_210 = referrerInfo.Cancels > 0 ? ((double)referrerInfo.Cancellations_120_210 * 100) / ((double)referrerInfo.Cancels) : 0;
                referrerInfo.CancellationsPercentage_210 = referrerInfo.Cancels > 0 ? ((double)referrerInfo.Cancellations_210 * 100) / ((double)referrerInfo.Cancels) : 0;

                referrerInfo.Refunds_30_120 = list.Sum(i => i.Refunds_30_120);
                referrerInfo.Refunds_120_210 = list.Sum(i => i.Refunds_120_210);
                referrerInfo.Refunds_210 = list.Sum(i => i.Refunds_210);

                referrerInfo.RefundsAmount_30_120 = list.Sum(i => i.RefundsAmount_30_120);
                referrerInfo.RefundsAmount_120_210 = list.Sum(i => i.RefundsAmount_120_210);
                referrerInfo.RefundsAmount_210 = list.Sum(i => i.RefundsAmount_210);

                referrerInfo.RefundsPercentage_30_120 = referrerInfo.Refunds > 0 ? ((double)referrerInfo.Refunds_30_120 * 100) / ((double)referrerInfo.Refunds) : 0;
                referrerInfo.RefundsPercentage_120_210 = referrerInfo.Refunds > 0 ? ((double)referrerInfo.Refunds_120_210 * 100) / ((double)referrerInfo.Refunds) : 0;
                referrerInfo.RefundsPercentage_210 = referrerInfo.Refunds > 0 ? ((double)referrerInfo.Refunds_210 * 100) / ((double)referrerInfo.Refunds) : 0;

                return referrerInfo;
            }
        }

        public static List<ReferrerSalesInfoDT> GetReferrerSalesInfo(DateTime startDate, DateTime endDate, DateGroupBy groupBy, int referrerId)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                bool all = false;

                if (referrerId != 0)
                {
                    Referrer referrer = entities.Referrers.Where(r => r.Id == referrerId).FirstOrDefault();
                    if (referrer == null)
                    {
                        throw new InvalidReferrerException();
                    }
                }
                else
                {
                    all = true;
                }

                List<ReferrerSalesInfoDT> salesInfoList = new List<ReferrerSalesInfoDT>();

                DateTime auxDate = startDate;
                while (auxDate <= endDate || (groupBy == DateGroupBy.MONTH && auxDate.Month == endDate.Month) || (groupBy == DateGroupBy.YEAR && auxDate.Year == endDate.Year))
                {
                    ReferrerSalesInfoDT item = new ReferrerSalesInfoDT();
                    switch (groupBy)
                    {
                        case DateGroupBy.DAY:
                            item.Date = new DateTime(auxDate.Year, auxDate.Month, auxDate.Day);
                            auxDate = auxDate.AddDays(1);
                            break;
                        case DateGroupBy.MONTH:
                            item.Date = new DateTime(auxDate.Year, auxDate.Month, 1);
                            auxDate = auxDate.AddMonths(1);
                            break;
                        case DateGroupBy.YEAR:
                            item.Date = new DateTime(auxDate.Year, 1, 1);
                            auxDate = auxDate.AddYears(1);
                            break;
                    }
                    salesInfoList.Add(item);
                }                            
                var visits = (
                    from c in entities.Visits
                    where c.CreatedAt >= startDate.Date && c.CreatedAt <= endDate.Date
                    select c);

                var signups = (
                    from c in entities.CustomersViews
                    where (c.ReferrerId == referrerId || all) && c.CreatedAt >= startDate.Date && c.CreatedAt <= endDate.Date
                    select c);

                var cancelled = (
                    from c in entities.CustomersViews
                    where (c.ReferrerId == referrerId || all) && c.CancelledDate.HasValue && c.CancelledDate >= startDate && c.CancelledDate <= endDate
                    select c);

                var billed = (
                    from i in entities.InvoicesViews
                    where (i.ReferrerId == referrerId || all) && i.BilledDate.HasValue && i.BilledDate >= startDate && i.BilledDate <= endDate && (i.StatusId == InvoiceStatus.BILLED.ToString() || i.StatusId == InvoiceStatus.REFUNDED.ToString() || i.StatusId == InvoiceStatus.REFUNDEDFAIL.ToString())
                    select i);

                var refunded = (
                    from i in entities.InvoicesViews
                    where (i.ReferrerId == referrerId || all) && i.RefundedDate.HasValue && i.RefundedDate >= startDate && i.RefundedDate <= endDate && i.StatusId == InvoiceStatus.REFUNDED.ToString()
                    select i);

                List<DateWithDataDT> visitsList = new List<DateWithDataDT>();
                List<DateWithDataDT> signupsList = new List<DateWithDataDT>();
                List<DateWithDataDT> cancelledList = new List<DateWithDataDT>();
                List<DateWithDataDT> billedList = new List<DateWithDataDT>();
                List<DateWithDataDT> billedAmountList = new List<DateWithDataDT>();
                List<DateWithDataDT> refundedList = new List<DateWithDataDT>();
                List<DateWithDataDT> refundedAmountList = new List<DateWithDataDT>();

                switch (groupBy)
                {
                    case DateGroupBy.DAY:
                        visitsList = visits.GroupBy(c => new { Day = c.CreatedAt.Day, Month = c.CreatedAt.Month, Year = c.CreatedAt.Year }).Select(c => new DateWithDataDT { Day = c.Key.Day, Month = c.Key.Month, Year = c.Key.Year, Data = c.Count() }).ToList();
                        signupsList = signups.GroupBy(c => new { Day = c.CreatedAtDay, Month = c.CreatedAtMonth, Year = c.CreatedAtYear }).Select(c => new DateWithDataDT { Day = c.Key.Day.Value, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        cancelledList = cancelled.GroupBy(c => new { Day = c.CancelledDay, Month = c.CancelledMonth, Year = c.CancelledYear }).Select(c => new DateWithDataDT { Day = c.Key.Day.Value, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        billedList = billed.GroupBy(c => new { Day = c.BilledDay, Month = c.BilledMonth, Year = c.BilledYear }).Select(c => new DateWithDataDT { Day = c.Key.Day.Value, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        billedAmountList = billed.GroupBy(c => new { Day = c.BilledDay, Month = c.BilledMonth, Year = c.BilledYear }).Select(c => new DateWithDataDT { Day = c.Key.Day.Value, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Sum(i => i.Amount) }).ToList();
                        refundedList = refunded.GroupBy(c => new { Day = c.RefundedDay, Month = c.RefundedMonth, Year = c.RefundedYear }).Select(c => new DateWithDataDT { Day = c.Key.Day.Value, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        refundedAmountList = refunded.GroupBy(c => new { Day = c.RefundedDay, Month = c.RefundedMonth, Year = c.RefundedYear }).Select(c => new DateWithDataDT { Day = c.Key.Day.Value, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Sum(i => i.Amount) }).ToList();
                        break;
                    case DateGroupBy.MONTH:
                        visitsList = visits.GroupBy(c => new { Month = c.CreatedAt.Month, Year = c.CreatedAt.Year }).Select(c => new DateWithDataDT { Day = 1, Month = c.Key.Month, Year = c.Key.Year, Data = c.Count() }).ToList();
                        signupsList = signups.GroupBy(c => new { Month = c.CreatedAtMonth, Year = c.CreatedAtYear }).Select(c => new DateWithDataDT { Day = 1, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        cancelledList = cancelled.GroupBy(c => new { Month = c.CancelledMonth, Year = c.CancelledYear }).Select(c => new DateWithDataDT { Day = 1, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        billedList = billed.GroupBy(c => new { Month = c.BilledMonth, Year = c.BilledYear }).Select(c => new DateWithDataDT { Day = 1, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        billedAmountList = billed.GroupBy(c => new { Month = c.BilledMonth, Year = c.BilledYear }).Select(c => new DateWithDataDT { Day = 1, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Sum(i => i.Amount) }).ToList();
                        refundedList = refunded.GroupBy(c => new { Month = c.RefundedMonth, Year = c.RefundedYear }).Select(c => new DateWithDataDT { Day = 1, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        refundedAmountList = refunded.GroupBy(c => new { Month = c.RefundedMonth, Year = c.RefundedYear }).Select(c => new DateWithDataDT { Day = 1, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Sum(i => i.Amount) }).ToList();
                        break;
                    case DateGroupBy.YEAR:
                        visitsList = visits.GroupBy(c => new { Year = c.CreatedAt.Year }).Select(c => new DateWithDataDT { Day = 1, Month = 1, Year = c.Key.Year, Data = c.Count() }).ToList();
                        signupsList = signups.GroupBy(c => new { Year = c.CreatedAtYear }).Select(c => new DateWithDataDT { Day = 1, Month = 1, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        cancelledList = cancelled.GroupBy(c => new { Year = c.CancelledYear }).Select(c => new DateWithDataDT { Day = 1, Month = 1, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        billedList = billed.GroupBy(c => new { Year = c.BilledYear }).Select(c => new DateWithDataDT { Day = 1, Month = 1, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        billedAmountList = billed.GroupBy(c => new { Year = c.BilledYear }).Select(c => new DateWithDataDT { Day = 1, Month = 1, Year = c.Key.Year.Value, Data = c.Sum(i => i.Amount) }).ToList();
                        refundedList = refunded.GroupBy(c => new { Year = c.RefundedYear }).Select(c => new DateWithDataDT { Day = 1, Month = 1, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        refundedAmountList = refunded.GroupBy(c => new { Year = c.RefundedYear }).Select(c => new DateWithDataDT { Day = 1, Month = 1, Year = c.Key.Year.Value, Data = c.Sum(i => i.Amount) }).ToList();
                        break;
                }

                foreach (ReferrerSalesInfoDT item in salesInfoList)
                {
                    int day = item.Date.Day;
                    int month = item.Date.Month;
                    int year = item.Date.Year;

                    switch (groupBy)
                    {
                        case DateGroupBy.DAY:
                            item.DateString = item.Date.ToString("MM-dd-yyyy");
                            break;
                        case DateGroupBy.MONTH:
                            item.DateString = item.Date.ToString("MM-yyyy");
                            day = 1;
                            break;
                        case DateGroupBy.YEAR:
                            item.DateString = item.Date.ToString("yyyy");
                            day = 1;
                            month = 1;
                            break;
                    }
                    DateWithDataDT visitsData = visitsList.Where(d => d.Day == day && d.Month == month && d.Year == year).FirstOrDefault();
                    item.Visits = visitsData != null ? Convert.ToInt32(visitsData.Data) : 0;
                    DateWithDataDT signupsData = signupsList.Where(d => d.Day == day && d.Month == month && d.Year == year).FirstOrDefault();
                    item.Signups = signupsData != null ? Convert.ToInt32(signupsData.Data) : 0;
                    DateWithDataDT cancelledData = cancelledList.Where(d => d.Day == day && d.Month == month && d.Year == year).FirstOrDefault();
                    item.Cancelled = cancelledData != null ? Convert.ToInt32(cancelledData.Data) : 0;
                    DateWithDataDT billedData = billedList.Where(d => d.Day == day && d.Month == month && d.Year == year).FirstOrDefault();
                    item.Billed = billedData != null ? Convert.ToInt32(billedData.Data) : 0;
                    DateWithDataDT billedAmountData = billedAmountList.Where(d => d.Day == day && d.Month == month && d.Year == year).FirstOrDefault();
                    item.Profit = billedAmountData != null ? billedAmountData.Data : 0;
                    DateWithDataDT refundedData = refundedList.Where(d => d.Day == day && d.Month == month && d.Year == year).FirstOrDefault();
                    item.Refunded = refundedData != null ? Convert.ToInt32(refundedData.Data) : 0;
                    DateWithDataDT refundedAmountData = refundedAmountList.Where(d => d.Day == day && d.Month == month && d.Year == year).FirstOrDefault();
                    item.Refund = refundedAmountData != null ? refundedAmountData.Data : 0;
                    item.Revenue = item.Profit - item.Refund;
                }

                return salesInfoList;
            }
        }

        public static List<ReferrerSalesInfoDT> GetReferrerSiteSalesInfo(DateTime startDate, DateTime endDate, DateGroupBy groupBy, int referrerId, int siteId)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                bool allReferrer = false;
                bool allSites = false;

                if (referrerId != 0)
                {
                    Referrer referrer = entities.Referrers.Where(r => r.Id == referrerId).FirstOrDefault();
                    if (referrer == null)
                    {
                        throw new InvalidReferrerException();
                    }
                }
                else
                {
                    allReferrer = true;
                }

                if (siteId != 0)
                {
                    if (entities.Sites.FirstOrDefault(x => x.Id == siteId) == null)
                    {
                        throw new InvalidSiteException();
                    }
                }
                else
                {
                    allSites = true;
                }

                List<ReferrerSalesInfoDT> salesInfoList = new List<ReferrerSalesInfoDT>();

                DateTime auxDate = startDate;
                DateTime endDateVisits = endDate.Date.AddDays(1).AddSeconds(-1);
                while (auxDate <= endDate || (groupBy == DateGroupBy.MONTH && auxDate.Month == endDate.Month) || (groupBy == DateGroupBy.YEAR && auxDate.Year == endDate.Year))
                {
                    ReferrerSalesInfoDT item = new ReferrerSalesInfoDT();
                    switch (groupBy)
                    {
                        case DateGroupBy.DAY:
                            item.Date = new DateTime(auxDate.Year, auxDate.Month, auxDate.Day);
                            auxDate = auxDate.AddDays(1);
                            break;
                        case DateGroupBy.MONTH:
                            item.Date = new DateTime(auxDate.Year, auxDate.Month, 1);
                            auxDate = auxDate.AddMonths(1);
                            break;
                        case DateGroupBy.YEAR:
                            item.Date = new DateTime(auxDate.Year, 1, 1);
                            auxDate = auxDate.AddYears(1);
                            break;
                    }
                    salesInfoList.Add(item);
                }
                var visits = (
                    from c in entities.Visits
                    where (c.SiteId == siteId || allSites) && (c.CreatedAt >= startDate.Date &&
                    c.CreatedAt <= endDateVisits)
                    select c);                 

                var signups = (
                    from c in entities.CustomersViews
                    where (c.ReferrerId == referrerId || allReferrer) && (c.SiteId == siteId || allSites) && c.CreatedAt >= startDate.Date && c.CreatedAt <= endDate.Date
                    select c);

                var cancelled = (
                    from c in entities.CustomersViews
                    where (c.ReferrerId == referrerId || allReferrer) && (c.SiteId == siteId || allSites) && c.CancelledDate.HasValue && c.CancelledDate >= startDate && c.CancelledDate <= endDate
                    select c);

                var billed = (
                    from i in entities.InvoicesViews
                    where (i.ReferrerId == referrerId || allReferrer) && (i.SiteId == siteId || allSites) && i.BilledDate.HasValue && i.BilledDate >= startDate && i.BilledDate <= endDate && (i.StatusId == InvoiceStatus.BILLED.ToString() || i.StatusId == InvoiceStatus.REFUNDED.ToString() || i.StatusId == InvoiceStatus.REFUNDEDFAIL.ToString())
                    select i);

                var refunded = (
                    from i in entities.InvoicesViews
                    where (i.ReferrerId == referrerId || allReferrer) && (i.SiteId == siteId || allSites) && i.RefundedDate.HasValue && i.RefundedDate >= startDate && i.RefundedDate <= endDate && i.StatusId == InvoiceStatus.REFUNDED.ToString()
                    select i);

                List<DateWithDataDT> visitsList = new List<DateWithDataDT>();
                List<DateWithDataDT> signupsList = new List<DateWithDataDT>();
                List<DateWithDataDT> cancelledList = new List<DateWithDataDT>();
                List<DateWithDataDT> billedList = new List<DateWithDataDT>();
                List<DateWithDataDT> billedAmountList = new List<DateWithDataDT>();
                List<DateWithDataDT> refundedList = new List<DateWithDataDT>();
                List<DateWithDataDT> refundedAmountList = new List<DateWithDataDT>();

                switch (groupBy)
                {                    
                    case DateGroupBy.DAY:
                        visitsList = visits.GroupBy(c => new { Day = c.CreatedAt.Day, Month = c.CreatedAt.Month, Year = c.CreatedAt.Year }).Select(c => new DateWithDataDT { Day = c.Key.Day, Month = c.Key.Month, Year = c.Key.Year, Data = c.Count() }).ToList();
                        signupsList = signups.GroupBy(c => new { Day = c.CreatedAtDay, Month = c.CreatedAtMonth, Year = c.CreatedAtYear }).Select(c => new DateWithDataDT { Day = c.Key.Day.Value, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        cancelledList = cancelled.GroupBy(c => new { Day = c.CancelledDay, Month = c.CancelledMonth, Year = c.CancelledYear }).Select(c => new DateWithDataDT { Day = c.Key.Day.Value, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        billedList = billed.GroupBy(c => new { Day = c.BilledDay, Month = c.BilledMonth, Year = c.BilledYear }).Select(c => new DateWithDataDT { Day = c.Key.Day.Value, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        billedAmountList = billed.GroupBy(c => new { Day = c.BilledDay, Month = c.BilledMonth, Year = c.BilledYear }).Select(c => new DateWithDataDT { Day = c.Key.Day.Value, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Sum(i => i.Amount) }).ToList();
                        refundedList = refunded.GroupBy(c => new { Day = c.RefundedDay, Month = c.RefundedMonth, Year = c.RefundedYear }).Select(c => new DateWithDataDT { Day = c.Key.Day.Value, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        refundedAmountList = refunded.GroupBy(c => new { Day = c.RefundedDay, Month = c.RefundedMonth, Year = c.RefundedYear }).Select(c => new DateWithDataDT { Day = c.Key.Day.Value, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Sum(i => i.Amount) }).ToList();
                        break;
                    case DateGroupBy.MONTH:
                        visitsList = visits.GroupBy(c => new { Month = c.CreatedAt.Month, Year = c.CreatedAt.Year }).Select(c => new DateWithDataDT { Day = 1, Month = c.Key.Month, Year = c.Key.Year, Data = c.Count() }).ToList();
                        signupsList = signups.GroupBy(c => new { Month = c.CreatedAtMonth, Year = c.CreatedAtYear }).Select(c => new DateWithDataDT { Day = 1, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        cancelledList = cancelled.GroupBy(c => new { Month = c.CancelledMonth, Year = c.CancelledYear }).Select(c => new DateWithDataDT { Day = 1, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        billedList = billed.GroupBy(c => new { Month = c.BilledMonth, Year = c.BilledYear }).Select(c => new DateWithDataDT { Day = 1, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        billedAmountList = billed.GroupBy(c => new { Month = c.BilledMonth, Year = c.BilledYear }).Select(c => new DateWithDataDT { Day = 1, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Sum(i => i.Amount) }).ToList();
                        refundedList = refunded.GroupBy(c => new { Month = c.RefundedMonth, Year = c.RefundedYear }).Select(c => new DateWithDataDT { Day = 1, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        refundedAmountList = refunded.GroupBy(c => new { Month = c.RefundedMonth, Year = c.RefundedYear }).Select(c => new DateWithDataDT { Day = 1, Month = c.Key.Month.Value, Year = c.Key.Year.Value, Data = c.Sum(i => i.Amount) }).ToList();
                        break;
                    case DateGroupBy.YEAR:
                        visitsList = visits.GroupBy(c => new { Year = c.CreatedAt.Year }).Select(c => new DateWithDataDT { Day = 1, Month = 1, Year = c.Key.Year, Data = c.Count() }).ToList();
                        signupsList = signups.GroupBy(c => new { Year = c.CreatedAtYear }).Select(c => new DateWithDataDT { Day = 1, Month = 1, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        cancelledList = cancelled.GroupBy(c => new { Year = c.CancelledYear }).Select(c => new DateWithDataDT { Day = 1, Month = 1, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        billedList = billed.GroupBy(c => new { Year = c.BilledYear }).Select(c => new DateWithDataDT { Day = 1, Month = 1, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        billedAmountList = billed.GroupBy(c => new { Year = c.BilledYear }).Select(c => new DateWithDataDT { Day = 1, Month = 1, Year = c.Key.Year.Value, Data = c.Sum(i => i.Amount) }).ToList();
                        refundedList = refunded.GroupBy(c => new { Year = c.RefundedYear }).Select(c => new DateWithDataDT { Day = 1, Month = 1, Year = c.Key.Year.Value, Data = c.Count() }).ToList();
                        refundedAmountList = refunded.GroupBy(c => new { Year = c.RefundedYear }).Select(c => new DateWithDataDT { Day = 1, Month = 1, Year = c.Key.Year.Value, Data = c.Sum(i => i.Amount) }).ToList();
                        break;
                }

                foreach (ReferrerSalesInfoDT item in salesInfoList)
                {
                    int day = item.Date.Day;
                    int month = item.Date.Month;
                    int year = item.Date.Year;

                    switch (groupBy)
                    {
                        case DateGroupBy.DAY:
                            item.DateString = item.Date.ToString("MM-dd-yyyy");
                            break;
                        case DateGroupBy.MONTH:
                            item.DateString = item.Date.ToString("MM-yyyy");
                            day = 1;
                            break;
                        case DateGroupBy.YEAR:
                            item.DateString = item.Date.ToString("yyyy");
                            day = 1;
                            month = 1;
                            break;
                    }

                    DateWithDataDT visitsData = visitsList.Where(d => d.Day == day && d.Month == month && d.Year == year).FirstOrDefault();
                    item.Visits = visitsData != null ? Convert.ToInt32(visitsData.Data) : 0;
                    DateWithDataDT signupsData = signupsList.Where(d => d.Day == day && d.Month == month && d.Year == year).FirstOrDefault();
                    item.Signups = signupsData != null ? Convert.ToInt32(signupsData.Data) : 0;
                    DateWithDataDT cancelledData = cancelledList.Where(d => d.Day == day && d.Month == month && d.Year == year).FirstOrDefault();
                    item.Cancelled = cancelledData != null ? Convert.ToInt32(cancelledData.Data) : 0;
                    DateWithDataDT billedData = billedList.Where(d => d.Day == day && d.Month == month && d.Year == year).FirstOrDefault();
                    item.Billed = billedData != null ? Convert.ToInt32(billedData.Data) : 0;
                    DateWithDataDT billedAmountData = billedAmountList.Where(d => d.Day == day && d.Month == month && d.Year == year).FirstOrDefault();
                    item.Profit = billedAmountData != null ? billedAmountData.Data : 0;
                    DateWithDataDT refundedData = refundedList.Where(d => d.Day == day && d.Month == month && d.Year == year).FirstOrDefault();
                    item.Refunded = refundedData != null ? Convert.ToInt32(refundedData.Data) : 0;
                    DateWithDataDT refundedAmountData = refundedAmountList.Where(d => d.Day == day && d.Month == month && d.Year == year).FirstOrDefault();
                    item.Refund = refundedAmountData != null ? refundedAmountData.Data : 0;
                    item.Revenue = item.Profit - item.Refund;
                }

                return salesInfoList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="referrerId">With SiteId equal to 0, is for all sites</param>
        /// <param name="activeUser"></param>
        /// <param name="stratDate"></param>
        /// <param name="endDate"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Dictionary<string, List<EmailReportDT>> GetEmailReport(int referrerId, bool activeUser, DateTime stratDate, DateTime endDate)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                Dictionary<string, List<EmailReportDT>> emailReportDictionary = new Dictionary<string, List<EmailReportDT>>();
                IEnumerable<Customer> listInvoice = null;

                if (activeUser)
                {
                    listInvoice = entities.Customers.Where(i => referrerId == 0 || i.ReferrerId == referrerId);
                }
                else
                {
                    DateTime newEndDate = endDate.AddDays(1);
                    listInvoice = entities.Customers.Where(i => (referrerId == 0 || i.ReferrerId == referrerId)
                        && (stratDate.CompareTo(i.CreatedAt) <= 0 && newEndDate.CompareTo(i.CreatedAt) >= 0));
                }

                foreach (var item in listInvoice)
                {
                    var creditCard = item.CreditCards.FirstOrDefault();
                    if (creditCard != null)
                    {
                        EmailReportDT emailReport = new EmailReportDT()
                        {
                            CustomerID = item.Id.ToString(),
                            CustomerPass = "********",
                            DateSent = item.CreatedAt.ToString(),
                            Email = item.Email,
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            Zip = item.ZipCode,
                            CreditCardType = creditCard.Type,
                            EmailForm = item.EmailForm
                        };
                        if (referrerId == 0)
                        {
                            if (emailReportDictionary.ContainsKey("All Websites"))
                            {
                                emailReportDictionary["All Websites"].Add(emailReport);
                            }
                            else
                            {
                                emailReportDictionary.Add("All Websites", new List<EmailReportDT>() { emailReport });
                            }
                        }

                        if (emailReportDictionary.ContainsKey(item.Referrer.Name))
                        {
                            emailReportDictionary[item.Referrer.Name].Add(emailReport);
                        }
                        else
                        {
                            emailReportDictionary.Add(item.Referrer.Name, new List<EmailReportDT>() { emailReport });
                        }
                    }
                }

                return emailReportDictionary;
            }
        }

        /// <summary>
        /// Active new offer, and disabled the old active offer.
        /// </summary>
        /// <param name="siteID">Site ID.</param>
        /// <returns>Return true if all ok, or return false if was an error.</returns>
        public static bool SetActiveSite(int siteID)
        {
            try
            {
                using (CastleClubEntities entites = new CastleClubEntities())
                {
                    var site = entites.Sites.FirstOrDefault(x => x.Id == siteID);
                    if (site != null)
                    {
                        foreach (var sites in entites.Sites)
                        {
                            sites.Active = false;
                        }
                        site.Active = true;

                        entites.SaveChanges();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Region charts reports
        /// <summary>
        /// Report for char. For the last four day in the last four week.
        /// </summary>
        /// <param name="siteID">SiteID 0 for all website, or website ID.</param>
        /// <returns>In double[] in index "0" is revenue, in index "1" is cost.</returns>
        public static Dictionary<DateTime, double[]> GetSiteReportTheLastFourDay(int siteID)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                DateTime now = DateTime.Now.Date;

                Dictionary<DateTime, double[]> response = new Dictionary<DateTime, double[]>();

                var revenue = entities.ReportTheLastFourDaysRevenue(now.AddDays(-1), now.AddDays(-8), now.AddDays(-15), now.AddDays(-22), siteID);
                var cost = entities.ReportTheLastFourDaysCost(now.AddDays(-1), now.AddDays(-8), now.AddDays(-15), now.AddDays(-22), siteID);

                foreach (var item in revenue)
                {
                    var values = new double[2];
                    values[0] = (double)item.Amount;
                    values[1] = 0.00;
                    response.Add(item.BilledDate.Value, values);
                }

                foreach (var item in cost)
                {
                    if (response.Keys.Contains(item.RefundedDate.Value))
                    {
                        var cell = response[item.RefundedDate.Value];
                        cell[1] = (double)item.Amount;
                    }
                    else
                    {
                        var values = new double[2];
                        values[0] = 0.00;
                        values[1] = (double)item.Amount;
                        response.Add(item.RefundedDate.Value, values);
                    }
                }

                return Utils.Dictionary.SortByKey(response);
            }
        }
        /// <summary>
        /// Report for char. For the last four months in the first six months day.
        /// </summary>
        /// <param name="siteID">SiteID 0 for all website, or website ID.</param>
        /// <returns>In double[] in index "0" is revenue, in index "1" is cost.</returns>
        public static Dictionary<DateTime, double[]> GetSiteReportTheLastFourMonth(int siteID)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                DateTime now = DateTime.Now.Date;
                int daysCount = 6;

                Dictionary<DateTime, double[]> response = new Dictionary<DateTime, double[]>();

                var revenue = entities.ReportTheLastFourMonthRevenue(
                    new DateTime(now.Year, now.Month, 1), new DateTime(now.Year, now.Month, daysCount),
                    new DateTime(now.AddMonths(-1).Year, now.AddMonths(-1).Month, 1), new DateTime(now.AddMonths(-1).Year, now.AddMonths(-1).Month, daysCount),
                    new DateTime(now.AddMonths(-2).Year, now.AddMonths(-2).Month, 1), new DateTime(now.AddMonths(-2).Year, now.AddMonths(-2).Month, daysCount),
                    new DateTime(now.AddMonths(-3).Year, now.AddMonths(-3).Month, 1), new DateTime(now.AddMonths(-3).Year, now.AddMonths(-3).Month, daysCount),
                    siteID);
                var cost = entities.ReportTheLastFourMonthCost(
                    new DateTime(now.Year, now.Month, 1), new DateTime(now.Year, now.Month, 6),
                    new DateTime(now.AddMonths(-1).Year, now.AddMonths(-1).Month, 1), new DateTime(now.AddMonths(-1).Year, now.AddMonths(-1).Month, daysCount),
                    new DateTime(now.AddMonths(-2).Year, now.AddMonths(-2).Month, 1), new DateTime(now.AddMonths(-2).Year, now.AddMonths(-2).Month, daysCount),
                    new DateTime(now.AddMonths(-3).Year, now.AddMonths(-3).Month, 1), new DateTime(now.AddMonths(-3).Year, now.AddMonths(-3).Month, daysCount),
                    siteID);

                //Here is necessary remove one to year.
                bool yearChange = now.Month <= 3;

                foreach (var item in revenue)
                {
                    var values = new double[2];
                    values[0] = (double)item.Amount;
                    values[1] = 0.00;
                    int year = (yearChange && item.BilledDate.Value >= 9) ? now.Year - 1 : now.Year;
                    response.Add(Convert.ToDateTime(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.BilledDate.Value) + " - " + year), values);
                }

                foreach (var item in cost)
                {
                    int year = (yearChange && item.RefundedDate.Value >= 9) ? now.Year - 1 : now.Year;
                    DateTime month = Convert.ToDateTime(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.RefundedDate.Value) + " - " + year);
                    if (response.Keys.Contains(month))
                    {
                        var cell = response[month];
                        cell[1] = (double)item.Amount;
                    }
                    else
                    {
                        var values = new double[2];
                        values[0] = 0.00;
                        values[1] = (double)item.Amount;
                        response.Add(month, values);
                    }
                }

                return Utils.Dictionary.SortByKey(response);
            }
        }
        /// <summary>
        /// Report for char. For the last four months in the first six months day.
        /// </summary>
        /// <param name="siteID">SiteID 0 for all website, or website ID.</param>
        /// <returns>In double[] in index "0" is revenue, in index "1" is cost.</returns>
        public static Dictionary<DateTime, double[]> GetSiteReportTheLastFifteenDays(int siteID)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                DateTime now = DateTime.Now.Date;

                Dictionary<DateTime, double[]> response = new Dictionary<DateTime, double[]>();

                var revenue = entities.ReportTheLastFifteenDaysRevenue(now.AddDays(-15), now.AddDays(1), siteID);
                var cost = entities.ReportTheLastFifteenDaysCost(now.AddDays(-15), now.AddDays(1), siteID);

                bool yearChange = now.Month <= 3;

                foreach (var item in revenue)
                {
                    var values = new double[2];
                    values[0] = (double)item.Amount;
                    values[1] = 0.00;
                    response.Add(item.BilledDate.Value, values);
                }

                foreach (var item in cost)
                {
                    if (response.Keys.Contains(item.RefundedDate.Value))
                    {
                        var cell = response[item.RefundedDate.Value];
                        cell[1] = (double)item.Amount;
                    }
                    else
                    {
                        var values = new double[2];
                        values[0] = 0.00;
                        values[1] = (double)item.Amount;
                        response.Add(item.RefundedDate.Value, values);
                    }
                }

                return Utils.Dictionary.SortByKey(response);
            }
        }
        #endregion


        public static Dictionary<DateTime, double[]> GetSiteReportTheLastSixtyDays(int siteID)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                DateTime now = DateTime.Now.Date;

                Dictionary<DateTime, double[]> response = new Dictionary<DateTime, double[]>();

                var signups = entities.ReportsignupsPerDays(now.AddDays(-60), now.AddDays(1), siteID);
                //var visits = entities.ReportVisitsPerDays(now.AddDays(-60), now.AddDays(1), siteID);
                var billeds = entities.ReportBilledPerDays(now.AddDays(-60), now.AddDays(1), siteID);
                var cancelations = entities.ReportCancelationsPerDays(now.AddDays(-60), now.AddDays(1), siteID);
                //var cost = entities.ReportTheLastFifteenDaysCost(now.AddDays(-60), now.AddDays(1), siteID);

                bool yearChange = now.Month <= 3;

                foreach (var item in signups)
                {
                    var values = new double[3];
                    values[0] = (double)item.quantity;
                    values[1] = 0.00;
                    values[2] = 0.00;
                    response.Add(item.CreatedAt.Value, values);
                }

                foreach (var item in billeds)
                {
                    if (response.Keys.Contains(item.BilledDate.Value))
                    {
                        var cell = response[item.BilledDate.Value];
                        cell[1] = (double)item.quantity;
                    }
                    else
                    {
                        var values = new double[3];
                        values[0] = 0.00;
                        values[1] = (double)item.quantity;
                        values[2] = 0.00;
                        response.Add(item.BilledDate.Value, values);
                    }
                }

                foreach (var item in cancelations)
                {
                    if (response.Keys.Contains(item.CancelledDate.Value))
                    {
                        var cell = response[item.CancelledDate.Value];
                        cell[2] = (double)item.quantity;
                    }
                    else
                    {
                        var values = new double[3];
                        values[0] = 0.00;
                        values[2] = (double)item.quantity;
                        values[1] = 0.00;
                        response.Add(item.CancelledDate.Value, values);
                    }
                }
                return Utils.Dictionary.SortByKey(response);
            }

        }
        /// <summary>
        /// Update total referer revenue and cost.
        /// </summary>
        public static void UpdateTotalsReferrers()
        {
            using (CastleClubEntities entites = new CastleClubEntities())
            {
                decimal revenue = 0;
                decimal refund = 0;
                foreach (var referrer in entites.Referrers)
                {
                    revenue = entites.Invoices.Any(x => x.Customer.ReferrerId == referrer.Id && x.StatusId == InvoiceStatus.BILLED.ToString()) ? entites.Invoices.Where(x => x.Customer.ReferrerId == referrer.Id && x.StatusId == InvoiceStatus.BILLED.ToString()).Sum(x => x.Amount) : 0;
                    refund = entites.Invoices.Any(x => x.Customer.ReferrerId == referrer.Id && x.StatusId == InvoiceStatus.REFUNDED.ToString()) ? entites.Invoices.Where(x => x.Customer.ReferrerId == referrer.Id && x.StatusId == InvoiceStatus.REFUNDED.ToString()).Sum(x => x.Amount) : 0;

                    referrer.RevenueAmount = revenue;
                    referrer.RefundAmount = refund;
                }

                entites.SaveChanges();
            }
        }
        /// <summary>
        /// Get all billed invoice created in range date.
        /// </summary>
        /// <param name="siteID">0 for all sites.</param>
        /// <param name="from">From date.</param>
        /// <param name="to">To date.</param>
        /// <returns>Invoices for the range date.</returns>
        public static List<InvoiceDT> GetInvoiceSitesForDateRange(int siteID, DateTime from, DateTime to)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                DateTime max = to.AddDays(1).AddSeconds(-1);
                var tmp = entities.Invoices.Where(x => (siteID == 0 || x.Customer.SiteId == siteID) && x.CreatedAt >= from && x.CreatedAt <= max).ToList();
                var tmpDT = tmp.Select(x => x.GetDT());
                var tmpList = tmpDT.ToList();
                return (tmpList);
            }
        }
        private static string GenerateActiveUsersAccessLists(List<int> sitesID)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                var to = DateTime.Now;
                string path = GlobalParameters.ExcelOutPath + "\\Access\\" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(to.Month) + to.Year + "_active_Exportcon.csv";
                if (!System.IO.Directory.Exists(GlobalParameters.ExcelOutPath + "\\Access\\"))
                {
                    System.IO.Directory.CreateDirectory(GlobalParameters.ExcelOutPath + "\\Access\\");
                }

                System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                CsvHelper.CsvWriter writer = new CsvHelper.CsvWriter(sw);
                writer.WriteHeader<Data.AccessListData>();
                var sites = entities.Sites.Where(x => sitesID.Contains(x.Id)).ToList();
                foreach (var site in sites)
                {
                    if (site != null)
                    {                       

                        foreach (var activeCustomer in site.Customers.Where(x => x.CancelledDate == null ))
                        {
                            var access = new AccessListData()
                            {
                                CancelDate = activeCustomer.CancelledDate.HasValue ? activeCustomer.CancelledDate.Value.ToShortDateString() : string.Empty,
                                ClubDomain = site.OfferDomain,
                                ClubID = site.GroupId,
                                Email = activeCustomer.Email,
                                CustomerId = activeCustomer.Id.ToString(),
                                CustomerFirstName = activeCustomer.FirstName,
                                CustomerLastName = activeCustomer.LastName,
                                Signup = activeCustomer.CreatedAt.Date.ToShortDateString(),
                                Status = activeCustomer.Status.ToString(),
                                Zip = activeCustomer.ZipCode
                            };

                            writer.WriteRecord<Data.AccessListData>(access);
                        }
                    }
                }

                sw.Close();
                fs.Close();

                return path;
            }
        }
        public static byte[] GetActiveUsersAccessListFile(List<int> sitesID)
        {
            var file = GenerateActiveUsersAccessLists(sitesID);
            var bytes = System.IO.File.ReadAllBytes(file);
            if (File.Exists(file)) 
            {
                File.Delete(file);
            }

            return (bytes);
        }
        public static byte[] GetAccessCustomersCsvBytes(List<int> sitesID) 
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                var to = DateTime.Now;
                string path = GlobalParameters.ExcelOutPath + "\\Access\\" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(to.Month) + to.Year + "_Import.csv";
                if (!System.IO.Directory.Exists(GlobalParameters.ExcelOutPath + "\\Access\\"))
                {
                    System.IO.Directory.CreateDirectory(GlobalParameters.ExcelOutPath + "\\Access\\");
                }

                System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
                CsvHelper.CsvWriter writer = new CsvHelper.CsvWriter(sw);
                writer.WriteHeader<AccessCustomerDT>();
                foreach (var siteID in sitesID)
                {
                    var site = entities.Sites.FirstOrDefault(x => x.Id == siteID);
                    if (site != null)
                    {
                        foreach (var activeCustomer in site.Customers.Where(x => x.CancelledDate == null))
                        {
                            var access = new AccessCustomerDT()
                            {
                                programcustomeridentifier = site.GroupId,
                                membercustomeridentifier = activeCustomer.Id.ToString(),
                                firstName = "",
                                lastName = "",
                                postalCode = "",
                                emailAddress = ""
                                /*
                                 * 
                                firstName = activeCustomer.FirstName,
                                lastName = activeCustomer.LastName,
                                postalCode = activeCustomer.ZipCode,
                                emailAddress = activeCustomer.Email
                                 *                                  
                                 */
                            };
                            writer.WriteRecord<AccessCustomerDT>(access);
                        }
                    }
                }

                sw.Close();
                fs.Close();

              
                var bytes = System.IO.File.ReadAllBytes(path);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                return (bytes);
            }
        }
        public static void ChangeCreditCardPercentageAllSites(CastleClub.DataTypes.Enums.CCType creditCardType, int emailForm, int fullForm)
        {
            using (CastleClubEntities entities= new CastleClubEntities())
            {
                foreach (var site in entities.Sites)
                {
                    switch (creditCardType)
                    {
                        case CCType.VISA:
                            site.VisaEmailFormPercentage = emailForm;
                            site.VisaEmailFormCount = 0;
                            site.VisaEmailFormTotal = 0;
                            break;
                        case CCType.MASTERCARD:
                            site.MasterCardEmailFormPercentage = emailForm;
                            site.MasterCardEmailFormCount = 0;
                            site.MasterCardEmailFormTotal = 0;
                            break;
                        case CCType.DISCOVER:
                            site.DiscoverEmailFormPercentage = emailForm;
                            site.DiscoverEmailFormCount = 0;
                            site.DiscoverEmailFormTotal = 0;
                            break;
                        default:
                            break;
                    }
                }

                entities.SaveChanges();
            }
        }
        public static void UpdateCreditCardDataAllSites(CastleClub.DataTypes.Enums.CCType creditCardType, bool count)
        {
            using (CastleClubEntities entities= new CastleClubEntities())
            {
                foreach (var site in entities.Sites)
                {
                    switch (creditCardType)
                    {
                        case CCType.VISA:
                            if (count)
                            {
                                site.VisaEmailFormCount = site.VisaEmailFormCount + 1;
                            }
                            site.VisaEmailFormTotal = site.VisaEmailFormTotal + 1;
                            break;
                        case CCType.MASTERCARD:
                            if (count)
                            {
                                site.MasterCardEmailFormCount = site.MasterCardEmailFormCount + 1;
                            }
                            site.MasterCardEmailFormTotal = site.MasterCardEmailFormTotal + 1;
                            break;
                        case CCType.DISCOVER:
                            if (count)
                            {
                                site.DiscoverEmailFormCount = site.DiscoverEmailFormCount + 1;
                            }
                            site.DiscoverEmailFormTotal = site.DiscoverEmailFormTotal + 1;
                            break;
                        default:
                            break;
                    }
                }

                entities.SaveChanges();
            }
        }

        public static void ReportSiteCreditCards(DateTime from, DateTime to, int siteID, CCType ccType, bool emailForm, out int sigunp, out int active)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                to = to.AddDays(1).AddSeconds(-1);
                string ccTypeString = ccType.ToString().ToLower();
                List<Customer> customers = entities.Customers.Where(x => (x.SiteId==0 || x.SiteId==siteID) && x.CreatedAt>=from && x.CreatedAt<=to && x.CreditCards.Any(y => y.Type.ToLower() == ccTypeString)).ToList();

                sigunp = customers.Count(x => x.EmailForm == emailForm);
                active = customers.Count(x => x.CancelledDate == null && x.EmailForm == emailForm);
            }
        }

        public static int CustomersCountRegisterRange(DateTime from, DateTime to)
        {
            using (CastleClubEntities entities= new CastleClubEntities())
            {
                to = to.AddDays(1).AddSeconds(-1);
                return entities.Customers.Count(x => x.CreatedAt>=from && x.CreatedAt<=to);
            }
        }

        public static int WelcomeEmailsSent(DateTime from, DateTime to, int siteId, int referredId, int delay)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                to = to.AddDays(1).AddSeconds(-1);

                return entities.Customers.Count(x => x.CreatedAt >= from && x.CreatedAt <= to && x.WelcomeEmail == true && (x.SiteId == siteId || siteId == 0) && (x.ReferrerId == referredId || referredId==0) && x.WelcomeEmailDelay == delay);
            }
        }

        public static int WelcomeEmailsSentActive(DateTime from, DateTime to, int siteId, int referredId, int delay)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                to = to.AddDays(1).AddSeconds(-1);

                return entities.Customers.Count(x => x.CreatedAt >= from && x.CreatedAt <= to && x.WelcomeEmail == true && (x.SiteId == siteId || siteId == 0) && (x.ReferrerId == referredId || referredId == 0) && x.StatusId == "ACTIVE" && x.WelcomeEmailDelay == delay);
            }
        }

        public static int WelcomeEmailsSentCancelled(DateTime from, DateTime to, int siteId, int referredId, int delay)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                to = to.AddDays(1).AddSeconds(-1);

                return entities.Customers.Count(x => x.CreatedAt >= from && x.CreatedAt <= to && x.WelcomeEmail == true && (x.SiteId == siteId || siteId == 0) && (x.ReferrerId == referredId || referredId == 0) && x.StatusId == "CANCELLED" && x.WelcomeEmailDelay == delay);
            }
        }

        public static int GetMaxID()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                return entities.Sites.Max(x => x.Id);
            }
        }

        public static int GetEmailSentToday()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                DateTime from = DateTime.Today;
                DateTime to = from.AddDays(1).AddMinutes(-1);
                return entities.Customers.Count(x => x.WelcomeEmail == true && x.WelcomeEmailSent >= from && x.WelcomeEmailSent <= to);
            }
        }

        public static int GetEmailSentToday(int delay, int siteId)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                DateTime from = DateTime.Today;
                DateTime to = from.AddDays(1).AddMinutes(-1);
                return entities.Customers.Count(x => x.WelcomeEmail == true && x.WelcomeEmailSent >= from && x.WelcomeEmailSent <= to && x.WelcomeEmailDelay == delay && x.SiteId == siteId);
            }
        }

        public static List<IISResetLogDT> GetIISResetLog()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                return entities.IISResetLogs.OrderByDescending(r => r.Date).ToList().Select(r => r.GetDT()).ToList();               
            }
        }
    }
}