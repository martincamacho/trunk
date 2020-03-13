using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class AccountVM
    {
        public LoginFormVM LoginFormVM { get; set; }

        public ForgotPasswordVM ForgotPasswordVM { get; set; }

        [Required]
        public string Captcha { get; set; } 
    }
}