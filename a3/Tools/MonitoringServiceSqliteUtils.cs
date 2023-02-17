using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace a3.Tools
{
    public class MonitoringServiceSqliteUtils
    {
        public static SQLiteConnection dbConnection = null;
        public static bool init()
        {
            try
            {
                if (dbConnection == null || dbConnection.State != ConnectionState.Open)
                {
                    string dbPath = @"Data Source = " + Environment.CurrentDirectory + @"\destiny_statistics.db; version=3;Compress=True;";
                    string filePath = Environment.CurrentDirectory + @"\destiny_statistics.db";
                    if (!File.Exists(filePath))
                    {
                        SQLiteConnection.CreateFile(filePath);
                    }
                    dbConnection = new SQLiteConnection(dbPath);
                    dbConnection.Open();

                    DataDBExists();
                    return true;
                }
            }
            catch 
            {
                XtraMessageBox.Show("建立destiny_statistics.db统计表失败!请以管理员全权限运行重试或手动打开开服工具下destiny_statistics.db文件(使用navicat或其他软件)并运行sql 目录下的 destiny_statistics.sql", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;

        }


        private static void DataDBExists()
        {
            string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (!File.Exists(path + @"sql\destiny_statistics.sql"))
            {
                XtraMessageBox.Show("建表失败!当前开服工具根目录下 sql -> destiny_statistics.sql不存在!", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            FileStream fs = new FileStream(path + @"sql\destiny_statistics.sql", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, CfgTool.UTF8);
            string sql = sr.ReadToEnd();
            sr.Close();
            fs.Close();
            SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, dbConnection);
            int ok = cmdCreateTable.ExecuteNonQuery();
        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            return ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();
        }


        public static int DeleteData(string date)
        {
            alive();
            SQLiteCommand insert = new SQLiteCommand(string.Format("DELETE FROM a3_object_manipulation_num WHERE create_time_timestamp < {0}", date), dbConnection);
            int row = insert.ExecuteNonQuery();
            insert.Cancel();
            return row;
        }


        public static int InitPlayerOnlineInfo(string serverUUID) {
            alive();
            SQLiteCommand insert = new SQLiteCommand(string.Format("UPDATE a3_player_info SET online = '0' WHERE server_id = (SELECT id FROM a3_servers WHERE server_name = '{0}')",serverUUID), dbConnection);
            int row = insert.ExecuteNonQuery();
            insert.Cancel();
            return row;
        }

        public static int UpdatePlayerOnlineInfo(int id, string[] args)
        {
            alive();
            SQLiteCommand insert = new SQLiteCommand(string.Format("UPDATE a3_player_info SET online = '{0}' WHERE  player_id = '{1}' AND server_id = {2}", args[3], args[2], id), dbConnection);
            int row = insert.ExecuteNonQuery();
            insert.Cancel();
            return row;
        }


        public static int InsertOrUpdatePlayerInfo(int id, string[] args)
        {
            alive();
            try
            {
                string sql = string.Format("SELECT id FROM a3_player_info where player_id = '{0}'", args[2]);
                SQLiteCommand cmd = new SQLiteCommand(sql, dbConnection);
                SQLiteDataReader reader = cmd.ExecuteReader();
                bool isExist = reader.Read();
                reader.Close();
                cmd.Cancel();
                if (!isExist)
                {
                    SQLiteCommand insert = new SQLiteCommand(string.Format("INSERT INTO a3_player_info(server_id, data_key, player_id, player_name, infantry_kills, soft_vehicle_kills, armor_kills, air_kills, deaths, total_score, create_time,online,create_time_timestamp) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}')", id, args[0], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),1,GetTimeStamp()), dbConnection);
                    int row = insert.ExecuteNonQuery();
                    insert.Cancel();
                    return row;
                }
                else
                {
                    SQLiteCommand insert = new SQLiteCommand(string.Format("UPDATE a3_player_info SET player_name = '{0}', infantry_kills = infantry_kills + {1}, soft_vehicle_kills = soft_vehicle_kills + {2}, armor_kills = armor_kills + {3}, air_kills = air_kills + {4}, deaths = deaths + {5}, total_score = total_score + {6} WHERE player_id = '{7}'", args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[2]), dbConnection);
                    int row = insert.ExecuteNonQuery();
                    insert.Cancel();
                    return row;
                }
            }
            catch { }
            return 0;
        }


        public static int InsertObjectNum(int id,string[] args) {

            alive();
            try
            {
                string sql = string.Format("INSERT INTO main.a3_object_manipulation_num(server_id, data_key, all_player, all_units, all_car, all_helicopter, all_motorcycle, all_plane, all_ship, all_static_weapon, all_apc, all_tank, all_units_uav, all_mission_objects, all_dead_men, all_groups, all_mines,fps,fps_min,create_time,create_time_timestamp) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}','{18}','{19}','{20}')", id, args[0], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14], args[15], args[16], (int)Convert.ToDouble(args[17]), (int)Convert.ToDouble(args[18]) , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") ,GetTimeStamp());
                SQLiteCommand cmd = new SQLiteCommand(sql, dbConnection);
                return cmd.ExecuteNonQuery();
            }
            catch { }
            return 0;
        }
        public static int GetAndCreateServer(string serverName) {
            alive();
            try
            {
                string sql = string.Format("SELECT id from a3_servers where server_name = '{0}'", serverName);
                SQLiteCommand cmd = new SQLiteCommand(sql, dbConnection);
                if (dbConnection.State == ConnectionState.Open)
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    bool isExist = reader.Read();
                    if (!isExist)
                    {
                        SQLiteCommand insert = new SQLiteCommand(string.Format("INSERT INTO a3_servers(server_name) VALUES ('{0}')", serverName), dbConnection);
                        insert.ExecuteNonQuery();
                        insert.Cancel();
                        insert.Clone();
                        cmd.Clone();
                        reader.Close();
                        reader = cmd.ExecuteReader();
                        int id = reader.Read() ? reader.GetInt32(0) : 0;
                        reader.Close();
                        cmd.Cancel();
                        return id;
                    }
                    else
                    {
                        return reader.GetInt32(0);
                    }
                }
            }
            catch {  }
            return 0;
        }

        public static void InsertObjectManipulationNum() { 

        }

        public static void alive()
        {
            if (dbConnection == null) {
                init();
                return;
            }
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Close();
                init();
            }
        }

    }
}
