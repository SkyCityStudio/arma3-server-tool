using a3.Config;
using a3.Dialog;
using a3.Entity;
using a3.Tools;
using Arma3ServerTools.Dialog;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace a3.Modules
{
    public partial class ModSettingsUserControl : DevExpress.DXperience.Demos.TutorialControlBase
    {
        private DataTable dataTable = new DataTable();
        public ModSettingsUserControl()
        {
            InitializeComponent();

            dataTable.Columns.Add("更新选中模组", typeof(bool));
            dataTable.Columns.Add("文件夹名", typeof(string));
            dataTable.Columns.Add("模组名", typeof(string));
            dataTable.Columns.Add("模组ID", typeof(long));
            dataTable.Columns.Add("客户端模组", typeof(bool));
            dataTable.Columns.Add("服务器模组", typeof(bool));
            dataTable.Columns.Add("无头客户端模组", typeof(bool));
            dataTable.Columns.Add("本地添加的模组", typeof(bool));
            dataTable.Columns.Add("模组路径", typeof(string));
            dataTable.Columns.Add("更新时间", typeof(string));

        }

        public ModsEntity GetMod(string modName)
        {
            for (int i = 0; i < DefaultConfig.DefaultServer.StartupParameters.modsEntities.Count; i++)
            {
                if (DefaultConfig.DefaultServer.StartupParameters.modsEntities[i].ModPath == modName)
                {
                    return DefaultConfig.DefaultServer.StartupParameters.modsEntities[i];
                }
            }
            return null;
        }


        public void UserControl_Init() {
            if (!string.IsNullOrEmpty(DefaultConfig.steamcmd.d) && DefaultConfig.ModuleScanPath.Count == 0 && Directory.Exists(DefaultConfig.steamcmd.d + @"\steamapps\workshop\content\107410"))
            {
                DefaultConfig.ModuleScanPath.Add(new ModuleScanPathEntity(DefaultConfig.steamcmd.d + @"\steamapps\workshop\content\107410", "", "自动设置的steamcmd模组路径"));
            }
            else if (!string.IsNullOrEmpty(DefaultConfig.steamcmd.d) && DefaultConfig.ModuleScanPath.Count > 0 && Directory.Exists(DefaultConfig.steamcmd.d + @"\steamapps\workshop\content\107410"))
            {
                bool ExistsCmd = false;
                DefaultConfig.ModuleScanPath.ForEach(x => {
                    if (x.ModulePath == DefaultConfig.steamcmd.d + @"\steamapps\workshop\content\107410")
                    {
                        ExistsCmd = true;
                        return;
                    }
                });
                if (!ExistsCmd)
                {
                    DefaultConfig.ModuleScanPath.Add(new ModuleScanPathEntity(DefaultConfig.steamcmd.d + @"\steamapps\workshop\content\107410", "", "自动设置的steamcmd模组路径"));
                }
            }


            dataTable.Rows.Clear();
 
            DefaultConfig.ModuleScanPath.ForEach(module => {
                if (Directory.Exists(module.ModulePath))
                {
                    List<string> dir = FileTools.getDir(module.ModulePath, module.Prefix);
                    DefaultConfig.DefaultServer.StartupParameters.modsEntities.ForEach(mod => {
                        if (Directory.Exists(mod.ModPath) && !dir.Contains(mod.ModPath))
                        {
                            dir.Add(mod.ModPath);
                        }
                    });
                    for (int i = 0; i < dir.Count; i++)
                    {
                        ModsEntity entity = GetMod(dir[i]);
                        ModMeta meta = FileTools.getModMeta(dir[i]);
                        string time = "";
                        string name = "";
                        long id = 0;
                        try
                        {
                            if (null != meta)
                            {
                                if (meta.Name != String.Empty)
                                {
                                    name = meta.Name;
                                }
                                else
                                {
                                    if (entity != null)
                                    {
                                        name = entity.ModName;
                                    }
                                }

                                if (meta.PublishedId != 0)
                                {
                                    id = meta.PublishedId;
                                }
                                else
                                {
                                    if (entity != null)
                                    {
                                        id = entity.ModId;
                                    }

                                }
                                if (meta.TimeStamp != 0)
                                {
                                    DateTime localDate2 = DateTime.FromBinary(meta.TimeStamp);
                                    time = localDate2.ToString();
                                }
                                else
                                {
                                    time = "";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            XtraMessageBox.Show(ex.Message, "通知", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        }

                        if (null != entity)
                        {
                            dataTable.Rows.Add(false, entity.ModDirName, name, id, entity.LocalMod, entity.ServerMod, entity.HeadlessClientMod, entity.InputLocalMod, entity.ModPath, time);
                        }
                        else
                        {
                            dataTable.Rows.Add(false, FileTools.getDirName(dir[i]), name, id, false, false, false, false, dir[i], time);
                        }

                    }
                }
            });

            gridControl1.DataSource = dataTable;
            gridControl1.ForceInitialize();
            if (dataTable.Rows.Count != 0)
            {
                gridView1.Columns[0].ToolTip = "选择需要更新的模组";
                gridView1.Columns[1].ToolTip = "模组的文件夹名";
                gridView1.Columns[2].ToolTip = "模组的名字";
                gridView1.Columns[3].ToolTip = "模组的创意工坊物品ID";
                gridView1.Columns[4].ToolTip = "是否作为客户端模组挂载";
                gridView1.Columns[5].ToolTip = "是否作为服务器模组挂载";
                gridView1.Columns[6].ToolTip = "是否作为无头客户端的模组挂载";
                gridView1.Columns[7].ToolTip = "是否是本地导入的模组";
                gridView1.Columns[8].ToolTip = "模组的绝对路径";
                gridView1.Columns[9].ToolTip = "模组的更新时间";
                gridView1.Columns[1].OptionsColumn.AllowEdit = false;
                gridView1.Columns[2].OptionsColumn.AllowEdit = false;
                gridView1.Columns[3].OptionsColumn.AllowEdit = false;
                gridView1.Columns[7].OptionsColumn.AllowEdit = false;
                gridView1.Columns[8].OptionsColumn.AllowEdit = false;
                gridView1.Columns[9].OptionsColumn.AllowEdit = false;
            }
            toggleSwitch1.IsOn = DefaultConfig.DefaultServer.AutoCopyBikey;
            toggleSwitch5.IsOn = DefaultConfig.DefaultServer.StartupParameters.DLCWS;
            toggleSwitch6.IsOn = DefaultConfig.DefaultServer.StartupParameters.DLCVN;
            toggleSwitch4.IsOn = DefaultConfig.DefaultServer.StartupParameters.DLCCSLA;
            toggleSwitch3.IsOn = DefaultConfig.DefaultServer.StartupParameters.DLCGM;
            toggleSwitch2.IsOn = DefaultConfig.DefaultServer.StartupParameters.DLCcontact;
        }


        public void UserControl_Save()
        {
            ModSettingsUserControl_Leave(null, null);
        }
        private void ModSettingsUserControl_Leave(object sender, EventArgs e)
        {
            DefaultConfig.DefaultServer.StartupParameters.modsEntities.Clear();
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                string DirPath = gridView1.GetRowCellValue(i, "模组路径").ToString();
                string ModDirName = gridView1.GetRowCellValue(i, "文件夹名").ToString();
                string ModeName = gridView1.GetRowCellValue(i, "模组名").ToString();
                long ModId = RegularMatchTool.ToLong(gridView1.GetRowCellValue(i, "模组ID").ToString(), 0);
                bool ClientMod = MissionsTool.getBoolean(gridView1.GetRowCellValue(i, "客户端模组").ToString());
                bool ServerMod = MissionsTool.getBoolean(gridView1.GetRowCellValue(i, "服务器模组").ToString());
                bool HCMod = MissionsTool.getBoolean(gridView1.GetRowCellValue(i, "无头客户端模组").ToString());
                bool InputMod = MissionsTool.getBoolean(gridView1.GetRowCellValue(i, "本地添加的模组").ToString());
                DefaultConfig.DefaultServer.StartupParameters.modsEntities.Add(new ModsEntity(DirPath, ModDirName,ModeName, ModId, ClientMod, ServerMod, HCMod, InputMod));
            }
            DefaultConfig.DefaultServer.StartupParameters.DLCWS = toggleSwitch5.IsOn;
            DefaultConfig.DefaultServer.StartupParameters.DLCVN = toggleSwitch6.IsOn;
            DefaultConfig.DefaultServer.StartupParameters.DLCCSLA = toggleSwitch4.IsOn;
            DefaultConfig.DefaultServer.StartupParameters.DLCGM = toggleSwitch3.IsOn;
            DefaultConfig.DefaultServer.StartupParameters.DLCcontact = toggleSwitch2.IsOn;

            DefaultConfig.DefaultServer.SetTime();
            FileTools.SaveConfig(DefaultConfig.DefaultServer.ServerUUID);
        }


        private void gridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            DataRowView data = (DataRowView)e.Row;
            object[] vs =  data.Row.ItemArray;
            if (vs[3].ToString() == "0" && (bool)vs[0] == true) {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("无法更新的模组在第 " + (e.RowHandle + 1) +" 行\r\n");
                stringBuilder.AppendLine("模组路径:" + vs[8].ToString());
                stringBuilder.AppendLine("文件夹名:" + vs[1].ToString());
                stringBuilder.AppendLine("模组名:" + vs[2].ToString());
                stringBuilder.AppendLine("模组ID:" + vs[3].ToString());
                stringBuilder.AppendLine("客户端模组:" + ((bool)vs[4] ? "是" : "否"));
                stringBuilder.AppendLine("服务器模组:" + ((bool)vs[5] ? "是" : "否"));
                stringBuilder.AppendLine("无头客户端模组:" + ((bool)vs[6] ? "是" : "否"));
                stringBuilder.AppendLine("本地添加的模组:" + ((bool)vs[7] ? "是" : "否"));
                stringBuilder.AppendLine("更新时间:" + vs[9].ToString());
                XtraMessageBox.Show("你选择了一个无法更新的模组!因为它的模组ID为空!更新时将忽略它!\r\n"+ stringBuilder.ToString(), "通知", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                stringBuilder = null;
            }



        }

        private void gridView1_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            try
            {
                DataRowView data = (DataRowView)e.Row;
                object[] vs = data.Row.ItemArray;
                if (Directory.Exists(vs[8].ToString()))
                {
                    if (DefaultConfig.DefaultServer.AutoCopyBikey)
                    {
                        FileTools.bikey.Clear();
                        FileTools.GetBikey(new DirectoryInfo(vs[8].ToString()));
                        FileTools.bikey.ForEach(bikey =>
                        {
                            try
                            {
                                string KeyPath = DefaultConfig.DefaultServer.ServerDir + @"\Keys\" + vs[1].ToString().Replace(" ", "_").Replace("@", "") + "-" + bikey.Name.Replace(" ", "_").Replace("bikey", "").Replace(".", "") + bikey.Extension;
                                if ((bool)vs[4])
                                {
                                    File.Copy(bikey.FullName, KeyPath, true);
                                }
                                else if (!(bool)vs[4] && File.Exists(KeyPath)) 
                                { 
                                   File.Delete(KeyPath);
                                }
                            }
                            catch { }
                        });
                    }
                }
            }
            catch
            {
            }
        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            XtraDialog.Show(new BikeyListDialog(), "管理Bikey", MessageBoxButtons.OK);
        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {
            if (xtraFolderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (Directory.Exists(xtraFolderBrowserDialog1.SelectedPath) && Directory.Exists(xtraFolderBrowserDialog1.SelectedPath + @"\addons"))
                {
                    for (int i = 0; i < gridView1.DataRowCount; i++)
                    {
                        string DirPath = gridView1.GetRowCellValue(i, "模组路径").ToString();
                        if (DirPath == xtraFolderBrowserDialog1.SelectedPath) {
                            XtraMessageBox.Show("你选择的模组已经存在!", "模组已存在", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    if (DefaultConfig.ModuleScanPath.Count == 0) {
                        XtraMessageBox.Show("你成功添加了一个本地模组，但是你没有配置模组扫描路径!请配置扫描路径后才能显示!", "模组已添加但是没有扫描模组路径", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    }
                    DefaultConfig.DefaultServer.StartupParameters.modsEntities.Add(new ModsEntity(xtraFolderBrowserDialog1.SelectedPath, FileTools.getDirName(xtraFolderBrowserDialog1.SelectedPath), FileTools.getDirName(xtraFolderBrowserDialog1.SelectedPath), 0, false, false, false, true));
                    UserControl_Init();
                }
                else
                {
                    XtraMessageBox.Show("你选择的模组没有检测到ARMA3的模组文件，请你重新选择真实存在的模组目录!", "找不到模组", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(DefaultConfig.steamcmd.d) || String.IsNullOrEmpty(DefaultConfig.steamcmd.u) || String.IsNullOrEmpty(DefaultConfig.steamcmd.p))
            {
                XtraMessageBox.Show("请先配置steamcmd信息后重试!", "提示!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            if (xtraOpenFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                DefaultConfig.HtmlMods.Clear();
                try
                {
                    FileStream fs = new FileStream(xtraOpenFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs, Encoding.Default);
                    string mods = sr.ReadToEnd();
                    fs.Close();
                    sr.Close();
                    Regex r = new Regex("\\bid=.*?\\d\\b");
                    MatchCollection matches = r.Matches(mods);
                    foreach (Match match in matches)
                    {
                        string id = match.Value.Replace("id=", "").Trim();
                        if (!DefaultConfig.HtmlMods.Contains(id)) { 
                            DefaultConfig.HtmlMods.Add(id);
                        }
                    }
                    if (DefaultConfig.HtmlMods.Count <= 0) {
                        XtraMessageBox.Show("没有从文件中读取到模组的ID!", "读取失败!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        return;
                    }
                    if (SteamcmdTools.DownloadMod()==0)
                    {
                        XtraMessageBox.Show("启动MOD更新/下载模块失败!", "错误!", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    }
                }
                catch {
                    XtraMessageBox.Show("读取和解析文件失败!确保文件是否正确!", "出错!", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                }
            }
        }


        private void simpleButton7_Click(object sender, EventArgs e)
        {
            UserControl_Init();
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DefaultConfig.steamcmd.d))
            {
                if (!SteamcmdTools.SetSteamcmdInfo())
                {
                    return;
                }
            }

            ModuleScanPathDialog scanPathDialog = new ModuleScanPathDialog();
            if (DevExpress.XtraEditors.XtraDialog.Show(scanPathDialog, "配置模组扫描路径", MessageBoxButtons.OKCancel) == DialogResult.OK) {
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "moduleScanPath.json";
                string data = JsonConversionTool.ObjectToJson(DefaultConfig.ModuleScanPath);
                File.WriteAllText(path, data, CfgTool.UTF8);
                UserControl_Init();
            }
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(DefaultConfig.steamcmd.d) || String.IsNullOrEmpty(DefaultConfig.steamcmd.u) || String.IsNullOrEmpty(DefaultConfig.steamcmd.p))
            {
                XtraMessageBox.Show("请先配置steamcmd信息后重试!", "提示!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            DefaultConfig.HtmlMods.Clear();
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                string Select = gridView1.GetRowCellValue(i, "更新选中模组").ToString();
                string Path = gridView1.GetRowCellValue(i, "模组路径").ToString();
                string ModName = gridView1.GetRowCellValue(i, "模组名").ToString();
                
                long ModId = RegularMatchTool.ToLong(gridView1.GetRowCellValue(i, "模组ID").ToString(), 0);
                if (MissionsTool.getBoolean(Select) && ModId != 0) {
                    if (!DefaultConfig.HtmlMods.Contains(ModId.ToString()))
                    {
                        if (!Path.Contains(DefaultConfig.steamcmd.d + @"\steamapps\workshop\content\107410")) {
                            stringBuilder.Append("模组名:").AppendLine(ModName).Append("模组路径:").AppendLine(Path).AppendLine();
                        }
                        DefaultConfig.HtmlMods.Add(ModId.ToString());
                    }
                }
            }

            if (!string.IsNullOrEmpty(stringBuilder.ToString())) {
                if (XtraMessageBox.Show("你选择的这些模组中它本来的路径不在您设置的steamcmd安装路径之中，这会导致重新下载一份模组到你设置的steamcmd模组安装路径中，继续此操作吗?\r\n" + stringBuilder.ToString(), "注意!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) {
                    return;
                }
            }
            if (SteamcmdTools.DownloadMod()==0)
            {
                XtraMessageBox.Show("启动MOD更新/下载模块失败!", "错误!", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();
            if (data.GetDataPresent(DataFormats.Text))
            {
                DefaultConfig.HtmlMods.Clear();
                string Jdata = (string)data.GetData(DataFormats.Text);
                Regex r = new Regex("\\bid=.*?\\d\\b");
                MatchCollection matches = r.Matches(Jdata);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        string id = match.Value.Replace("id=", "").Trim();
                        if (!DefaultConfig.HtmlMods.Contains(id))
                        {
                            DefaultConfig.HtmlMods.Add(id);
                        }
                    }
                }
                else
                {
                    try { 
                        DefaultConfig.HtmlMods.Add(long.Parse(Jdata).ToString());
                    }
                    catch {
                        XtraMessageBox.Show("你复制的不是有效的模组URL地址或模组的ID!识别失败!", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (SteamcmdTools.DownloadMod() == 0)
                {
                    XtraMessageBox.Show("启动MOD更新/下载模块失败!", "错误!", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("读取剪辑版内容失败!剪辑版中的内容并不是文本!", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void gridControl1_Load(object sender, EventArgs e)
        {
 
           
        }

        private void toggleSwitch1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            DefaultConfig.DefaultServer.AutoCopyBikey = e.NewValue.ToString().ToLower() == "true";
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            ModSettingsUserControl_Leave(null,null);
        }

        private void toggleSwitch5_Toggled(object sender, EventArgs e)
        {
            CheckDLC(toggleSwitch5, "WS");
        }

        private void toggleSwitch6_Toggled(object sender, EventArgs e)
        {
            CheckDLC(toggleSwitch6, "vn");
        }

        private void toggleSwitch4_Toggled(object sender, EventArgs e)
        {
            CheckDLC(toggleSwitch4, "CSLA");
        }

        private void toggleSwitch3_Toggled(object sender, EventArgs e)
        {
            CheckDLC(toggleSwitch3, "GM");
        }

        private void toggleSwitch2_Toggled(object sender, EventArgs e)
        {
            CheckDLC(toggleSwitch2, "Contact");
        }

        private void CheckDLC(ToggleSwitch tg,string dlc) {
            if (string.IsNullOrEmpty(DefaultConfig.DefaultServer.ServerDir))
            {
                tg.IsOn = false;
                XtraMessageBox.Show("请在基本设置中设置好服务端的路径后再次尝试设置此项!", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Directory.Exists(DefaultConfig.DefaultServer.ServerDir + @"\"+ dlc+@"\addons"))
            {
                tg.IsOn = false;
                XtraMessageBox.Show("你的服务端目录里没有此DLC!", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
