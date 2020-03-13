using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes
{
    public class AccessCustomerDT
    {
        public string recordIdentifier { get; set; }
        public string recordType { get; set; }
        public string organizationcustomeridentifier { get { return "WEj5g"; } }
        public string programcustomeridentifier { get; set; }
        public string membercustomeridentifier { get; set; }
        public string previousMemberCustomerIdentifier { get; set; }
        public string memberStatus { get { return "OPEN"; } }
        public string fullName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string streetLine1 { get; set; }
        public string streetLine2{ get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
        public string phoneNumber { get; set; }
        public string emailAddress{ get; set; }
        public string membershipRenewalDate { get {
            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, 15).AddMonths(1);
            return date.ToString("yyyyMMdd");
        } }
        public string productIdentifier { get; set; }
        public string productTemplateField1 { get; set; }
        public string productTemplateField2 { get; set; }
        public string productTemplateField3 { get; set; }
        public string productTemplateField4 { get; set; }
        public string productTemplateField5 { get; set; }
    }
}
