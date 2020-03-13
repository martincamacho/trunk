using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Data
{
    partial class Customer
    {
        public CustomerDT GetDT(bool loadNotes)
        {
            CustomerDT res = new CustomerDT();
            res.Id = Id;
            res.MemberId =  Site!=null && !string.IsNullOrEmpty(Site.GroupId) && Id>0 ? String.Format("M{0}-{1}",Site.GroupId,Id) :string.Empty;
            res.SiteId = SiteId;
            res.Site = Site!=null ? Site.Name : string.Empty;
            res.Referrer = Referrer!=null ? Referrer.Name : string.Empty;
            res.NcId = NcId;
            res.Email = Email;
            res.Password = Password;
            res.SaltKey = SaltKey;
            res.FirstName = FirstName;
            res.LastName = LastName;
            res.Phone = Phone;
            res.Address = Address;
            res.City = City;
            res.StateId = StateId;
            res.ZipCode = ZipCode;
            res.AuthorizeProfileId = AuthorizeProfileId;
            res.CreatedAt = CreatedAt;
            res.Status = Status;
            res.Refunded = Refunded;
            res.LastBillDate = LastBillDate.HasValue ? LastBillDate.Value : DateTime.MinValue;
            res.NextBillDate = NextBillDate;
            res.CancelledDate = CancelledDate.HasValue ? CancelledDate.Value : DateTime.MinValue;
            res.BadLoginCount = BadLoginCount;
            res.EmailForm = EmailForm;
            res.WelcomeEmail = WelcomeEmail;
            res.welcomeEmailDelay = WelcomeEmailDelay;
            res.WelcomeEmailSent = WelcomeEmailSent;
            res.UnEncryptPass = UnEncryptPass;
            res.CancelForFailedBilling = CancelForFailedBilling;
            if (loadNotes)
            {
                res.Notes = Notes.ToList().Select(n => n.GetDT()).ToList();
            }
            return res;
        }

        public CustomerStatus Status 
        {
            get { return (CustomerStatus)Enum.Parse(typeof(CustomerStatus), StatusId); }
            set { StatusId = value.ToString(); }
        }
    
    }
}
