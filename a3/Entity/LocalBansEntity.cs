using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arma3ServerTools.Entity
{
    public class LocalBansEntity
    {
        public LocalBansEntity()
        {
        }

        public LocalBansEntity(string gUID, string time, string reason, string addTime,string syncName)
        {
            GUID = gUID;
            Time = time;
            Reason = reason;
            AddTime = addTime;
            SyncName = syncName;
        }

        public string GUID { get; set; }
        public string Time { get; set; }

        public string Reason { get; set; }

        public string AddTime { get; set; }

        public string SyncName { get; set; }

    }
}
