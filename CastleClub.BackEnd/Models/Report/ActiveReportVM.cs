using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CastleClub.BackEnd.Models.Report
{
    public class ActiveReportVM
    {
        public List<SiteVM> sites { get; set; }
        public bool AccessList { get; set; }

        public ActiveReportVM() 
        {
            sites = new List<SiteVM>();
            AccessList = false;
        }
    }
}
