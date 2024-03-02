using a3.Config;
using a3.Dialog;
using a3.Entity;
using Arma3ServerTools.Entity;
using BytexDigital.BattlEye.Rcon;
using BytexDigital.BattlEye.Rcon.Commands;
using BytexDigital.BattlEye.Rcon.Domain;
using DevExpress.CodeParser;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace a3.Modules
{
    public partial class RconModules : DevExpress.DXperience.Demos.TutorialControlBase
    {
        private string id;
        private RconClient networkClient = null;
        private StringBuilder htmlMsg = new StringBuilder();

        private bool AllowEdit = true;
        private bool AllowEditBans = true;
        private bool Exit = false;
        private Task PlayerTask;

        private DataTable dataTable = new DataTable();
        private DataTable BansDataTable = new DataTable();

        public RconModules()
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
        }

        public bool setId(string id) {
            this.id = id;
            try {
                DefaultConfig.RconMap.TryGetValue(id, out RconEntity r);
                if (r != null)
                {
                    networkClient = new RconClient(r.IP, r.Port, r.Password);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch 
            {
                return false;
            }
            
        }

        private void barButtonItem28_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool connect = false;
            try
            {
                networkClient.ReconnectOnFailure = true;
                connect = networkClient.Connect();
                if (connect)
                {
                    networkClient.Disconnected += NetworkClient_Disconnected;
                    networkClient.MessageReceived += NetworkClient_MessageReceived;
                    networkClient.PlayerConnected += NetworkClient_PlayerConnected;
                    networkClient.PlayerDisconnected += NetworkClient_PlayerDisconnected;
                    networkClient.PlayerRemoved += NetworkClient_PlayerRemoved;

                    if (PlayerTask == null)
                    {
                        PlayerTask = Task.Run(async () => {

                            while (!Exit)
                            {
                                DefaultConfig.RconMap.TryGetValue(id, out RconEntity r);
                                if (r == null)
                                {
                                    Exit = true;
                                    break;
                                }
                                if (networkClient != null && networkClient.IsConnected)
                                {
                                    LoadPlayers();
                                    LoadBans();
                                    LoadMissions();
                                    await Task.Delay(90000);
                                }
                                await Task.Delay(1000);
                            }
                        });
                    }
                    barButtonItem28.Enabled = false;
                }
                else
                {
                    barButtonItem28.Enabled = true;
                }
            }
            catch {
                barButtonItem28.Enabled = true;
            }
            htmlMsg.AppendLine($"<font color=\" {(connect ? "green" : "red")} \">[{getTime()}]:{(connect ? "连接成功!" : "连接失败!")}</font><br/>");
            richEditControl1.Invoke(new Action(() => {
                richEditControl1.HtmlText = htmlMsg.ToString();
            }));
        }

        private void NetworkClient_Disconnected(object sender, EventArgs e)
        {
            barButtonItem28.Enabled = true;
            htmlMsg.AppendLine($"<font color=\"red \">[{getTime()}]:已断开连接...</font><br/>");
            richEditControl1.Invoke(new Action(() => {
                richEditControl1.HtmlText = htmlMsg.ToString();
            }));
        }
        private void NetworkClient_MessageReceived(object sender, string e)
        {
            htmlMsg.AppendLine($"<font color=\"green \">[{getTime()}]:{e}</font><br/>");
            richEditControl1.Invoke(new Action(() => {
                richEditControl1.HtmlText = htmlMsg.ToString();
            }));
        }
        private void NetworkClient_PlayerDisconnected(object sender, BytexDigital.BattlEye.Rcon.Events.PlayerDisconnectedArgs e)
        {
            htmlMsg.AppendLine($"<font color=\"blue \">[{getTime()}]:玩家 {e.Name} [id:{e.Id}] 与服务器断开连接.</font><br/>");
            richEditControl1.Invoke(new Action(() => {
                richEditControl1.HtmlText = htmlMsg.ToString();
            }));
            Task.Run( () => LoadPlayers());
        }

        private void NetworkClient_PlayerConnected(object sender, BytexDigital.BattlEye.Rcon.Events.PlayerConnectedArgs e)
        {
            htmlMsg.AppendLine($"<font color=\"white \">[{getTime()}]:玩家 {e.Name} [id:{e.Id}] [guid:{e.Guid}] 连接到服务器.</font><br/>");
            richEditControl1.Invoke(new Action(() => {
                richEditControl1.HtmlText = htmlMsg.ToString();
            }));
            Task.Run(() => LoadPlayers());
        }
        private void NetworkClient_PlayerRemoved(object sender, BytexDigital.BattlEye.Rcon.Events.PlayerRemovedArgs e)
        {
            htmlMsg.AppendLine($"<font color=\"red \">[{getTime()}]:玩家 {e.Name} [id:{e.Id}] [guid:{e.Guid}] {(e.IsBan ? "被封禁，原因是:" : "被踢出，原因是:")} {e.Reason} </font><br/>");
            richEditControl1.Invoke(new Action(() => {
                richEditControl1.HtmlText = htmlMsg.ToString();
            }));
            Task.Run(() => LoadPlayers());
        }

        private string getTime() {
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            int second = DateTime.Now.Second;
            return $"{hour}:{minute}:{second}";
        }
       

        public void LoadPlayers()
        {
            try
            {
                bool requestSuccess = networkClient.Fetch(
                   command: new GetPlayersRequest(),
                   timeout: 5000,
                   result: out List<Player> onlinePlayers);
                if (requestSuccess)
                {
                    Invoke(new Action(() => {
                        try
                        {
                            dataTable.Rows.Clear();
                            foreach (var player in onlinePlayers)
                            {
                                dataTable.Rows.Add(player.Id.ToString(), player.Name, player.IsVerified ? "是" : "否", player.IsInLobby ? "是" : "否", player.RemoteEndpoint.Address.ToString(), player.Guid, player.Ping.ToString());
                            }
                            if (dataTable.Rows.Count != 0 && AllowEdit)
                            {
                                gridControl1.DataSource = dataTable;
                                gridControl1.ForceInitialize();
                                AllowEdit = false;
                                for (int i = 0; i < 7; i++)
                                {
                                    gridView1.Columns[i].OptionsColumn.AllowEdit = false;
                                }
                            }
                        }
                        catch
                        {

                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void LoadBans()
        {
            bool requestSuccess = networkClient.Fetch(
            command: new GetBansRequest(),
            timeout: 5000,
            result: out List<PlayerBan> playerban);
            if (requestSuccess)
            {
                this.Invoke(new Action(() => {
                    BansDataTable.Rows.Clear();
                    try
                    {
                        foreach (PlayerBan ban in playerban)
                        {
                            BansDataTable.Rows.Add(ban.Id.ToString(), ban.Guid == null ? "无GUID" : ban.Guid, ban.Ip == null ? "无IP" : ban.Ip.ToString(), ban.DurationLeft.ToString(), ban.IsPermanent ? "是" : "否", ban.Reason, ban.IsIpBan ? "是" : "否", ban.IsGuidBan ? "是" : "否");
                        }
                        if (BansDataTable.Rows.Count != 0 && AllowEditBans)
                        {
                            gridControl2.DataSource = BansDataTable;
                            gridControl2.ForceInitialize();
                            AllowEditBans = false;
                            for (int i = 0; i < 8; i++)
                            {
                                gridView2.Columns[i].OptionsColumn.AllowEdit = false;
                            }
                        }
                    }
                    catch{}
                }));
            }
        }

        private void LoadMissions()
        {
            bool requestSuccess = networkClient.Fetch(
                     command: new GetMissionsRequest(),
                     timeout: 5000,
                     result: out IEnumerable<Mission> missions);
            Invoke(new Action(() => {
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
                catch { };
            }));
        }

        private void barButtonItem14_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                networkClient.Send(new ShutdownCommand());
            }
        }

        private void barButtonItem15_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                networkClient.Send(new ReassignCommand());
            }
        }

        private void barButtonItem16_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                networkClient.Send(new RestartMissionCommand());
            }
        }

        private void barButtonItem17_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                networkClient.Send(new LockServerCommand());
            }
        }

        private void barButtonItem18_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                networkClient.Send(new UnlockCommand());
            }
        }

        private void barButtonItem21_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                networkClient.Send(new SaveBansCommand());
            }
        }

        private void barButtonItem22_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                networkClient.Send(new LoadBansCommand());
            }
        }

        private void barButtonItem23_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                networkClient.Send(new LoadScriptFiltersCommand());
            }
        }

        private void barButtonItem24_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                networkClient.Send(new LoadEventFiltersCommand());
            }
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPageIndex = 0;
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPageIndex = 1;
        }

        private void barButtonItem25_ItemClick(object sender, ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPageIndex = 2;
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            navigationFrame1.SelectedPageIndex = 3;
        }

        private void barButtonItem26_ItemClick(object sender, ItemClickEventArgs e)
        {
            AddRconInfoDialog addRconInfo = new AddRconInfoDialog();
            addRconInfo.InitByID(id);
            if (XtraDialog.Show(addRconInfo, "编辑Rcon", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (addRconInfo.port == 0 || string.IsNullOrEmpty(addRconInfo.ip) || string.IsNullOrEmpty(addRconInfo.password) || string.IsNullOrEmpty(addRconInfo.name))
                {
                    XtraMessageBox.Show("请将表单输入完整!", "提示!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DefaultConfig.RconMap.TryGetValue(id, out RconEntity r);
                if (r != null) {
                    r.Port = addRconInfo.port;
                    r.IP = addRconInfo.ip;
                    r.Password = addRconInfo.password;
                    r.ServerName = addRconInfo.name;
                }

            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            popupMenu1.HidePopup();
        }

        private void gridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {
                popupMenu1.ShowPopup(MousePosition);
            }
        }

        private void gridView2_Click(object sender, EventArgs e)
        {
            popupMenu2.HidePopup();
        }

        private void gridView2_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {
                popupMenu2.ShowPopup(MousePosition);
            }
        }

        private void RemoveBan_barButtonItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                int Id = Convert.ToInt32(gridView2.GetFocusedRowCellValue("ID"));
                networkClient.Send(new RemoveBanCommand(Id));
                networkClient.Send(new LoadBansCommand());
                Task.Run(async () => {
                    await Task.Delay(1000);
                    LoadBans();
                });
            }
        }

        private void barButtonItem30_ItemClick(object sender, ItemClickEventArgs e)
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

        private void barButtonItem29_ItemClick(object sender, ItemClickEventArgs e)
        {
            string miss = listBoxControl1.GetItemText(listBoxControl1.SelectedIndex).Replace(".pbo", "");
            if (networkClient != null && networkClient.IsConnected)
                networkClient.Send(new LoadMissionCommand(miss));
        }

        private void Kick_Bar_ItemClick(object sender, ItemClickEventArgs e)
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
                if (networkClient != null && networkClient.IsConnected)
                {
                    int PlayerId = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
                    networkClient.Send(new KickCommand(PlayerId, result));
                    Task.Run(async () => {
                        await Task.Delay(1000);
                        LoadPlayers();
                    });
                }
            }
        }

        private void barButtonItem27_ItemClick(object sender, ItemClickEventArgs e)
        {
            BanPlayerDialog dlg = new BanPlayerDialog();
            if (DevExpress.XtraEditors.XtraDialog.Show(dlg, "封禁玩家", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (networkClient != null && networkClient.IsConnected)
                {
                    string PlayerGUID = gridView1.GetFocusedRowCellValue("GUID").ToString();
                    string PlayerIP = gridView1.GetFocusedRowCellValue("IP").ToString();
                    int PlayerId = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
                    if (dlg.BanType)
                    {
                        TimeSpan timeSpan = new TimeSpan(0, 0, dlg.Date, 0, 0);
                        if (dlg.GUID)
                        {

                            networkClient.Send(new BanOnlinePlayerCommand(PlayerGUID, dlg.Reason, timeSpan));
                            networkClient.Send(new BanPlayerCommand(PlayerGUID, dlg.Reason, timeSpan));
                            networkClient.Send(new KickCommand(PlayerId, dlg.Reason));
                        }
                        if (dlg.IP)
                        {
                            networkClient.Send(new BanOnlinePlayerCommand(PlayerIP, dlg.Reason, timeSpan));
                            networkClient.Send(new BanPlayerCommand(PlayerIP, dlg.Reason, timeSpan));
                            networkClient.Send(new KickCommand(PlayerId, dlg.Reason));
                        }
                    }
                    else
                    {
                        networkClient.Send(new BanOnlinePlayerCommand(PlayerIP, dlg.Reason));
                        networkClient.Send(new BanPlayerCommand(PlayerIP, dlg.Reason));
                        networkClient.Send(new KickCommand(PlayerId, dlg.Reason));
                    }
                    Task.Run(async () => {
                        await Task.Delay(1000);
                        LoadPlayers();
                    });
                }
            }
        }

        private void barButtonItem31_ItemClick(object sender, ItemClickEventArgs e)
        {
            var result = XtraInputBox.Show("给玩家发送一条消息", "发送消息", "");
            if (result.Length <= 0)
            {
                XtraMessageBox.Show("请输入内容", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            int PlayerId = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
            if (networkClient != null && networkClient.IsConnected)
            {
                networkClient.Send(new SendMessageCommand(PlayerId, result));
            }
        }

        private void barButtonItem32_ItemClick(object sender, ItemClickEventArgs e)
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

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (networkClient != null && networkClient.IsConnected)
            {
                networkClient.Send(new InitCommand());
            }
        }
    }
}
