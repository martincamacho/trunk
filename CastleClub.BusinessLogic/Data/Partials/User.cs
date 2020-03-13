using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Data
{
    partial class User
    {
        public UserDT GetDT()
        {
            UserDT res = new UserDT();
            res.Id = Id;
            res.Email = Email;
            res.FirstName = FirstName;
            res.LastName = LastName;
            res.Role = Role;
            res.RoleId = RoleId;
            res.AspNetId = AspNetId;
            return res;
        }

        public UserRole Role
        {
            get { return (UserRole)Enum.Parse(typeof(UserRole), RoleId); }
            set { RoleId = value.ToString(); }
        }
    }
}
