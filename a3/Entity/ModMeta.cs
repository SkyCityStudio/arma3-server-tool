using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Entity
{
    public class ModMeta
    {
        public ModMeta(long publishedId, string name, long timeStamp)
        {
            PublishedId = publishedId;
            Name = name;
            TimeStamp = timeStamp;
        }

        public ModMeta()
        {
        }

        public long PublishedId { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public long TimeStamp { get; set; } = 0;

    }
}
