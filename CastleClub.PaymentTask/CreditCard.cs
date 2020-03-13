//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CastleClub.Tasks.PaymentTask
{
    using System;
    using System.Collections.Generic;
    
    public partial class CreditCard
    {
        public CreditCard()
        {
            this.Transactions = new HashSet<Transaction>();
        }
    
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public long AuthorizePaymentProfileId { get; set; }
        public string Data { get; set; }
        public bool Successful { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string LastFourDigit { get; set; }
        public string Type { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
