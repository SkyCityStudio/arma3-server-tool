using a3.Config;
using Arma3ServerTools.Entity;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace a3.Dialog
{
    public partial class AddRconInfoDialog : DevExpress.XtraEditors.XtraUserControl
    {
        public string ip;
        public int port=2210;
        public string password;
        public string name = "娱乐至上";
        public AddRconInfoDialog()
        {
            InitializeComponent();
        }

        public void InitByID(string id) {
            DefaultConfig.RconMap.TryGetValue(id,out RconEntity r);
            if (r!=null) {
                ip = r.IP;
                port = r.Port;
                password = r.Password;
                name = r.ServerName;
                textEdit1.Text = ip;
                spinEdit1.Value = port;
                textEdit2.Text = password;
                textEdit3.Text = name;
            }
        }

        private void textEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            ip = e.NewValue.ToString();
        }

        private void spinEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {

            if (spinEdit1.Value<99999) {
                port = (int)spinEdit1.Value;
            }
           
        }

        private void textEdit2_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            password = e.NewValue.ToString();
        }

        private void textEdit3_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            name = e.NewValue.ToString();
        }
    }
}
