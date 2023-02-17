using a3.Config;
using a3.Entity;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace a3.Dialog
{
    public partial class ModuleScanPathDialog : DevExpress.XtraEditors.XtraUserControl
    {
        private DataTable dataTable = new DataTable();
        public ModuleScanPathDialog()
        {
            InitializeComponent();
            dataTable.Columns.Add("模组扫描路径", typeof(string));
            dataTable.Columns.Add("模组前缀", typeof(string));
            dataTable.Columns.Add("备注", typeof(string));
        }

        public void InitData() {
            dataTable.Clear();
            DefaultConfig.ModuleScanPath.ForEach(x => {
                dataTable.Rows.Add(x.ModulePath, (string.IsNullOrEmpty(x.Prefix) ? "无前缀" : x.Prefix), x.Remark);
            });
            gridControl1.DataSource = dataTable;
            gridControl1.ForceInitialize();
            if (dataTable.Rows.Count != 0)
            {
                gridView1.Columns[0].OptionsColumn.AllowEdit = false;
                gridView1.Columns[1].OptionsColumn.AllowEdit = false;
                gridView1.Columns[2].OptionsColumn.AllowEdit = false;
            }
        }
        private void gridControl1_Load(object sender, EventArgs e)
        {
            InitData();
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            popupMenu1.HidePopup();
            popupMenu2.HidePopup();
        }

        private void gridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {
                popupMenu1.ShowPopup(MousePosition);
            }
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.User)
            {
                popupMenu2.ShowPopup(MousePosition);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string str1 = gridView1.GetFocusedRowCellValue("模组扫描路径").ToString();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = str1,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string str1 = gridView1.GetFocusedRowCellValue("模组扫描路径").ToString();
            if (str1 == DefaultConfig.steamcmd.d + @"\steamapps\workshop\content\107410") {
                XtraMessageBox.Show("此路径无法删除!因为它是配置好的默认模组安装路径!", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (XtraMessageBox.Show("你确定要删除吗?\r\n扫描路径:"+ str1, "操作提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes) {

                ModuleScanPathEntity sc = null;
                DefaultConfig.ModuleScanPath.ForEach(x => {
                    if (x.ModulePath == str1) {
                        sc = x;
                        return;
                    }
                });
                if (sc!=null) {
                    DefaultConfig.ModuleScanPath.Remove(sc);
                    InitData();
                }
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string path = XtraInputBox.Show("输入新的模组扫描路径", "添加扫描路径", "");
            if (!Directory.Exists(path))
            {
                XtraMessageBox.Show("路径不存在!请输入正确的路径!", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string Prefix = XtraInputBox.Show("输入需要匹配模组的前缀，例如:@ 将扫描以@开头的模组文件夹。如果为空它将扫描所有文件夹", "设置扫描前缀", "");
            string Remark = XtraInputBox.Show("输入这个路径的备注，养成添加备注的好习惯!1-100字符。", "设置备注", "");
            if (String.IsNullOrEmpty(Remark) && Remark.Length>100) {
                XtraMessageBox.Show("备注不能为空!且只允许1-100字符!", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DefaultConfig.ModuleScanPath.ForEach(x => {
                if (x.ModulePath == path)
                {
                    XtraMessageBox.Show("路径已经存在!", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            });
            ModuleScanPathEntity pathEntity = new ModuleScanPathEntity(path, Prefix, Remark);
            DefaultConfig.ModuleScanPath.Add(pathEntity);
            InitData();
        }
    }
}
