using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.ClientSystem.Base
{
    public class ConnectionEventArgs : EventArgs
    {
        public IConnection Connection { get; private set; }

        public ConnectionEventArgs(IConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            Connection = connection;
        }

        public override string ToString()
        {
            return Connection.RemoteEndPoint != null
                ? Connection.RemoteEndPoint.ToString()
                : "Not Connected";
        }
    }
}
