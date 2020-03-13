using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class AuthorizeFileVM
    {
        [DataType(DataType.Upload)]
        [Required]
        public HttpPostedFileBase File { get; set; }
        public bool Upload { get; set; }
        public bool OnlyRefund { get; set; }
    }
}