using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


[Serializable]
public class UserAccountDataNetworkMember
{
   

    public string _uid;

    public string username;

    public int _accountlv;


    public int _Max_Exp = 10;

    public int _Exp;

    public string LastestConnect;

    public string MaxStageClearData;

    public string StarChracter;

    public List< item> Itemlist;
    


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
        Itemlist = new List<item>();
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
            = CharacterContentMapping.Canonicalize(Encoding.Unicode.GetString(segment.Array, segment.Offset + count, Starchr));
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
            Characterlist.Add( a);
            //_CharacterList.Add(a);


        }


        ushort IDataListLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);

        for (int n = 0; n < IDataListLen; n++)
        {
            var a = new item();
            count = a.Read(segment, count);
            Itemlist.Add( a);

        }
        ushort SDataListLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);

        count += sizeof(ushort);
        for (int n = 0; n < SDataListLen; n++)
        {
            var a = new StageData();
            count = a.Read(segment, count);
            Stagelist.Add( a);

        }


        return count;
    }


    public Tuple<ArraySegment<byte>, ushort> Write(ArraySegment<byte> segment, ushort count)
    {
        CharacterContentMapping.NormalizeData(this);




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
        LastestConnect = DateTime.Now.ToString("yyyyMMddhhmmss");
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
        foreach ( PlayerData data in Characterlist)
        {
            var a = data.Write(segment, count);
            segment = a.Item1;
            count = a.Item2;
        }


        ushort IDataListLen = (ushort)Itemlist.Count;
        Array.Copy(BitConverter.GetBytes(IDataListLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        foreach ( item data in Itemlist)
        {
            var a = data.Write(segment, count);
            segment = a.Item1;
            count = a.Item2;
        }

        ushort SDataListLen = (ushort)Stagelist.Count;
        Console.WriteLine("SdataListLen:" + SDataListLen);
        Array.Copy(BitConverter.GetBytes(SDataListLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        foreach ( StageData data in Stagelist)
        {
            var a = data.Write(segment, count);
            segment = a.Item1;
            count = a.Item2;
        }

        return new Tuple<ArraySegment<byte>, ushort>(segment, count);
    }

}
