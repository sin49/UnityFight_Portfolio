using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;
using static C_CreateAccountData;

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
        _makeFunc.Add((ushort)PacketID.C_SaveUserAccountData, MakePacket<C_SaveAccountData>);
        _handler.Add((ushort)PacketID.C_SaveUserAccountData, PacketHandler.   C_SaveAccountDataPacket);
        
        _makeFunc.Add((ushort)PacketID.C_CreateAccount, MakePacket<C_CreateAccountData>);
        _handler.Add((ushort)PacketID.C_CreateAccount, PacketHandler.C_CreateAccountHandler);
        _makeFunc.Add((ushort)PacketID.C_ConnectRequest, MakePacket<C_ConnectRequest>);
        _handler.Add((ushort)PacketID.C_ConnectRequest, PacketHandler.C_ConnectRequestHandler);
        _makeFunc.Add((ushort)PacketID.C_ApplicationVersionCheck, MakePacket<C_ApplicationVersionCheck>);
        _handler.Add((ushort)PacketID.C_ApplicationVersionCheck, PacketHandler.C_ApplicationVersionCheckHandler);
        _makeFunc.Add((ushort)PacketID.C_RequestShopList, MakePacket<C_RequestShopList>);
        _handler.Add((ushort)PacketID.C_RequestShopList, PacketHandler.C_RequestShopListHandler);
        _makeFunc.Add((ushort)PacketID.C_SaveShop, MakePacket<C_SaveShopPacket>);
        _handler.Add((ushort)PacketID.C_SaveShop, PacketHandler.C_SaveShopHandler);
    }
    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
    {
        ushort count = 0;
        ushort size= BitConverter.ToUInt16(buffer.Array, buffer.Offset );
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;
        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;


        Console.WriteLine("받은 id" + id);
        if (_makeFunc.TryGetValue(id, out func))
        {
            IPacket packet = func.Invoke(session, buffer);
            if (onRecvCallback != null)
                onRecvCallback.Invoke(session, packet);
            else
                HandlePacket(session, packet);
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

