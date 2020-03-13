using CastleClub.BusinessLogic.Managers;
using CastleClub.BusinessLogic.Utils;
using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CastleClub.WelcomeEmailWithDelayTask
{
    class Program
    {
        static void Main(string[] args)
        {

            using (CastleClub.BusinessLogic.Data.CastleClubEntities entities = new BusinessLogic.Data.CastleClubEntities())
            {
                

                var sites = entities.Sites.ToList();
                int capEmail = int.Parse(System.Configuration.ConfigurationManager.AppSettings["CapEmail"]); 
                foreach (var site in sites)
                {
                    if ((site.SendWelcomeEmail == true) && (site.WelcomeEmailDelay != null) && (site.WelcomeEmailDelay > 0))
                    {
                        int delay = site.WelcomeEmailDelay==null ? 0: (int)site.WelcomeEmailDelay ;
                        DateTime mindate = DateTime.Now.Date.AddDays(-1 * (delay));
                        DateTime maxdate = DateTime.Now.Date.AddDays(-1 * (delay-1));
                        var customerlist = entities.Customers.Where(x => x.CreatedAt >= mindate && x.CreatedAt < maxdate && x.WelcomeEmail != true && x.SiteId == site.Id && x.StatusId=="ACTIVE").ToList();
                        foreach (var customer in customerlist)
                        {
                            if (capEmail == 0 || SitesManager.GetEmailSentToday()<capEmail) 
                            {
                                CustomerDT cust = customer.GetDT(false);
                                SendEmail(site.Id, cust, cust.UnEncryptPass);
                                //customer.Password = cust.Password;
                                customer.WelcomeEmail = true;
                                customer.WelcomeEmailDelay = delay;
                                customer.WelcomeEmailSent = DateTime.Now;
                                customer.UnEncryptPass = "";
                                entities.SaveChanges();
                            }
                        }
                        
                    }
                }
                

            }

        }


        private static void SendEmail(int SiteID, CustomerDT Customer, string ClearPassword)
        {


            SiteDT siteDT = SitesManager.GetSite(SiteID);
            string body = string.Format(Email.WelcomeBodyEmail(), Customer.FirstName,
                                                Customer.Email, ClearPassword, siteDT.Name, siteDT.Name, "PartsGeek",
                                                siteDT.Price, siteDT.PricePerQuarter, siteDT.Phone, siteDT.Email, siteDT.OfferDomain);

            string subject = string.Format(Email.WelcomeSubjectEmail(), siteDT.Name);
            
            /**/
            CastleClub.BusinessLogic.Utils.Email.SendEmailWithBCC(siteDT.Email, siteDT.PasswordEmail, siteDT.SmtpAddress, subject, body
                , new List<string>() { Customer.Email }, new List<string>() { siteDT.WelcomeEmailBCC }, true);
            /**/
        }
    }
}
