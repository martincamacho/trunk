using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class TestVM
    {
        [DataType(DataType.EmailAddress)]
        public string EmailToSend { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Name { get; set; }
        public List<string> Emails { get; set; }
        [Required]
        public int SiteID { get; set; }
        public List<SiteDT> Sites { get; set; }
    }
}