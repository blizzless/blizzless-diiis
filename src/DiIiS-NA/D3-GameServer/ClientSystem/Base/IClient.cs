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
    public interface IClient
    {
        /// <summary>
        /// Gets or sets the TCP connection bound to client.
        /// </summary>
        IConnection Connection { get; set; }
    }
}
