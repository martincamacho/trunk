using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace CastleClub.FrontEnd.Utils.Validators
{
    public class MonthYearAttribute : ValidationAttribute
    {
        public string Fields { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        private List<string> FieldsList
        {
            get
            {
                return Fields.Split(',').ToList();
            }
        }

        private DateTime? ParseDate(string time)
        {
            if (time == null)
            {
                return null;
            }
            Match m = Regex.Match(time, "((\\+|-)[0-9]+)((days)|(years)|(months))");
            DateTime res = DateTime.Now.Date;
            if (m.Success)
            {
                int amount = int.Parse(m.Groups[1].Value);
                if (m.Groups[3].Value == "days")
                {
                    res = res.AddDays(amount);
                }
                else if (m.Groups[3].Value == "years")
                {
                    res = res.AddYears(amount);
                }
                else if (m.Groups[3].Value == "months")
                {
                    res = res.AddMonths(amount);
                }
            }
            return res;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo property = validationContext.ObjectType.GetProperty(FieldsList[0]);
            if (property.GetValue(validationContext.ObjectInstance, null) == null)
                return ValidationResult.Success;
            int year = (int)property.GetValue(validationContext.ObjectInstance, null);
            property = validationContext.ObjectType.GetProperty(FieldsList[1]);
            if (property.GetValue(validationContext.ObjectInstance, null) == null)
                return ValidationResult.Success;
            int month = (int)property.GetValue(validationContext.ObjectInstance, null);
            if (year == 0 || month == 0)
            {
                return new ValidationResult(ErrorMessage);
            }
            DateTime minimumDate = new DateTime(year, month, 1);
            DateTime maximumDate = minimumDate.AddMonths(1).AddDays(-1);
            DateTime? fromDT = ParseDate(From);
            DateTime? toDT = ParseDate(To);

            if (fromDT.HasValue && maximumDate < fromDT.Value)
            {
                return new ValidationResult(ErrorMessage);
            }
            if (toDT.HasValue && minimumDate > toDT.Value)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}