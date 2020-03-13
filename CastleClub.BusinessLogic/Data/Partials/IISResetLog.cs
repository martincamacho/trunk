using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;

namespace CastleClub.BusinessLogic.Data
{
    partial class IISResetLog
    {
        public IISResetLogDT GetDT()
        {
            IISResetLogDT res = new IISResetLogDT();
            res.Status = Status;
            res.Date = Date.HasValue ? Date.Value : DateTime.MinValue ;

            return res;
        }
    }
}
