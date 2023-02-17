using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Entity
{
    public class SteamcmdEntity
    {
        public SteamcmdEntity(string u, string p, string d, string i)
        {
            this.u = u;
            this.p = p;
            this.d = d;
            this.i = i;
        }

        public SteamcmdEntity()
        {
        }

        public string u { get; set; }
        public string p { get; set; }
        public string d { get; set; }
        public string i { get; set; }
    }
}