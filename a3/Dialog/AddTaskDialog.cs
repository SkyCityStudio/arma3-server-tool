using CronExpressionDescriptor;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace a3.Dialog
{
    public partial class AddTaskDialog : DevExpress.XtraEditors.XtraUserControl
    {

        public string Cron;
        public int EnableCron;
        public string SysRemark;
        public int CronAction;
        public string CronActionText = "重启服务器";
        public AddTaskDialog()
        {
            InitializeComponent();
        }



        private void toggleSwitch1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            EnableCron = e.NewValue.ToString().ToLower()=="true"?1:0;
            Console.WriteLine("<<<<" + e.NewValue.ToString().ToLower());
            Console.WriteLine("<<<<"+EnableCron);
        }

        private void textEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            SysRemark = e.NewValue.ToString();
        }

        private void comboBoxEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            string msg;
            try
            {
                msg = ExpressionDescriptor.GetDescription(e.NewValue.ToString(), new Options
                {
                    Verbose = true,
                    Locale = "zh-hans"
                });
            }
            catch {
                msg = string.Empty;
            }
            textEdit1.Text = msg;
            SysRemark = msg;
            Cron = e.NewValue.ToString();
        }

        private void comboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CronAction = comboBoxEdit2.SelectedIndex;
            CronActionText = comboBoxEdit2.Text;
        }
    }
}
