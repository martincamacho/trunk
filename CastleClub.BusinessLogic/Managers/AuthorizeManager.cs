using CastleClub.BusinessLogic.AuthorizeAPI;
using CastleClub.BusinessLogic.Data;
using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Managers
{
    public class AuthorizeManager
    {
        /// <summary>
        /// Read all transaction from authorize report file.
        /// </summary>
        /// <param name="path">Authorize report file path.</param>
        /// <param name="onlyRefund">Indicate if only include refund transactions.</param>
        /// <returns>Indicate if all ok or was any error.</returns>
        public static bool ReadReportFile(string path, bool onlyRefund)
        {
            try
            {
                var reports = ReadFile(path, true, onlyRefund);

                Dictionary<int, int> map = new Dictionary<int, int>();

                using (CastleClubEntities entities = new CastleClubEntities())
                {
                    bool cancellCustomer = false;
                    foreach (var report in reports)
                    {
                        int invoiceID = map.ContainsKey(report.InvoiceID) ? map[report.InvoiceID] : report.InvoiceID;
                        if (report.SubmitDate >= new DateTime(2014, 11, 04))
                        {
                            var aux = report.CustomerID;
                            var customer = entities.Customers.FirstOrDefault(x => x.Id == aux);
                            if (customer != null)
                            {
                                #region
                                aux = report.CustomerID;
                                var invoice = entities.Invoices.FirstOrDefault(x => x.Id == invoiceID && x.CustomerId == aux);
                                if (invoice != null)
                                {
                                    var auxLong = report.TranscactionID;
                                    var transaction = entities.Transactions.FirstOrDefault(x => x.AuthorizeTransactionId == auxLong);
                                    if (transaction == null)
                                    {
                                        transaction = new Transaction()
                                        {
                                            AuthorizeTransactionId = report.TranscactionID,
                                            CreditCard = customer.CreditCards.FirstOrDefault(),
                                            //Invoice = invoice,
                                            Message = string.Empty,
                                            StatusId = report.Status,
                                            SubmitDate = report.SubmitDate,
                                            TypeId = report.TransactionType
                                        };
                                        entities.Transactions.Add(transaction);

                                        if ((report.Status == "SUCCESFULL") && (report.TransactionType == "SALE"))
                                        {
                                            invoice.BilledDate = report.SettlementDate;
                                            invoice.StatusId = "BILLED";
                                            invoice.Transactions.Add(transaction);
                                            transaction.Invoice = invoice;
                                        }
                                        else if ((report.Status == "SUCCESFULL") && (report.TransactionType == "REFUND"))
                                        {
                                            if (customer.Invoices.Any())
                                            {
                                                var invoiceAux = customer.Invoices.Where(x => x.Transactions.Any(y => y.StatusId == "SUCCESFULL" && y.AuthorizeTransactionId == report.RefererTransactionID)).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                                                if (invoiceAux != null)
                                                {
                                                    invoice = invoiceAux;
                                                    transaction.RefundedTransactionId = invoice.Transactions.FirstOrDefault(x => x.AuthorizeTransactionId == report.RefererTransactionID).Id;
                                                }
                                            }
                                            invoice.RefundedDate = report.SettlementDate;
                                            invoice.StatusId = "REFUNDED";
                                            invoice.Amount = report.Amount;
                                            cancellCustomer = true;

                                            transaction.Invoice = invoice;
                                            invoice.Transactions.Add(transaction);
                                        }
                                        else if ((report.Status == "FAILED") && (report.TransactionType == "SALE"))
                                        {
                                            invoice.StatusId = "BILLEDFAIL";
                                            invoice.BilledDate = report.SettlementDate;
                                            invoice.Transactions.Add(transaction);
                                            transaction.Invoice = invoice;
                                        }
                                        else if ((report.Status == "FAILED") && (report.TransactionType == "REFUND"))
                                        {
                                            if (customer.Invoices.Any())
                                            {
                                                var invoiceAux = customer.Invoices.Where(x => x.Transactions.Any(y => y.StatusId == "SUCCESFULL" && y.AuthorizeTransactionId == report.RefererTransactionID)).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                                                if (invoiceAux != null)
                                                {
                                                    invoice = invoiceAux;
                                                    transaction.RefundedTransactionId = invoice.Transactions.FirstOrDefault(x => x.AuthorizeTransactionId == report.RefererTransactionID).Id;
                                                }
                                            }
                                            invoice.RefundedDate = report.SettlementDate;
                                            invoice.StatusId = "REFUNDEDFAIL";
                                            invoice.Amount = report.Amount;
                                            cancellCustomer = true;

                                            invoice.Transactions.Add(transaction);
                                            transaction.Invoice = invoice;
                                            invoice.Transactions.Add(transaction);
                                        }

                                        if (cancellCustomer)
                                        {
                                            customer.CancelledDate = report.SubmitDate;
                                            customer.StatusId = "CANCELLED";
                                        }

                                        try
                                        {
                                            entities.SaveChanges();
                                        }
                                        catch(Exception)
                                        {
                                            Utils.EventViewer.Writte("Castle Club Admin","Authorize Synchronize","The synchronize for customer: \""+report.CustomerID+"\", with the transaction: \""+report.TranscactionID+"\", and this date: \""+report.SubmitDate+"\", failed.", System.Diagnostics.EventLogEntryType.Error);
                                        }
                                        if (!map.ContainsKey(report.InvoiceID))
                                        {
                                            map.Add(report.InvoiceID, invoice.Id);
                                        }
                                    }
                                }
                                else
                                {
                                    var auxlong = report.TranscactionID;
                                    var transaction = entities.Transactions.FirstOrDefault(x => x.AuthorizeTransactionId == auxlong);
                                    if (transaction == null)
                                    {
                                        if (customer.Invoices.Count == 0)
                                        {
                                            invoice = new Invoice()
                                            {
                                                Amount = report.Amount,
                                                BilledDate = report.SubmitDate,
                                                CreatedAt = report.SubmitDate,
                                                Customer = customer,
                                                CustomerId = report.CustomerID,
                                                FailCount = 0,
                                                Transactions = new List<Transaction>(),
                                                Id = invoiceID
                                            };
                                            customer.Invoices.Add(invoice);
                                            entities.Invoices.Add(invoice);
                                        }
                                        else
                                        {
                                            if (report.RefererTransactionID > 0 && customer.Invoices.Any(x => x.Transactions.Any(y => y.StatusId == "SUCCESFULL" && y.AuthorizeTransactionId == report.RefererTransactionID)))
                                            {
                                                invoice = customer.Invoices.Where(x => x.Transactions.Any(y => y.StatusId == "SUCCESFULL" && y.AuthorizeTransactionId == report.RefererTransactionID)).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                                            }
                                            else
                                            {
                                                invoice = new Invoice()
                                                {
                                                    Amount = report.Amount,
                                                    BilledDate = report.SubmitDate,
                                                    CreatedAt = report.SubmitDate,
                                                    Customer = customer,
                                                    CustomerId = report.CustomerID,
                                                    FailCount = 0,
                                                    Transactions = new List<Transaction>(),
                                                    Id = invoiceID
                                                };
                                                customer.Invoices.Add(invoice);
                                                entities.Invoices.Add(invoice);
                                            }
                                        }

                                        transaction = new Transaction()
                                        {
                                            AuthorizeTransactionId = report.TranscactionID,
                                            CreditCard = customer.CreditCards.FirstOrDefault(),
                                            Invoice = invoice,
                                            Message = string.Empty,
                                            StatusId = report.Status,
                                            SubmitDate = report.SubmitDate,
                                            TypeId = report.TransactionType
                                        };

                                        invoice.Transactions.Add(transaction);
                                        entities.Transactions.Add(transaction);

                                        if (report.TransactionType == "SALE")
                                        {
                                            if (report.Status == "SUCCESFULL")
                                            {
                                                invoice.StatusId = "BILLED";
                                            }
                                            else if (report.Status == "FAILED")
                                            {
                                                invoice.StatusId = "BILLEDFAIL";
                                            }
                                        }
                                        else if (report.TransactionType == "REFUND")
                                        {
                                            if (report.Status == "SUCCESFULL")
                                            {
                                                invoice.StatusId = "REFUNDED";
                                            }
                                            else if (report.Status == "FAILED")
                                            {
                                                invoice.StatusId = "REFUNDEDFAIL";
                                            }
                                            cancellCustomer = true;

                                            var transactionRefund = invoice.Transactions.FirstOrDefault(x => x.AuthorizeTransactionId == report.RefererTransactionID);
                                            if (transactionRefund != null)
                                            {
                                                transaction.RefundedTransactionId = transactionRefund.Id;
                                            }
                                        }

                                        if (cancellCustomer)
                                        {
                                            customer.CancelledDate = report.SubmitDate;
                                            customer.StatusId = "CANCELLED";
                                        }

                                        try
                                        {
                                            entities.SaveChanges();
                                        }
                                        catch(Exception)
                                        {
                                            Utils.EventViewer.Writte("Castle Club Admin", "Authorize Synchronize", "The synchronize for customer: \"" + report.CustomerID + "\", with the transaction: \"" + report.TranscactionID + "\", and this date: \"" + report.SubmitDate + "\", failed.", System.Diagnostics.EventLogEntryType.Error);
                                        }

                                        if (!map.ContainsKey(report.InvoiceID))
                                        {
                                            map.Add(report.InvoiceID, invoice.Id);
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Resume all transaction of range. Successfull and Fail
        /// </summary>
        /// <param name="siteID">0 for all site.</param>
        /// <param name="from">From date</param>
        /// <param name="to">To date</param>
        /// <returns>Index 0 is successfull, and index 1 is fail.</returns>
        public static decimal[] TransactionReport(int siteID, DateTime from, DateTime to)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                decimal[] resp = new decimal[2];
                resp[0] = 0;
                resp[1] = 0;

                if (siteID == 0)
                {
                    var sites = SitesManager.GetSites();
                    foreach (var site in sites)
                    {
                        GetTransactionReportFromAuthorizeForSite(site, from, to.Date.AddDays(1).AddSeconds(-1), resp, entities);
                    }
                }
                else
                {
                    GetTransactionReportFromAuthorizeForSite(SitesManager.GetSite(siteID), from, to.Date.AddDays(1).AddSeconds(-1), resp, entities);
                }

                return resp;
            }
        }

        #region Helpers
        /// <summary>
        /// Get all transaction for a specific date range.
        /// </summary>
        /// <param name="path">Authorize report file path.</param>
        /// <param name="includeHeaders">Boolean, indicate if include headers.</param>
        /// <param name="onlyRefund">Indicate if only include refund transactions.</param>
        /// <returns>Return transaction list.</returns>
        private static List<Report> ReadFile(string path, bool includeHeaders, bool onlyRefund)
        {
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.StreamReader sr = new System.IO.StreamReader(fs);

            List<Report> reports = new List<Report>();

            string[] separator = new string[] { "," };
            string[] separatorDate = new string[] { "-", " ", ":" };
            string line = string.Empty;
            string[] parameters;

            while (!sr.EndOfStream)
            {
                try
                {
                    line = sr.ReadLine();

                    if (!includeHeaders)
                    {
                        parameters = line.Split(separator, StringSplitOptions.None);
                        Report report = new Report();
                        for (int i = 0; i < parameters.Count(); i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    report.TranscactionID = long.Parse(parameters[i]);
                                    break;
                                case 1:
                                    report.Status = parameters[i];
                                    if (report.Status.Contains("Successfully") || report.Status.Contains("Credit"))
                                    {
                                        report.Status = "SUCCESFULL";
                                    }
                                    else
                                    {
                                        report.Status = "FAILED";
                                    }
                                    break;
                                case 4:
                                    string[] dateData = parameters[i].Split(separatorDate, StringSplitOptions.RemoveEmptyEntries);
                                    int month = 1;
                                    int aftherMeridiane = dateData[6] == "AM" ? 0 : 12;
                                    switch (dateData[1])
                                    {
                                        case "Feb":
                                            month = 2;
                                            break;
                                        case "Mar":
                                            month = 3;
                                            break;
                                        case "Apr":
                                            month = 4;
                                            break;
                                        case "May":
                                            month = 5;
                                            break;
                                        case "Jun":
                                            month = 6;
                                            break;
                                        case "Jul":
                                            month = 7;
                                            break;
                                        case "Agu":
                                            month = 8;
                                            break;
                                        case "Sep":
                                            month = 9;
                                            break;
                                        case "Oct":
                                            month = 10;
                                            break;
                                        case "Nov":
                                            month = 11;
                                            break;
                                        case "Dic":
                                            month = 12;
                                            break;
                                    }
                                    int hour = (int.Parse(dateData[3]) + aftherMeridiane) == 24 ? 12 : (int.Parse(dateData[3]) + aftherMeridiane);
                                    report.SettlementDate = new DateTime(int.Parse(dateData[2]), month, int.Parse(dateData[0]), hour, int.Parse(dateData[4]), int.Parse(dateData[5]));
                                    break;
                                case 7:
                                    string[] dateDataSubmit = parameters[i].Split(separatorDate, StringSplitOptions.RemoveEmptyEntries);
                                    int monthSubmit = 1;
                                    int aftherMeridianeSubmit = dateDataSubmit[6] == "AM" ? 0 : 12;
                                    switch (dateDataSubmit[1])
                                    {
                                        case "Feb":
                                            monthSubmit = 2;
                                            break;
                                        case "Mar":
                                            monthSubmit = 3;
                                            break;
                                        case "Apr":
                                            monthSubmit = 4;
                                            break;
                                        case "May":
                                            monthSubmit = 5;
                                            break;
                                        case "Jun":
                                            monthSubmit = 6;
                                            break;
                                        case "Jul":
                                            monthSubmit = 7;
                                            break;
                                        case "Agu":
                                            monthSubmit = 8;
                                            break;
                                        case "Sep":
                                            monthSubmit = 9;
                                            break;
                                        case "Oct":
                                            monthSubmit = 10;
                                            break;
                                        case "Nov":
                                            monthSubmit = 11;
                                            break;
                                        case "Dic":
                                            monthSubmit = 12;
                                            break;
                                    }
                                    int hourSubmit = (int.Parse(dateDataSubmit[3]) + aftherMeridianeSubmit) == 24 ? 12 : (int.Parse(dateDataSubmit[3]) + aftherMeridianeSubmit);
                                    report.SubmitDate = new DateTime(int.Parse(dateDataSubmit[2]), monthSubmit, int.Parse(dateDataSubmit[0]), hourSubmit, int.Parse(dateDataSubmit[4]), int.Parse(dateDataSubmit[5]));
                                    break;
                                case 9:
                                    report.RefererTransactionID = long.Parse(parameters[i]);
                                    break;
                                case 10:
                                    report.TransactionType = parameters[i] == "Credit" ? "REFUND" : "SALE";
                                    break;
                                case 20:
                                    report.Amount = decimal.Parse(parameters[i]);
                                    break;
                                case 22:
                                    report.InvoiceID = !string.IsNullOrEmpty(parameters[i]) ? int.Parse(parameters[i]) : -1;
                                    break;
                                case 35:
                                    report.CustomerID = !string.IsNullOrEmpty(parameters[i]) ? int.Parse(parameters[i]) : -1;
                                    break;
                            }
                        }

                        if ((!onlyRefund) || (report.TransactionType == "REFUND"))
                        {
                            reports.Add(report);
                        }
                    }
                    else
                    {
                        includeHeaders = false;
                    }
                }
                catch (Exception)
                { }
            }

            sr.Close();
            fs.Close();

            return reports;
        }
        /// <summary>
        /// Get the resume of all transaction for a specific site.
        /// </summary>
        /// <param name="site">Site.</param>
        /// <param name="from">From date.</param>
        /// <param name="to">To date.</param>
        /// <param name="resp">Response, in index 0 is for successfull transactions, in index 1 is for failed transactions.</param>
        /// <param name="entities">Data base context.</param>
        private static void GetTransactionReportFromAuthorizeForSite(SiteDT site, DateTime from, DateTime to, decimal[] resp, CastleClubEntities entities)
        {
            var parameter = new Dictionary<SiteDT, List<TransactionDT>>();
            var tmpList = entities.Transactions.Where(x => x.Invoice.Customer.SiteId == site.Id && x.TypeId != "REFUND" && x.SubmitDate >= from && x.SubmitDate <= to).ToList();
            var tmpTransform = tmpList.Select(x => x.GetDT());
            var tmpToList = tmpTransform.ToList();
            parameter.Add(site, tmpToList);

            decimal[] report = CIM.GetTransactionRange(parameter);

            resp[0] = resp[0] + report[0];
            resp[1] = resp[1] + report[1];
        }
        private class Report
        {
            public long TranscactionID { get; set; }
            public string Status { get; set; }
            public decimal Amount { get; set; }
            public DateTime SettlementDate { get; set; }
            public DateTime SubmitDate { get; set; }
            public long RefererTransactionID { get; set; }
            public string TransactionType { get; set; }
            public int CustomerID { get; set; }
            public int InvoiceID { get; set; }
        }
        #endregion
    }
}
