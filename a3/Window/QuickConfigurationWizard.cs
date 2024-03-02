using a3.Config;
using a3.Dialog;
using a3.Modules;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arma3ServerTools.Window
{
    public partial class QuickConfigurationWizard : DevExpress.XtraEditors.XtraForm
    {
        public QuickConfigurationWizard()
        {
            InitializeComponent();
        }

        private void welcomeWizardPage1_PageInit(object sender, EventArgs e)
        {
            welcomeWizardPage1.Controls.Add(steamcmd);
            steamcmd.Dock = DockStyle.Fill;

        }

        private void wizardPage1_PageInit(object sender, EventArgs e)
        {
            textEdit2.Text = DefaultConfig.DefaultServer.ServerConfig.HostName;
            textEdit1.Text = String.IsNullOrEmpty(DefaultConfig.DefaultServer.ServerDir) ? "" : DefaultConfig.DefaultServer.ServerDir;
            toggleSwitch1.IsOn = DefaultConfig.DefaultServer.ServerConfig.BattlEye;
        }

        private void textEdit1_Click(object sender, EventArgs e)
        {
            
        }

        private void textEdit2_EditValueChanged(object sender, EventArgs e)
        {
            DefaultConfig.DefaultServer.ServerConfig.HostName = textEdit2.Text;
        }

        private void toggleSwitch1_Toggled(object sender, EventArgs e)
        {
           DefaultConfig.DefaultServer.ServerConfig.BattlEye = toggleSwitch1.IsOn;
        }

        private void welcomeWizardPage1_PageCommit(object sender, EventArgs e)
        {
            
        }

        private void wizardControl1_NextClick(object sender, DevExpress.XtraWizard.WizardCommandButtonClickEventArgs e)
        {
            switch (e.Page.Tag) {
                case "SET1":
                    if (string.IsNullOrEmpty(DefaultConfig.steamcmd.u) || string.IsNullOrEmpty(DefaultConfig.steamcmd.p) || string.IsNullOrEmpty(DefaultConfig.steamcmd.i) || string.IsNullOrEmpty(DefaultConfig.steamcmd.d))
                    {
                        XtraMessageBox.Show("请先配置完整steam信息后进行下一步!", "步骤未完成", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Handled = true;
                        return;
                    }
                    steamcmd.SaveConfig();
                    textEdit1.Text = DefaultConfig.steamcmd.i;
                    DefaultConfig.DefaultServer.ServerDir = DefaultConfig.steamcmd.i;
                    break;
                case "SET2":
                    if (string.IsNullOrEmpty(textEdit1.Text) || string.IsNullOrEmpty(textEdit2.Text))
                    {
                        XtraMessageBox.Show("请先配置好再进行下一步!", "步骤未完成", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Handled = true;
                        return;
                    }
                    break;
                case "SET3":
                    mission.UserControl_Save();
                    break;
                case "SET4":
                    modSettings.UserControl_Save();
                    break;
            }

        }
        private SteamcmdConfigDialog steamcmd = new SteamcmdConfigDialog();
        private MissionSettingUserControl mission = new MissionSettingUserControl();
        private ModSettingsUserControl modSettings = new ModSettingsUserControl();
        private ModuleScanPathDialog moduleScan = new ModuleScanPathDialog();

        private void wizardPage2_PageInit(object sender, EventArgs e)
        {
            wizardPage2.Controls.Add(mission);
            mission.UserControl_Init();
            mission.Dock = DockStyle.Fill;
        }

        private void wizardPage3_PageInit(object sender, EventArgs e)
        {
            wizardPage3.Controls.Add(modSettings);
            modSettings.UserControl_Init();
            modSettings.Dock = DockStyle.Fill;
        }

        private void wizardPage4_PageInit(object sender, EventArgs e)
        {
            wizardPage4.Controls.Add(moduleScan);
            moduleScan.Dock = DockStyle.Fill;
        }

        private void imageSlider1_CurrentImageIndexChanged(object sender, DevExpress.XtraEditors.Controls.ImageSliderCurrentImageIndexChangedEventArgs e)
        {
            SetInfo(e.CurrentIndex);
        }

        private void SetInfo(int CurrentIndex) {
            string Tip = "";
            switch (CurrentIndex)
            {
                case 0:
                    Tip = "第1张图片:1.保存配置:当你全部设置好后，点击图中箭头位置的保存设置，这时会弹出一个输入框，输入配置的昵称(随便输入)，点击确定即可!";
                    break;
                case 1:
                    Tip = "第2张图片:2.启动服务器:点击图中的 ‘服务器’，然后右键你保存的配置(昵称就是上个步骤你输入的)，选择 ‘服务器管理’ 打开服务器管理界面。";
                    break;
                case 2:
                    Tip = "第3张图片:3.启动服务器:点击启动服务器即可立即启动服务器，如果启动时服务端报错，请检查你是否安装了VC运行库合集和DirectX";
                    break;
            }
            labelControl5.Text = Tip;
        }

        private void completionWizardPage1_PageInit(object sender, EventArgs e)
        {
            SetInfo(0);
        }
    }
}