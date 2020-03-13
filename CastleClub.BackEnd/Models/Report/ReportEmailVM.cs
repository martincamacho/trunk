using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using CastleClub.BackEnd.Models;
using CastleClub.BusinessLogic.Managers;
using CastleClub.BusinessLogic.Utils;
using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;

namespace CastleClub.BackEnd.Models
{
    public class ReportEmailVM : BaseVM
    {
        public List<SiteDT> Sites { get; set; }
      
        public int SiteId { get; set; }
        public int ReferrerId { get; set; }
        public List<ReferrerDT> Referrers { get; set; }
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int[] delayDef { get; set; }

        public int[,] delayEmailSent { get; set; }

        public int[,] delayActiveUsers { get; set; }

        public int[,] delayCancelledUsers { get; set; }
        
    }
}