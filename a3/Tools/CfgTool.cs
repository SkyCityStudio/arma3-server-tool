using a3.Config;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace a3.Tools
{
    public class CfgTool
    {
        public static UTF8Encoding UTF8 = new UTF8Encoding(false);

        public static string GetHeadlessClientCommandLine(string serverUid) {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(DefaultConfig.ServerList[serverUid].ServerConfig.Password)) {
                sb.Append(" ").Append("-password=").Append(DefaultConfig.ServerList[serverUid].ServerConfig.Password);
            }
            int hcPort = DefaultConfig.ServerList[serverUid].StartupParameters.Port + 5;
            for (int i = 0; i < 10;i++) {
                Random r = new Random();
                hcPort = r.Next(100) + DefaultConfig.ServerList[serverUid].StartupParameters.Port;
                if (!UsePort(hcPort)) {
                    break;
                }
            }
            sb.Append(" -limitFPS=1000 -client -connect=127.0.0.1:").Append(DefaultConfig.ServerList[serverUid].StartupParameters.Port).Append(" -prot=").Append(hcPort);
            sb.Append(" ").Append(DefaultConfig.DoubleQuotes).Append("-profiles=").Append(DefaultConfig.ServerList[serverUid].ServerDir + @"\destiny_serverconfig\").Append(DefaultConfig.ServerList[serverUid].ServerUUID).Append(DefaultConfig.DoubleQuotes);
            sb.Append(" ").Append(DefaultConfig.DoubleQuotes).Append("-name=").Append(DefaultConfig.ServerList[serverUid].ServerUUID).Append(DefaultConfig.DoubleQuotes);

            string HeadlessClientMod = "";
            DefaultConfig.ServerList[serverUid].StartupParameters.modsEntities.ForEach(entity => {
                if (entity.HeadlessClientMod)
                {
                    HeadlessClientMod += entity.ModPath + DefaultConfig.Semicolon;
                }
            });
            sb.Append(" ").Append(DefaultConfig.DoubleQuotes).Append("-mod=").Append(HeadlessClientMod).Append(DefaultConfig.DoubleQuotes);
            sb.Append(" -noPause -noSound");
            return sb.ToString();
        }
        public static void SaveStartCommandLine(string serverUid) {
            StringBuilder sb=new StringBuilder();
            sb.Clear();
            sb.Append(DefaultConfig.ServerList[serverUid].StartupParameters.AutoInit ? " -autoInit" : "")
              .Append(DefaultConfig.ServerList[serverUid].StartupParameters.FilePatching ? " -filePatching" : "")
              .Append(string.IsNullOrEmpty(DefaultConfig.ServerList[serverUid].StartupParameters.PidFile) ? "" : " -pid=" + DefaultConfig.ServerList[serverUid].StartupParameters.PidFile)
              .Append(string.IsNullOrEmpty(DefaultConfig.ServerList[serverUid].StartupParameters.Ranking) ? "" : " -ranking=" + DefaultConfig.ServerList[serverUid].StartupParameters.Ranking)
              .Append(" -port=").Append(DefaultConfig.ServerList[serverUid].StartupParameters.Port.ToString())
              .Append(DefaultConfig.ServerList[serverUid].StartupParameters.BandwidthAlg ? " -bandwidthAlg=2" : "")
              .Append(DefaultConfig.ServerList[serverUid].StartupParameters.EnableHT ? " -enableHT" : "")
              .Append(DefaultConfig.ServerList[serverUid].StartupParameters.Hugepages ? " -hugepages" : "")
              .Append(DefaultConfig.ServerList[serverUid].StartupParameters.LoadMissionToMemory ? " -loadMissionToMemory" : "")
              .Append(DefaultConfig.ServerList[serverUid].StartupParameters.DisableServerThread ? " -disableServerThread" : "")
              .Append(DefaultConfig.ServerList[serverUid].StartupParameters.CpuCount > 0 ? " -cpuCount=" + DefaultConfig.ServerList[serverUid].StartupParameters.CpuCount : "")
              .Append(DefaultConfig.ServerList[serverUid].StartupParameters.ExThreads > 0 ? " -exThreads=" + DefaultConfig.ServerList[serverUid].StartupParameters.ExThreads : "")
              .Append(DefaultConfig.ServerList[serverUid].StartupParameters.MaxMem > 0 ? " -maxMem=" + DefaultConfig.ServerList[serverUid].StartupParameters.MaxMem : "")
              .Append(" -limitFPS=").Append(DefaultConfig.ServerList[serverUid].StartupParameters.LimitFPS.ToString())
              .Append(DefaultConfig.ServerList[serverUid].StartupParameters.NoLogs ? " -noLogs" : "")
              .Append(DefaultConfig.ServerList[serverUid].StartupParameters.Netlog ? " -netlog" : "");

            sb.Append(" ").Append(DefaultConfig.DoubleQuotes).Append("-config=").Append(DefaultConfig.ServerList[serverUid].ServerDir + @"\destiny_serverconfig\").Append(DefaultConfig.ServerList[serverUid].ServerUUID).Append(@"\server.cfg").Append(DefaultConfig.DoubleQuotes);
            sb.Append(" ").Append(DefaultConfig.DoubleQuotes).Append("-cfg=").Append(DefaultConfig.ServerList[serverUid].ServerDir + @"\destiny_serverconfig\").Append(DefaultConfig.ServerList[serverUid].ServerUUID).Append(@"\basic.cfg").Append(DefaultConfig.DoubleQuotes);

            sb.Append(" ").Append(DefaultConfig.DoubleQuotes).Append("-profiles=").Append(DefaultConfig.ServerList[serverUid].ServerDir + @"\destiny_serverconfig\").Append(DefaultConfig.ServerList[serverUid].ServerUUID).Append(DefaultConfig.DoubleQuotes);
            sb.Append(" ").Append(DefaultConfig.DoubleQuotes).Append("-name=").Append(DefaultConfig.ServerList[serverUid].ServerUUID).Append(DefaultConfig.DoubleQuotes);

            string ClientMod = "";
            string ServerMod = "";
            if (DefaultConfig.ServerList[serverUid].StartupParameters.DLCWS)
            {
                ClientMod += "WS;";
            }

            if (DefaultConfig.ServerList[serverUid].StartupParameters.DLCVN)
            {
                ClientMod += "VN;";
            }

            if (DefaultConfig.ServerList[serverUid].StartupParameters.DLCCSLA)
            {
                ClientMod += "CSLA;";
            }

            if (DefaultConfig.ServerList[serverUid].StartupParameters.DLCGM)
            {
                ClientMod += "GM;";
            }

            if (DefaultConfig.ServerList[serverUid].StartupParameters.DLCcontact)
            {
                ClientMod += "contact;";
            }

            DefaultConfig.ServerList[serverUid].StartupParameters.modsEntities.ForEach(entity => {
                if (entity.LocalMod)
                {
                    ClientMod += entity.ModPath + DefaultConfig.Semicolon;
                }
                if (entity.ServerMod)
                {
                    ServerMod += entity.ModPath + DefaultConfig.Semicolon;
                }
            });

            

            if (DefaultConfig.ServerList[serverUid].ServerTaskManagement.EnableMonitor) {
                ServerMod += "@destiny_server" + DefaultConfig.Semicolon;
            }
            sb.Append(" ").Append(DefaultConfig.DoubleQuotes).Append("-mod=").Append(ClientMod).Append(DefaultConfig.DoubleQuotes);
            sb.Append(" ").Append(DefaultConfig.DoubleQuotes).Append("-serverMod=").Append(ServerMod).Append(DefaultConfig.DoubleQuotes);
            try
            {
                string args = Encoding.Default.GetString(Convert.FromBase64String(DefaultConfig.ServerList[serverUid].StartupParameters.StartConfigArgs));
                string[] ss = args.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                foreach (string s in ss)
                {
                    sb.Append(" ").Append(s);
                }
            }
            catch { }
            DefaultConfig.ServerList[serverUid].StartCommandLine = sb.ToString();
            Console.WriteLine(sb.ToString());
        }
        public static bool SaveCfg() {
            if (!Directory.Exists(DefaultConfig.DefaultServer.ServerDir+@"\destiny_serverconfig")) {
                Directory.CreateDirectory(DefaultConfig.DefaultServer.ServerDir + @"\destiny_serverconfig");
            }
            if (!Directory.Exists(DefaultConfig.DefaultServer.ServerDir + @"\destiny_serverconfig\"+ DefaultConfig.DefaultServer.ServerUUID))
            {
                Directory.CreateDirectory(DefaultConfig.DefaultServer.ServerDir + @"\destiny_serverconfig\" + DefaultConfig.DefaultServer.ServerUUID);
            }
            if (!Directory.Exists(DefaultConfig.DefaultServer.ServerDir + @"\destiny_serverconfig\" + DefaultConfig.DefaultServer.ServerUUID + @"\Users"))
            {
                Directory.CreateDirectory(DefaultConfig.DefaultServer.ServerDir + @"\destiny_serverconfig\" + DefaultConfig.DefaultServer.ServerUUID + @"\Users");
            }
            if (!Directory.Exists(DefaultConfig.DefaultServer.ServerDir + @"\destiny_serverconfig\" + DefaultConfig.DefaultServer.ServerUUID + @"\Users\"+ DefaultConfig.DefaultServer.ServerUUID))
            {
                Directory.CreateDirectory(DefaultConfig.DefaultServer.ServerDir + @"\destiny_serverconfig\" + DefaultConfig.DefaultServer.ServerUUID + @"\Users\"+ DefaultConfig.DefaultServer.ServerUUID);
            }

            if (!Directory.Exists(DefaultConfig.DefaultServer.ServerDir + @"\destiny_serverconfig\" + DefaultConfig.DefaultServer.ServerUUID + @"\BattlEye"))
            {
                Directory.CreateDirectory(DefaultConfig.DefaultServer.ServerDir + @"\destiny_serverconfig\" + DefaultConfig.DefaultServer.ServerUUID + @"\BattlEye");
            }

            
            StringBuilder sb = new StringBuilder();
            //写出serverconfig.cfg
            sb.Append("hostname=").Append(DefaultConfig.DoubleQuotes).Append(DefaultConfig.DefaultServer.ServerConfig.HostName).Append(DefaultConfig.DoubleQuotes).AppendLine(DefaultConfig.Semicolon);
            sb.Append("password=").Append(DefaultConfig.DoubleQuotes).Append(DefaultConfig.DefaultServer.ServerConfig.Password).Append(DefaultConfig.DoubleQuotes).AppendLine(DefaultConfig.Semicolon);
            sb.Append("maxPlayers=").Append(DefaultConfig.DefaultServer.ServerConfig.MaxPlayers.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("persistent=").Append(DefaultConfig.DefaultServer.ServerConfig.Persistent? "1" : "0").AppendLine(DefaultConfig.Semicolon);
            sb.Append("skipLobby=").Append(DefaultConfig.DefaultServer.ServerConfig.SkipLobby.ToString().ToLower()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("drawingInMap=").Append(DefaultConfig.DefaultServer.ServerConfig.DrawingInMap.ToString().ToLower()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("statisticsEnabled=").Append(DefaultConfig.DefaultServer.ServerConfig.StatisticsEnabled.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("forceRotorLibSimulation=").Append(DefaultConfig.DefaultServer.ServerConfig.ForceRotorLibSimulation.ToString()).AppendLine(DefaultConfig.Semicolon);
            if (DefaultConfig.DefaultServer.ServerConfig.ForcedDifficulty != "none") {
                sb.Append("forcedDifficulty=").Append(DefaultConfig.DoubleQuotes).Append(DefaultConfig.DefaultServer.ServerConfig.ForcedDifficulty).Append(DefaultConfig.DoubleQuotes).AppendLine(DefaultConfig.Semicolon);
            }
            if (DefaultConfig.DefaultServer.ServerConfig.Motd.Count > 0) {
                WriteCfgArray("motd[]=", sb, DefaultConfig.DefaultServer.ServerConfig.Motd);
            }
           

            sb.Append("motdInterval=").Append(DefaultConfig.DefaultServer.ServerConfig.MotdInterval.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("disableVoN=").Append(DefaultConfig.DefaultServer.ServerConfig.DisableVoN.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("vonCodecQuality=").Append(DefaultConfig.DefaultServer.ServerConfig.VonCodecQuality.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("vonCodec=").Append(DefaultConfig.DefaultServer.ServerConfig.VonCodec.ToString()).AppendLine(DefaultConfig.Semicolon);


            if (DefaultConfig.DefaultServer.ServerConfig.HeadlessClients.Count > 0)
            {
                WriteCfgArray("headlessClients[]=", sb, DefaultConfig.DefaultServer.ServerConfig.HeadlessClients);
            }
            if (DefaultConfig.DefaultServer.ServerConfig.LocalClient.Count > 0)
            {
                WriteCfgArray("LocalClient[]=", sb, DefaultConfig.DefaultServer.ServerConfig.LocalClient);
            }

            
            

            if (DefaultConfig.DefaultServer.ServerConfig.VoteThreshold !=0) {
                sb.Append("voteThreshold=").Append(DefaultConfig.DefaultServer.ServerConfig.VoteThreshold.ToString()).AppendLine(DefaultConfig.Semicolon);
            }
            if (DefaultConfig.DefaultServer.ServerConfig.VotingTimeOut != 0)
            {
                sb.Append("votingTimeOut=").Append(DefaultConfig.DefaultServer.ServerConfig.VotingTimeOut.ToString()).AppendLine(DefaultConfig.Semicolon);
            }
            sb.Append("roleTimeOut=").Append(DefaultConfig.DefaultServer.ServerConfig.RoleTimeOut.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("briefingTimeOut=").Append(DefaultConfig.DefaultServer.ServerConfig.BriefingTimeOut.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("debriefingTimeOut=").Append(DefaultConfig.DefaultServer.ServerConfig.DebriefingTimeOut.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("lobbyIdleTimeout=").Append(DefaultConfig.DefaultServer.ServerConfig.LobbyIdleTimeout.ToString()).AppendLine(DefaultConfig.Semicolon);
            if (DefaultConfig.DefaultServer.ServerConfig.VoteMissionPlayers != 0)
            {
                sb.Append("voteMissionPlayers=").Append(DefaultConfig.DefaultServer.ServerConfig.VoteMissionPlayers.ToString()).AppendLine(DefaultConfig.Semicolon);
            }
            sb.Append("BattlEye=").Append(DefaultConfig.DefaultServer.ServerConfig.BattlEye?"1":"0").AppendLine(DefaultConfig.Semicolon);
            sb.Append("verifySignatures=").Append(DefaultConfig.DefaultServer.ServerConfig.VerifySignatures ? "2" : "0").AppendLine(DefaultConfig.Semicolon);
            sb.Append("kickduplicate=").Append(DefaultConfig.DefaultServer.ServerConfig.Kickduplicate.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("allowedFilePatching=").Append(DefaultConfig.DefaultServer.ServerConfig.AllowedFilePatching.ToString()).AppendLine(DefaultConfig.Semicolon);

            if (DefaultConfig.DefaultServer.ServerConfig.FilePatchingExceptions.Count>0) {
                WriteCfgArray("filePatchingExceptions[]=", sb, DefaultConfig.DefaultServer.ServerConfig.FilePatchingExceptions);
            }
            sb.Append("serverCommandPassword=").Append(DefaultConfig.DoubleQuotes).Append(DefaultConfig.DefaultServer.ServerConfig.ServerCommandPassword.ToString()).Append(DefaultConfig.DoubleQuotes).AppendLine(DefaultConfig.Semicolon);
            sb.Append("passwordAdmin=").Append(DefaultConfig.DoubleQuotes).Append(DefaultConfig.DefaultServer.ServerConfig.PasswordAdmin.ToString()).Append(DefaultConfig.DoubleQuotes).AppendLine(DefaultConfig.Semicolon);

            if (DefaultConfig.DefaultServer.ServerConfig.Admins.Count > 0) {
                WriteCfgArray("admins[]=", sb, DefaultConfig.DefaultServer.ServerConfig.Admins);
            }
            

            WriteCfgEvent("doubleIdDetected=",sb, DefaultConfig.DefaultServer.ServerConfig.DoubleIdDetected);
            WriteCfgEvent("onUserConnected=", sb, DefaultConfig.DefaultServer.ServerConfig.onUserConnected);
            WriteCfgEvent("onUserDisconnected=", sb, DefaultConfig.DefaultServer.ServerConfig.onUserDisconnected);
            WriteCfgEvent("onHackedData=", sb, DefaultConfig.DefaultServer.ServerConfig.onHackedData);
            WriteCfgEvent("onDifferentData=", sb, DefaultConfig.DefaultServer.ServerConfig.onDifferentData);
            WriteCfgEvent("onUnsignedData=", sb, DefaultConfig.DefaultServer.ServerConfig.onUnsignedData);
            WriteCfgEvent("onUserKicked=", sb, DefaultConfig.DefaultServer.ServerConfig.onUserKicked);
            WriteCfgEvent("regularCheck=", sb, DefaultConfig.DefaultServer.ServerConfig.RegularCheck);

            if (DefaultConfig.DefaultServer.ServerConfig.AllowedLoadFileExtensions.Count > 0) {
                WriteCfgArray("allowedLoadFileExtensions[]=", sb, DefaultConfig.DefaultServer.ServerConfig.AllowedLoadFileExtensions);
            }
            if (DefaultConfig.DefaultServer.ServerConfig.AllowedPreprocessFileExtensions.Count > 0)
            {
                WriteCfgArray("allowedPreprocessFileExtensions[]=", sb, DefaultConfig.DefaultServer.ServerConfig.AllowedPreprocessFileExtensions);
            }
            if (DefaultConfig.DefaultServer.ServerConfig.AllowedHTMLLoadExtensions.Count > 0)
            {
                WriteCfgArray("allowedHTMLLoadExtensions[]=", sb, DefaultConfig.DefaultServer.ServerConfig.AllowedHTMLLoadExtensions);
            }
            if (DefaultConfig.DefaultServer.ServerConfig.AllowedHTMLLoadURIs.Count > 0)
            {
                WriteCfgArray("allowedHTMLLoadURIs[]=", sb, DefaultConfig.DefaultServer.ServerConfig.AllowedHTMLLoadURIs);
            }
            
            
            

            sb.Append("upnp=").Append(DefaultConfig.DefaultServer.ServerConfig.UPNP.ToString().ToLower()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("steamProtocolMaxDataSize=").Append(DefaultConfig.DefaultServer.ServerConfig.SteamProtocolMaxDataSize.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("loopback=").Append(DefaultConfig.DefaultServer.ServerConfig.LoopBack.ToString().ToLower()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("disconnectTimeout=").Append(DefaultConfig.DefaultServer.ServerConfig.DisconnectTimeout.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("maxdesync=").Append(DefaultConfig.DefaultServer.ServerConfig.Maxdesync.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("maxping=").Append(DefaultConfig.DefaultServer.ServerConfig.MaxPing.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("maxpacketloss=").Append(DefaultConfig.DefaultServer.ServerConfig.MaxPacketLoss.ToString()).AppendLine(DefaultConfig.Semicolon);
            WriteMissions(sb);
            WriteMissionWhitelist(sb);
            if (DefaultConfig.DefaultServer.ServerConfig.AutoSelectMission) {
                sb.Append("autoSelectMission=").Append(DefaultConfig.DefaultServer.ServerConfig.AutoSelectMission.ToString()).AppendLine(DefaultConfig.Semicolon);
            }
            if (DefaultConfig.DefaultServer.ServerConfig.RandomMissionOrder)
            {
                sb.Append("randomMissionOrder=").Append(DefaultConfig.DefaultServer.ServerConfig.RandomMissionOrder.ToString()).AppendLine(DefaultConfig.Semicolon);
            }
            sb.Append("logFile=").Append(DefaultConfig.DoubleQuotes).Append(DefaultConfig.DefaultServer.ServerConfig.LogFile.ToString()).Append(DefaultConfig.DoubleQuotes).AppendLine(DefaultConfig.Semicolon);
            sb.Append("timeStampFormat=").Append(DefaultConfig.DoubleQuotes).Append(DefaultConfig.DefaultServer.ServerConfig.TimeStampFormat==0? "none": DefaultConfig.DefaultServer.ServerConfig.TimeStampFormat == 1? "short" : "full").Append(DefaultConfig.DoubleQuotes).AppendLine(DefaultConfig.Semicolon);
            sb.Append("callExtReportLimit=").Append(DefaultConfig.DefaultServer.ServerConfig.CallExtReportLimit.ToString()).AppendLine(DefaultConfig.Semicolon);
            try
            {
                sb.AppendLine(Encoding.Default.GetString(Convert.FromBase64String(DefaultConfig.DefaultServer.ServerConfig.ServerConfigArgs)));
            }
            catch
            {

            }


            try
            {
                File.WriteAllText(DefaultConfig.DefaultServer.ServerDir + @"\destiny_serverconfig\" + DefaultConfig.DefaultServer.ServerUUID + @"\server.cfg", sb.ToString(), UTF8);
            }
            catch(Exception e)
            {
                XtraMessageBox.Show("保存失败!"+ e.Message, "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            
            //写出basicconfig.cfg
            sb.Clear();
            sb.Append("MaxMsgSend=").Append(DefaultConfig.DefaultServer.BasicConfig.MaxMsgSend.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("MaxSizeGuaranteed=").Append(DefaultConfig.DefaultServer.BasicConfig.MaxSizeGuaranteed.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("MaxSizeNonguaranteed=").Append(DefaultConfig.DefaultServer.BasicConfig.MaxSizeNonguaranteed.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("MinBandwidth=").Append(DefaultConfig.DefaultServer.BasicConfig.MinBandwidth.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("MaxBandwidth=").Append(DefaultConfig.DefaultServer.BasicConfig.MaxBandwidth.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("MinErrorToSend=").Append(DefaultConfig.DefaultServer.BasicConfig.MinErrorToSend.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("MinErrorToSendNear=").Append(DefaultConfig.DefaultServer.BasicConfig.MinErrorToSendNear.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("MaxPacketSize=").Append(DefaultConfig.DefaultServer.BasicConfig.MaxPacketSize.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("MaxCustomFileSize=").Append(DefaultConfig.DefaultServer.BasicConfig.MaxCustomFileSize.ToString()).AppendLine(DefaultConfig.Semicolon);
            try
            {
                sb.AppendLine(Encoding.Default.GetString(Convert.FromBase64String(DefaultConfig.DefaultServer.BasicConfig.BasicConfigArgs)));
            }
            catch
            {

            }

            
            try
            {
                File.WriteAllText(DefaultConfig.DefaultServer.ServerDir + @"\destiny_serverconfig\" + DefaultConfig.DefaultServer.ServerUUID + @"\basic.cfg", sb.ToString(), UTF8);
            }
            catch (Exception e)
            {
                XtraMessageBox.Show("保存失败!" + e.Message, "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            //写出Server Profile 配置
            sb.Clear();
            sb.Append("difficulty=").Append(DefaultConfig.DoubleQuotes).Append("CustomDifficulty").Append(DefaultConfig.DoubleQuotes).AppendLine(DefaultConfig.Semicolon);
            sb.AppendLine("class DifficultyPresets");
            sb.AppendLine(DefaultConfig.LeftSquareBrackets);
            sb.Append(DefaultConfig.Tab).AppendLine("class CustomDifficulty");
            sb.Append(DefaultConfig.Tab).AppendLine(DefaultConfig.LeftSquareBrackets);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).AppendLine("class Options");
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).AppendLine(DefaultConfig.LeftSquareBrackets);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("groupIndicators=").Append(DefaultConfig.DefaultServer.serverProfile.GroupIndicators.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("friendlyTags=").Append(DefaultConfig.DefaultServer.serverProfile.FriendlyTags.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("enemyTags=").Append(DefaultConfig.DefaultServer.serverProfile.EnemyTags.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("detectedMines=").Append(DefaultConfig.DefaultServer.serverProfile.DetectedMines.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("commands=").Append(DefaultConfig.DefaultServer.serverProfile.Commands.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("waypoints=").Append(DefaultConfig.DefaultServer.serverProfile.WayPoints.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("tacticalPing=").Append(DefaultConfig.DefaultServer.serverProfile.TacticalPing.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("weaponInfo=").Append(DefaultConfig.DefaultServer.serverProfile.WeaponInfo.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("stanceIndicator=").Append(DefaultConfig.DefaultServer.serverProfile.StanceIndicator.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("staminaBar=").Append(DefaultConfig.DefaultServer.serverProfile.StaminaBar.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("weaponCrosshair=").Append(DefaultConfig.DefaultServer.serverProfile.WeaponCrosshair.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("visionAid=").Append(DefaultConfig.DefaultServer.serverProfile.VisionAid.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("thirdPersonView=").Append(DefaultConfig.DefaultServer.serverProfile.ThirdPersonView.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("cameraShake=").Append(DefaultConfig.DefaultServer.serverProfile.CameraShake.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("scoreTable=").Append(DefaultConfig.DefaultServer.serverProfile.ScoreTable.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("deathMessages=").Append(DefaultConfig.DefaultServer.serverProfile.DeathMessages.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("vonID=").Append(DefaultConfig.DefaultServer.serverProfile.VonID.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("mapContent=").Append(DefaultConfig.DefaultServer.serverProfile.MapContent.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("mapContentFriendly=").Append(DefaultConfig.DefaultServer.serverProfile.MapContentFriendly.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("mapContentEnemy=").Append(DefaultConfig.DefaultServer.serverProfile.MapContentEnemy.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("mapContentMines=").Append(DefaultConfig.DefaultServer.serverProfile.MapContentMines.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("reducedDamage=").Append(DefaultConfig.DefaultServer.serverProfile.ReducedDamage.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("autoReport=").Append(DefaultConfig.DefaultServer.serverProfile.AutoReport.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("multipleSaves=").Append(DefaultConfig.DefaultServer.serverProfile.MultipleSaves.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append(DefaultConfig.RightSquareBrackets).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("description=").Append(DefaultConfig.DoubleQuotes).Append("自定义难度设置 by Destiny Studio(娱乐至上专用启动器)").Append(DefaultConfig.DoubleQuotes).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("aiLevelPreset=3").AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.RightSquareBrackets).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).AppendLine("class CustomAILevel");
            sb.Append(DefaultConfig.Tab).AppendLine(DefaultConfig.LeftSquareBrackets);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("skillAI=").Append(DefaultConfig.DefaultServer.serverProfile.SkillAI).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("precisionAI=").Append(DefaultConfig.DefaultServer.serverProfile.PrecisionAI).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.Tab).Append(DefaultConfig.RightSquareBrackets).AppendLine(DefaultConfig.Semicolon);
            sb.Append(DefaultConfig.RightSquareBrackets).AppendLine(DefaultConfig.Semicolon);
            sb.Append("TerrainGrid=").Append(DefaultConfig.DefaultServer.BasicConfig.TerrainGrid.ToString()).AppendLine(DefaultConfig.Semicolon);
            sb.Append("ViewDistance=").Append(DefaultConfig.DefaultServer.BasicConfig.ViewDistance.ToString()).AppendLine(DefaultConfig.Semicolon);
            try
            {
                sb.AppendLine(Encoding.Default.GetString(Convert.FromBase64String(DefaultConfig.DefaultServer.serverProfile.ServerProfileArgs)));
            }
            catch { 

            }

            try
            {
                File.WriteAllText(DefaultConfig.DefaultServer.ServerDir + @"\destiny_serverconfig\" + DefaultConfig.DefaultServer.ServerUUID + @"\Users\" + DefaultConfig.DefaultServer.ServerUUID + @"\" + DefaultConfig.DefaultServer.ServerUUID + ".Arma3Profile", sb.ToString(), UTF8);
            }
            catch (Exception e)
            {
                XtraMessageBox.Show("保存失败!" + e.Message, "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
           
            //写出BEServer_x64.cfg
            sb.Clear();
            sb.Append("RConPassword").Append(" ").AppendLine(DefaultConfig.DefaultServer.BattlEyeConfig.RConPassword);
            sb.Append("RConPort").Append(" ").AppendLine(DefaultConfig.DefaultServer.BattlEyeConfig.RConPort.ToString());
            //sb.Append("MaxPing").Append(" ").AppendLine(DefaultConfig.DefaultServer.BattlEyeConfig.MaxPing.ToString());
            if (DefaultConfig.DefaultServer.BattlEyeConfig.MaxCreateVehiclePerInterval.MaxNumbe != 0 && DefaultConfig.DefaultServer.BattlEyeConfig.MaxCreateVehiclePerInterval.Seconds != 0) {
                sb.Append("MaxCreateVehiclePerInterval").Append(" ").Append(DefaultConfig.DefaultServer.BattlEyeConfig.MaxCreateVehiclePerInterval.MaxNumbe).Append(" ").AppendLine(DefaultConfig.DefaultServer.BattlEyeConfig.MaxCreateVehiclePerInterval.Seconds.ToString());
            }
            if (DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetPosPerInterval.MaxNumbe != 0 && DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetPosPerInterval.Seconds != 0)
            {
                sb.Append("MaxSetPosPerInterval").Append(" ").Append(DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetPosPerInterval.MaxNumbe).Append(" ").AppendLine(DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetPosPerInterval.Seconds.ToString());
            }
            if (DefaultConfig.DefaultServer.BattlEyeConfig.MaxDeleteVehiclePerInterval.MaxNumbe != 0 && DefaultConfig.DefaultServer.BattlEyeConfig.MaxDeleteVehiclePerInterval.Seconds != 0)
            {
                sb.Append("MaxDeleteVehiclePerInterval").Append(" ").Append(DefaultConfig.DefaultServer.BattlEyeConfig.MaxDeleteVehiclePerInterval.MaxNumbe).Append(" ").AppendLine(DefaultConfig.DefaultServer.BattlEyeConfig.MaxDeleteVehiclePerInterval.Seconds.ToString());
            }
            if (DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetDamagePerInterval.MaxNumbe != 0 && DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetDamagePerInterval.Seconds != 0)
            {
                sb.Append("MaxSetDamagePerInterval").Append(" ").Append(DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetDamagePerInterval.MaxNumbe).Append(" ").AppendLine(DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetDamagePerInterval.Seconds.ToString());
            }
            if (DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddBackpackCargoPerInterval.MaxNumbe != 0 && DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddBackpackCargoPerInterval.Seconds != 0)
            {
                sb.Append("MaxAddBackpackCargoPerInterval").Append(" ").Append(DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddBackpackCargoPerInterval.MaxNumbe).Append(" ").AppendLine(DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddBackpackCargoPerInterval.Seconds.ToString());
            }
            if (DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddMagazineCargoPerInterval.MaxNumbe != 0 && DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddMagazineCargoPerInterval.Seconds != 0)
            {
                sb.Append("MaxAddMagazineCargoPerInterval").Append(" ").Append(DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddMagazineCargoPerInterval.MaxNumbe).Append(" ").AppendLine(DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddMagazineCargoPerInterval.Seconds.ToString());
            }
            if (DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddWeaponCargoPerInterval.MaxNumbe != 0 && DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddWeaponCargoPerInterval.Seconds != 0)
            {
                sb.Append("MaxAddWeaponCargoPerInterval").Append(" ").Append(DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddWeaponCargoPerInterval.MaxNumbe).Append(" ").AppendLine(DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddWeaponCargoPerInterval.Seconds.ToString());
            }
            

            try
            {
               File.WriteAllText(DefaultConfig.DefaultServer.ServerDir + @"\destiny_serverconfig\" + DefaultConfig.DefaultServer.ServerUUID + @"\BattlEye" + @"\BEServer_x64.cfg", sb.ToString(), UTF8);
               File.WriteAllText(DefaultConfig.DefaultServer.ServerDir + @"\destiny_serverconfig\" + DefaultConfig.DefaultServer.ServerUUID + @"\BattlEye" + @"\BEServer.cfg", sb.ToString(), UTF8);
            }
            catch (Exception e)
            {
                XtraMessageBox.Show("保存失败!" + e.Message, "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }




            return true;
        }

        public static void WriteMissions(StringBuilder sb) {
            if (DefaultConfig.DefaultServer.ServerConfig.missions.Count < 1) {
                return;
            }
            bool wClass = false;
            for (int i = 0; i < DefaultConfig.DefaultServer.ServerConfig.missions.Count; i++) {
                if (DefaultConfig.DefaultServer.ServerConfig.missions[i].Choose) {
                    if (!wClass) {
                        sb.Append("class Missions ").AppendLine(DefaultConfig.LeftSquareBrackets);
                        wClass = true;
                    }
                    sb.Append(DefaultConfig.Tab).Append("class ").Append("Destiny_studio_").Append((i + 1).ToString()).AppendLine(DefaultConfig.LeftSquareBrackets);
                    sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("template = ").Append(DefaultConfig.DoubleQuotes).Append(DefaultConfig.DefaultServer.ServerConfig.missions[i].Template.Replace(".pbo", "")).Append(DefaultConfig.DoubleQuotes).AppendLine(DefaultConfig.Semicolon);
                    sb.Append(DefaultConfig.Tab).Append(DefaultConfig.Tab).Append("difficulty = ").Append(DefaultConfig.DoubleQuotes).Append(MissionsTool.intToDifficulty(DefaultConfig.DefaultServer.ServerConfig.missions[i].Difficulty)).Append(DefaultConfig.DoubleQuotes).AppendLine(DefaultConfig.Semicolon);
                    sb.Append(DefaultConfig.Tab).Append(DefaultConfig.RightSquareBrackets).AppendLine(DefaultConfig.Semicolon);
                }
            }
            if (wClass)
            {
                sb.Append(DefaultConfig.RightSquareBrackets).AppendLine(DefaultConfig.Semicolon);
            }
        }

        public static void WriteMissionWhitelist(StringBuilder sb)
        {
            StringBuilder sb2 = new StringBuilder();
            sb2.Append("missionWhitelist[] = ").AppendLine(DefaultConfig.LeftSquareBrackets);
            bool wComma = false;
            for (int i = 0; i < DefaultConfig.DefaultServer.ServerConfig.missions.Count; i++)
            {
                if (DefaultConfig.DefaultServer.ServerConfig.missions[i].WhiteList)
                {
                    sb2.Append(DefaultConfig.Tab).Append(DefaultConfig.DoubleQuotes).Append(DefaultConfig.DefaultServer.ServerConfig.missions[i].Template.Replace(".pbo","")).Append(DefaultConfig.DoubleQuotes).AppendLine(DefaultConfig.Comma);
                    wComma = true;
                }
            }
            string strComma = "";
            if (wComma) {
                int lastComma = sb2.ToString().LastIndexOf(',');
                strComma = sb2.ToString().Remove(lastComma, 1);
                sb.Append(strComma).Append(DefaultConfig.RightSquareBrackets).AppendLine(DefaultConfig.Semicolon);
            }
        }


        public static void WriteCfgEvent(string key, StringBuilder sb,string text) {
            try
            {
                text = Encoding.Default.GetString(Convert.FromBase64String(text));
                text = text.Replace(DefaultConfig.DoubleQuotes, "'");
            }
            catch 
            {
                text = string.Empty;
            }
            sb.Append(key).Append(DefaultConfig.DoubleQuotes).Append(text).Append(DefaultConfig.DoubleQuotes).AppendLine(DefaultConfig.Semicolon);
        }
        public static void WriteCfgArray(string key,StringBuilder sb,List<string> strList) {
            sb.Append(key).AppendLine(DefaultConfig.LeftSquareBrackets);
            for (int i = 0;i< strList.Count;i++) {
                if (!string.IsNullOrEmpty(strList[i])) {
                    sb.Append(DefaultConfig.Tab).Append(DefaultConfig.DoubleQuotes).Append(strList[i]).Append(DefaultConfig.DoubleQuotes).AppendLine((i == (strList.Count - 1)) ? "" : DefaultConfig.Comma);
                }
            }
            sb.Append("}").AppendLine(DefaultConfig.Semicolon);
        }

        public static bool UsePort(int port) {
            var activeUdpListeners = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners();
            return activeUdpListeners.Any(p => p.Port == port);
        }
    }
}
