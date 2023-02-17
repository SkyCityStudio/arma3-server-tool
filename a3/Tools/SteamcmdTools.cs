using a3.Config;
using a3.Dialog;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace a3.Tools
{
    public class SteamcmdTools
    {

        public static void InstallUpdateArma3Server(string dir) {
            if (string.IsNullOrEmpty(DefaultConfig.steamcmd.i))
            {
                if (!SetSteamcmdInfo())
                {
                    return;
                }
            }
            string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (!File.Exists(path + @"extension\steamcmd.exe"))
            {
                XtraMessageBox.Show("错误!开服器目录下没有steamcmd.exe，它应该放在:" + path + @"extension\steamcmd.exe", "找不到steamcmd.exe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            XtraMessageBox.Show("即将直接使用steamcmd下载，确保账号密码正确后，会出现一个控制台(黑框框)输入你的验证码即可。更新/下载成功黑框框中会显示:Success! App '233780' fully installed.", "提示!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Process process = new Process();
            process.StartInfo.FileName = path + @"extension\steamcmd.exe";
            process.StartInfo.Arguments = "+force_install_dir "+ DefaultConfig.DoubleQuotes + dir + DefaultConfig.DoubleQuotes + " +login " + DefaultConfig.steamcmd.u + " " + DefaultConfig.steamcmd.p + " +app_update 233780 -beta creatordlc validate";
            process.Start();
        }

        public static bool SetSteamcmdInfo() {
            SteamcmdConfigDialog configdialog = new SteamcmdConfigDialog();
            if (DevExpress.XtraEditors.XtraDialog.Show(configdialog, "配置SteamCMD", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(DefaultConfig.steamcmd.d) && !string.IsNullOrEmpty(DefaultConfig.steamcmd.u) && !string.IsNullOrEmpty(DefaultConfig.steamcmd.p) && !string.IsNullOrEmpty(DefaultConfig.steamcmd.i))
                {
                    Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");
                    if (RegCHZN.Match(DefaultConfig.steamcmd.d).Success || RegCHZN.Match(DefaultConfig.steamcmd.i).Success) {
                        XtraMessageBox.Show("你选择的路径里包含中文，这会导致一系列问题，请确保安装路径里不包含中文!", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        DefaultConfig.steamcmd.d = String.Empty;
                        DefaultConfig.steamcmd.i = String.Empty;
                        return false;
                    }
                    configdialog.SaveConfig();
                    return true;
                }
                else
                {
                    XtraMessageBox.Show("保存失败!配置不完整!", "出错!", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
        int hWnd, // handle to destination window    
        int Msg, // message    
        int wParam, // first message parameter    
        ref COPYDATASTRUCT lParam // second message parameter    
        );
        const int WM_COPYDATA = 0x004A;
        public struct COPYDATASTRUCT
        {

            public IntPtr dwData;

            public int cbData;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;

        }

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        public static int DownloadMod() {

            if (XtraDialog.Show(new SubscribeModsDialog(), "确认需要更新/下载的模组", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                Process process = new Process();
                if (XtraMessageBox.Show("直接使用steamcmd下载吗?确保账号密码正确后会控制台(黑框框)会显示输入你的验证码，输入即可。取消则使用自带工具。", "选择下载工具!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                    if (!File.Exists(path + @"extension\steamcmd.exe")) {
                        XtraMessageBox.Show("错误!开服器目录下没有steamcmd.exe，它应该放在:" + path + @"extension\steamcmd.exe", "找不到steamcmd.exe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                    }
                    if (!File.Exists(DefaultConfig.steamcmd.d + @"\steamcmd.exe"))
                    {
                        File.Copy(path + @"extension\steamcmd.exe", DefaultConfig.steamcmd.d + @"\steamcmd.exe", true);
                    }
                    StringBuilder sb = new StringBuilder();
                    sb.Append("+login "+ DefaultConfig.steamcmd.u + " " + DefaultConfig.steamcmd.p);
                    sb.Append(" ");
                    DefaultConfig.HtmlMods.ForEach((m) => {
                        sb.Append("+workshop_download_item 107410 ").Append(m).Append(" ");
                    });
                    Thread.Sleep(1000);
                    process.StartInfo.FileName = DefaultConfig.steamcmd.d + @"\steamcmd.exe";
                    process.StartInfo.Arguments = sb.ToString();
                    process.Start();
                    return 1;
                }
                
                
                process.StartInfo.FileName = path + "steamcmdTools.exe";
                process.Start();
                Thread.Sleep(1000);
                int WINDOW_HANDLER = process.MainWindowHandle.ToInt32();
                SendCmdMsg("0x001"+ DefaultConfig.steamcmd.d, WINDOW_HANDLER);
                SendCmdMsg("0x002login " + DefaultConfig.steamcmd.u + " " + DefaultConfig.steamcmd.p, WINDOW_HANDLER);
                DefaultConfig.HtmlMods.ForEach((m) => {
                    SendCmdMsg("0x002" + "workshop_download_item 107410 " + m, WINDOW_HANDLER);
                });
                SendCmdMsg("0x002quit", WINDOW_HANDLER);
                return WINDOW_HANDLER;
            }
            return -1;
        }
        public static int SendCmdMsg(string msg,int WINDOW_HANDLER) {
            if (WINDOW_HANDLER != 0)
            {
                byte[] sarr = Encoding.Default.GetBytes(msg);
                int len = sarr.Length;
                COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)100;
                cds.lpData = msg;
                cds.cbData = len + 1;
                SendMessage(WINDOW_HANDLER, WM_COPYDATA, 0, ref cds);
            }
            return WINDOW_HANDLER;
        }
    }
}
