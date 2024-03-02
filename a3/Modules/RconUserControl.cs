using a3.Config;
using a3.Dialog;
using a3.Entity;
using a3.Tools;
using Arma3ServerTools.Entity;
using DevExpress.Skins;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace a3.Modules
{
    public partial class RconUserControl : DevExpress.DXperience.Demos.TutorialControlBase
    {
        private string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "rcon.json";
        public RconUserControl()
        {
            InitializeComponent();
            try
            {
                if (File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fs, CfgTool.UTF8);
                    string json = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();
                    DefaultConfig.RconMap = new JavaScriptSerializer().Deserialize<Dictionary<string, RconEntity>>(json);
                    foreach (var rcon in DefaultConfig.RconMap) {
                        RconModules r = new RconModules();
                        if (r.setId(rcon.Key))
                        {
                            XtraTabPage xtraTab = new XtraTabPage();
                            xtraTab.Text = rcon.Value.ServerName;
                            xtraTab.Tag = rcon.Key;
                            r.Dock = DockStyle.Fill;
                            xtraTab.Controls.Add(r);
                            xtraTabControl1.TabPages.Add(xtraTab);
                        }
                    }
                }
            }
            catch { }
        }

        private void xtraTabControl1_CloseButtonClick(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show("你确定要删除此配置吗?", "提示!", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes) {

                ClosePageButtonEventArgs e1 = (ClosePageButtonEventArgs)e;
                XtraTabPage page = (XtraTabPage)e1.Page;
                if (DefaultConfig.RconMap.Remove(page.Tag.ToString())) {
                    xtraTabControl1.TabPages.Remove(page);
                    SaveRconInfo();
                }
            }
        }

        private void xtraTabControl1_CustomHeaderButtonClick(object sender, DevExpress.XtraTab.ViewInfo.CustomHeaderButtonEventArgs e)
        {
            if (e.Button.Tag.ToString() == "add") {
                AddRconInfoDialog addRconInfo = new AddRconInfoDialog();
                if (XtraDialog.Show(addRconInfo, "创建Rcon管理", MessageBoxButtons.OKCancel) == DialogResult.OK) {
                    if (addRconInfo.port == 0 || string.IsNullOrEmpty(addRconInfo.ip) || string.IsNullOrEmpty(addRconInfo.password) || string.IsNullOrEmpty(addRconInfo.name))
                    {
                        XtraMessageBox.Show("请将表单输入完整!", "提示!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string uid = System.Guid.NewGuid().ToString("N");
                    RconEntity rconEntity = new RconEntity(addRconInfo.name, addRconInfo.ip, addRconInfo.port, addRconInfo.password, uid);
                    DefaultConfig.RconMap.Add(uid, rconEntity);
                    RconModules rcon = new RconModules();
                    if (!rcon.setId(uid)) {
                        DefaultConfig.RconMap.Remove(uid);
                        XtraMessageBox.Show("创建失败!创建时发生错误!请重试并且确保参数正确！", "提示!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    XtraTabPage xtraTab = new XtraTabPage();
                    xtraTab.Text = addRconInfo.name;
                    xtraTab.Tag = uid;

                    rcon.Dock = DockStyle.Fill;
                    xtraTab.Controls.Add(rcon);
                    xtraTabControl1.TabPages.Add(xtraTab);
                    SaveRconInfo();
                }
            } else if (e.Button.Tag.ToString() == "save") {
                SaveRconInfo();
            }
        }
        private void SaveRconInfo() {
            string Json = JsonConversionTool.ObjectToJson(DefaultConfig.RconMap);
            File.WriteAllText(path, Json, CfgTool.UTF8);
        }

    }
}
