using System;
using ServerCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using network;
using UnityEngine;
class PacketManager
{
    #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance { get { return _instance; } }
    #endregion

    PacketManager()
    {
        Register();
    }
    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>
        _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler =
        new Dictionary<ushort, Action<PacketSession, IPacket>>();
    public void Register()
    {
        _makeFunc.Add((ushort)PacketID.S_LoadAccount, MakePacket<S_LoadAccount>);
        _handler.Add((ushort)PacketID.S_LoadAccount, PacketHandler.S_LoadAccountHandler);
        _makeFunc.Add((ushort)PacketID.S_Message, MakePacket<S_Message>);
        _handler.Add((ushort)PacketID.S_Message, PacketHandler.S_MessageHandler);
        _makeFunc.Add((ushort)PacketID.S_ErrorPacket, MakePacket<S_ErrorPacket>);
        _handler.Add((ushort)PacketID.S_ErrorPacket, PacketHandler.S_ErrorHandler);
        _makeFunc.Add((ushort)PacketID.S_LoadShopList, MakePacket<S_LoadShopList>);
        _handler.Add((ushort)PacketID.S_LoadShopList, PacketHandler.S_LoadShopListHandler);
    }
    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
   
        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if (_makeFunc.TryGetValue(id, out func))
        {
  
            IPacket packet = func.Invoke(session, buffer);
        
            if (onRecvCallback != null) {
                onRecvCallback.Invoke(session, packet);
             
            }
            else
                HandlePacket(session, packet);
        }
        else
        {
            Debug.Log("Can;t Find id" + id);
        }
    }
    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T pkt = new T();
        pkt.Read(buffer);
        return pkt;
    }

    public void HandlePacket(PacketSession session, IPacket packet)
    {
    
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(packet.Protocol, out action))
            action.Invoke(session, packet);
    }
}

