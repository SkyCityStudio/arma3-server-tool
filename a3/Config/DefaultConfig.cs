using a3.Entity;
using a3.Tools;
using Arma3ServerTools.Entity;
using BytexDigital.BattlEye.Rcon.Domain;
using DevExpress.Xpo.Metadata;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Config
{
    public class DefaultConfig
    {

        public static string Version = "1.0.9.6";

        public static ISchedulerFactory schedFact = new StdSchedulerFactory();

        public static IScheduler sched;

        public async static void StartIScheduler()
        {
            sched = await schedFact.GetScheduler();
            await sched.Start();
        }

        public static List<BansUrlEntity> bansUrlEntities = new List<BansUrlEntity>();

        public static List<ModuleScanPathEntity> ModuleScanPath = new List<ModuleScanPathEntity>();
        public static string ModInfoUrl = "http://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/";

        public static SteamcmdEntity steamcmd = new SteamcmdEntity();

        public static string mx = MachineCodeTools.GetCpuInfo().Trim() + MachineCodeTools.GetHDid().Trim() + MachineCodeTools.GetMoAddress().Trim() + "383121955";

        public static ArmaServerConfig DefaultServer { get; set; } = new ArmaServerConfig();

        public static Dictionary<String, ArmaServerConfig> ServerList { get; set; } = new Dictionary<String, ArmaServerConfig>();

        public static bool Saveing { get; set; } = false;

        public static List<PlayerDB> PlayerInfo { get; set; } = new List<PlayerDB>();

        public static List<string> HtmlMods { get; set; } = new List<string>();

        public static string DoubleQuotes = "\"";

        public static string LeftSquareBrackets = "{";

        public static string RightSquareBrackets = "}";

        public static string Semicolon = ";";

        public static string Comma = ",";

        public static string Tab = "    ";

        public static Dictionary<string, RconEntity> RconMap { get; set; } = new Dictionary<string, RconEntity>();
    }
}
