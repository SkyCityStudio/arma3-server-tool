
using System.Data.SQLite;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using a3.Entity;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using a3.Tools;

namespace a3.Config
{
    public class SqliteUtils
    {
       public static SQLiteConnection dbConnection = null;

        public static void init()
        {
            try
            {
                if (dbConnection == null || dbConnection.State != ConnectionState.Open)
                {
                    //Environment.CurrentDirectory 
                    string dbPath = @"Data Source = " + Environment.CurrentDirectory+ @"\destiny_players.db; version=3;Compress=True;";
                    string filePath = Environment.CurrentDirectory +  @"\destiny_players.db";

                      if (!File.Exists(filePath))
                      {
                          SQLiteConnection.CreateFile(filePath);
                      }
                      
                     Console.WriteLine(dbPath);
                     dbConnection = new SQLiteConnection(dbPath);
                   



                    dbConnection.Open();//打开数据库，若文件不存在会自动创建  

                    //dbConnection.StateChange += Conn_StateChange;
                    DataDBExists();





                }
            }
            catch
            {
             
            }

        }

        private static void DataDBExists()
        {
            string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (!File.Exists(path + @"sql\destiny_players.sql"))
            {
                XtraMessageBox.Show("建表失败!当前开服工具根目录下 sql -> destiny_players.sql不存在!", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            FileStream fs = new FileStream(path + @"sql\destiny_players.sql", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, CfgTool.UTF8);
            string sql = sr.ReadToEnd();
            sr.Close();
            fs.Close();
            SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, dbConnection);
            cmdCreateTable.ExecuteNonQuery();
        }

        private static void Conn_StateChange(object sender, StateChangeEventArgs e)
        {

            if(e.CurrentState == ConnectionState.Closed)
            {
                init();
            }
            throw new NotImplementedException();
        }

        public static int QueryPlayerCount(string Guid)
        {
            
            isLife();
            string sql = string.Format("SELECT count(*) FROM destiny_players where guid = '{0}'", Guid);
            SQLiteCommand cmd = new SQLiteCommand(sql, dbConnection);
            int count = 0;
            if (dbConnection.State == ConnectionState.Open)
            {
                SQLiteDataReader reader = cmd.ExecuteReader();

              
                while (reader.Read())
                {
                    count = count + reader.GetInt32(0);
                }
            }
           
            return count;
        }


        public static int InsertPlayerData(string guid,string playerName,string ip,string createDate)
        {
            isLife();
            int count = 0;
            if (dbConnection.State == ConnectionState.Open)
            {
                string sql = string.Format("INSERT INTO destiny_players (\"guid\", \"player_name\", \"ip\", \"create_date\") VALUES ('{0}', '{1}', '{2}', '{3}')", guid, playerName, ip, createDate);
                SQLiteCommand cmd = new SQLiteCommand(sql, dbConnection);
                count =  cmd.ExecuteNonQuery();
            }
           
            return count;
        }


        public static int UpdatePlayerData(string guid, string playerName, string ip, string createDate)
        {
            isLife();
            int count = 0;
            if (dbConnection.State == ConnectionState.Open)
            {
                string sql = string.Format("UPDATE destiny_players SET \"player_name\" = '{0}', \"ip\" = '{1}', \"create_date\" = '{2}' WHERE guid = '{3}'", playerName, ip, createDate, guid);
                SQLiteCommand cmd = new SQLiteCommand(sql, dbConnection);
                count = cmd.ExecuteNonQuery();
            }
           
            return count;
        }

        public static void SelectPlayerAll()
        {
            isLife();
            string sql = string.Format("SELECT * FROM destiny_players");
            SQLiteCommand cmd = new SQLiteCommand(sql, dbConnection);

            Console.WriteLine("READ>>>>>>>>" + dbConnection.State);

            if (dbConnection.State == ConnectionState.Open)
            {
                SQLiteDataReader reader = cmd.ExecuteReader();
                DefaultConfig.PlayerInfo.Clear();
                while (reader.Read())
                {
                  
                    PlayerDB pdb = new PlayerDB(reader.GetInt32(0), reader.GetString(3), reader.GetString(1), reader.GetString(2),  reader.GetString(4));
                    DefaultConfig.PlayerInfo.Add(pdb);
                }
            }

        }

        public static void isLife()
        {
            if(dbConnection.State != ConnectionState.Open)
            {
               
                dbConnection.Close();
                init();
            }
        }

        public static void Close()
        {

            dbConnection.Close();

        }
    }
}
