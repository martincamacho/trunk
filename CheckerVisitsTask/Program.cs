using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckerVisitsTask
{
    class Program
    {
        static void Main(string[] args)
        { 
            using (CastleClub.BusinessLogic.Data.CastleClubEntities entities = new CastleClub.BusinessLogic.Data.CastleClubEntities())
            {
                //check if i need to send the limit in the arguments
                int limitVisists = 10;                
                DateTime twoHoursBefore = DateTime.Now.AddHours(-2);
                DateTime fourHoursBefore = DateTime.Now.AddHours(-4);
                DateTime today = DateTime.Now;
                int visitFrom2Hours = entities.Visits.Where(x => x.CreatedAt >= twoHoursBefore && x.CreatedAt < today).Count();
                int visitFrom4Hours = entities.Visits.Where(x => x.CreatedAt >= fourHoursBefore && x.CreatedAt < twoHoursBefore).Count();
                int newCustomers2Hours = entities.Customers.Where(x => x.CreatedAt >= twoHoursBefore && x.CreatedAt < today).Count();
                int newCustomers4Hours = entities.Customers.Where(x => x.CreatedAt >= fourHoursBefore && x.CreatedAt < today).Count();
                bool validToSend = false;
                string body = "<html><body><h3>The report of the last two hours at " + today.ToString() + " is: </h3>";
                string subject = "";


                if (visitFrom2Hours < limitVisists || newCustomers2Hours <= 0)
                {                   
                    body += "<h4>Total Visits : " + visitFrom2Hours.ToString() + "</h4>";
                    body += "<h4>New Sign-Ups : " + newCustomers2Hours.ToString() + "</h4>";    
                    subject = "Alert from unusual event at " + today.ToShortDateString() + " regarding the visits and the sign-ups of the site.";                    
                    validToSend = true;        
                }
                else if (visitFrom4Hours < limitVisists || newCustomers4Hours <= 0)
                {
                    body += "<h4>Total Visits : " + visitFrom2Hours.ToString() + "</h4>";
                    body += "<h4>New Sign-Ups : " + newCustomers2Hours.ToString() + "</h4>";
                    subject = "Fixed - The member signup site it's working as expected now.";
                    validToSend = true;
                }                                   
                if (validToSend)
                {
                    body += "</p></table></body></html>";
                    string[] separator = new string[] { ";" };
                    var emailsTo = System.Configuration.ConfigurationManager.AppSettings["EmailTo"].Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();
                    CastleClub.BusinessLogic.Utils.Email.SendEmail(System.Configuration.ConfigurationManager.AppSettings["EmailFrom"], System.Configuration.ConfigurationManager.AppSettings["EmailPassword"], System.Configuration.ConfigurationManager.AppSettings["smtp"], subject, body, emailsTo, true);
                }
            }
        }
    }
}
