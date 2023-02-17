using steamcmdTools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace steamcmd
{
    public class Helper
    {
        public static string[] parameter;
        public static string progress;
        public static int sum = 0;

        public static void ProgressFun(string str)
        {
            try
            {
                if (str.Contains("Update state") && str.Contains("(0x1)"))
                {

                    string data = GetMiddleStr(str, "progress:", "(");
                    float v = float.Parse(data.Trim());
                    MainWindow.MainForm.LabelText.Content = "开始运行...(" + v + ")%";
                    MainWindow.MainForm.DownloadProgressBar.Value = v;

                }
                if (str.Contains("Update state") && str.Contains("(0x3)"))
                {
                  
                    string data = GetMiddleStr(str, "progress:", "(");
                    float v = float.Parse(data.Trim());
                    MainWindow.MainForm.LabelText.Content = "正在重新配置...(" + v + ")%";
                    MainWindow.MainForm.DownloadProgressBar.Value = v;

                }
                else if (str.Contains("Update state") && str.Contains("(0x5)"))
                {
                    string data = GetMiddleStr(str, "progress:", "(");
                    float v = float.Parse(data.Trim());
                    MainWindow.MainForm.LabelText.Content = "正在验证安装...(" + v + ")%";
                    MainWindow.MainForm.DownloadProgressBar.Value = v;

                }
                else if (str.Contains("Update state") && str.Contains("(0x61)"))
                {
                    string data = GetMiddleStr(str, "progress:", "(");
                    float v = float.Parse(data.Trim());
                    MainWindow.MainForm.LabelText.Content = "正在下载安装...(" + v + "%)";
                    MainWindow.MainForm.DownloadProgressBar.Value = v;

                }
                else if (str.Contains("Update state") && str.Contains("(0x81)"))
                {
                    string data = GetMiddleStr(str, "progress:", "(");
                    float v = float.Parse(data.Trim());
                    MainWindow.MainForm.LabelText.Content = "正在验证更新...(" + v + "%)";
                    MainWindow.MainForm.DownloadProgressBar.Value = v;

                }

                else if (str.Contains("Update state") && str.Contains("(0x101)"))
                {
                    string data = GetMiddleStr(str, "progress:", "(");
                    float v = float.Parse(data.Trim());
                    MainWindow.MainForm.LabelText.Content = "正在提交...(" + v + "%)";
                    MainWindow.MainForm.DownloadProgressBar.Value = v;

                }


               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public static void SetCode(string s)
        {

            ProgressFun(s);
            if (!String.IsNullOrEmpty(s))
            {
                if (s.Contains("Assertion Failed: Unable to access Steam files due to incompatible path"))
                {
                    MessageBox.Show("断言失败:无法访问Steam文件由于不兼容的路径", "错误");
                    Application.Current.Shutdown(0);

                }
                else if (s.Contains("Steamcmd needs to be online to update.	 Please confirm your network connection and try again."))
                {
                    MessageBox.Show("Steamcmd需要在线更新。请确认您的网络连接，然后重试", "错误");
                    Application.Current.Shutdown(0);
                }

                else if (s.Contains("You can also enter this code at any time using 'set_steam_guard_code'"))
                {
                    MessageBox.Show("你也可以在任何时候使用'set_steam_guard_code '输入这段代码", "错误");
                    Application.Current.Shutdown(0);
                }
                else if (s.Contains("Steam Guard code:Login Failure: Invalid Login Auth Code"))
                {
                    MainWindow.MainForm.LabelText.Content = "Steam Guard 代码：登录失败：登录验证码无效";
                    MessageBox.Show("Steam Guard 代码：登录失败：登录验证码无效", "错误");
                    Application.Current.Shutdown(0);
                }
                else if (s.Contains("to Steam Public..."))
                {
                    Console.WriteLine("正在登陆");
                    MainWindow.MainForm.LabelText.Content = "正在登陆...";
                }
                else if (s.Contains("Waiting for client config...OK") || s.Contains("Waiting for user info...OK"))
                {
                    Console.WriteLine("登陆成功!");
                    MainWindow.MainForm.LabelText.Content = "登陆成功!";
                }

                else if (s.Contains("Invalid Password"))
                {
                    MainWindow.MainForm.LabelText.Content = "账号或密码错误!";
                    MessageBox.Show("账号或密码错误", "错误");
                    Application.Current.Shutdown(0);
                }
                else if (s.Contains("error"))
                {
                    MessageBox.Show("发生错误", "错误");
                    Application.Current.Shutdown(0);
                }
                else if (s.Contains("Rate Limit Exceeded"))
                {
                    MessageBox.Show("速率限制超过", "错误");
                    Application.Current.Shutdown(0);
                }
                else if (s.Contains("Two - factor code mismatch"))
                {
                    MainWindow.MainForm.LabelText.Content = "令牌错误";
                    MessageBox.Show("令牌错误!", "错误");
                    Application.Current.Shutdown(0);
                }
                else if (s.Contains("Two-factor code:"))
                {
                    InputData inputData = new InputData();
                    inputData.ShowDialog();

                }
                else if (s.Contains("Error! App"))
                {
                    MainWindow.MainForm.LabelText.Content = "错误!更新失败!";
                    MessageBox.Show("错误!更新失败!", "错误");
                    Application.Current.Shutdown(0);
                }
                else if (s.Contains("Success") && s.Contains("fully installed"))
                {
                    MainWindow.MainForm.LabelText.Content = "更新完成!";
                    MessageBox.Show("更新完成!", "提示");
                    Application.Current.Shutdown(0);
                }
                else if (s.Contains("Two - factor code mismatch"))
                {
                    MainWindow.MainForm.LabelText.Content = "错误!更新失败!";
                    MessageBox.Show("验证码不匹配!", "错误");
                    Application.Current.Shutdown(0);
                }
                else if (s.Contains("Public...FAILED"))
                {
                    MainWindow.MainForm.LabelText.Content = "错误!更新失败!";
                    MessageBox.Show("失败!请查看日志", "错误");
                    Application.Current.Shutdown(0);
                }
                else if (s.Contains("Success") && s.Contains("Downloaded item"))
                {
                    sum += 1;
                    MainWindow.MainForm.LabelText_Sum.Content = "已下载"+sum +"个";
                }
                else if (s.Contains("Downloaded item"))
                {
                    MainWindow.MainForm.LabelText.Content = "正在下载...";
                }



                //LabelText_Sum



            }

        }


        /// <summary>
        /// 取中间文本
        /// </summary>
        /// <param name="str">全文本</param>
        /// <param name="str1">前文本</param>
        /// <param name="str2">后文本</param>
        /// <returns></returns>
        public static string TakeMiddleText(string str, string str1, string str2)
        {
            int i1 = str.IndexOf(str1);
            int i2 = -1;
            string strok = "";
            if (i1 != -1)
            {
                i2 = str.IndexOf(str2, i1);
            }
            if (i2 != -1)
            {
                strok = str.Substring(i1 + 1, i2 - i1 - 1);
            }
            return strok;
        }


        /// <summary>
        /// 取中间文本 + static string GetMiddleStr(string oldStr,string preStr,string nextStr)
        /// </summary>
        /// <param name="oldStr">原文</param>
        /// <param name="preStr">前文</param>
        /// <param name="nextStr">后文</param>
        /// <returns></returns>
        public static string GetMiddleStr(string oldStr, string preStr, string nextStr)
        {
            string tempStr = oldStr.Substring(oldStr.IndexOf(preStr) + preStr.Length);
            tempStr = tempStr.Substring(0, tempStr.IndexOf(nextStr));
            return tempStr;
        }

    }
}
