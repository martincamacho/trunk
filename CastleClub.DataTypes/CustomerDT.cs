using CastleClub.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes
{
    public class CustomerDT
    {
        public int Id { get; set; }

        public string MemberId { get; set; }

        public int SiteId { get; set; }

        public string Site { get; set; }

        public string Referrer { get; set; }

        public int NcId { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int SaltKey { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string StateId { get; set; }

        public string ZipCode { get; set; }

        public long AuthorizeProfileId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ClearPassword { get; set; }

        public CustomerStatus Status { get; set; }

        public bool Refunded { get; set; }

        public DateTime LastBillDate { get; set; }

        public DateTime NextBillDate { get; set; }

        public DateTime? CancelledDate { get; set; }

        public int BadLoginCount { get; set; }

        public List<NoteDT> Notes { get; set; }
        public bool EmailForm { get; set; }

        public Nullable<bool> WelcomeEmail { get; set; }

        public Nullable<DateTime> WelcomeEmailSent { get; set; }

        public Nullable<int> welcomeEmailDelay { get; set; }

        public string UnEncryptPass { get; set; }

        public int FailledBillingCount { get; set; }

        public int LastInvoice { get; set; }

        public InvoiceDT LastInvoiceDT { get; set; }

        public bool CancelForFailedBilling { get; set; }
    }
}
