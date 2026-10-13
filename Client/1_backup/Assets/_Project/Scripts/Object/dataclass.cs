using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class ConnectionCache
{
    public string IP;
    public string Name;
    public string UID;
    public string LaunchingVersion;
    public string LastestConnection;

    public string accountBackupjsondata;

}

[Serializable]
public class GamePlayCache
{
    public bool BasciGamePlayTutorialComplete1;
    public bool BasciGamePlayTutorialComplete2;
    public int CharacterSortOption1;
    public int CharacterSortOption2;
}

[Serializable]
public class CharacterData
{
    public string id;
    string _name;
    public string Name { get {
            if (_name == null)
            {
                return _name=LoadManager.instance.ReturnChrDataName(this);
            }
            else
                return _name;
        } set { 
        Name = value;
        } }
    public int LV;
    public int Grade;
    public Sprite charactersprite;
  
    public virtual void setData(CharacterInformation info)
    {
        id = CharacterContentMapping.Canonicalize(info.id);
        Name = info.name;
        LV = 1;
    }
    public virtual Tuple<ArraySegment<byte>, ushort> Write(ArraySegment<byte> segment, ushort count)
    {


        this.id = CharacterContentMapping.Canonicalize(this.id);
        ushort UIDLen = (ushort)Encoding.Unicode.GetByteCount(this.id);
        Array.Copy(BitConverter.GetBytes(UIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Console.WriteLine($" IDLen{UIDLen} ID{id}");
        Array.Copy(Encoding.Unicode.GetBytes(this.id), 0, segment.Array, segment.Offset + count, UIDLen);
        count += UIDLen;
        //ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.Name);
        //Array.Copy(BitConverter.GetBytes(nameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        //count += sizeof(ushort);
        //Array.Copy(Encoding.Unicode.GetBytes(this.Name), 0, segment.Array, segment.Offset + count, nameLen);
        //count += nameLen;

        Array.Copy(BitConverter.GetBytes(this.LV), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);


        Array.Copy(BitConverter.GetBytes(this.Grade), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);


        return new Tuple<ArraySegment<byte>, ushort>(segment, count);

    }

    public virtual ushort Read(ArraySegment<byte> segment, ushort count)
    {

        ushort IDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);

        this.id = CharacterContentMapping.Canonicalize(Encoding.Unicode.GetString(segment.Array, segment.Offset + count, IDLen));
        count += IDLen;


        //ushort nameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        //count += sizeof(ushort);

        //this.Name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nameLen);
        //count += nameLen;

        this.LV = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);

        this.Grade = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);


        return count;
    }

}

[Serializable]
public class PlayerData : CharacterData
{
    public PlayerData()
    {
        if (skillLV != null)
        {
            for (int n = 0; n < skillLV.Length; n++)
            {
                if (skillLV[n] == 0)
                    skillLV[n] = 1;
            }
        }
        if (SpecialLV == 0)
            SpecialLV = 1;
    }
    public int EXP;

    public int[] skillLV;
    public int SpecialLV;
    public void getEXP(int n)
    {
        EXP += n;
        while (EXP >= getmaxexp(LV))
        {
            EXP -= getmaxexp(LV);
            LV++;



        }
    }
    public int getmaxexp(int lv)
    {
        int exp = 100;
        for (int n = 0; n < lv; n++)
        {
            if (exp > 5000)
                break;
            else
                exp += exp / 5;
        }

        return exp;
    }

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
public class ItemData
{
    public string id;
    public string Name;
    public string desc;
    public Sprite itemSprite;

}
[Serializable]
public class item
{
    public string itemid;
    public ItemData data { get; set; }
    public int Amount;
    public virtual ushort Read(ArraySegment<byte> segment, ushort count)
    {
        ushort IDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        this.itemid = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, IDLen);
        count += IDLen;


        Amount = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);



        return count;
    }

    public virtual Tuple<ArraySegment<byte>, ushort> Write(ArraySegment<byte> segment, ushort count)
    {

        ushort IDLen = (ushort)Encoding.Unicode.GetByteCount(this.itemid);
        Array.Copy(BitConverter.GetBytes(IDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(Encoding.Unicode.GetBytes(this.itemid), 0, segment.Array, segment.Offset + count, IDLen);
        count += IDLen;
        Array.Copy(BitConverter.GetBytes(this.Amount), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);



        return new Tuple<ArraySegment<byte>, ushort>(segment, count);

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
            count = a.Read(segment, count);
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
    public StageMissionEnum mission;
    public Tuple<ArraySegment<byte>, ushort> Write(ArraySegment<byte> segment, ushort count)
    {

        Array.Copy(BitConverter.GetBytes(this.Clear), 0, segment.Array, segment.Offset + count, sizeof(bool));
        count += sizeof(bool);
        Array.Copy(BitConverter.GetBytes((int)mission), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);
        return new Tuple<ArraySegment<byte>, ushort>(segment, count);
    }
    public virtual ushort Read(ArraySegment<byte> segment, ushort count = 2)
    {
        Clear = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
        count += sizeof(bool);
        mission = (StageMissionEnum)BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
        return count;
    }
}
[Serializable]
public enum StageMissionEnum { clear, PlayerDeath_0, minLimit_120 }


