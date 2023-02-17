using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace a3.Entity
{
    public class PlayerDB
    {
        public int Id { get; private set; }
        public string Ip { get; private set; }
        public string Guid { get; private set; }
        public string Name { get; private set; }

        public string Time { get; private set; }

        public PlayerDB(int id, string ip, string guid, string name, string time)
        {
            Id = id;
            Ip = ip;
            Guid = guid;
            Name = name;
            Time = time;
        }



    }
}
