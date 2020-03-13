using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class SalesReportVM : BaseVM
    {
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }

        public string Range { get; set; }

        public List<ReferrerDT> Referrers { get; set; }

        public int ReferrerId { get; set; }

        public List<SiteDT> Sites { get; set; }

        public int SiteId { get; set; }
        
        [Required]
        public string GroupBy { get; set; }
    }
}