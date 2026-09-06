using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    class SessionManager
    {
		static SessionManager _session = new SessionManager();
		public static SessionManager Instance { get { return _session; } }

		int _sessioncount = 0;
	
		object _lock = new object();

		public ClientSession Generate()
		{
			lock (_lock)
			{


				ClientSession session = new ClientSession();



			Console.WriteLine($"Connected : {++_sessioncount}");

			return session;
			}
		}

	
		public void Remove(ClientSession session)
		{
		lock (_lock)
		{
			Console.WriteLine($"Connected : {--_sessioncount}");
		}
    }
	}

