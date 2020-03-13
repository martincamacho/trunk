using CastleClub.BusinessLogic.Data;
using CastleClub.BusinessLogic.Utils;
using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Service
{
    public class GCDWebSvcImpl
    {
        private static string cryptKey = "bq!*p/Z]z7'Hq2K_aQTnb-3X";
        private static char[] keySep = { '=' };
        private static char[] valSep = { '&' };

        public static string CreateCustomer(string data, bool sendEmail)
        {
            try
            {
                var hash = EncryptionHelper.Unpack(EncryptionHelper.Decrypt(data, cryptKey));
                GCDSite site = new GCDSite(hash);
                GCDCustomer customer = new GCDCustomer(hash);
                customer.CreatedAt = DateTime.Now;
                customer.Success = false;

                if (!string.IsNullOrEmpty(site.GroupID) && !string.IsNullOrEmpty(customer.CreditCardNumber) && !string.IsNullOrEmpty(customer.Email))
                {
                    CustomerDT customerDT = new CustomerDT()
                    {
                        Address = customer.Address1,
                        BadLoginCount = 0,
                        CancelledDate = null,
                        City = customer.City,
                        ClearPassword = customer.Password,
                        CreatedAt = customer.CreatedAt,
                        Email = customer.Email,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        MemberId = customer.MemberID,
                        NcId = 0,
                        NextBillDate = DateTime.Now.AddDays(30),
                        Phone = customer.Phone,
                        StateId = CastleClub.BusinessLogic.Managers.LocationsManager.GetStates().FirstOrDefault(x => x.Name.ToLower() == customer.State.ToLower()).Id,
                        Status = CustomerStatus.ACTIVE,
                        ZipCode = customer.Zip
                    };
                    //var siteDT = CastleClub.BusinessLogic.Managers.SitesManager.GetSite(customerDT.SiteId);
                    customerDT = CastleClub.BusinessLogic.Managers.CustomersManager.NewCustomer(site.GroupID, customerDT, customer.CreditCardNumber, CreditCardHelper.GetExpDate(customer.CreditCardMonthExpired, customer.CreditCardYearExpired));
                    if (customerDT != null)
                    {
                        customer.Success = true;

                        //if (sendEmail)
                        /****
                        if ((siteDT.SendWelcomeEmail) && (siteDT.WelcomeEmailDelay != null) && (siteDT.WelcomeEmailDelay == 0))
                            //send welcome email if it hasn't a delay
                        {
                            string smtpAddress = siteDT.SmtpAddress;//CastleClub.BusinessLogic.Data.GlobalParameters.Smtp;
                            //var siteDT = CastleClub.BusinessLogic.Managers.SitesManager.GetSite(customerDT.SiteId);

                            string emailFrom = siteDT.Email;
                            string passwordEmail = siteDT.PasswordEmail;//CastleClub.BusinessLogic.Data.GlobalParameters.EmailPassword;
                            string subject = string.Format(Properties.Resources.WelcomeSubjectEmail, siteDT.Name);
                            // + " " + customer.LastName
                            string body = string.Format(Properties.Resources.WelcomeBodyEmail, customer.FirstName,
                                                        customer.Email, customer.Password, siteDT.Name, siteDT.Name, "PartsGeek",
                                                        siteDT.Price, siteDT.PricePerQuarter, siteDT.Phone, siteDT.Email, siteDT.OfferDomain);

                            Utils.Email.SendEmail(emailFrom, passwordEmail, smtpAddress, subject, body, new List<string>() { customer.Email }, true);
                        }
                         * **/
                    }
                }
                else
                {
                    customer.Messages = "Credit card and email are necessary.";
                }
                return EncryptionHelper.Encrypt(customer.ToString(), cryptKey);
            }catch(Exception e)
            {
                Utils.EventViewer.Writte("WEBSERVICE", "Create Customers", e.Message + " " + e.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                throw e;
            }
        }

        public static bool CancelCustomer(string customerData)
        {
            bool ret = false;
            int cancelCustomerId = 0;
            Hashtable cusHash = EncryptionHelper.Unpack(EncryptionHelper.Decrypt(customerData, cryptKey));
            if (cusHash.ContainsKey("CustomerID") && int.TryParse(cusHash["CustomerID"].ToString(), out cancelCustomerId))
            {
                using (CastleClubEntities entities = new CastleClubEntities())
                {
                    Customer customer = entities.Customers.FirstOrDefault(x => x.Id == cancelCustomerId);
                    if (customer != null)
                    {
                        customer.Status = CustomerStatus.CANCELLED;
                        customer.CancelledDate = DateTime.Now;

                        entities.SaveChanges();
                        ret = true;
                    }
                }
            }
            return ret;
        }

        public static string UpdatePassword(string passwordData)
        {
            int updateCustomerId = 0;
            Hashtable cusHash = EncryptionHelper.Unpack(EncryptionHelper.Decrypt(passwordData, cryptKey));
            GCDCustomer retCus = new GCDCustomer(cusHash);
            retCus.Success = false;
            if (cusHash.ContainsKey("CustomerID") && int.TryParse(cusHash["CustomerID"].ToString(), out updateCustomerId))
            {
                if (cusHash.ContainsKey("NewPassword")) 
                {
                    string newPassword = cusHash["NewPassword"].ToString();
                    using (CastleClubEntities entities = new CastleClubEntities())
                    {
                        Customer customer = entities.Customers.Where(c => c.Id == updateCustomerId).FirstOrDefault();
                        Password password = new Password(newPassword);
                        customer.Password = password.SaltedPassword;
                        customer.SaltKey = password.SaltKey;
                        customer.UnEncryptPass = "";// newPassword;
                        entities.SaveChanges();
                        retCus.Success = true;
                    }
                }
            }
            return EncryptionHelper.Encrypt(retCus.ToString(), cryptKey);
        }

        public static string ResetPasswordByGroup(string resetData, string groupId, bool sendMail)
        {
            int siteId = 0;
            Hashtable cusHash = EncryptionHelper.Unpack(EncryptionHelper.Decrypt(resetData, cryptKey));
            GCDCustomer retCus = new GCDCustomer(cusHash);
            retCus.Success = false;

            if (cusHash.ContainsKey("Email") && int.TryParse(groupId, out siteId))
            {
                using (CastleClubEntities entities = new CastleClubEntities())
                {
                    Site site = entities.Sites.FirstOrDefault(x => x.GroupId == siteId.ToString());
                    if (site != null)
                    {
                        siteId = site.Id;
                        string email = cusHash["Email"].ToString();
                        Customer customer = entities.Customers.FirstOrDefault(c => c.SiteId == siteId && c.Email == email);
                        if (customer != null)
                        {
                            /*Password password = new Password();
                            customer.Password = password.SaltedPassword;
                            customer.SaltKey = password.SaltKey;
                            entities.SaveChanges();
                            retCus.Success = true;*/

                            if (sendMail && !string.IsNullOrEmpty(retCus.Password))
                            {
                                string smtpAddress = CastleClub.BusinessLogic.Data.GlobalParameters.Smtp;
                                string emailFrom = site.Email;
                                string passwordEmail = site.PasswordEmail; //CastleClub.BusinessLogic.Data.GlobalParameters.EmailPassword;
                                string subject = string.Format(Properties.Resources.SetPasswordSubjectEmail,site.Name);
                                var url = string.Format(GlobalParameters.UrlResetPassword, retCus.Password, retCus.Validator);
                                //pasamos el dominio en password y el guid en validator
                                string body = string.Format(Properties.Resources.SetPasswordBodyEmail, url,site.Name);

                                Utils.Email.SendEmail(emailFrom, passwordEmail, smtpAddress, subject, body, new List<string>() { customer.Email }, true);
                                retCus.Success = true;

                            }
                            else if (!sendMail && !string.IsNullOrEmpty(retCus.Password))
                            {
                                Password password = new Password(retCus.Password);
                                customer.Password = password.SaltedPassword;
                                customer.SaltKey = password.SaltKey;
                                entities.SaveChanges();
                                retCus.Success = true;
                            }

                        }
                        else
                        {
                            retCus.Messages = "Invalid Email";
                        }
                    }
                    else
                    {
                        retCus.Messages = "Invalid GroupId";
                    }
                }
            }
            else
            {
                retCus.Messages = "Invalid Email";
            }
            CastleClub.BusinessLogic.Utils.EventViewer.WriteToFile("Log Web Service",retCus.Messages,"c:\\users\\administrator\\desktop\\log.txt");
            return EncryptionHelper.Encrypt(retCus.ToString(), cryptKey);  
        }

        public static string UpdateCustomer(string customerData, bool sendEmail)
        {
            int updateCustomerId = 0;
            Hashtable cusHash = EncryptionHelper.Unpack(EncryptionHelper.Decrypt(customerData, cryptKey));
            GCDCustomer retCus = new GCDCustomer(cusHash);
            GCDCreditCard retCreditCard = new GCDCreditCard(cusHash);
            retCus.Success = false;
            if (cusHash.ContainsKey("CustomerID") && int.TryParse(cusHash["CustomerID"].ToString(), out updateCustomerId))
            {
                using (CastleClubEntities entities = new CastleClubEntities())
                {
                    Customer customer = entities.Customers.Where(c => c.Id == updateCustomerId).FirstOrDefault();
                    if (customer != null)
                    {
                        Password passwordHash = new Password(cusHash["PasswordOld"].ToString(), customer.SaltKey);
                        if (customer.Password == passwordHash.SaltedPassword)
                        {
                            customer.FirstName = retCus.FirstName;
                            customer.LastName = retCus.LastName;
                            customer.Address = retCus.Address1;
                            customer.City = retCus.City;
                            State state = entities.States.FirstOrDefault(s => s.Name.ToLower() == retCus.State.ToLower());
                            customer.StateId = state != null ? state.Id.ToString() : string.Empty;
                            customer.Email = retCus.Email;
                            customer.Phone = retCus.Phone;
                            customer.ZipCode = retCus.Zip;

                            var creditCard = customer.CreditCards.FirstOrDefault();
                            if (creditCard != null && !string.IsNullOrEmpty(retCreditCard.CreditCard) && Utils.CreditCardHelper.Valid(retCreditCard.CreditCard, false))
                            {
                                var today = DateTime.Now;
                                if (today.Year > retCus.CreditCardYearExpired || (today.Year == retCus.CreditCardYearExpired && today.Month >= retCreditCard.Month))
                                {
                                    throw new Exception("The credit card is invalid.");
                                }

                                var encryptData = CastleClub.BusinessLogic.Utils.EncryptionHelper.EncryptRSACertificate("ccNum:" + retCreditCard.CreditCard + ";cVV:;ccExp:" + Utils.CreditCardHelper.GetExpDate(retCreditCard.Month, retCreditCard.Year));
                                string lastFourDigit = retCreditCard.CreditCard.Substring(retCreditCard.CreditCard.Length - 4, 4);
                                string type = Utils.CreditCardHelper.GetCardType(retCreditCard.CreditCard);

                                creditCard.CreatedAt = DateTime.Now;
                                creditCard.Data = encryptData;
                                creditCard.LastFourDigit = lastFourDigit;
                                creditCard.Type = type;

                                long newPaymentProfile;
                                string message = string.Empty;
                                if (CastleClub.BusinessLogic.AuthorizeAPI.CIM.CreateNewCustomerPaymentProfile(customer.Site.GetDT(), customer.GetDT(false), retCreditCard.CreditCard, Utils.CreditCardHelper.GetExpDate(retCreditCard.Month, retCreditCard.Year), out newPaymentProfile, out message))
                                {
                                    creditCard.AuthorizePaymentProfileId = newPaymentProfile;
                                }
                                else
                                {
                                    throw new Exception(message);
                                }
                            }

                            bool setPassword = false;
                            if (!string.IsNullOrEmpty(retCus.PasswordNow))
                            {
                                Password password = new Password(retCus.PasswordNow);
                                customer.Password = password.SaltedPassword;
                                customer.SaltKey = password.SaltKey;
                                setPassword = true;
                            }

                            retCus.Success = true;

                            entities.SaveChanges();

                            if (sendEmail && setPassword)
                            {
                                string smtpAddress = CastleClub.BusinessLogic.Data.GlobalParameters.Smtp;
                                string emailFrom = customer.Site.Email;
                                string passwordEmail = customer.Site.PasswordEmail; //CastleClub.BusinessLogic.Data.GlobalParameters.EmailPassword;
                                string subject = string.Format(Properties.Resources.SetPasswordSubjectEmail, customer.Site.Name);
                                string body = string.Format(Properties.Resources.SetPasswordBodyEmail, customer.Site.Name, retCus.PasswordNow);

                                Utils.Email.SendEmail(emailFrom, passwordEmail, smtpAddress, subject, body, new List<string>() { customer.Email }, false);
                            }
                        }
                        else
                        {
                            retCus.Success = false;
                            retCus.Messages = "The password for the customer not match.";
                        }
                    }
                    else
                    {
                        retCus.Messages = "Error custmer ID.";
                    }
                }
            }
            return EncryptionHelper.Encrypt(retCus.ToString(), cryptKey);
        }

        public static string LogonSecureByGroup(string logonData, string groupId)
        {
            GCDCustomer retCus = new GCDCustomer();
            retCus.Success = false;
            retCus.AccountLocked = false;

            if (string.IsNullOrEmpty(groupId))
            {
                groupId = "1181";
            }

            using (CastleClubEntities entities = new CastleClubEntities())
            {
                Site site = entities.Sites.Where(s => s.GroupId == groupId).FirstOrDefault();
                if (site != null)
                {
                    string li = EncryptionHelper.Decrypt(logonData, cryptKey);
                    string[] linf = li.Split('&');

                    if (linf.Length == 2)
                    {
                        string[] cinf = linf[0].Split('=');
                        string[] pinf = linf[1].Split('=');
                        string email =cinf[1];

                        if (cinf.Length == 2 && pinf.Length == 2)
                        {
                            Customer customer = entities.Customers.Where(c => c.SiteId == site.Id && c.Email == email).FirstOrDefault();
                            if (customer != null)
                            {
                                string chkPass = pinf[1];
                                Password pwd = new Password(chkPass, customer.SaltKey);
                                if (customer.BadLoginCount < 5 && customer.Password == pwd.SaltedPassword)
                                {
                                    if (customer.Status != CustomerStatus.ACTIVE)
                                    {
                                        retCus.Messages = "Customer Account Not Found, Invalid or Cancelled";
                                    }
                                    else
                                    {
                                        retCus.FirstName = customer.FirstName;
                                        retCus.LastName = customer.LastName;
                                        retCus.Address1 = customer.Address;
                                        retCus.Address2 = String.Empty;
                                        retCus.City = customer.City;
                                        retCus.State = entities.States.Where(s => s.Id == customer.StateId).FirstOrDefault().Name;
                                        retCus.Country = "USA";
                                        retCus.Email = customer.Email;
                                        retCus.Phone = customer.Phone;
                                        retCus.CustomerID = customer.Id;
                                        retCus.RegisterDate = customer.CreatedAt;
                                        retCus.MemberID = "M" + site.GroupId + "-" + customer.Id;
                                        retCus.Validator = logonData;
                                        retCus.Success = true;
                                        retCus.Zip = customer.ZipCode;

                                        retCus.CreatedAt = customer.CreatedAt;
                                        var creditCard = customer.CreditCards.FirstOrDefault();
                                        if (creditCard!=null)
                                        {
                                            retCus.CreditCardNumber = creditCard.LastFourDigit;
                                            retCus.CreditCardType = creditCard.Type;
                                            retCus.CreditCardMonthExpired = 1;
                                            retCus.CreditCardYearExpired = DateTime.Now.Year;
                                        }

                                        customer.BadLoginCount = 0;
                                        entities.SaveChanges();
                                    }
                                }
                                else
                                {
                                    retCus.Messages = "Logon Failed";
                                    if (customer.BadLoginCount > 4)
                                    {
                                        retCus.AccountLocked = true;
                                    }
                                    else
                                    {
                                        customer.BadLoginCount = customer.BadLoginCount + 1;
                                        entities.SaveChanges();

                                        retCus.AccountLocked = false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        retCus.Messages = "Missing or Null Data Field";
                    }

                    if (retCus.Success == false && retCus.Messages.Length == 0)
                    {
                        retCus.Messages = "Logon Failed";
                    }
                    return EncryptionHelper.Encrypt(retCus.ToString(), cryptKey);
                }
                else
                {
                    return String.Empty;
                }
            }                  
        }
    }

    public class GCDCustomer
    {
        public int CustomerID = 0;
        public long SubscriptionID = 0;
        public string MemberID = String.Empty;
        public string FirstName = String.Empty;
        public string LastName = String.Empty;
        public string Address1 = String.Empty;
        public string Address2 = String.Empty;
        public string City = String.Empty;
        public string State = String.Empty;
        public string Zip = String.Empty;
        public string Country = String.Empty;
        public string Email = String.Empty;
        public string Phone = String.Empty;
        public DateTime RegisterDate = DateTime.MinValue;
        public DateTime LastPayment = DateTime.MinValue;
        public DateTime NextPayment = DateTime.MinValue;
        public bool Success = false;
        public bool AccountLocked = false;
        public string Messages = String.Empty;
        public string Validator = String.Empty;
        public string Password = String.Empty;
        public string PasswordNow = string.Empty;
        public string PasswordOld = string.Empty;
        public string CreditCardNumber = string.Empty;
        public string CreditCardType = string.Empty;
        public int CreditCardMonthExpired = int.MinValue;
        public int CreditCardYearExpired = int.MinValue;
        public DateTime CreatedAt = DateTime.MinValue;
        public string CreditCard = string.Empty;

        public Hashtable Hash = new Hashtable();

        public GCDCustomer()
        {
        }

        public GCDCustomer(Hashtable hash)
        {
            if (hash.ContainsKey("CustomerID")) CustomerID = Convert.ToInt32(hash["CustomerID"]);
            if (hash.ContainsKey("SubscriptionID")) SubscriptionID = (long)Convert.ToDouble(hash["SubscriptionID"]);
            if (hash.ContainsKey("MemberID")) MemberID = hash["MemberID"].ToString();
            if (hash.ContainsKey("FirstName")) FirstName = hash["FirstName"].ToString();
            if (hash.ContainsKey("LastName")) LastName = hash["LastName"].ToString();
            if (hash.ContainsKey("Address1")) Address1 = hash["Address1"].ToString();
            if (hash.ContainsKey("Address2")) Address2 = hash["Address2"].ToString();
            if (hash.ContainsKey("City")) City = hash["City"].ToString();
            if (hash.ContainsKey("State")) State = hash["State"].ToString();
            if (hash.ContainsKey("Zip")) Zip = hash["Zip"].ToString();
            if (hash.ContainsKey("Country")) Country = hash["Country"].ToString();
            if (hash.ContainsKey("Email")) Email = hash["Email"].ToString();
            if (hash.ContainsKey("Phone")) Phone = hash["Phone"].ToString();
            if (hash.ContainsKey("RegisterDate")) RegisterDate = Convert.ToDateTime(hash["RegisterDate"].ToString());
            if (hash.ContainsKey("Success")) Success = Convert.ToBoolean(hash["Success"]);
            if (hash.ContainsKey("Messages")) Messages = hash["Messages"].ToString();
            if (hash.ContainsKey("CreditCardNumber")) CreditCardNumber = hash["CreditCardNumber"].ToString();
            if (hash.ContainsKey("CreditCardType")) CreditCardType = hash["CreditCardType"].ToString();
            if (hash.ContainsKey("CreditCardMonthExpired")) CreditCardMonthExpired = int.Parse(hash["CreditCardMonthExpired"].ToString());
            if (hash.ContainsKey("CreditCardYearExpired")) CreditCardYearExpired = int.Parse(hash["CreditCardYearExpired"].ToString());
            if (hash.ContainsKey("CreatedAt")) CreatedAt = DateTime.Parse(hash["CreatedAt"].ToString());
            if (hash.ContainsKey("CreditCard")) CreditCard = hash["CreditCard"].ToString();
            if (hash.ContainsKey("Password")) Password = hash["Password"].ToString();
            if (hash.ContainsKey("PasswordNow")) PasswordNow = hash["PasswordNow"].ToString();
            if (hash.ContainsKey("PasswordOld")) PasswordOld = hash["PasswordOld"].ToString();
            if (hash.ContainsKey("Validator")) Validator = hash["Validator"].ToString();
        }

        public override String ToString()
        {
            Hash.Clear();
            Hash.Add("CustomerID", CustomerID.ToString());
            Hash.Add("SubscriptionID", SubscriptionID.ToString());
            Hash.Add("MemberID", MemberID);
            Hash.Add("FirstName", FirstName);
            Hash.Add("LastName", LastName);
            Hash.Add("Address1", Address1);
            Hash.Add("Address2", Address2);
            Hash.Add("City", City);
            Hash.Add("State", State);
            Hash.Add("Zip", Zip);
            Hash.Add("Country", Country);
            Hash.Add("Email", Email);
            Hash.Add("Phone", Phone);
            Hash.Add("RegisterDate", RegisterDate.ToShortDateString());
            Hash.Add("Success", Success.ToString());
            Hash.Add("Messages", Messages);
            Hash.Add("AccountLocked", Convert.ToInt32(AccountLocked));
            Hash.Add("CreditCardNumber", CreditCardNumber);
            Hash.Add("CreditCardType", CreditCardType);
            Hash.Add("CreditCardMonthExpired", CreditCardMonthExpired.ToString());
            Hash.Add("CreditCardYearExpired", CreditCardYearExpired.ToString());
            Hash.Add("CreatedAt", CreatedAt.ToShortDateString());
            if (Password != String.Empty) { Hash.Add("Password", Password); }
            return EncryptionHelper.Pack(Hash);
        }

    }

    public class GCDCreditCard
    {
        public string CreditCard { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string CreditCardType { get; set; }
        public Hashtable Hash = new Hashtable();

        public GCDCreditCard()
        { }

        public GCDCreditCard(Hashtable hash)
        {
            if (hash.ContainsKey("CreditCardNumber")) CreditCard = hash["CreditCardNumber"].ToString();
            if (hash.ContainsKey("CreditCardMonthExpired")) Month = int.Parse(hash["CreditCardMonthExpired"].ToString());
            if (hash.ContainsKey("CreditCardYearExpired")) Year = int.Parse(hash["CreditCardYearExpired"].ToString());
            if (hash.ContainsKey("CreditCardType")) CreditCardType = hash["CreditCardType"].ToString();
        }
    }

    public class GCDSite
    {
        public int Id { get; set; }
        public string GroupID { get; set; }

        public GCDSite()
        { }

        public GCDSite(Hashtable hash)
        {
            if (hash.ContainsKey("Id")) Id = int.Parse(hash["id"].ToString());
            if (hash.ContainsKey("GroupID")) GroupID = hash["GroupID"].ToString();
        }
    }
}
