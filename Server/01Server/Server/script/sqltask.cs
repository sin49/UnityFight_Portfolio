using MySqlConnector;
using Newtonsoft.Json;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.script
{
    public class SqlTask
    {
        #region sql
       public static UserAccountData LoadDBsql(string _UId, MySqlConnection conn)
        {
            lock (_lock)
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                try
                {
                    using var command = new MySqlCommand("SELECT accountjson FROM accountinfojson WHERE uid=@uid;", conn);
                    command.Parameters.AddWithValue("@uid", _UId);
                    var value = command.ExecuteScalar();
                    return value == null || value == DBNull.Value ? null :
                        JsonConvert.DeserializeObject<UserAccountData>((string)value);
                }
                finally { conn.Close(); }
            }
        }
        public static void DeleteDBsql(string _UId, MySqlConnection conn)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            string s = string.Format("DELETE FROM accountinfojson WHERE iod = \"{0}\";", _UId);
            MySqlCommand cmd = new MySqlCommand(s, conn);
            lock (_lock)
            {
                cmd.ExecuteNonQuery();
            }
            conn.Close();
            Console.WriteLine(_UId + "Delete Complete");

        }

        public static void UpdateDBUID(UserAccountData data, MySqlConnection conn)//insert- on dulicate key update 문
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            string json = JsonConvert.SerializeObject(data);
            string s = string.Format("INSERT into Accountinfojson(uid,accountjson) VALUES(\"{0}\",'{1}')" +
                "\r\nON DUPLICATE KEY UPDATE accountjson='{1}';", data._uid, json);
            MySqlCommand mySqlCommand = new MySqlCommand(s, conn);
            lock (_lock)
            {
                mySqlCommand.ExecuteNonQuery();
            }
            conn.Close();
        }

        public static List<ShopItemListforsql> getshopdb(MySqlConnection conn)
        {
            conn.Open();
            string s = string.Format("SELECT * FROM dailyshopdb;");
            MySqlDataAdapter adapter = new MySqlDataAdapter(s, conn);

            DataSet dataSet = new DataSet();
            lock (_lock)
            {
                adapter.Fill(dataSet);
            }
            conn.Close();
            List<ShopItemListforsql> list = new List<ShopItemListforsql>();

            foreach (DataRow r in dataSet.Tables[0].Rows)
            {
                ShopItemListforsql obj = new ShopItemListforsql()
                {
                    Item = new ShopItem() { Itemid = (string)r["itemid"], NeedItemid = (string)r["needitemID"], Amount = (int)r["Amount"], price = (int)r["price"] },
                    weight = (SByte)r["weight"]
                };
                list.Add(obj);
            }

            return list;
        }
        public static void insertDailyshopplayerdb(string uid, ShopListCache shoplist, MySqlConnection conn)
        {
            conn.Open();

            var json = JsonConvert.SerializeObject(shoplist);
            string s = string.Format("INSERT into accountdailyshopdb(UID,ShopListjson) VALUES(\"{0}\",'{1}');", uid, json);
            MySqlCommand mySqlCommand = new MySqlCommand(s, conn);
            lock (_lock)
            {
                mySqlCommand.ExecuteNonQuery();
            }
            conn.Close();

        }
        public static ShopListCache LoadDailyshopplayerdb(string uid, MySqlConnection conn)
        {
            conn.Open();

            string s = string.Format("SELECT ShopListjson AS shoplist FROM accountdailyshopdb WHERE uid=\"{0}\";", uid);
            MySqlDataAdapter myDataAdapter = new MySqlDataAdapter(s, conn);
            DataSet dataSet = new DataSet();
            lock (_lock)
            {
                myDataAdapter.Fill(dataSet);
            }
            conn.Close();


            string shopjson = "";
            foreach (DataRow r in dataSet.Tables[0].Rows)
            {

                shopjson = (string)r["shoplist"];
            }
            try
            {
                return JsonConvert.DeserializeObject<ShopListCache>(shopjson);
            }
            catch
            {
                return null;
            }


        }
        public static void updateDailyshopplayerdb(string uid, ShopListCache shoplist, MySqlConnection conn)
        {
            conn.Open();

            var json = JsonConvert.SerializeObject(shoplist);
            string s = string.Format("UPDATE accountdailyshopdb SET ShopListjson='{1}; WHERE UID=\"{0}\";", uid, json);
            MySqlCommand mySqlCommand = new MySqlCommand(s, conn);
            lock (_lock)
            {
                mySqlCommand.ExecuteNonQuery();
            }
            conn.Close();
            //s = string.Format("INSERT into Accountinfojson(uid,accountjson) VALUES(\"{0}\",'{1}');", jsondata._uid, json);
        }
        public static void UpdateDBUID2(UserAccountData data, MySqlConnection conn)//update문
        {
            lock (_lock)
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                try
                {
                    using var command = new MySqlCommand("UPDATE accountinfojson SET accountjson=@json WHERE uid=@uid;", conn);
                    command.Parameters.AddWithValue("@uid", data._uid);
                    command.Parameters.AddWithValue("@json", JsonConvert.SerializeObject(data));
                    if (command.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException("Account save did not update an existing account.");
                }
                finally { conn.Close(); }
            }
        }
        static object _lock = new object();
        public static UserAccountData insertDBsql(MySqlConnection conn, UserAccountData jsondata)
        {
            lock (_lock)
            {
                if (conn.State != ConnectionState.Open) conn.Open();
                try
                {
                    // Generate a UID in C#; do not DROP/CREATE a DB procedure.
                    for (int attempt = 0; attempt < 5; attempt++)
                    {
                        jsondata._uid = Guid.NewGuid().ToString("N").Substring(0, 12);
                        using var command = new MySqlCommand("INSERT INTO accountinfojson(uid,accountjson) VALUES(@uid,@json);", conn);
                        command.Parameters.AddWithValue("@uid", jsondata._uid);
                        command.Parameters.AddWithValue("@json", JsonConvert.SerializeObject(jsondata));
                        try { command.ExecuteNonQuery(); return jsondata; }
                        catch (MySqlException exception) when (exception.Number == 1062) { }
                    }
                    throw new InvalidOperationException("Could not allocate a unique account UID.");
                }
                finally { conn.Close(); }
            }
        }

        #endregion
    }



    //public class SqlTask
    //{

    //    //비동기 task (테스트 필요)
    //    //실험 결과 가능
    //    //1번 설정과 2번 설정 중 뭐가 더 나은지는 잘 모르겠음
    //    //2번 설정은 아마도 더 많은 스레드가 투자 될려나?
    //    //스레드 많다-> 부하 많이 걸림
    //    //성능이 떨어져야 하지만....
    //    //- 10개의 계정을 짧은 시간안에 생성시키게 했을 때
    //    //1번 설정 2번 설정 모두 경합 조건으로 오류가 발생
    //    //1번은 lock을 사용하여 해결 가능
    //    //2번은 모르겠음...
    //    //1번은 패킷을 받는 과정에서 멀티스레드 작동
    //    //이 스레드들이 각각 처리함
    //    //2번은 패킷을 받는 스레드가 데이터를 처리하는데 스레드를 추가로 씀
    //    //task로 스레드를 생성하는데 부하가 더 걸리는데

    //    //lock을 쓰면 1번과 차이점이 거의 없는디?
    //    //그러면 스레드만 많이 쓰고 과정이 거의 똑같으면
    //    //1번이 더 좋다가 결론?
    //    //task 생성에 lock 걸어도
    //    //안되네?
    //    //2번 왜 씀?
    //    ///결론 2번 쓰지마 바보 같은 짓이였음 <summary>
    //    /// 결론 2번 쓰지마 바보 같은 짓이였음
    //    /// 

    //    /// </summary>
    //    /// <param name="_UId"></param>
    //    /// <param name="conn"></param>
    //    /// <param name="completed"></param>
    //    /// <returns></returns>
    //    public static async Task LoadDBsql(string _UId, MySqlConnection conn, Action<UserAccountData> completed)
    //    {

    //        conn.Open();
    //        string s = string.Format("SELECT accountjson AS accountdata FROM accountinfojson where uid=\"{0}\";", _UId);
    //        MySqlDataAdapter adapter = new MySqlDataAdapter(s, conn);
    //        DataSet ds = new DataSet();

    //        await Task.Run(() => { adapter.Fill(ds); });

    //        conn.Clone();
    //        string accountjson = "";
    //        foreach (DataRow r in ds.Tables[0].Rows)
    //        {
    //            accountjson = (string)r["accountdata"];
    //        }
    //        var account = JsonConvert.DeserializeObject<UserAccountData>(accountjson);
    //        completed.Invoke(account);
    //    }
    //    static async Task DeleteDBsql(string _UId, MySqlConnection conn, Action completed)
    //    {
    //        conn.Open();
    //        string s = string.Format("DELETE FROM accountinfojson WHERE iod = \"{0}\";", _UId);
    //        MySqlCommand cmd = new MySqlCommand(s, conn);
    //        await cmd.ExecuteNonQueryAsync();
    //        conn.Clone();
    //        Console.WriteLine(_UId + "Delete Complete");
    //        completed.Invoke();
    //    }

    //    public static async Task UpdateDBUIDAsync(UserAccountData data, MySqlConnection conn, Action completed)//insert- on dulicate key update 문
    //    {
    //        conn.Open();
    //        string json = JsonConvert.SerializeObject(data);
    //        string s = string.Format("INSERT into Accountinfojson(uid,accountjson) VALUES(\"{0}\",'{1}')" +
    //            "\r\nON DUPLICATE KEY UPDATE accountjson='{1}';", data._uid, json);
    //        MySqlCommand mySqlCommand = new MySqlCommand(s, conn);
    //        await mySqlCommand.ExecuteNonQueryAsync();
    //        conn.Clone();
    //        completed.Invoke();
    //    }
    //    public static async Task UpdateDBUID2(UserAccountData data, MySqlConnection conn, Action completed)//update문
    //    {
    //        conn.Open();
    //        string json = JsonConvert.SerializeObject(data);
    //        string s = string.Format("UPDATE accountinfojson SET accountjson ='{1}' WHERE uid=\"{0}\";", data._uid, json);
    //        MySqlCommand mySqlCommand = new MySqlCommand(s, conn);
    //        await mySqlCommand.ExecuteNonQueryAsync();
    //        conn.Clone();
    //        completed.Invoke();
    //    }
    //    public static async Task insertDBsql(MySqlConnection conn, UserAccountData jsondata, Action<UserAccountData> CompleteAction)
    //    {
    //        string uid = "";

    //        conn.Open();

    //        string s = string.Format("DROP PROCEDURE if EXISTS testwhileloop;" +

    //            "\r\nCREATE PROCEDURE testwhileloop()" +
    //            "\r\nBEGIN\r\n" +
    //            "DECLARE i INT DEFAULT 1;\r\n" +
    //            "DECLARE randomstring VARCHAR(12); " +
    //            "\r\nwhile i >0 DO" +
    //            "\r\nSET randomstring= (SELECT SUBSTR(MD5(RAND()),1,12));" +
    //            "\r\nSET i =(SELECT COUNT(*) AS count FROM accountinfojson WHERE uid=randomstring);" +
    //            "\r\nEND while;" +
    //            "SELECT randomstring AS uid;\r\nEND;" +
    //            "\r\nCALL testwhileloop();");
    //        MySqlDataAdapter adapter = new MySqlDataAdapter(s, conn);

    //        DataSet dataSet = new DataSet();

    //        adapter.Fill(dataSet);




    //        foreach (DataRow r in dataSet.Tables[0].Rows)
    //        {
    //            uid = (string)r["uid"];
    //        }
    //        jsondata._uid = uid;
    //        string json = JsonConvert.SerializeObject(jsondata);
    //        s = string.Format("INSERT into Accountinfojson(uid,accountjson) VALUES(\"{0}\",'{1}');", jsondata._uid, json);
    //        MySqlCommand cmd = new MySqlCommand(s, conn);
    //        await cmd.ExecuteNonQueryAsync();
    //        conn.Clone();
    //        CompleteAction.Invoke(jsondata);
    //    }
    //}
}
