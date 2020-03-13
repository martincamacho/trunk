using CastleClub.BusinessLogic.Data;
using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Managers
{
    public static class LocationsManager
    {
        public static List<StateDT> GetStates()
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                return entities.States.OrderBy(s => s.Name).ToList().Select(s => s.GetDT()).ToList();
            }
        }

        public static string GetStateName(string id)
        {
            using (CastleClubEntities entities = new CastleClubEntities())
            {
                return entities.States.Where(s => s.Id == id).FirstOrDefault().Name;
            }
        }
    }
}
