using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace _01dummyclient
{
    class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }
        List<ServerSession> _sessions = new List<ServerSession>();
        object _lock = new object();
        public void SendForEach()
        {
            lock (_lock)
            {
                foreach (ServerSession session in _sessions)
                {
                    C_CreateAccountData testpacket = new C_CreateAccountData()
                    {
                        name = "servertest"
                    };
                    session.Send(testpacket.Write());
                    session.Disconnect();
                }
            }
        }

        public void Send(string name)
        {
            lock (_lock)
            {
               

                    foreach (ServerSession session in _sessions)
                {

                    C_ApplicationVersionCheck d1= new C_ApplicationVersionCheck();
                    d1.Version = "0.1";
                    d1.BundleVersionList = new List<string>();
                    d1.BundleVersionList.Add("0.1");
                    session.Send(d1.Write());
                    Console.Write("보낸다!0");
                 
                    
                    //session.Disconnect();
                }

            }
        }

        public ServerSession Generate()
        {
            lock (_lock)
            {
                ServerSession session = new ServerSession();
                _sessions.Add(session);
              
                return session;
            }
        }
    }
}
