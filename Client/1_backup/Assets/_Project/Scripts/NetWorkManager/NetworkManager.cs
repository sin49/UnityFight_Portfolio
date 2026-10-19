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
      
        public string ServerHost = "127.0.0.1";
        public int ServerPort = 7777;
        ServerSession _session = new ServerSession();
        Connector connector;
        volatile bool connecting;
        volatile string connectionError;
        public bool IsConnected { get { return _session.IsConnected; } }
        public string ConnectionError { get { return connectionError; } }

        public void Connect()
        {
            if (GameManager.instance.singlemode || IsConnected || connecting) return;
            IPAddress address;
            if (ServerHost == "localhost") address = IPAddress.Loopback;
            else if (!IPAddress.TryParse(ServerHost, out address))
                throw new ArgumentException("ServerHost must be localhost or an explicit IP address.");
            connectionError = null;
            connecting = true;
            var attempt = new ServerSession();
            _session = attempt;
            connector = new Connector();
            connector.Connect(new IPEndPoint(address, ServerPort), () => attempt, 1,
                error => { if (ReferenceEquals(_session, attempt)) { connectionError = error; connecting = false; } });
        }

        public void Send(ArraySegment<byte> segment)
        {
            if (GameManager.instance.singlemode || !IsConnected)
                throw new InvalidOperationException("Online session is not connected.");
            _session.Send(segment);
        }

        public void DisconnectSession()
        {
            if (connector != null) connector.Cancel();
            _session.IsConnected = false;
            _session.Disconnect();
            connecting = false;
            connectionError = null;
            PacketQueue.instance.PopAll();
            if (GameManager.instance != null) GameManager.instance.OnServerConnect = false;
        }

        public bool CheckNetwork() { return IsConnected; }

        void Update()
        {
            if (GameManager.instance == null) return;
            GameManager.instance.OnServerConnect = IsConnected;
            if (IsConnected) connecting = false;
            foreach (var received in PacketQueue.instance.PopAll())
            {
                // Late packets from a closed/replaced session cannot update a new account.
                if (GameManager.instance.singlemode || !ReferenceEquals(received.Session, _session)) continue;
                try
                {
                    Debug.Log("[Online] RECV " + received.Packet.Protocol);
                    PacketManager.Instance.HandlePacket(received.Session, received.Packet);
                }
                catch (Exception exception)
                {
                    GameManager.instance.ActiveError("Account response failed: " + exception.Message);
                }
            }
        }

        private void OnApplicationQuit() { DisconnectSession(); }

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
        public IEnumerator WaitForConnection()
        {
            if (GameManager.instance.singlemode) yield break;
            if (network.IsConnected) yield break;
            string failure = null;
            try { network.Connect(); }
            catch (Exception exception) { failure = exception.Message; }
            if (failure != null)
            {
                GameManager.instance.ActiveError("Connect failed: " + failure);
                yield break;
            }
            float deadline = Time.realtimeSinceStartup + ServerRespondWaitingTime;
            while (!network.IsConnected && network.ConnectionError == null && Time.realtimeSinceStartup < deadline)
                yield return null;
            if (!network.IsConnected)
            {
                string reason = network.ConnectionError ?? "Connection timeout";
                network.DisconnectSession();
                GameManager.instance.ActiveError("Connect failed: " + reason);
            }
            else
            {
                GameManager.instance.OnServerConnect = true;
                Debug.Log("[Online] TCP connected");
            }
        }

        IEnumerator WaitForAccountResponse(string expected, bool needsAccount)
        {
            float deadline = Time.realtimeSinceStartup + ServerRespondWaitingTime;
            while (!GameManager.instance.OnError)
            {
                if (GameManager.instance.ServerMessage == expected &&
                    (!needsAccount || GameManager.instance.Account != null))
                {
                    Debug.Log("[Online] " + expected);
                    GameManager.instance.ServerMessage = null;
                    yield break;
                }
                if (!network.IsConnected || Time.realtimeSinceStartup >= deadline)
                {
                    GameManager.instance.ActiveError("Account response timeout or connection closed.");
                    yield break;
                }
                yield return null;
            }
        }

        public IEnumerator TryConnectAccount(string UID)
        {
            if (GameManager.instance.singlemode) yield break;
            yield return WaitForConnection();
            if (!network.IsConnected) yield break;
            GameManager.instance.ServerMessage = null;
            GameManager.instance.Account = null;
            Debug.Log("[Online] SEND C_ConnectRequest");
            network.Send(new C_ConnectRequest { UID = UID }.Write());
            yield return WaitForAccountResponse("account Load Complete", true);
        }
        public IEnumerator TryConnectAccount(string UID,UserAccountData account)
        {
            yield return TryConnectAccount(UID);
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
            if (GameManager.instance.singlemode) yield break;
            yield return WaitForConnection();
            if (!network.IsConnected) yield break;
            if (act != null) act();
            GameManager.instance.ServerMessage = null;
            accountpacket.d = data.ReturnNetworkMember();
            Debug.Log("[Online] SEND C_SaveAccountData");
            network.Send(accountpacket.Write());
            yield return WaitForAccountResponse("account Save Complete", true);
        }

        public IEnumerator TryCreateAccount(string name)
        {
            if (GameManager.instance.singlemode) yield break;
            yield return WaitForConnection();
            if (!network.IsConnected) yield break;
            GameManager.instance.ServerMessage = null;
            GameManager.instance.Account = null;
            Debug.Log("[Online] SEND C_CreateAccountData");
            network.Send(new C_CreateAccountData { name = name }.Write());
            yield return WaitForAccountResponse("account create Complete", true);
        }
        public void updateAccount(UserAccountData data)//동기화 영어 찿아서 수정
        {

        }

    }
}