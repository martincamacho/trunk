using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class MagnementActivesSitesVM
    {
        public List<MagnementActiveSiteVM> Sites { get; set; }
        [Required]
        public int SiteID { get; set; }

        [Display(Description="Test")]
        public bool MasterCardEmailForm { get; set; }
        public int MasterEmailFormProcentage { get; set; }
        public bool VisaCardEmailForm { get; set; }
        public int VisaEmailFormPorcentage { get; set; }
        public bool DiscoverCardEmailForm { get; set; }
        public int DiscoverEmailFormPorcentage { get; set; }
    }
    public class MagnementActiveSiteVM
    {
        public bool Active { get; set; }
        public int SiteId { get; set; }
        public SiteDT Site { get; set; }
        public string Url { get; set; }
    }
}