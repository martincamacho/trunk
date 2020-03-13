using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes
{
    public class ReferrerDT
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string URL { get; set; }

        public string SiteKey { get; set; }

        public string SiteIdentifier { get; set; }

        public string SiteValidator { get; set; }

        public string PostBackURL { get; set; }
    }
}
