using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class ReportSiteCreditCardsVM
    {
        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }
        [Required]
        public int SiteID { get; set; }
        public List<SiteDT> Sites
        {
            get
            {
                var list = CastleClub.BusinessLogic.Managers.SitesManager.GetSites();
                list.Add(new SiteDT() { Id = 0, Name = "All Web Sites" });
                list = list.OrderBy(x => x.Id).ToList();

                return list;
            }
        }
        public Dictionary<SiteDT,List<ReportSiteCreditCardDataVM>> Data { get; set; }
        public int Total { get; set; }
    }

    public class ReportSiteCreditCardDataVM
    {
        public string Name { get; set; }
        public int TotalSignup { get; set; }
        public int TotalActive { get; set; }
    }
}