using CastleClub.BusinessLogic.Managers;
using CastleClub.BusinessLogic.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.EmailRegistrationTask
{
    class Program
    {
        static void Main(string[] args)
        {
            /**
            Hashtable ht = EncryptionHelper.Unpack(EncryptionHelper.Decrypt("FD YKCE8aO0CnJggjXGWqy10+CWVbunn", "hC0aZvZQZfotDkwvRZGy3LRb"));
            var referrerDT = ReferrersManagers.GetReferrer("LF113B3N").GetDT();
            string customerId = ht["CustomerID"].ToString();
            string data = CustomersManager.RequestToPartsGeek(referrerDT, customerId);
           

            Char[] cons = { 'b', 'c','d', 'f', 'g', 'h',  'j', 'k', 'l', 'm', 'n', 'p', 'q','r','s','t','v','x','w','z' };
            Char[] vow = { 'a', 'e', 'i', 'o', 'u' };
            System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\CVCV.txt");
            
            
            foreach (var c1 in cons)
            {
                foreach (var v1 in vow)
                {
                    foreach (var c2 in cons)
                    {
                        foreach (var v2 in vow)
                        {
                            sw.WriteLine(c1.ToString() + v1.ToString() + c2.ToString() + v2.ToString());
                        }
                    }
                }
            }
            sw.Close();
             * **/
            SHA1 sha1 = SHA1Managed.Create();

             ASCIIEncoding encoding = new ASCIIEncoding();
             byte[] stream = null;
             StringBuilder sb = new StringBuilder();
             stream = sha1.ComputeHash(encoding.GetBytes("WEJ5G-101036-2251193"));
             for (int i = 0; i < stream.Length; i++)
                 sb.AppendFormat("{0:x2}", stream[i]);
             string cvt = sb.ToString();


             System.Diagnostics.Process.Start("https://foodvalueshop.enjoymydeals.com/home?CVT=" + cvt);
            int a = 1;

             




            using (CastleClub.BusinessLogic.Data.CastleClubEntities entities = new BusinessLogic.Data.CastleClubEntities())
            {
                DateTime yesterday = DateTime.Now.Date.AddDays(-1);
                DateTime today = DateTime.Now.Date;
                var customerlist = entities.Customers.Where(x => x.CreatedAt >= yesterday && x.CreatedAt < today).ToList();

                string body = "<html><body><p>These are the new customers for Golden Club Sites at " + yesterday.ToShortDateString() + "</p>";

                body += "<table><tr><th>Name </th><th> Email </th><th>Offer </th><th>Time</th></tr> ";
                foreach (var customer in customerlist)
                {
                    body += "<tr><td>" + customer.FirstName + " " + customer.LastName + "</td><td>" + customer.Email + "</td><td>" + customer.Site.Name + "</td><td>" + customer.CreatedAt + "</td></tr>";
                }

                body += "</table></body></html>";
                string subject = "GCDSERVER - New Customers for " + yesterday.ToShortDateString();
                string[] separator = new string[] { ";" };
                var emailsTo = System.Configuration.ConfigurationManager.AppSettings["emailTo"].Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();

                CastleClub.BusinessLogic.Utils.Email.SendEmail(System.Configuration.ConfigurationManager.AppSettings["emailFrom"], System.Configuration.ConfigurationManager.AppSettings["emailPassword"], System.Configuration.ConfigurationManager.AppSettings["smtp"], subject, body, emailsTo, true);

            }



        }
    }
}
