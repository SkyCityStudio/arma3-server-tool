using a3.Config;
using a3.Dialog;
using a3.Entity;
using a3.Modules;
using a3.test;
using a3.Tools;
using a3.Window;
using Arma3ServerTools.Entity;
using Arma3ServerTools.Window;
using CronExpressionDescriptor;
using DevExpress.DXperience.Demos;
using DevExpress.Snap.Core.API;
using DevExpress.Xpf.Core;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;


namespace a3
{
    public partial class FluentDesignForm : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        public static BarStaticItem SaveInfoTip = null;
        public static BarStaticItem CurrentConfigTup = null;
        public string NavName = String.Empty;
        public FluentDesignForm()
        {
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(a3.test.WaitForm1));
            InitializeComponent();

            Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");
            Match m = RegCHZN.Match(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            if (m.Success) {
                XtraMessageBox.Show("你当前的开服工具路径里包含中文，这会导致一系列问题，请确保安装路径里不包含中文!", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }


            HttpHelper httpHelper = new HttpHelper();

			/*
            Task.Run(()=> {
                try
                {
                    httpHelper.DoGet("" + Convert.ToBase64String(Encoding.Default.GetBytes(DefaultConfig.mx)), null);
                }
                catch { }
            });*/


            try
            {
                
                string version = httpHelper.DoGet("http://tools.destiny.cool/arma3_server_tools/version.txt", null);
                if (string.IsNullOrEmpty(version) || version != DefaultConfig.Version)
                {
                    XtraMessageBox.Show("检测更新失败!请前往 https://destiny.cool/s/arma3-tool 下载最新版本!或QQ群文件:977432417\r\n1.您仍可以继续使用此版本，但是可能伴随着些许错误或问题！\r\n2.您可以按上述方法下载最新版本，可能解决了您提交的问题！", "失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else {
                    /*
                    string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                    if (!DefaultConfig.Version.Equals(version)) {
                        Process.Start("https://destiny.cool/arma3-tool");
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            Arguments = DefaultConfig.Version,
                            FileName = path + "AppUpdate.exe"
                        };
                        Process.Start(startInfo);
                        Process.GetCurrentProcess().Kill();
                    }*/
                }
                
            }
            catch {
                XtraMessageBox.Show("检测更新失败!请前往 https://destiny.cool/s/arma3-tool 下载最新版本!或QQ群文件:977432417\r\n1.您仍可以继续使用此版本，但是可能伴随着些许错误或问题！\r\n2.您可以按上述方法下载最新版本，可能解决了您提交的问题！", "失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            DefaultConfig.StartIScheduler();
            SaveInfoTip = barStaticItem2;
            CurrentConfigTup = barStaticItem1;
            barStaticItem2.Caption = DefaultConfig.DefaultServer.SaveTime;

            ProcessCommunication process = new ProcessCommunication();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            process.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            process.Show();



            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();

            Task.Factory.StartNew(() => {
                while (true)
                {
                    Thread.Sleep(60000);
                    GC.Collect();
                }
            });

        }


        public void LoadSkin() {
            string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            try
            {
                if (File.Exists(path + "data.json"))
                {
                    FileStream fs = new FileStream(path + "data.json", FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs, CfgTool.UTF8);
                    string json = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();
                    json = Security.DecryptByAES(json, DefaultConfig.mx);
                    DefaultConfig.steamcmd = new JavaScriptSerializer().Deserialize<SteamcmdEntity>(json);
                }
            }
            catch { 

            }
            try
            {
                if (File.Exists(path + "skin.json"))
                {
                    FileStream fs = new FileStream(path + "skin.json", FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs, CfgTool.UTF8);
                    string json = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();
                    SkinEntity Skin = new JavaScriptSerializer().Deserialize<SkinEntity>(json);
                    DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(Skin.ActiveSkinName, Skin.ActiveSvgPaletteName);
                }
            }
            catch { }
            try
            {
                if (File.Exists(path + "moduleScanPath.json"))
                {
                    FileStream fs = new FileStream(path + "moduleScanPath.json", FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs, CfgTool.UTF8);
                    string json = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();
                    DefaultConfig.ModuleScanPath = new JavaScriptSerializer().Deserialize<List<ModuleScanPathEntity>>(json);
                }
            }
            catch { }



            try
            {
                if (File.Exists(path + "bans.json"))
                {
                    FileStream fs = new FileStream(path + "bans.json", FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs, CfgTool.UTF8);
                    string json = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();
                    DefaultConfig.bansUrlEntities = new JavaScriptSerializer().Deserialize<List<BansUrlEntity>>(json);
                }
            }
            catch { }
        }


        private bool TaskDone = true;



        async Task TryExecuteModuleMethodInvoker(string moduleName,string targetMethod)
        {
            ModuleInfo module  = ModulesInfo.GetItem(moduleName);
            await Task.Factory.StartNew(() =>
            {
                if (!fluentDesignFormContainer1.Controls.ContainsKey(module.Name))
                {
                    TutorialControlBase controlBase = module.TModule as TutorialControlBase;
                    if (controlBase != null)
                    {
                        fluentDesignFormContainer1.Invoke(new MethodInvoker(delegate ()
                        {
                            ExectueTargetMethod(controlBase, targetMethod);
                        }));
                    }
                }
                else
                {
                    var controlBase = fluentDesignFormContainer1.Controls.Find(module.Name, true);
                    if (controlBase.Length == 1)
                    {
                        fluentDesignFormContainer1.Invoke(new MethodInvoker(delegate () {
                            ExectueTargetMethod(controlBase[0], targetMethod);
                        }));
                    }
                }
            });
        }

        async Task LoadModuleAsync(ModuleInfo module)
        {
            if (!TaskDone) {
                return;
            }
            TaskDone = false;
            await Task.Factory.StartNew(() =>
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(a3.test.WaitForm1));
                if (!fluentDesignFormContainer1.Controls.ContainsKey(module.Name))
                {
                    TutorialControlBase controlBase = module.TModule as TutorialControlBase;
                    if (controlBase != null)
                    {
                        controlBase.Dock = DockStyle.Fill;
                        controlBase.CreateWaitDialog();
                        fluentDesignFormContainer1.Invoke(new MethodInvoker(delegate ()
                        {
                            fluentDesignFormContainer1.Controls.Add(controlBase);
                            controlBase.BringToFront();
                            ExectueTargetMethod(controlBase, "UserControl_Init");
                        }));
                    }
                }
                else
                {
                    var controlBase = fluentDesignFormContainer1.Controls.Find(module.Name, true);
                    if (controlBase.Length == 1)
                    {
                        fluentDesignFormContainer1.Invoke(new MethodInvoker(delegate () {
                            controlBase[0].BringToFront();
                            ExectueTargetMethod(controlBase[0], "UserControl_Init");
                        }));
                    }
                }
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                TaskDone = true;
            });
        }
        public void ExectueTargetMethod<T>(T controlBase,string motedName) {
            fluentDesignFormContainer1.Invoke(new MethodInvoker(delegate () {
                Type type = controlBase.GetType();
                MethodInfo targetMethod = type.GetMethod(motedName);
                if (targetMethod != null)
                {
                    targetMethod.Invoke(controlBase, null);
                }
            }));
        }

        

        private async void FluentDesignForm_Load(object sender, EventArgs e)
        {

            Text = "武装突袭3 开服工具 测试版 V" + DefaultConfig.Version;
            LoadSkin();
            //this.fluentDesignFormContainer1.Controls.Add(new IndexUserControl() { Dock = DockStyle.Fill });
            this.ItemNav.Caption = "服务器列表";

            if (ModulesInfo.GetItem("IndexUserControl") == null)
            {
                ModulesInfo.Add(new ModuleInfo("IndexUserControl", "a3.Modules.IndexUserControl"));
            }
            NavName = "IndexUserControl";
            await LoadModuleAsync(ModulesInfo.GetItem(NavName));

        }
        private async void accordionControlElement1_Click(object sender, EventArgs e)
        {

            this.ItemNav.Caption = "服务器列表";
            if (ModulesInfo.GetItem("IndexUserControl") == null) 
            {
                ModulesInfo.Add(new ModuleInfo("IndexUserControl", "a3.Modules.IndexUserControl"));
            }
            NavName = "IndexUserControl";
            await LoadModuleAsync(ModulesInfo.GetItem(NavName));
        }

        private async void accordionControlElementServerBasicSettings_Click(object sender, EventArgs e)
        {
            this.ItemNav.Caption = $"{accordionControlElementIndex.Text}/{accordionControlElementServerBasicSettings.Text}";
            if (ModulesInfo.GetItem("BasicSettingUserControl") == null)
            {
                ModulesInfo.Add(new ModuleInfo("BasicSettingUserControl", "a3.Modules.BasicSettingUserControl"));
            }
            NavName = "BasicSettingUserControl";
            await LoadModuleAsync(ModulesInfo.GetItem(NavName));
        }

        private async void accordionControlElementSecuritySettings_Click(object sender, EventArgs e)
        {
            this.ItemNav.Caption = $"{accordionControlElementIndex.Text}/{accordionControlElementSecuritySettings.Text}";
            if (ModulesInfo.GetItem("SecuritySettingsUserControl") == null)
            {
                ModulesInfo.Add(new ModuleInfo("SecuritySettingsUserControl", "a3.Modules.SecuritySettingsUserControl"));
            }
            NavName = "SecuritySettingsUserControl";
            await LoadModuleAsync(ModulesInfo.GetItem(NavName));
        }

        private async void accordionControlElementNetworkSettings_Click(object sender, EventArgs e)
        {
            this.ItemNav.Caption = $"{accordionControlElementIndex.Text}/{accordionControlElementNetworkSettings.Text}";
            if (ModulesInfo.GetItem("NetworkSettingsUserControl") == null)
            {
                ModulesInfo.Add(new ModuleInfo("NetworkSettingsUserControl", "a3.Modules.NetworkSettingsUserControl"));
            }
            NavName = "NetworkSettingsUserControl";
            await LoadModuleAsync(ModulesInfo.GetItem(NavName));
        }

        private async void accordionControlElement2_Click(object sender, EventArgs e)
        {
            this.ItemNav.Caption = $"{accordionControlElementIndex.Text}/{accordionControlElementMissionSetting.Text}";
            if (ModulesInfo.GetItem("MissionSettingUserControl") == null)
            {
                ModulesInfo.Add(new ModuleInfo("MissionSettingUserControl", "a3.Modules.MissionSettingUserControl"));
            }
            NavName = "MissionSettingUserControl";
            await LoadModuleAsync(ModulesInfo.GetItem(NavName));
        }

        private async void accordionControlElementModSettings_Click(object sender, EventArgs e)
        {
            this.ItemNav.Caption = $"{accordionControlElementIndex.Text}/{accordionControlElementModSettings.Text}";
            if (ModulesInfo.GetItem("ModSettingsUserControl") == null)
            {
                ModulesInfo.Add(new ModuleInfo("ModSettingsUserControl", "a3.Modules.ModSettingsUserControl"));
            }
            NavName = "ModSettingsUserControl";
            await LoadModuleAsync(ModulesInfo.GetItem(NavName));
        }

        private async void accordionControlElementDifficultySetting_Click(object sender, EventArgs e)
        {
            this.ItemNav.Caption = $"{accordionControlElementIndex.Text}/{accordionControlElementDifficultySetting.Text}";
            if (ModulesInfo.GetItem("DifficultySettingUserControl") == null)
            {
                ModulesInfo.Add(new ModuleInfo("DifficultySettingUserControl", "a3.Modules.DifficultySettingUserControl"));
            }
            NavName = "DifficultySettingUserControl";
            await LoadModuleAsync(ModulesInfo.GetItem(NavName));
        }

        private async void accordionControlElementPerformanceSettings_Click(object sender, EventArgs e)
        {
            this.ItemNav.Caption = $"{accordionControlElementIndex.Text}/{accordionControlElementPerformanceSettings.Text}";
            if (ModulesInfo.GetItem("PerformanceSettingUserControl") == null)
            {
                ModulesInfo.Add(new ModuleInfo("PerformanceSettingUserControl", "a3.Modules.PerformanceSettingUserControl"));
            }
            NavName = "PerformanceSettingUserControl";
            await LoadModuleAsync(ModulesInfo.GetItem(NavName));
        }

        private async void accordionControlElementLogSettings_Click(object sender, EventArgs e)
        {
            this.ItemNav.Caption = $"{accordionControlElementIndex.Text}/{accordionControlElementLogSettings.Text}";
            if (ModulesInfo.GetItem("LogSettingUserControl") == null)
            {
                ModulesInfo.Add(new ModuleInfo("LogSettingUserControl", "a3.Modules.LogSettingUserControl"));
            }
            NavName = "LogSettingUserControl";
            await LoadModuleAsync(ModulesInfo.GetItem(NavName));
        }

        private async void accordionControlElement3_Click(object sender, EventArgs e)
        {
            this.ItemNav.Caption = "关于Destiny Studio";
            if (ModulesInfo.GetItem("AboutUsControl") == null)
            {
                ModulesInfo.Add(new ModuleInfo("AboutUsControl", "a3.Modules.AboutUsControl"));
            }
            NavName = "AboutUsControl";
            await LoadModuleAsync(ModulesInfo.GetItem(NavName));
        }


        private async void accordionControlElementBans_Click(object sender, EventArgs e)
        {
            this.ItemNav.Caption = "在线联合封禁-封神榜";
            if (ModulesInfo.GetItem("BansUserControl") == null)
            {
                ModulesInfo.Add(new ModuleInfo("BansUserControl", "a3.Modules.BansUserControl"));
            }
            NavName = "BansUserControl";
            await LoadModuleAsync(ModulesInfo.GetItem(NavName));
        }


        private void barStaticItem1_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private async void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DefaultConfig.DefaultServer.ServerDir == null || DefaultConfig.DefaultServer.ServerDir == string.Empty) {
                XtraMessageBox.Show( "请先设置ARMA3服务端目录!它在:基本设置->通用设置->服务端目录", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (DefaultConfig.DefaultServer.ServerUUID == null) {
                string name = XtraInputBox.Show("请输入配置名称，长度不要超过100!", "输入配置昵称","我的服务器配置名称");
                if (string.IsNullOrEmpty(name)) {
                    name = System.Guid.NewGuid().ToString("N");
                }
                if (name.Length>100) {
                    name = name.Substring(0,100);
                }
                string id = Guid.NewGuid().ToString("N");
                DefaultConfig.DefaultServer.ServerUUID = id;
                DefaultConfig.DefaultServer.ConfigName = name;
                DefaultConfig.DefaultServer.CreateTime = DateTime.Now.ToString();
                DefaultConfig.ServerList.Add(id, DefaultConfig.DefaultServer);
            }
            if (!Directory.Exists("config")) {
                Directory.CreateDirectory("config");
            }
            barStaticItem1.Caption = "当前配置:" + DefaultConfig.DefaultServer.ConfigName;
            await TryExecuteModuleMethodInvoker(NavName, "UserControl_Save");
            DefaultConfig.DefaultServer.SetTime();
            FileTools.SaveConfig(DefaultConfig.DefaultServer.ServerUUID);
            CfgTool.SaveCfg();
            /*
            if (DefaultConfig.ServerList.ContainsKey(DefaultConfig.DefaultServer.ServerUUID))
            {
                DefaultConfig.ServerList[DefaultConfig.DefaultServer.ServerUUID] = DefaultConfig.DefaultServer;
            }*/
            TryExecuteModuleMethodInvoker(NavName, "UserControl_Init");
        }

        private void FluentDesignForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SkinEntity skin = new SkinEntity(DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveSkinName, DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveSvgPaletteName);
            string skinJson = JsonConversionTool.ObjectToJson(skin);
            string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "skin.json";
            File.WriteAllText(path, skinJson, CfgTool.UTF8);
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (NavName != "IndexUserControl") {
                XtraMessageBox.Show("请离开当前页面前往服务器列表后再新建配置!", "失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DefaultConfig.DefaultServer = new ArmaServerConfig();
            barStaticItem1.Caption = "当前配置:没有保存的配置";
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            Console.WriteLine(DefaultConfig.DefaultServer.ServerTaskManagement.ProcessById);
        }

        private async void accordionControlElement1_Click_1(object sender, EventArgs e)
        {
            this.ItemNav.Caption = $"{accordionControlElementIndex.Text}/{accordionControlElement1.Text}";
            if (ModulesInfo.GetItem("QuickConfigurationWizardControl") == null)
            {
                ModulesInfo.Add(new ModuleInfo("QuickConfigurationWizardControl", "a3.Modules.QuickConfigurationWizardControl"));
            }
            NavName = "QuickConfigurationWizardControl";
            await LoadModuleAsync(ModulesInfo.GetItem(NavName));
        }

        private async void accordionControlElement2_Click_1(object sender, EventArgs e)
        {
            this.ItemNav.Caption = "RCON管理器";
            if (ModulesInfo.GetItem("RconUserControl") == null)
            {
                ModulesInfo.Add(new ModuleInfo("RconUserControl", "a3.Modules.RconUserControl"));
            }
            NavName = "RconUserControl";
            await LoadModuleAsync(ModulesInfo.GetItem(NavName));
        }
    }
}
