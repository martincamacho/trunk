using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes.Enums
{
    public enum CCType
    {
        VISA,
        MASTERCARD,
        DISCOVER
    }

    public static class CCTypeHelper
    {
        public static string GetString(CCType item)
        {
            switch (item)
            {
                case CCType.VISA: return "Visa";
                case CCType.MASTERCARD: return "Mastercard";
                case CCType.DISCOVER: return "Discover";
            }
            throw new InvalidOperationException();
        }
    }
}
