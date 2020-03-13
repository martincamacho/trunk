using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Data
{
    partial class Site
    {
        public SiteDT GetDT()
        {
            SiteDT res = new SiteDT();
            res.Id = Id;
            res.Name = Name;
            res.AltNames = AltNames;
            res.Subtitle = Subtitle;
            res.SignupDomain = SignupDomain;
            res.OfferDomain = OfferDomain;
            res.PricePerQuarter = PricePerQuarter;
            res.GroupId = GroupId;
            res.AuthorizeLoginId = AuthorizeLoginId;
            res.AuthorizeTransactionKey = AuthorizeTransactionKey;
            res.Active = Active;
            res.Phone = Phone;
            res.OfficeHours = OfficeHours;
            res.CSSOuterColor = CSSOuterColor;
            res.CSSBlueColor = CSSBlueColor;
            res.CSSEmphasizeColor = CSSEmphasizeColor;
            res.ParticipatingPropertiesLogos = ParticipatingPropertiesLogos;
            res.BusinessSector = BusinessSector;
            res.LastUpdate = LastUpdate;
            res.Benefits = Benefits;
            res.WeHaveAmassed = WeHaveAmassed;
            res.Email = Email;
            res.MailAddress = MailAddress;
            res.ShortName = ShortName;
            res.Price = Price;
            res.MasterCardEmailFormPercentage = (int)MasterCardEmailFormPercentage;
            res.VisaEmailFormPercentage = (int)VisaEmailFormPercentage;
            res.DiscoverEmailFormPercentage = (int)DiscoverEmailFormPercentage;
            res.MasterCardEmailFormCount= MasterCardEmailFormCount;
            res.VisaEmailFormCount= VisaEmailFormCount;
            res.DiscoverEmailFormCount=DiscoverEmailFormCount;
            res.MasterCardEmailFormTotal= MasterCardEmailFormTotal;
            res.VisaEmailFormTotal= VisaEmailFormTotal;
            res.DiscoverEmailFormTotal = DiscoverEmailFormTotal;
            res.SmtpAddress = SmtpAddress;
            res.SendWelcomeEmail = (bool) SendWelcomeEmail;
            res.PasswordEmail = PasswordEmail;
            res.WelcomeEmailDelay = WelcomeEmailDelay;
            res.SignUpFormat = SignUpFormat;
            res.WelcomeEmailBCC = WelcomeEmailBCC;
            res.ShortAddress = ShortAddress;
            return res;
        }
    }
}
