using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class MembershipAgeCountVM
    {
        public int Range_0_to_3_Months_Count_Active {get;set;}
        public int Range_0_to_3_Months_Count_Unactive { get; set; }
        public int Range_3_to_6_Months_Count_Active { get; set; }
        public int Range_3_to_6_Months_Count_Unactive { get; set; }
        public int Range_6_to_12_Months_Count_Active { get; set; }
        public int Range_6_to_12_Months_Count_Unactive { get; set; }
        public int Range_1_to_2_Years_Count_Active { get; set; }
        public int Range_1_to_2_Years_Count_Unactive { get; set; }
        public int Range_2_to_3_Years_Count_Active { get; set; }
        public int Range_2_to_3_Years_Count_Unactive { get; set; }
        public int Range_Old_Count_Active { get; set; }
        public int Range_Old_Count_Unactive { get; set; }
        public int Total { get; set; }

        public bool Show { get; set; }

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
    }
}