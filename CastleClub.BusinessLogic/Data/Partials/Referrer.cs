using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Data
{
    partial class Referrer
    {
        public ReferrerDT GetDT()
        {
            ReferrerDT res = new ReferrerDT();
            res.Id = Id;
            res.Name = Name;
            res.URL = URL;
            res.SiteKey = SiteKey;
            res.SiteIdentifier = SiteIdentifier;
            res.SiteValidator = SiteValidator;
            res.PostBackURL = PostBackURL;
            return res;
        }
    }
}
