using a3.Tools;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace a3.Window
{
    public partial class ProcessCommunication : DevExpress.XtraEditors.XtraForm
    {
        public ProcessCommunication()
        {
            InitializeComponent();
            Random ran = new Random();
            Task.Run(() => {
                int wait = ran.Next(2, 3)*1000;
                Thread.Sleep(wait);
                this.Invoke(new Action(() => {
                    labelControl1.Text = "完成...";
                }));
                Thread.Sleep(2000);
                this.Invoke(new Action(() => {
                    this.Hide();
                }));
              
            });
        }

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }
        const int WM_COPYDATA = 0x004A;


        
        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            Console.WriteLine(">>>" + m.Msg);
            switch (m.Msg)
            {
                case WM_COPYDATA:
                    COPYDATASTRUCT mystr = new COPYDATASTRUCT();
                    Type mytype = mystr.GetType();
                    mystr = (COPYDATASTRUCT)m.GetLParam(mytype);
                    Task.Run(() => MonitoringService(mystr.lpData));
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }

        }
        
        public void MonitoringService(string data)
        {
            Console.WriteLine(data);
            Task.Run(() => {
                if (data.Contains("|"))
                {
                    string[] unitObjectsNum = data.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < unitObjectsNum.Length; i++) {
                        string[] args = unitObjectsNum[i].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (args.Length == 10)
                        {
                            switch (args[0])
                            {
                                case "PlayerInfo":
                                    int id = MonitoringServiceSqliteUtils.GetAndCreateServer(args[1]);
                                    if (id != 0)
                                    {
                                        int row = MonitoringServiceSqliteUtils.InsertOrUpdatePlayerInfo(id, args);
                                    }
                                    break;
                            }
                        }
                    }
                }
                else 
                {
                    string[] unitObjectsNum = data.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (unitObjectsNum.Length == 19)
                    {
                        switch (unitObjectsNum[0])
                        {
                            case "ObjectManipulationNum":
                                int id = MonitoringServiceSqliteUtils.GetAndCreateServer(unitObjectsNum[1]);
                                if (id != 0)
                                {
                                    int row = MonitoringServiceSqliteUtils.InsertObjectNum(id, unitObjectsNum);
                                }
                                break;
                        }
                    }
                    if (unitObjectsNum.Length == 4)
                    {
                        switch (unitObjectsNum[0])
                        {
                            case "UpdateOnlineInfo":
                                int id = MonitoringServiceSqliteUtils.GetAndCreateServer(unitObjectsNum[1]);
                                if (id != 0)
                                {
                                    int row = MonitoringServiceSqliteUtils.UpdatePlayerOnlineInfo(id, unitObjectsNum);
                                }
                                break;
                        }
                    }
                }
            });
        }

        private void ProcessCommunication_Load(object sender, EventArgs e)
        {
            labelControl1.BackColor = Color.Transparent;
            labelControl1.Parent = marqueeProgressBarControl1;
        }

        private void labelControl1_Click(object sender, EventArgs e)
        {

        }
    }
}