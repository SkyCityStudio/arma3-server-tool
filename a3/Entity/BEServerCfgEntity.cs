using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Entity
{
    public class BEServerCfgEntity
    {
        public BEServerCfgEntity(int maxNumbe, int seconds)
        {
            MaxNumbe = maxNumbe;
            Seconds = seconds;
        }

        public BEServerCfgEntity()
        {
        }

        public int MaxNumbe { get; set; }
        public int Seconds { get; set; }
    }
}
