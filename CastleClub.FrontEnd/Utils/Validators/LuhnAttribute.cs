using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CastleClub.FrontEnd.Utils.Validators
{
    public class LuhnAttribute : ValidationAttribute
    {
        public bool AllowSpaces { get; set; }
        public bool AllowEmpty { get; set; }

        public override bool IsValid(object value)
        {
            string cardNumber = (string)value;

            if (String.IsNullOrEmpty(cardNumber))
            {
                return AllowEmpty;
            }

            return LuhnUtility.IsCardNumberValid(cardNumber, AllowSpaces);
        }
    }
}