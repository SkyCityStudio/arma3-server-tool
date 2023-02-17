using a3.Config;
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

namespace a3.Modules
{
    public partial class AboutUsControl : DevExpress.DXperience.Demos.TutorialControlBase
    {
        public AboutUsControl()
        {
            InitializeComponent();
            labelControl3.Text = "版本 "+ DefaultConfig .Version+ " (64-bit) - Beta";
        }
    }
}
