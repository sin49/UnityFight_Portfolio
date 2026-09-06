using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public static class FIleIOManager 
{
   static string ObjectTojson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }
    static T JsonToObject<T>(string jsondata)
    {
        return JsonUtility.FromJson<T>(jsondata);
    }
    static void createFile(string FilePath, string jsondata)
    {

        FileStream file = new FileStream(string.Format(FilePath), FileMode.Create);

        byte[] data = Encoding.UTF8.GetBytes(jsondata);
        file.Write(data, 0, data.Length);
        file.Close();
    }
    static string ReadFile(string FilePath)
    {

        FileStream file = new FileStream(string.Format(FilePath), FileMode.Open);

        byte[] data = new byte[file.Length];
        file.Read(data, 0, data.Length);
        file.Close();
        return Encoding.UTF8.GetString(data);
    }
    static void DeleteFile(string FilePath)
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
        else
            return;
    }
    static string Streaming_Directory = Directory.GetCurrentDirectory() + "\\Assets" + "\\StreamingAssets";
    #region ini
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    static extern long WritePrivateProfileString(
    string Section, string Key, string Value, string FilePath);
    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    static extern int GetPrivateProfileString(
 string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

    #region setting
    public static  void CreateSetting(SettingData s)
    {


        WritePrivateProfileString("Sound", "BackGround", s.BGMVolume.ToString(), Streaming_Directory + "\\config.ini");
        WritePrivateProfileString("Sound", "SoundEffect", s.SEVolume.ToString(), Streaming_Directory + "\\config.ini");
        WritePrivateProfileString("Graphic", "GraphicLevel", s.GraphicSettingindex.ToString(),Streaming_Directory+ "\\config.ini");
    }
    public static SettingData loadSetting()
    {
        string filepath = Streaming_Directory + "\\config.ini";
        if (File.Exists(filepath))
        {
            SettingData data = new SettingData();
            var value = new StringBuilder(255);
            GetPrivateProfileString("Sound", "BackGround", "100", value, 255, filepath);
            data.BGMVolume = int.Parse(value.ToString());
            GetPrivateProfileString("Sound", "SoundEffect", "100", value, 255, filepath);
            data.SEVolume = int.Parse(value.ToString());
            GetPrivateProfileString("Graphic", "GraphicLevel","2",value,255, filepath);
            data.GraphicSettingindex = int.Parse(value.ToString());
    
            return data;
        }
        else
        {
            SettingData s = new SettingData() { BGMVolume = 100, SEVolume = 100 ,GraphicSettingindex=2};
            CreateSetting(s);
            return s;
        }

    }
    public static void ReWriteSetting(string section, string key, string value)
    {
        string filepath = Streaming_Directory + "\\config.ini";
        WritePrivateProfileString(section, key, value, filepath);
    }
    #endregion
    #endregion
    #region Encrypt
   static Rfc2898DeriveBytes createkey(string password)
    {
        byte[] KeyByte = Encoding.UTF8.GetBytes(password);
        byte[] saltByte = SHA512.Create().ComputeHash(KeyByte);
        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(KeyByte, saltByte, 1000);
        return key;
    }
    static Rfc2898DeriveBytes CreateVector(string vector)
    {
        byte[] vectorByte = Encoding.UTF8.GetBytes(vector);
        byte[] saltByte = SHA512.Create().ComputeHash(vectorByte);
        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(vectorByte, saltByte, 10000);
        return key;
    }
   static byte[] Encrypt(byte[] origin, string password)
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
    static byte[] Decrypt(byte[] origin, string password)
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
    static byte[] StringToByte(string value)
    {
        Byte[] buffer = Encoding.UTF8.GetBytes(value);

        return buffer;
    }
    static string ByteToString(byte[] value)
    {
        string str = Encoding.Default.GetString(value);
        return str;
    }
    static void saveEncryptFile(byte[] b, string filepath, string filename)
    {
        string path = filepath + $"\\{filename}.txt";
        string encryptfile = "";
        for (int n = 0; n < b.Length; n++)
        {
            encryptfile += (char)b[n];
        }
        createFile(path, encryptfile);
  
        changefileexception(filepath, filename, "txt", "Enctypt");
    }
    static byte[] LoadEncryptFile(string filepath, string filename)
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
    static void changefileexception(string filepath, string filename, string currentexception, string changeexception)
    {
        string path = filepath + $"\\{filename}.{currentexception}";
        string changePath = filepath + $"\\{filename}.{changeexception}";
        File.Copy(path, changePath, true);
        File.Delete(path);
    }
    #endregion
    #region connectorCache
    public static void DeleteConnectionCache()
    {
        string filepath = Streaming_Directory/*+ "\\ConnectCache.Enctypt"*/;


        changefileexception(filepath, "ConnectCache", "Enctypt", "txt");

        string path = filepath + $"\\ConnectCache.txt";

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("파일 삭재 완료:" + path);

        }
        else
        {
            Debug.Log("파일을 찿을 수 없습니다.:" + path);
        }
        cache = null;
    }
    public static void MakeGamePlayCache(GamePlayCache cache)
    {
        Debug.Log("chk" + cache.BasciGamePlayTutorialComplete1 + " " + cache.BasciGamePlayTutorialComplete1);
        string s = ObjectTojson(cache);
        var encrypt = Encrypt(StringToByte(s), "260139");
        string filepath = Streaming_Directory;
        saveEncryptFile(encrypt, filepath, "GameplayCache");
    }
    public static GamePlayCache LoadGameplayCache()
    {
        try
        {
            string filepath = Streaming_Directory;
            var encryptbyte = LoadEncryptFile(filepath, "GameplayCache");
            var dectyptbyte = Decrypt(encryptbyte, "260139");
            string s = ByteToString(dectyptbyte);
            return JsonToObject<GamePlayCache>(s);
        }
        catch
        {
            return null;
        }
    }
    static ConnectionCache _cache;
    public static ConnectionCache cache { get {
            if (_cache == null)
            {
                _cache = loadConnectionCache();
                if (_cache == null)
                    return null;
            }
            return _cache;
        }set
        {
            if (_cache == null)
            {
                _cache = loadConnectionCache();

            }
            _cache = value; } }
    public static void MakeConnectionCache(UserAccountData data)
    {
        //ConnectionCache cache=loadConnectionCache();
        //if (cache == null)
        //{
        //    cache = new ConnectionCache()
        //    {
        //        Name = System.Net.Dns.GetHostName(),
        //        IP = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).ToString()
        //        ,
        //        LaunchingVersion = Application.version,
        //        UID = data.uid,
        //        LastestConnection = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),
        //        accountBackupjsondata = ObjectTojson(data)

        //    };
        //}
        //else
        //{
        if(cache==null)
            cache = new ConnectionCache();

            cache.Name = System.Net.Dns.GetHostName();
            cache.IP = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).ToString();


            cache.LaunchingVersion = Application.version;
            cache.UID = data.uid;
            cache.LastestConnection = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            cache.accountBackupjsondata = ObjectTojson(data);
        //}

        string s = ObjectTojson(cache);
        var encrypt = Encrypt(StringToByte(s), "420241");
        string filepath = Streaming_Directory;
        saveEncryptFile(encrypt, filepath, "ConnectCache");

    }
    public static ConnectionCache loadConnectionCache()
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
}
