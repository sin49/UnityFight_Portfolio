using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
	public class Listener
	{
		Socket _listenSocket;
		Func<Session> _sessionFactory;
		//Queue<SocketAsyncEventArgs> SocketQueue = new Queue<SocketAsyncEventArgs>();

		public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
		{
			_listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_sessionFactory += sessionFactory;
    
            _listenSocket.Bind(endPoint);
			_listenSocket.Listen(backlog);

			for (int i = 0; i < register; i++)
			{
				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
				RegisterAccept(args);
			}
		}

		void RegisterAccept(SocketAsyncEventArgs args)
		{
			args.AcceptSocket = null;

			bool pending = _listenSocket.AcceptAsync(args);
			if (pending == false)
				OnAcceptCompleted(null, args);
		}
		//int Limit=10;
		//public void PlustSocket()
		//{
		//	for (int n = 0; n < Limit; n++)
		//	{
		//		if (SocketQueue.Count == 0)
		//			break;
		//		var args = SocketQueue.Dequeue();
		//		Session session = _sessionFactory.Invoke();
		//		session.Start(args.AcceptSocket);
		//		session.OnConnected(args.AcceptSocket.RemoteEndPoint);
  //              RegisterAccept(args);
  //          }
  //      }
		void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
		{

			if (args.SocketError == SocketError.Success)
			{
				Session session = _sessionFactory.Invoke();
				session.Start(args.AcceptSocket);
				session.OnConnected(args.AcceptSocket.RemoteEndPoint);
			}
			else
				Console.WriteLine(args.SocketError.ToString());

			RegisterAccept(args);
		}
	}
}
