using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;
using UnityEngine;
namespace network
{
    class PacketHandler
    {
        public static void S_LoadAccountHandler(PacketSession session, IPacket packet)
        {

            S_LoadAccount pkt = packet as S_LoadAccount;

            if (GameManager.instance.Account == null)
            {
                GameManager.instance.Account = new UserAccountData();
            }
            GameManager.instance.Account.GetNetworkMember( pkt.d);
       
            GameManager.instance.ServerMessage = pkt.ServerMessage;
        }
        
        public static void S_ErrorHandler(PacketSession session, IPacket packet)
        {
            S_ErrorPacket pkt = packet as S_ErrorPacket;
            GameManager.instance.ActiveError(pkt.Error);
        }
        public static void S_MessageHandler(PacketSession session, IPacket packet)
        {
            S_Message pkt = packet as S_Message;
        
            GameManager.instance.ServerMessage = pkt.message;
        }
        public static void S_DownloadHandler(PacketSession session, IPacket packet)
        {

            S_DownLoadPacket pkt = packet as S_DownLoadPacket;
            for(int n = 0; n < pkt.URL.Length; n++)
            {
                Debug.Log(pkt.FileName[n] + ":" + pkt.URL[n]);
            }
           
           DownLoadManager.instance.getDownloadInfo(pkt.URL, pkt.FileName,pkt.FileByte);
            GameManager.instance.ServerMessage = pkt.message;
        }
        public static void S_LoadShopListHandler(PacketSession session,IPacket packet)
        {

            S_LoadShopList pkt = packet as S_LoadShopList;
            GameManager.instance.CurrentShopList = pkt.itemlist;
                
            GameManager.instance.ServerMessage = pkt.message;
        }
        
    }

}