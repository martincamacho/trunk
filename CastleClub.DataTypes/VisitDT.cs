using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes
{
    public class VisitDT
    {
        public int Id { get; set; }

        public int SiteId { get; set; }

        public int CustomerId { get; set; }

        public string IPAddress { get; set; }

        public string UserAgent { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
