using a3.Config;
using a3.Tools;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace a3.Dialog
{
    public partial class SubscribeModsDialog : DevExpress.XtraEditors.XtraUserControl
    {
        private DataTable dataTable = new DataTable();
        public SubscribeModsDialog()
        {
            InitializeComponent();

            dataTable.Columns.Add("确认模组", typeof(bool));
            dataTable.Columns.Add("模组名称", typeof(string));
            dataTable.Columns.Add("模组ID", typeof(string));
            dataTable.Columns.Add("模组LOGO", typeof(Image));
            dataTable.Columns.Add("模组大小", typeof(string));
            dataTable.Columns.Add("模组描述", typeof(string));
            dataTable.Columns.Add("模组标签", typeof(string));
            dataTable.Columns.Add("创建时间", typeof(string));
            dataTable.Columns.Add("更新时间", typeof(string));





        }

        public async Task LoadModInfo() {
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
                    new KeyValuePair<string, string>("itemcount", DefaultConfig.HtmlMods.Count.ToString())
                };
                for (int i = 0; i < DefaultConfig.HtmlMods.Count; i++)
                {
                    body.Add(new KeyValuePair<string, string>("publishedfileids[" + i + "]", DefaultConfig.HtmlMods[i]));
                }
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.Timeout = TimeSpan.FromMilliseconds(10000);
                var res = await client.PostAsync("", new FormUrlEncodedContent(body));
                if (res.IsSuccessStatusCode)
                {
                    StringBuilder sb = new StringBuilder();
                    var exec = await res.Content.ReadAsStringAsync();
                    Console.WriteLine(exec);
                    JObject json = JObject.Parse(exec);
                    JArray jarray = JArray.Parse(json["response"]["publishedfiledetails"].ToString());
                    int index = 0;
                    foreach (var item in jarray)
                    {
                        if (item["creator_app_id"].ToString() != "107410")
                        {
                            break;
                        }
                        sb.Clear();
                        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                        var CreateTime = startTime.AddSeconds(long.Parse(item["time_created"].ToString()));
                        var UpdateTime = startTime.AddSeconds(long.Parse(item["time_updated"].ToString()));
                        string previewUrl = item["preview_url"].ToString();
                        WebRequest webreq = WebRequest.Create(previewUrl);
                        WebResponse webres = webreq.GetResponse();
                        Stream stream = webres.GetResponseStream();
                        decimal size = Math.Round(decimal.Parse(item["file_size"].ToString()) / 1024 / 1024, 2);
                        JArray tags = JArray.Parse(item["tags"].ToString());
                        foreach (var tag in tags)
                        {
                            sb.Append(tag["tag"].ToString() + ",");
                        }
                        index++;
                        this.Invoke(new Action(() => {
                            progressPanel1.Description = "正在加载模组:" + item["title"] + " (" + size + "MB" + ") " + index + "/" + jarray.Count;
                        }));
                        dataTable.Rows.Add(true, item["title"], item["publishedfileid"], Image.FromStream(stream), size + "MB", item["description"], sb.Length > 0 ? sb.ToString().Substring(0, sb.Length - 1) : "", CreateTime, UpdateTime);
                    }

                }
                else {
                    throw new Exception("加载模组信息失败,重新显示");
                }
            }
            catch  
            {
                DefaultConfig.HtmlMods.ForEach(modId => {
                    string info = "加载模组信息失败!";
                    dataTable.Rows.Add(true, info, modId, SystemIcons.Error.ToBitmap(),  "0MB", info, info, info, info);
                });
            }
            if (dataTable.Rows.Count != 0)
            {
                this.Invoke(new Action(() => {
                    gridControl1.DataSource = dataTable;
                    gridControl1.ForceInitialize();
                    gridView1.Columns[1].OptionsColumn.AllowEdit = false;
                    gridView1.Columns[2].OptionsColumn.AllowEdit = false;
                    gridView1.Columns[3].OptionsColumn.AllowEdit = false;
                    gridView1.Columns[4].OptionsColumn.AllowEdit = false;
                    gridView1.Columns[6].OptionsColumn.AllowEdit = false;
                    gridView1.Columns[7].OptionsColumn.AllowEdit = false;
                    gridView1.Columns[8].OptionsColumn.AllowEdit = false;
                }));
            }
            this.Invoke(new Action(() => {
                progressPanel1.Hide();
            }));
        }

        private void gridControl1_Load_1(object sender, EventArgs e)
        {
            dataTable.Rows.Clear();
            Task.Run(() => LoadModInfo());
        }

        private void gridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            Console.WriteLine(2);
            DefaultConfig.HtmlMods.Clear();
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                bool Mods = MissionsTool.getBoolean(gridView1.GetRowCellValue(i, "确认模组").ToString());
                string ModId = gridView1.GetRowCellValue(i, "模组ID").ToString();
                if (Mods) {
                    DefaultConfig.HtmlMods.Add(ModId);
                }
            }
        }
    }
}
