using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CastleClub.BackEnd.Models;
using CastleClub.BusinessLogic.Managers;
using CastleClub.BusinessLogic.Utils;
using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;

namespace CastleClub.BackEnd.Models
{
    public class WelcomeEmailVM : BaseVM
    {
        public List<SiteDT> sites { get; set; }
        //public bool[] siteID { get; set; }
        public int prueba { get; set; }
        public int CapEmail { get; set; }
        public List<Tuple<int, string>> Delays
        {
            get
            {
                var res = new List<Tuple<int, string>>();
                
                res.Add(new Tuple<int, string>(0, "0"));
                res.Add(new Tuple<int, string>(1, "1"));
                res.Add(new Tuple<int, string>(2, "2"));
                res.Add(new Tuple<int, string>(3, "3"));
                res.Add(new Tuple<int, string>(7, "7"));
                
                return res;
            }
        }
        public int Delay { get; set; }
    }
}
