using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Utils
{
    public class Email
    {
        public static bool SendEmail(string emailFrom, string password, string smtpAddress, string subject, string body, List<string> emailsTo, bool isHtml)
        {
            try
            {
                int portNumber = 587;
                bool enableSSL = true;

                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    foreach (var emailTo in emailsTo)
                    {
                        mail.To.Add(emailTo);
                    }
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = isHtml;

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool SendEmailWithCC(string emailFrom, string password, string smtpAddress, string subject, string body, List<string> emailsTo, List<string> CC, bool isHtml)
        {
            try
            {
                int portNumber = 587;
                bool enableSSL = true;

                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    foreach (var emailTo in emailsTo)
                    {
                        mail.To.Add(emailTo);
                    }
                    foreach (var emailCC in CC)
                    {
                        mail.CC.Add(emailCC);
                    }
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = isHtml;

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static bool SendEmailWithBCC(string emailFrom, string password, string smtpAddress, string subject, string body, List<string> emailsTo, List<string> BCC, bool isHtml)
        {
            try
            {
                int portNumber = 587;
                bool enableSSL = true;

                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);
                    foreach (var emailTo in emailsTo)
                    {
                        mail.To.Add(emailTo);
                    }
                    foreach (var emailCC in BCC)
                    {
                        mail.Bcc.Add(emailCC);
                    }
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = isHtml;

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static string WelcomeBodyEmail()
        {
            return CastleClub.BusinessLogic.Properties.Resources.WelcomeBodyEmail;
        }

        public static string WelcomeSubjectEmail()
        {
            return CastleClub.BusinessLogic.Properties.Resources.WelcomeSubjectEmail;
        }
    }
}
