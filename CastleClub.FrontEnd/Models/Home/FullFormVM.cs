using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;
using CastleClub.FrontEnd.Utils.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CastleClub.FrontEnd.Models.Home
{
    public class FullFormVM : BaseVM
    {
        public List<Tuple<int, string>> Months
        {
            get
            {
                var res = new List<Tuple<int, string>>();
                for (int i = 1; i <= 12; i++)
                {
                    res.Add(new Tuple<int, string>(i, i + " - " + new DateTime(DateTime.Now.Year, i, 1).ToString("MMMM")));
                }
                return res;
            }
        }

        public List<Tuple<int, int>> Years
        {
            get
            {
                var res = new List<Tuple<int, int>>();
                int thisYear = DateTime.Now.Year;
                for (int i = 0; i <= 20; i++)
                {
                    res.Add(new Tuple<int, int>(thisYear + i, thisYear + i));
                }
                return res;
            }
        }

        public List<StateDT> States { get; set; }

        public int SiteId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string StateId { get; set; }

        [Required]
        [RegularExpression("^[0-9]{5}$")]
        public string ZipCode { get; set; }

        [Required]
        public CCType CardType { get; set; }

        [Required]
        [Luhn]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression("^[0-9]{3,4}$")]
        public string CVV { get; set; }

        [Required]
        [MonthYear(Fields = "ExpYear,ExpMonth", From="+0days")]
        public int ExpMonth { get; set; }

        [Required]
        [MonthYear(Fields = "ExpYear,ExpMonth", From = "+0days")]
        public int ExpYear { get; set; }

        [Required]
        [EmailAddress]
        [UniqueEmail]
        public string Email { get; set; }

        public string Referrer { get; set; }

        public string SelCCType { get; set; }
    }
}