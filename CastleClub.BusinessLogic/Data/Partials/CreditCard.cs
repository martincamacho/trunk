using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Data
{
    partial class CreditCard
    {
        public CreditCardDT GetDT()
        {
            CreditCardDT res = new CreditCardDT();
            res.Id = Id;
            res.CustomerId = CustomerId;
            res.AuthorizePaymentProfileId = AuthorizePaymentProfileId;
            res.Data = Data;
            res.Succesfull = Successful;
            res.CreatedAt = CreatedAt;
            res.LastFourDigit = LastFourDigit;
            return res;
        }
    }
}
