using System;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Server.script;
using ServerCore;


class Program
	{



		static Listener _listener = new Listener();
	static string getDBDirectory
	{
		get
		{
			string s = Directory.GetCurrentDirectory();
			int n = s.IndexOf("\\Server\\");
			return s.Substring(0, n) + "\\DBprototype";
		}
	}
	static UserAccountData LoadAccountTemplet()
	{
		string FilePath = getDBDirectory + "\\DataTemplet\\AccountTemplet.json";
		UserAccountData data;
		using (StreamReader r = new StreamReader(FilePath))
		{
			string json = r.ReadToEnd();
			data = JsonConvert.DeserializeObject<UserAccountData>(json);
		}
		return data;
	}

	static void Main(string[] args)
		{
			// DNS (Domain Name System)
			
		Console.WriteLine("서버 실행 중...");
		Console.WriteLine("데이터 저장 방식을 선택해주세요(0:파일 디렉토리 1:mysql)");
		int n= short.Parse(Console.ReadLine());
		if(n >2|| n < 0)
		{
			ServerSetting.Instance.DBSaveSetting = DBsavesetting.Directory;

            Console.WriteLine("범위 초과! 0번 설정으로 실행");
		}
		else
		{
            ServerSetting.Instance.DBSaveSetting = (DBsavesetting)n;
            Console.WriteLine($"{n}번 설정으로 실행");
        }
		Console.WriteLine("서버 실행 위치:" + Directory.GetCurrentDirectory());
		
		string s = Directory.GetCurrentDirectory();
        n = s.IndexOf("\\Server\\");
        if (ServerSetting.Instance.DBSaveSetting == DBsavesetting.Directory)
		{
			
			string DB = s.Substring(0, n) + "\\DBprototype";
			Console.WriteLine("DB 저장 위치:" + DB);
		}
		else
		{
			var tmp = ServerSetting.Instance.sqlSetting;
			Console.WriteLine(string.Format("SQL 서버 정보\n" +
				"서버IP:{0}\n" +
				"사용자ID:{1}\n" +
				"비밀번호:{2}\n" +
				"데이터베이스:{3}\n" +
				"포트:{4}",tmp.Serverip,tmp.UserID,tmp.password,tmp.Database,tmp.port));
		}


        string ApplicationVersiontxt = s.Substring(0, n) + "\\ApplicationVersion.txt";
		string FileText = File.ReadAllText(ApplicationVersiontxt);

        string version = FileText.Substring(FileText.IndexOf(":"), FileText.IndexOf("-", 1) - FileText.IndexOf(":"));
		Console.WriteLine("버젼" + version);

		Console.WriteLine("로그)");
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        _listener.Init(endPoint, () =>
        {
            return SessionManager.Instance
.Generate();
        });
        while (true)
			{
			Thread.Sleep(1000);
	
			ShopDBManager.UpdateShopDBFile();
			}
		
		}
	}

