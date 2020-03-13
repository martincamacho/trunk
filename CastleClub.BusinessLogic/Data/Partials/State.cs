using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Data
{
    partial class State
    {
        public StateDT GetDT()
        {
            StateDT res = new StateDT();
            res.Id = Id;
            res.Name = Name;
            return res;
        }
    }
}
