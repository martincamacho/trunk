using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes.Enums
{
    public enum InvoiceStatus
    {
        NEW,
        CANCELED,
        BILLED,
        BILLEDFAIL,
        REFUNDED,
        REFUNDING,
        REFUNDEDFAIL
    }

    public static class InvoiceStatusHelper
    {
        public static string GetString(InvoiceStatus item)
        {
            switch (item)
            {
                case InvoiceStatus.NEW: return "NEW";
                case InvoiceStatus.CANCELED: return "CANCELED";
                case InvoiceStatus.BILLED: return "BILLED";
                case InvoiceStatus.BILLEDFAIL: return "BILLING FAILED";
                case InvoiceStatus.REFUNDED: return "REFUNDED";
                case InvoiceStatus.REFUNDING: return "REFUNDING";
                case InvoiceStatus.REFUNDEDFAIL: return "REFUND FAIL";
            }
            throw new InvalidOperationException();
        }
    }
}
