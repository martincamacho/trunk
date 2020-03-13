using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes
{
    public class SiteDT
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AltNames { get; set; }

        public string Subtitle { get; set; }

        public string SignupDomain { get; set; }

        public string OfferDomain { get; set; }

        public decimal PricePerQuarter { get; set; }

        public string GroupId { get; set; }

        public string AuthorizeLoginId { get; set; }

        public string AuthorizeTransactionKey { get; set; }

        public bool Active { get; set; }

        public string Phone { get; set; }
        
        public string OfficeHours { get; set; }

        public string CSSOuterColor { get; set; }
        
        public string CSSBlueColor { get; set; }
        
        public string CSSEmphasizeColor { get; set; }
        
        public string ParticipatingPropertiesLogos { get; set; }

        public string BusinessSector { get; set; }

        public string LastUpdate { get; set; }

        public string Benefits { get; set; }

        public string WeHaveAmassed { get; set; }

        public string Email { get; set; }
        public string MailAddress { get; set; }
        public string ShortName { get; set; }
        public decimal Price { get; set; }
        public int MasterCardEmailFormPercentage { get; set; }
        public int VisaEmailFormPercentage { get; set; }
        public int DiscoverEmailFormPercentage { get; set; }
        public int MasterCardEmailFormCount { get; set; }
        public int VisaEmailFormCount { get; set; }
        public int DiscoverEmailFormCount { get; set; }
        public int MasterCardEmailFormTotal { get; set; }
        public int VisaEmailFormTotal { get; set; }
        public int DiscoverEmailFormTotal { get; set; }
        public bool SendWelcomeEmail { get; set; }
        public string PasswordEmail { get; set; }
        public string SmtpAddress { get; set; }
        public string SignUpFormat { get; set; }
        public string WelcomeEmailBCC { get; set; }
        public string ShortAddress { get; set; }
        public Nullable<int> WelcomeEmailDelay { get; set; }
    }
}
