using CastleClub.BusinessLogic.AuthorizeAPI;
using CastleClub.BusinessLogic.Data;
using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mail;

namespace CastleClub.BusinessLogic.Managers
{
    public static class PaymentsManager
    {
        public static void CreateInvoices()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                if (CastleClub.BusinessLogic.Data.GlobalParameters.Create)
                {
                    int yearMin = CastleClub.BusinessLogic.Data.GlobalParameters.YearMin;
                    DateTime min = new DateTime(yearMin, 1, 1);
                    DateTime max = DateTime.Now.Date.AddDays(1).AddMinutes(-1);
                    List<Customer> list = entities.Customers.Where(c => c.NextBillDate >= min && c.NextBillDate < max && c.StatusId == CustomerStatus.ACTIVE.ToString()).ToList();
                    foreach (Customer c in list)
                    {
                        try
                        {
                            while (c.NextBillDate <= DateTime.Now)
                            {
                                Invoice invoice = new Invoice();
                                invoice.CustomerId = c.Id;
                                invoice.Amount = c.Site.PricePerQuarter;
                                invoice.Status = InvoiceStatus.NEW;
                                invoice.CreatedAt = DateTime.Now;

                                c.LastBillDate = DateTime.Now;
                                c.NextBillDate = c.NextBillDate.AddMonths(3);

                                entities.Invoices.Add(invoice);

                                entities.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            Utils.EventViewer.Writte("CastleClub", "PaymentTask - Creations", ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                        }
                    }
                }
            }
        }

