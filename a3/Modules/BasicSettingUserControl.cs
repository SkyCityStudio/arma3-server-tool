using a3.Config;
using a3.Entity;
using a3.Tools;
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
    public partial class BasicSettingUserControl : DevExpress.DXperience.Demos.TutorialControlBase
    {

        public static BasicSettingUserControl basicSettingUserControl = null;
        private void BasicSettingUserControl_Load(object sender, EventArgs e)
        {
            Console.WriteLine(2);
        }

        public void UserControl_Init()
        {
            textEdit3.Text = DefaultConfig.DefaultServer.ServerConfig.HostName;
            textEdit4.Text = DefaultConfig.DefaultServer.ServerConfig.Password;
            spinEdit3.Value = DefaultConfig.DefaultServer.ServerConfig.MaxPlayers;
            toggleSwitch4.IsOn = DefaultConfig.DefaultServer.StartupParameters.AutoInit;
            toggleSwitch3.IsOn = DefaultConfig.DefaultServer.ServerConfig.Persistent;
            toggleSwitch32.IsOn = DefaultConfig.DefaultServer.ServerConfig.SkipLobby;
            toggleSwitch311.IsOn = DefaultConfig.DefaultServer.ServerConfig.DrawingInMap;
            toggleSwitch31.IsOn = DefaultConfig.DefaultServer.ServerConfig.StatisticsEnabled == 1;
            mruEdit1.SelectedIndex = DefaultConfig.DefaultServer.ServerConfig.ForceRotorLibSimulation;
            StringBuilder sb = new StringBuilder();
            DefaultConfig.DefaultServer.ServerConfig.Motd.ForEach(m => {
                if (!String.IsNullOrEmpty(m)) {
                    sb.AppendLine(m);
                }
            });

            textEdit5.Text = String.IsNullOrEmpty(DefaultConfig.DefaultServer.ServerDir) ? "" : DefaultConfig.DefaultServer.ServerDir;
            toggleSwitch1.IsOn = DefaultConfig.DefaultServer.x64;

            memoEdit1.Text = sb.ToString();
            spinEdit1.Value = DefaultConfig.DefaultServer.ServerConfig.MotdInterval;
            textEdit1.Text = DefaultConfig.DefaultServer.StartupParameters.PidFile;
            textEdit2.Text = DefaultConfig.DefaultServer.StartupParameters.Ranking;
            //加载默认语音设置
            toggleSwitch312.IsOn = DefaultConfig.DefaultServer.ServerConfig.DisableVoN == 0;
            spinEdit31.Value = DefaultConfig.DefaultServer.ServerConfig.VonCodecQuality;
            mruEdit11.SelectedIndex = DefaultConfig.DefaultServer.ServerConfig.VonCodec;
            //加载默认无头客户端设置
            Console.WriteLine(">>>"+DefaultConfig.DefaultServer.ServerConfig.HeadlessClients.Count);
            Console.WriteLine(">>>" + listBoxControl1.Items.Count);
            listBoxControl1.Items.Clear();
            DefaultConfig.DefaultServer.ServerConfig.HeadlessClients.ForEach(client => {
                listBoxControl1.Items.Add(client);
            });
            listBoxControl2.Items.Clear();
            DefaultConfig.DefaultServer.ServerConfig.LocalClient.ForEach(client => {
                listBoxControl2.Items.Add(client);
            });
            //加载默认投票设置
            spinEdit2.Value = DefaultConfig.DefaultServer.ServerConfig.VoteThreshold;
            spinEdit4.Value = DefaultConfig.DefaultServer.ServerConfig.VotingTimeOut;
            spinEdit5.Value = DefaultConfig.DefaultServer.ServerConfig.RoleTimeOut;
            spinEdit61.Value = DefaultConfig.DefaultServer.ServerConfig.BriefingTimeOut;
            spinEdit6.Value = DefaultConfig.DefaultServer.ServerConfig.DebriefingTimeOut;
            spinEdit7.Value = DefaultConfig.DefaultServer.ServerConfig.LobbyIdleTimeout;
            spinEdit8.Value = DefaultConfig.DefaultServer.ServerConfig.VoteMissionPlayers;
            //加载默认附加参数
            try
            {
                if (!String.IsNullOrEmpty(DefaultConfig.DefaultServer.ServerConfig.ServerConfigArgs))
                {
                    byte[] outputb = Convert.FromBase64String(DefaultConfig.DefaultServer.ServerConfig.ServerConfigArgs);
                    string orgStr = Encoding.Default.GetString(outputb);
                    memoEdit2.Text = orgStr;
                }
                else {
                    memoEdit2.Text = "";
                }
                if (!String.IsNullOrEmpty(DefaultConfig.DefaultServer.BasicConfig.BasicConfigArgs))
                {
                    byte[] outputb = Convert.FromBase64String(DefaultConfig.DefaultServer.BasicConfig.BasicConfigArgs);
                    string orgStr = Encoding.Default.GetString(outputb);
                    memoEdit3.Text = orgStr;
                }
                else
                {
                    memoEdit3.Text = "";
                }
                if (!String.IsNullOrEmpty(DefaultConfig.DefaultServer.StartupParameters.StartConfigArgs))
                {
                    byte[] outputb = Convert.FromBase64String(DefaultConfig.DefaultServer.StartupParameters.StartConfigArgs);
                    string orgStr = Encoding.Default.GetString(outputb);
                    memoEdit4.Text = orgStr;
                }
                else
                {
                    memoEdit4.Text = "";
                }
                if (!String.IsNullOrEmpty(DefaultConfig.DefaultServer.serverProfile.ServerProfileArgs))
                {
                    byte[] outputb = Convert.FromBase64String(DefaultConfig.DefaultServer.serverProfile.ServerProfileArgs);
                    string orgStr = Encoding.Default.GetString(outputb);
                    memoEdit5.Text = orgStr;
                }
                else
                {
                    memoEdit5.Text = "";
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("加载附加参数时发生错误!请检查附加参数是否正确!\r\n" + ex.Message, "错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }

        }


        public BasicSettingUserControl()
        {
           
            InitializeComponent();
            basicSettingUserControl = this;



            //ArmaServerConfig server = new ArmaServerConfig();
            // string s = JsonConversionTool.ObjectToJson(server);

            // Console.WriteLine(s);

            //Console.WriteLine(System.Guid.NewGuid().ToString("N"));
           
        }



        private void toggleSwitch311_Toggled(object sender, EventArgs e)
        {

        }

        private void mruEdit11_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void simpleButton3_Click(object sender, EventArgs e)
        {
            var result = XtraInputBox.Show("添加一条无头客户端IP地址", "添加IP", "127.0.0.1");
            if (IPv4Tools.ValidateIPAddress(result)) {
                listBoxControl1.Items.Add(result);
            }
            else {
                XtraMessageBox.Show("IP格式有误!", "错误");
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            var result = XtraInputBox.Show("添加一条本地无头客户端IP地址", "添加IP", "127.0.0.1");
            if (IPv4Tools.ValidateIPAddress(result))
            {
                listBoxControl2.Items.Add(result);
            }
            else
            {
                XtraMessageBox.Show("IP格式有误!", "错误");
            }
        }

        private void Root_Click(object sender, EventArgs e)
        {

        }

        public void UserControl_Save() {
            BasicSettingUserControl_Leave(null, null);
        }

        private void BasicSettingUserControl_Leave(object sender, EventArgs e)
        {
            DefaultConfig.DefaultServer.ServerConfig.HostName = textEdit3.Text;
            DefaultConfig.DefaultServer.ServerConfig.Password = textEdit4.Text;
            DefaultConfig.DefaultServer.ServerConfig.MaxPlayers = (int)spinEdit3.Value;

            DefaultConfig.DefaultServer.StartupParameters.AutoInit = toggleSwitch4.IsOn;
            DefaultConfig.DefaultServer.ServerConfig.Persistent = toggleSwitch3.IsOn;
            DefaultConfig.DefaultServer.ServerConfig.SkipLobby = toggleSwitch32.IsOn;
            DefaultConfig.DefaultServer.ServerConfig.DrawingInMap = toggleSwitch311.IsOn;
            

            DefaultConfig.DefaultServer.ServerConfig.ForceRotorLibSimulation = mruEdit1.SelectedIndex;


            DefaultConfig.DefaultServer.ServerConfig.Motd.Clear();
            string[] sArray = Regex.Split(memoEdit1.Text, Environment.NewLine, RegexOptions.IgnoreCase);
            foreach (string i in sArray) {
                if (!string.IsNullOrEmpty(i)) {
                    DefaultConfig.DefaultServer.ServerConfig.Motd.Add(i);
                }
            }
            DefaultConfig.DefaultServer.ServerDir = textEdit5.Text;
            DefaultConfig.DefaultServer.x64 = toggleSwitch1.IsOn;
            DefaultConfig.DefaultServer.ServerConfig.MotdInterval = (int)spinEdit1.Value;
            DefaultConfig.DefaultServer.StartupParameters.PidFile = textEdit1.Text;
            DefaultConfig.DefaultServer.StartupParameters.Ranking = textEdit2.Text;
            //加载默认语音设置
            DefaultConfig.DefaultServer.ServerConfig.DisableVoN = toggleSwitch312.IsOn?0:1;


            DefaultConfig.DefaultServer.ServerConfig.VonCodecQuality = spinEdit31.Value;
            DefaultConfig.DefaultServer.ServerConfig.VonCodec = mruEdit11.SelectedIndex;

            //加载默认无头客户端设置
            DefaultConfig.DefaultServer.ServerConfig.HeadlessClients.Clear();
            for (int i = 0; i < listBoxControl1.Items.Count; i++) {
                DefaultConfig.DefaultServer.ServerConfig.HeadlessClients.Add(listBoxControl1.GetItemText(i));
            }
            DefaultConfig.DefaultServer.ServerConfig.LocalClient.Clear();
            for (int i = 0; i < listBoxControl2.Items.Count; i++)
            {
                DefaultConfig.DefaultServer.ServerConfig.LocalClient.Add(listBoxControl2.GetItemText(i));
            }
            //保存投票设置
            DefaultConfig.DefaultServer.ServerConfig.VoteThreshold = spinEdit2.Value;
            DefaultConfig.DefaultServer.ServerConfig.VotingTimeOut = (int)spinEdit4.Value;
            DefaultConfig.DefaultServer.ServerConfig.RoleTimeOut = (int)spinEdit5.Value;
            DefaultConfig.DefaultServer.ServerConfig.BriefingTimeOut = (int)spinEdit61.Value;
            DefaultConfig.DefaultServer.ServerConfig.DebriefingTimeOut = (int)spinEdit6.Value;
            DefaultConfig.DefaultServer.ServerConfig.LobbyIdleTimeout = (int)spinEdit7.Value;
            DefaultConfig.DefaultServer.ServerConfig.VoteMissionPlayers = (int)spinEdit8.Value;
            DefaultConfig.DefaultServer.ServerConfig.StatisticsEnabled = toggleSwitch31.IsOn ? 1 : 0;
            //保存附加参数
            try
            {
                if (!String.IsNullOrEmpty(memoEdit2.Text))
                {
                    string str = Convert.ToBase64String(Encoding.Default.GetBytes(memoEdit2.Text));
                    DefaultConfig.DefaultServer.ServerConfig.ServerConfigArgs = str;
                }
                if (!String.IsNullOrEmpty(memoEdit3.Text))
                {
                    string str = Convert.ToBase64String(Encoding.Default.GetBytes(memoEdit3.Text));
                    DefaultConfig.DefaultServer.BasicConfig.BasicConfigArgs = str;
                }
                if (!String.IsNullOrEmpty(memoEdit4.Text))
                {
                    string str = Convert.ToBase64String(Encoding.Default.GetBytes(memoEdit4.Text));
                    DefaultConfig.DefaultServer.StartupParameters.StartConfigArgs = str;
                }
                if (!String.IsNullOrEmpty(memoEdit5.Text))
                {
                    string str = Convert.ToBase64String(Encoding.Default.GetBytes(memoEdit5.Text));
                    DefaultConfig.DefaultServer.serverProfile.ServerProfileArgs = str;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("保存附加参数时发生错误!请检查附加参数是否正确!\r\n" + ex.Message, "错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
            DefaultConfig.DefaultServer.SetTime();
            FileTools.SaveConfig(DefaultConfig.DefaultServer.ServerUUID);
        }

        private void textEdit3_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textEdit5_Click(object sender, EventArgs e)
        {
            if (xtraFolderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (Directory.Exists(xtraFolderBrowserDialog1.SelectedPath) && (File.Exists(xtraFolderBrowserDialog1.SelectedPath + @"\arma3server_x64.exe") || File.Exists(xtraFolderBrowserDialog1.SelectedPath + @"\arma3server.exe")))
                {
                    textEdit5.Text = xtraFolderBrowserDialog1.SelectedPath;
                    DefaultConfig.DefaultServer.ServerDir = xtraFolderBrowserDialog1.SelectedPath;
                }
                else {
                    XtraMessageBox.Show("你选择的目录没有ARMA3服务端的可执行文件!请你重新选择带有arma3server_x64.exe的可执行文件的服务端目录!", "找不到文件", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
