using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Data
{
    partial class Visit
    {
        public VisitDT GetDT()
        {
            VisitDT res = new VisitDT();
            res.Id = Id;
            res.SiteId = SiteId;
            res.CustomerId = CustomerId.HasValue ? CustomerId.Value : 0;
            res.IPAddress = IPAddress;
            res.UserAgent = UserAgent;
            res.CreatedAt = CreatedAt;
            return res;
        }
    }
}
