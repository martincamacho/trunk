using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Data
{
    partial class Invoice
    {
        public InvoiceDT GetDT()
        {
            InvoiceDT res = new InvoiceDT();
            res.Id = Id;
            res.CustomerId = CustomerId;
            res.Amount = Amount;
            res.Status = Status;
            res.CreatedAt = CreatedAt;
            res.BilledDate = BilledDate.HasValue ? BilledDate.Value : DateTime.MinValue;
            res.RefundedDate = RefundedDate.HasValue ? RefundedDate.Value : DateTime.MinValue;
            res.RefundReason = RefundReason;
            res.Credit = Status == InvoiceStatus.BILLED || Status == InvoiceStatus.REFUNDED || Status == InvoiceStatus.REFUNDEDFAIL ? Amount : 0;
            res.Debit = Status == InvoiceStatus.REFUNDED ? Amount : 0;
            res.Balance = res.Credit - res.Debit;
            res.Transactions = Transactions.ToList().Select(t => t.GetDT()).ToList();
            return res;
        }

        public InvoiceStatus Status
        {
            get { return (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), StatusId); }
            set { StatusId = value.ToString(); }
        }
    }
}
