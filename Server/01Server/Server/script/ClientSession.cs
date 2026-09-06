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
        PacketManager.Instance.OnRecvPacket(this, buffer);
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
    
