using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Data
{
    partial class Note
    {
        public NoteDT GetDT()
        {
            NoteDT res = new NoteDT();
            res.Id = Id;
            res.CustomerId = CustomerId;
            res.UserId = UserId;
            res.User = User.GetDT();
            res.Text = Text;
            res.CreatedAt = CreatedAt;
            return res;
        }
    }
}
