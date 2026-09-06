using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;



[Serializable]
public class ShopItem : item
{

    public String Needitemid;
    public ItemData NeedItem { get; set; }
    public int price;
    public override Tuple<ArraySegment<byte>, ushort> Write(ArraySegment<byte> segment, ushort count)
    {
        ushort IDLen = (ushort)Encoding.Unicode.GetByteCount(this.itemid);
        Array.Copy(BitConverter.GetBytes(IDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        Array.Copy(Encoding.Unicode.GetBytes(this.itemid), 0, segment.Array, segment.Offset + count, IDLen);
        count += IDLen;

        Array.Copy(BitConverter.GetBytes(this.Amount), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        ushort NeeditemidLen = (ushort)Encoding.Unicode.GetByteCount(this.Needitemid);
        Array.Copy(BitConverter.GetBytes(NeeditemidLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        Array.Copy(Encoding.Unicode.GetBytes(this.Needitemid), 0, segment.Array, segment.Offset + count, NeeditemidLen);
        count += NeeditemidLen;

        Array.Copy(BitConverter.GetBytes(this.price), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);

        return new Tuple<ArraySegment<byte>, ushort>(segment, count);
    }
    public override ushort Read(ArraySegment<byte> segment, ushort count)
    {
        ushort IDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        this.itemid = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, IDLen);
        count += IDLen;


        Amount = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);

        ushort NeeditemidLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        this.Needitemid = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, NeeditemidLen);
        count += NeeditemidLen;

        price = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);

        return count;
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
    public int itemCount;

    public List<ShopItemList> ItemDictionary;

}