using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using ServerCore;
using System;
using static C_SaveAccountData;

namespace network
{
    public class NetworkManager : MonoBehaviour
    {

        public List<IPacket> PacketList = new List<IPacket>();
        public IEnumerator corutine;
      
        ServerSession _session = new ServerSession();
        public void Connect()
        {
            if (_session._disconnected == 1)
                _session = new ServerSession();
            string host = Dns.GetHostName();
            IPHostEntry iphost = Dns.GetHostEntry(host);

            IPAddress ipaddr = iphost.AddressList[0];

            IPEndPoint endPoint = new IPEndPoint(ipaddr, 7777);
            Connector _connector = new Connector();
            _connector.Connect(endPoint, () => { return _session; }, 1);
        }
        public void Send(ArraySegment<byte> segment)
        {
            _session.Send(segment);
        }
        public void DisconnectSession()
        {
            _session.Disconnect();
        }
        public bool CheckNetwork()
        {
            return _session.socket();
        }

        // Update is called once per frame
        void Update()
        {
            List<IPacket> packetlist = PacketQueue.instance.PopAll();
            foreach (IPacket packet in packetlist)
            {
                PacketManager.Instance.HandlePacket(_session, packet);
            }

        }
        private void OnApplicationQuit()
        {
            DisconnectSession();
        }
    }


    public class UnityNetWorkHandler
    {


        NetworkManager network;
        public UnityNetWorkHandler(NetworkManager n)
        {
            try
            {
                network =n;
            }
            catch
            {
                Debug.Log("Crtical Error Can't find NetworkController");
            }
        }
 
        public void ConnectServer()
        {
            network.Connect();

        }
        public IEnumerator TryCheckApplication(
            List<string> bundleList)
        {
            C_ApplicationVersionCheck c = new C_ApplicationVersionCheck();
            c.Version = Application.version;

            c.BundleVersionList = bundleList;


            float timer = 0;
            network.Send(c.Write());

            while (true)
            {
                if (GameManager.instance.ServerMessage == "Application Version Check Complete"||
                    GameManager.instance.ServerMessage == "Asset Download Need")
                {
                  
                   
                    yield break;
                }
                else
                {
                    timer += Time.fixedDeltaTime;
                    if (timer > ServerRespondWaitingTime)
                    {
                        Debug.Log("대기 시간 초과");
                        GameManager.instance.ActiveError("서버와의 연결이 " +
                                "원활하지 않습니다.");
                        yield break;
                    }
                    yield return null;
                }
            }
        }


        public void DisConnectServer()
        {
            network.DisconnectSession();

        }
        float ServerRespondWaitingTime = 20;
        public IEnumerator TryConnectAccount(string UID)
        {

            C_ConnectRequest connectrequest = new C_ConnectRequest() { UID = UID };//연결요청 패킷을 보낸다
            float timer = 0;
            network.Send(connectrequest.Write());

            while (true)
            {
                if (GameManager.instance.ServerMessage == "account Load Complete" && GameManager.instance.Account != null)
                {
                    Debug.Log("계정 불려오기 성공");
                    GameManager.instance.ServerMessage = null;
                    yield break;
                }
                else
                {
                    timer += Time.fixedDeltaTime;
                    if (timer > ServerRespondWaitingTime)
                    {
                        Debug.Log("대기 시간 초과");
                        GameManager.instance.ActiveError("서버와의 연결이 " +
                               "원활하지 않습니다.");
                        yield break;
                    }
                    yield return null;
                }
            }
        }
        public IEnumerator TryConnectAccount(string UID,UserAccountData account)
        {
            float timer = 0;
            if (!GameManager.instance.OnServerConnect)
            {
                network.Connect();

                while (true)
                {
                    if (GameManager.instance.OnServerConnect)
                    {
                        break;
                    }
                    else
                    {
                        timer += Time.fixedDeltaTime;
                        if (timer > ServerRespondWaitingTime)
                        {
                            Debug.Log("대기 시간 초과 통신 오류");
                            //GameManager.instance.ActiveError("서버와의 연결이 " +
                            //    "원활하지 않습니다.");

                            yield break;
                        }
                    }

                }

            }
            C_ConnectRequest connectrequest = new C_ConnectRequest() { UID = UID };
          timer = 0;
            network.Send(connectrequest.Write());

            while (true)
            {
                if (GameManager.instance.ServerMessage == "account Load Complete" && GameManager.instance.Account != null)
                {
                    Debug.Log("계정 불려오기 성공");
                    GameManager.instance.ServerMessage = null;
                    yield break;
                }
                else
                {
                    timer += Time.fixedDeltaTime;
                    if (timer > ServerRespondWaitingTime)
                    {
                        Debug.Log("대기 시간 초과");
                        GameManager.instance.ActiveError("서버와의 연결이 " +
                               "원활하지 않습니다.");
                        yield break;
                    }
                    yield return null;
                }
            }
        }
        C_SaveAccountData accountpacket = new C_SaveAccountData();
        public IEnumerator TrySaveShopList(int shopid, List<ShopItem> list, string uid)
        {
            float timer = 0;
            if (!GameManager.instance.OnServerConnect)
            {
                network.Connect();
             
                while (true)
                {
                    if (GameManager.instance.OnServerConnect)
                    {
                        break;
                    }
                    else
                    {
                        timer += Time.fixedDeltaTime;
                        if (timer > ServerRespondWaitingTime)
                        {
                            Debug.Log("대기 시간 초과 통신 오류");
                            GameManager.instance.ActiveError("서버와의 연결이 " +
                                "원활하지 않습니다.");

                            yield break;
                        }
                    }
                   
                }
 
            }
            C_SaveShopPacket packet = new C_SaveShopPacket() { 
            shopid=shopid,rerollcount=0,itemlist=list,UID=uid,account=GameManager.instance.Account.ReturnNetworkMember()
                                       };
            network.Send(packet.Write());
          timer = 0;
            while (true)
            {
                if (GameManager.instance.ServerMessage == "ShopList Save Compete")
                {
                    Debug.Log("상점 저장 성공");
                    GameManager.instance.ServerMessage = null;
                    yield break;
                }
                else
                {
                    timer += Time.fixedDeltaTime;
                    if (timer > ServerRespondWaitingTime)
                    {
                        Debug.Log("대기 시간 초과 통신 오류");
                        GameManager.instance.ActiveError("서버와의 연결이 " +
                            "원활하지 않습니다.");
                        
                        yield break;
                    }
                    yield return null;
                }
            }
            //rerollcount 사용 안함

        }
        
