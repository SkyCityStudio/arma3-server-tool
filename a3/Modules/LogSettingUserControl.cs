using a3.Config;
using a3.Tools;
using DevExpress.XtraEditors;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace a3.Modules
{
    public partial class LogSettingUserControl : DevExpress.DXperience.Demos.TutorialControlBase
    {
        public LogSettingUserControl()
        {
            InitializeComponent();
            

        }

        public void UserControl_Init() {
            toggleSwitch1.IsOn = DefaultConfig.DefaultServer.StartupParameters.NoLogs;
            toggleSwitch2.IsOn = DefaultConfig.DefaultServer.StartupParameters.Netlog;
            textEdit1.Text = DefaultConfig.DefaultServer.ServerConfig.LogFile;
            comboBoxEdit1.SelectedIndex = DefaultConfig.DefaultServer.ServerConfig.TimeStampFormat;
            spinEdit1.Value = DefaultConfig.DefaultServer.ServerConfig.CallExtReportLimit;
        }

        private void Root_Click(object sender, EventArgs e)
        {

        }

        public void UserControl_Save()
        {
            LogSettingUserControl_Leave(null, null);
        }
        private void LogSettingUserControl_Leave(object sender, EventArgs e)
        {
            DefaultConfig.DefaultServer.StartupParameters.NoLogs = toggleSwitch1.IsOn;
            DefaultConfig.DefaultServer.StartupParameters.Netlog = toggleSwitch2.IsOn;
            DefaultConfig.DefaultServer.ServerConfig.LogFile = textEdit1.Text;
            DefaultConfig.DefaultServer.ServerConfig.TimeStampFormat = comboBoxEdit1.SelectedIndex;
            DefaultConfig.DefaultServer.ServerConfig.CallExtReportLimit = (int)spinEdit1.Value;
            DefaultConfig.DefaultServer.SetTime();
            FileTools.SaveConfig(DefaultConfig.DefaultServer.ServerUUID);
        }
    }
}
