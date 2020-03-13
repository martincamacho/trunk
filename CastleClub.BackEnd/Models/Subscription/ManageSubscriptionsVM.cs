using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class ManageSubscriptionsVM : BaseVM
    {
        public List<SiteDT> Sites { get; set; }

        [Required]
        public string SiteId { get; set; }

        public string MemberId { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Name { get; set; }

        public string LastFourDigit { get; set; }
    }

    public class InvoiceVM
    {
        [Required]
        public int Id { get; set; }
    }
}