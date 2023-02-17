using a3.Config;
using a3.Tools;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace a3.Modules
{
    public partial class DifficultySettingUserControl : DevExpress.DXperience.Demos.TutorialControlBase
    {
        public DifficultySettingUserControl()
        {
            InitializeComponent();
        }

        public void UserControl_Init() {
            comboBoxEdit11.SelectedIndex = DefaultConfig.DefaultServer.serverProfile.GroupIndicators;
            comboBoxEdit21.SelectedIndex = DefaultConfig.DefaultServer.serverProfile.FriendlyTags;
            comboBoxEdit31.SelectedIndex = DefaultConfig.DefaultServer.serverProfile.EnemyTags;
            comboBoxEdit41.SelectedIndex = DefaultConfig.DefaultServer.serverProfile.DetectedMines;
            comboBoxEdit51.SelectedIndex = DefaultConfig.DefaultServer.serverProfile.Commands;
            comboBoxEdit61.SelectedIndex = DefaultConfig.DefaultServer.serverProfile.WayPoints;
            toggleSwitch11.IsOn = DefaultConfig.DefaultServer.serverProfile.TacticalPing == 1;
            comboBoxEdit71.SelectedIndex = DefaultConfig.DefaultServer.serverProfile.WeaponInfo;
            comboBoxEdit81.SelectedIndex = DefaultConfig.DefaultServer.serverProfile.StanceIndicator;
            toggleSwitch21.IsOn = DefaultConfig.DefaultServer.serverProfile.StaminaBar == 1;
            toggleSwitch31.IsOn = DefaultConfig.DefaultServer.serverProfile.WeaponCrosshair == 1;
            toggleSwitch41.IsOn = DefaultConfig.DefaultServer.serverProfile.VisionAid == 1;
            comboBoxEdit1.SelectedIndex = DefaultConfig.DefaultServer.serverProfile.ThirdPersonView;
            toggleSwitch1.IsOn = DefaultConfig.DefaultServer.serverProfile.CameraShake == 1;
            toggleSwitch2.IsOn = DefaultConfig.DefaultServer.serverProfile.ScoreTable == 1;
            toggleSwitch3.IsOn = DefaultConfig.DefaultServer.serverProfile.DeathMessages == 1;
            toggleSwitch4.IsOn = DefaultConfig.DefaultServer.serverProfile.VonID == 1;
            toggleSwitch61.IsOn = DefaultConfig.DefaultServer.serverProfile.MapContent == 1;
            toggleSwitch6.IsOn = DefaultConfig.DefaultServer.serverProfile.MapContentFriendly == 1;
            toggleSwitch9.IsOn = DefaultConfig.DefaultServer.serverProfile.MapContentEnemy == 1;
            toggleSwitch10.IsOn = DefaultConfig.DefaultServer.serverProfile.MapContentMines == 1;
            toggleSwitch5.IsOn = DefaultConfig.DefaultServer.serverProfile.ReducedDamage == 1;
            toggleSwitch7.IsOn = DefaultConfig.DefaultServer.serverProfile.AutoReport == 1;
            toggleSwitch8.IsOn = DefaultConfig.DefaultServer.serverProfile.MultipleSaves == 1;
            spinEdit1.Value = (decimal)DefaultConfig.DefaultServer.serverProfile.SkillAI;
            spinEdit2.Value = (decimal)DefaultConfig.DefaultServer.serverProfile.PrecisionAI;
        }

        public void UserControl_Save()
        {
            DifficultySettingUserControl_Leave(null, null);
        }
        private void DifficultySettingUserControl_Leave(object sender, EventArgs e)
        {
            DefaultConfig.DefaultServer.serverProfile.GroupIndicators = comboBoxEdit11.SelectedIndex;
            DefaultConfig.DefaultServer.serverProfile.FriendlyTags = comboBoxEdit21.SelectedIndex;
            DefaultConfig.DefaultServer.serverProfile.EnemyTags = comboBoxEdit31.SelectedIndex;
            DefaultConfig.DefaultServer.serverProfile.DetectedMines = comboBoxEdit41.SelectedIndex;
            DefaultConfig.DefaultServer.serverProfile.Commands = comboBoxEdit51.SelectedIndex;
            DefaultConfig.DefaultServer.serverProfile.WayPoints = comboBoxEdit61.SelectedIndex;
            DefaultConfig.DefaultServer.serverProfile.TacticalPing = toggleSwitch11.IsOn?1:0;
            DefaultConfig.DefaultServer.serverProfile.WeaponInfo = comboBoxEdit71.SelectedIndex;
            DefaultConfig.DefaultServer.serverProfile.StanceIndicator = comboBoxEdit81.SelectedIndex;
            DefaultConfig.DefaultServer.serverProfile.StaminaBar = toggleSwitch21.IsOn ? 1 : 0;
            DefaultConfig.DefaultServer.serverProfile.WeaponCrosshair = toggleSwitch31.IsOn ? 1 : 0;
            DefaultConfig.DefaultServer.serverProfile.VisionAid = toggleSwitch41.IsOn ? 1 : 0;
            DefaultConfig.DefaultServer.serverProfile.ThirdPersonView = comboBoxEdit1.SelectedIndex;
            DefaultConfig.DefaultServer.serverProfile.CameraShake = toggleSwitch1.IsOn ? 1 : 0;
            DefaultConfig.DefaultServer.serverProfile.ScoreTable = toggleSwitch2.IsOn ? 1 : 0;
            DefaultConfig.DefaultServer.serverProfile.DeathMessages = toggleSwitch3.IsOn ? 1 : 0;
            DefaultConfig.DefaultServer.serverProfile.VonID = toggleSwitch4.IsOn ? 1 : 0;
            DefaultConfig.DefaultServer.serverProfile.MapContent = toggleSwitch61.IsOn ? 1 : 0;
            DefaultConfig.DefaultServer.serverProfile.MapContentFriendly = toggleSwitch6.IsOn ? 1 : 0;
            DefaultConfig.DefaultServer.serverProfile.MapContentEnemy = toggleSwitch9.IsOn ? 1 : 0;
            DefaultConfig.DefaultServer.serverProfile.MapContentMines = toggleSwitch10.IsOn ? 1 : 0;
            DefaultConfig.DefaultServer.serverProfile.ReducedDamage = toggleSwitch5.IsOn ? 1 : 0;
            DefaultConfig.DefaultServer.serverProfile.AutoReport = toggleSwitch7.IsOn ? 1 : 0;
            DefaultConfig.DefaultServer.serverProfile.MultipleSaves = toggleSwitch8.IsOn ? 1 : 0;
            DefaultConfig.DefaultServer.serverProfile.SkillAI = (double)spinEdit1.Value;
            DefaultConfig.DefaultServer.serverProfile.PrecisionAI = (double)spinEdit2.Value;
            DefaultConfig.DefaultServer.SetTime();
            FileTools.SaveConfig(DefaultConfig.DefaultServer.ServerUUID);
        }


        private void spinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if (spinEdit1.Value > 1)
            {
                spinEdit1.Value = 1.0M;
            }
            if (spinEdit1.Value < 0)
            {
                spinEdit1.Value = 0;
            }
        }

        private void spinEdit2_EditValueChanged(object sender, EventArgs e)
        {
            if (spinEdit2.Value > 1)
            {
                spinEdit2.Value = 1.0M;
            }
            if (spinEdit2.Value < 0)
            {
                spinEdit2.Value = 0;
            }
        }
    }
}
