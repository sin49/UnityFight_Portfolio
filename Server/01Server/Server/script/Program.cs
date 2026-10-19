using System;
using System.Net;
using System.Threading;
using Server.script;
using ServerCore;

class Program
{
    static readonly Listener Listener = new Listener();

    static string Option(string[] args, string name, string fallback)
    {
        int index = Array.IndexOf(args, name);
        if (index < 0) return fallback;
        if (index + 1 >= args.Length) throw new ArgumentException("Missing value for " + name);
        return args[index + 1];
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string storage = Option(args, "--storage", "file");
        if (storage != "file" && storage != "mysql")
            throw new ArgumentException("--storage must be file or mysql");
        ServerSetting.Instance.DBSaveSetting = storage == "mysql" ? DBsavesetting.Mysql : DBsavesetting.Directory;
        if (storage == "mysql")
        {
            try { _ = ServerSetting.Instance.sqlSetting.password; }
            catch (InvalidOperationException exception)
            {
                Console.Error.WriteLine("Configuration error: " + exception.Message);
                Environment.ExitCode = 1;
                return;
            }
        }
        ServerPaths.Initialize(Option(args, "--data-root", null));

        string host = Option(args, "--host", "127.0.0.1");
        IPAddress address = host == "localhost" ? IPAddress.Loopback : IPAddress.Parse(host);
        int port = int.Parse(Option(args, "--port", "7777"));
        var endpoint = new IPEndPoint(address, port);
        Console.WriteLine("Storage: " + storage);
        Console.WriteLine("Data root: " + ServerPaths.Root);
        // Legacy application/bundle version download is outside the online account path.
        // No version file or database connection is required before listening.
        // MariaDB credentials are validated above; file mode needs no DB secret.
        Listener.Init(endpoint, () => SessionManager.Instance.Generate());
        Console.WriteLine("LISTEN " + endpoint);
        while (true) Thread.Sleep(1000);
    }
}
