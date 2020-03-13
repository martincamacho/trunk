using CastleClub.FrontEnd.Utils.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CastleClub.FrontEnd.Models.Home
{
    public class EmailFormVM : BaseVM
    {
        public int SiteId { get; set; }

        [Required]
        [EmailAddress]
        [UniqueEmail]
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        [Compare("Email")]
        public string EmailVerification { get; set; }
    }
}