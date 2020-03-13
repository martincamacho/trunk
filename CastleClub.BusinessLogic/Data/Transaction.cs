
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace CastleClub.BusinessLogic.Data
{

using System;
    using System.Collections.Generic;
    
public partial class Transaction
{

    public Transaction()
    {

        this.RefundsTransactionId = new HashSet<Transaction>();

    }


    public int Id { get; set; }

    public int InvoiceId { get; set; }

    public int CreditCardId { get; set; }

    public long AuthorizeTransactionId { get; set; }

    public string TypeId { get; set; }

    public string StatusId { get; set; }

    public System.DateTime SubmitDate { get; set; }

    public Nullable<int> RefundedTransactionId { get; set; }

    public string Message { get; set; }



    public virtual CreditCard CreditCard { get; set; }

    public virtual ICollection<Transaction> RefundsTransactionId { get; set; }

    public virtual Transaction RefundedTransaction { get; set; }

    public virtual Invoice Invoice { get; set; }

}

}
