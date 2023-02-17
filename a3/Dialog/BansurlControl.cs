using a3.Config;
using a3.Tools;
using Arma3ServerTools.Entity;
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

namespace Arma3ServerTools.Dialog
{
    public partial class BansurlControl : DevExpress.XtraEditors.XtraUserControl
    {
        private DataTable dataTable = new DataTable();
        public BansurlControl()
        {
            InitializeComponent();
            dataTable.Columns.Add("封禁文件Url", typeof(string));
            dataTable.Columns.Add("启用", typeof(bool));
        }


        public void InitData()
        {
            dataTable.Clear();
            DefaultConfig.bansUrlEntities.ForEach(x => {
                dataTable.Rows.Add(x.url,x.enable);
            });
            gridControl1.DataSource = dataTable;
            gridControl1.ForceInitialize();
            if (dataTable.Rows.Count != 0)
            {
                gridView1.Columns[0].OptionsColumn.AllowEdit = false;
            }
        }

        private void gridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {
                popupMenu1.ShowPopup(MousePosition);
            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            popupMenu1.HidePopup();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {

            string url = XtraInputBox.Show("请输入bans.txt的url文件地址!例如:http://tools.arma3bbs.com/arma3_server_tools/bans.txt", "输入地址", "", MessageBoxButtons.OKCancel);

            for (int i=0;i< DefaultConfig.bansUrlEntities.Count;i++) {
                if ("http://tools.arma3bbs.com/arma3_server_tools/bans.txt".Equals(url))
                {
                    XtraMessageBox.Show("不能添加重复的url!", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            try {
                Uri uri = new Uri(url);
            }
            catch {
                XtraMessageBox.Show("地址不正确!", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            BansUrlEntity bansUrl = new BansUrlEntity(url,true);
            DefaultConfig.bansUrlEntities.Add(bansUrl);
            InitData();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string str1 = gridView1.GetFocusedRowCellValue("封禁文件Url").ToString();
            if (str1.Equals("http://tools.arma3bbs.com/arma3_server_tools/bans.txt"))
            {
                XtraMessageBox.Show("此Url无法删除!因为它是配置好的默认URL路径!", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (XtraMessageBox.Show("你确定要这个共享bans url吗?\r\nurl路径:" + str1, "操作提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes)
            {

                BansUrlEntity sc = null;
                DefaultConfig.bansUrlEntities.ForEach(x => {
                    if (x.url == str1)
                    {
                        sc = x;
                        return;
                    }
                });
                if (sc != null)
                {
                    DefaultConfig.bansUrlEntities.Remove(sc);
                    InitData();
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DefaultConfig.bansUrlEntities.Clear();
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                string url = gridView1.GetRowCellValue(i, "封禁文件Url").ToString();
                bool enb = MissionsTool.getBoolean(gridView1.GetRowCellValue(i, "启用").ToString());
                DefaultConfig.bansUrlEntities.Add(new BansUrlEntity(url,enb));
            }

            string bans = JsonConversionTool.ObjectToJson(DefaultConfig.bansUrlEntities);
            string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "bans.json";
            try
            {
                File.WriteAllText(path, bans, CfgTool.UTF8);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("保存失败!" + ex.Message, "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void BansurlControl_Load(object sender, EventArgs e)
        {
            InitData();
        }
    }
}
