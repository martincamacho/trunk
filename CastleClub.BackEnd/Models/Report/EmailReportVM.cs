using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class EmailReportVM : BaseVM
    {
        public List<ReferrerDT> Referrers { get; set; }

        [Required]
        public string ReportType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
        public string Time { get; set; }

        [Required]
        public int ReferrerId { get; set; }
    }
}