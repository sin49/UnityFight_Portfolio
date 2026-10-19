using System;
using System.Collections.Generic;

using ServerCore;
using Newtonsoft.Json;

using System.IO;

using MySqlConnector;
using System.Data;

using Server.script;

class PacketHandler
    {
   

    #region handler

    public static void C_ApplicationVersionCheckHandler(PacketSession session, IPacket packet)
    {
        C_ApplicationVersionCheck d = packet as C_ApplicationVersionCheck;

  
  
        string directory = Directory.GetCurrentDirectory();
        int n = directory.IndexOf("\\Server\\");
        directory = directory.Substring(0, n);
        string s = File.ReadAllText(directory + "\\ApplicationVersion.txt");
        s = s.Replace("\n", "");
        string s2 = File.ReadAllText(directory + "\\ApplicationDownLoadURL.txt");
        s2 = s2.Replace("\n", "");
        int startindex1 = 0;
        int startindex2 = 0;
        int Downloadstartindex1 = 0;
        int Downloadstartindex2 = 0;
        int FIleByte = 0;
        int index1 = s.IndexOf(":", startindex1)+1;
        startindex2 = index1 + 1;
        int index2 = s.IndexOf("-", startindex2);
        string tmp;
        tmp = s.Substring(index1, index2 - index1);
        startindex1 = index1 + 1;

        int douwnloadindex1 = s2.IndexOf(":", Downloadstartindex1)+1;
        Downloadstartindex2 = Downloadstartindex1 + 1;
        int downloadindex2 = s2.IndexOf("DownLoadurl-", Downloadstartindex2);
 
        string tmp2;
        Downloadstartindex1 = douwnloadindex1 + 1;

        //if (tmp != d.Version)
        //{;
        //    tmp2 = s2.Substring(douwnloadindex1, downloadindex2 - douwnloadindex1);
        //    S_ApplicationDownLoadPacket apppkt = new S_ApplicationDownLoadPacket();
        //    apppkt.ApplicationDownLoadLink = tmp2;
        //    apppkt.message = "you need to update application";
        //    session.Send(apppkt.Write());
        //    return;
        //}
        S_DownLoadPacket pkt = new S_DownLoadPacket();
        pkt.URL = new List<string>();
        pkt.FileName = new List<string>();
        for (int i = 0; i < d.BundleVersionList.Count; i++)
        {
           index1 = s.IndexOf(":", startindex1)+1;
            startindex2 = index1 + 1;
           index2 = s.IndexOf("-", startindex2);
           
            if (index2 != -1) {
                tmp = s.Substring(index1 , index2 - index1);
            }
            else
            {
                tmp = s.Substring(index1 );
            }
          
          
                douwnloadindex1 = s2.IndexOf(":", Downloadstartindex1)+1;
                Downloadstartindex2 = douwnloadindex1 + 1;
               downloadindex2 = s2.IndexOf("[byte", Downloadstartindex2);
              int  downloadindex3 = s2.IndexOf("DownLoadurl-", Downloadstartindex1);
            Downloadstartindex1 = douwnloadindex1 + 1;
            string tmp3;
           
        FIleByte+= int.Parse(  s2.Substring(downloadindex2 + "[byte".Length,
              s2.IndexOf("KB]", downloadindex2) - downloadindex2 - "[byte".Length));
            tmp3 = s2.Substring(downloadindex3 + "DownLoadurl-".Length, douwnloadindex1 - downloadindex3  - "DownLoadurl-".Length-1);
            
            if (tmp != d.BundleVersionList[i])
            {
                
                if (downloadindex2 != -1)
                {

                    tmp2 = s2.Substring(douwnloadindex1 , downloadindex2 - douwnloadindex1);
                }
                else
                {
                    tmp2 = s2.Substring(douwnloadindex1 );
                }

                pkt.FileName.Add(tmp3);
                pkt.URL.Add(tmp2);
             
            }

            pkt.FileByte = FIleByte;



        }
        if (pkt.URL.Count != 0)
        {
            pkt.message = "Asset Download Need";
            session.Send(pkt.Write());
        }
        else
        {

            S_Message pkt2 = new S_Message();
            pkt2.message = "Application Version Check Complete";
            session.Send(pkt2.Write());
        }
    }
   public static int testcounter;
    public static void C_CreateAccountHandler(PacketSession session, IPacket packet)
    {
        C_CreateAccountData d= packet as C_CreateAccountData;
        var a = LocalTask. LoadAccountTemplet();
        a.username = d.name;
        a.LastestConnect= DateTime.Now.ToString("yyyyMMddhhmmss");
    
        if (ServerSetting.Instance.DBSaveSetting==DBsavesetting.Directory)
        {
            a._uid = LocalTask.createuid();


         LocalTask.   saveaccount(a);
            Console.WriteLine($"Name:{a.username} UID:{a._uid}의 계정이 생성되었습니다");

            S_LoadAccount s = new S_LoadAccount();
            s.d = a;
            s.ServerMessage = "account create Complete";
            session.Send(s.Write());
        }
        else if(ServerSetting.Instance.DBSaveSetting == DBsavesetting.Mysql)
        {

            a =  SqlTask.insertDBsql(ServerSetting.Instance.sqlSetting.conn, a);
            testcounter++;
            Console.WriteLine($"Name:{a.username} UID:{a._uid}의 계정이 생성되었습니다 오늘 생성된 계정 수:{testcounter}");
            S_LoadAccount s = new S_LoadAccount();
            s.d = a;
            s.ServerMessage = "account create Complete";
            session.Send(s.Write());
        }
       
            

            
        
        
        
    }
    public static void C_SaveShopHandler(PacketSession session, IPacket packet)
    {
        C_SaveShopPacket d= packet as C_SaveShopPacket;

        ShopListCache c = new ShopListCache() { rerollCount = d.rerollcount, shoplist = d.itemlist };
        S_Message pkt = new S_Message();

        if (ServerSetting.Instance.DBSaveSetting == DBsavesetting.Directory)
        {
            
            ShopDBManager.SaveShopCacheFile((Shopid)d.shopid, c, d.UID);
         LocalTask.   saveaccount(d.account);
         
  


        }
        else if (ServerSetting.Instance.DBSaveSetting == DBsavesetting.Mysql)
        {
           SqlTask. updateDailyshopplayerdb(d.UID, c, ServerSetting.Instance.sqlSetting.conn);
            SqlTask.UpdateDBUID2(d.account, ServerSetting.Instance.sqlSetting.conn);

           

         
        }

        pkt.message = "ShopList Save Compete";
        session.Send(pkt.Write());
    }
    public static void C_RequestShopListHandler(PacketSession session, IPacket packet)
    {
        C_RequestShopList d = packet as C_RequestShopList;
        ShopListCache a=null;
        if (ServerSetting.Instance.DBSaveSetting == DBsavesetting.Directory)
        {

           a = ShopDBManager.LoadShopCacheFile((Shopid)d.shopid, d.UID);



        }
        else if (ServerSetting.Instance.DBSaveSetting == DBsavesetting.Mysql)
        {
            Console.WriteLine(d.UID + "의 상점 데이터 발견 되지 않음 상점 데이터 생성 중");
            if ((a= SqlTask.LoadDailyshopplayerdb(d.UID, ServerSetting.Instance.sqlSetting.conn)) == null){
                var shoplist= SqlTask.getshopdb(ServerSetting.Instance.sqlSetting.conn);
   
                //가중치 만큼 데이터 추가 생성
                int count = shoplist.Count;
                for(int n = 0; n < count; n++)
                {
                    for(int i = 0; i < shoplist[n].weight; i++)
                    {
                        shoplist.Add(shoplist[n]);
                    }
                }
                //리스트 섞기
                Random r = new Random();
              count= shoplist.Count;
                while (count > 1)
                {
                    count--;
                    int tmp = r.Next(count + 1);
                    ShopItemListforsql value = shoplist[tmp];
                    shoplist[tmp] = shoplist[count];
                    shoplist[count] = value;
                }//피셔-에이츠 셔플 참조 https://stackoverflow.com/questions/273313/randomize-a-listt
                 //리스트 생성
                a = new ShopListCache();
                a.shoplist = new List<ShopItem>();
                for(int n = 0; n < 12; n++)
                {
                    int rng = r.Next(shoplist.Count);
                    a.shoplist.Add(shoplist[rng].Item);
                }
                SqlTask.insertDailyshopplayerdb(d.UID, a, ServerSetting.Instance.sqlSetting.conn);
            }
            else
            {
                Console.WriteLine(d.UID + "의 상점 데이터 발견 됨");
            }



        }


        S_LoadShopList pkt = new S_LoadShopList();
        pkt.shopid = d.shopid;
        pkt.itemlist = a.shoplist;
        pkt.rerollcount = a.rerollCount;
        pkt.message = "ShopList Load Complete";
        session.Send(pkt.Write());


    }
    public static void C_ConnectRequestHandler(PacketSession session, IPacket packet)
    {
        C_ConnectRequest pkt = packet as C_ConnectRequest;
        ClientSession clientSession = session as ClientSession;

        Console.WriteLine($"UID{pkt.UID}의 연결 요청...");
        UserAccountData a = null;
        if (ServerSetting.Instance.DBSaveSetting == DBsavesetting.Directory)
        {
            a = LocalTask.LoadAccount(pkt.UID);

            //탐색 실패시 클라이언트로 부터 오류를 보낸다
            if (a == null)
            {
                Console.WriteLine($"[위험]{pkt.UID} 탐색 실패 DB나 클라이언트의 손상이 우려됩니다.");
                S_ErrorPacket Connecterror = new S_ErrorPacket();
                Connecterror.Error = "계정을 찿을 수 없습니다. 캐시 손상의 우려가 있습니다."
                    ;
                clientSession.Send(Connecterror.Write());
                return;
            }
            else
            {
                Console.WriteLine($"UID{pkt.UID}불려오기 성공...");
                S_LoadAccount AccountData = new S_LoadAccount();
                AccountData.d = a;
                AccountData.ServerMessage = "account Load Complete";
                clientSession.Send(AccountData.Write());

            }
        }else if (ServerSetting.Instance.DBSaveSetting == DBsavesetting.Mysql)
        {
            a= SqlTask.LoadDBsql(pkt.UID, ServerSetting.Instance.sqlSetting.conn);
            if (a == null)
            {
                Console.WriteLine($"[위험]{pkt.UID} 탐색 실패 DB나 클라이언트의 손상이 우려됩니다.");
                S_ErrorPacket Connecterror = new S_ErrorPacket();
                Connecterror.Error = "계정을 찿을 수 없습니다."
                    ;
                clientSession.Send(Connecterror.Write());
                return;
            }
            else
            {
                Console.WriteLine($"UID{pkt.UID}불려오기 성공...");
                S_LoadAccount AccountData = new S_LoadAccount();
                AccountData.d = a;
                AccountData.ServerMessage = "account Load Complete";
                clientSession.Send(AccountData.Write());

            }
        }
        
    }
    public static void C_SaveAccountDataPacket(PacketSession session, IPacket packet)
    {
        C_SaveAccountData pkt = packet as C_SaveAccountData;
        if (ServerSetting.Instance.DBSaveSetting == DBsavesetting.Directory)
        {
           LocalTask. saveaccount(pkt.d);
          
            //save 완료를 알리는 string만 보내는 패킷 생성 후 보내기

            S_Message s = new S_Message();

            s.message = "account Save Complete";
            session.Send(s.Write());
        }
        else if(ServerSetting.Instance.DBSaveSetting == DBsavesetting.Mysql)
        {
            SqlTask.UpdateDBUID2(pkt.d, ServerSetting.Instance.sqlSetting.conn);
          
            //save 완료를 알리는 string만 보내는 패킷 생성 후 보내기

            S_Message s = new S_Message();

            s.message = "account Save Complete";
            session.Send(s.Write());
        }
     
            

    }
    #endregion
    
    
   


}

