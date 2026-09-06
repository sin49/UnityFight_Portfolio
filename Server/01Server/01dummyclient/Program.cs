using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;
namespace _01dummyclient
{
    class Program
    {
        static void Main(string[] args)
        {


                string host = Dns.GetHostName();
                IPHostEntry iphost = Dns.GetHostEntry(host);

                IPAddress ipaddr = iphost.AddressList[0];

                IPEndPoint endPoint = new IPEndPoint(ipaddr, 7777);

                Random rand = new Random();
                string s = rand.Next(0, 100).ToString();
          
                Connector _connector = new Connector();

            //_connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); }, 10);
            //SessionManager.Instance.SendForEach();


            while (true)
            {
                try
                {
                    _connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); }, 10);
                    SessionManager.Instance.SendForEach();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(3000);
            }

        }
    }
}
