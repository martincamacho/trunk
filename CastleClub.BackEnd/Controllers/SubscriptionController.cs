using CastleClub.BackEnd.Models;
using CastleClub.BusinessLogic.Managers;
using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace CastleClub.BackEnd.Controllers
{
    [Authorize(Roles="Super Admin, Admin, Customer Service")]
    public class SubscriptionController : Controller
    {
        public ActionResult ManageSubscriptions(string customerEmail)
        {
            if (!string.IsNullOrEmpty(customerEmail))
            {
                ManageSubscriptionsVM manageSubscription=new ManageSubscriptionsVM()
                {
                    Email=customerEmail
                };
                return ManageSubscriptions(manageSubscription);
            }

            var model = new ManageSubscriptionsVM();
            model.Sites = SitesManager.GetSites();

            return View(model);
        }

        [HttpPost]
        public ActionResult ManageSubscriptions(ManageSubscriptionsVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Sites = SitesManager.GetSites();
                return View(model);
            }
            int memberId = 0;
            Int32.TryParse(model.MemberId, out memberId);
            int siteId = 0;
            Int32.TryParse(model.SiteId, out siteId);
            int lastFourDigit=-1;
            lastFourDigit = !string.IsNullOrEmpty(model.LastFourDigit) && int.TryParse(model.LastFourDigit, out lastFourDigit) ? int.Parse(model.LastFourDigit) : -1;

            if (memberId != 0 || model.Email != null || model.Phone != null || model.Name != null || (lastFourDigit>=0 && lastFourDigit<=9999))
            {
                string lastFourDigitCreditCard = GenerateLastFourDigitCreditCard(lastFourDigit);

                List<CustomerDT> data = CustomersManager.GetCustomers(siteId, memberId, model.Email, model.Phone, model.Name, lastFourDigitCreditCard);
                ViewBag.Data = data;
            }

            model.Sites = SitesManager.GetSites();
            return View(model);
        }

        public ActionResult SubscriptionInfo(string customerId)
        {
            int id = 0;
            Int32.TryParse(customerId, out id);
            CustomerDT customer = CustomersManager.GetCustomer(id);
            List<InvoiceDT> invoices = CustomersManager.GetCustomerInvoices(id);
            decimal balance = CustomersManager.GetCustomerBalance(id);
            string cardType = string.Empty;

            ViewBag.Customer = customer;
            ViewBag.Invoices = invoices;
            ViewBag.Balance = balance;
            ViewBag.CardNumber = CustomersManager.GetLastFourDigitCreditCards(customer.Id, out cardType);
            ViewBag.CardType= cardType;

            return View();
        }


        public ActionResult BillingInfo(string customerId)
        {
            int id = 0;
            Int32.TryParse(customerId, out id);
            CustomerDT customer = CustomersManager.GetCustomer(id);
            List<InvoiceDT> invoices = CustomersManager.GetCustomerInvoices(id);
            decimal balance = CustomersManager.GetCustomerBalance(id);
            string cardType = string.Empty;

            ViewBag.Customer = customer;
            ViewBag.Invoices = invoices;
            ViewBag.Balance = balance;
            ViewBag.CardNumber = CustomersManager.GetLastFourDigitCreditCards(customer.Id, out cardType);
            ViewBag.CardType = cardType;

            return View();
        }
        public ActionResult AddCustomerNote(string customerId, string text)
        {
            int cId = 0;
            Int32.TryParse(customerId, out cId);
            UserDT user = GetLoggedUser();
            NoteDT note = CustomersManager.AddCustomerNote(cId, user.Id, text);
            ViewBag.Note = note;
            return View("NoteRow");
        }

        public string CancelCustomer(string customerId)
        {
            int id = 0;
            Int32.TryParse(customerId, out id);
            CustomersManager.CancelCustomer(id);
            return DateTime.Now.ToLongDateString();
        }
        public string ChangePassword(string customerId, string newPassword)
        {
            int id = 0;
            Int32.TryParse(customerId, out id);
            CustomersManager.ChangePassword(id, newPassword);
            return DateTime.Now.ToLongDateString();
        }

        public string ActivateCustomer(string customerId)
        {
            int id = 0;
            Int32.TryParse(customerId, out id);
            string nextBillDate = CustomersManager.ActivateCustomer(id);
            return nextBillDate;
        }

        public ActionResult Refund(string customerId, string invoiceId, string refundReason)
        {
            int cId = 0;
            Int32.TryParse(customerId, out cId);
            int iId = 0;
            Int32.TryParse(invoiceId, out iId);
            decimal amount = 0;
            TransactionDT transactionDT = CustomersManager.Refund(cId, iId, refundReason, out amount);

            ViewBag.Transaction = transactionDT;
            ViewBag.Amount = amount;

            return View("TransactionRow");
        }

        [HttpPost]
        public ActionResult TryChargeAgain(InvoiceVM model)
        {
            if ((!ModelState.IsValid) && (CastleClub.BusinessLogic.Data.GlobalParameters.TryChargeAgain))
            {
                return Json(new { Result = false });
            }

            bool resp = PaymentsManager.ProcessInvoice(model.Id);
            
            InvoiceDT invoice = PaymentsManager.GetInvoice(model.Id);
            TransactionDT transaction = invoice.Transactions.OrderByDescending(x => x.SubmitDate).FirstOrDefault();
           /* InvoiceDT invoice = null;
            TransactionDT transaction = null;
            resp = false;
            return Json(new { Result = false, Info = new { Authorize = "", SubmitDate = DateTime.Now.ToString(), Type = "", Message ="hola", Status = "FAIL", Amount = "222", Parent = string.Empty } }); */
            if (resp)
            {
                return Json(new { Result = true, Info = new { Authorize = transaction.AuthorizeTransactionId.ToString(), SubmitDate = transaction.SubmitDate.ToString(), Type = transaction.Type.ToString(), Message = transaction.Message, Status = transaction.Status.ToString(), Amount = invoice.Amount.ToString(), Parent = string.Empty } });
            }
            else
            {
                if (transaction != null)
                {
                    return Json(new { Result = false, Info = new { Authorize = transaction.AuthorizeTransactionId.ToString(), SubmitDate = transaction.SubmitDate.ToString(), Type = transaction.Type.ToString(), Message = transaction.Message, Status = transaction.Status.ToString(), Amount = invoice.Amount.ToString(), Parent = string.Empty } });
                }
                else
                {
                    return Json(new { Result = false });
                }
            }
        }

        public string UpdateBalance(string customerId)
        {
            int cId = 0;
            Int32.TryParse(customerId, out cId);
            decimal balance = CustomersManager.GetCustomerBalance(cId);
            return String.Format("{0:N2}", balance);
        }

        private UserDT GetLoggedUser()
        {
            return UsersManager.GetUserByAspNetId(User.Identity.GetUserId());
        }

        private string GenerateLastFourDigitCreditCard(int lastFourDigit)
        {
            if (lastFourDigit<0)
            {
                return null;
            }

            string lastFourDigitCreditCard = string.Empty;
            if (lastFourDigit < 10)
            {
                return (lastFourDigitCreditCard = "000" + lastFourDigit);
            }
            else if (lastFourDigit < 100)
            {
                return (lastFourDigitCreditCard = "00" + lastFourDigit);
            }
            else if (lastFourDigit < 1000)
            {
                return (lastFourDigitCreditCard = "0" + lastFourDigit);
            }
            else
            {
                return (lastFourDigitCreditCard = lastFourDigit.ToString());
            }
        }
    }

}