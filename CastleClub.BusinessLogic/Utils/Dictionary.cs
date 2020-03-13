using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Utils
{
    public class Dictionary
    {
        public static Dictionary<K,V> SortByKey<K,V>(Dictionary<K,V> dictionary)
        {
            var response = new Dictionary<K, V>();
            foreach (var key in dictionary.Keys.OrderBy(x => x))
            {
                response.Add(key, dictionary[key]);
            }

            return response;
        }
    }
}
