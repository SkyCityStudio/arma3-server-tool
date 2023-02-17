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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace a3.Modules
{
    public partial class SecuritySettingsUserControl : DevExpress.DXperience.Demos.TutorialControlBase
    {
        public SecuritySettingsUserControl()
        {
            InitializeComponent();
        }


        public void UserControl_Init()
        {
            //加载默认安全设置
            toggleSwitch1.IsOn = DefaultConfig.DefaultServer.ServerConfig.BattlEye;
            toggleSwitch111.IsOn = DefaultConfig.DefaultServer.ServerConfig.VerifySignatures;
            toggleSwitch11.IsOn = DefaultConfig.DefaultServer.ServerConfig.Kickduplicate == 0;
            toggleSwitch2.IsOn = DefaultConfig.DefaultServer.StartupParameters.FilePatching;
            mruEdit1.SelectedIndex = DefaultConfig.DefaultServer.ServerConfig.AllowedFilePatching;

            StringBuilder sb = new StringBuilder();
            DefaultConfig.DefaultServer.ServerConfig.FilePatchingExceptions.ForEach(e => {
                sb.AppendLine(e);
            });
            memoEdit1.Text = sb.ToString();
            //加载默认管理员设置
            textEdit1.Text = DefaultConfig.DefaultServer.ServerConfig.ServerCommandPassword;
            textEdit11.Text = DefaultConfig.DefaultServer.ServerConfig.PasswordAdmin;
            sb.Clear();
            DefaultConfig.DefaultServer.ServerConfig.Admins.ForEach(uid => {
                sb.AppendLine(uid);
            });
            memoEdit11.Text = sb.ToString();
            //加载服务器基本脚本设置
            string orgStr;
            try
            {
                orgStr = Encoding.Default.GetString(Convert.FromBase64String(DefaultConfig.DefaultServer.ServerConfig.DoubleIdDetected));
                textEdit2.Text = orgStr;
            }
            catch
            {
                textEdit2.Text = String.Empty;
            }
            try
            {
                orgStr = Encoding.Default.GetString(Convert.FromBase64String(DefaultConfig.DefaultServer.ServerConfig.onUserConnected));
                textEdit3.Text = orgStr;
            }
            catch
            {
                textEdit3.Text = String.Empty;
            }
            try
            {
                orgStr = Encoding.Default.GetString(Convert.FromBase64String(DefaultConfig.DefaultServer.ServerConfig.onUserDisconnected));
                textEdit31.Text = orgStr;
            }
            catch
            {
                textEdit31.Text = String.Empty;
            }
            try
            {
                orgStr = Encoding.Default.GetString(Convert.FromBase64String(DefaultConfig.DefaultServer.ServerConfig.onHackedData));
                textEdit32.Text = orgStr;
            }
            catch
            {
                textEdit32.Text = String.Empty;
            }
            try
            {
                orgStr = Encoding.Default.GetString(Convert.FromBase64String(DefaultConfig.DefaultServer.ServerConfig.onDifferentData));
                textEdit33.Text = orgStr;
            }
            catch
            {
                textEdit33.Text = String.Empty;
            }
            try
            {
                orgStr = Encoding.Default.GetString(Convert.FromBase64String(DefaultConfig.DefaultServer.ServerConfig.onUnsignedData));
                textEdit34.Text = orgStr;
            }
            catch 
            {
                textEdit34.Text = String.Empty;
            }
            try
            {
                orgStr = Encoding.Default.GetString(Convert.FromBase64String(DefaultConfig.DefaultServer.ServerConfig.onUserKicked));
                textEdit35.Text = orgStr;
            }
            catch
            {
                textEdit35.Text = String.Empty;
            }
            try
            {
                orgStr = Encoding.Default.GetString(Convert.FromBase64String(DefaultConfig.DefaultServer.ServerConfig.RegularCheck));
                textEdit36.Text = orgStr;
            }
            catch
            {
                textEdit36.Text = String.Empty;
            }


            /*
            byte[] bytes = Encoding.Default.GetBytes("kick (_this select 0)");
            Console.WriteLine(Convert.ToBase64String(bytes));
            */

            //载入 BattlEye 基本安全规则设置
            textEdit19.Text = DefaultConfig.DefaultServer.BattlEyeConfig.RConPassword;
            spinEdit2.Value = DefaultConfig.DefaultServer.BattlEyeConfig.RConPort;
            spinEdit1.Value = DefaultConfig.DefaultServer.BattlEyeConfig.MaxPing;
            textEdit6.Text = DefaultConfig.DefaultServer.BattlEyeConfig.MaxCreateVehiclePerInterval.Seconds.ToString();
            textEdit12.Text = DefaultConfig.DefaultServer.BattlEyeConfig.MaxCreateVehiclePerInterval.MaxNumbe.ToString();
            textEdit17.Text = DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetPosPerInterval.Seconds.ToString();
            textEdit18.Text = DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetPosPerInterval.MaxNumbe.ToString();
            textEdit4.Text = DefaultConfig.DefaultServer.BattlEyeConfig.MaxDeleteVehiclePerInterval.Seconds.ToString();
            textEdit5.Text = DefaultConfig.DefaultServer.BattlEyeConfig.MaxDeleteVehiclePerInterval.MaxNumbe.ToString();
            textEdit7.Text = DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetDamagePerInterval.Seconds.ToString();
            textEdit8.Text = DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetDamagePerInterval.MaxNumbe.ToString();
            textEdit9.Text = DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddBackpackCargoPerInterval.Seconds.ToString();
            textEdit10.Text = DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddBackpackCargoPerInterval.MaxNumbe.ToString();
            textEdit13.Text = DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddMagazineCargoPerInterval.Seconds.ToString();
            textEdit14.Text = DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddMagazineCargoPerInterval.MaxNumbe.ToString();
            textEdit15.Text = DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddWeaponCargoPerInterval.Seconds.ToString();
            textEdit16.Text = DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddWeaponCargoPerInterval.MaxNumbe.ToString();
            sb.Clear();
            DefaultConfig.DefaultServer.ServerConfig.AllowedLoadFileExtensions.ForEach(f => {
                sb.AppendLine(f);
            });
            memoEdit24.Text = sb.ToString();
            sb.Clear();
            DefaultConfig.DefaultServer.ServerConfig.AllowedPreprocessFileExtensions.ForEach(f => {
                sb.AppendLine(f);
            });
            memoEdit211.Text = sb.ToString();
            sb.Clear();
            DefaultConfig.DefaultServer.ServerConfig.AllowedHTMLLoadExtensions.ForEach(f => {
                sb.AppendLine(f);
            });
            memoEdit221.Text = sb.ToString();
            sb.Clear();
            DefaultConfig.DefaultServer.ServerConfig.AllowedHTMLLoadURIs.ForEach(f => {
                sb.AppendLine(f);
            });
            memoEdit231.Text = sb.ToString();

        }

        private void memoEdit211_EditValueChanged(object sender, EventArgs e)
        {

        }


        public void UserControl_Save()
        {
            SecuritySettingsUserControl_Leave(null, null);
        }
        private void SecuritySettingsUserControl_Leave(object sender, EventArgs e)
        {
            //加载默认安全设置
            DefaultConfig.DefaultServer.ServerConfig.BattlEye = toggleSwitch1.IsOn;
            DefaultConfig.DefaultServer.ServerConfig.VerifySignatures = toggleSwitch111.IsOn;
            DefaultConfig.DefaultServer.ServerConfig.Kickduplicate = toggleSwitch11.IsOn?1:0;

            DefaultConfig.DefaultServer.StartupParameters.FilePatching = toggleSwitch2.IsOn;
            DefaultConfig.DefaultServer.ServerConfig.AllowedFilePatching = mruEdit1.SelectedIndex;

            DefaultConfig.DefaultServer.ServerConfig.FilePatchingExceptions.Clear();
            string[] sArray = Regex.Split(memoEdit1.Text, Environment.NewLine, RegexOptions.IgnoreCase);
            foreach (string i in sArray) {
                if (!string.IsNullOrEmpty(i)) {
                    DefaultConfig.DefaultServer.ServerConfig.FilePatchingExceptions.Add(i);
                }
            }

            //加载默认管理员设置
            DefaultConfig.DefaultServer.ServerConfig.ServerCommandPassword = textEdit1.Text;
            DefaultConfig.DefaultServer.ServerConfig.PasswordAdmin = textEdit11.Text;


            DefaultConfig.DefaultServer.ServerConfig.Admins.Clear();
            sArray = Regex.Split(memoEdit11.Text, Environment.NewLine, RegexOptions.IgnoreCase);
            foreach (string i in sArray) {
                if (!string.IsNullOrEmpty(i)) {
                    DefaultConfig.DefaultServer.ServerConfig.Admins.Add(i);
                }
            }


            //加载服务器基本脚本设置
            try
            {
                byte[] bytes = Encoding.Default.GetBytes(textEdit2.Text);
                DefaultConfig.DefaultServer.ServerConfig.DoubleIdDetected = Convert.ToBase64String(bytes);
            }
            catch 
            {
                DefaultConfig.DefaultServer.ServerConfig.DoubleIdDetected = String.Empty;
            }
            try
            {
                byte[] bytes = Encoding.Default.GetBytes(textEdit3.Text);
                DefaultConfig.DefaultServer.ServerConfig.onUserConnected = Convert.ToBase64String(bytes);
            }
            catch 
            {
                DefaultConfig.DefaultServer.ServerConfig.onUserConnected = String.Empty;
            }

            try
            {
                byte[] bytes = Encoding.Default.GetBytes(textEdit31.Text);
                DefaultConfig.DefaultServer.ServerConfig.onUserDisconnected = Convert.ToBase64String(bytes);
            }
            catch 
            {
                DefaultConfig.DefaultServer.ServerConfig.onUserDisconnected = String.Empty;
            }

            try
            {
                byte[] bytes = Encoding.Default.GetBytes(textEdit32.Text);
                DefaultConfig.DefaultServer.ServerConfig.onHackedData = Convert.ToBase64String(bytes);
            }
            catch 
            {
                DefaultConfig.DefaultServer.ServerConfig.onHackedData = String.Empty;
            }

            try
            {
                byte[] bytes = Encoding.Default.GetBytes(textEdit33.Text);
                DefaultConfig.DefaultServer.ServerConfig.onDifferentData = Convert.ToBase64String(bytes);
            }
            catch 
            {
                DefaultConfig.DefaultServer.ServerConfig.onDifferentData = String.Empty;
            }

            try
            {
                byte[] bytes = Encoding.Default.GetBytes(textEdit34.Text);
                DefaultConfig.DefaultServer.ServerConfig.onUnsignedData = Convert.ToBase64String(bytes);
            }
            catch 
            {
                DefaultConfig.DefaultServer.ServerConfig.onUnsignedData = String.Empty;
            }

            try
            {
                byte[] bytes = Encoding.Default.GetBytes(textEdit35.Text);
                DefaultConfig.DefaultServer.ServerConfig.onUserKicked = Convert.ToBase64String(bytes);
            }
            catch 
            {
                DefaultConfig.DefaultServer.ServerConfig.onUserKicked = String.Empty;
            }

            try
            {
                byte[] bytes = Encoding.Default.GetBytes(textEdit36.Text);
                DefaultConfig.DefaultServer.ServerConfig.RegularCheck = Convert.ToBase64String(bytes);
            }
            catch 
            {
                DefaultConfig.DefaultServer.ServerConfig.RegularCheck = String.Empty;
            }


            //保存 BattlEye 基本安全规则设置
            DefaultConfig.DefaultServer.BattlEyeConfig.RConPassword = textEdit19.Text;
            DefaultConfig.DefaultServer.BattlEyeConfig.RConPort = (int)spinEdit2.Value;
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxPing = (int)spinEdit1.Value;
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxCreateVehiclePerInterval.Seconds = RegularMatchTool.ToInteger(textEdit6.Text, 0); 
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxCreateVehiclePerInterval.MaxNumbe = RegularMatchTool.ToInteger(textEdit12.Text, 0); 
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetPosPerInterval.Seconds = RegularMatchTool.ToInteger(textEdit17.Text, 0); 
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetPosPerInterval.MaxNumbe = RegularMatchTool.ToInteger(textEdit18.Text, 0); 
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxDeleteVehiclePerInterval.Seconds = RegularMatchTool.ToInteger(textEdit4.Text, 0); 
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxDeleteVehiclePerInterval.MaxNumbe = RegularMatchTool.ToInteger(textEdit5.Text, 0); 
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetDamagePerInterval.Seconds = RegularMatchTool.ToInteger(textEdit7.Text, 0); 
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxSetDamagePerInterval.MaxNumbe = RegularMatchTool.ToInteger(textEdit8.Text, 0); 
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddBackpackCargoPerInterval.Seconds = RegularMatchTool.ToInteger(textEdit9.Text, 0); 
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddBackpackCargoPerInterval.MaxNumbe = RegularMatchTool.ToInteger(textEdit10.Text, 0);
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddMagazineCargoPerInterval.Seconds = RegularMatchTool.ToInteger(textEdit13.Text, 0); 
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddMagazineCargoPerInterval.MaxNumbe = RegularMatchTool.ToInteger(textEdit14.Text, 0);
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddWeaponCargoPerInterval.Seconds = RegularMatchTool.ToInteger(textEdit15.Text, 0); 
            DefaultConfig.DefaultServer.BattlEyeConfig.MaxAddWeaponCargoPerInterval.MaxNumbe = RegularMatchTool.ToInteger(textEdit16.Text, 0); 


            DefaultConfig.DefaultServer.ServerConfig.AllowedLoadFileExtensions.Clear();
            sArray = Regex.Split(memoEdit24.Text, Environment.NewLine, RegexOptions.IgnoreCase);
            foreach (string i in sArray) {
                if (!string.IsNullOrEmpty(i)) {
                    DefaultConfig.DefaultServer.ServerConfig.AllowedLoadFileExtensions.Add(i);
                }
            }


            DefaultConfig.DefaultServer.ServerConfig.AllowedPreprocessFileExtensions.Clear();
            sArray = Regex.Split(memoEdit211.Text, Environment.NewLine, RegexOptions.IgnoreCase);
            foreach (string i in sArray) {
                if (!string.IsNullOrEmpty(i))
                {
                    DefaultConfig.DefaultServer.ServerConfig.AllowedPreprocessFileExtensions.Add(i);
                }
            }

            DefaultConfig.DefaultServer.ServerConfig.AllowedHTMLLoadExtensions.Clear();
            sArray = Regex.Split(memoEdit221.Text, Environment.NewLine, RegexOptions.IgnoreCase);
            foreach (string i in sArray) {
                if (!string.IsNullOrEmpty(i))
                {
                    DefaultConfig.DefaultServer.ServerConfig.AllowedHTMLLoadExtensions.Add(i);
                }
            }

            DefaultConfig.DefaultServer.ServerConfig.AllowedHTMLLoadURIs.Clear();
            sArray = Regex.Split(memoEdit231.Text, Environment.NewLine, RegexOptions.IgnoreCase);
            foreach (string i in sArray) {
                if (!string.IsNullOrEmpty(i))
                {
                    DefaultConfig.DefaultServer.ServerConfig.AllowedHTMLLoadURIs.Add(i);
                }
            }
            DefaultConfig.DefaultServer.SetTime();
            FileTools.SaveConfig(DefaultConfig.DefaultServer.ServerUUID);
        }
    }
}
