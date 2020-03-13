//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CastleClub.Tool.ModifyPassword.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customer
    {
        public Customer()
        {
            this.CreditCards = new HashSet<CreditCard>();
            this.Invoices = new HashSet<Invoice>();
            this.Notes = new HashSet<Note>();
            this.Visits = new HashSet<Visit>();
        }
    
        public int Id { get; set; }
        public int SiteId { get; set; }
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
        public System.DateTime CreatedAt { get; set; }
        public string StatusId { get; set; }
        public bool Refunded { get; set; }
        public Nullable<System.DateTime> LastBillDate { get; set; }
        public System.DateTime NextBillDate { get; set; }
        public Nullable<System.DateTime> CancelledDate { get; set; }
        public int BadLoginCount { get; set; }
        public Nullable<int> ReferrerId { get; set; }
    
        public virtual ICollection<CreditCard> CreditCards { get; set; }
        public virtual Referrer Referrer { get; set; }
        public virtual Site Site { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Visit> Visits { get; set; }
    }
}