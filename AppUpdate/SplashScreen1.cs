using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AppUpdate
{

    public partial class SplashScreen1 : SplashScreen
    {
        public static double porss = 0;
        public static int success = 0;
        public string url = "http://tools.destiny.cool/arma3_server_tools/Arma3ServerTools.zip";
        public string path = Environment.CurrentDirectory + @"\Arma3ServerTools.zip";

        public SplashScreen1(string[] args)
        {
            InitializeComponent();
            Console.WriteLine(Environment.CurrentDirectory);
            this.labelCopyright.Text = "Copyright © Destiny娱乐至上 2014-" + DateTime.Now.Year.ToString();
            try
            {
                HttpHelper httpHelper = new HttpHelper();
               string version = httpHelper.DoGet("http://tools.destiny.cool/arma3_server_tools/version.txt", null);
                Console.WriteLine(version);
                
                if (version != null && args[0] != version)
                {
                    Thread thread = new Thread(HttpDownloadFile);
                    thread.Start();
                }
                else
                {
                    System.Environment.Exit(0);
                }

            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
              
            }

        }


        /// <summary>
        /// Http下载文件
        /// </summary>
        public void HttpDownloadFile()
        {
            try
            {
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                
                //创建本地文件写入流
                Stream stream = new FileStream(path, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                long resp = response.ContentLength;
                while (size > 0)
                {
                     double a = Math.Round((double) stream.Length / resp, 2);
                     porss = a * 100;
                    stream.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                stream.Close();
                responseStream.Close();
                 
                bool ok = ZipHelper.UnZip(path, Environment.CurrentDirectory);

                Process m_Process = new Process();
                m_Process.StartInfo.FileName = Environment.CurrentDirectory+ @"\unzip.bat";
                m_Process.Start();
                System.Environment.Exit(0);
            }
            catch 
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("更新失败!请访问:https://destiny.cool/s/arma3-tool 手动下载!", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                System.Environment.Exit(0);
            }
 
        }


        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion

        public enum SplashScreenCommand
        {
        }

        private void labelCopyright_Click(object sender, EventArgs e)
        {

        }

        private void peImage_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void SplashScreen1_Load(object sender, EventArgs e)
        {
           
        }

        private void hyperlinkLabelControl1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://destiny.cool/");
        }

        private void hyperlinkLabelControl2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://destiny.cool/");
        }
    }
}