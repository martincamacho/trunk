using CastleClub.BusinessLogic.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace CastleClub.FrontEnd.Utils.Validators
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo property = validationContext.ObjectType.GetProperty("SiteId");
            int siteId = (int)property.GetValue(validationContext.ObjectInstance, null);
            string email = (string)value;

            if (CustomersManager.EmailExists(email, siteId))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}