using System.Collections.Generic;
using ServerCore;

public class PacketQueue
{
    public sealed class ReceivedPacket
    {
        public readonly PacketSession Session;
        public readonly IPacket Packet;
        public ReceivedPacket(PacketSession session, IPacket packet) { Session = session; Packet = packet; }
    }
    public static PacketQueue instance { get; } = new PacketQueue();
    readonly Queue<ReceivedPacket> packets = new Queue<ReceivedPacket>();
    readonly object gate = new object();

    public void Push(PacketSession session, IPacket packet)
    {
        lock (gate) packets.Enqueue(new ReceivedPacket(session, packet));
    }

    public List<ReceivedPacket> PopAll()
    {
        lock (gate)
        {
            var result = new List<ReceivedPacket>(packets);
            packets.Clear();
            return result;
        }
    }
}
