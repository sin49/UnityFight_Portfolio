using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ServerCore;


    class PacketHandler
    {
    public static void S_MessageHandler(PacketSession session, IPacket packet)
    {
        S_Message pkt = packet as S_Message;
        Console.WriteLine("수신 메세지:" + pkt.message);
        //C_CreateAccountData d = new C_CreateAccountData();
        //d.name = "tester1234567";
        //session.Send(d.Write());
        //Console.Write("보낸다!");
    }
    public static void S_LoadAccountHandler(PacketSession session, IPacket packet)
    {
        S_LoadAccount pkt = packet as S_LoadAccount;
        Console.WriteLine($"{pkt.d._uid}의 계정을 불려오는데 성공했습니다.");

    }

}

