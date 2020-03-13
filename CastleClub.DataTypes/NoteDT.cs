using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.DataTypes
{
    public class NoteDT
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
                
        public int  UserId { get; set; }

        public UserDT User { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
