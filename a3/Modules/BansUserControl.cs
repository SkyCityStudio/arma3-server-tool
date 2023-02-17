using a3.Config;
using a3.Tools;
using Arma3ServerTools.Dialog;
using Arma3ServerTools.Entity;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace a3.Modules
{
    public partial class BansUserControl : DevExpress.DXperience.Demos.TutorialControlBase
    {
        private Dictionary<string, LocalBansEntity> localBans = new Dictionary<string, LocalBansEntity>();
        private Dictionary<string, LocalBansEntity> onlineBans = new Dictionary<string, LocalBansEntity>();

        string[] path = { };

        private DataTable localBansDataTable = new DataTable();
        private DataTable onlineBansDataTable = new DataTable();

        public BansUserControl()
        {
            InitializeComponent();
            foreach (var key in DefaultConfig.ServerList)
            {
                mruEdit1.Properties.Items.Add(DefaultConfig.ServerList[key.Key].ConfigName + "  [" + key.Key + "]");
            }
            localBansDataTable.Columns.Add("GUID/IP/UID", typeof(string));
            localBansDataTable.Columns.Add("到期日期", typeof(string));
            localBansDataTable.Columns.Add("原因", typeof(string));
            onlineBansDataTable.Columns.Add("GUID/IP/UID", typeof(string));
            onlineBansDataTable.Columns.Add("到期日期", typeof(string));
            onlineBansDataTable.Columns.Add("原因", typeof(string));
            onlineBansDataTable.Columns.Add("添加日期", typeof(string));
            onlineBansDataTable.Columns.Add("列表来源", typeof(string));
            if (DefaultConfig.bansUrlEntities.Count == 0)
            {
                DefaultConfig.bansUrlEntities.Add(new BansUrlEntity("http://tools.destiny.cool/arma3_server_tools/bans.txt", true));
            }

        }


        private void ReadBans(string bansUrl) {
            try
            {
                if (string.IsNullOrEmpty(bansUrl)) {
                    return;
                }
                Stream stream = WebRequest.Create(bansUrl).GetResponse().GetResponseStream();
                StreamReader bans = new StreamReader(stream);
                AddToGrid(bans.ReadToEnd(), onlineBans, onlineBansDataTable,false);
                gridControl2.DataSource = onlineBansDataTable;
                gridControl2.ForceInitialize();
                if (onlineBansDataTable.Rows.Count != 0)
                {
                    gridView2.Columns[0].OptionsColumn.AllowEdit = false;
                    gridView2.Columns[1].OptionsColumn.AllowEdit = false;
                    gridView2.Columns[2].OptionsColumn.AllowEdit = false;
                    gridView2.Columns[3].OptionsColumn.AllowEdit = false;
                    gridView2.Columns[4].OptionsColumn.AllowEdit = false;
                }
            }
            catch
            {

            }
        }


        private void AddToGrid(string data, Dictionary<string, LocalBansEntity> bansList, DataTable table,bool local)
        {
            string[] striparr = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string j in striparr)
            {
                string[] banInfo = j.Split(new string[] { " " }, StringSplitOptions.None);
                if (banInfo.Length > 0)
                {
                    LocalBansEntity entity = new LocalBansEntity(banInfo[0], banInfo.Length > 1 ? (banInfo[1].Equals("-1") ? "永久封禁" : banInfo[1]) : "", banInfo.Length > 2 ? banInfo[2] : "", banInfo.Length > 3 ? banInfo[3] : "", banInfo.Length > 4 ? banInfo[4] : "");
                    if (!bansList.ContainsKey(banInfo[0]) &&  !string.IsNullOrEmpty(banInfo[0]))
                    {
                        bansList.Add(banInfo[0], entity);
                        if (local) {
                            table.Rows.Add(entity.GUID, entity.Time, entity.Reason);
                        }
                        else {
                            table.Rows.Add(entity.GUID, entity.Time, entity.Reason, entity.AddTime, entity.SyncName);
                            
                        }
                    }
                }
            }
        }



        private void mruEdit1_SelectedValueChanged(object sender, EventArgs e)
        {
            onlineBans.Clear();
            onlineBansDataTable.Clear();


            string text = mruEdit1.SelectedItem.ToString();
            if (text.Equals("选择一个服务器")) {
                localBansDataTable.Clear();
                onlineBansDataTable.Clear();
                return;
            }

            DefaultConfig.bansUrlEntities.ForEach(bans => {
                ReadBans(bans.enable ? bans.url : "");
            });

            string key = "";
            string left = "[";
            int Lindex = text.IndexOf(left);
            if (Lindex == -1)
            {
                key = "";
            }

            Lindex = Lindex + left.Length;

            int Rindex = text.IndexOf("]", Lindex);

            if (Rindex == -1)
            {
                key = "";
            }

            key = text.Substring(Lindex, Rindex - Lindex);
            if (string.IsNullOrEmpty(key)) {
                XtraMessageBox.Show("查询选中的服务器失败!", "找不到服务器配置!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                groupControl1.Text = "本地封禁列表:" + DefaultConfig.ServerList[key].ConfigName;
                Array.Clear(path,0,path.Length);
                Array.Resize(ref path,3);
                path[0] = DefaultConfig.ServerList[key].ServerDir + @"\bans.txt";
                path[1] = DefaultConfig.ServerList[key].ServerDir + @"\destiny_serverconfig\" + key + @"\BattlEye\bans.txt";
                path[2] = DefaultConfig.ServerList[key].ServerDir + @"\destiny_serverconfig\" + key + @"\Users\" + key + @"\bans.txt";
                localBans.Clear();
                localBansDataTable.Clear();
                foreach (string s in path) {
                    if (File.Exists(s))
                    {
                        FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
                        StreamReader sr = new StreamReader(fs, CfgTool.UTF8);
                        string data = sr.ReadToEnd();
                        AddToGrid(data,localBans, localBansDataTable,true);
                    }
                }
                gridControl1.DataSource = localBansDataTable;
                gridControl1.ForceInitialize();
                if (localBansDataTable.Rows.Count != 0)
                {
                    gridView1.Columns[0].OptionsColumn.AllowEdit = false;
                    gridView1.Columns[1].OptionsColumn.AllowEdit = false;
                    gridView1.Columns[2].OptionsColumn.AllowEdit = false;
                }
            }
            catch (Exception ex){
                XtraMessageBox.Show("查询选中的服务器bans文件失败!发生异常!"+ ex.Message, "查询bans失败!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridView2.DataRowCount; i++)
            {
                string guid = gridView2.GetRowCellValue(i, "GUID/IP/UID").ToString();
                string time = gridView2.GetRowCellValue(i, "到期日期").ToString();
                string res = gridView2.GetRowCellValue(i, "原因").ToString();
                if (!localBans.ContainsKey(guid) && !string.IsNullOrEmpty(guid)) {
                    localBans.Add(guid, new LocalBansEntity(guid, time.Equals("永久封禁") ? "-1" : time, res,"",""));
                }
            }

            SaveToBans();
            mruEdit1_SelectedValueChanged(null,null);
        }

        private bool SaveToBans() {
            StringBuilder sb = new StringBuilder();
            bool Success = true;
            foreach (var key in localBans)
            {
                sb.Append(key.Value.GUID).Append(" ").Append(key.Value.Time.Equals("永久封禁") ? "-1" : key.Value.Time).Append(" ").AppendLine(key.Value.Reason);
            }
            try
            {
                File.WriteAllText(path[0], sb.ToString(), CfgTool.UTF8);
            }
            catch (Exception ex)
            {
                Success = false;
                XtraMessageBox.Show("保存失败!" + ex.Message + "保存到路径:\r\n" + path[0], "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                File.WriteAllText(path[1], sb.ToString(), CfgTool.UTF8);
            }
            catch (Exception ex)
            {
                Success = false;
                XtraMessageBox.Show("保存失败!" + ex.Message + "保存到路径:\r\n" + path[1], "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                File.WriteAllText(path[2], sb.ToString(), CfgTool.UTF8);
            }
            catch (Exception ex)
            {
                Success = false;
                XtraMessageBox.Show("保存失败!" + ex.Message + "保存到路径:\r\n" + path[2], "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Success;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            BansurlControl bansurl = new BansurlControl();
            XtraDialog.Show(bansurl, "配置共享bans地址", MessageBoxButtons.OKCancel);
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string guid = gridView1.GetFocusedRowCellValue("GUID/IP/UID").ToString();
            if (!localBans.ContainsKey(guid)) {
                XtraMessageBox.Show("找不到:"+guid, "找不到UID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            localBans.Remove(guid);
            if (SaveToBans()) {
                gridView1.DeleteSelectedRows();
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string guid = gridView2.GetFocusedRowCellValue("GUID/IP/UID").ToString();
            string time = gridView2.GetFocusedRowCellValue("到期日期").ToString().Trim();
            string reason = gridView2.GetFocusedRowCellValue("原因").ToString();
            if (localBans.ContainsKey(guid))
            {
                XtraMessageBox.Show("已经存在了:" + guid, "不能添加重复的信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LocalBansEntity local = new LocalBansEntity(guid, time.Equals("永久封禁") ? "-1" : time, reason, "", "");
            localBans.Add(guid,local );
            localBansDataTable.Rows.Add(local.GUID, time.Equals("-1") ? "永久封禁" : time, local.Reason);
            if (SaveToBans())
            {
                gridControl1.ForceInitialize();
            }
        }

        private void gridControl2_Click(object sender, EventArgs e)
        {
            popupMenu2.HidePopup();
        }

        private void gridView2_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == GridMenuType.Row)
            {
                popupMenu2.ShowPopup(MousePosition);
            }
        }
    }
}
