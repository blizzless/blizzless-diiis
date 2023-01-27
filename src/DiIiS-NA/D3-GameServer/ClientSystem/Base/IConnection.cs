using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.ClientSystem.Base
{
	public interface IConnection
	{
		// Wrapper for the Send method that will send the data either to the
		// Socket (unecnrypted) or to the TLSStream (encrypted).
		// Note that the flags will be ignored for TLSStream.
		int _Send(byte[] buffer, int start, int count, SocketFlags flags);

		// Read bytes from the Sokcet into the buffer in a non-blocking call.
		// This allows us to read no more than the specified count number of bytes.
		int Receive(int start, int count);

		// Expose the RecvBuffer.
		byte[] RecvBuffer { get; }

		/// <summary>
		/// Returns remote endpoint.
		/// </summary>
		IPEndPoint RemoteEndPoint { get; }

		/// <summary>
		/// Returns local endpoint.
		/// </summary>
		IPEndPoint LocalEndPoint { get; }

		/// <summary>
		/// Gets or sets bound client.
		/// </summary>
		IClient Client { get; set; }

		/// <summary>
		/// Gets underlying socket.
		/// </summary>
		Socket Socket { get; }

		int Send(byte[] buffer);

		int Send(byte[] buffer, SocketFlags flags);

		int Send(byte[] buffer, int start, int count);

		int Send(byte[] buffer, int start, int count, SocketFlags flags);

		int Send(IEnumerable<byte> data);

		int Send(IEnumerable<byte> data, SocketFlags flags);

		/// <summary>
		/// Kills the connection to remote endpoint.
		/// </summary>
		void Disconnect();

		bool IsOpen();
	}
}
