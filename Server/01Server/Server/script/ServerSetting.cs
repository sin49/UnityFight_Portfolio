using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.script
{
    public enum DBsavesetting { Directory, Mysql }
    public class sqlSetting
    {
        public string Serverip = "127.0.0.1";
        public string UserID = "root";
        // Process environment only; never store DB credentials in source or logs.
        public string password
        {
            get
            {
                string value = Environment.GetEnvironmentVariable("UNITYFIGHT_DB_PASSWORD");
                if (string.IsNullOrEmpty(value))
                    throw new InvalidOperationException("MariaDB mode requires UNITYFIGHT_DB_PASSWORD. Set it in the server process environment before starting with --storage mysql.");
                return value;
            }
        }
        public string Database = "01database";
        public int port = 3306;
        MySqlConnection _conn;
        public MySqlConnection conn
        {
            get
            {
                if (_conn == null)
                {
                    var builder = new MySqlConnectionStringBuilder
                    {
                        Server = Serverip,
                        UserID = UserID,
                        Password = password,
                        Database = Database,
                        Port = 3306
                    };
                    _conn = new MySqlConnection(builder.ConnectionString);
                }
                return _conn;
            }
        }
    }

    public class ServerSetting
    {

        static ServerSetting _instance = new ServerSetting();
        public static ServerSetting Instance { get { return _instance; } }
        DBsavesetting _DBsaveSetting;
        public DBsavesetting DBSaveSetting
        {
            get { return _DBsaveSetting; }
            set
            {
                _DBsaveSetting = value;

            }
        }


        //1번 설정
        sqlSetting _sqlSetting;
        public sqlSetting sqlSetting
        {
            get
            {
                if (_sqlSetting == null)
                    _sqlSetting = new sqlSetting();

                return _sqlSetting;
            }
        }

    }
}
