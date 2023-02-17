using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Entity
{
    public class ModuleScanPathEntity
    {
        public ModuleScanPathEntity(string modulePath, string prefix,string remark)
        {
            ModulePath = modulePath;
            Prefix = prefix;
            Remark = remark;
        }

        public ModuleScanPathEntity()
        {
        }

        public string ModulePath { get; set; }
        public string Prefix { get; set; }
        public string Remark { get; set; }
    }
}
