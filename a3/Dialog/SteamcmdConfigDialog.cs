using a3.Config;
using a3.Tools;
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

namespace a3.Dialog
{
    public partial class SteamcmdConfigDialog : DevExpress.XtraEditors.XtraUserControl
    {
        public SteamcmdConfigDialog()
        {
            InitializeComponent();
            if (!String.IsNullOrEmpty(DefaultConfig.steamcmd.d) && !String.IsNullOrEmpty(DefaultConfig.steamcmd.u) && !String.IsNullOrEmpty(DefaultConfig.steamcmd.p)) { 
                textEdit1.Text = DefaultConfig.steamcmd.u;
                textEdit2.Text = DefaultConfig.steamcmd.p;
                textEdit3.Text = DefaultConfig.steamcmd.d;
                textEdit4.Text = DefaultConfig.steamcmd.i;
            }
        }

        private void textEdit3_Click(object sender, EventArgs e)
        {
            if (xtraFolderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (Directory.Exists(xtraFolderBrowserDialog1.SelectedPath))
                {
                    textEdit3.Text = xtraFolderBrowserDialog1.SelectedPath;
                    DefaultConfig.steamcmd.d = xtraFolderBrowserDialog1.SelectedPath;
                }
                else
                {
                    XtraMessageBox.Show("你选择的目录不存在!", "找不到目录", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
            }
        }

        private void textEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            DefaultConfig.steamcmd.u = e.NewValue.ToString();
        }

        private void textEdit2_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            DefaultConfig.steamcmd.p = e.NewValue.ToString();
        }

        private void textEdit4_Click(object sender, EventArgs e)
        {
            if (xtraFolderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                if (Directory.Exists(xtraFolderBrowserDialog1.SelectedPath))
                {
                    textEdit4.Text = xtraFolderBrowserDialog1.SelectedPath;
                    DefaultConfig.steamcmd.i = xtraFolderBrowserDialog1.SelectedPath;
                }
                else
                {
                    XtraMessageBox.Show("你选择的目录不存在!", "找不到目录", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
            }
        }
        public void SaveConfig() {
            string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "data.json";
            string data = JsonConversionTool.ObjectToJson(DefaultConfig.steamcmd);
            string d = Security.EncryptByAES(data, DefaultConfig.mx);
            File.WriteAllText(path, d, CfgTool.UTF8);
            DirectoryInfo dir = new DirectoryInfo(DefaultConfig.steamcmd.d);
            try
            {
                dir.CreateSubdirectory(@"steamapps\workshop\content\107410");
            }
            catch { }
        }
    }
}
