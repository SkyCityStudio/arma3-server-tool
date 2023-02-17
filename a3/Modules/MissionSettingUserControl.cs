using a3.Config;
using a3.Entity;
using a3.Tools;
using DevExpress.DataAccess.Json;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
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

namespace a3.Modules
{
    public partial class MissionSettingUserControl : DevExpress.DXperience.Demos.TutorialControlBase
    {
        private DataTable dataTable = new DataTable();
        private RepositoryItemComboBox _riEditor = new RepositoryItemComboBox();

        public MissionSettingUserControl()
        {
            InitializeComponent();
            dataTable.Columns.Add("任务名称", typeof(string));
            dataTable.Columns.Add("任务难度", typeof(string));
            dataTable.Columns.Add("任务白名单", typeof(bool));
            dataTable.Columns.Add("选用此任务", typeof(bool));
            _riEditor.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            _riEditor.Items.AddRange(new string[] { "新兵", "正常", "老兵", "自定义" });
        }

        public void UserControl_Init() {
            //加载强制难度
            comboBoxEdit2.SelectedIndex = MissionsTool.DifficultyToInt(DefaultConfig.DefaultServer.ServerConfig.ForcedDifficulty);
            toggleSwitch1.IsOn = DefaultConfig.DefaultServer.ServerConfig.AutoSelectMission;
            i.IsOn = DefaultConfig.DefaultServer.ServerConfig.RandomMissionOrder;
            RefreshMissionsTable();
        }


        public void RefreshMissionsTable()
        {
            //加载并设置任务选中
            dataTable.Rows.Clear();
            
            string path = DefaultConfig.DefaultServer.ServerDir + @"\MPMissions";
            if (!Directory.Exists(path))//判断是否存在
            {
                return;
            }
            List<FileInfo> files = FileTools.getFile(path, ".pbo");
            for (int i = 0; i < files.Count; i++)
            {
                MissionsEntity mission = GetMission(files[i].Name);
                int Difficulty = 3;
                bool WhiteList = false;
                bool Choose = false;
                if (null != mission)
                {
                    Difficulty = mission.Difficulty;
                    WhiteList = mission.WhiteList;
                    Choose = mission.Choose;
                }
                dataTable.Rows.Add(files[i].Name, files[i].Name, WhiteList, Choose);
                gridControl1.DataSource = dataTable;
                gridControl1.ForceInitialize();
                gridView1.SetRowCellValue(i, "任务难度", _riEditor.Items[Difficulty]);
            }
            if (files.Count != 0)
            {
                gridControl1.DataSource = dataTable;
                gridControl1.ForceInitialize();
                gridControl1.RepositoryItems.Add(_riEditor);
                gridView1.Columns[1].ColumnEdit = _riEditor;
                gridView1.Columns[0].OptionsColumn.AllowEdit = false;
            }
        }
        public void UserControl_Save()
        {
            MissionSettingUserControl_Leave(null, null);
        }

        //离开时自动保存设置
        private void MissionSettingUserControl_Leave(object sender, EventArgs e)
        {
            DefaultConfig.DefaultServer.ServerConfig.missions.Clear();
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                string template = gridView1.GetRowCellValue(i, "任务名称").ToString();
                int difficulty = MissionsTool.difficultyNameToInt(gridView1.GetRowCellValue(i, "任务难度").ToString());
                bool whiteList = MissionsTool.getBoolean(gridView1.GetRowCellValue(i, "任务白名单").ToString());
                bool choose = MissionsTool.getBoolean(gridView1.GetRowCellValue(i, "选用此任务").ToString());
                DefaultConfig.DefaultServer.ServerConfig.missions.Add(new MissionsEntity(template, difficulty, whiteList, choose));
            }
            DefaultConfig.DefaultServer.ServerConfig.ForcedDifficulty = MissionsTool.intToDifficulty(comboBoxEdit2.SelectedIndex);
            DefaultConfig.DefaultServer.ServerConfig.AutoSelectMission = toggleSwitch1.IsOn;
            DefaultConfig.DefaultServer.ServerConfig.RandomMissionOrder = i.IsOn;
            DefaultConfig.DefaultServer.SetTime();
            FileTools.SaveConfig(DefaultConfig.DefaultServer.ServerUUID);
        }
        public MissionsEntity GetMission(string MissionName)
        {
            for (int i = 0; i < DefaultConfig.DefaultServer.ServerConfig.missions.Count; i++)
            {
                if (DefaultConfig.DefaultServer.ServerConfig.missions[i].Template == MissionName)
                {
                    return DefaultConfig.DefaultServer.ServerConfig.missions[i];
                }
            }
            return null;
        }

        //string s = JsonConversionTool.ObjectToJson(DefaultConfig.DefaultServer.ServerConfig.missions);
        //Console.WriteLine(s);
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            UserControl_Init();
        }

        private void gridControl1_Load_2(object sender, EventArgs e)
        {
           
        }



        private void i_Toggled(object sender, EventArgs e)
        {

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            MissionSettingUserControl_Leave(null, null);
            XtraMessageBox.Show("保存本页配置完成!请点右上角击标题栏的保存按钮进行写入配置。", "ok", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
