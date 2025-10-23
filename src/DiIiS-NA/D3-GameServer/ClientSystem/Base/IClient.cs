using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.ClientSystem.Base
{
    public interface IClient
    {
        /// <summary>
        /// Gets or sets the TCP connection bound to client.
        /// </summary>
        IConnection Connection { get; set; }
    }
}
