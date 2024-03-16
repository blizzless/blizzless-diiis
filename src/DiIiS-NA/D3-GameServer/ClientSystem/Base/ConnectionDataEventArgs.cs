using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.ClientSystem.Base
{
	public sealed class ConnectionDataEventArgs : ConnectionEventArgs
	{
		public IEnumerable<byte> Data { get; private set; }

		public ConnectionDataEventArgs(IConnection connection, IEnumerable<byte> data)
			: base(connection)
		{
			Data = data ?? Array.Empty<byte>();
		}

		public override string ToString()
		{
			return Connection.RemoteEndPoint != null
				? $"{Connection.RemoteEndPoint}: {Data.Count()} bytes"
				: $"Not Connected: {Data.Count()} bytes";
		}
	}
}
