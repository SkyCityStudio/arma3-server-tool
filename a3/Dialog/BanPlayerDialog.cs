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

namespace a3.Dialog
{
    public partial class BanPlayerDialog : DevExpress.XtraEditors.XtraUserControl
    {
        public string Reason = "管理员封禁";
        public bool GUID;
        public bool IP;
        public bool BanType = false;
        public int Time = 1;
        public int Date = 1;

        public BanPlayerDialog()
        {
            InitializeComponent();
        }

        private void radioGroup1_Click(object sender, EventArgs e)
        {
            
        }

        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioGroup1.SelectedIndex == 1)
            {
                BanType = true;
                spinEdit1.Enabled = true;
                mruEdit1.Enabled = true;
            }
            else
            {
                BanType = false;
                spinEdit1.Enabled = false;
                mruEdit1.Enabled = false;
            }

        }

        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {
            Reason = textEdit1.Text;
        }

        private void checkEdit1_CheckStateChanged(object sender, EventArgs e)
        {
            GUID = checkEdit1.CanSelect;
        }

        private void checkEdit2_CheckStateChanged(object sender, EventArgs e)
        {
            IP = checkEdit2.CanSelect;
        }



        private void spinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if ((int)spinEdit1.Value <= 0)
            {
                Time = 1;
            }
            else
            {
                Time = (int)spinEdit1.Value;
            }
            
        }

        private void mruEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
             * 分钟
                小时
                天
                周
                月
             */
            switch (mruEdit1.SelectedIndex)
            {
                    case 1:
                    Date = Time * 60;
                    break;
                    case 2:
                    Date = Time * 1440;
                    break;
                    case 3:
                    Date = Time * 10080;
                    break;
                    case 4:
                    Date = Time * 43200;
                    break;
                    case 5:
                    Date = Time * 525600;
                    break;
            }
        }
    }
}
