using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

    class ClientSession : PacketSession
    {



    public override void OnDisconnected(EndPoint endPoint)
        {
        SessionManager.Instance.Remove(this);
        
            Console.WriteLine($"[Disconnected] {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
        try
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }
        catch (Exception exception)
        {
            // A failed account/DB request must produce a response, not stop receiving.
            Console.WriteLine("Request failed: " + exception.GetType().Name + ": " + exception.Message);
            var error = new S_ErrorPacket { Error = "Server account request failed; check server log." };
            Send(error.Write());
        }
    }

        public override void OnSend(int numofByte)
        {

        }
        //처리할 컨텐츠
       
             public override void OnConnected(EndPoint endpoint)
        {
        Console.WriteLine($"OnConnected : {endpoint}");
    }
    }
    
