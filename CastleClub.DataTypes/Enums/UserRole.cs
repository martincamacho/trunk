using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes.Enums
{
    public enum UserRole
    {
        NOUSER = 0,
        PARTNER = 10,
        FULLPARTNER = 20,
        CUSTOMERSERVICE = 30,
        ADMIN = 40,
        SUPERADMIN = 50
    }

    public static class UserRoleHelper
    {
        public static string GetString(UserRole item)
        {
            switch (item)
            {
                case UserRole.NOUSER: return "No User";
                case UserRole.PARTNER: return "Partner";
                case UserRole.FULLPARTNER: return "Full Partner";
                case UserRole.CUSTOMERSERVICE: return "Customer Service";
                case UserRole.ADMIN: return "Admin";
                case UserRole.SUPERADMIN: return "Super Admin";
            }
            throw new InvalidOperationException();
        }
    }
}
