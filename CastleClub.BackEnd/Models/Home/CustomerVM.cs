using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CastleClub.BackEnd.Models
{
    public class CustomerVM
    {
        public int Id { get; set; }

        public string MemberId { get; set; }

        public int SiteId { get; set; }

        public string Site { get; set; }

        public string Referrer { get; set; }

        public int NcId { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int SaltKey { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string StateId { get; set; }

        public string ZipCode { get; set; }

        public long AuthorizeProfileId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ClearPassword { get; set; }

        public string Status { get; set; }

        public bool Refunded { get; set; }

        public DateTime LastBillDate { get; set; }

        public DateTime NextBillDate { get; set; }

        public DateTime? CancelledDate { get; set; }

        public int BadLoginCount { get; set; }

        public string CreditCardType { get; set; }
        public bool EmailForm { get; set; }

        public static CustomerVM TransformFromCustomerDT(CustomerDT customer, string creditCardType)
        {
            return (new CustomerVM()
            {
                Address = customer.Address,
                AuthorizeProfileId = customer.AuthorizeProfileId,
                BadLoginCount = customer.BadLoginCount,
                CancelledDate = customer.CancelledDate,
                City = customer.City,
                CreatedAt = customer.CreatedAt,
                Email = customer.Email,
                FirstName = customer.FirstName,
                Id = customer.Id,
                LastBillDate = customer.LastBillDate,
                LastName = customer.LastName,
                NcId = customer.NcId,
                NextBillDate = customer.NextBillDate,
                Phone = customer.Phone,
                Referrer = customer.Referrer,
                Refunded = customer.Refunded,
                Site = customer.Site,
                SiteId = customer.SiteId,
                StateId = customer.StateId,
                Status = customer.Status.ToString(),
                ZipCode = customer.ZipCode,
                CreditCardType= creditCardType,
                EmailForm=customer.EmailForm
            });
        }
    }
}