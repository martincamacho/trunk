using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using CastleClub.BackEnd.Models;
using CastleClub.BusinessLogic.Managers;
using CastleClub.BusinessLogic.Utils;
using CastleClub.DataTypes;
using CastleClub.DataTypes.Enums;

namespace CastleClub.BackEnd.Models
{
    public class ReportIISResetVM : BaseVM
    {
        public List<IISResetLogDT> Log { get; set; }
    }
}