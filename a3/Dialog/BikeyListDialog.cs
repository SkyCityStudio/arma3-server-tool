using a3.Config;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arma3ServerTools.Dialog
{
    public partial class BikeyListDialog : DevExpress.XtraEditors.XtraUserControl
    {
        private DataTable dataTable = new DataTable();


        public BikeyListDialog()
        {
            InitializeComponent();
            dataTable.Columns.Add("Bikey文件名", typeof(string));
            dataTable.Columns.Add("路径", typeof(string));
        }

        private void ReLoadBikey() {
            dataTable.Rows.Clear();
            string path = DefaultConfig.DefaultServer.ServerDir + @"\Keys";
            if (!Directory.Exists(path))
            {
                return;
            }
            List<FileInfo> files = FileTools.getFile(path, ".bikey");
            for (int i = 0; i < files.Count; i++)
            {
                dataTable.Rows.Add(files[i].Name, files[i].FullName);
            }
            gridControl1.DataSource = dataTable;
            gridControl1.ForceInitialize();
        }

        private void gridControl1_Load(object sender, EventArgs e)
        {
            ReLoadBikey();
            if (dataTable.Rows.Count != 0)
            {
                gridView1.Columns[0].ToolTip = "签名档文件名";
                gridView1.Columns[1].ToolTip = "签名档路径";
                gridView1.Columns[0].OptionsColumn.AllowEdit = false;
                gridView1.Columns[1].OptionsColumn.AllowEdit = false;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            ReLoadBikey();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string path = gridView1.GetFocusedRowCellValue("路径").ToString();
            if (File.Exists(path)) { 
                File.Delete(path);
                ReLoadBikey();
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = DefaultConfig.DefaultServer.ServerDir+@"\Keys",
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }

        private void gridView1_Click(object sender, EventArgs e)
        {

        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            popupMenu1.HidePopup();
        }

        private void gridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Row)
            {
                popupMenu1.ShowPopup(MousePosition);
            }
        }
    }
}
