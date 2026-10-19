// Non-Unity smoke harness: links production Client transport/packet files.
// Unity API and account container are test doubles; this is not Unity Play.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using ServerCore;
using network;

namespace UnityEngine
{
    public class MonoBehaviour { }
    public static class Application { public static string version = "smoke"; }
    public static class Time
    {
        static readonly Stopwatch Clock = Stopwatch.StartNew();
        public static float realtimeSinceStartup { get { return (float)Clock.Elapsed.TotalSeconds; } }
        public static float fixedDeltaTime { get { return 0.02f; } }
    }
    public static class Debug { public static void Log(object message) { Console.WriteLine(message); } }
}
namespace UnityEngine.SceneManagement { }

public class UserAccountDataNetworkMember : ServerCore.UserAccountData { }
public class UserAccountData
{
    public UserAccountDataNetworkMember Member;
    public void GetNetworkMember(UserAccountDataNetworkMember data) { Member = data; }
    public UserAccountDataNetworkMember ReturnNetworkMember() { return Member; }
}
public class GameManager
{
    public static GameManager instance = new GameManager();
    public bool singlemode;
    public bool OnServerConnect;
    public bool OnError;
    public string ServerMessage;
    public UserAccountData Account;
    public List<ShopItem> CurrentShopList;
    public int MainThread = Thread.CurrentThread.ManagedThreadId;
    public void ActiveError(string message)
    {
        if (Thread.CurrentThread.ManagedThreadId != MainThread) throw new Exception("Unity API off main thread");
        OnError = true;
        Console.WriteLine("ERROR " + message);
    }
}
public class DownLoadManager
{
    public static DownLoadManager instance = new DownLoadManager();
    public void getDownloadInfo(string[] urls, string[] files, int bytes) { throw new Exception("Legacy download must not run"); }
}

class TransportSmoke
{
    static NetworkManager manager;
    static readonly MethodInfo Update = typeof(NetworkManager).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
    static void Run(IEnumerator routine)
    {
        var stack = new Stack<IEnumerator>();
        stack.Push(routine);
        var deadline = Stopwatch.StartNew();
        while (stack.Count > 0)
        {
            if (deadline.Elapsed.TotalSeconds > 25) throw new Exception("Smoke coroutine stalled");
            Update.Invoke(manager, null);
            var current = stack.Peek();
            if (!current.MoveNext()) { stack.Pop(); continue; }
            if (current.Current is IEnumerator) stack.Push((IEnumerator)current.Current);
            else Thread.Sleep(5);
        }
        Update.Invoke(manager, null);
        if (GameManager.instance.OnError) throw new Exception("Client signalled error");
    }
    static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    static void Main(string[] args)
    {
        manager = new NetworkManager { ServerPort = int.Parse(args[0]) };
        GameManager.instance.singlemode = true;
        manager.Connect();
        Require(!manager.IsConnected, "Standalone must not connect");
        GameManager.instance.singlemode = false;
        var handler = new UnityNetWorkHandler(manager);
        try
        {
            Run(handler.WaitForConnection());
            Require(manager.IsConnected, "TCP not connected");
            Run(handler.TryCreateAccount("client_transport_probe"));
            var data = GameManager.instance.Account;
            Require(data != null, "Missing account callback");
            string uid = data.Member._uid;
            data.Member.username = "client_transport_saved";
            data.Member._Exp = 41;
            Run(handler.TrySaveAccount(data, null));
            Run(handler.TryConnectAccount(uid));
            Require(GameManager.instance.Account.Member.username == "client_transport_saved", "Name did not persist");
            Require(GameManager.instance.Account.Member._Exp == 41, "EXP did not persist");
            manager.DisconnectSession();
            manager.DisconnectSession(); // Cancellation/ownership regression: must be idempotent.
            Run(handler.WaitForConnection());
            Run(handler.TryConnectAccount(uid));
            Require(GameManager.instance.Account.Member._Exp == 41, "Fresh session reload failed");
            manager.DisconnectSession();
            GameManager.instance.singlemode = true;
            manager.Connect();
            Require(!manager.IsConnected, "Standalone connected after switch");
            Console.WriteLine("PASS PRODUCTION CLIENT TRANSPORT create/save/reload/reconnect/cleanup + Standalone no-connect");
        }
        finally { manager.DisconnectSession(); }
    }
}
