using CastleClub.DataTypes.Enums;
using CastleClub.FrontEnd.Utils.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class NewUserVM : BaseVM
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        [UniqueEmail]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "User Level")]
        public UserRole UserLevel { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } 
    }
}