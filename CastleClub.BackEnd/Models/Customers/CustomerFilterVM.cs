using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class CustomerFilterVM
    {
        public string Letter { get; set; }
        public int Page { get; set; }
        public string Word { get; set; }
        public bool All { get; set; }
        public bool OnlyActive { get; set; }
        public string SiteName { get; set; }
    }

    public class CustomerEnabled
    {
        public int IdCustomer { get; set; }
    }
}