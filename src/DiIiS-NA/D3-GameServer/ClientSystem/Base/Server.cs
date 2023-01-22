//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Net.Sockets;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.ClientSystem.Base
{
	public class Server
	{
		public bool IsListening { get; private set; }
		public int Port { get; private set; }

		protected AsyncAcceptor Acceptor;
		// protected Dictionary<Socket, IConnection> Connections = new Dictionary<Socket, IConnection>();
		NetworkThread NetThread;

		public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs e);
		public delegate void ConnectionDataEventHandler(object sender, ConnectionDataEventArgs e);

		public event ConnectionEventHandler OnConnect;
		public event ConnectionEventHandler OnDisconnect;
		public event ConnectionDataEventHandler DataReceived;

		protected static readonly Logger Logger = LogManager.CreateLogger("S");
		private static bool _disposed = false;

		public virtual void Run() { }

		#region listener

		public virtual bool Listen(string bindIP, int port)
		{
			// Check if the server has been disposed.
			if (_disposed) throw new ObjectDisposedException(GetType().Name, "Server has been disposed.");

			// Check if the server is already listening.
			if (IsListening) throw new InvalidOperationException("Server is already listening.");
			Port = port;

			Acceptor = new AsyncAcceptor();
			if (!Acceptor.Start(bindIP, port))
			{
				Logger.Fatal("Listen failed to Start AsyncAcceptor on {0}", bindIP);
				return false;
			}

			//TODO: multiple threads
			NetThread = new NetworkThread();
			NetThread.Start();


			// Setup our options:
			// * NoDelay - true - don't use packet coalescing
			// * DontLinger - true - don't keep sockets around once they've been disconnected
			// * IPv6Only - false - create a dual-socket that both supports IPv4 and IPv6 - check the IPv6 support note above.
			//Listener.Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
			//Listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
			//Listener.Socket.LingerState = new LingerOption(enable: false, seconds: 0);

			// Start listening for incoming connections.
			IsListening = true;

			Acceptor.AsyncAcceptSocket(OnSocketOpen);
			return true;
		}

		public virtual void OnSocketOpen(Socket sock)
		{
			try
			{
				//TSocketType newSocket = (TSocketType)Activator.CreateInstance(typeof(TSocketType), sock);
				//newSocket.Start();
				var conn = new Connection(sock);
				conn.Server = this;
				NetThread.AddSocket(conn);

				OnClientConnection(new ConnectionEventArgs(conn)); // Raise the OnConnect event.
				conn.AsyncRead();
			}
			catch (Exception err)
			{
				Logger.ErrorException(err, "Exception: ");
			}
		}


		#endregion

		#region events

		public virtual void OnClientConnection(ConnectionEventArgs e)
		{
			var handler = OnConnect;
			if (handler != null) handler(this, e);
		}

		public virtual void OnClientDisconnect(ConnectionEventArgs e)
		{
			var handler = OnDisconnect;
			if (handler != null) handler(this, e);
		}

		public virtual void OnDataReceived(ConnectionDataEventArgs e)
		{
			var handler = DataReceived;
			if (handler != null) handler(this, e);
		}

		#endregion
	}
}