public class LocalTask
{
    static int sliceSpcialChacharacter(char c)
    {
        if ((c > 47 && c < 58) || (c > 64 && c < 91) || c > 96)
            return c;
        else
            return 0;
    }
    static string setuid()
    {
        string uid = "";
        Random r = new Random();
        int random1 = r.Next(10, 13);
        for (int i = 0; i < random1; i++)
        {
            int random2 = 48;
            while (true)
            {
                random2 = r.Next(48, 123);
                if (sliceSpcialChacharacter((char)random2) != 0)
                    break;
            }
            uid += (char)random2;
        }
        return uid;
    }
    public static string createuid()
    {
        // Existing UID input expects 12 characters. Preserve old accounts as-is.
        string uid;
        do { uid = Guid.NewGuid().ToString("N").Substring(0, 12); }
        while (File.Exists(Path.Combine(getDBDirectory, uid + ".json")));
        return uid;
    }
    #region account
    static string getDBDirectory
    {
        get { return ServerPaths.AccountDirectory; }
    }
    public static void saveaccount(UserAccountData account)
    {
        string FilePath = getDBDirectory + $"\\{account._uid}.json";

        string json = JsonConvert.SerializeObject(account, Formatting.Indented);

        System.IO.File.WriteAllText(FilePath, json);
        Console.WriteLine($"{FilePath}의 경로로 {account._uid}.json 저장");
    }
    public static UserAccountData LoadAccount(string UID)
    {
        try
        {
            UserAccountData data;


            string FilePath = getDBDirectory + $"\\{UID}.json";
            if (!File.Exists(FilePath))
            {
                Console.WriteLine($"계정을 찿을 수 없음");
                return null;
            }
            using (StreamReader r = new StreamReader(FilePath))
            {
                string json = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<UserAccountData>(json);
            }

            return data;

        }
        catch
        {
            Console.WriteLine($"계정 불려오기 실패");
            return null;
        }
    }
    public static UserAccountData LoadAccountTemplet()
    {
        string FilePath = getDBDirectory + "\\DataTemplet\\AccountTemplet.json";
        UserAccountData data;
        using (StreamReader r = new StreamReader(FilePath))
        {
            string json = r.ReadToEnd();
            data = JsonConvert.DeserializeObject<UserAccountData>(json);
        }

        return data;
    }
    #endregion

}