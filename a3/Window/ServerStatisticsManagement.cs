
using a3.Config;
using a3.Dialog;
using a3.Entity;
using a3.TaskJob;
using a3.Tools;
using BytexDigital.BattlEye.Rcon;
using BytexDigital.BattlEye.Rcon.Commands;
using BytexDigital.BattlEye.Rcon.Domain;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace a3.Window
{
    public partial class ServerStatisticsManagement : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        // Console.WriteLine();
        // Console.WriteLine();
        public CriteriaOperator op;
        public int serverId = 4;
        public ArmaServerConfig serverConfig;
        StartManagementTools tools = null;

        public SQLiteConnectionParameters SQLite = new SQLiteConnectionParameters();


        //Timer不要声明成局部变量，否则会被GC回收
        
        private DataTable dataTable = new DataTable();
        private DataTable BansDataTable = new DataTable();
        private DataTable PlayersDataTable = new DataTable();
        private RconClient networkClient = null;
        private StringBuilder htmlMsg = new StringBuilder();



        private DataTable TaskCronDataTable = new DataTable();
        public ServerStatisticsManagement()
        {
            InitializeComponent();
            dataTable.Columns.Add("ID", typeof(string));
            dataTable.Columns.Add("名称", typeof(string));
            dataTable.Columns.Add("已验证", typeof(string));
            dataTable.Columns.Add("在大厅", typeof(string));
            dataTable.Columns.Add("IP", typeof(string));
            dataTable.Columns.Add("GUID", typeof(string));
            dataTable.Columns.Add("Ping", typeof(string));
            gridControl1.DataSource = dataTable;
            gridView1.OptionsMenu.ShowAutoFilterRowItem = false;
            gridView1.OptionsMenu.ShowConditionalFormattingItem = true;
            gridView1.OptionsMenu.ShowGroupSummaryEditorItem = true;
            BansDataTable.Columns.Add("ID", typeof(string));
            BansDataTable.Columns.Add("GUID", typeof(string));
            BansDataTable.Columns.Add("IP", typeof(string));
            BansDataTable.Columns.Add("剩余时间", typeof(string));
            BansDataTable.Columns.Add("是否永久", typeof(string));
            BansDataTable.Columns.Add("理由", typeof(string));
            BansDataTable.Columns.Add("封IP", typeof(string));
            BansDataTable.Columns.Add("封GUID", typeof(string));
            gridControl2.DataSource = BansDataTable;
            gridView2.OptionsMenu.ShowAutoFilterRowItem = false;
            gridView2.OptionsMenu.ShowConditionalFormattingItem = true;
            gridView2.OptionsMenu.ShowGroupSummaryEditorItem = true;
            PlayersDataTable.Columns.Add("ID", typeof(string));
            PlayersDataTable.Columns.Add("GUID", typeof(string));
            PlayersDataTable.Columns.Add("玩家名称", typeof(string));
            PlayersDataTable.Columns.Add("IP", typeof(string));
            PlayersDataTable.Columns.Add("更新时间", typeof(string));
            gridControl3.DataSource = PlayersDataTable;

            TaskCronDataTable.Columns.Add("任务ID", typeof(string));
            TaskCronDataTable.Columns.Add("服务器UUID", typeof(string));
            TaskCronDataTable.Columns.Add("服务器昵称", typeof(string));
            TaskCronDataTable.Columns.Add("CRON表达式", typeof(string));
            TaskCronDataTable.Columns.Add("操作类型", typeof(string));
            TaskCronDataTable.Columns.Add("备注", typeof(string));
            TaskCronDataTable.Columns.Add("状态", typeof(string));
            //gridControl5.DataSource = TaskCronDataTable;

            tabNavigationPage1.PageVisible = false;
            tabNavigationPage2.PageVisible = false;
            tabNavigationPage3.PageVisible = false;
            tabNavigationPage4.PageVisible = false;
            tabNavigationPage5.PageVisible = false;
            tabNavigationPage6.PageVisible = false;
            tabNavigationPage8.PageVisible = false;
            tabNavigationPage9.PageVisible = false;

            SqliteUtils.init();
            ProcessMonitoringTaskRun = true;
            ProcessMonitoringTask = Task.Factory.StartNew(() => ProcessMonitoring());



            

        }

        
        public Task ProcessMonitoringTask;
        public bool ProcessMonitoringTaskRun;
        public void ProcessMonitoring() {
            try
            {
                Process process = null;
                Series total = chartControl20.Series["Series 1"];
                Series UserProcessorTime = chartControl21.Series["进程运行代码耗时"];
                Series PrivilegedProcessorTime = chartControl21.Series["内核运行代码耗时"];
                Series TotalProcessorTime = chartControl21.Series["使用CPU总耗时"];
                UserProcessorTime.ValueScaleType = ScaleType.TimeSpan;
                PrivilegedProcessorTime.ValueScaleType = ScaleType.TimeSpan;
                TotalProcessorTime.ValueScaleType = ScaleType.TimeSpan;
                while (ProcessMonitoringTaskRun)
                {
                    Thread.Sleep(1000);
                    if (toggleSwitch2.IsOn) {
                        try
                        {
                            if (process == null && DefaultConfig.ServerList.ContainsKey(serverConfig.ServerUUID))
                            {
                                process = Process.GetProcessById(DefaultConfig.ServerList[serverConfig.ServerUUID].ServerTaskManagement.ProcessById);
                            }
                            process.Refresh();
                            var cl = process.TotalProcessorTime;
                            Thread.Sleep(1000);
                            double value = (process.TotalProcessorTime - cl).TotalMilliseconds / 1000 / Environment.ProcessorCount * 100;
                            this.Invoke(new Action(() =>
                            {
                                total.Points[0].Values[0] = (double)decimal.Round(decimal.Parse(value.ToString()), 2);
                                chartControl20.RefreshData();
                            }));

                            string dt = DateTime.Now.ToString("hh:mm:ss");
                            this.Invoke(new Action(() => {
                                UserProcessorTime.Points.Add(new SeriesPoint(dt, process.UserProcessorTime));
                                PrivilegedProcessorTime.Points.Add(new SeriesPoint(dt, process.PrivilegedProcessorTime));
                                TotalProcessorTime.Points.Add(new SeriesPoint(dt, process.TotalProcessorTime));
                                if (UserProcessorTime.Points.Count > 1000)
                                {
                                    UserProcessorTime.Points.RemoveRange(0, 1);
                                    PrivilegedProcessorTime.Points.RemoveRange(0, 1);
                                    TotalProcessorTime.Points.RemoveRange(0, 1);
                                }
                                chartControl21.RefreshData();
                            }));

                            this.Invoke(new Action(() => {
                                try
                                {
                                    labelControl14.Text = process.ProcessName;
                                    labelControl3.Text = ((process.WorkingSet64 / 1024) / 1024).ToString() + "MB";
                                    labelControl18.Text = ((process.VirtualMemorySize64 / 1024) / 1024).ToString() + "MB"; ;
                                    labelControl4.Text = process.BasePriority.ToString();
                                    labelControl5.Text = process.PriorityClass.ToString();
                                    labelControl6.Text = process.UserProcessorTime.Ticks.ToString() + "Ticks";
                                    labelControl7.Text = process.PrivilegedProcessorTime.Ticks.ToString() + "Ticks";
                                    labelControl8.Text = process.TotalProcessorTime.Ticks.ToString() + "Ticks";
                                    labelControl9.Text = ((process.PagedSystemMemorySize64 / 1024) / 1024).ToString() + "MB";
                                    labelControl10.Text = ((process.PagedMemorySize64 / 1024) / 1024).ToString() + "MB";
                                    labelControl17.Text = ((process.NonpagedSystemMemorySize64 / 1024) / 1024).ToString() + "MB";
                                    labelControl11.Text = process.Threads.Count.ToString();
                                    labelControl12.Text = process.Handle.ToString();
                                    labelControl13.Text = process.HandleCount.ToString();
                                    labelControl15.Text = process.MaxWorkingSet.ToString();
                                    labelControl16.Text = process.MinWorkingSet.ToString();
                                    labelControl19.Text = process.Id.ToString();
                                    labelControl20.Text = (DateTime.Now - process.StartTime).ToString();
                                }
                                catch { }
                            }));
                        }
                        catch { process = null; }
                    }

                }
                Console.WriteLine("CPU TASK clos");
            }
            catch { }

        }



        private System.Timers.Timer LoadPlayerTime;


        private void ServerStatisticsManagement_Load(object sender, EventArgs e)
        {
            LoadPlayerTime = new System.Timers.Timer(10000);
            LoadPlayerTime.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            LoadPlayerTime.AutoReset = true;
            LoadPlayerTime.Enabled = true;
        }

       
        private void ServerStatisticsManagement_FormClosing(object sender, FormClosingEventArgs e)
        {
            ProcessMonitoringTaskRun = false;
            LoadPlayerTime.Stop();
            LoadPlayerTime.EndInit();
            LoadPlayerTime.Dispose();
            Console.WriteLine("clos");
        }

        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                Task.Run(() => LoadPlayers());
            }
        }

        public void Init()
        {
            serverId = MonitoringServiceSqliteUtils.GetAndCreateServer(serverConfig.ServerUUID);
            tools = new StartManagementTools(serverConfig.ServerUUID);
            UserControl_Init();
            InitJob();
            LoadJobList();
        }
        private void barButtonItem44_ItemClick(object sender, ItemClickEventArgs e)
        {
            ReconnectRcon();
        }

        private int ReNum = 0;


        public void ReconnectRcon() {
            networkClient.ReconnectOnFailure = true;
            if (networkClient.Connect())
            {
                networkClient.Disconnected += NetworkClient_Disconnected;
                networkClient.MessageReceived += NetworkClient_MessageReceived;
                networkClient.PlayerConnected += NetworkClient_PlayerConnected;
                networkClient.PlayerDisconnected += NetworkClient_PlayerDisconnected;
                networkClient.PlayerRemoved += NetworkClient_PlayerRemoved;
                Task.Run(() => LoadPlayers());
                barButtonItem44.Enabled = false;
            }
            else {
                ReNum++;
                if (ReNum>=3) {
                    XtraMessageBox.Show("如果你始终连接不上，确保以下设置一定能连接上:\r\n1.已经启用了Battleye反作弊，必须启用才能用。\r\n2.确保rcon的端口正确，不能是游戏端口+4的范围内，最好是+8范围内，比如你游戏端口是2302，RCON端口最好就是2310 \r\n3.确保密码是在本工具内设置的且有效", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                }
                barButtonItem44.Enabled = true;
                SetStatusText("连接 BattleyrRcon 失败", "连接 BattleyrRcon 失败", "连接 BattleyrRcon 失败", "连接 BattleyrRcon 失败 " + DateTime.Now.ToString());
            }
        }

        private void NetworkClient_Disconnected(object sender, EventArgs e)
        {
            barButtonItem44.Enabled = false;
        }

        public void UserControl_Init()
        {
            MonitoringServiceSqliteUtils.init();
            MonitoringServiceSqliteUtils.InitPlayerOnlineInfo(serverConfig.ServerUUID);
            ServerStatisticsManagement_Leave(null, null);

            networkClient = new RconClient("127.0.0.1", serverConfig.BattlEyeConfig.RConPort, serverConfig.BattlEyeConfig.RConPassword);
            


            SQLite.FileName = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "destiny_statistics.db";

            try
            {
                sqlDataSource1.Fill();
                sqlDataSource2.Fill();
                sqlDataSource3.Fill();
                sqlDataSource4.Fill();
            }
            catch {
                XtraMessageBox.Show("服务器物体监控统计服务数据库 destiny_statistics.db 载入失败，你可以重试，任然不行可以自行备份，然后请手动删除该文件尝试恢复!", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

            }

            toggleSwitch4.IsOn = serverConfig.ServerTaskManagement.EnableMonitor;
            toggleSwitch3.IsOn = serverConfig.ServerTaskManagement.EnableMonitoringService;
            toggleSwitch1.IsOn = serverConfig.ServerTaskManagement.RestartTime > 0;
            spinEdit1.Value = serverConfig.ServerTaskManagement.RestartTime;
            textEdit2.Text = serverConfig.ServerTaskManagement.RestartInfo;
            spinEdit2.Value = serverConfig.ServerTaskManagement.RestartLastTime;

           
        }


        private void NetworkClient_PlayerRemoved(object sender, BytexDigital.BattlEye.Rcon.Events.PlayerRemovedArgs e)
        {
            Console.WriteLine(11);
            htmlMsg.AppendLine("<font color=\"white \">玩家 " + e.Name + "[id:" + e.Id + "] [guid:" + e.Guid + "] " + (e.IsBan ? "被封禁，原因是:" : "被踢出，原因是:") + e.Reason + "</font><br/>");
            this.Invoke(new Action(() => {
                richEditControl1.HtmlText = htmlMsg.ToString();
            }));
        }

        private void NetworkClient_MessageReceived(object sender, string e)
        {
            Console.WriteLine(22);
            htmlMsg.AppendLine("<font color=\"green \">" + e + "</font><br/>");
            this.Invoke(new Action(() => {
                richEditControl1.HtmlText = htmlMsg.ToString();
            }));

        }

        private void NetworkClient_PlayerDisconnected(object sender, BytexDigital.BattlEye.Rcon.Events.PlayerDisconnectedArgs e)
        {
            Console.WriteLine(33);
            htmlMsg.AppendLine("<font color=\"white \">玩家 " + e.Name + "[id:" + e.Id + "] 与服务器断开连接.</font><br/>");
            this.Invoke(new Action(() => {
                richEditControl1.HtmlText = htmlMsg.ToString();
            }));
        }

        private void NetworkClient_PlayerConnected(object sender, BytexDigital.BattlEye.Rcon.Events.PlayerConnectedArgs e)
        {
            Console.WriteLine(44);
            htmlMsg.AppendLine("<font color=\"white \">玩家 " + e.Name + "[id:" + e.Id + "] [guid:" + e.Guid + "] 连接到服务器.</font><br/>");
            this.Invoke(new Action(() => {
                richEditControl1.HtmlText = htmlMsg.ToString();
            }));
        }



        public void LoadPlayers()
        {
            try
            {
                bool requestSuccess = networkClient.Fetch(
                   command: new GetPlayersRequest(),
                   timeout: 5000,
                   result: out List<Player> onlinePlayers);

                if (onlinePlayers == null) {
                    return;
                }
                this.Invoke(new Action(() => {
                    dataTable.Rows.Clear();
                }));

                if (requestSuccess)
                {
                    this.Invoke(new Action(() => {
                        try
                        {
                            foreach (var player in onlinePlayers)
                            {
                                dataTable.Rows.Add(player.Id.ToString(), player.Name, player.IsVerified ? "是" : "否", player.IsInLobby ? "是" : "否", player.RemoteEndpoint.Address.ToString(), player.Guid, player.Ping.ToString());
                            }
                        }
                        catch
                        {

                        }

                    }));
                    gridControl1.DataSource = dataTable;
                    gridControl1.ForceInitialize();
                    if (dataTable.Rows.Count != 0)
                    {
                        this.Invoke(new Action(() => {
                            for (int i = 0; i < 7; i++)
                            {
                                gridView1.Columns[i].OptionsColumn.AllowEdit = false;
                            }
                        }));
                    }


                    foreach (var player in onlinePlayers)
                    {
                        int count = SqliteUtils.QueryPlayerCount(player.Guid);
                        if (count <= 0)
                        {
                            SqliteUtils.InsertPlayerData(player.Guid, player.Name, player.RemoteEndpoint.Address.ToString(), DateTime.Now.ToString());
                        }
                        else
                        {
                            SqliteUtils.UpdatePlayerData(player.Guid, player.Name, player.RemoteEndpoint.Address.ToString(), DateTime.Now.ToString());
                        }

                    }

                    StatusTextUpdate(onlinePlayers.Count, DateTime.Now.ToString());
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public async void InitJob()
        {
            foreach (KeyValuePair<string, CronEntity> item in serverConfig.ServerTaskManagement.CronEntity)
            {
                try
                {
                    JobKey jk = JobKey.Create(item.Value.TaskId, serverConfig.ServerUUID);
                    IJobDetail iJob = await DefaultConfig.sched.GetJobDetail(jk);
                    if (iJob == null) {
                        string triggerId = Guid.NewGuid().ToString().Replace("-", "");
                        ITrigger trigger = TriggerBuilder.Create()
                       .WithIdentity(triggerId, serverConfig.ServerUUID)
                       .WithCronSchedule(item.Value.Cron)
                       .ForJob(item.Value.TaskId, serverConfig.ServerUUID)
                       .Build();
                        JobDataMap jobDataMap = new JobDataMap();
                        KeyValuePair<string, object> Arma3Config = new KeyValuePair<string, object>("Arma3Config", serverConfig.ServerTaskManagement.CronEntity[item.Value.TaskId]);
                        KeyValuePair<string, object> TaskKey = new KeyValuePair<string, object>("TaskKey", item.Value.TaskId);
                        jobDataMap.Add(Arma3Config);
                        jobDataMap.Add(TaskKey);
                        IJobDetail job = JobBuilder.Create<ServerRestartManagementJob>()
                        .SetJobData(jobDataMap)
                        .WithIdentity(item.Value.TaskId, serverConfig.ServerUUID)
                        .Build();
                        await DefaultConfig.sched.ScheduleJob(job, trigger);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("初始化指定的计划任务失败:" + item.Value.String() + "\r\n" + ex.Message, "错误的CRON表达式!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
            }
        }




        private void barStaticItem1_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void barStaticItem2_ItemClick(object sender, ItemClickEventArgs e)
        {

        }



        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void sqlDataSource1_CustomizeFilterExpression(object sender, DevExpress.DataAccess.Sql.CustomizeFilterExpressionEventArgs e)
        {
            e.FilterExpression = op;
        }

        private void sqlDataSource1_ValidateCustomSqlQuery(object sender, DevExpress.DataAccess.ValidateCustomSqlQueryEventArgs e)
        {
            
        }

        private void sqlDataSource1_ConfigureDataConnection(object sender, ConfigureDataConnectionEventArgs e)
        {
            e.ConnectionParameters = SQLite;
        }

        //查询当天
        private void barButtonItem9_ItemClick(object sender, ItemClickEventArgs e)
        {
            op = GroupOperator.Combine(GroupOperatorType.And, new BinaryOperator("server_id", serverId, BinaryOperatorType.Equal), new BinaryOperator("create_time_timestamp", (DateTime.Today.ToUniversalTime().Ticks - 621355968000000000) / 10000000, BinaryOperatorType.Greater), new BinaryOperator("create_time_timestamp", (DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59).ToUniversalTime().Ticks - 621355968000000000) / 10000000, BinaryOperatorType.Less));
            RefreshData();
        }

        //查询本月
        private void barButtonItem10_ItemClick(object sender, ItemClickEventArgs e)
        {
            op = GroupOperator.Combine(GroupOperatorType.And, new BinaryOperator("server_id", serverId, BinaryOperatorType.Equal), new BinaryOperator("create_time_timestamp", (DateTime.Now.AddDays(1 - DateTime.Now.Day).Date.ToUniversalTime().Ticks - 621355968000000000) / 10000000, BinaryOperatorType.Greater), new BinaryOperator("create_time_timestamp", (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000, BinaryOperatorType.Less));
            RefreshData();
        }
        //查询全部
        private void barButtonItem11_ItemClick(object sender, ItemClickEventArgs e)
        {
            op = new BinaryOperator("server_id", serverId, BinaryOperatorType.Equal);
            RefreshData();
        }



        
        private void RefreshData() {
            try
            {
                sqlDataSource1.Fill();
                sqlDataSource2.Fill();
                sqlDataSource3.Fill();
                sqlDataSource4.Fill();
                gridControl4.RefreshDataSource();
                chartControl3.RefreshData();
                chartControl2.RefreshData();
                chartControl1.RefreshData();
                chartControl4.RefreshData();
                chartControl6.RefreshData();
                chartControl5.RefreshData();
                chartControl12.RefreshData();
                chartControl6.RefreshData();
                chartControl17.RefreshData();
                chartControl7.RefreshData();
                chartControl13.RefreshData();
                chartControl8.RefreshData();
                chartControl18.RefreshData();
                chartControl10.RefreshData();
                chartControl14.RefreshData();
                chartControl9.RefreshData();
                chartControl19.RefreshData();
                chartControl11.RefreshData();
                chartControl15.RefreshData();
            }
            catch { 
            }
        }




        private void ServerStatisticsManagement_Leave(object sender, EventArgs e)
        {
            try
            {
                if (networkClient != null && networkClient.IsConnected)
                {
                    networkClient.Disconnect();
                }
                SqliteUtils.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void gridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {
                popupMenu5.ShowPopup(MousePosition);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string str1 = gridView1.GetFocusedRowCellValue("ID").ToString();
            string str2 = gridView1.GetFocusedRowCellValue("名称").ToString();
            string str3 = gridView1.GetFocusedRowCellValue("已验证").ToString();
            string str4 = gridView1.GetFocusedRowCellValue("在大厅").ToString();
            string str5 = gridView1.GetFocusedRowCellValue("IP").ToString();
            string str6 = gridView1.GetFocusedRowCellValue("GUID").ToString();
            string str7 = gridView1.GetFocusedRowCellValue("Ping").ToString();
            Clipboard.SetText("ID=" + str1 + "\r\n名称=" + str2 + "\r\n已验证=" + str3 + "\r\n在大厅=" + str4 + "\r\nIP=" + str5 + "\r\nGUID=" + str6 + "\r\nPing=" + str7);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var result = XtraInputBox.Show("给玩家发送一条消息", "发送消息", "");
            if (result.Length <= 0)
            {
                XtraMessageBox.Show("请输入内容", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            int PlayerId = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
            if (BeIsConnect())
            {
                networkClient.Send(new SendMessageCommand(PlayerId, result));
            }
        }
        public bool BeIsConnect()
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                return true;
            }
            return false;
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            popupMenu5.HidePopup();
        }

        private void kickPlayer_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string name = gridView1.GetFocusedRowCellValue("名称").ToString();
            DialogResult resultDR = XtraMessageBox.Show("您确定踢出 " + name + " ?", "踢出玩家", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (resultDR == DialogResult.OK)
            {
                var result = XtraInputBox.Show("踢出原因", "踢出玩家", "");
                if (result.Length <= 0)
                {
                    result = "管理员踢出";
                }
                if (BeIsConnect())
                {
                    int PlayerId = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
                    networkClient.Send(new KickCommand(PlayerId, result));
                }
            }

        }




        private void Ban_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BanPlayerDialog dlg = new BanPlayerDialog();
            if (DevExpress.XtraEditors.XtraDialog.Show(dlg, "封禁玩家", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (BeIsConnect())
                {
                    string PlayerGUID = gridView1.GetFocusedRowCellValue("GUID").ToString();
                    string PlayerIP = gridView1.GetFocusedRowCellValue("IP").ToString();
                    if (dlg.BanType)
                    {
                        TimeSpan timeSpan = new TimeSpan(0, 0, dlg.Date, 0, 0);
                        if (dlg.GUID)
                        {

                            networkClient.Send(new BanOnlinePlayerCommand(PlayerGUID, dlg.Reason, timeSpan));
                        }
                        if (dlg.IP)
                        {
                            networkClient.Send(new BanOnlinePlayerCommand(PlayerIP, dlg.Reason, timeSpan));
                        }
                    }
                    else
                    {
                        networkClient.Send(new BanOnlinePlayerCommand(PlayerIP, dlg.Reason));
                    }
                }
            }
        }

        private void gridControl2_Load(object sender, EventArgs e)
        {
            Task.Run(() => GetBans());
        }


        private void GetBans()
        {
            if (!BeIsConnect())
            {
                return;
            }
            bool requestSuccess = networkClient.Fetch(
            command: new GetBansRequest(),
            timeout: 5000,
            result: out List<PlayerBan> playerban);
            this.Invoke(new Action(() => {
                BansDataTable.Rows.Clear();
            }));
            if (requestSuccess)
            {
                this.Invoke(new Action(() => {
                    try
                    {
                        foreach (PlayerBan ban in playerban)
                        {
                            BansDataTable.Rows.Add(ban.Id.ToString(), ban.Guid == null ? "无GUID" : ban.Guid, ban.Ip == null ? "无IP" : ban.Ip.ToString(), ban.DurationLeft.ToString(), ban.IsPermanent ? "是" : "否", ban.Reason, ban.IsIpBan ? "是" : "否", ban.IsGuidBan ? "是" : "否");
                        }
                    }
                    catch 
                    {

                    }

                }));
                gridControl2.DataSource = BansDataTable;
                gridControl2.ForceInitialize();
                if (BansDataTable.Rows.Count != 0)
                {
                    this.Invoke(new Action(() => {
                        for (int i = 0; i < 8; i++)
                        {
                            gridView2.Columns[i].OptionsColumn.AllowEdit = false;
                        }
                    }));
                }
            }
        }

        private void gridView2_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {
                popupMenu2.ShowPopup(MousePosition);
            }
        }

        private void gridView2_Click(object sender, EventArgs e)
        {
            popupMenu2.HidePopup();
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (BeIsConnect())
            {
                int Id = Convert.ToInt32(gridView2.GetFocusedRowCellValue("ID"));
                networkClient.Send(new RemoveBanCommand(Id));
                networkClient.Send(new LoadBansCommand());
                Task.Run(() => GetBans());
            }
        }

        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            string str1 = gridView2.GetFocusedRowCellValue("ID").ToString();
            string str2 = gridView2.GetFocusedRowCellValue("GUID").ToString();
            string str3 = gridView2.GetFocusedRowCellValue("IP").ToString();
            string str4 = gridView2.GetFocusedRowCellValue("剩余时间").ToString();
            string str5 = gridView2.GetFocusedRowCellValue("是否永久").ToString();
            string str6 = gridView2.GetFocusedRowCellValue("理由").ToString();
            string str7 = gridView2.GetFocusedRowCellValue("封IP").ToString();
            string str8 = gridView2.GetFocusedRowCellValue("封GUID").ToString();
            Clipboard.SetText("ID=" + str1 + "\r\nGUID=" + str2 + "\r\nIP=" + str3 + "\r\n剩余时间=" + str4 + "\r\n是否永久=" + str5 + "\r\n理由=" + str6 + "\r\n封IP=" + str7 + "\r\n封GUID=" + str8);
        }


        private void LoadMissions()
        {
            if (!BeIsConnect())
            {
                return;
            }
            bool requestSuccess = networkClient.Fetch(
                     command: new GetMissionsRequest(),
                     timeout: 5000,
                     result: out IEnumerable<Mission> missions);
            this.Invoke(new Action(() => {
                listBoxControl1.Items.Clear();
                try
                {
                    if (requestSuccess)
                    {
                        foreach (Mission miss in missions)
                        {
                            listBoxControl1.Items.Add(miss.Name + "." + miss.Map + ".pbo");
                        }
                    }
                }
                catch (Exception) { };
            }));
        }

        private void listBoxControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listBoxControl1.SelectedIndex >= 0)
                {
                    popupMenu3.ShowPopup(MousePosition);
                }
            }
            else
            {
                popupMenu3.HidePopup();
            }
        }

        private void barButtonItem27_ItemClick(object sender, ItemClickEventArgs e)
        {
            string miss = listBoxControl1.GetItemText(listBoxControl1.SelectedIndex).Replace(".pbo", "");
            networkClient.Send(new LoadMissionCommand(miss));
        }

        private void textEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!BeIsConnect())
                {
                    return;
                }
                if (textEdit1.Text.Length >= 1)
                {
                    networkClient.Send(new SendMessageCommand(textEdit1.Text));
                    textEdit1.Text = "";
                }
            }
        }

        private void barButtonItem17_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!BeIsConnect())
            {
                return;
            }
            networkClient.Send(new SaveBansCommand());
        }

        private void barButtonItem24_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!BeIsConnect())
            {
                return;
            }
            networkClient.Send(new ShutdownCommand());
        }

        private void barButtonItem18_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!BeIsConnect())
            {
                return;
            }
            networkClient.Send(new LoadBansCommand());
        }

        private void barButtonItem19_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!BeIsConnect())
            {
                return;
            }
            networkClient.Send(new LoadScriptFiltersCommand());
        }

        private void barButtonItem21_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!BeIsConnect())
            {
                return;
            }
            networkClient.Send(new LockServerCommand());
        }

        private void barButtonItem25_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!BeIsConnect())
            {
                return;
            }
            networkClient.Send(new ReassignCommand());
        }

        private void barButtonItem26_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!BeIsConnect())
            {
                return;
            }
            networkClient.Send(new RestartMissionCommand());
        }

        private void barButtonItem23_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!BeIsConnect())
            {
                return;
            }
            networkClient.Send(new LoadEventFiltersCommand());
        }

        private void barButtonItem22_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!BeIsConnect())
            {
                return;
            }
            networkClient.Send(new UnlockCommand());
        }


        public void loadPlayerDB()
        {
            try
            {
                PlayersDataTable.Rows.Clear();
                SqliteUtils.SelectPlayerAll();
                DefaultConfig.PlayerInfo.ForEach(p => {
                    PlayersDataTable.Rows.Add(p.Id.ToString(), p.Guid, p.Name, p.Ip, p.Time);
                });
            }
            catch {}
            gridControl3.DataSource = PlayersDataTable;
            gridControl3.ForceInitialize();
            if (PlayersDataTable.Rows.Count != 0)
            {
                this.Invoke(new Action(() => {
                    for (int i = 0; i < 5; i++)
                    {
                        gridView3.Columns[i].OptionsColumn.AllowEdit = false;
                    }
                }));
            }
        }

        private void gridControl3_Load(object sender, EventArgs e)
        {
            loadPlayerDB();
        }

        private void gridControl3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                popupMenu4.HidePopup();
            }
        }

        private void barButtonItem28_ItemClick(object sender, ItemClickEventArgs e)
        {
            string PlayerGUID = gridView3.GetFocusedRowCellValue("GUID").ToString();
            if (null != PlayerGUID && PlayerGUID.Length >= 1)
            {
                BanPlayerDialog dlg = new BanPlayerDialog();
                if (XtraDialog.Show(dlg, "封禁离线玩家", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (BeIsConnect())
                    {
                        string PlayerIP = gridView3.GetFocusedRowCellValue("IP").ToString();
                        if (dlg.BanType)
                        {
                            TimeSpan timeSpan = new TimeSpan(0, 0, dlg.Date, 0, 0);
                            if (dlg.GUID)
                            {

                                networkClient.Send(new BanPlayerCommand(PlayerGUID, dlg.Reason, timeSpan));
                            }
                            if (dlg.IP)
                            {
                                networkClient.Send(new BanPlayerCommand(PlayerIP, dlg.Reason, timeSpan));
                            }
                        }
                        else
                        {
                            networkClient.Send(new BanPlayerCommand(PlayerIP, dlg.Reason));
                        }
                    }
                }
            }
        }

        private void barButtonItem29_ItemClick(object sender, ItemClickEventArgs e)
        {
            string str1 = gridView3.GetFocusedRowCellValue("ID").ToString();
            string str2 = gridView3.GetFocusedRowCellValue("GUID").ToString();
            string str3 = gridView3.GetFocusedRowCellValue("玩家名称").ToString();
            string str4 = gridView3.GetFocusedRowCellValue("IP").ToString();
            string str5 = gridView3.GetFocusedRowCellValue("更新时间").ToString();

            Clipboard.SetText("ID=" + str1 + "\r\nGUID=" + str2 + "\r\n玩家名称=" + str3 + "\r\nIP=" + str4 + "\r\n更新时间=" + str5);
        }

        private void gridView3_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {
                popupMenu4.ShowPopup(MousePosition);
            }
        }



        public void StatusTextUpdate(int count, string date)
        {
            Invoke(new Action(() => {
                try
                {
                    SetStatusText(
                        string.Format("连接: {0}", "127.0.0.1:" + serverConfig.BattlEyeConfig.RConPort),
                        string.Format("玩家: {0}", count),
                        string.Format("上次刷新时间: {0}", date),
                        string.Format("最大延迟: {0}", serverConfig.ServerConfig.MaxPing)
                     );
                }
                catch { };
            }));
        }
        public void SetStatusText(string conn,string playerNum,string lastUpdate,string maxPing) {
            barStaticItem2.Caption = conn;
            barStaticItem3.Caption = playerNum;
            barStaticItem4.Caption = lastUpdate;
            barStaticItem5.Caption = maxPing;
        }

        private void barButtonItem14_ItemClick(object sender, ItemClickEventArgs e)
        {
            Task.Run(() => LoadMissions());
            tabPane1.SelectedPageIndex = 3;
        }

        private void barButtonItem13_ItemClick(object sender, ItemClickEventArgs e)
        {
            Task.Run(() => GetBans());
            tabPane1.SelectedPageIndex = 2;
        }

        private void barButtonItem12_ItemClick(object sender, ItemClickEventArgs e)
        {
            Task.Run(() => LoadPlayers());
            tabPane1.SelectedPageIndex = 0;
        }

        private void barButtonItem16_ItemClick(object sender, ItemClickEventArgs e)
        {
            loadPlayerDB();
            tabPane1.SelectedPageIndex = 5;
        }

        private void barButtonItem15_ItemClick(object sender, ItemClickEventArgs e)
        {
            tabPane1.SelectedPageIndex = 4;
        }

        private void ribbon_SelectedPageChanged(object sender, EventArgs e)
        {
            
        }

        private void ribbon_SelectedPageChanging(object sender, RibbonPageChangingEventArgs e)
        {
            switch (e.Page.Tag) {
                case 0:
                    tabPane1.SelectedPageIndex = 0;
                    break;
                case 1:
                    tabPane1.SelectedPageIndex = 1;
                    barButtonItem9_ItemClick(null, null);
                    break;
                case 2:
                    tabPane1.SelectedPageIndex = 6;
                    break;
            }
        }

        private void gridControl4_Load(object sender, EventArgs e)
        {
            if (gridView4.RowCount > 0) {
                for (int i = 0; i < gridView4.Columns.Count; i++) {
                    gridView4.Columns[i].OptionsColumn.AllowEdit = false;
                }
            }
        }




        //添加任务
        private void barButtonItem39_ItemClick(object sender, ItemClickEventArgs e)
        {
            AddTaskDialog scanPathDialog = new AddTaskDialog();
            if (XtraDialog.Show(scanPathDialog, "创建任务", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    string triggerId = Guid.NewGuid().ToString().Replace("-", "");
                    string jobId = Guid.NewGuid().ToString().Replace("-", "");
                    ITrigger trigger = TriggerBuilder.Create()
                     .WithIdentity(triggerId, serverConfig.ServerUUID)
                     .WithCronSchedule(scanPathDialog.Cron)
                     .ForJob(jobId, serverConfig.ServerUUID)
                     .Build();
                    CronEntity cron = new CronEntity(jobId, serverConfig.ServerUUID, serverConfig.ServerConfig.HostName, scanPathDialog.Cron, scanPathDialog.SysRemark, scanPathDialog.EnableCron, scanPathDialog.CronAction, scanPathDialog.CronActionText);
                    serverConfig.ServerTaskManagement.CronEntity.Add(jobId, cron);
                    JobDataMap jobDataMap = new JobDataMap();
                    KeyValuePair<string, object> Arma3Config = new KeyValuePair<string, object>("Arma3Config", serverConfig.ServerTaskManagement.CronEntity[jobId]);
                    KeyValuePair<string, object> TaskKey = new KeyValuePair<string, object>("TaskKey", jobId);
                    jobDataMap.Add(Arma3Config);
                    jobDataMap.Add(TaskKey);
                    IJobDetail job = JobBuilder.Create<ServerRestartManagementJob>()
                    .SetJobData(jobDataMap)
                    .WithIdentity(jobId, serverConfig.ServerUUID)
                    .Build();
                    DefaultConfig.sched.ScheduleJob(job, trigger);
                    LoadJobList();
                    FileTools.SaveConfig(serverConfig.ServerUUID);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message, "错误的CRON表达式!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
            }
        }



        public void LoadJobList() {
            TaskCronDataTable.Rows.Clear();
            foreach (KeyValuePair<string, CronEntity> item in serverConfig.ServerTaskManagement.CronEntity) {
                TaskCronDataTable.Rows.Add(item.Value.TaskId, item.Value.ServerUUID, item.Value.ServerName , item.Value.Cron, item.Value.ActionText, item.Value.Remark, item.Value.Status==1?"已启用":"已禁用");
            }
            gridControl5.DataSource = TaskCronDataTable;
            gridControl5.ForceInitialize();

            if (gridView5.RowCount > 0)
            {
                for (int i = 0; i < gridView5.Columns.Count; i++)
                {
                    gridView5.Columns[i].OptionsColumn.AllowEdit = false;
                }
            }
        }

        private async void barButtonItem40_ItemClick(object sender, ItemClickEventArgs e)
        {
            string TaskId = gridView5.GetFocusedRowCellValue("任务ID").ToString();
            string ServerUUID = gridView5.GetFocusedRowCellValue("服务器UUID").ToString();
            if (serverConfig.ServerTaskManagement.CronEntity.ContainsKey(TaskId))
            {
                JobKey jk = JobKey.Create(TaskId, ServerUUID);
                bool isOk = await DefaultConfig.sched.DeleteJob(jk, CancellationToken.None);
                if (isOk)
                {
                    serverConfig.ServerTaskManagement.CronEntity.Remove(TaskId);
                    gridView5.DeleteRow(gridView5.GetFocusedDataSourceRowIndex());
                }
                else {
                    XtraMessageBox.Show("删除任务失败!任务调度程序返回了失败标识!", "失败!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
            }
            else {
                XtraMessageBox.Show("删除指定的任务失败!没有从列表里找到指定的任务!", "失败!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
        }

        private void gridControl5_Load(object sender, EventArgs e)
        {
            LoadJobList();
        }

        private void barButtonItem38_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadJobList();
        }

        private void barButtonItem41_ItemClick(object sender, ItemClickEventArgs e)
        {
            string TaskId = gridView5.GetFocusedRowCellValue("任务ID").ToString();
            if (serverConfig.ServerTaskManagement.CronEntity.ContainsKey(TaskId))
            {
                serverConfig.ServerTaskManagement.CronEntity[TaskId].Status = serverConfig.ServerTaskManagement.CronEntity[TaskId].Status == 1 ? 0 : 1;
                LoadJobList();
            }
            else
            {
                XtraMessageBox.Show("启停指定的任务失败!没有从列表里找到指定的任务!", "失败!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
        }

        private void gridView5_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {
                popupMenu6.ShowPopup(MousePosition);
            }
        }

        private void gridControl5_Click(object sender, EventArgs e)
        {
            popupMenu5.HidePopup();
        }




        private void gridControl5_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && gridView5.RowCount == 0) {
                barButtonItem39_ItemClick(null,null);
            }
        }

        private void barButtonItem30_ItemClick(object sender, ItemClickEventArgs e)
        {
            int StartPort = DefaultConfig.ServerList[tools.serverUUID].StartupParameters.Port;
            if (CfgTool.UsePort(StartPort)) {
                XtraMessageBox.Show("检测到端口:"+ StartPort+"已经被使用!无法启动服务器!", "端口被占用!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            tools.Start();
        }

        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            tools.Stop();
        }
        private void barButtonItem8_ItemClick(object sender, ItemClickEventArgs e)
        {
            tools.Stop();
            tools.Start();
        }

        private void barButtonItem36_ItemClick(object sender, ItemClickEventArgs e)
        {
            tabPane1.SelectedPageIndex = 6;
        }

        private void barButtonItem37_ItemClick(object sender, ItemClickEventArgs e)
        {
            tabPane1.SelectedPageIndex = 7;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (!Directory.Exists(path + @"mod\@destiny_server"))
            {
                XtraMessageBox.Show("创建模组失败!当前开服工具根目录下 mod -> @destiny_server模组不存在!", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (File.Exists(path + @"mod\@destiny_server\addons\destiny_server.pbo"))
            {
                File.Delete(path + @"mod\@destiny_server\addons\destiny_server.pbo");
            }

            if (!File.Exists(path + @"pbo\PBOConsole.exe"))
            {
                XtraMessageBox.Show("创建模组失败!当前开服工具根目录下 pbo -> PBOConsole.exe不存在!", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try {
                FileStream fs = new FileStream(path + @"mod\@destiny_server\addons\destiny_server\args.sqf", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, CfgTool.UTF8);
                string sqf = sr.ReadToEnd();
                sr.Close();
                fs.Close();
                sqf = string.Format(sqf, toggleSwitch1.IsOn ? spinEdit1.Value : 0, textEdit2.Text, spinEdit2.Value, serverConfig.ServerConfig.ServerCommandPassword, serverConfig.ServerTaskManagement.EnableMonitoringService.ToString().ToLower(),serverConfig.ServerUUID);
                File.WriteAllText(path + @"mod\@destiny_server\addons\destiny_server\fn_initFunctions.sqf", sqf, CfgTool.UTF8);
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = "-pack "+ path + @"mod\@destiny_server\addons\destiny_server" + " " + path + @"mod\@destiny_server\addons\destiny_server.pbo",
                    FileName = path + @"pbo\PBOConsole.exe"
                };
               
                Process.Start(startInfo);
                Thread.Sleep(3000);

                if (Directory.Exists(serverConfig.ServerDir + @"\@destiny_server"))
                {
                    DirectoryInfo di = new DirectoryInfo(serverConfig.ServerDir + @"\@destiny_server");
                    di.Delete(true);
                }
                if (!FileTools.CopyDirectory(path + @"mod\@destiny_server", serverConfig.ServerDir + @"\@destiny_server")) {
                    XtraMessageBox.Show("无法将:\r\n"+ path + @"mod\@destiny_server"+"\r\n移动到:\r\n"+ serverConfig.ServerDir + @"\@destiny_server", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (File.Exists(serverConfig.ServerDir + @"\DestinyServerMonitoring.dll")) {
                    File.Delete(serverConfig.ServerDir + @"\DestinyServerMonitoring.dll");
                }
                if (File.Exists(serverConfig.ServerDir + @"\DestinyServerMonitoring_x64.dll"))
                {
                    File.Delete(serverConfig.ServerDir + @"\DestinyServerMonitoring_x64.dll");
                }
                if (!FileTools.CopyDirectory(path + "extension", serverConfig.ServerDir))
                {
                    XtraMessageBox.Show("无法将:\r\n" + path + "extension" + "\r\n移动到:\r\n" + serverConfig.ServerDir, "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!File.Exists(serverConfig.ServerDir+ @"\@destiny_server\addons\destiny_server.pbo"))
                {
                    XtraMessageBox.Show("打包PBO文件失败!请手动打包成PBO文件，需要打包的文件夹是:" + serverConfig.ServerDir + @"\@destiny_server\addons\destiny_server", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                FileTools.SaveConfig(serverConfig.ServerUUID);
            }
            catch(Exception ex) {
                XtraMessageBox.Show("创建模组失败!创建时发生异常:"+ex.Message, "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void toggleSwitch3_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            serverConfig.ServerTaskManagement.EnableMonitoringService = e.NewValue.ToString().ToLower().Equals("true");
        }

        private void toggleSwitch1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            serverConfig.ServerTaskManagement.RestartTime = e.NewValue.ToString().ToLower().Equals("true") ? (int)spinEdit1.Value : 0;
        }

        private void spinEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try
            {
                serverConfig.ServerTaskManagement.RestartTime = toggleSwitch1.IsOn ? int.Parse(e.NewValue.ToString()) : 0;
            }
            catch { 
            }
        }

        private void spinEdit2_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try
            {
                serverConfig.ServerTaskManagement.RestartLastTime = int.Parse(e.NewValue.ToString());
            }
            catch { 
            }
        }

        private void textEdit2_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            serverConfig.ServerTaskManagement.RestartInfo = e.NewValue.ToString();
        }

        private void toggleSwitch4_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            serverConfig.ServerTaskManagement.EnableMonitor = e.NewValue.ToString().ToLower().Equals("true");
        }

        private void barButtonItem42_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                MonitoringServiceSqliteUtils.DeleteData(((DateTime.Now.AddMonths(-1).ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString());
                XtraMessageBox.Show("成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) {
                XtraMessageBox.Show("删除失败!"+ ex.Message, "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sqlDataSource2_ConfigureDataConnection(object sender, ConfigureDataConnectionEventArgs e)
        {
            e.ConnectionParameters = SQLite;
        }

        private void sqlDataSource3_ConfigureDataConnection(object sender, ConfigureDataConnectionEventArgs e)
        {
            e.ConnectionParameters = SQLite;
        }

        private void sqlDataSource4_ConfigureDataConnection(object sender, ConfigureDataConnectionEventArgs e)
        {
            e.ConnectionParameters = SQLite;
        }

        private void barButtonItem45_ItemClick(object sender, ItemClickEventArgs e)
        {
            tools.StartHeadlessClient();
        }
    }
}