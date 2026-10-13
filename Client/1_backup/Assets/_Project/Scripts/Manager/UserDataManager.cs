using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

using UnityEngine;

namespace DataExecute
{
    public class UserDataManager
    {
        
       
  
  

        string Streaming_Directory;
        string DB_Directory;
        public UserDataManager()
        {
            Streaming_Directory = Directory.GetCurrentDirectory() + "\\Assets" + "\\StreamingAssets";
            DB_Directory= Directory.GetCurrentDirectory() + "\\Assets\\_Project\\Data\\Json";
        }

      
      
        #region JSON
        string ObjectTojson(object obj)
        {
    
            return JsonUtility.ToJson(CharacterContentMapping.NormalizeData(obj));
        }
        T JsonToObject<T>(string jsondata)
        {
            return CharacterContentMapping.NormalizeData(JsonUtility.FromJson<T>(jsondata));
        }
        void createFile(string FilePath,string jsondata)
        {

                FileStream file = new FileStream(string.Format(FilePath), FileMode.Create);
          
            byte[] data = Encoding.UTF8.GetBytes(jsondata);
            file.Write(data, 0, data.Length);
            file.Close();
        }
        string ReadFile(string FilePath)
        {

            FileStream file = new FileStream(string.Format(FilePath), FileMode.Open);

            byte[] data =new byte[file.Length];
            file.Read(data, 0, data.Length);
            file.Close();
            return Encoding.UTF8.GetString(data);
        }
        void DeleteFile(string FilePath)
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
            else
                return;
        }
        #region itemCreate
        public void CreateItemData(ItemData d)
        {
            string s = ObjectTojson(d);
            string filepath = DB_Directory + "\\item\\" + d.id + ".json";
            if (!Directory.Exists(DB_Directory + "\\item"))
            {
                Directory.CreateDirectory(DB_Directory + "\\item");
            }
            createFile(filepath, s);
        }
        public ItemData LoadItemData(string id)
        {
            try
            {
                string filepath = DB_Directory + "\\item\\" + id + ".json";
                return JsonToObject<ItemData>(ReadFile(filepath));
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region StageCreate
        public void createStageinfo(StageInformation d,string version)
        {
            string s = ObjectTojson(d);
            string filepath = DB_Directory + "\\Stage\\" +d.ID + ".json";
            if (!Directory.Exists(DB_Directory + "\\Stage"))
            {
                Directory.CreateDirectory(DB_Directory + "\\Stage");
            }
            createFile(filepath, s);
            StageData a= new StageData() { ID = d.ID, name = d.name, stagelock = false };
            if(!GetUserAccountTemplet().Stagelist.Contains( a))
            {
                UpdateUserAccountTemplet(a, version);
            }
        }
        public StageInformation LoadStageinfo(string id)
        {
            try
            {
                string filepath = DB_Directory + "\\Stage\\" + id + ".json";
                return JsonToObject<StageInformation>(ReadFile(filepath));
            }
            catch
            {
                Debug.Log("맵 불려오기 에러");
                return null;
            }
        }
        #endregion
        #region accountDataCreate
        public UserAccountDataNetworkMember GetUserAccountTemplet()
        {
            string filepath = DB_Directory+"\\AccountTemplet.json";
            
            if (!new FileInfo(filepath).Exists)
            {
                Debug.Log("생성");
                CreateUserAccountTemplet(filepath);
            }
          
            return JsonToObject<UserAccountDataNetworkMember>(ReadFile(filepath));
        }
        public UserAccountDataNetworkMember GetUserAccountTemplet(string version)
        {
            string filepath = DB_Directory + "\\AccountTemplet.json";

            if (!new FileInfo(filepath).Exists)
            {
                Debug.Log("생성");
                CreateUserAccountTemplet(filepath, version);
            }
            var a= JsonToObject<UserAccountDataNetworkMember>(ReadFile(filepath));
          
            return a;
        }
        public void CreateUserAccountTemplet(string filepath)
        {

            UserAccountDataNetworkMember d = new UserAccountDataNetworkMember()
            {
                _uid = "randomstring 10-12",
                username = "",
                _accountlv = 1,
                _MaxStamina = 30,
                _Stamina = 30,
                _Max_Exp = 100,
                _Exp = 0
                ,
                LastestConnect = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")
            };
         
            createFile(filepath, ObjectTojson(d));
        }
   
        public void CreateUserAccountTemplet(string filepath,string version)
        {

            UserAccountDataNetworkMember d = new UserAccountDataNetworkMember()
            {
                _uid = "randomstring 10-12",
                username = "",
                _accountlv = 1,
                _MaxStamina = 30,
                _Stamina = 30,
                _Max_Exp = 100,
                _Exp = 0
                ,
             
                LastestConnect = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")

            };
            createFile(filepath, ObjectTojson(d));
        }
    
    
        public void UpdateUserAccountTemplet(PlayerData Cdata, string version)
        {
            string filepath = DB_Directory + "\\AccountTemplet.json";
            UserAccountDataNetworkMember d = GetUserAccountTemplet();
            if (d.Characterlist == null)
                d.Characterlist = new System.Collections.Generic.List<PlayerData>();
            if (!d.Characterlist.Contains(Cdata))
                d.Characterlist.Add(Cdata);



            createFile(filepath, ObjectTojson(d));
         
        }
        public void UpdateUserAccountTemplet( StageData Sdata, string version)
        {
            string filepath = DB_Directory + "\\AccountTemplet.json";
            UserAccountDataNetworkMember d = GetUserAccountTemplet(version);
            if (d.Stagelist == null)
                d.Stagelist = new System.Collections.Generic.List<StageData>();
          if(!d.Stagelist.Contains(Sdata))
                d.Stagelist.Add(Sdata);
        
           

            createFile(filepath, ObjectTojson(d));
        }
       
        public UserAccountDataNetworkMember GetAccount()
        {
            
            string filepath = Streaming_Directory + "\\User.json";
           
            try
            {
                return JsonToObject<UserAccountDataNetworkMember>(ReadFile(filepath));
            }
            catch
            {
                return null;
            }

        }
        public void createAccount(UserAccountDataNetworkMember data,string directory,string name)
        {
            string s = ObjectTojson(data);
            string filepath = directory + $"\\{name}.json";
            createFile(filepath, s);
        }
        public void createAccount(UserAccountData data)
        {
            string s = ObjectTojson(data);
            string filepath = Streaming_Directory + "\\User.json";
            createFile(filepath, s);
        }
        //public bool UpdateAccount(UserAccountData account)
        //{

        //    UserAccountData templet = GetUserAccountTemplet();
            
        //    Debug.Log("계정 업데이트");
           
        //    foreach (var d in templet.get_CharacterDictionary())
        //    {
        //        if (!account.get_CharacterDictionary().ContainsKey(d.Key))
        //        {
        //            account.set_Character(d.Value);
        //        }
        //        else
        //        {
        //            //if (d.Value.name!=)
        //        }
               
        //    }
        //    foreach (var d in templet.get_StageList())
        //    {
        //        if (!account.get_StageList().ContainsKey(d.Key))
        //        {
        //            account.set_Stage(d.Value);
        //        }
        //    }
        //    createAccount(account);
        //    return true;
        //}
        #endregion
                     
         #region ChracterinformationCreatejson
        public void createChracterInformation(CharacterInformation c)
        {
            string assetName = CharacterContentMapping.Get(c.id).JsonName;
            string s = ObjectTojson(c);
            string filepath = DB_Directory+ $"\\Character\\{assetName}.json";
            if(!Directory.Exists(DB_Directory + "\\Character")){
                Directory.CreateDirectory(DB_Directory + "\\Character");
                    }
            createFile(filepath, s);
        }
        public CharacterInformation readCharacterinformation(string DataName)
        {
            try
            {
                string assetName = CharacterContentMapping.Get(DataName).JsonName;
                string FilePath = DB_Directory + $"\\Character\\{assetName}.json";
                return JsonToObject<CharacterInformation>(ReadFile(FilePath));
            }
            catch
            {
                return null;
            }
        }

        #endregion
        #region ShopCatalog
        public void createShopCatalog(ShopItemDictionary S)
        {
            string s = ObjectTojson(S);
            string filepath = DB_Directory + $"\\Shop\\{S.id}.json";
            if (!Directory.Exists(DB_Directory + "\\Shop"))
            {
                Directory.CreateDirectory(DB_Directory + "\\Shop");
            }
            createFile(filepath, s);
        }
        public ShopItemDictionary readShopCatalog(string DataName)
        {
            string FilePath = Application.streamingAssetsPath + $"\\Shop\\{DataName}.json";
            try
            {
                return JsonToObject<ShopItemDictionary>(ReadFile(FilePath));
            }
            catch
            {
                return null;
            }
        }

        #endregion
        #endregion
        #region Encrypt
        public Rfc2898DeriveBytes createkey(string password)
        {
            byte[] KeyByte = Encoding.UTF8.GetBytes(password);
            byte[] saltByte = SHA512.Create().ComputeHash(KeyByte);
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(KeyByte, saltByte, 1000);
            return key;
        }
        public Rfc2898DeriveBytes CreateVector(string vector)
        {
            byte[] vectorByte = Encoding.UTF8.GetBytes(vector);
            byte[] saltByte = SHA512.Create().ComputeHash(vectorByte);
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(vectorByte, saltByte, 10000);
            return key;
        }
        public byte[] Encrypt(byte[] origin, string password)
        {
            RijndaelManaged aes = new RijndaelManaged();
            Rfc2898DeriveBytes key = createkey(password);
            Rfc2898DeriveBytes vector = CreateVector("rla9rb9x0oghk9dl1x9ld");

            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(32);
            aes.IV = vector.GetBytes(16);

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(origin, 0, origin.Length);
                }
                return ms.ToArray();
            }
        }
        public byte[] Decrypt(byte[] origin, string password)
        {
            RijndaelManaged aes = new RijndaelManaged();
            Rfc2898DeriveBytes key = createkey(password);
            Rfc2898DeriveBytes vector = CreateVector("rla9rb9x0oghk9dl1x9ld");
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key.GetBytes(32);
            aes.IV = vector.GetBytes(16);
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(origin, 0, origin.Length);
                }
                return ms.ToArray();
            }
        }
        byte[] StringToByte(string value)
        {
            Byte[] buffer = Encoding.UTF8.GetBytes(value);

            return buffer;
        }
        string ByteToString(byte[] value)
        {
            string str = Encoding.Default.GetString(value);
            return str;
        }
        public void saveEncryptFile(byte[] b, string filepath,string filename)
        {
            string path = filepath + $"\\{filename}.txt";
            string encryptfile = "";
            for (int n = 0; n < b.Length; n++)
            {
                encryptfile += (char)b[n];
            }
            createFile(path, encryptfile);
            Debug.Log("연결 캐시 생성");
            changefileexception(filepath, filename, "txt", "Enctypt");
        }
        public byte[] LoadEncryptFile(string filepath, string filename)
        {
            changefileexception(filepath, filename, "Enctypt", "txt");

            string path = filepath + $"\\{filename}.txt";
            var a = ReadFile(path);
            byte[] result = new byte[a.Length];
            for (int n = 0; n < result.Length; n++)
            {
                result[n] = (byte)a[n];
            }
            changefileexception(filepath, filename, "txt", "Enctypt");
            return result;
        }
        void changefileexception(string filepath,string filename,string currentexception,string changeexception)
        {
            string path = filepath + $"\\{filename}.{currentexception}";
            string changePath = filepath + $"\\{filename}.{changeexception}";
            File.Copy(path, changePath, true);
            File.Delete(path);
        }
        #endregion
        #region connectorCache

        public void MakeConnectionCache(string UID_,string Version)
        {
            ConnectionCache cache = new ConnectionCache()
            {
                Name = System.Net.Dns.GetHostName(),
                IP = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).ToString()
                ,LaunchingVersion= Version,
                UID = UID_,
                LastestConnection= DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")

            };

            string s = ObjectTojson(cache);
            var encrypt = Encrypt(StringToByte(s), "420241");
            string filepath = Streaming_Directory;
            saveEncryptFile(encrypt, filepath, "ConnectCache");
            
        }
        public ConnectionCache loadConnectionCache()
        {
            try
            {
                string filepath = Streaming_Directory;
                var encryptbyte = LoadEncryptFile(filepath, "ConnectCache");
                var dectyptbyte = Decrypt(encryptbyte, "420241");
                string s = ByteToString(dectyptbyte);
                return JsonToObject<ConnectionCache>(s);
            }
            catch
            {
                return null;
            }
        }

        #endregion
        #region ini
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(
        string Section, string Key, string Value, string FilePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(
     string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);
      
        #region setting
        public void CreateSetting(SettingData s)
        {


            WritePrivateProfileString("Sound", "BackGround", s.BGMVolume.ToString(), Streaming_Directory + "\\config.ini") ;
            WritePrivateProfileString("Sound", "SoundEffect", s.SEVolume.ToString(), Streaming_Directory + "\\config.ini");
        }
        public SettingData loadSetting()
        {
            string filepath = Streaming_Directory + "\\config.ini";
            if (File.Exists(filepath))
            {
                SettingData data = new SettingData();
                var value =new StringBuilder(255);
                GetPrivateProfileString("Sound", "BackGround", "100", value, 255, filepath);
                data.BGMVolume = int.Parse(value.ToString());
                GetPrivateProfileString("Sound", "SoundEffect", "100", value, 255, filepath);
                data.SEVolume = int.Parse(value.ToString());
                return data;
            }
            else
            {
                SettingData s = new SettingData() { BGMVolume = 100, SEVolume = 100 };
                CreateSetting(s);
                return s;
            }
               
        }
        public void ReWriteSetting(string section,string key,string value)
        {
            string filepath = Streaming_Directory + "\\config.ini";
            WritePrivateProfileString(section, key, value, filepath);
        }
        #endregion
        #endregion


        #region 미사용
        //xml,json 공부를 위해 계정의 데이터를 유저 데이터 맵 데이터, 캐릭터 데이터로 나눠서 코딩하다가
        //파일 하나로 관리하는게 낫겠다는 판단으로
        //전부 비활성화 시키고
        //account데이터 하나만 관리하기로 결정

        //Q 왜 남김?
        //A 공부의 흔적을 남기기 위해
        //public UserAccountData GetUserData()
        //{
        //    XmlDocument xml = new XmlDocument();
        //    string itemDirectory = this_Directory + "\\ItemData.json";
        //    string StageDiretory = this_Directory + "\\StageData.json";
        //    string CharacterDirectory = this_Directory + "\\CharacterData.json";
        //    if (!new DirectoryInfo(this_Directory + "\\UserData.xml").Exists)
        //    {
        //        xml = CreateUserData(xml, FileName);
        //        if (!File.Exists(itemDirectory))
        //            createItem(itemDirectory, Items);
        //        if (File.Exists(CharacterDirectory))
        //            createCharacter(CharacterDirectory, characters);
        //        if (File.Exists(StageDiretory))
        //            createStageData(StageDiretory, stages);
        //    }

        //    createItem(itemDirectory, Items);
        //    var account = LoadUserData(xml, this_Directory + "\\UserData.xml");


        //    try
        //    {
        //        foreach (CharacterData data in ReadCharacterjson(CharacterDirectory).list)
        //        {
        //            account._CharacterList.Add(data);
        //        }
        //    }
        //    catch
        //    {
        //        DeleteFile(CharacterDirectory);
        //        createCharacter(CharacterDirectory, characters);
        //        foreach (CharacterData data in ReadCharacterjson(CharacterDirectory).list)
        //        {
        //            account._CharacterList.Add(data);
        //        }
        //    }
        //    try
        //    {
        //        foreach (ItemData data in ReadItemjson(itemDirectory).list)
        //        {
        //            account._Itemlist.Add(data);

        //        }
        //    }
        //    catch
        //    {
        //        DeleteFile(itemDirectory);
        //        createItem(itemDirectory, Items);
        //        foreach (ItemData data in ReadItemjson(itemDirectory).list)
        //        {
        //            account._Itemlist.Add(data);
        //        }
        //    }
        //    try
        //    {
        //        foreach (StageData data in ReadStagejson(StageDiretory).list)
        //        {
        //            account._StageList.Add(data);
        //        }
        //    }
        //    catch
        //    {
        //        DeleteFile(StageDiretory);
        //        createStageData(StageDiretory, stages);
        //        foreach (StageData data in ReadStagejson(StageDiretory).list)
        //        {
        //            account._StageList.Add(data);
        //        }
        //    }

        //    return account;
        //}
        //#region XML
        //#region PlayerDataXml

        //XmlDocument CreateUserData(XmlDocument xml, string FileName)
        //{
        //    if (!new DirectoryInfo(this_Directory).Exists)
        //        Directory.CreateDirectory(this_Directory);

        //    //File.Create(Directory.GetCurrentDirectory() + "\\StreamingAssets");
        //    PlayerData p_data = new PlayerData
        //    {
        //        Id = ((int)UnityEngine.Random.Range(1, 100)).ToString(),
        //        Playername = "Test",
        //        PlayerLV = 1.ToString(),
        //        LastestConnect = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")
        //    };

        //    xml = CreateXmlData("UserData", p_data);
        //    xml.Save(this_Directory + "\\UserData.xml");
        //    return xml;
        //}



        //XmlDocument CreateXmlData(string xmlname, PlayerData PlayerTable)
        //{
        //    XmlDocument xml = new XmlDocument();
        //    xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", "yes"));
        //    XmlElement Version = xml.CreateElement("BuildVersion");
        //    Version.InnerText = VersionBundleManager.instance.Version;
        //    xml.AppendChild(Version);
        //    XmlElement UserData = xml.CreateElement(xmlname);

        //    UserData.AppendChild(CreatePlayerData(xml, PlayerTable));

        //    // xml.AppendChild(UserData);

        //    return xml;
        //}
        //#region createElement
        //XmlNode CreatePlayerData(XmlDocument xml, PlayerData Data)
        //{
        //    XmlNode PlayerData = xml.CreateElement("PlayerData");

        //    XmlNode ID = xml.CreateElement("ID");
        //    ID.InnerText = Data.Id;
        //    PlayerData.AppendChild(ID);
        //    XmlNode name = xml.CreateElement("Playername");
        //    name.InnerText = Data.Playername;
        //    PlayerData.AppendChild(name);
        //    XmlNode LV = xml.CreateElement("PlayerLv");
        //    LV.InnerText = Data.PlayerLV;
        //    PlayerData.AppendChild(LV);

        //    XmlNode connect = xml.CreateElement("LastestConnect");
        //    connect.InnerText = Data.LastestConnect;
        //    PlayerData.AppendChild(connect);

        //    return PlayerData;
        //}


        //#endregion

        //#region DeleteXml
        //void DeleteXmlData(string Path)
        //{
        //    File.Delete("Path");
        //}
        //void DeleteXmlData(XmlDocument node)
        //{
        //    XmlNode Node = node.SelectSingleNode("UserData");
        //    node.RemoveAll();
        //}
        //#endregion
        //#region replaceXml
        //XmlDocument ReplaceXmlData(XmlDocument xml)
        //{

        //    return xml;
        //}
        //void ReplacePlayerData(XmlDocument xml, PlayerData t)
        //{
        //    XmlNode PlayerNode = xml.SelectSingleNode("/UserData/PlayerData");

        //    ReplaceXmlNode(xml, PlayerNode, "Playername", t.Playername);
        //    ReplaceXmlNode(xml, PlayerNode, "PlayerLv", t.PlayerLV);
        //    ReplaceXmlNode(xml, PlayerNode, "LastestConnect", t.LastestConnect);

        //    ;


        //}


        //void ReplaceXmlNode(XmlDocument xml, XmlNode node, string Target, string input)
        //{
        //    XmlNode Serach;
        //    Serach = node.SelectSingleNode(Target);
        //    if (Serach == null)
        //    {
        //        XmlElement element = xml.CreateElement(Target);
        //        element.InnerText = input;
        //        node.AppendChild(element);
        //    }
        //    else
        //    {
        //        Serach.InnerText = input;
        //    }
        //}
        //#endregion

        //#region Load
        //UserAccountData LoadUserData(XmlDocument xml, String Path)
        //{
        //    UserAccountData account = new UserAccountData();
        //    xml.Load(Path);
        //    //CheckBuildVersion(xml);
        //    LoadPlayerData(xml, account);

        //    return account;
        //}
        //void CheckBuildVersion(XmlDocument xml)
        //{
        //    XmlNode node = xml.SelectSingleNode("/UserData/BuildVersion");
        //    if (VersionBundleManager.instance.Version == node.InnerText)
        //        return;
        //    if (node == null)
        //    {
        //        XmlNode Version = xml.CreateElement("BuildVersion");
        //        Version.InnerText = VersionBundleManager.instance.Version;
        //        xml.AppendChild(Version);

        //    }
        //    else if (VersionBundleManager.instance.Version != node.InnerText)
        //    {
        //        node.InnerText = VersionBundleManager.instance.Version;
        //        DeleteFile(this_Directory + "\\ItemData.json");
        //        DeleteFile(this_Directory + "\\CharacterData.json");
        //        DeleteFile(this_Directory + "\\StageData.json");
        //    }
        //    var a = VersionBundleManager.instance.BundlePackage();
        //    createItem(this_Directory + "\\ItemData.json", a.items);
        //    createCharacter(this_Directory + "\\CharacterData.json", a.Character);
        //    createStageData(this_Directory + "\\StageData.json", a.Stage);
        //}
        //void LoadPlayerData(XmlDocument xml, UserAccountData account)
        //{
        //    XmlNodeList node_list = xml.SelectNodes("/UserData/PlayerData");
        //    foreach (XmlNode node in node_list)
        //    {
        //        account.uid = node["ID"].InnerText;
        //        account.username = node["Playername"].InnerText;
        //        account.accountlv = XmlConvert.ToInt32(node["PlayerLv"].InnerText);
        //    }
        //}


        //#endregion
        //#endregion
        //#endregion
        //#region Characterjson
        //void createCharacter(string FilePath, CharacterDataList list = null)
        //{

        //    if (list == null)
        //        list = new CharacterDataList();
        //    createFile(FilePath, ObjectTojson(list));

        //}

        //CharacterDataList ReadCharacterjson(string FilePath)
        //{


        //    CharacterDataList tmp = JsonToObject<CharacterDataList>(ReadFile(FilePath));
        //    return tmp;
        //}
        //#endregion
        //#region Itemjson
        //void createItem(string FilePath, ItemDataList list = null)
        //{

        //    if (list == null)
        //        list = new ItemDataList();

        //    createFile(FilePath, ObjectTojson(list));

        //}

        //ItemDataList ReadItemjson(string FilePath)
        //{


        //    ItemDataList tmp = JsonToObject<ItemDataList>(ReadFile(FilePath));

        //    return tmp;
        //}
        //#endregion
        //#region Stagejson
        //void createStageData(string FilePath, StageDataList list = null)
        //{

        //    if (list == null)
        //        list = new StageDataList();
        //    createFile(FilePath, ObjectTojson(list));

        //}

        //StageDataList ReadStagejson(string FilePath)
        //{


        //    StageDataList tmp = JsonToObject<StageDataList>(ReadFile(FilePath));
        //    return tmp;
        //}
        //#endregion
        #endregion
    }



}
