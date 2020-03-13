using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.Tool.SynchronizateRefund
{
    class Program
    {
        static void Main(string[] args)
        {


            string command = string.Empty;
            var reports = ReadFile(args[0], true, TransactionTypes.REFUND);
            int index = 0;

            while (command.ToLower() != "exit")
            {
                if (!string.IsNullOrEmpty(command))
                {
                    int count = 0;

                    if (int.TryParse(command, out count))
                    {
                        int existsTransactions = 0;

                        for (int i = 0; i < count; i++)
                        {
                            try
                            {
                                Dictionary<int, int> map = new Dictionary<int, int>();

                                bool finish = false;
                                while ((!finish) && (index < reports.Count))
                                {
                                    using (CastleClubEntities entities = new CastleClubEntities())
                                    {
                                        bool cancellCustomer = false;
                                        int invoiceID = map.ContainsKey(reports[index].InvoiceID) ? map[reports[index].InvoiceID] : reports[index].InvoiceID;
                                        if (reports[index].SubmitDate >= DateTime.MinValue)
                                        {
                                            var aux = reports[index].CustomerID;
                                            var customer = entities.Customers.FirstOrDefault(x => x.Id == aux);
                                            if (customer != null)
                                            {
                                                System.Console.WriteLine("Synchronizate with customer: " + customer.Id + ", transactionID: " + reports[index].TranscactionID + ", and at datetime: " + reports[index].SubmitDate + ".\n");
                                                #region
                                                aux = reports[index].CustomerID;
                                                var invoice = entities.Invoices.FirstOrDefault(x => x.Id == invoiceID && x.CustomerId == aux);

                                                var auxlong = reports[index].TranscactionID;
                                                var transaction = entities.Transactions.FirstOrDefault(x => x.AuthorizeTransactionId == auxlong);
                                                if (transaction == null)
                                                {
                                                    #region There are not any transaction with this authorize id
                                                    if (customer.Invoices.Count == 0)
                                                    {
                                                        invoice = new Invoice()
                                                        {
                                                            Amount = reports[index].Amount,
                                                            BilledDate = reports[index].SubmitDate,
                                                            CreatedAt = reports[index].SubmitDate,
                                                            Customer = customer,
                                                            CustomerId = reports[index].CustomerID,
                                                            FailCount = 0,
                                                            Transactions = new List<Transaction>(),
                                                            Id = invoiceID
                                                        };
                                                        customer.Invoices.Add(invoice);
                                                        entities.Invoices.Add(invoice);
                                                    }
                                                    else
                                                    {
                                                        if (reports[index].RefererTransactionID > 0 && customer.Invoices.Any(x => x.Transactions.Any(y => y.StatusId == "SUCCESFULL" && y.AuthorizeTransactionId == reports[index].RefererTransactionID)))
                                                        {
                                                            invoice = customer.Invoices.Where(x => x.Transactions.Any(y => y.StatusId == "SUCCESFULL" && y.AuthorizeTransactionId == reports[index].RefererTransactionID)).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                                                        }
                                                        else
                                                        {
                                                            invoice = new Invoice()
                                                            {
                                                                Amount = reports[index].Amount,
                                                                BilledDate = reports[index].SubmitDate,
                                                                CreatedAt = reports[index].SubmitDate,
                                                                Customer = customer,
                                                                CustomerId = reports[index].CustomerID,
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
                                                        AuthorizeTransactionId = reports[index].TranscactionID,
                                                        CreditCard = customer.CreditCards.FirstOrDefault(),
                                                        Invoice = invoice,
                                                        Message = string.Empty,
                                                        StatusId = reports[index].Status,
                                                        SubmitDate = reports[index].SubmitDate,
                                                        TypeId = reports[index].TransactionType
                                                    };

                                                    invoice.Transactions.Add(transaction);
                                                    entities.Transactions.Add(transaction);

                                                    if (reports[index].TransactionType == "SALE")
                                                    {
                                                        if (reports[index].Status == "SUCCESFULL")
                                                        {
                                                            invoice.StatusId = "BILLED";
                                                        }
                                                        else if (reports[index].Status == "FAILED")
                                                        {
                                                            invoice.StatusId = "BILLEDFAIL";
                                                        }
                                                    }
                                                    else if (reports[index].TransactionType == "REFUND")
                                                    {
                                                        if (reports[index].Status == "SUCCESFULL")
                                                        {
                                                            invoice.StatusId = "REFUNDED";
                                                            invoice.RefundedDate = reports[index].SubmitDate;
                                                        }
                                                        else if (reports[index].Status == "FAILED")
                                                        {
                                                            invoice.StatusId = "REFUNDEDFAIL";
                                                        }
                                                        cancellCustomer = true;

                                                        var transactionRefund = invoice.Transactions.FirstOrDefault(x => x.AuthorizeTransactionId == reports[index].RefererTransactionID);
                                                        if (transactionRefund != null)
                                                        {
                                                            transaction.RefundedTransactionId = transactionRefund.Id;
                                                        }
                                                    }

                                                    if (cancellCustomer)
                                                    {
                                                        customer.CancelledDate = reports[index].SubmitDate;
                                                        customer.StatusId = "CANCELLED";
                                                    }

                                                    try
                                                    {
                                                        entities.SaveChanges();
                                                    }
                                                    catch (Exception)
                                                    {
                                                        System.Console.WriteLine("Error with customer: " + reports[index].CustomerID + " and refund transaction: " + reports[index].TranscactionID + " with date: " + reports[index].SubmitDate.ToString());
                                                    }

                                                    finish = true;
                                                    if (!map.ContainsKey(reports[index].InvoiceID))
                                                    {
                                                        map.Add(reports[index].InvoiceID, invoice.Id);
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    //Exists any transaction in the system with this authorize id.
                                                    existsTransactions++;
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                System.Console.WriteLine("\tCustomer with id: " + aux + " do not exists in the system.");
                                            }
                                        }
                                        index = index + 1;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine("\tAn error ocurred in the process. This is the error: " + ex.ToString());
                                System.Console.WriteLine();
                            }
                        }

                        if (existsTransactions > 0)
                        {
                            System.Console.WriteLine("\tThere are " + existsTransactions + " that exists.");
                        }
                    }
                }

                Console.WriteLine("How many refund do you want to sycnhronize? (number) (" + index + "/" + reports.Count + ") (there are " + reports.Count + " refund transaction/s), or write \"exit\" for exit.");
                command = Console.ReadLine();
            }

            System.Console.WriteLine("\nUpdate totals in refrerrers.");
            UpdateTotalsReferrers();
            System.Console.WriteLine("Finish, press any key for exit.");
            System.Console.ReadLine();
        }

        /// <summary>
        /// Get all transaction from Authorize of a type.
        /// </summary>
        /// <param name="path">Authorize report file path</param>
        /// <param name="includeHeaders">Boolean indicate that the report file include headers.</param>
        /// <param name="transactionType">Type aof transaction</param>
        /// <returns></returns>
        private static List<Report> ReadFile(string path, bool includeHeaders, TransactionTypes transactionType)
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
                                    month = ToMonth(dateData[1]);
                                    int hour = (int.Parse(dateData[3]) + aftherMeridiane) == 24 ? 12 : (int.Parse(dateData[3]) + aftherMeridiane);
                                    report.SettlementDate = new DateTime(int.Parse(dateData[2]), month, int.Parse(dateData[0]), hour, int.Parse(dateData[4]), int.Parse(dateData[5]));
                                    break;
                                case 7:
                                    string[] dateDataSubmit = parameters[i].Split(separatorDate, StringSplitOptions.RemoveEmptyEntries);
                                    int monthSubmit = 1;
                                    int aftherMeridianeSubmit = dateDataSubmit[6] == "AM" ? 0 : 12;
                                    monthSubmit = ToMonth(dateDataSubmit[1]);
                                    int hourSubmit = (int.Parse(dateDataSubmit[3]) + aftherMeridianeSubmit) == 24 ? 12 : (int.Parse(dateDataSubmit[3]) + aftherMeridianeSubmit);
                                    report.SubmitDate = new DateTime(int.Parse(dateDataSubmit[2]), monthSubmit, int.Parse(dateDataSubmit[0]), hourSubmit, int.Parse(dateDataSubmit[4]), int.Parse(dateDataSubmit[5]));
                                    break;
                                case 9:
                                    report.RefererTransactionID = long.Parse(parameters[i]);
                                    break;
                                case 10:
                                    report.TransactionType = parameters[i] == Properties.Resources.AuthorizeTransactionRefund ? Properties.Resources.TransactionRefund : Properties.Resources.TransactionSale;
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

                        switch (transactionType)
                        {
                            case TransactionTypes.REFUND:
                                if (report.TransactionType.ToLower()==Properties.Resources.TransactionRefund)
                                {
                                    reports.Add(report);
                                }
                                break;
                            case TransactionTypes.SALE:
                                if (report.TransactionType.ToLower()==Properties.Resources.TransactionSale)
                                {
                                    reports.Add(report);
                                }
                                break;
                            default:
                                break;
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

        private static int ToMonth(string dateData)
        {
            int month = 1;
            switch (dateData)
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
                case "Dec":
                    month = 12;
                    break;
            }
            return month;
        }

        /// <summary>
        /// Update the money for each referrer.
        /// </summary>
        public static void UpdateTotalsReferrers()
        {
            using (CastleClubEntities entites = new CastleClubEntities())
            {
                decimal revenue = 0;
                decimal refund = 0;
                foreach (var referrer in entites.Referrers)
                {
                    revenue = entites.Invoices.Any(x => x.Customer.ReferrerId == referrer.Id && x.StatusId == "BILLED") ? entites.Invoices.Where(x => x.Customer.ReferrerId == referrer.Id && x.StatusId == "BILLED").Sum(x => x.Amount) : 0;
                    refund = entites.Invoices.Any(x => x.Customer.ReferrerId == referrer.Id && x.StatusId == "REFUNDED") ? entites.Invoices.Where(x => x.Customer.ReferrerId == referrer.Id && x.StatusId == "REFUNDED").Sum(x => x.Amount) : 0;

                    referrer.RevenueAmount = revenue;
                    referrer.RefundAmount = refund;
                }

                entites.SaveChanges();
            }
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

        public enum TransactionTypes
        {
            REFUND,
            SALE
        }
    }
}
