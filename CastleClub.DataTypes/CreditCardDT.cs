using CastleClub.DataTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes
{
    public class CreditCardDT
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public long AuthorizePaymentProfileId { get; set; }

        public string Data { get; set; }

        public bool Succesfull { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CardNumber { get; set; }

        public string CVV { get; set; }

        public string ExpDate { get; set; }

        public string LastFourDigit { get; set; }

        public CCType Type { get; set; }
    }
}
