using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes
{
    public class SitesInfoDT
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Signup { get; set; }
        public int Visits { get; set; }
        public double Percentage { get; set; }
        public int ActiveCustomers { get; set; }
        public double PercentageSignupToday { get; set; }
        public double Refund { get; set; }
        public int TotalCustomers { get; set; }
    }
}
