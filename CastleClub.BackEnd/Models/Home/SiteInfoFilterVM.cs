using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class SiteInfoFilterVM
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public List<SitesInfoDT> SitesInfo { get; set; }

        public int CurrentActiveCustomers { get; set; }

        public int Refound { get; set; }
    }
}