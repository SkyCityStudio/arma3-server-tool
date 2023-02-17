using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace a3.Entity
{
    public class ArmaServerConfig {

        public ServerManagement ServerTaskManagement { get; set; } = new ServerManagement();
        public ServerConfig ServerConfig { get; set; } = new ServerConfig();
        public StartupParameters StartupParameters { get; set; } = new StartupParameters();
        public ServerBasic BasicConfig { get; set; } = new ServerBasic();
        public BattlEye BattlEyeConfig { get; set; } = new BattlEye();
        public ServerProfile serverProfile { get; set; } = new ServerProfile();
        public string SaveTime { get; set; } = "上次保存于:" + DateTime.Now.ToLocalTime().ToString();
        public string ServerDir { get; set; } = null;

        public bool AutoCopyBikey { get; set; } = true;

        public string StartCommandLine { get; set; } = null;

        public bool x64 { get; set; } = true;
        public string ServerUUID { get; set; } = null;
        public string ConfigName { get; set; } = null;

        public string CreateTime { get; set; } = null;

        public void SetTime() {
            string time = "上次保存于:" + DateTime.Now.ToLocalTime().ToString();
            FluentDesignForm.SaveInfoTip.Caption = time;
            SaveTime = time;
            //System.Diagnostics.Process.GetProcessById
        }
    }


    public class ServerManagement {
        public int ProcessById { get; set; } = 0;

        public bool EnableMonitor { get; set; } = false;

        public bool EnableMonitoringService { get; set; } = false;

        public int RestartTime { get; set; } = 0;

        public string RestartInfo { get; set; } = "服务器重启马上就好!";

        public int RestartLastTime { get; set; } = 60;




        public Dictionary<string, CronEntity>  CronEntity { get; set; } = new Dictionary<string, CronEntity>();
    }




    //服务器配置文件
    public class ServerConfig
    {
        //服务器昵称 hostname
        public string HostName { get; set; } = "娱乐至上测试服务器";

        //服务器密码 password
        public string Password { get; set; } = String.Empty;

        //服务器人数 maxPlayers
        public int MaxPlayers { get; set; } = 10;

        //任务持久化 persistent
        public bool Persistent { get; set; } = false;

        //跳过大厅 skipLobby
        public bool SkipLobby { get; set; } = false;

        //允许玩家绘制地图 drawingInMap
        public bool DrawingInMap { get; set; } = true;

        //允许使用服务器的Arma 3 分析 statisticsEnabled  0禁用，1启用
        public int StatisticsEnabled { get; set; } = 0;

        //强制执行高级飞行模型 0（由玩家决定）。  forceRotorLibSimulation 1 - 强制 AFM，2 - 强制 SFM。
        public int ForceRotorLibSimulation { get; set; } = 0;

        //强制任务难度 forcedDifficulty
        public string ForcedDifficulty { get; set; } = "none";
        

        //motd 欢迎信息 motd
        public List<string> Motd { get; set; } = new List<string>() { "欢迎进入本服务器！", "欢迎进入本服务器！" };

        //motd 消息间隔  motdInterval
        public int MotdInterval { get; set; } = 1;

        //禁用语音  disableVoN  0 =启用
        public int DisableVoN { get; set; } = 0;

        //语音质量 vonCodecQuality
        public int VonCodecQuality { get; set; } = 30;

        //语音编码器 vonCodec  0=SPEEX 1=OPUS
        public int VonCodec { get; set; } = 1;

        //无头客户端地址 headlessClients
        public List<string> HeadlessClients { get; set; } = new List<string>() { "127.0.0.1" };

        //本地客户端地址   localClient
        public List<string> LocalClient { get; set; } = new List<string>() { "127.0.0.1" };

        //投票百分比设置 voteThreshold     默认不写配置
        public int VoteThreshold { get; set; } = 0;

        //游戏投票超时 votingTimeOut    默认不写配置
        public int VotingTimeOut { get; set; } = 0;

        //角色选择超时 roleTimeOut
        public int RoleTimeOut { get; set; } = 99999;

        //任务简报超时 briefingTimeOut
        public int BriefingTimeOut { get; set; } = 60;

        //任务汇报超时 debriefingTimeOut
        public int DebriefingTimeOut { get; set; } = 45;

        //大厅选择超时 lobbyIdleTimeout
        public int LobbyIdleTimeout { get; set; } = 99999;

        //玩家连接投票 voteMissionPlayers     默认不写配置
        public int VoteMissionPlayers { get; set; } = 0;

        //启用Battleye反作弊 BattlEye
        public bool BattlEye { get; set; } = true;

        //模组签名验证 verifySignatures 2=验证 0=禁用
        public bool VerifySignatures { get; set; } = true;

        //踢出重复玩家ID kickduplicate
        public int Kickduplicate { get; set; } = 0;

        //文件修补类型 allowedFilePatching
        public int AllowedFilePatching { get; set; } = 0;

        //允许文件修补的UID filePatchingExceptions    默认不写配置
        public List<string> FilePatchingExceptions { get; set; } = new List<string>();

        //服务器命令密码 serverCommandPassword
        public string ServerCommandPassword { get; set; } = System.Guid.NewGuid().ToString("N");

        //服务器管理员密码  passwordAdmin
        public string PasswordAdmin { get; set; } = System.Guid.NewGuid().ToString("N");

        //管理员UID白名单 admins
        public List<string> Admins { get; set; } = new List<string>();

        //当玩家检测到相同ID后执行 doubleIdDetected
        public string DoubleIdDetected { get; set; } = String.Empty;

        //当玩家连接到服务器后执行  onUserConnected 
        public string onUserConnected { get; set; } = String.Empty;

        //当玩家断开服务器后执行 onUserDisconnected
        public string onUserDisconnected { get; set; } = String.Empty;

        //检测到签名文件被修改后执行 onHackedData 
        public string onHackedData { get; set; } = "a2ljayAoX3RoaXMgc2VsZWN0IDAp";

        //检测到签名文件与服务器不一致 onDifferentData 
        public string onDifferentData { get; set; } = "a2ljayAoX3RoaXMgc2VsZWN0IDAp";

        //检测到未签名数据时执行 onUnsignedData
        public string onUnsignedData { get; set; } = "a2ljayAoX3RoaXMgc2VsZWN0IDAp";

        //当玩家被踢出服务器后执行  onUserKicked
        public string onUserKicked { get; set; } = String.Empty;

        //定期检查脚本    regularCheck
        public string RegularCheck { get; set; } = String.Empty;

        //loadFile命令白名单  allowedLoadFileExtensions
        public List<string> AllowedLoadFileExtensions { get; set; } = new List<string> {
             "hpp",
             "sqs",
             "sqf",
             "fsm",
             "cpp",
             "paa",
             "txt",
             "xml",
             "inc",
             "ext",
             "sqm",
             "ods",
             "fxy",
             "lip",
             "csv",
             "kb",
             "bik",
             "bikb",
             "html",
             "htm",
             "biedi"
        };

        //preprocessFile / preprocessFileLineNumbers 命令扩展文件白名单  allowedPreprocessFileExtensions
        public List<string> AllowedPreprocessFileExtensions { get; set; } = new List<string> {
             "hpp",
             "sqs",
             "sqf",
             "fsm",
             "cpp",
             "paa",
             "txt",
             "xml",
             "inc",
             "ext",
             "sqm",
             "ods",
             "fxy",
             "lip",
             "csv",
             "kb",
             "bik",
             "bikb",
             "html",
             "htm",
             "biedi"
        };

        //htmlLoad 命令扩展文件白名单: allowedHTMLLoadExtensions
        public List<string> AllowedHTMLLoadExtensions { get; set; } = new List<string> {
             "html",
             "txt",
             "xml"
        };

        //htmlLoad URL白名单   allowedHTMLLoadURIs
        public List<string> AllowedHTMLLoadURIs { get; set; } = new List<string> {
             "https://arma3.com",
             "https://it.o.ls",
             "https://*.arma3bbs.com"
        };


        //UPNP模式 upnp
        public bool UPNP { get; set; } = false;

        //Steam查询包大小 steamProtocolMaxDataSize 
        public int SteamProtocolMaxDataSize { get; set; } = 1024;

        //Lan模式 loopback
        public bool LoopBack { get; set; } = false;

        //断开重连等待时间: disconnectTimeout
        public int DisconnectTimeout { get; set; } = 10;

        //高于此不同步踢出: maxdesync 
        public int Maxdesync { get; set; } = 150;

        //高于此延迟时踢出: maxping
        public int MaxPing { get; set; } = 300;

        //高于此丢包时踢出: maxpacketloss
        public int MaxPacketLoss { get; set; } = 50;

        //任务列表 missionWhitelist任务白名单
        public List<MissionsEntity> missions { get; set; } = new List<MissionsEntity>();

        //服务器自动选择循环 autoSelectMission   默认不写
        public bool AutoSelectMission { get; set; } = false;

        //随机遍历任务列表 randomMissionOrder   默认不写
        public bool RandomMissionOrder { get; set; } = false;

        //控制台日志文件名 logFile
        public string LogFile { get; set; } = "server_console.log";

        //时间戳格式 timeStampFormat 值是“none”、“short”、“full”。
        public int TimeStampFormat { get; set; } = 1;

        //callExtension时间限制
        public int CallExtReportLimit { get; set; } = 1000;

        //server.cfg 附加参数
        public string ServerConfigArgs { get; set; } = String.Empty;
    }

    //Server Profile 配置
    public class ServerProfile {

        public string ServerProfileArgs { get; set; } = "c2luZ2xlVm9pY2U9MDsNCm1heFNhbXBsZXNQbGF5ZWQ9OTY7DQpiYXR0bGV5ZUxpY2Vuc2U9MTsNCnNjZW5lQ29tcGxleGl0eT0xMDAwMDAwOw0Kc2hhZG93WkRpc3RhbmNlPTA7DQpwcmVmZXJyZWRPYmplY3RWaWV3RGlzdGFuY2U9OTAwOw0KcGlwVmlld0Rpc3RhbmNlPTUwMDsNCnZvbHVtZUNEPTEwOw0Kdm9sdW1lRlg9MTA7DQp2b2x1bWVTcGVlY2g9MTA7DQp2b2x1bWVWb049MTA7DQp2b25SZWNUaHJlc2hvbGQ9MC4wMjk5OTk5OTk7DQp2b2x1bWVNYXBEdWNraW5nPTE7";
        //小队指示标识: groupIndicators
        public int GroupIndicators { get; set; } = 1;

        //友军名称标签: friendlyTags
        public int FriendlyTags { get; set; } = 1;

        //敌人名称标签: enemyTags
        public int EnemyTags { get; set; } = 0;

        //检测地雷范围: detectedMines
        public int DetectedMines { get; set; } = 0;

        //显示命令图标: commands
        public int Commands { get; set; } = 1;

        //显示航点: waypoints
        public int WayPoints { get; set; }= 1;

        //战术 Ping: tacticalPing
        public int TacticalPing { get; set; } = 1;



        //显示武器信息 weaponInfo
        public int WeaponInfo { get; set; } = 2;

        //姿态指示: stanceIndicator
        public int StanceIndicator { get; set; } = 2;

        //显示耐力条: staminaBar
        public int StaminaBar { get; set; } = 1;

        //显示武器十字准线: weaponCrosshair
        public int WeaponCrosshair { get; set; } = 1;

        //准星视觉辅助: visionAid
        public int VisionAid { get; set; } = 1;


        //第三人称视角: thirdPersonView
        public int ThirdPersonView { get; set; } = 1;

        //相机摇晃: cameraShake
        public int CameraShake { get; set; } = 1;



        //启用得分表: scoreTable
        public int ScoreTable { get; set; } = 1;

        //显示死亡消息: deathMessages
        public int DeathMessages { get; set; } = 1;

        //显示谁在通信讲话: vonID
        public int VonID { get; set; } = 1;



        //扩展地图内容: mapContent
        public int MapContent { get; set; } = 1;

        //显示友方单位: mapContentFriendly
        public int MapContentFriendly { get; set; } = 1;

        //显示敌方单位: mapContentEnemy
        public int MapContentEnemy { get; set; } = 1;

        //显示检测到的地雷: mapContentMines
        public int MapContentMines { get; set; } = 1;


        //减少对玩家及其团队成员造成的伤害。     reducedDamage
        public int ReducedDamage { get; set; } = 1;

        //自动报告 autoReport
        public int AutoReport { get; set; } = 1;

        //多次保存 multipleSaves
        public int MultipleSaves { get; set; } = 1;


        //AI 水平 skillAI
        public double SkillAI { get; set; } = 0.5;

        //AI精准度 precisionAI
        public double PrecisionAI { get; set; } = 0.5;










    }

    //battleye 配置
    public class BattlEye
    {
        //beRcon密码
        public string RConPassword { get; set; } = "destiny" + new Random().Next(9999).ToString();

        //beRcon端口
        public int RConPort { get; set; } = 2310;

        //最大延迟
        public int MaxPing { get; set; } = 500;

        //限制创建对象调用
        public BEServerCfgEntity MaxCreateVehiclePerInterval { get; set; } = new BEServerCfgEntity();

        //限制传送调用
        public BEServerCfgEntity MaxSetPosPerInterval { get; set; } = new BEServerCfgEntity();

        //限制删除调用
        public BEServerCfgEntity MaxDeleteVehiclePerInterval { get; set; } = new BEServerCfgEntity();

        //限制设置伤害调用
        public BEServerCfgEntity MaxSetDamagePerInterval { get; set; } = new BEServerCfgEntity();

        //限制生成背包调用
        public BEServerCfgEntity MaxAddBackpackCargoPerInterval { get; set; } = new BEServerCfgEntity();

        //限制生成杂项和物品调用
        public BEServerCfgEntity MaxAddMagazineCargoPerInterval { get; set; } = new BEServerCfgEntity();

        //限制生成武器调用
        public BEServerCfgEntity MaxAddWeaponCargoPerInterval { get; set; } = new BEServerCfgEntity();




    }

    //basi 配置
    public class ServerBasic
    {
        //服务器每帧最大数据包
        public int MaxMsgSend { get; set; } = 128;

        //服务器保证数据包的最大大小
        public int MaxSizeGuaranteed { get; set; } = 512;

        //服务器非保证数据包的最大大小
        public int MaxSizeNonguaranteed { get; set; } = 256;

        //服务器最小带宽
        public long MinBandwidth { get; set; } = 256;

        //服务器最大带宽
        public long MaxBandwidth { get; set; } = 1048576000;

        //服务器发送更新的最小错误
        public double MinErrorToSend { get; set; } = 0.001;

        //为附近单位发送更新的最小错误:
        public double MinErrorToSendNear { get; set; } = 0.001;

        //发送数据包最大大小 maxPacketSize
        public int MaxPacketSize { get; set; } = 1400;

        //最大自定义文件大小:
        public int MaxCustomFileSize { get; set; } = 1024;

        //basic.cfg 附加参数
        public string BasicConfigArgs { get; set; } = "bGFuZ3VhZ2U9IkVuZ2xpc2giOw0KTWF4UGVybWFuZW50Q3JhdGVycz01MDsNCmFkYXB0ZXI9LTE7DQozRF9QZXJmb3JtYW5jZT0xLjAwMDAwMDsNClJlc29sdXRpb25fVz04MDA7DQpSZXNvbHV0aW9uX0g9NjAwOw0KUmVzb2x1dGlvbl9CcHA9MzI7";

        //地形网格视距 terrainGrid
        public int TerrainGrid { get; set; } = 30;

        //单位物体视距 viewDistance
        public int ViewDistance { get; set; } = 1600;


    }

    //启动参数
    public class StartupParameters 
    {
        //自动初始化 -autoInit
        public bool AutoInit { get; set; } = false;

        //允许的文件修补类型 -filePatching
        public bool FilePatching { get; set; } = false;

        //PID文件名: -pid=
        public string PidFile { get; set; } = String.Empty;

        //排名文件名 -ranking=
        public string Ranking { get; set; } = String.Empty;

        //服务器端口 -port
        public int Port { get; set; } = 2302;

        //使用新的实验性网络算法 -bandwidthAlg=2
        public bool BandwidthAlg { get; set; } = false;

        //mod配置
        public List<ModsEntity> modsEntities { get; set; } = new List<ModsEntity>();



        //启用HT: -enableHT
        public bool EnableHT { get; set; } = true;

        //启用巨大内存页: -hugepages
        public bool Hugepages { get; set; } = false;

        //加载任务到内存 -loadMissionToMemory
        public bool LoadMissionToMemory { get; set; } = true;

        //禁用服务器超线程 -disableServerThread
        public bool DisableServerThread { get; set; } = false;

        //设置CPU核心数 -cpuCount
        public int CpuCount { get; set; } = 0;

        //设置线程数 -exThreads
        public int ExThreads { get; set; } = 0;

        //限制内存分配 -maxMem
        public int MaxMem { get; set; } = 0;

        //服务器最大FPS limitFPS
        public int LimitFPS { get; set; } = 1000;


        //禁用日志 -noLogs
        public bool NoLogs { get; set; } = false;

        //启用多人网络流量记录: -netlog
        public bool Netlog { get; set; } = false;

        //启动附加参数
        public string StartConfigArgs { get; set; } = String.Empty;

        public bool DLCWS { get; set; } = false;
        public bool DLCVN { get; set; } = false;
        public bool DLCCSLA { get; set; } = false;
        public bool DLCGM { get; set; } = false;
        public bool DLCcontact { get; set; } = false;
    }
}
