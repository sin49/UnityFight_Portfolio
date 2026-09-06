using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;
namespace Server.script
{

    class PacketQueue : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }
        public void Flush()
        {
            for (int n = 0; n < _sessions.Count; n++)
            {
                _sessions[n].Send(_pendingList);
                _sessions.RemoveAt(n);
                n--;
            }

            _pendingList.Clear();
        }
        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }
        public void Enter(ClientSession session)
        {

            _sessions.Add(session);


            //S_PlayerList players = new S_PlayerList();

            //session.Send(players.Write());

            //Broadcast(enter.Write());
        }

    }
}
