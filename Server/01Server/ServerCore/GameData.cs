using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    [Serializable]
    public class UserAccountData
    {


        public string _uid;

        public string username;

        public int _accountlv;


        public int _Max_Exp = 10;

        public int _Exp;

        public string LastestConnect;

        public string MaxStageClearData;

        public string StarChracter;

        public List<Item> Itemlist;



        public List<PlayerData> Characterlist;

        public bool TutorialClear1;
        public bool TutorialClear2;

        public List<StageData> Stagelist;


        public int _MaxStamina;

        public int _Stamina;

        public float _StaminaTimer;

        public ushort Read(ArraySegment<byte> segment, ushort count = 2)
        {
            Characterlist = new List<PlayerData>();
            Itemlist = new List<Item>();
            Stagelist = new List<StageData>();




            ushort UIDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);


            this._uid = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, UIDLen);
            count += UIDLen;


            ushort Namelen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);

            this.username = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, Namelen);
            count += Namelen;

            this._accountlv = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);

            this._Max_Exp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);
            this._Exp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);
            this._MaxStamina = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);


            this._Stamina = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);

            this._StaminaTimer = BitConverter.ToSingle(segment.Array, segment.Offset + count);
            count += sizeof(float);

            ushort Starchr = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);

            this.StarChracter
                = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, Starchr);
            count += Starchr;


            ushort Scleardata = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);

            this.MaxStageClearData = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, Scleardata);
            count += Scleardata;


            ushort Lconnect = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);

            this.LastestConnect = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, Lconnect);

            count += Lconnect;
            TutorialClear1 = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);
            TutorialClear2 = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);
            ushort CDataListLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);

            for (int n = 0; n < CDataListLen; n++)
            {

                var a = new PlayerData();
                count = a.Read(segment, count);
                Characterlist.Add(a);
                //_CharacterList.Add(a);


            }


            ushort IDataListLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);

            for (int n = 0; n < IDataListLen; n++)
            {
                var a = new Item();
                count = a.Read(segment, count);
                Itemlist.Add(a);

            }
            ushort SDataListLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);

            count += sizeof(ushort);
            for (int n = 0; n < SDataListLen; n++)
            {
                var a = new StageData();
                count = a.Read(segment, count);
                Stagelist.Add(a);

            }


            return count;
        }


        public Tuple<ArraySegment<byte>, ushort> Write(ArraySegment<byte> segment, ushort count)
        {




            ushort UIDLen = (ushort)Encoding.Unicode.GetByteCount(this._uid);
            Array.Copy(BitConverter.GetBytes(UIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            Array.Copy(Encoding.Unicode.GetBytes(this._uid), 0, segment.Array, segment.Offset + count, UIDLen);
            count += UIDLen;

            ushort Namelen = (ushort)Encoding.Unicode.GetByteCount(this.username);
            Array.Copy(BitConverter.GetBytes(Namelen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            Array.Copy(Encoding.Unicode.GetBytes(this.username), 0, segment.Array, segment.Offset + count, Namelen);
            count += Namelen;

            Array.Copy(BitConverter.GetBytes(this._accountlv), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);

            Array.Copy(BitConverter.GetBytes(this._Max_Exp), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);

            Array.Copy(BitConverter.GetBytes(this._Exp), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);

            Array.Copy(BitConverter.GetBytes(this._MaxStamina), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);

            Array.Copy(BitConverter.GetBytes(this._Stamina), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);


            Array.Copy(BitConverter.GetBytes(this._StaminaTimer), 0, segment.Array, segment.Offset + count, sizeof(float));
            count += sizeof(float);


            ushort Starchr = (ushort)Encoding.Unicode.GetByteCount(StarChracter);
            Array.Copy(BitConverter.GetBytes(Starchr), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(StarChracter), 0, segment.Array, segment.Offset + count, Starchr);
            count += Starchr;


            ushort Scleardata = (ushort)Encoding.Unicode.GetByteCount(MaxStageClearData);
            Array.Copy(BitConverter.GetBytes(Scleardata), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(MaxStageClearData), 0, segment.Array, segment.Offset + count, Scleardata);
            count += Scleardata;

            ushort LConnect = (ushort)Encoding.Unicode.GetByteCount(LastestConnect);
            Array.Copy(BitConverter.GetBytes(LConnect), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(LastestConnect), 0, segment.Array, segment.Offset + count, LConnect);
            count += LConnect;

            Array.Copy(BitConverter.GetBytes(TutorialClear1), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);
            Array.Copy(BitConverter.GetBytes(TutorialClear2), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);




            ushort CDataListLen = (ushort)Characterlist.Count;
            Array.Copy(BitConverter.GetBytes(CDataListLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            foreach (PlayerData data in Characterlist)
            {
                var a = data.Write(segment, count);
                segment = a.Item1;
                count = a.Item2;
            }


            ushort IDataListLen = (ushort)Itemlist.Count;
            Array.Copy(BitConverter.GetBytes(IDataListLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            foreach (Item data in Itemlist)
            {
                var a = data.Write(segment, count);
                segment = a.Item1;
                count = a.Item2;
            }

            ushort SDataListLen = (ushort)Stagelist.Count;

            Array.Copy(BitConverter.GetBytes(SDataListLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            foreach (StageData data in Stagelist)
            {
                var a = data.Write(segment, count);
                segment = a.Item1;
                count = a.Item2;
            }

            return new Tuple<ArraySegment<byte>, ushort>(segment, count);
        }

    }
   
    [Serializable]
    public class Item
    {
        public string Itemid;
        public ItemData data { get; set; }
        public int Amount;
        public virtual ushort Read(ArraySegment<byte> segment, ushort count)
        {
            ushort IDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.Itemid = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, IDLen);
            count += IDLen;


            Amount = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);



            return count;
        }

        public virtual Tuple<ArraySegment<byte>, ushort> Write(ArraySegment<byte> segment, ushort count)
        {

            ushort IDLen = (ushort)Encoding.Unicode.GetByteCount(this.Itemid);
            Array.Copy(BitConverter.GetBytes(IDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(this.Itemid), 0, segment.Array, segment.Offset + count, IDLen);
            count += IDLen;
            Array.Copy(BitConverter.GetBytes(this.Amount), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);



            return new Tuple<ArraySegment<byte>, ushort>(segment, count);

        }
    }
    [Serializable]
    public class ItemData
    {
        public string id;
        public string Name;
        public string desc;
        public Tuple<ArraySegment<byte>, ushort> Write(ArraySegment<byte> segment, ushort count)
        {

            ushort UIDLen = (ushort)Encoding.Unicode.GetByteCount(this.id);
            Array.Copy(BitConverter.GetBytes(UIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            Array.Copy(Encoding.Unicode.GetBytes(this.id), 0, segment.Array, segment.Offset + count, UIDLen);
            count += UIDLen;
            ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.Name);
            Array.Copy(BitConverter.GetBytes(nameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(this.Name), 0, segment.Array, segment.Offset + count, nameLen);
            count += nameLen;

            ushort descLen = (ushort)Encoding.Unicode.GetByteCount(this.desc);
            Array.Copy(BitConverter.GetBytes(descLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(this.desc), 0, segment.Array, segment.Offset + count, descLen);
            count += descLen;

            return new Tuple<ArraySegment<byte>, ushort>(segment, count);

        }

        public ushort Read(ArraySegment<byte> segment, ushort count = 2)
        {
            ushort IDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, IDLen);
            count += IDLen;

            ushort nameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.Name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nameLen);
            count += nameLen;


            ushort descLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.desc = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, descLen);
            count += descLen;

            return count;
        }

    }
    [Serializable]
    public class StageData
    {
        public string ID;
        public string name;
        public List<StageMission> stagemission = new List<StageMission>();
        public bool stagelock;
        public ushort Read(ArraySegment<byte> segment, ushort count = 0)
        {
            ushort IDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.ID = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, IDLen);
            count += IDLen;

            ushort namelen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, namelen);
            count += namelen;


            ushort stagemissionLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);


            for (int n = 0; n < stagemissionLen; n++)
            {
                StageMission a = new StageMission();
                count = a.Read(segment,count);
                stagemission.Add(a);
            }
            this.stagelock = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);

            return count;
        }

        public Tuple<ArraySegment<byte>, ushort> Write(ArraySegment<byte> segment, ushort count)
        {

            ushort UIDLen = (ushort)Encoding.Unicode.GetByteCount(this.ID);
            Array.Copy(BitConverter.GetBytes(UIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(this.ID), 0, segment.Array, segment.Offset + count, UIDLen);
            count += UIDLen;

            ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.name);
            Array.Copy(BitConverter.GetBytes(nameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(this.name), 0, segment.Array, segment.Offset + count, nameLen);
            count += nameLen;

            ushort stagemissionLen = (ushort)stagemission.Count;
            Array.Copy(BitConverter.GetBytes(stagemissionLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            for (int n = 0; n < stagemission.Count; n++)
            {
                var a = stagemission[n].Write(segment, count);
                segment = a.Item1;
                count = a.Item2;

            }

          
            Array.Copy(BitConverter.GetBytes(stagelock), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);


            return new Tuple<ArraySegment<byte>, ushort>(segment, count);

        }
    }
    [Serializable]
    public class StageMission
    {
        public bool Clear = false;
        public int mission;
        public Tuple<ArraySegment<byte>, ushort> Write(ArraySegment<byte> segment, ushort count)
        {

            Array.Copy(BitConverter.GetBytes(this.Clear), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);
            Array.Copy(BitConverter.GetBytes(this.mission), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);
            return new Tuple<ArraySegment<byte>, ushort>(segment, count);
        }
        public virtual ushort Read(ArraySegment<byte> segment, ushort count = 2)
        {
            Clear = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);
            mission = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(int);
            return count;
        }
}
        [Serializable]
    public class CharacterData
    {
        public string id;
        public string Name;
        public int LV;
        public int Grade;
        public virtual Tuple<ArraySegment<byte>, ushort> Write(ArraySegment<byte> segment, ushort count)
        {

            ushort UIDLen = (ushort)Encoding.Unicode.GetByteCount(this.id);
            Array.Copy(BitConverter.GetBytes(UIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            Array.Copy(Encoding.Unicode.GetBytes(this.id), 0, segment.Array, segment.Offset + count, UIDLen);
            count += UIDLen;
         
            Array.Copy(BitConverter.GetBytes(this.LV), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);

            Array.Copy(BitConverter.GetBytes(this.Grade), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);

            return new Tuple<ArraySegment<byte>, ushort>(segment, count);

        }
   

        public virtual ushort Read(ArraySegment<byte> segment, ushort count = 2)
        {
            ushort IDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
          
            this.id = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, IDLen);
            count += IDLen;


            this.LV = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);



            this.Grade = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);

            return count;
        }

    }

    [Serializable]
    public class PlayerData:CharacterData
    {

        public int EXP;

        public int[] skillLV;
        public int SpecialLV;


        public bool characterlock;
        public override ushort Read(ArraySegment<byte> segment, ushort count = 2)
        {
            count = base.Read(segment, count);


            this.EXP = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);

            ushort SkillListLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);

            skillLV = new int[SkillListLen];

            for (int n = 0; n < SkillListLen; n++)
            {
                int tmp = BitConverter.ToInt32(segment.Array, segment.Offset + count);
                skillLV[n] = tmp;
                count += sizeof(int);
            }

            this.SpecialLV = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);

            this.characterlock = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);

            return count;
        }
        public override Tuple<ArraySegment<byte>, ushort> Write(ArraySegment<byte> segment, ushort count)
        {
            Tuple<ArraySegment<byte>, ushort> tuple = base.Write(segment, count);
            segment = tuple.Item1;
            count = tuple.Item2;


            Array.Copy(BitConverter.GetBytes(EXP), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);

            ushort SkillLVLen = (ushort)skillLV.Length;
            Array.Copy(BitConverter.GetBytes(SkillLVLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            for (int n = 0; n < skillLV.Length; n++)
            {
                Array.Copy(BitConverter.GetBytes(this.skillLV[n]), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);

            }
            Array.Copy(BitConverter.GetBytes(SpecialLV), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);
            Array.Copy(BitConverter.GetBytes(characterlock), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);


            return new Tuple<ArraySegment<byte>, ushort>(segment, count);
        }
    }
    [Serializable]
    public class ShopItem : Item
    {

        public String NeedItemid;

        public int price;
        public override ushort Read(ArraySegment<byte> segment, ushort count)
        {
            ushort IDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.Itemid = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, IDLen);
            count += IDLen;


            Amount = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);

            ushort NeedItemidLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.NeedItemid = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, NeedItemidLen);
            count += NeedItemidLen;

            price = BitConverter.ToInt32(segment.Array, segment.Offset + count);
            count += sizeof(int);

            return count;
        }
        public override Tuple<ArraySegment<byte>, ushort> Write(ArraySegment<byte> segment, ushort count)
        {
            ushort IDLen = (ushort)Encoding.Unicode.GetByteCount(this.Itemid);
            Array.Copy(BitConverter.GetBytes(IDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            Array.Copy(Encoding.Unicode.GetBytes(this.Itemid), 0, segment.Array, segment.Offset + count, IDLen);
            count += IDLen;

            Array.Copy(BitConverter.GetBytes(this.Amount), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            ushort NeedItemidLen = (ushort)Encoding.Unicode.GetByteCount(this.NeedItemid);
            Array.Copy(BitConverter.GetBytes(NeedItemidLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            Array.Copy(Encoding.Unicode.GetBytes(this.NeedItemid), 0, segment.Array, segment.Offset + count, NeedItemidLen);
            count += NeedItemidLen;

            Array.Copy(BitConverter.GetBytes(this.price), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);

            return new Tuple<ArraySegment<byte>, ushort>(segment, count);
        }
    }
    [Serializable]
    public class ShopItemList
    {
        public List<ShopItem> List;
    }

    [Serializable]
    public class ShopItemDictionary
    {

        public string id;
        public string desc;
        public int ItemCount;

        public List<ShopItemList> ItemDictionary;

    }
    [Serializable]
    public class ShopListCache
    {
      public  int rerollCount;
        public List<ShopItem> shoplist;
    }

    [Serializable]
    public class ShopItemListforsql
    {
        public ShopItem Item;
        public SByte weight;
    }
    public enum Shopid { Daily }
}
