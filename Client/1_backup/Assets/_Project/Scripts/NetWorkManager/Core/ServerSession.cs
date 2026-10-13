using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerCore;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace network
{
    class ServerSession : PacketSession
    {

        public override void OnConnected(EndPoint endPoint)
        {
          
            GameManager.instance.OnServerConnect = true;
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
          
            GameManager.instance.OnServerConnect = false;
            //GameManager.instance.Account = null;
            //GameManager.instance.ActiveError("서버와의 연결이 끊어졌습니다.");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }




}