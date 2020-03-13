using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Utils
{
    public class CreditCardHelper
    {
        public static string GetCardType(string cardNumber)
        {
            string reVisa = @"^4[0-9]{12}(?:[0-9]{3})?$";
            string reMC = @"^5[1-5][0-9]{14}$";
            string reAmex = @"^3[47][0-9]{13}$";
            string reDiscover = @"^6(?:011|5[0-9]{2})[0-9]{12}$";

            string ret = "";
            if (Regex.Match(cardNumber, reVisa).Success)
            {
                ret = "VISA";
            }
            else if (Regex.Match(cardNumber, reMC).Success)
            {
                ret = "MASTERCARD";
            }
            else if (Regex.Match(cardNumber, reAmex).Success)
            {
                ret = "AMEX";
            }
            else if (Regex.Match(cardNumber, reDiscover).Success)
            {
                ret = "DISCOVER";
            }
            return ret;
        }

        public static string GetExpDate(int expMonth, int expYear)
        {
            return String.Format("{0:0000}-{1:00}", expYear, expMonth);
        }

        public static bool Valid(string cardNumber, bool allowSpaces)
        {
            if (allowSpaces)
            {
                cardNumber = cardNumber.Replace(" ", "");
            }

            if (cardNumber.Any(c => !Char.IsDigit(c)))
            {
                return false;
            }

            int checksum = cardNumber
               .Select((c, i) => (c - '0') << ((cardNumber.Length - i - 1) & 1))
               .Sum(n => n > 9 ? n - 9 : n);

            return (checksum % 10) == 0 && checksum > 0;
        }
    }
}
