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
    public partial class NetworkSettingsUserControl : DevExpress.DXperience.Demos.TutorialControlBase
    {
        public NetworkSettingsUserControl()
        {
            InitializeComponent();


        }

        public void UserControl_Init() {
            //加载默认网络设置
            textEdit1.Text = DefaultConfig.DefaultServer.StartupParameters.Port.ToString();
            toggleSwitch3.IsOn = DefaultConfig.DefaultServer.ServerConfig.UPNP;
            spinEdit5.Value = DefaultConfig.DefaultServer.BasicConfig.MaxMsgSend;
            spinEdit6.Value = DefaultConfig.DefaultServer.BasicConfig.MaxSizeGuaranteed;
            spinEdit7.Value = DefaultConfig.DefaultServer.BasicConfig.MaxSizeNonguaranteed;
            spinEdit8.Value = DefaultConfig.DefaultServer.BasicConfig.MinBandwidth;
            spinEdit9.Value = DefaultConfig.DefaultServer.BasicConfig.MaxBandwidth;
            spinEdit10.Value = (decimal)DefaultConfig.DefaultServer.BasicConfig.MinErrorToSend;
            spinEdit11.Value = (decimal)DefaultConfig.DefaultServer.BasicConfig.MinErrorToSendNear;
            spinEdit12.Value = DefaultConfig.DefaultServer.BasicConfig.MaxPacketSize;
            spinEdit13.Value = DefaultConfig.DefaultServer.BasicConfig.MaxCustomFileSize;
            spinEdit14.Value = DefaultConfig.DefaultServer.ServerConfig.SteamProtocolMaxDataSize;
            toggleSwitch2.IsOn = DefaultConfig.DefaultServer.StartupParameters.BandwidthAlg;
            toggleSwitch1.IsOn = DefaultConfig.DefaultServer.ServerConfig.LoopBack;
            spinEdit1.Value = DefaultConfig.DefaultServer.ServerConfig.DisconnectTimeout;
            spinEdit2.Value = DefaultConfig.DefaultServer.ServerConfig.Maxdesync;
            spinEdit3.Value = DefaultConfig.DefaultServer.ServerConfig.MaxPing;
            spinEdit4.Value = DefaultConfig.DefaultServer.ServerConfig.MaxPacketLoss;
        }

        public void UserControl_Save()
        {
            NetworkSettingsUserControl_Leave(null, null);
        }
        private void NetworkSettingsUserControl_Leave(object sender, EventArgs e)
        {
            //保存默认网络设置
            DefaultConfig.DefaultServer.StartupParameters.Port = RegularMatchTool.ToInteger(textEdit1.Text, 2302);
            DefaultConfig.DefaultServer.ServerConfig.UPNP = toggleSwitch3.IsOn;
            DefaultConfig.DefaultServer.BasicConfig.MaxMsgSend = (int)spinEdit5.Value;
            DefaultConfig.DefaultServer.BasicConfig.MaxSizeGuaranteed = (int)spinEdit6.Value;
            DefaultConfig.DefaultServer.BasicConfig.MaxSizeNonguaranteed = (int)spinEdit7.Value;
            DefaultConfig.DefaultServer.BasicConfig.MinBandwidth = (long)spinEdit8.Value;
            DefaultConfig.DefaultServer.BasicConfig.MaxBandwidth = (long)spinEdit9.Value;
            DefaultConfig.DefaultServer.BasicConfig.MinErrorToSend = (double)spinEdit10.Value;
            DefaultConfig.DefaultServer.BasicConfig.MinErrorToSendNear = (double)spinEdit11.Value;
            DefaultConfig.DefaultServer.BasicConfig.MaxPacketSize = (int)spinEdit12.Value;
            DefaultConfig.DefaultServer.BasicConfig.MaxCustomFileSize = (int)spinEdit13.Value;
            DefaultConfig.DefaultServer.ServerConfig.SteamProtocolMaxDataSize = (int)spinEdit14.Value;
            DefaultConfig.DefaultServer.StartupParameters.BandwidthAlg = toggleSwitch2.IsOn;
            DefaultConfig.DefaultServer.ServerConfig.LoopBack = toggleSwitch1.IsOn;
            DefaultConfig.DefaultServer.ServerConfig.DisconnectTimeout = (int)spinEdit1.Value;
            DefaultConfig.DefaultServer.ServerConfig.Maxdesync = (int)spinEdit2.Value;
            DefaultConfig.DefaultServer.ServerConfig.MaxPing = (int)spinEdit3.Value;
            DefaultConfig.DefaultServer.ServerConfig.MaxPacketLoss = (int)spinEdit4.Value;
            DefaultConfig.DefaultServer.SetTime();
            FileTools.SaveConfig(DefaultConfig.DefaultServer.ServerUUID);
        }
    }
}
