using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ServerCore
{
	public class Connector
	{
		Func<Session> _sessionFactory;
		float timer;
		bool Tryconnect;
		public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
		{
			if (Tryconnect)
				return;
			for (int i = 0; i < count; i++)
			{
				// 휴대폰 설정
				Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				_sessionFactory = sessionFactory;

				SocketAsyncEventArgs args = new SocketAsyncEventArgs();
				args.Completed += OnConnectCompleted;
				args.RemoteEndPoint = endPoint;
				args.UserToken = socket;
				timer = 0;
				Tryconnect = true;
				RegisterConnect(args);
			}
		}

		void RegisterConnect(SocketAsyncEventArgs args)
		{
			
			Socket socket = args.UserToken as Socket;
			if (socket == null)
				return;

			bool pending = socket.ConnectAsync(args);
  
            Task.Delay(1000);
            timer += 1;
            if (pending == false)
				OnConnectCompleted(null, args);
		}

		void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
		{
			if (args.SocketError == SocketError.Success)
			{
				Tryconnect = false;
				Session session = _sessionFactory.Invoke();
				session.Start(args.ConnectSocket);
				session.OnConnected(args.RemoteEndPoint);
			
			
			}
			else
			{
                if (timer >= 10)
                    RegisterConnect(args);
                else
                {
                    Tryconnect = false;
					GameManager.instance.ActiveError("서버와의 대기시간이 초과했습니다.");
                }

            }
		}
	}
}
