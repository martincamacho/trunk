using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CastleClub.BackEnd.Models
{
    public class ResetPasswordVM
    {
        [Required]
        [Display(Name="Current password:")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [Display(Name = "New password:")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}