using CastleClub.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes
{
    public class ReferrerSalesInfoDT
    {
        public DateTime Date { get; set; }

        public string DateString { get; set; }
        public int Visits { get; set; }
        public int Signups { get; set; }

        public int Cancelled { get; set; }

        public int Billed { get; set; }

        public int Refunded { get; set; }

        public decimal Revenue { get; set; }

        public decimal Refund { get; set; }

        public decimal Profit { get; set; }
    }
}
