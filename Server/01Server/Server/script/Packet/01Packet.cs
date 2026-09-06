using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;


public enum PacketID
{
    C_ConnectRequest, C_SaveUserAccountData, S_ErrorPacket, S_LoadAccount, C_CreateAccount, S_Message, S_LoadShopList, C_RequestShopList, C_ApplicationVersionCheck, S_DownLoadPacket
        , S_ApplicationDownLoadPacket, C_SaveShop
}
public class S_ApplicationDownLoadPacket : IPacket
{
    public string ApplicationDownLoadLink;
    public string message;
    public ushort Protocol { get { return (ushort)PacketID.S_ApplicationDownLoadPacket; } }

    public ushort Read(ArraySegment<byte> segment, ushort count = 4)
    {

        ushort URLCount = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);



        ushort ApplicationDownLoadLinklen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        ApplicationDownLoadLink = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, ApplicationDownLoadLinklen);
        count += ApplicationDownLoadLinklen;

        ushort messagelen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        message = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, messagelen);
        count += messagelen;

        return count;
    }

    public ArraySegment<byte> Write()
    {


        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
      ushort count = 2;


        Array.Copy(BitConverter.GetBytes(Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);


        ushort ApplicationDownLoadLinklen = (ushort)Encoding.Unicode.GetByteCount(this.ApplicationDownLoadLink);
        Array.Copy(BitConverter.GetBytes(ApplicationDownLoadLinklen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(Encoding.Unicode.GetBytes(ApplicationDownLoadLink), 0, segment.Array, segment.Offset + count, ApplicationDownLoadLinklen);
        count += ApplicationDownLoadLinklen;



        ushort messagelen = (ushort)Encoding.Unicode.GetByteCount(this.message);
        Array.Copy(BitConverter.GetBytes(messagelen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(Encoding.Unicode.GetBytes(message), 0, segment.Array, segment.Offset + count, messagelen);
        count += messagelen;
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));
        return SendBufferHelper.Close(count);
    }
}
public class S_DownLoadPacket : IPacket
{

    public List<string> URL;
    public List<string> FileName;
    public int FileByte;
    public string message;
    public ushort Protocol { get { return (ushort)PacketID.S_DownLoadPacket; } }

    public ushort Read(ArraySegment<byte> segment, ushort count = 4)
    {

        ushort URLCount = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        URL = new List<string>();
        FileName = new List<string>();

        for (int n = 0; n < URLCount; n++)
        {
            ushort URLlen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            URL[n] = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, URLlen);
            count += URLlen;

            ushort FileNamelen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            FileName[n] = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, FileNamelen);
            count += FileNamelen;


        }
        FileByte = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);

        ushort messagelen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        message = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, messagelen);
        count += messagelen;

        return count;
    }

    public ArraySegment<byte> Write()
    {

 
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
      ushort count = 2;


        Array.Copy(BitConverter.GetBytes(Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        Array.Copy(BitConverter.GetBytes(URL.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        for (int n = 0; n < URL.Count; n++)
        {
            ushort URLlen = (ushort)Encoding.Unicode.GetByteCount(this.URL[n]);

            Array.Copy(BitConverter.GetBytes(URLlen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(URL[n]), 0, segment.Array, segment.Offset + count, URLlen);
            count += URLlen;

            ushort FileNamelen = (ushort)Encoding.Unicode.GetByteCount(this.FileName[n]);
            Array.Copy(BitConverter.GetBytes(FileNamelen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(FileName[n]), 0, segment.Array, segment.Offset + count, FileNamelen);
            count += FileNamelen;

        }
        Array.Copy(BitConverter.GetBytes(FileByte), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);

        ushort messagelen = (ushort)Encoding.Unicode.GetByteCount(this.message);
        Array.Copy(BitConverter.GetBytes(messagelen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(Encoding.Unicode.GetBytes(message), 0, segment.Array, segment.Offset + count, messagelen);
        count += messagelen;

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }

}
public interface IPacket
{
    ushort Protocol { get; }
    ushort Read(ArraySegment<byte> segment, ushort count = 4);
    ArraySegment<byte> Write();
}
public class S_Message : IPacket
{
    public string message;

    public ushort Protocol { get { return (ushort)PacketID.S_Message; } }

    public ushort Read(ArraySegment<byte> segment, ushort count = 4)
    {
        ushort messageLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        this.message = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, messageLen);
        count += messageLen;

        return 0;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
      ushort count = 2;
        Array.Copy(BitConverter.GetBytes(Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        ushort messageLen = (ushort)Encoding.Unicode.GetByteCount(this.message);
        Array.Copy(BitConverter.GetBytes(messageLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        Array.Copy(Encoding.Unicode.GetBytes(message), 0, segment.Array, segment.Offset + count, messageLen);
        count += messageLen;
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }

}
public class C_SaveShopPacket : IPacket
{

    public string UID;
    public int shopid;
    public int rerollcount;
    public List<ShopItem> itemlist;
    public UserAccountData account;
    public ushort Protocol { get { return (ushort)PacketID.C_SaveShop; } }

    public ushort Read(ArraySegment<byte> segment, ushort count = 4)
    {
        ushort UIDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        this.UID = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, UIDLen);
        count += UIDLen;
        this.shopid = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
        this.rerollcount = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
        ushort itemListLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        itemlist = new List<ShopItem>();
        for (int n = 0; n < itemListLen; n++)
        {
            itemlist.Add(new ShopItem());
            count = itemlist[n].Read(segment, count);
        }
        account = new UserAccountData();
        count += account.Read(segment, count);
        return count;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 2;
        Array.Copy(BitConverter.GetBytes(Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        ushort UIDLen = (ushort)Encoding.Unicode.GetByteCount(this.UID);
        Array.Copy(BitConverter.GetBytes(UIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(Encoding.Unicode.GetBytes(this.UID), 0, segment.Array, segment.Offset + count, UIDLen);
        count += UIDLen;
        Array.Copy(BitConverter.GetBytes(shopid), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);
        Array.Copy(BitConverter.GetBytes(rerollcount), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);
        Array.Copy(BitConverter.GetBytes(itemlist.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        for (int n = 0; n < itemlist.Count; n++)
        {
            var a = itemlist[n].Write(segment, count);
            segment = a.Item1;
            count = a.Item2;
        }
        var tuple1 = account.Write(segment, count);
        segment = tuple1.Item1;
        count = tuple1.Item2;

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));
    
        return SendBufferHelper.Close(count);
    }

}

public class S_LoadShopList : IPacket
{
    public string message;
    public int shopid;
    public int rerollcount;
    public List<ShopItem> itemlist;
    public ushort Protocol { get { return (ushort)PacketID.S_LoadShopList; } }

    public ushort Read(ArraySegment<byte> segment, ushort count = 4)
    {
        ushort messageLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        this.message = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, messageLen);
        count += messageLen;
        this.shopid = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
        this.rerollcount = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
        ushort itemListLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        itemlist = new List<ShopItem>();
        for (int n = 0; n < itemListLen; n++)
        {
            itemlist.Add(new ShopItem());
            count = itemlist[n].Read(segment, count);
        }
        return count;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
      ushort count = 2;
        Array.Copy(BitConverter.GetBytes(Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        ushort messageLen = (ushort)Encoding.Unicode.GetByteCount(this.message);
        Array.Copy(BitConverter.GetBytes(messageLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        Array.Copy(Encoding.Unicode.GetBytes(message), 0, segment.Array, segment.Offset + count, messageLen);
        count += messageLen;

        Array.Copy(BitConverter.GetBytes(shopid), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);
        Array.Copy(BitConverter.GetBytes(rerollcount), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);
        Array.Copy(BitConverter.GetBytes(itemlist.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        for (int n = 0; n < itemlist.Count; n++)
        {
            var a = itemlist[n].Write(segment, count);
            segment = a.Item1;
            count = a.Item2;
        }
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));
   
        return SendBufferHelper.Close(count);
    }

}

public class C_RequestShopList : IPacket
{
    public string UID;
    public int shopid;
    public ushort Protocol { get { return (ushort)PacketID.C_RequestShopList; } }

    public ushort Read(ArraySegment<byte> segment, ushort count = 4)
    {

        ushort UIDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        this.UID = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, UIDLen);
        count += UIDLen;
        this.shopid = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
        return count;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
      ushort count = 2;



        Array.Copy(BitConverter.GetBytes(Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        ushort UIDLen = (ushort)Encoding.Unicode.GetByteCount(this.UID);
        Array.Copy(BitConverter.GetBytes(UIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(Encoding.Unicode.GetBytes(this.UID), 0, segment.Array, segment.Offset + count, UIDLen);
        count += UIDLen;
        Array.Copy(BitConverter.GetBytes(shopid), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));
        return SendBufferHelper.Close(count);
    }
}
public class S_ErrorPacket : IPacket
{
    public string Error;
    public ushort Protocol { get { return (ushort)PacketID.S_ErrorPacket; } }

    public ushort Read(ArraySegment<byte> segment, ushort count = 4)
    {

        count += sizeof(ushort);
        ushort ErrorLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        this.Error = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, ErrorLen);
        count += ErrorLen;
        return count;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
      ushort count = 2;



        Array.Copy(BitConverter.GetBytes(Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        ushort ErrorLen = (ushort)Encoding.Unicode.GetByteCount(this.Error);
        Array.Copy(BitConverter.GetBytes(ErrorLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(Encoding.Unicode.GetBytes(this.Error), 0, segment.Array, segment.Offset + count, ErrorLen);
        count += ErrorLen;

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));
        return SendBufferHelper.Close(count);
    }
}
public class C_ConnectRequest : IPacket
{
    public string UID;
    public ushort Protocol { get { return (ushort)PacketID.C_ConnectRequest; } }

    public ushort Read(ArraySegment<byte> segment, ushort count = 4)
    {

        ushort UIDLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        this.UID = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, UIDLen);
        count += UIDLen;
        return count;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
      ushort count = 2;



        Array.Copy(BitConverter.GetBytes(Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        ushort UIDLen = (ushort)Encoding.Unicode.GetByteCount(this.UID);
        Array.Copy(BitConverter.GetBytes(UIDLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(Encoding.Unicode.GetBytes(this.UID), 0, segment.Array, segment.Offset + count, UIDLen);
        count += UIDLen;
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
//스테미나 1 회복 =5분(300초)
public class S_LoadAccount : IPacket
{
    public S_LoadAccount()
    {
        d = new UserAccountData();
    }
    public UserAccountData d;

    public string ServerMessage;


    public ushort Protocol { get { return (ushort)PacketID.S_LoadAccount; } }

    public ushort Read(ArraySegment<byte> segment, ushort count = 4)
    {

        ushort ServerMessageLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);

        this.ServerMessage = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, ServerMessageLen);
        count += ServerMessageLen;

        d = new UserAccountData();
        count = d.Read(segment, count);

        return count;
    }

    public ArraySegment<byte> Write()
    {



   
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
      ushort count = 2;


        Array.Copy(BitConverter.GetBytes(Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));

        count += sizeof(ushort);
        ushort Messagelen = (ushort)(ushort)Encoding.Unicode.GetByteCount(this.ServerMessage);
        Array.Copy(BitConverter.GetBytes(Messagelen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);


        Array.Copy(Encoding.Unicode.GetBytes(ServerMessage), 0, segment.Array, segment.Offset + count, Messagelen);

        count += Messagelen;

        var a = d.Write(segment, count);

        segment = a.Item1;
        count = a.Item2;
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }

}


public class C_SaveAccountData : IPacket
{
    public C_SaveAccountData()
    {
        d = new UserAccountData();
    }
    public UserAccountData d;

    public ushort Protocol { get { return (ushort)PacketID.C_SaveUserAccountData; } }

    public ushort Read(ArraySegment<byte> segment, ushort count = 4)
    {
        d.Read(segment, count);
        return count;
    }

    public ArraySegment<byte> Write()
    {

       ;
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
      ushort count = 2;


        Array.Copy(BitConverter.GetBytes(Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        var a = d.Write(segment, count);
        segment = a.Item1;
        count = a.Item2;

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));
        return SendBufferHelper.Close(count);
    }

}
public class C_CreateAccountData : IPacket
{

    public string name;

    public ushort Protocol { get { return (ushort)PacketID.C_CreateAccount; } }

    public ushort Read(ArraySegment<byte> segment, ushort count = 4)
    {
        ushort nameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);

        this.name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nameLen);
        count += nameLen;

        return 0;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
      ushort count = 2;
        Array.Copy(BitConverter.GetBytes(Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.name);
        Array.Copy(BitConverter.GetBytes(nameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);

        Array.Copy(Encoding.Unicode.GetBytes(name), 0, segment.Array, segment.Offset + count, nameLen);
        count += nameLen;

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));
        return SendBufferHelper.Close(count);
    }
}
public class C_ApplicationVersionCheck : IPacket
{
    public string Version;

    public List<string> BundleVersionList;


    public ushort Protocol { get { return (ushort)PacketID.C_ApplicationVersionCheck; } }

    public ushort Read(ArraySegment<byte> segment, ushort count = 4)
    {
        ushort VersionLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        this.Version = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, VersionLen);
        count += VersionLen;
        ushort bundleversioncount = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);
        BundleVersionList = new List<string>();
        for (int n = 0; n < bundleversioncount; n++)
        {
            ushort BundleVerLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);

            string BundleVer = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, BundleVerLen);
            count += BundleVerLen;

            BundleVersionList.Add(BundleVer);
        }



        return count;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
      ushort count = 2;
        Array.Copy(BitConverter.GetBytes(Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);


        ushort VersionLen = (ushort)Encoding.Unicode.GetByteCount(this.Version);
        Array.Copy(BitConverter.GetBytes(VersionLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(Encoding.Unicode.GetBytes(Version), 0, segment.Array, segment.Offset + count, VersionLen);
        count += VersionLen;

        Array.Copy(BitConverter.GetBytes(BundleVersionList.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        for (int n = 0; n < BundleVersionList.Count; n++)
        {

            ushort BundleVerLen = (ushort)Encoding.Unicode.GetByteCount(this.BundleVersionList[n]);
            Array.Copy(BitConverter.GetBytes(BundleVerLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(BundleVersionList[n]), 0, segment.Array, segment.Offset + count, BundleVerLen);
            count += BundleVerLen;
        }
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset , sizeof(ushort));

        return SendBufferHelper.Close(count);
    }

}




















