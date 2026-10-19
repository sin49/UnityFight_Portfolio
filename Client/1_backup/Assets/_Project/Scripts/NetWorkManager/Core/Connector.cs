using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    // One connect attempt. No Unity API is called by socket callbacks.
    public class Connector
    {
        Func<Session> sessionFactory;
        Action<string> onError;
        Socket socket;
        volatile bool cancelled;
        readonly object gate = new object();
        bool ownedBySession;

        public void Connect(IPEndPoint endpoint, Func<Session> factory, int count = 1, Action<string> failure = null)
        {
            if (count != 1) throw new ArgumentOutOfRangeException("count", "Only one account connection is supported.");
            sessionFactory = factory;
            onError = failure;
            socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var args = new SocketAsyncEventArgs { RemoteEndPoint = endpoint };
            args.Completed += OnConnectCompleted;
            try
            {
                if (!socket.ConnectAsync(args)) OnConnectCompleted(null, args);
            }
            catch (Exception exception)
            {
                args.Dispose();
                socket.Close();
                if (!cancelled && onError != null) onError(exception.Message);
            }
        }

        public void Cancel()
        {
            lock (gate)
            {
                cancelled = true;
                if (socket != null && !ownedBySession) socket.Close();
            }
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                lock (gate)
                {
                if (cancelled) return;
                if (args.SocketError != SocketError.Success)
                {
                    socket.Close();
                    if (onError != null) onError(args.SocketError.ToString());
                    return;
                }
                Session session = sessionFactory();
                // Initialize socket ownership before publishing the connected state.
                session.Start(args.ConnectSocket);
                ownedBySession = true;
                if (session._disconnected == 0) session.OnConnected(args.RemoteEndPoint);
                }
            }
            catch (Exception exception)
            {
                socket.Close();
                if (!cancelled && onError != null) onError(exception.Message);
            }
            finally { args.Dispose(); }
        }
    }
}
