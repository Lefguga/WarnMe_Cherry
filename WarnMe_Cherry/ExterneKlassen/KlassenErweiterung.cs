using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarnMe_Cherry.ExterneKlassen
{
    public static class KlassenErweiterung
    {
        public static int IndexOf(this SortedDictionary<DateTime, Arbeitstag> dictionary, DateTime key)
        {
            var value = dictionary[key];
            int i = 0;
            foreach (var _key in dictionary.Keys)
            {
                if (dictionary[_key] == value)
                {
                    return i++;
                }
            }
            return -1;
        }

    }
}