        public static void ProcessInvoices()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                if (CastleClub.BusinessLogic.Data.GlobalParameters.Procces)
                {
                    int maxFailCount = CastleClub.BusinessLogic.Data.GlobalParameters.FailCount;
                    int yearMin = CastleClub.BusinessLogic.Data.GlobalParameters.YearMin;
                    DateTime min = new DateTime(yearMin, 1, 1);
                    List<Invoice> list = entities.Invoices.Where(i => (i.Customer.CancelledDate == null) && (i.CreatedAt>=min) && (!i.FailCount.HasValue || i.FailCount.Value < maxFailCount) && (i.StatusId == InvoiceStatus.NEW.ToString() || i.StatusId == InvoiceStatus.BILLEDFAIL.ToString())).ToList();
                    foreach (Invoice invoice in list)
                    {
                        try
                        {
                            SiteDT siteDT = invoice.Customer.Site.GetDT();
                            InvoiceDT invoiceDT = invoice.GetDT();
                            CustomerDT customerDT = invoice.Customer.GetDT(false);
                            CreditCardDT creditCardDT = invoice.Customer.CreditCards.FirstOrDefault().GetDT();

                            Transaction transaction = new Transaction();
                            transaction.InvoiceId = invoiceDT.Id;
                            transaction.CreditCardId = creditCardDT.Id;
                            transaction.AuthorizeTransactionId = 0;
                            transaction.TypeId = TransactionType.SALE.ToString();
                            transaction.StatusId = TransactionStatus.FAILED.ToString();
                            transaction.SubmitDate = DateTime.Now;

                            entities.Transactions.Add(transaction);
                            entities.SaveChanges();

                            long transactionId = 0;
                            string message = "";
                            string code = string.Empty;

                            bool succesfull = CIM.CreateCustomerProfileTransactionAuthCapture(siteDT, invoiceDT, customerDT, creditCardDT, out transactionId, out message,out code);

                            if (succesfull)
                            {
                                invoice.Status = InvoiceStatus.BILLED;
                                invoice.BilledDate = DateTime.Now;

                                transaction.Status = TransactionStatus.SUCCESFULL;
                                transaction.AuthorizeTransactionId = transactionId;

                                invoice.Customer.Referrer.BilledTotal++;
                                invoice.Customer.Referrer.RevenueAmount += invoice.Amount;

                                entities.SaveChanges();
                            }
                            else
                            {
                                invoice.FailCount = invoice.FailCount.HasValue ? invoice.FailCount.Value + 1 : 1;
                                transaction.Message = message + code;
                                invoice.Status = InvoiceStatus.BILLEDFAIL;
                                entities.SaveChanges();

                                if (invoice.FailCount >= maxFailCount && entities.NotificationProcess.Any())
                                {
                                    //To do some.
                                    //Send email for notification.

                                    string smtpAddress = CastleClub.BusinessLogic.Data.GlobalParameters.Smtp;
                                    int portNumber = 587;
                                    bool enableSSL = true;

                                    string emailFrom = CastleClub.BusinessLogic.Data.GlobalParameters.EmailAccount;
                                    string password = CastleClub.BusinessLogic.Data.GlobalParameters.EmailPassword;
                                    string subject = "Error an ocurred with invoice: " + invoiceDT.Id;
                                    string body = "Error ocurred at " + maxFailCount + " time with invoice: " + invoiceDT.Id + ", customerId: " + invoice.CustomerId + ", and with credit card: " + invoice.Customer.CreditCards.FirstOrDefault().Id;

                                    using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                                    {
                                        mail.From = new MailAddress(emailFrom);
                                        foreach (var emailTo in entities.NotificationProcess)
                                        {
                                            mail.To.Add(emailTo.To);
                                        }
                                        mail.Subject = subject;
                                        mail.Body = body;
                                        mail.IsBodyHtml = false;

                                        using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                                        {
                                            smtp.Credentials = new NetworkCredential(emailFrom, password);
                                            smtp.EnableSsl = enableSSL;
                                            smtp.Send(mail);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Utils.EventViewer.Writte("CastleClubAdmin", "PaymentTask - Process", ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Process a specific invoices, is using for tr charge again.
        /// </summary>
        /// <param name="invoiceId">Id for invoice fail</param>
        /// <returns>Return true if the invoice charge ok. Return false if the invoice charge false.</returns>
        public static bool ProcessInvoice(int invoiceId)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                Invoice invoice = entities.Invoices.FirstOrDefault(i => i.Id == invoiceId && (i.Customer.CancelledDate == null) && (i.StatusId == InvoiceStatus.NEW.ToString() || i.StatusId == InvoiceStatus.BILLEDFAIL.ToString()));
                if (invoice != null)
                {
                    try
                    {
                        SiteDT siteDT = invoice.Customer.Site.GetDT();
                        InvoiceDT invoiceDT = invoice.GetDT();
                        CustomerDT customerDT = invoice.Customer.GetDT(false);
                        CreditCardDT creditCardDT = invoice.Customer.CreditCards.FirstOrDefault().GetDT();

                        Transaction transaction = new Transaction();
                        transaction.InvoiceId = invoiceDT.Id;
                        transaction.CreditCardId = creditCardDT.Id;
                        transaction.AuthorizeTransactionId = 0;
                        transaction.Type = TransactionType.SALE;
                        transaction.Status = TransactionStatus.FAILED;
                        transaction.SubmitDate = DateTime.Now;
                        invoice.Transactions.Add(transaction);

                        entities.Transactions.Add(transaction);
                        entities.SaveChanges();

                        long transactionId = 0;
                        string message = "";
                        string code = string.Empty;

                        bool succesfull = CIM.CreateCustomerProfileTransactionAuthCapture(siteDT, invoiceDT, customerDT, creditCardDT, out transactionId, out message, out code);

                        if (succesfull)
                        {
                            invoice.Status = InvoiceStatus.BILLED;
                            invoice.BilledDate = DateTime.Now;

                            transaction.Status = TransactionStatus.SUCCESFULL;
                            transaction.AuthorizeTransactionId = transactionId;

                            invoice.Customer.Referrer.BilledTotal++;
                            invoice.Customer.Referrer.RevenueAmount += invoice.Amount;

                            entities.SaveChanges();
                            return true;
                        }
                        else
                        {
                            invoice.FailCount = invoice.FailCount.HasValue ? invoice.FailCount.Value + 1 : 1;
                            transaction.Message = message + code;
                            invoice.Status = InvoiceStatus.BILLEDFAIL;
                            entities.SaveChanges();

                            if (entities.NotificationProcess.Any())
                            {
                                //To do some.
                                //Send email for notification.

                                string smtpAddress = CastleClub.BusinessLogic.Data.GlobalParameters.Smtp;
                                int portNumber = 587;
                                bool enableSSL = true;

                                string emailFrom = CastleClub.BusinessLogic.Data.GlobalParameters.EmailAccount;
                                string password = CastleClub.BusinessLogic.Data.GlobalParameters.EmailPassword;
                                string subject = "Error an ocurred with invoice: " + invoiceDT.Id;
                                string body = "Error ocurred at time with invoice: " + invoiceDT.Id + ", customerId: " + invoice.CustomerId + ", and with credit card: " + invoice.Customer.CreditCards.FirstOrDefault().Id;

                                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                                {
                                    mail.From = new MailAddress(emailFrom);
                                    foreach (var emailTo in entities.NotificationProcess)
                                    {
                                        mail.To.Add(emailTo.To);
                                    }
                                    mail.Subject = subject;
                                    mail.Body = body;
                                    mail.IsBodyHtml = false;

                                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                                    {
                                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                                        smtp.EnableSsl = enableSSL;
                                        smtp.Send(mail);
                                    }
                                }
                            }
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.EventViewer.Writte("CastleClubAdmin", "PaymentTask - Process", ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
                    }
                    return false;
                }

                return false;
            }
        }

        /// <summary>
        /// Get a specific invoice.
        /// </summary>
        /// <param name="invoiceId">Id of the invoice for search.</param>
        /// <returns></returns>
        public static InvoiceDT GetInvoice(int invoiceId)
        {
            using (CastleClubEntities entities=new CastleClubEntities())
            {
                var invoice = entities.Invoices.FirstOrDefault(x => x.Id == invoiceId);
                if (invoice!=null)
                {
                    return invoice.GetDT();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}