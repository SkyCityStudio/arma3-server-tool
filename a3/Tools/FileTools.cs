using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using a3.Config;
using a3.Entity;
using a3.Tools;

namespace a3.Tools
{
    public partial class FileTools
    {


        public static void SaveConfig(string ServerUUID)
        {
            
            if (ServerUUID == null || !DefaultConfig.ServerList.ContainsKey(ServerUUID)) {
                return;
            }
            DefaultConfig.Saveing = true;
            string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            SaveFile(path + @"config\" + DefaultConfig.ServerList[ServerUUID].ServerUUID + ".json", JsonConversionTool.ObjectToJson(DefaultConfig.ServerList[ServerUUID]));
            DefaultConfig.Saveing = false;
        }

        public static void SaveFile(string path,string text) {
            File.WriteAllText(path, text,CfgTool.UTF8);
        }
        /// <summary>
        /// 私有变量
        /// </summary>
        private static List<FileInfo> lst = new List<FileInfo>();
        /// <summary>
        /// 获得目录下所有文件或指定文件类型文件(包含所有子文件夹)
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="extName">扩展名可以多个 例如 .mp3.wma.rm</param>
        /// <returns>List<FileInfo></returns>
        public static List<FileInfo> getFile(string path, string extName)
        {
            lst.Clear();
            getdir(path, extName);
            return lst;
        }
        /// <summary>
        /// 私有方法,递归获取指定类型文件,包含子文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <param name="extName"></param>
        private static void getdir(string path, string extName)
        {
            try
            {
                string[] dir = Directory.GetDirectories(path); //文件夹列表
                DirectoryInfo fdir = new DirectoryInfo(path);
                FileInfo[] file = fdir.GetFiles();
                //FileInfo[] file = Directory.GetFiles(path); //文件列表
                if (file.Length != 0 || dir.Length != 0) //当前目录文件或文件夹不为空
                {
                    foreach (FileInfo f in file) //显示当前目录所有文件
                    {
                        if (extName.ToLower().IndexOf(f.Extension.ToLower()) >= 0)
                        {
                            lst.Add(f);
                        }
                    }
                    /*
                    foreach (string d in dir)
                    {
                        getdir(d, extName);//递归
                    }*/
                }
            }
            catch (Exception ex)
            {

                //BaseLogHelper.WriteLogFile();

                throw ex;
            }
        }


        public static List<string> getDir(string path, string extName)
        {
            List<string> dir = new List<string>();
            string[] dirTemp = Directory.GetDirectories(path);
            for (int i = 0; i < dirTemp.Length; i++) {
                if (dirTemp[i].Contains(extName)) {
                    dir.Add(dirTemp[i]);
                }
            }
            return dir;
        }


        public static ModMeta getModMeta(string path)
        {
            if (File.Exists(path + @"\meta.cpp"))
            {
                try
                {
                    StreamReader s = File.OpenText(path + @"\meta.cpp");
                    String line;
                    ModMeta modMeta = new ModMeta();
                    while ((line = s.ReadLine()) != null)
                    {
                        if (line.Contains("name"))
                        {
                            string[] temp = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                            if (temp.Length > 1)
                            {
                                modMeta.Name = temp[1].Trim().Replace(";", "").Replace("\"", "");
                            }
                        }

                        if (line.Contains("publishedid"))
                        {
                            string[] temp = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                            if (temp.Length > 1)
                            {
                                modMeta.PublishedId = RegularMatchTool.ToLong(temp[1].Trim().Replace(";", ""), 0);
                            }
                        }

                        if (line.Contains("timestamp"))
                        {
                            string[] temp = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                            if (temp.Length > 1)
                            {
                                long timestamp;
                                try
                                {
                                    timestamp = long.Parse(temp[1].Trim().Replace(";", ""));
                                }
                                catch 
                                {
                                    timestamp = 0;
                                }
                                modMeta.TimeStamp = timestamp;
                            }
                        }
                    }
                    return modMeta;
                }
                catch  {
                    return null;
                }
            }
            else {
                return null;
            }
        }

        public static string getDirName(string path)
        {
            path = path.LastIndexOf(@"\") + 1 == path.Length ? path.Substring(0, path.Length - 1) : path;
            string result = path.Substring(path.LastIndexOf(@"\") + 1);
            return result;
        }
        


        /// <summary>
        /// 获得目录下所有文件或指定文件类型文件(包含所有子文件夹)
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="extName">扩展名可以多个 例如 .mp3.wma.rm</param>
        /// <returns>List<FileInfo></returns>
        public static List<FileInfo> getMultipleFile(string path, string extName)
        {
            try
            {
                List<FileInfo> lst = new List<FileInfo>();
                string[] dir = Directory.GetDirectories(path); //文件夹列表
                DirectoryInfo fdir = new DirectoryInfo(path);
                FileInfo[] file = fdir.GetFiles();
                //FileInfo[] file = Directory.GetFiles(path); //文件列表
                if (file.Length != 0 || dir.Length != 0) //当前目录文件或文件夹不为空
                {
                    foreach (FileInfo f in file) //显示当前目录所有文件
                    {
                        if (extName.ToLower().IndexOf(f.Extension.ToLower()) >= 0)
                        {
                            lst.Add(f);
                        }
                    }
                    /*
                    foreach (string d in dir)
                    {
                        getFile(d, extName);//递归
                    }*/
                }
                return lst;
            }
            catch (Exception ex)
            {
                //   LogHelper.WriteLog(ex);
                throw ex;
            }
        }


        /// <summary>
        /// 复制文件夹中的所有内容
        /// </summary>
        /// <param name="sourceDirPath">源文件夹目录</param>
        /// <param name="saveDirPath">指定文件夹目录</param>
        public static bool CopyDirectory(string sourceDirPath, string saveDirPath)
        {
            try
            {
                if (!Directory.Exists(saveDirPath))
                {
                    Directory.CreateDirectory(saveDirPath);
                }
                string[] files = Directory.GetFiles(sourceDirPath);
                foreach (string file in files)
                {
                    string pFilePath = saveDirPath + "\\" + Path.GetFileName(file);
                    if (File.Exists(pFilePath))
                        continue;
                    File.Copy(file, pFilePath, true);
                }

                string[] dirs = Directory.GetDirectories(sourceDirPath);
                foreach (string dir in dirs)
                {
                    CopyDirectory(dir, saveDirPath + "\\" + Path.GetFileName(dir));
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }



        /// <summary>
        /// 得某文件夹下所有的文件
        /// </summary>
        /// <param name="directory">文件夹名称</param>
        /// <param name="pattern">搜寻指类型</param>
        /// <returns></returns>
        /// 
        public static List<FileInfo> bikey = new List<FileInfo>();

        private static string key = "*.bikey";
        public static void GetBikey(DirectoryInfo directory)
        {
            if (directory.Exists || key.Trim() != string.Empty)
            {
                foreach (FileInfo info in directory.GetFiles(key))
                {
                    bikey.Add(info);
                }
                foreach (DirectoryInfo info in directory.GetDirectories())
                {
                    GetBikey(info);
                }
            }
        }


    }
}
