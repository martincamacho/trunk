using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Data
{
    partial class Transaction
    {
        public TransactionDT GetDT()
        {
            TransactionDT res = new TransactionDT();
            res.Id = Id;
            res.InvoiceId = InvoiceId;
            res.CreditCardId = CreditCardId;
            res.AuthorizeTransactionId = AuthorizeTransactionId;
            res.Type = Type;
            res.Status = Status;
            res.SubmitDate = SubmitDate;
            res.RefundedTransactionId = RefundedTransactionId.HasValue ? RefundedTransactionId.Value : 0;
            res.RefundedTrnAuthorizeId = RefundedTransactionId.HasValue ? RefundedTransaction.AuthorizeTransactionId : 0;
            res.Message = Message;
            return res;
        }

        public TransactionType Type
        {
            get { return (TransactionType)Enum.Parse(typeof(TransactionType), TypeId); }
            set { TypeId = value.ToString(); }
        }

        public TransactionStatus Status
        {
            get { return (TransactionStatus)Enum.Parse(typeof(TransactionStatus), StatusId); }
            set { StatusId = value.ToString(); }
        }
    }
}
