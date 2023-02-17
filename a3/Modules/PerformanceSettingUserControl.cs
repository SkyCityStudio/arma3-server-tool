using a3.Config;
using a3.Tools;
using DevExpress.XtraEditors;
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
    public partial class PerformanceSettingUserControl : DevExpress.DXperience.Demos.TutorialControlBase
    {
        public PerformanceSettingUserControl()
        {
            InitializeComponent();
        }

        public void UserControl_Init() {
            toggleSwitch1.IsOn = DefaultConfig.DefaultServer.StartupParameters.EnableHT;
            toggleSwitch3.IsOn = DefaultConfig.DefaultServer.StartupParameters.Hugepages;
            toggleSwitch2.IsOn = DefaultConfig.DefaultServer.StartupParameters.LoadMissionToMemory;
            toggleSwitch4.IsOn = DefaultConfig.DefaultServer.StartupParameters.DisableServerThread;
            spinEdit2.Value = DefaultConfig.DefaultServer.StartupParameters.CpuCount;
            spinEdit3.Value = DefaultConfig.DefaultServer.StartupParameters.ExThreads;
            spinEdit4.Value = DefaultConfig.DefaultServer.StartupParameters.MaxMem;
            spinEdit5.Value = DefaultConfig.DefaultServer.StartupParameters.LimitFPS;
            spinEdit6.Value = DefaultConfig.DefaultServer.BasicConfig.TerrainGrid;
            spinEdit1.Value = DefaultConfig.DefaultServer.BasicConfig.ViewDistance;
        }

        private void PerformanceSettingUserControl_Load(object sender, EventArgs e)
        {
            Console.WriteLine(1);
        }

        public void UserControl_Save()
        {
            PerformanceSettingUserControl_Leave(null, null);
        }
        private void PerformanceSettingUserControl_Leave(object sender, EventArgs e)
        {
            DefaultConfig.DefaultServer.StartupParameters.EnableHT = toggleSwitch1.IsOn;
            DefaultConfig.DefaultServer.StartupParameters.Hugepages = toggleSwitch3.IsOn;
            DefaultConfig.DefaultServer.StartupParameters.LoadMissionToMemory = toggleSwitch2.IsOn;
            DefaultConfig.DefaultServer.StartupParameters.DisableServerThread = toggleSwitch4.IsOn;
            DefaultConfig.DefaultServer.StartupParameters.CpuCount = (int)spinEdit2.Value;
            DefaultConfig.DefaultServer.StartupParameters.ExThreads = (int)spinEdit3.Value;
            DefaultConfig.DefaultServer.StartupParameters.MaxMem = (int)spinEdit4.Value;
            DefaultConfig.DefaultServer.StartupParameters.LimitFPS = (int)spinEdit5.Value;
            DefaultConfig.DefaultServer.BasicConfig.TerrainGrid = (int)spinEdit6.Value;
            DefaultConfig.DefaultServer.BasicConfig.ViewDistance = (int)spinEdit1.Value;
            DefaultConfig.DefaultServer.SetTime();
            FileTools.SaveConfig(DefaultConfig.DefaultServer.ServerUUID);
        }

    }
}
