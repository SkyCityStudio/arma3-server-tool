using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arma3ServerTools.Entity
{
    public class BansUrlEntity
    {

        public string url;
        public bool enable;

        public BansUrlEntity(string url, bool enable)
        {
            this.url = url;
            this.enable = enable;
        }

        public BansUrlEntity()
        {
        }
    }
}
