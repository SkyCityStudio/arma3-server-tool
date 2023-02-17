using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Entity
{
    public class ModsEntity
    {
        public ModsEntity(string ModPath,string ModDirName, string ModName,long ModId,bool LocalMod,bool ServerMod,bool HeadlessClientMod ,bool InputLocalMod) { 
            this.LocalMod = LocalMod;
            this.ServerMod = ServerMod;
            this.ModDirName = ModDirName;
            this.ModPath = ModPath;
            this.ModName = ModName;
            this.ModId = ModId;
            this.HeadlessClientMod = HeadlessClientMod;
            this.InputLocalMod = InputLocalMod;
        }

        public ModsEntity()
        {
        }

        public string ModPath { get; set; }

        public string ModDirName { get; set; }
        public string ModName { get; set; }
        public long ModId { get; set; }
        public bool LocalMod { get; set; }
        public bool ServerMod { get; set; }
        public bool HeadlessClientMod { get; set; }
        public long TimeStamp { get; set; }

        public bool InputLocalMod { get; set; }



    }
}
