using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _01Server
{
    class Program
    {
        static Listner listner = new Listner();

       
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry iphost = Dns.GetHostEntry(host);

            IPAddress ipaddr = iphost.AddressList[0];

            IPEndPoint endPoint = new IPEndPoint(ipaddr, 4444);
            listner.Init(endPoint, ()=> { return new gamesession(); });
            Console.WriteLine("Listne...");


            while (true)
            {
                ;

            }
        }
    }
}
