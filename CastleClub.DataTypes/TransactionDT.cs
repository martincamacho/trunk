using CastleClub.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes
{
    public class TransactionDT
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public int CreditCardId { get; set; }

        public long AuthorizeTransactionId { get; set; }

        public TransactionType Type { get; set; }

        public TransactionStatus Status { get; set; }

        public DateTime SubmitDate { get; set; }

        public int RefundedTransactionId { get; set; }

        public long RefundedTrnAuthorizeId { get; set; }

        public string Message { get; set; }
    }
}
