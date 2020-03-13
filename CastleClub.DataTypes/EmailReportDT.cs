using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes
{
    public class EmailReportDT
    {
        public string CustomerID {get; set;}
        public string FirstName	{get;set;}
        public string LastName {get;set;}
        public string Email {get;set;}
        public string Zip {get;set;}
        public string CustomerPass {get; set;}
        public string DateSent { get; set; }
        public string CreditCardType { get; set; }
        public bool EmailForm { get; set; }
    }
}
