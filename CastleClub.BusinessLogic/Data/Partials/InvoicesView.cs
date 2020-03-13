using CastleClub.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Data
{
    partial class InvoicesView
    {
        public InvoiceStatus Status
        {
            get { return (InvoiceStatus)Enum.Parse(typeof(InvoiceStatus), StatusId); }
        }
    }
}
