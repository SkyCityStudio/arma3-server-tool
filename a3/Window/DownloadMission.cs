using a3.Config;
using DevExpress.DataAccess.Native.Data;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Data.Linq.Mapping;
using a3.Entity;
using System.Web.Script.Serialization;
using DevExpress.XtraPrinting;
using DevExpress.Data.ExpressionEditor;
using DevExpress.XtraRichEdit.Painters;
using DevExpress.XtraRichEdit;
using DevExpress.Office.Export;
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Model;
using System.Xml.Serialization;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Export;
using DevExpress.Office;

namespace a3.Window
{
    public partial class DownloadMission : DevExpress.XtraEditors.XtraForm
    {
        public string id;
        const int KB = 1000;
        const int MB = 1000 * KB;
        const int GB = 1000 * MB;
        private string PboUri;
        private string PboFile;
        private static readonly HttpClient httpClient = new HttpClient();
        private string DisplayText;
        public DownloadMission()
        {
            InitializeComponent();
        }

        private void DownloadMission_Load(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                await LoadModInfo();
            });
        }
        static string GetSize(string size)
        {
            int sizeInBytes = int.TryParse(size, out int num) ? num : 0;
            if (sizeInBytes >= GB)
                return $"{sizeInBytes / (double)GB:0.00} GB";
            else if (sizeInBytes >= MB)
                return $"{sizeInBytes / (double)MB:0.00} MB";
            else if (sizeInBytes >= KB)
                return $"{sizeInBytes / (double)KB:0.00} KB";
            else
                return $"{sizeInBytes} B";
        }
        public async Task LoadModInfo()
        {
            try
            {
                string url = DefaultConfig.ModInfoUrl;
                var handler = new HttpClientHandler()
                {
                    CookieContainer = new CookieContainer()
                };
                var client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(url)
                };
                var body = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("itemcount", "1")
                };
                body.Add(new KeyValuePair<string, string>("publishedfileids[0]", id));
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.Timeout = TimeSpan.FromMilliseconds(10000);
                var res = await client.PostAsync("", new FormUrlEncodedContent(body));
                if (res.IsSuccessStatusCode)
                {
                    var exec = await res.Content.ReadAsStringAsync();
                    PublishedFileDetailsEntity fileDetailsEntity = new JavaScriptSerializer().Deserialize<PublishedFileDetailsEntity>(exec);
                    Image preview = null;
                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] imageBytes = webClient.DownloadData(fileDetailsEntity.response.publishedfiledetails[0].preview_url);
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                preview = Image.FromStream(ms);
                            }
                        }
                    }
                    catch { 
                    }

                    Invoke(new Action(() =>
                    {
                        labelControl3.Text = GetSize(fileDetailsEntity.response.publishedfiledetails[0].file_size);
                        //仅仅处理简单的BBC标签
                        richEditControl1.HtmlText = fileDetailsEntity.response.publishedfiledetails[0].description.Replace("[b]", "<b>").Replace("[/b]", "</b>")
                             .Replace("[i]", "<i>").Replace("[/i]", "</i>")
                             .Replace("[h1]", "<h1>").Replace("[/h1]", "</h1>")
                             .Replace("[h21]", "<h2>").Replace("[/h2]", "</h2>")
                             .Replace("[h3]", "<h3>").Replace("[/h3]", "</h3>");

                        labelControl5.Text = fileDetailsEntity.response.publishedfiledetails[0].subscriptions.ToString();
                        labelControl7.Text = fileDetailsEntity.response.publishedfiledetails[0].favorited.ToString();
                        labelControl13.Text = fileDetailsEntity.response.publishedfiledetails[0].views.ToString();
                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        Console.WriteLine(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        labelControl14.Text = dateTime.AddSeconds(fileDetailsEntity.response.publishedfiledetails[0].time_created).ToString("yyyy-MM-dd HH:mm:ss");
                        labelControl15.Text = dateTime.AddSeconds(fileDetailsEntity.response.publishedfiledetails[0].time_updated).ToString("yyyy-MM-dd HH:mm:ss");
                        memoEdit1.Text = string.Join(", ", fileDetailsEntity.response.publishedfiledetails[0].tags.Select(t => t.tag));
                        if(preview!=null)
                            imageSlider1.Images.Add(preview);
                        labelControl1.Text = fileDetailsEntity.response.publishedfiledetails[0].title;
                        Text = "下载 " + fileDetailsEntity.response.publishedfiledetails[0].title;
                    }));
                    PboUri = fileDetailsEntity.response.publishedfiledetails[0].file_url;
                    PboFile = fileDetailsEntity.response.publishedfiledetails[0].filename;
                    if (string.IsNullOrEmpty(PboFile)) {
                        XtraMessageBox.Show("加载地图信息失败!此链接或ID不是有效的地图!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Invoke((Action)(() => Close()));
                    }
                }
                else
                {
                    XtraMessageBox.Show("加载地图信息失败!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Invoke((Action)(() => Close()));
                }
            }
            catch
            {
                XtraMessageBox.Show("加载信息时发生异常请重试!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Invoke((Action)(() => Close()));

            }
            Invoke(new Action(() =>
            {
                navigationFrame1.SelectedPageIndex = 1;
            }));
        }

        private async Task DownloadFileAsync(string url, string destinationPath, IProgress<DownloadProgressEntity> progress)
        {
            using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                var totalBytes = response.Content.Headers.ContentLength.GetValueOrDefault();
                var buffer = new byte[8192];
                var bytesReceived = 0L;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                FileStream fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize: 8192, useAsync: true);
                using (var downloadStream = await response.Content.ReadAsStreamAsync())
                {
                    while (true)
                    {
                        var bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0) break;
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        bytesReceived += bytesRead;
                        var progressReport = new DownloadProgressEntity
                        {
                            BytesReceived = bytesReceived,
                            TotalBytes = totalBytes,
                            ProgressPercentage = bytesReceived / (double)totalBytes,
                            Speed = bytesReceived / 1024.0 / watch.Elapsed.TotalSeconds
                        };
                        progress.Report(progressReport);
                    }
                }
                watch.Stop();
                Invoke(new Action(() =>
                {
                    simpleButton1.Enabled = true;
                    simpleButton1.Text = "已下载完成";
                }));
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            simpleButton1.Enabled = false;
            string SavePath = DefaultConfig.DefaultServer.ServerDir;
            if (string.IsNullOrEmpty(DefaultConfig.DefaultServer.ServerDir))
            {
                if (XtraMessageBox.Show("你当前没有配置服务端路径，继续下载将会要求你选择一个文件保存路径。要继续下载吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    simpleButton1.Enabled = true;
                    return;
                }
                if (folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                SavePath = folderBrowserDialog1.SelectedPath;
            }
            simpleButton1.Text = "正在下载";
            Directory.CreateDirectory($@"{SavePath}\mpmissions");
            Task.Run(async () => await DownloadPboFile($@"{SavePath}\mpmissions\{PboFile}", PboUri));
        }
        private async Task DownloadPboFile(string Pbo, string uri)
        {
            var progress = new Progress<DownloadProgressEntity>();
            progress.ProgressChanged += (s, e1) =>
            {
                DisplayText = $"进度: {e1.ProgressPercentage:P}   速度: {e1.Speed:F2} KB/s    已下载: {GetSize(e1.BytesReceived.ToString())}/{GetSize(e1.TotalBytes.ToString())}";
                Invoke(new Action(() =>
                {
                    progressBarControl1.EditValue = e1.ProgressPercentage * 100;
                }));

            };
            await DownloadFileAsync(uri, Pbo, progress);
        }

        private void progressBarControl1_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            e.DisplayText = DisplayText;
        }

    }
}