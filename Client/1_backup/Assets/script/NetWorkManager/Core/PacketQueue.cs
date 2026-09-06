using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketQueue
{
    public static PacketQueue instance { get; } = new PacketQueue();

    Queue<IPacket> packetqueue = new Queue<IPacket>();
    object _lock = new object();
    public void Push(IPacket packet)
    {
        packetqueue.Enqueue(packet);
    }
    public IPacket Pop()
    {
        lock (_lock)
        {
            if (packetqueue.Count == 0)
                return null;

            return packetqueue.Dequeue();
        }
    }
    public List<IPacket> PopAll()
    {
        List<IPacket> list = new List<IPacket>();

        lock (_lock)
        {

            while (packetqueue.Count > 0)
            {
                list.Add(packetqueue.Dequeue());
            }

        }
        return list;
    }
}
