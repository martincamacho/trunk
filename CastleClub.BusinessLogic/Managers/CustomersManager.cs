using CastleClub.BusinessLogic.AuthorizeAPI;
using CastleClub.BusinessLogic.AuthorizeService;
using CastleClub.BusinessLogic.Data;
using CastleClub.BusinessLogic.Utils;
using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;
using CastleClub.DataTypes.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CastleClub.BusinessLogic.Managers
{
    public static class CustomersManager
    {
        public static bool EmailExists(string email, int siteId)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                return entities.Customers.Any(c => c.SiteId == siteId && c.Email == email);
            }
        }

        public static CustomerDT NewCustomer(SiteDT site, string email, string token, int ncId, int visitId, ReferrerDT referrer)
        {
            CustomerDT customer = new CustomerDT();
            CreditCardDT cc = new CreditCardDT();

            Hashtable ht = EncryptionHelper.Unpack(EncryptionHelper.Decrypt(token, referrer.SiteKey));
            string customerId = String.Empty;

            if (!EmailExists(email, site.Id))
            {

                if (ht.ContainsKey("CustomerID"))
                {
                    customerId = ht["CustomerID"].ToString();

                    string seData = RequestToPartsGeek(referrer, customerId);

                    if (seData.Length > 0)
                    {
                        Hashtable customerHT = EncryptionHelper.Unpack(EncryptionHelper.Decrypt(seData, referrer.SiteKey));

                        string creditCardType = CreditCardHelper.GetCardType(customerHT["CardNumber"].ToString());
                        if (creditCardType != "AMEX" && customerHT["Email"].ToString() == email)
                        {
                            customer.SiteId = site.Id;
                            customer.NcId = ncId;
                            customer.Email = customerHT["Email"].ToString();
                            customer.FirstName = customerHT["FirstName"].ToString();
                            customer.LastName = customerHT["LastName"].ToString();
                            customer.Phone = customerHT["Phone"].ToString();
                            customer.Address = customerHT["Address1"].ToString();
                            customer.City = customerHT["City"].ToString();
                            customer.StateId = customerHT["State"].ToString();
                            customer.ZipCode = customerHT["Zip"].ToString();

                            int expMonth = 0;
                            int expYear = 0;
                            Int32.TryParse(customerHT["CCMonth"].ToString(), out expMonth);
                            Int32.TryParse(customerHT["CCYear"].ToString(), out expYear);
                            cc.CardNumber = customerHT["CardNumber"].ToString();
                            cc.ExpDate = CreditCardHelper.GetExpDate(expMonth, expYear);
                            cc.CVV = String.Empty;
                            CCType cardType = CCType.VISA;
                            CCType.TryParse(creditCardType, out cardType);
                            cc.Type = cardType;

                            return NewCustomer(site, customer, cc, visitId, referrer.Id, true);
                        }
                    }
                }
            }
            throw new TokenErrorException();
        }

        public static string RequestToPartsGeek(ReferrerDT referrer, string customerId)
        {
            string tokenToSend = HttpUtility.UrlEncode(EncryptionHelper.Encrypt("SiteKey=" + referrer.SiteIdentifier + "&CustomerID=" + customerId, referrer.SiteKey));
            string rURL = referrer.PostBackURL + "?v=" + referrer.SiteValidator + "&t=" + tokenToSend;
            string seData = String.Empty;

            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("User-Agent", "GCD");
                byte[] data = wc.DownloadData(rURL);
                seData = Encoding.ASCII.GetString(data);

                //if (HttpContext.Current.IsDebuggingEnabled)
                //{
                //    if (Environment.MachineName == "VITAMINS7")
                //    {
                //        seData = "5dzlzZihDcwjll4SaTiTOvXrs7Ng5ciswlIOZWZgEZh3JCpDQyRx5UNcP1PDAQHWUSEYtzDfIOdjOS+VuRaV3fKx4shTXMdKzqCl7l5XirFcZlN0X40GKKP6AR1lU1e40J/7jIJxiKed/ZEUo69CqXLkCZeY8YaF5+e26fgttE8y6XK2UWPhy91C2K3lLera7hJRkuejRLDp2/8hRACnyExoFJRgx8D4t0tda5jfQb73Qii6KrB+14lzg/J+h1aM0pFOx5UVrxmP1ZUayoC0HA==";
                //    }
                //}
            }
            return seData;
        }

        public static CustomerDT NewCustomer(SiteDT site, CustomerDT customer, CreditCardDT cc, int visitId, int referrerId, bool emailForm)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                Customer newCustomer = new Customer();

                CreditCard newCreditCard = new CreditCard();

                Visit visit = null;

                newCreditCard.Data = string.Empty;
                newCreditCard.Successful = false;
                newCreditCard.CreatedAt = DateTime.Now;
                newCreditCard.LastFourDigit = string.Empty;
                newCreditCard.Type = cc.Type.ToString();

                Password p = new Password();
                customer.Password = p.SaltedPassword;
                customer.SaltKey = p.SaltKey;
                customer.ClearPassword = p.ClearPassword;

                newCustomer.SiteId = customer.SiteId;
                newCustomer.NcId = customer.NcId;
                newCustomer.Email = customer.Email;
                newCustomer.Password = customer.Password;
                newCustomer.UnEncryptPass = customer.ClearPassword;
                newCustomer.SaltKey = customer.SaltKey;
                newCustomer.FirstName = customer.FirstName;
                newCustomer.LastName = customer.LastName;
                newCustomer.Phone = customer.Phone != null ? customer.Phone : String.Empty;
                newCustomer.Address = customer.Address;
                newCustomer.City = customer.City;
                newCustomer.StateId = customer.StateId;
                newCustomer.ZipCode = customer.ZipCode;
                newCustomer.CreatedAt = DateTime.Now;
                newCustomer.Status = CustomerStatus.ACTIVE;
                newCustomer.Refunded = false;
                newCustomer.LastBillDate = null;
                newCustomer.NextBillDate = DateTime.Now.AddDays(30);
                newCustomer.BadLoginCount = 0;
                newCustomer.CreditCards.Add(newCreditCard);
                newCustomer.ReferrerId = referrerId;
                newCustomer.EmailForm = emailForm;

                entities.Customers.Add(newCustomer);
                entities.SaveChanges();

                customer.Id = newCustomer.Id;

                if (visitId != 0)
                {
                    visit = entities.Visits.Where(v => v.Id == visitId).FirstOrDefault();
                    if (visit != null)
                    {
                        visit.CustomerId = newCustomer.Id;
                        entities.SaveChanges();
                    }
                }

                long customerProfileId = 0;
                long customerPaymentProfileId = 0;
                bool succesfull = CIM.CreateCustomerProfile(site, customer, cc, out customerProfileId, out customerPaymentProfileId);

                if (succesfull)
                {
                    string crediCardData = "ccNum:" + cc.CardNumber + ";cVV:" + cc.CVV + ";expDate:" + cc.ExpDate;
                    newCreditCard.Data = EncryptionHelper.EncryptRSACertificate(crediCardData);
                    newCreditCard.LastFourDigit = cc.CardNumber.Substring(cc.CardNumber.Length - 4, 4);

                    newCustomer.AuthorizeProfileId = customerProfileId;
                    newCreditCard.AuthorizePaymentProfileId = customerPaymentProfileId;
                    newCreditCard.Successful = true;
                    entities.SaveChanges();
                    if (CastleClub.BusinessLogic.Data.GlobalParameters.AMTNewPlatform)
                    {
                        string response = NewActivateMember(SitesManager.GetSite(site.Id), CustomersManager.GetCustomer(customer.Id), "OPEN");
                    }
                    else
                    {
                        ActivateMember(site, customer);
                    }


                    //SEND EMAIL
                    int capEmail = GlobalParameters.CapEmail;

                    if ((site.SendWelcomeEmail) && (site.WelcomeEmailDelay != null) && (site.WelcomeEmailDelay == 0) && ((capEmail == 0) || (SitesManager.GetEmailSentToday() < capEmail)))
                    {
                        string smtpAddress = site.SmtpAddress;//CastleClub.BusinessLogic.Data.GlobalParameters.Smtp;
                        //var siteDT = CastleClub.BusinessLogic.Managers.SitesManager.GetSite(customerDT.SiteId);

                        string emailFrom = site.Email;
                        string passwordEmail = site.PasswordEmail;//CastleClub.BusinessLogic.Data.GlobalParameters.EmailPassword;
                        string subject = string.Format(Properties.Resources.WelcomeSubjectEmail, site.Name);
                        // + " " + customer.LastName
                        string body = string.Format(Properties.Resources.WelcomeBodyEmail, customer.FirstName,
                                                    customer.Email, newCustomer.UnEncryptPass, site.Name, site.Name, "PartsGeek",
                                                    site.Price, site.PricePerQuarter, site.Phone, site.Email, site.OfferDomain);

                        Utils.Email.SendEmailWithBCC(emailFrom, passwordEmail, smtpAddress, subject, body, new List<string>() { customer.Email }, new List<string>() { site.WelcomeEmailBCC }, true);

                        newCustomer.WelcomeEmail = true;
                        newCustomer.WelcomeEmailDelay = 0;
                        newCustomer.WelcomeEmailSent = DateTime.Now;
                        //newCustomer.UnEncryptPass = "";
                        entities.SaveChanges();

                    }
                }
                else
                {
                    if (visit != null)
                    {
                        entities.Visits.Remove(visit);
                    }
                    entities.CreditCards.Remove(newCreditCard);
                    entities.Customers.Remove(newCustomer);
                    entities.SaveChanges();

                    throw new Exception();
                }

                return customer;
            }
        }

        public static CustomerDT NewCustomer(string groupID, CustomerDT customerDT, string creditCardNumber, string creditCardExpDate)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                if (string.IsNullOrEmpty(creditCardNumber) || !Utils.CreditCardHelper.Valid(creditCardNumber, false))
                {
                    throw new Exception("The credit card number is invalid.");
                }

                Site site = entities.Sites.FirstOrDefault(x => x.GroupId == groupID);
                if (site == null)
                {
                    throw new Exception("Not exists site with this group id:" + groupID);
                }

                if (EmailExists(customerDT.Email, site.Id))
                {
                    throw new Exception("Already exists a customer with email " + customerDT.Email + "registered on site with this group id:" + groupID);
                }
                string creditCardType = CreditCardHelper.GetCardType(creditCardNumber);
                if (creditCardType != "AMEX")
                {
                    Customer customer = new Customer()
                    {
                        CreatedAt = customerDT.CreatedAt,
                        Email = customerDT.Email,
                        SiteId = site.Id,
                        Status = CustomerStatus.ACTIVE,
                        StatusId = "ACTIVE",
                        NextBillDate = customerDT.NextBillDate,
                        Address = customerDT.Address,
                        BadLoginCount = 0,
                        CancelledDate = null,
                        City = customerDT.City,
                        FirstName = customerDT.FirstName,
                        LastBillDate = null,
                        LastName = customerDT.LastName,
                        NcId = 0,
                        Password = string.Empty,
                        Phone = customerDT.Phone,
                        Refunded = false,
                        SaltKey = 0,
                        StateId = customerDT.StateId,
                        ZipCode = customerDT.ZipCode
                    };

                    Password password = new Password(customerDT.ClearPassword);
                    customer.Password = password.SaltedPassword;
                    customer.SaltKey = password.SaltKey;
                    customer.UnEncryptPass = customerDT.ClearPassword;

                    CreditCard creditCard = new CreditCard()
                    {
                        CreatedAt = DateTime.Now,
                        Customer = customer,
                        Data = EncryptionHelper.EncryptRSACertificate("ccNum:" + creditCardNumber + ";cVV:;cExp:" + creditCardExpDate),
                        LastFourDigit = creditCardNumber.Substring(creditCardNumber.Length - 4, 4),
                        Transactions = new List<Transaction>(),
                        Type = Utils.CreditCardHelper.GetCardType(creditCardNumber),
                        Successful = false
                    };

                    long customerProfile;
                    long customerPaymentProfile;

                    var creditCardDT = creditCard.GetDT();
                    creditCardDT.CardNumber = creditCardNumber;
                    creditCardDT.ExpDate = creditCardExpDate;

                    if (CIM.CreateCustomerProfile(site.GetDT(), customer.GetDT(false), creditCardDT, out customerProfile, out customerPaymentProfile))
                    {
                        customer.AuthorizeProfileId = customerProfile;
                        creditCard.AuthorizePaymentProfileId = customerPaymentProfile;
                        creditCard.Successful = true;

                        //var siteDT = CastleClub.BusinessLogic.Managers.SitesManager.GetSite(site.Id);
                        int capEmail = GlobalParameters.CapEmail;
                        if ((site.SendWelcomeEmail == true) && (site.WelcomeEmailDelay != null) && (site.WelcomeEmailDelay == 0) && ((capEmail == 0) || (SitesManager.GetEmailSentToday() < capEmail)))
                        //send welcome email if it hasn't a delay
                        {
                            string smtpAddress = site.SmtpAddress;//CastleClub.BusinessLogic.Data.GlobalParameters.Smtp;
                            //var siteDT = CastleClub.BusinessLogic.Managers.SitesManager.GetSite(customerDT.SiteId);

                            string emailFrom = site.Email;
                            string passwordEmail = site.PasswordEmail;//CastleClub.BusinessLogic.Data.GlobalParameters.EmailPassword;
                            string subject = string.Format(Properties.Resources.WelcomeSubjectEmail, site.Name);
                            // + " " + customer.LastName
                            string body = string.Format(Properties.Resources.WelcomeBodyEmail, customer.FirstName,
                                                        customer.Email, customer.UnEncryptPass, site.Name, site.Name, "PartsGeek",
                                                        site.Price, site.PricePerQuarter, site.Phone, site.Email, site.OfferDomain);

                            //Utils.Email.SendEmail(emailFrom, passwordEmail, smtpAddress, subject, body, new List<string>() { customer.Email }, true);
                            Utils.Email.SendEmailWithBCC(emailFrom, passwordEmail, smtpAddress, subject, body, new List<string>() { customer.Email }, new List<string>() { site.WelcomeEmailBCC }, true);

                            customer.WelcomeEmail = true;
                            customer.WelcomeEmailDelay = 0;
                            customer.WelcomeEmailSent = DateTime.Now;
                            //customer.UnEncryptPass = "";
                        }

                        entities.Customers.Add(customer);
                        entities.CreditCards.Add(creditCard);


                        entities.SaveChanges();
                        return customer.GetDT(false);
                    }
                }
                return null;
            }
        }

        private static void ActivateMember(SiteDT site, CustomerDT customer)
        {
            using (WebClient wc = new WebClient())
            {
                string url = CastleClub.BusinessLogic.Data.GlobalParameters.AccessDActivationURL;
                StringBuilder finalURL = new StringBuilder();

                //finalURL = finalURL.AppendFormat(url, site.GroupId, customer.Id.ToString(), customer.FirstName, customer.LastName, customer.Address,
                //   customer.City, customer.StateId, "US", customer.ZipCode, customer.Phone);
                string fakeEmail = customer.Id.ToString().Trim() + "@goldclubdiscounts.com";
                finalURL = finalURL.AppendFormat(url, site.GroupId, customer.Id.ToString(), customer.FirstName, customer.LastName, "", "", "", "US", customer.ZipCode, "", "");
                String response = String.Empty;
                try
                {
                    EventViewer.Writte("BUSINESS LOGIC", "add member", finalURL.ToString(), System.Diagnostics.EventLogEntryType.Warning);

                    Email.SendEmail("santiago.duarte@abstracta.com.uy", "Nov$2014", "smtp.gmail.com", "ADD MEMBERS URL", finalURL.ToString(), new List<string>() { "dario.deleon@abstracta.com.uy" }, true);
                    byte[] data = wc.DownloadData(finalURL.ToString());

                    response = Encoding.ASCII.GetString(data);

                    Email.SendEmail("santiago.duarte@abstracta.com.uy", "Nov$2014", "smtp.gmail.com", "ADD MEMBERS RESPONSE", response, new List<string>() { "dario.deleon@abstracta.com.uy" }, true);
                    
                }
                catch { }
            }
        }
        /*
        private static void ActivateMember(SiteDT site, CustomerDT customer, StreamWriter file)
        {
            using (WebClient wc = new WebClient())
            {
                string url = CastleClub.BusinessLogic.Data.GlobalParameters.AccessDActivationURL;
                StringBuilder finalURL = new StringBuilder();

                //finalURL = finalURL.AppendFormat(url, site.GroupId, customer.Id.ToString(), customer.FirstName, customer.LastName, customer.Address,
                //   customer.City, customer.StateId, "US", customer.ZipCode, customer.Phone);
                string fakeEmail = customer.Id.ToString().Trim() + "@goldclubdiscounts.com";
                finalURL = finalURL.AppendFormat(url, site.GroupId, customer.Id.ToString(), "", "", "", "", "", "US", "", "", fakeEmail);
                file.WriteLine("REQUEST " + finalURL.ToString());
                String response = String.Empty;
                try
                {
                    //CastleClub.BusinessLogic.Utils.EventViewer.Writte("BUSINESS LOGIC", "add member", finalURL.ToString(), System.Diagnostics.EventLogEntryType.Warning);

                    ///Utils.Email.SendEmail("dario.deleon@abstracta.com.uy", "Nov$2014", "smtp.gmail.com", "ADD MEMBERS URL", finalURL.ToString() , new List<string>() { "dario.deleon@abstracta.com.uy" }, true);
                    byte[] data = wc.DownloadData(finalURL.ToString());

                    response = Encoding.ASCII.GetString(data);
                    file.WriteLine("RESPONSE " + response);
                    ///Utils.Email.SendEmail("dario.deleon@abstracta.com.uy", "Nov$2014", "smtp.gmail.com", "ADD MEMBERS RESPONSE", response, new List<string>() { "dario.deleon@abstracta.com.uy" }, true);
                    //CastleClub.BusinessLogic.Utils.EventViewer.Writte("BUSINESS LOGIC", "add member", finalURL.ToString(), System.Diagnostics.EventLogEntryType.Warning);
                }
                catch { }
            }
        }*/
        public static int RemoveEmailInAccess()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                List<Customer> customers = new List<Customer>();
                //int customersToDelete = GlobalParameters.DebtorsToCancel;
                customers = entities.Customers.Where(x => !x.CancelledDate.HasValue && !string.IsNullOrEmpty(x.Email)
                   && !string.IsNullOrEmpty(x.FirstName) && x.StatusId == "ACTIVE" && !string.IsNullOrEmpty(x.LastName) && x.Id > 3191871).ToList();
                StreamWriter file = new StreamWriter("LogAccess.txt");
                int count = customers.Count;
                foreach (var item in customers)
                {

                    CustomersManager.ActivateMember(SitesManager.GetSite(item.SiteId), item.GetDT(false));//, file);

                }
                file.Close();
                return count;
            }
        }
        public static string NewActivateMemberAllActives()//SiteDT site, CustomerDT customer, string statusInAccess)
        {
            /***
             * 
             * COMENTO ESTA PARTE PARA PROBAR*
            string postData = "";
            using (CastleClubEntities entities = new CastleClubEntities())
            {

                    bool firstTime = true;
                        
                    List<CustomerDT> customers = entities.Customers.Where(c => c.StatusId == "ACTIVE").ToList().Select(c => c.GetDT(false)).ToList();
                    
                    postData = "{\"import\":{" +
                           "\"members\":[";
                    foreach (CustomerDT customer in customers)
                    {
                        if (!firstTime)
                        {
                            postData += ",";
                        }
                        else
                        {
                            firstTime = false;
                        }
                        postData += "{ \"organization_customer_identifier\":\"WEJ5G\"," +
                            "\"program_customer_identifier\":\"" + SitesManager.GetSite(customer.SiteId).GroupId + "\"," +
                            "\"first_name\":\"" + customer.FirstName + "\"," +
                            "\"last_name\":\"" + customer.LastName + "\"," +
                            "\"email_address\":\"" + customer.Email + "\"," +
                            "\"member_customer_identifier\":\"" + customer.Id + "\"," +
                            "\"member_status\":\"" + "OPEN" + "\"," +
                            "\"record_identifier\":\"" + customer.Id + "\"," +
                            "\"city\":\"" + customer.City + "\"," +
                            "\"state\":\"" + customer.StateId + "\"," +
                            "\"postal_code \":\"" + customer.ZipCode + "\"" +
                        "}";
                                   
                    }
                    postData +=  "]" +
                                "}}";
                    
            }
             
            //para probar 
            return SendAccessJSonCustomer(postData);
           ****/
            return NewActivateMember(SitesManager.GetSite(12), CustomersManager.GetCustomer(2251193), "OPEN");
        }

        private static string SendAccessJSonCustomer(string postData)
        {
            string responseText = String.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;            
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(CastleClub.BusinessLogic.Data.GlobalParameters.AMTActivationURL);
            request.ProtocolVersion = HttpVersion.Version11;           
            request.Method = "POST";
            request.Headers.Add("Access-Token", CastleClub.BusinessLogic.Data.GlobalParameters.AMTAccessToken);
            request.ContentType = "application/json";
            request.Accept = "application/json"; //"application/json";
            /***/
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            System.Net.WebResponse response = request.GetResponse();
            Console.WriteLine(((System.Net.HttpWebResponse)response).StatusDescription);
            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            Console.WriteLine(responseFromServer);
            reader.Close();
            dataStream.Close();
            response.Close();

            //model.response = responseFromServer;
            return responseFromServer;
            /*  
           return "";***/
        }      
        public static string NewActivateMember(SiteDT site, CustomerDT customer, string statusInAccess) //statusInAccess = {"OPEN", "CLOSED"}
        {
            string postData = "{\"import\":{" +
                                    "\"members\":[" +
                                    "{ \"organization_customer_identifier\":\"WEJ5G\"," +
                                        "\"program_customer_identifier\":\"" + site.GroupId + "\"," +
                                        "\"first_name\":\"" + customer.FirstName + "\"," +
                                        "\"last_name\":\"" + customer.LastName + "\"," +
                                        "\"email_address\":\"" + "cust_" + customer.Id.ToString().Trim() +"@"+ site.OfferDomain.ToString().Trim() + "\"," +
                                        "\"member_customer_identifier\":\"" + customer.Id + "\"," +
                                        "\"member_status\":\"" + statusInAccess + "\"," +
                                        "\"record_identifier\":\"" + customer.Id + "\"," +
                                        "\"city\":\"" + customer.City + "\"," +
                                        "\"state\":\"" + customer.StateId + "\"," +
                                        "\"postal_code \":\"" + customer.ZipCode + "\"" +
                                    "}" +
                                    "]" +
                                "}}";
            return SendAccessJSonCustomer(postData);
        }    

        public static List<CustomerDT> GetCustomers(int siteId, int memeberId, string email, string phone, string name, string lastFourDigit)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                bool any = siteId == 0;
                if (memeberId != 0)
                {
                    return entities.Customers.Where(c => c.Id == memeberId && (c.SiteId == siteId || any)).ToList().Select(c => c.GetDT(false)).ToList();
                }
                else
                {
                    return entities.Customers.Where(c =>
                        ((c.CreditCards.Any() && c.CreditCards.FirstOrDefault().LastFourDigit == lastFourDigit) || string.IsNullOrEmpty(lastFourDigit)) &&
                        (c.Email.ToLower().Contains(email.ToLower()) || email == null) &&
                        (c.Phone.ToLower().Replace("-", "").Replace(" ", "").Contains(phone.ToLower().Replace("-", "").Replace(" ", "")) || phone == null) &&
                        (c.FirstName.ToLower().Contains(name.ToLower()) || c.LastName.ToLower().Contains(name.ToLower()) || name == null) &&
                        (c.SiteId == siteId || any)
                        ).ToList().Select(c => c.GetDT(false)).ToList();
                }
            }
        }

        public static CustomerDT GetCustomer(int id)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                Customer customer = entities.Customers.Where(c => c.Id == id).FirstOrDefault();
                if (customer == null)
                {
                    throw new InvalidCustomerException();
                }
                return customer.GetDT(true);
            }
        }

        public static List<InvoiceDT> GetCustomerInvoices(int id)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                Customer customer = entities.Customers.Where(c => c.Id == id).FirstOrDefault();
                if (customer == null)
                {
                    throw new InvalidCustomerException();
                }
                return customer.Invoices.ToList().Select(t => t.GetDT()).ToList();
            }
        }

        public static decimal GetCustomerBalance(int id)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                Customer customer = entities.Customers.Where(c => c.Id == id).FirstOrDefault();
                if (customer == null)
                {
                    throw new InvalidCustomerException();
                }
                return customer.Invoices.Where(i => i.Status == InvoiceStatus.BILLED || i.Status == InvoiceStatus.REFUNDEDFAIL).Sum(i => i.Amount);
            }
        }

        public static NoteDT AddCustomerNote(int cId, int uId, string text)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                Note note = new Note();
                note.CustomerId = cId;
                note.User = entities.Users.Where(u => u.Id == uId).First();
                note.Text = text;
                note.CreatedAt = DateTime.Now;

                entities.Notes.Add(note);
                entities.SaveChanges();

                return note.GetDT();
            }
        }

        public static void CancelCustomer(int id)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                Customer customer = entities.Customers.Where(c => c.Id == id).FirstOrDefault();
                if (customer == null)
                {
                    throw new InvalidCustomerException();
                }
                customer.Status = CustomerStatus.CANCELLED;
                customer.CancelledDate = DateTime.Now;
                foreach (Invoice invoice in customer.Invoices.Where(i => i.StatusId == InvoiceStatus.NEW.ToString() || i.StatusId == InvoiceStatus.BILLEDFAIL.ToString()))
                {
                    invoice.StatusId = InvoiceStatus.CANCELED.ToString();
                }
                entities.SaveChanges();

                if (CastleClub.BusinessLogic.Data.GlobalParameters.AMTNewPlatform)
                {
                    string response = NewActivateMember(SitesManager.GetSite(customer.Site.Id), CustomersManager.GetCustomer(id), "SUSPEND");
                }
            }
        }

        public static void ChangePassword(int id, string newPassword)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                Customer customer = entities.Customers.Where(c => c.Id == id).FirstOrDefault();
                if (customer == null)
                {
                    throw new InvalidCustomerException();
                }
                Password p = new Password(newPassword);
                customer.Password = p.SaltedPassword;
                customer.UnEncryptPass = p.ClearPassword;
                //customer.
                customer.SaltKey = p.SaltKey;

                entities.SaveChanges();
            }
        }

        public static string ActivateCustomer(int id)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                Customer customer = entities.Customers.Where(c => c.Id == id).FirstOrDefault();
                if (customer == null)
                {
                    throw new InvalidCustomerException();
                }
                customer.Status = CustomerStatus.ACTIVE;
                customer.CancelledDate = null;
                entities.SaveChanges();
                return customer.NextBillDate.ToLongDateString();
            }
        }

        public static TransactionDT Refund(int cId, int iId, string refundReason, out decimal amount)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                Invoice invoice = entities.Invoices.Where(i => i.Id == iId && i.CustomerId == cId).FirstOrDefault();
                if (invoice == null)
                {
                    throw new InvalidInvoiceException();
                }
                Transaction RefundedTransaction = invoice.Transactions.Where(t => t.TypeId == TransactionType.SALE.ToString() && t.StatusId == TransactionStatus.SUCCESFULL.ToString()).FirstOrDefault();
                if (RefundedTransaction == null)
                {
                    throw new InvalidTransactionException();
                }

                SiteDT siteDT = invoice.Customer.Site.GetDT();
                InvoiceDT invoiceDT = invoice.GetDT();
                TransactionDT transactionDT = RefundedTransaction.GetDT();
                CustomerDT customerDT = invoice.Customer.GetDT(false);
                CreditCardDT creditCardDT = invoice.Customer.CreditCards.FirstOrDefault().GetDT();

                Transaction transaction = new Transaction();
                transaction.InvoiceId = invoiceDT.Id;
                transaction.CreditCardId = creditCardDT.Id;
                transaction.AuthorizeTransactionId = 0;
                transaction.Type = TransactionType.REFUND;
                transaction.Status = TransactionStatus.FAILED;
                transaction.SubmitDate = DateTime.Now;
                transaction.RefundedTransactionId = transactionDT.Id;

                entities.Transactions.Add(transaction);
                entities.SaveChanges();

                long transactionId = 0;
                string message = "";

                bool succesfull = CIM.CreateCustomerProfileTransactionRefund(siteDT, invoiceDT, transactionDT, customerDT, creditCardDT, out transactionId, out message);

                if (succesfull)
                {
                    invoice.Status = InvoiceStatus.REFUNDED;
                    invoice.RefundReason = refundReason;
                    invoice.RefundedDate = DateTime.Now;

                    invoice.Customer.Refunded = true;

                    transaction.Status = TransactionStatus.SUCCESFULL;
                    transaction.AuthorizeTransactionId = transactionId;

                    invoice.Customer.Referrer.RefundAmount += invoice.Amount;
                    entities.Database.CommandTimeout = 180;              
                    entities.SaveChanges();
                }
                else
                {
                    transaction.Message = message;
                    invoice.Status = InvoiceStatus.REFUNDEDFAIL;
                    entities.SaveChanges();
                }

                amount = transaction.Invoice.Amount;
                return transaction.GetDT();
            }
        }

        public static List<CustomerDT> GetFilterCustomers(string letter, int page, int pageSize, string word, bool all, bool onlyActive, string siteName, out int totalPage)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                int totalCustomers = 0;
                List<Customer> customers = new List<Customer>();
                bool allSite = ((string.IsNullOrEmpty(siteName)) || (siteName == "0"));
                if (!string.IsNullOrEmpty(word) && page > 0)
                {
                    if (all)
                    {
                        customers = entities.Customers.Where(c => (c.CancelledDate == null || (!onlyActive))
                            && (allSite || c.Site.Name == siteName)
                            && (c.FirstName.ToLower().Contains(word.ToLower())
                            || c.LastName.ToLower().Contains(word.ToLower()) || c.Email.ToLower().Contains(word.ToLower()))).OrderBy(c => c.LastName).ToList();
                        totalCustomers = entities.Customers.Where(c => (c.CancelledDate == null || (!onlyActive)) && (allSite || c.Site.Name == siteName) && (c.LastName.ToLower().Contains(word.ToLower()))).Count();
                    }
                    else
                    {
                        //Filter for word and page.
                        customers = entities.Customers.Where(c => (c.CancelledDate == null || (!onlyActive))
                            && (allSite || c.Site.Name == siteName)
                            && (c.FirstName.ToLower().Contains(word.ToLower())
                            || c.LastName.ToLower().Contains(word.ToLower()) || c.Email.ToLower().Contains(word.ToLower()))).OrderBy(c => c.LastName).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
                        totalCustomers = entities.Customers.Where(c => (c.CancelledDate == null || (!onlyActive)) && (allSite || c.Site.Name == siteName) && (c.LastName.ToLower().Contains(word.ToLower()))).Count();
                    }
                }
                else if (page > 0)
                {
                    if (all)
                    {
                        if (string.IsNullOrEmpty(letter))
                        {
                            customers = entities.Customers.Where(x => (x.CancelledDate == null || (!onlyActive)) && (allSite || x.Site.Name == siteName)).OrderBy(c => c.LastName).ToList();
                            totalCustomers = entities.Customers.Where(x => (x.CancelledDate == null || (!onlyActive)) && (allSite || x.Site.Name == siteName)).Count();
                        }
                        else
                        {
                            customers = entities.Customers.Where(c => (c.CancelledDate == null || (!onlyActive)) && (allSite || c.Site.Name == siteName) && (c.LastName.ToLower().StartsWith(letter.ToLower()))).OrderBy(c => c.LastName).ToList();
                            totalCustomers = entities.Customers.Where(c => (c.CancelledDate == null || (!onlyActive)) && (allSite || c.Site.Name == siteName) && (c.LastName.ToLower().StartsWith(letter.ToLower()))).Count();
                        }
                    }
                    else if (string.IsNullOrEmpty(letter))
                    {
                        customers = entities.Customers.Where(x => (x.CancelledDate == null || (!onlyActive)) && (allSite || x.Site.Name == siteName)).OrderBy(c => c.LastName).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
                        totalCustomers = entities.Customers.Where(x => (x.CancelledDate == null || (!onlyActive)) && (allSite || x.Site.Name == siteName)).Count();
                    }
                    else
                    {
                        customers = entities.Customers.Where(c => (c.CancelledDate == null || (!onlyActive)) && (allSite || c.Site.Name == siteName) && (c.LastName.ToLower().StartsWith(letter.ToLower()))).OrderBy(c => c.LastName).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
                        totalCustomers = entities.Customers.Where(c => (c.CancelledDate == null || (!onlyActive)) && (allSite || c.Site.Name == siteName) && (c.LastName.ToLower().StartsWith(letter.ToLower()))).Count();
                    }
                }


                List<CustomerDT> customersDT = new List<CustomerDT>();
                foreach (var c in customers)
                {
                    CustomerDT customerDT = new CustomerDT()
                    {
                        Address = c.Address,
                        AuthorizeProfileId = c.AuthorizeProfileId,
                        BadLoginCount = c.BadLoginCount,
                        City = c.City,
                        CreatedAt = c.CreatedAt,
                        NcId = c.NcId,
                        NextBillDate = c.NextBillDate,
                        Password = c.Password,
                        Phone = c.Phone,
                        Refunded = c.Refunded,
                        Site = c.Site.Name,
                        SiteId = c.SiteId,
                        StateId = c.StateId,
                        Id = c.Id,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        ZipCode = c.ZipCode,
                        Email = c.Email,
                        Status = c.Status,
                        CancelledDate = c.CancelledDate
                    };
                    customersDT.Add(customerDT);
                }

                totalPage = totalCustomers;
                return customersDT;
            }
        }

        public static string GetFilterCustomersToExcel(string letter, int page, int pageSize, string word, bool all, bool onlyActive, string siteName)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                List<Customer> customers = new List<Customer>();
                if (!string.IsNullOrEmpty(word) && page > 0)
                {
                    if (all)
                    {
                        customers = entities.Customers.Where(c => ((c.CancelledDate == null || !onlyActive))
                            && (string.IsNullOrEmpty(siteName) || siteName == "0" || siteName == c.Site.Name)
                            && ((c.FirstName.ToLower().Contains(word.ToLower())
                            || c.LastName.ToLower().Contains(word.ToLower()) || c.Email.ToLower().Contains(word.ToLower())))).OrderBy(c => c.LastName).ToList();
                    }
                    else
                    {
                        //Filter for word and page.
                        customers = entities.Customers.Where(c => ((c.CancelledDate == null || !onlyActive))
                            && (string.IsNullOrEmpty(siteName) || siteName == "0" || siteName == c.Site.Name)
                            && (c.FirstName.ToLower().Contains(word.ToLower())
                            || c.LastName.ToLower().Contains(word.ToLower()) || c.Email.ToLower().Contains(word.ToLower()))).OrderBy(c => c.LastName).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
                    }
                }
                else if (page > 0)
                {
                    if (all)
                    {
                        if (string.IsNullOrEmpty(letter))
                        {
                            customers = entities.Customers.Where(c => ((c.CancelledDate == null || !onlyActive))
                            && (string.IsNullOrEmpty(siteName) || siteName == "0" || siteName == c.Site.Name)).OrderBy(c => c.LastName).ToList();
                        }
                        else
                        {
                            customers = entities.Customers.Where(c => ((c.CancelledDate == null || !onlyActive))
                            && (string.IsNullOrEmpty(siteName) || siteName == "0" || siteName == c.Site.Name) && (c.LastName.ToLower().StartsWith(letter.ToLower()))).OrderBy(c => c.LastName).ToList();
                        }
                    }
                    else if (string.IsNullOrEmpty(letter))
                    {
                        customers = entities.Customers.Where(c => ((c.CancelledDate == null || !onlyActive))
                            && (string.IsNullOrEmpty(siteName) || siteName == "0" || siteName == c.Site.Name)).OrderBy(c => c.LastName).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
                    }
                    else
                    {
                        customers = entities.Customers.Where(c => ((c.CancelledDate == null || !onlyActive))
                            && (string.IsNullOrEmpty(siteName) || siteName == "0" || siteName == c.Site.Name) && (c.LastName.ToLower().StartsWith(letter.ToLower()))).OrderBy(c => c.LastName).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
                    }
                }

                List<List<string>> body = new List<List<string>>();
                for (int i = 0; i < customers.Count; i++)
                {
                    List<string> bodyList = new List<string>()
                    {
                        customers[i].Id.ToString(),
                        customers[i].CreatedAt.ToString(),
                        customers[i].FirstName+" "+customers[i].LastName,
                        "View",
                        customers[i].Email,
                        customers[i].Address,
                        customers[i].City,
                        customers[i].StateId
                    };

                    string creditCardsNumbers = string.Empty;
                    foreach (var item in customers[i].CreditCards)
                    {
                        creditCardsNumbers = string.IsNullOrEmpty(creditCardsNumbers) ? item.LastFourDigit : creditCardsNumbers + " - " + item.LastFourDigit;
                    }
                    bodyList.Add(creditCardsNumbers);
                    bodyList.Add(customers[i].Site.Name);
                    bodyList.Add(customers[i].CancelledDate.HasValue ? "FALSE" : "TRUE");
                    bodyList.Add(customers[i].CancelledDate.HasValue ? customers[i].CancelledDate.Value.ToShortDateString() : string.Empty);
                    bodyList.Add(customers[i].CreatedAt.ToString("MM/dd/yyyy"));

                    body.Add(bodyList);
                }

                string file = Utils.Excel.GenerateExcelFile(body);
                return file;
            }
        }

        public static int GetCustomersCount()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                return entities.Customers.Where(x => !x.CancelledDate.HasValue && !string.IsNullOrEmpty(x.Email)
                    && !string.IsNullOrEmpty(x.FirstName) && !string.IsNullOrEmpty(x.LastName)).Count();
            }
        }

        public static int GetDebtorsCount()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                return entities.Customers.Where(x => !x.CancelledDate.HasValue && !string.IsNullOrEmpty(x.Email)
                    && !string.IsNullOrEmpty(x.FirstName) && !string.IsNullOrEmpty(x.LastName) && (x.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL")).Count();
            }
        }

        public static string GetLastFourDigitCreditCards(int customerId, out string type)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                string creditCards = string.Empty;
                type = string.Empty;

                var customer = entities.Customers.FirstOrDefault(x => x.Id == customerId);
                if (customer != null)
                {
                    foreach (var creditCard in customer.CreditCards)
                    {
                        creditCards = string.IsNullOrEmpty(creditCards) ? creditCard.LastFourDigit : creditCards + " - " + creditCard.LastFourDigit;
                        if (!string.IsNullOrEmpty(creditCard.Type))
                        {
                            type = string.IsNullOrEmpty(type) ? creditCard.Type : type + " - " + creditCard.Type;
                        }
                    }
                }

                return creditCards;
            }
        }

        public static List<string> GetAllEmails(int siteID)
        {
            List<string> resp = new List<string>();

            using (CastleClubEntities entities = new CastleClubEntities())
            {
                resp = entities.Customers.Where(x => siteID == 0 || x.SiteId == siteID).Select(x => x.Email).ToList();
            }

            return resp;
        }

        public static List<CustomerDT> GetCustomersPerSite(int siteID, string customerEmail)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                return entities.Customers.Where(x => (siteID == 0 || x.SiteId == siteID) && x.Email.ToLower() == customerEmail.ToLower()).ToList().Select(x => x.GetDT(false)).ToList();
            }
        }

        public static void MembershipAgeCountRange(DateTime from, DateTime to, int siteID, out int countActive, out int countUnactive, out int total)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                countActive = entities.Customers.Count(x => (siteID == 0 || x.SiteId == siteID) && x.CreatedAt >= from && x.CreatedAt <= to && x.CancelledDate == null);
                countUnactive = entities.Customers.Count(x => (siteID == 0 || x.SiteId == siteID) && x.CreatedAt >= from && x.CreatedAt <= to && x.CancelledDate != null);
                total = entities.Customers.Count(x => (siteID == 0 || x.SiteId == siteID));
            }
        }

        public static List<CustomerDT> GetDebtorsCustomers(int siteId, int memberId, string email, string phone, string name, string lastFourDigit)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                bool any = siteId == 0;
                if (memberId != 0)
                {
                    return entities.Customers.Where(c => c.Id == memberId && (c.SiteId == siteId || any) && (c.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL")).ToList().Select(c => c.GetDT(false)).ToList();
                }
                else
                {
                    return entities.Customers.Where(c => (
                        ((c.CreditCards.Any() && c.CreditCards.FirstOrDefault().LastFourDigit == lastFourDigit) || string.IsNullOrEmpty(lastFourDigit)) &&
                        (c.Email.ToLower().Contains(email.ToLower()) || email == null) &&
                        (c.Phone.ToLower().Replace("-", "").Replace(" ", "").Contains(phone.ToLower().Replace("-", "").Replace(" ", "")) || phone == null) &&
                        (c.FirstName.ToLower().Contains(name.ToLower()) || c.LastName.ToLower().Contains(name.ToLower()) || name == null) &&
                        (c.SiteId == siteId || any) &&
                        (c.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL")
                        )).ToList().Select(c => c.GetDT(false)).ToList();
                }
            }
        }

        public static List<CustomerDT> GetDebtorsCustomers(string letter, int page, int pageSize, string word, bool all, bool onlyActive, string siteName, out int totalPage)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                int totalCustomers = 0;
                List<Customer> customers = new List<Customer>();
                bool allSite = ((string.IsNullOrEmpty(siteName)) || (siteName == "0"));
                if (!string.IsNullOrEmpty(word) && page > 0)
                {
                    if (all)
                    {
                        customers = entities.Customers.Where(c => (c.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL") && ((c.CancelledDate == null || (!onlyActive))
                            && (allSite || c.Site.Name == siteName)
                            && (c.FirstName.ToLower().Contains(word.ToLower())
                            || c.LastName.ToLower().Contains(word.ToLower()) || c.Email.ToLower().Contains(word.ToLower())))).OrderBy(c => c.LastName).ToList();
                        totalCustomers = entities.Customers.Where(c => (c.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL") && ((c.CancelledDate == null || (!onlyActive)) && (allSite || c.Site.Name == siteName) && (c.LastName.ToLower().Contains(word.ToLower())))).Count();
                    }
                    else
                    {
                        //Filter for word and page.
                        customers = entities.Customers.Where(c => (c.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL") && ((c.CancelledDate == null || (!onlyActive))
                            && (allSite || c.Site.Name == siteName)
                            && (c.FirstName.ToLower().Contains(word.ToLower())
                            || c.LastName.ToLower().Contains(word.ToLower()) || c.Email.ToLower().Contains(word.ToLower())))).OrderBy(c => c.LastName).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
                        totalCustomers = entities.Customers.Where(c => (c.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL") && ((c.CancelledDate == null || (!onlyActive)) && (allSite || c.Site.Name == siteName) && (c.LastName.ToLower().Contains(word.ToLower())))).Count();
                    }
                }
                else if (page > 0)
                {
                    if (all)
                    {
                        if (string.IsNullOrEmpty(letter))
                        {
                            customers = entities.Customers.Where(x => (x.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL") && ((x.CancelledDate == null || (!onlyActive)) && (allSite || x.Site.Name == siteName))).OrderBy(c => c.LastName).ToList();
                            totalCustomers = entities.Customers.Where(x => (x.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL") && ((x.CancelledDate == null || (!onlyActive)) && (allSite || x.Site.Name == siteName))).Count();
                        }
                        else
                        {
                            customers = entities.Customers.Where(c => (c.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL") && ((c.CancelledDate == null || (!onlyActive)) && (allSite || c.Site.Name == siteName) && (c.LastName.ToLower().StartsWith(letter.ToLower())))).OrderBy(c => c.LastName).ToList();
                            totalCustomers = entities.Customers.Where(c => (c.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL") && ((c.CancelledDate == null || (!onlyActive)) && (allSite || c.Site.Name == siteName) && (c.LastName.ToLower().StartsWith(letter.ToLower())))).Count();
                        }
                    }
                    else if (string.IsNullOrEmpty(letter))
                    {
                        customers = entities.Customers.Where(x => (x.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL") && ((x.CancelledDate == null || (!onlyActive)) && (allSite || x.Site.Name == siteName))).OrderBy(c => c.LastName).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
                        totalCustomers = entities.Customers.Where(x => (x.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL") && ((x.CancelledDate == null || (!onlyActive)) && (allSite || x.Site.Name == siteName))).Count();
                    }
                    else
                    {
                        customers = entities.Customers.Where(c => (c.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL") && ((c.CancelledDate == null || (!onlyActive)) && (allSite || c.Site.Name == siteName) && (c.LastName.ToLower().StartsWith(letter.ToLower())))).OrderBy(c => c.LastName).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
                        totalCustomers = entities.Customers.Where(c => (c.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL") && ((c.CancelledDate == null || (!onlyActive)) && (allSite || c.Site.Name == siteName) && (c.LastName.ToLower().StartsWith(letter.ToLower())))).Count();
                    }
                }


                List<CustomerDT> customersDT = new List<CustomerDT>();
                foreach (var c in customers)
                {
                    CustomerDT customerDT = new CustomerDT()
                    {
                        Address = c.Address,
                        AuthorizeProfileId = c.AuthorizeProfileId,
                        BadLoginCount = c.BadLoginCount,
                        City = c.City,
                        CreatedAt = c.CreatedAt,
                        NcId = c.NcId,
                        NextBillDate = c.NextBillDate,
                        Password = c.Password,
                        Phone = c.Phone,
                        Refunded = c.Refunded,
                        Site = c.Site.Name,
                        SiteId = c.SiteId,
                        StateId = c.StateId,
                        Id = c.Id,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        ZipCode = c.ZipCode,
                        Email = c.Email,
                        Status = c.Status,
                        CancelledDate = c.CancelledDate,
                        FailledBillingCount = (int)entities.Invoices.Where(i => i.CustomerId == c.Id).OrderByDescending(i => i.CreatedAt).FirstOrDefault().FailCount,
                        LastInvoice = (int)entities.Invoices.Where(i => i.CustomerId == c.Id).OrderByDescending(i => i.CreatedAt).FirstOrDefault().Id,
                        LastInvoiceDT = entities.Invoices.Where(i => i.CustomerId == c.Id).OrderByDescending(i => i.CreatedAt).FirstOrDefault().GetDT()
                    };

                    customersDT.Add(customerDT);
                }

                totalPage = totalCustomers;
                return customersDT;
            }
        }

        public static int CancelDebtorsCustomers()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {

                List<Customer> customers = new List<Customer>();
                int customersToDelete = GlobalParameters.DebtorsToCancel;
                customers = entities.Customers.Where(x => !x.CancelledDate.HasValue && !string.IsNullOrEmpty(x.Email)
                   && !string.IsNullOrEmpty(x.FirstName) && !string.IsNullOrEmpty(x.LastName) && (x.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault().StatusId == "BILLEDFAIL")).Take(customersToDelete).ToList();

                int count = customers.Count;
                foreach (var item in customers)
                {
                    item.CancelForFailedBilling = true;
                    entities.SaveChanges();
                    CustomersManager.CancelCustomer(item.Id);

                }
                return count;
            }
        }
    }


}
