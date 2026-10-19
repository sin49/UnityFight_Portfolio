using System;
using System.Net;
using ServerCore;

namespace network
{
    class ServerSession : PacketSession
    {
        public volatile bool IsConnected;

        public override void OnConnected(EndPoint endpoint) { IsConnected = true; }
        public override void OnDisconnected(EndPoint endpoint) { IsConnected = false; }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer,
                (session, packet) => PacketQueue.instance.Push(session, packet));
        }

        public override void OnSend(int numOfBytes) { }
    }
}
