using CastleClub.Tasks.PaymentTask.Authorize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.Tasks.PaymentTask
{
    class Program
    {
        static void Main(string[] args)
        {
            string message = string.Empty;
            string command = string.Empty;

            try
            {
                CreateInvoices(CustomerForCharge(), false);
                ProcessInvoices(InvoicesNewCount(), false);
            }
            catch (Exception ex)
            {
                CastleClub.BusinessLogic.Utils.EventViewer.Writte("Castle Club", "Payment Task", ex.Message + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                Console.WriteLine(ex.Message);
            }
        }

        public static void CreateInvoices()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                DateTime yearMin = new DateTime(2014, 1, 1);
                DateTime date = new DateTime(2014, 11, 24);
                DateTime tomorrow = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                DateTime today = DateTime.Now.Date;

                System.Console.WriteLine("\tThere are " + entities.Customers.Count(c => c.NextBillDate >= date && c.NextBillDate <= tomorrow && c.StatusId == "ACTIVE") + " customers for charge.");

                List<Customer> list = entities.Customers.Where(c => c.NextBillDate >= date && c.NextBillDate <= tomorrow && c.StatusId == "ACTIVE").ToList();
                foreach (Customer c in list)
                {
                    Invoice invoice = new Invoice();
                    invoice.CustomerId = c.Id;
                    invoice.Amount = c.Site.PricePerQuarter;
                    invoice.StatusId = "NEW";
                    invoice.CreatedAt = DateTime.Now;

                    c.LastBillDate = DateTime.Now;
                    c.NextBillDate = c.NextBillDate.AddMonths(3);

                    entities.Invoices.Add(invoice);

                    entities.SaveChanges();
                }
            }
        }

        public static void CreateInvoices(int count, bool all)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                 DateTime min = DateTime.Now.Date;
                 DateTime tomorrow = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                

                System.Console.WriteLine("\tThere are " + entities.Customers.Count(c => c.NextBillDate >= min && c.NextBillDate <= tomorrow && c.StatusId == "ACTIVE") + " customer for charge.");

                var list = entities.Customers.Where(c => c.NextBillDate >= min && c.NextBillDate <= tomorrow && c.StatusId == "ACTIVE").ToList();
                if (list.Count > count)
                {
                    list = list.Take(count).ToList();
                }
                foreach (var customer in list)
                {
                    Invoice invoice = new Invoice();
                    invoice.CustomerId = customer.Id;
                    invoice.Amount = customer.Site.PricePerQuarter;
                    invoice.StatusId = "NEW";
                    invoice.CreatedAt = DateTime.Now;

                    customer.LastBillDate = DateTime.Now;
                    customer.NextBillDate = customer.NextBillDate.AddDays(90);

                    entities.Invoices.Add(invoice);

                    entities.SaveChanges();
                }
            }
        }

        public static void ProcessInvoices(int count, bool all)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                DateTime dateMin = DateTime.Now.Date;        
          

                System.Console.WriteLine("\tCount is: " + count + ", and the total is: " + entities.Invoices.Count(i => (i.StatusId == "NEW") && (i.Customer.AuthorizeProfileId != 0 || all) && i.CreatedAt > dateMin) + ".");
                var list = entities.Invoices.Where(i => (i.StatusId == "NEW") && (i.Customer.AuthorizeProfileId != 0 || all) && i.CreatedAt > dateMin).Take(count).ToList();
                foreach (var invoice in list)
                {
                    if (invoice.StatusId == "NEW" && invoice.CreatedAt > new DateTime(2014, 1, 1))
                    {
                        Console.WriteLine("\tProcess invoice: " + invoice.Id);
                        Site siteDT = invoice.Customer.Site;
                        Invoice invoiceDT = invoice;
                        Customer customerDT = invoice.Customer;
                        CreditCard creditCardDT = invoice.Customer.CreditCards.FirstOrDefault();

                        Transaction transaction = new Transaction();
                        transaction.InvoiceId = invoiceDT.Id;
                        transaction.CreditCardId = creditCardDT.Id;
                        transaction.AuthorizeTransactionId = 0;
                        transaction.TypeId = "SALE";
                        transaction.StatusId = "FAILED";
                        transaction.SubmitDate = DateTime.Now;

                        entities.Transactions.Add(transaction);
                        entities.SaveChanges();

                        long transactionId = 0;
                        string message = string.Empty;
                        bool cancel = false;

                        bool succesfull = CIM.CreateCustomerProfileTransactionAuthCapture(siteDT, invoiceDT, customerDT, creditCardDT, out transactionId, out message, out cancel);

                        if (succesfull)
                        {
                            Console.WriteLine("\tCorrect process.");
                            invoice.StatusId = "BILLED";
                            invoice.BilledDate = DateTime.Now;

                            transaction.StatusId = "SUCCESFULL";
                            transaction.AuthorizeTransactionId = transactionId;

                            invoice.Customer.Referrer.BilledTotal++;
                            invoice.Customer.Referrer.RevenueAmount += invoice.Amount;

                            entities.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine("\tFail process.");
                            invoice.FailCount = invoice.FailCount.HasValue ? invoice.FailCount.Value + 1 : 1;
                            transaction.Message = message;
                            invoice.StatusId = "BILLEDFAIL";

                            /*
                            if (cancel)
                            {
                                invoice.Customer.CancelledDate = DateTime.Now;
                                invoice.Customer.StatusId = "CANCELLED";
                            }
                            */
                            entities.SaveChanges();

                            CastleClub.BusinessLogic.Utils.EventViewer.Writte("Castle Club", "Payment Task", message.ToString(), System.Diagnostics.EventLogEntryType.Error);
                        }
                    }
                }
            }
        }

        public static int InvoicesNewCount()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                DateTime aux = DateTime.Now.Date;
                return (entities.Invoices.Count(x => x.CreatedAt >= aux && (x.Customer.AuthorizeProfileId != 0) && x.StatusId == "NEW"));
            }
        }

        public static int CustomerForCharge()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                 DateTime min = DateTime.Now.Date;
                 DateTime tomorrow = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                
                return (entities.Customers.Count(x => x.NextBillDate >= min && x.NextBillDate <= tomorrow && x.StatusId == "ACTIVE"));
            }
        }
    }
}
