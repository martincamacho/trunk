using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BackEnd.Models.Report
{
    public class SiteVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool ProcessCsv { get; set; }
    }
}
