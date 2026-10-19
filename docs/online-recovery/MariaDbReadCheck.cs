using System;
using MySqlConnector;
using Newtonsoft.Json.Linq;
using Server.script;

class MariaDbReadCheck
{
    static void Main(string[] args)
    {
        using var connection = ServerSetting.Instance.sqlSetting.conn;
        connection.Open();
        using (var info = new MySqlCommand("SELECT CURRENT_USER(), DATABASE(), @@port;", connection))
        using (var reader = info.ExecuteReader())
        {
            if (!reader.Read()) throw new Exception("No connection identity");
            Console.WriteLine("Authenticated user: " + reader.GetString(0));
            Console.WriteLine("Database: " + reader.GetString(1));
            Console.WriteLine("Port: " + reader.GetValue(2));
        }
        using var command = new MySqlCommand("SELECT accountjson FROM accountinfojson WHERE uid=@uid;", connection);
        command.Parameters.AddWithValue("@uid", args[0]);
        var json = command.ExecuteScalar() as string;
        if (json == null) throw new Exception("Test account row missing");
        var row = JObject.Parse(json);
        if ((string)row["_uid"] != args[0] ||
            (string)row["username"] != "client_transport_saved" ||
            (int)row["_Exp"] != 41)
            throw new Exception("Persisted test account does not match saved values");
        Console.WriteLine("Verified UID: " + args[0]);
        Console.WriteLine("Stored username: " + (string)row["username"]);
        Console.WriteLine("Stored EXP: " + (int)row["_Exp"]);
        Console.WriteLine("PASS independent read-only MariaDB SELECT confirms TCP save");
    }
}
