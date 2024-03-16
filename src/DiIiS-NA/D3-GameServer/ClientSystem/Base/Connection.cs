using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.LoginServer.Battle;

namespace DiIiS_NA.GameServer.ClientSystem.Base
{
	public class Connection : IConnection
	{
		protected static readonly Logger Logger = LogManager.CreateLogger("CN"); // the logger.

		public Server Server { get; set; }


		// Read bytes from the Socket into the buffer in a non-blocking call.
		// This allows us to read no more than the specified count number of bytes.\
		// Note that this method should only be called prior to encryption!
		public int Receive(int start, int count)
		{
			return Socket.Receive(_recvBuffer, start, count, SocketFlags.None);
		}

		// Wrapper for the Send method that will send the data either to the
		// Socket (unecnrypted) or to the TLSStream (encrypted).
		public int _Send(byte[] buffer, int start, int count, SocketFlags flags)
		{
			if (!IsOpen()) return 0;
			int bytes = 0;
			try
			{
				bytes = Socket.Send(buffer, start, count, flags);
			}
			catch
			{
				return 0;
			}
			return bytes;
		}

		/// <summary>
		/// Gets underlying socket.
		/// </summary>
		public Socket Socket { get; private set; }

		/// <summary>
		/// Gets or sets bound client.
		/// </summary>
		public IClient Client { get; set; }

		/// <summary>
		/// Default buffer size.
		/// </summary>
		public static readonly int BufferSize = 16 * 1024; // 16 KB	   

		volatile bool _closed;
		private object socketLock = new object();

		/// <summary>
		/// The recieve buffer.
		/// </summary>
		private readonly byte[] _recvBuffer = new byte[BufferSize];

		public Connection(Socket socket)
		{
			if (socket == null)
				throw new ArgumentNullException("socket");

			LastKeepAliveTick = DateTime.Now.ToUnixTime();
			Socket = socket;
		}

		#region socket stuff

		/// <summary>
		/// Returns remote endpoint.
		/// </summary>
		public IPEndPoint RemoteEndPoint
		{
			get { return (Socket == null) ? null : Socket.RemoteEndPoint as IPEndPoint; }
		}

		/// <summary>
		/// Returns local endpoint.
		/// </summary>
		public IPEndPoint LocalEndPoint
		{
			get { return Socket.LocalEndPoint as IPEndPoint; }
		}

		/// <summary>
		/// Returns the recieve-buffer.
		/// </summary>
		public byte[] RecvBuffer
		{
			get { return _recvBuffer; }
		}

		public void AsyncRead()
		{
			if (!IsOpen())
				return;

			try
			{
				using (var socketEventargs = new SocketAsyncEventArgs())
				{
					socketEventargs.SetBuffer(_recvBuffer, 0, BufferSize);
					socketEventargs.Completed += (sender, args) => ReadCallback(args);
					socketEventargs.SocketFlags = SocketFlags.None;
					socketEventargs.RemoteEndPoint = Socket.RemoteEndPoint;
					socketEventargs.UserToken = this;

					if (!Socket.ReceiveAsync(socketEventargs))
						ReadCallback(socketEventargs);
				}
			}
			catch (Exception ex)
			{
				Logger.ErrorException(ex, "Exception: ");
			}
		}


		void ReadCallback(SocketAsyncEventArgs a)
		{
			if (!IsOpen())
				return;
			var connection = a.UserToken as Connection;
			try
			{
				if (connection.Socket.Available < 0 || connection.Socket == null) return;
			}
			catch
			{
				return;
			}
			try
			{
				if (a.BytesTransferred > 0)
				{
					Server.OnDataReceived(new ConnectionDataEventArgs(connection, connection.RecvBuffer.Enumerate(0, a.BytesTransferred))); // Raise the DataReceived event.

					if (connection.IsOpen())
						connection.AsyncRead();
					else
						Logger.Trace("Connection closed:" + connection.Client);
				}
				else connection.Disconnect(); 
			}
			catch (SocketException e)
			{
				connection.Disconnect(); 
				Logger.WarnException(e, "ReceiveCallback");
			}
			catch (Exception e)
			{
				connection.Disconnect();
				Logger.WarnException(e, "ReceiveCallback");
			}
		}

		

		/// <summary>
		/// Sends byte buffer to remote endpoint.
		/// </summary>
		/// <param name="buffer">Byte buffer to send.</param>
		/// <returns>Returns count of sent bytes.</returns>
		public int Send(byte[] buffer)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (!IsOpen()) return 0;
			return Send(buffer, 0, buffer.Length, SocketFlags.None);
		}

