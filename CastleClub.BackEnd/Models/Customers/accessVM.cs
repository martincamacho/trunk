using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class AccessVM
    {
        public string option { get; set; }
        public string response { get; set; }

        public long id { get; set; }
        public Nullable<DateTime> created_at { get; set; }
        public string status { get; set; }
        public Nullable<long> valid_members_count { get; set; }
        public Nullable<int> invalid_members_count { get; set; }
        public Nullable<DateTime> imported_at { get; set; }
        public string show_import { get; set; }
        public string valid_members_csv { get; set; }
        public string invalid_members_csv { get; set; }
        
    }
}