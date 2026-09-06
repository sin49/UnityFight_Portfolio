using MySqlConnector;
using Newtonsoft.Json;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server.script
{

    class ShopDBManager
    {
        static string UpdateFilePath;
        static DateTime UpDateDayTmp;
        static TimeSpan ServerDateChecker;

        public static void UpdateShopDBFile()
        {
            #region DailyShop
            UpdateFilePath = CreateDailyShopFolder();

            UpDateDayTmp = DateTime.Parse(File.ReadAllText(UpdateFilePath + "\\ExpDate.txt"));
            ServerDateChecker = UpDateDayTmp - DateTime.Now;
            if (ServerDateChecker.TotalSeconds < 0)
            {
           
                Directory.Delete(UpdateFilePath, true);
                CreateDailyShopFolder();
            }
            #endregion
        }
        DateTime GetShopDBDirectoryCreateTime(string FileName)
        {
            string FilePath = getDBDirectory + $"\\{FileName}";
            return File.GetCreationTime(FilePath);
        }
        static string getDBDirectory
        {
            get
            {
                string s = Directory.GetCurrentDirectory();
                int n = s.IndexOf("\\Server\\");
                return s.Substring(0, n) + "\\shopitemDB";
            }
        }
        static string CreateDailyShopFolder()
        {
            string FilePath = getDBDirectory + $"\\Daily";

            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
                
                string s = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59).ToString();

                File.WriteAllText(FilePath + "\\ExpDate.txt", s);
            }
            return FilePath;
        }



        static string CreateShopFolder(Shopid n)
        {
            switch (n)
            {
                case Shopid.Daily:
                    return CreateDailyShopFolder();
                default:
                    return null;

            }
        }

        public static ShopListCache SaveShopCacheFile(Shopid n, ShopListCache cache, string uid)
        {
            string FilePath = CreateShopFolder(n);
            FilePath += $"{uid}.json";
            string json = JsonConvert.SerializeObject(cache, Formatting.Indented);

            File.WriteAllText(FilePath, json);
            return cache;


        }
        public static ShopListCache LoadShopCacheFile(Shopid n, string uid)
        {
            string FilePath = CreateShopFolder(n);
            FilePath += $"\\{uid}.json";
            ShopListCache data;

            if (File.Exists(FilePath))
            {





                using (StreamReader r = new StreamReader(FilePath))
                {
                    string json = r.ReadToEnd();
                    data = JsonConvert.DeserializeObject<ShopListCache>(json);
                }

                return data;
            }
            else
            {
                data = new ShopListCache();
                data.rerollCount = 1;
                data.shoplist = LoadShopList(n);
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);

                File.WriteAllText(FilePath, json);

                return data;
            }
        }


        static List<ShopItem> LoadShopList(Shopid id)
        {
            string FilePath = "";
            switch (id)
            {
                case Shopid.Daily:
                    FilePath = getDBDirectory + $"\\json\\Daily.json";
                    break;
            }

            ShopItemDictionary data;
            using (StreamReader r = new StreamReader(FilePath))
            {
                string json = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<ShopItemDictionary>(json);
            }
            Random ran = new Random();
            List<ShopItem> list = new List<ShopItem>();
            foreach (ShopItemList n in data.ItemDictionary)
            {
                int num = ran.Next(0, n.List.Count);
                list.Add(n.List[num]);
            }
            return list;
        }

    }
}
