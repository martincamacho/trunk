using CastleClub.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes
{
    public class InvoiceDT
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public decimal Amount { get; set; }

        public InvoiceStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime BilledDate { get; set; }

        public DateTime RefundedDate { get; set; }

        public string RefundReason { get; set; }

        public decimal Credit { get; set; }

        public decimal Debit { get; set; }

        public decimal Balance { get; set; }

        public List<TransactionDT> Transactions { get; set; }
    }
}