            public IEnumerator TryRequestShopList(int shopid,string uid)
        {
            float timer = 0;
            if (!GameManager.instance.OnServerConnect)
            {
                network.Connect();

                while (true)
                {
                    if (GameManager.instance.OnServerConnect)
                    {
                        break;
                    }
                    else
                    {
                        timer += Time.fixedDeltaTime;
                        if (timer > ServerRespondWaitingTime)
                        {
                            Debug.Log("대기 시간 초과 통신 오류");
                            GameManager.instance.ActiveError("서버와의 연결이 " +
                                "원활하지 않습니다.");

                            yield break;
                        }
                    }

                }

            }
            C_RequestShopList pkt = new C_RequestShopList();
            pkt.UID =uid;
            pkt.shopid = shopid;
            network.Send(pkt.Write());
            timer = 0;
            while (true)
            {
                if (GameManager.instance.ServerMessage == "ShopList Load Complete")
                {
                    Debug.Log("상점 불려오기 성공");
                    GameManager.instance.ServerMessage = null;
                    yield break;
                }
                else
                {
                    timer += Time.fixedDeltaTime;
                    if (timer > ServerRespondWaitingTime)
                    {
                        Debug.Log("대기 시간 초과 통신 오류");
                        GameManager.instance.ActiveError("서버와의 연결이 " +
                            "원활하지 않습니다.");
                        yield break;
                    }
                    yield return null;
                }
            }
        }
        public IEnumerator TrySaveAccount(UserAccountData data,Action act)
        {
            float timer = 0;
            if (!GameManager.instance.OnServerConnect)
            {
                network.Connect();

                while (true)
                {
                    if (GameManager.instance.OnServerConnect)
                    {
                        break;
                    }
                    else
                    {
                        timer += Time.fixedDeltaTime;
                        if (timer > ServerRespondWaitingTime)
                        {
                            Debug.Log("대기 시간 초과 통신 오류");
                            GameManager.instance.ActiveError("서버와의 연결이 " +
                                "원활하지 않습니다.");

                            yield break;
                        }
                    }

                }

            }
            //통신 확인
            if (act!=null)
                act.Invoke();
            accountpacket.d=data.ReturnNetworkMember();
            network.Send(accountpacket.Write());
           timer = 0;
            while (true)
            {
                if (GameManager.instance.ServerMessage == "account Save Complete" && GameManager.instance.Account != null)
                {
                    Debug.Log("계정 저장 성공");
                    GameManager.instance.ServerMessage = null;
                    yield break;
                }
                else
                {
                    timer += Time.fixedDeltaTime;
                    if (timer > ServerRespondWaitingTime)
                    {
                        Debug.Log("대기 시간 초과 통신 오류");
                        GameManager.instance.ActiveError("서버와의 연결이 " +
                            "원활하지 않습니다.");
                        yield break;
                    }
                    yield return null;
                }
            }
        }

        public IEnumerator TryCreateAccount(string name)
        {
            float timer = 0;
            if (!GameManager.instance.OnServerConnect)
            {
                network.Connect();

                while (true)
                {
                    if (GameManager.instance.OnServerConnect)
                    {
                        break;
                    }
                    else
                    {
                        timer += Time.fixedDeltaTime;
                        if (timer > ServerRespondWaitingTime)
                        {
                            Debug.Log("대기 시간 초과 통신 오류");
                            GameManager.instance.ActiveError("서버와의 연결이 " +
                                "원활하지 않습니다.");

                            yield break;
                        }
                    }

                }

            }
            C_CreateAccountData pkt = new C_CreateAccountData();
            pkt.name=name;

            network.Send(pkt.Write());
           timer = 0;
            while (true)
            {
                if (GameManager.instance.ServerMessage == "account create Complete" && GameManager.instance.Account != null)
                {
                    Debug.Log("계정 생성 성공");
                    GameManager.instance.ServerMessage = null;
                    yield break;
                }
                else
                {
                    timer += Time.fixedDeltaTime;
         
                    if (timer > ServerRespondWaitingTime)
                    {
                        Debug.Log("대기 시간 초과");
                        GameManager.instance.ActiveError("서버와의 연결이 " +
                          "원활하지 않습니다.");
                        yield break;
                    }
                    yield return null;
                }
            }
        }
        public void updateAccount(UserAccountData data)//동기화 영어 찿아서 수정
        {

        }

    }
}