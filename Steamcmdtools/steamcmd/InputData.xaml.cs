using steamcmd;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace steamcmdTools
{

    /// <summary>
    /// InputData.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class InputData : Window
    {

        public InputData()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.steamCMDConPTY.WriteLine(TextBox.Text);
            this.Close();
        }
    }
}