		/// <summary>
		/// Sends byte buffer to remote endpoint.
		/// </summary>
		/// <param name="buffer">Byte buffer to send.</param>
		/// <param name="flags">Sockets flags to use.</param>
		/// <returns>Returns count of sent bytes.</returns>
		public int Send(byte[] buffer, SocketFlags flags)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (!IsOpen()) return 0;
			return Send(buffer, 0, buffer.Length, flags);
		}

		/// <summary>
		/// Sends byte buffer to remote endpoint.
		/// </summary>
		/// <param name="buffer">Byte buffer to send.</param>
		/// <param name="start">Start index to read from buffer.</param>
		/// <param name="count">Count of bytes to send.</param>
		/// <returns>Returns count of sent bytes.</returns>
		public int Send(byte[] buffer, int start, int count)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (!IsOpen()) return 0;
			return Send(buffer, start, count, SocketFlags.None);
		}

		/// <summary>
		/// Sends byte buffer to remote endpoint.
		/// </summary>
		/// <param name="buffer">Byte buffer to send.</param>
		/// <param name="start">Start index to read from buffer.</param>
		/// <param name="count">Count of bytes to send.</param>
		/// <param name="flags">Sockets flags to use.</param>
		/// <returns>Returns count of sent bytes.</returns>
		public int Send(byte[] buffer, int start, int count, SocketFlags flags)
		{
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (!IsOpen()) return 0;

			var totalBytesSent = 0;
			var bytesRemaining = buffer.Length;

			try
			{
				if (Socket == null || Socket.Available < 0) throw new Exception("socket is null");
				lock (socketLock)
				{
					while (bytesRemaining > 0 && IsOpen() && !_closing) // Ensure we send every byte.
					{
						int bytesSent = _Send(buffer, totalBytesSent, bytesRemaining, flags);

						if (bytesSent == 0) break;
						bytesRemaining -= bytesSent;
						totalBytesSent += bytesSent;
					}
				}
			}
			catch (SocketException)
			{
				Disconnect();
			}
			catch (Exception e)
			{
				Disconnect();
				Logger.WarnException(e, "Send");
			}

			return totalBytesSent;
		}

		/// <summary>
		/// Sends an enumarable byte buffer to remote endpoint.
		/// </summary>
		/// <param name="data">Enumrable byte buffer to send.</param>
		/// <returns>Returns count of sent bytes.</returns>
		public int Send(IEnumerable<byte> data)
		{
			if (data == null) throw new ArgumentNullException("data");
			if (!IsOpen()) return 0;
			return Send(data, SocketFlags.None);
		}

		/// <summary>
		/// Sends an enumarable byte buffer to remote endpoint.
		/// </summary>
		/// <param name="data">Enumrable byte buffer to send.</param>
		/// <param name="flags">Sockets flags to use.</param>
		/// <returns>Returns count of sent bytes.</returns>
		public int Send(IEnumerable<byte> data, SocketFlags flags)
		{
			if (data == null) throw new ArgumentNullException("data");
			if (!IsOpen()) return 0;
			var buffer = data.ToArray();
			return Send(buffer, 0, buffer.Length, SocketFlags.None);
		}

		private bool _closing = false;
		/// <summary>
		/// Kills the connection to remote endpoint.
		/// </summary>
		public void Close()
		{
			if (!_closing)
			{
				_closing = true;
				Task.Run(() => {
					try
					{
						Server.OnClientDisconnect(new ConnectionEventArgs(this));
						if (Socket != null)
						{
							try
							{
								Socket.Shutdown(SocketShutdown.Both);
								Socket.Close();
								Socket = null;
							}
							catch (Exception)
							{
								// Ignore any exceptions that might occur during attempt to close the Socket.
							}
						}
					}
					catch (Exception e)
					{
						Logger.WarnException(e, "Disconnect()");
					}
				});
			}
		}

		private bool IsMooNet
		{
			get
			{
				return (Client is BattleClient);
			}
		}

		public void Disconnect()
		{
			_closed = true;
		}

		public bool IsOpen() { return !_closed && Socket != null/* && (!this.IsMooNet || this.LastKeepAliveTick > (DateTime.Now.ToUnixTime() - 120U))*/; }
		public uint LastKeepAliveTick { get; set; }

		/// <summary>
		/// Returns a connection state string.
		/// </summary>
		/// <returns>Connection state string.</returns>
		public override string ToString()
		{
			if (Socket == null)
				return "No Socket!";
			else
				return Socket.RemoteEndPoint != null ? Socket.RemoteEndPoint.ToString() : "Not Connected!";
		}

		#endregion
	}
}
