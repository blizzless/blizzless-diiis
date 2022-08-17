//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.ClientSystem.Base
{
	public sealed class ConnectionDataEventArgs : ConnectionEventArgs
	{
		public IEnumerable<byte> Data { get; private set; }

		public ConnectionDataEventArgs(IConnection connection, IEnumerable<byte> data)
			: base(connection)
		{
			this.Data = data ?? new byte[0];
		}

		public override string ToString()
		{
			return Connection.RemoteEndPoint != null
				? string.Format("{0}: {1} bytes", Connection.RemoteEndPoint, Data.Count())
				: string.Format("Not Connected: {0} bytes", Data.Count());
		}
	}
}
