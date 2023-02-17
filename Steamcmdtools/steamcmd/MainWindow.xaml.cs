using SteamCMD.ConPTY;
using SteamCMD.ConPTY.Interop.Definitions;
using steamcmdTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace steamcmd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static SteamCMDConPTY steamCMDConPTY;

        public static MainWindow MainForm = null;


        public MainWindow()
        {

            InitializeComponent();
            MainForm = this;
            Title = "A3DestinySteamTools 创意工坊MOD更新/下载";
        }



            public void InitSteamCMD(string path)
        {

            steamCMDConPTY = new SteamCMDConPTY
            {
                // steamcmd.exe directory
                WorkingDirectory = path,//Directory.GetCurrentDirectory(),

                // You can pass argments like "+login anonymous +app_update 232250 +quit"
                Arguments = string.Empty,

                // We need to filter the control sequences in WPF
                FilterControlSequences = true,
            };

           

          
            /*
            steamCMDConPTY.TitleReceived += (sender, data) =>
            {
                Dispatcher.Invoke(() =>
                {
                   Title = "A3DestinySteamTools " + data;
                });
            };
            */

            //steamcmd输出信息时
            steamCMDConPTY.OutputDataReceived += (sender, data) =>
            {
                Dispatcher.Invoke(() =>
                {
                    TextBoxOutput.Text += data;
                    Helper.SetCode(data);
                    TextBoxOutput.ScrollToEnd();

                });
            };

            //STEAM退出时关闭程序
            steamCMDConPTY.Exited += (sender, exitCode) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Application.Current.Shutdown(exitCode);
                });
            };

            try
            {
                ProcessInfo processInfo = steamCMDConPTY.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }




        const int WM_COPYDATA = 0x004A; // 固定数值，不可更改
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;// 任意值
            public int cbData; // 指定lpData内存区域的字节数
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;// 发送给目标窗口所在进程的数据
        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                IntPtr handle = hwndSource.Handle;
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }


        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            if (msg == WM_COPYDATA)
            {
                COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));//从发送方接收到的数据结构
                string param = cds.lpData;//获取发送方传过来的消息

                if (param != null)
                {
                    int length = 5;
                    string header = param.Substring(0, 5);
                    string data = null;
                    if (param.Length > 5)
                    {
                         data = param.Substring(length, param.Length - length);
                    }
              

                    if (header.Equals("0x001"))
                    {
                        InitSteamCMD(data);

                  
                   
                        
                    }
                    if (header.Equals("0x002"))
                    {
                        steamCMDConPTY.WriteLine(data);
                        if (data.Contains("workshop_download_item"))
                        {
                            DownloadProgressBar.IsIndeterminate = true;
                        }
                        else
                        {
                            DownloadProgressBar.IsIndeterminate = false;
                        }
                    }

                    if (header.Equals("0x003"))
                    {
                        Application.Current.Shutdown(0);
                    }


                }

            }

            return hwnd;
        }

        
    }
}
