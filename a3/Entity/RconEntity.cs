using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arma3ServerTools.Entity
{
    public class RconEntity
    {
        public RconEntity()
        {
        }

        public RconEntity(string serverName, string iP, int port, string password, string id)
        {
            ServerName = serverName;
            IP = iP;
            Port = port;
            Password = password;
            this.id = id;
        }

        public string id { get; set; }
        public string ServerName { get; set; } = string.Empty;
        public string IP { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
