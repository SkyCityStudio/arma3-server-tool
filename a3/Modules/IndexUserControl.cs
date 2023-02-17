using a3.Config;
using a3.Entity;
using a3.Extension;
using a3.Tools;
using a3.Window;
using DevExpress.XtraEditors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace a3.Modules
{
    public partial class IndexUserControl : DevExpress.DXperience.Demos.TutorialControlBase
    {
        private DataTable dataTable = new DataTable();
        public IndexUserControl()
        {
            InitializeComponent();
            dataTable.Columns.Add("配置名", typeof(string));
            dataTable.Columns.Add("文件名", typeof(string));
            dataTable.Columns.Add("最后保存", typeof(string));
            dataTable.Columns.Add("创建时间", typeof(string));

            //string str = Convert.ToBase64String(Encoding.Default.GetBytes("singleVoice=0;\r\nmaxSamplesPlayed=96;\r\nbattleyeLicense=1;\r\nsceneComplexity=1000000;\r\nshadowZDistance=0;\r\npreferredObjectViewDistance=900;\r\npipViewDistance=500;\r\nvolumeCD=10;\r\nvolumeFX=10;\r\nvolumeSpeech=10;\r\nvolumeVoN=10;\r\nvonRecThreshold=0.029999999;\r\nvolumeMapDucking=1;"));

            //Console.WriteLine(str);
        }

        public void UserControl_Init() {
            string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config";
            if (!Directory.Exists(path))//判断是否存在
            {
                return;
            }
            List<FileInfo> files = FileTools.getFile(path, ".json");
            for (int i = 0; i < files.Count; i++) {
                try
                {
                    FileStream fs = new FileStream(files[i].FullName, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs, CfgTool.UTF8);
                    string json = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();

                    ArmaServerConfig config = new JavaScriptSerializer().Deserialize<ArmaServerConfig>(json);
                    bool AddDataTable = true;
                    for (int j = 0; j < gridView1.DataRowCount; j++)
                    {
                        string DirPath = gridView1.GetRowCellValue(j, "文件名").ToString();
                        if (DirPath == config.ServerUUID+ ".json") {
                            AddDataTable = false;
                            break;
                        }
                    }
                    if (AddDataTable) {
                        dataTable.Rows.Add(config.ConfigName, config.ServerUUID + ".json", config.SaveTime + "", config.CreateTime + "");
                        gridControl1.DataSource = dataTable;
                        gridControl1.ForceInitialize();
                    }
                    if (!DefaultConfig.ServerList.ContainsKey(config.ServerUUID)) {
                        DefaultConfig.ServerList.Add(config.ServerUUID, config);
                    }
                }
                catch (Exception e) {
                    XtraMessageBox.Show( "读取["+ files[i].Name+ "]服务器配置文件时发生错误!\r\n配置文件位于:" + path + "\r\n该文件是服务器配置文件，如果无法修复请删除它!\r\n错误信息:" + e.Message, "发生错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (dataTable.Rows.Count != 0)
            {
                gridControl1.DataSource = dataTable;
                gridControl1.ForceInitialize();
                gridView1.Columns[0].OptionsColumn.AllowEdit = false;
                gridView1.Columns[1].OptionsColumn.AllowEdit = false;
                gridView1.Columns[2].OptionsColumn.AllowEdit = false;
                gridView1.Columns[3].OptionsColumn.AllowEdit = false;
            }
        }
        

        private void IndexUserControl1_Load(object sender, EventArgs e)
        {

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string name = gridView1.GetFocusedRowCellValue("文件名").ToString().Replace(".json","");
            if (!DefaultConfig.ServerList.ContainsKey(name))
            {
                XtraMessageBox.Show( "读取选择的配置失败!在服务器列表中找不到它!", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else {
                DefaultConfig.DefaultServer = DefaultConfig.ServerList[name];
                FluentDesignForm.CurrentConfigTup.Caption = "当前配置:" + DefaultConfig.DefaultServer.ConfigName;
            }
        }

        private void gridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {

                popupMenu1.ShowPopup(MousePosition);

            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            popupMenu1.HidePopup();
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string name = gridView1.GetFocusedRowCellValue("文件名").ToString().Replace(".json", "");

            if (!DefaultConfig.ServerList.ContainsKey(name)) {
                XtraMessageBox.Show("读取选择的配置失败!在服务器列表中找不到它!", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string path = DefaultConfig.ServerList[name].ServerDir + @"\destiny_serverconfig\" + name;
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = path,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string ConfigName = gridView1.GetFocusedRowCellValue("配置名").ToString();
            string FieName = gridView1.GetFocusedRowCellValue("文件名").ToString();
            string LastSave = gridView1.GetFocusedRowCellValue("最后保存").ToString();
            string CreateTime = gridView1.GetFocusedRowCellValue("创建时间").ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append("配置名:").AppendLine(ConfigName);
            sb.Append("文件名:").AppendLine(FieName);
            sb.Append("最后保存:").AppendLine(LastSave);
            sb.Append("创建时间:").AppendLine(CreateTime);
            if (XtraMessageBox.Show("你确定要删除选中的配置吗?删除操作立即执行，不可恢复!\r\n"+ sb.ToString(), "确认操作", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config";
                try
                {
                    string serverKey = FieName.Replace(".json", "");
                    if (DefaultConfig.ServerList.ContainsKey(serverKey)) {
                        bool ok = DefaultConfig.ServerList.Remove(serverKey);
                    }
                    if (DefaultConfig.DefaultServer.ServerUUID == serverKey)
                    {
                        DefaultConfig.DefaultServer = new ArmaServerConfig();
                        FluentDesignForm.CurrentConfigTup.Caption = "当前配置:没有保存的配置";
                    }
                    File.Delete(path + @"\" + FieName);
                    gridView1.DeleteRow(gridView1.GetFocusedDataSourceRowIndex());
                    UserControl_Init();
                }
                catch {
                    XtraMessageBox.Show("删除选择的配置失败!!", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            sb.Clear();
        }
        //服务器管理
        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string name = gridView1.GetFocusedRowCellValue("文件名").ToString().Replace(".json", "");
            
          
            
            if (!DefaultConfig.ServerList.ContainsKey(name))
            {
                XtraMessageBox.Show("读取选择的配置失败!在服务器列表中找不到它!", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } else
            {
                string title = "ARMA3服务器管理-" + DefaultConfig.ServerList[name].ServerConfig.HostName + "-" + DefaultConfig.ServerList[name].ServerUUID;
                int Handler = ExtensionTools.FindServerWindow(null, title);
                if (Handler != 0) {
                    XtraMessageBox.Show("检测到此服务器的管理界面已经创建!为了防止多窗口操作数据库造成混乱或数据不同步请不要尝试打开多个窗口！", "无法再次打开", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                ServerStatisticsManagement server = new ServerStatisticsManagement();
                server.serverConfig = DefaultConfig.ServerList[name];
                server.Text = title;
                server.Init();
                server.Show();
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            SteamcmdTools.InstallUpdateArma3Server(DefaultConfig.steamcmd.i);
        }


        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string name = gridView1.GetFocusedRowCellValue("文件名").ToString().Replace(".json", "");
            string path = DefaultConfig.ServerList[name].ServerDir;
            SteamcmdTools.InstallUpdateArma3Server(path);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            SteamcmdTools.SetSteamcmdInfo();
        }
    }
}
