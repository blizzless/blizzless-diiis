//Blizzless Project 2022 
using DiIiS_NA.REST.Manager;
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

namespace DiIiS_NA.REST.Global
{
    public static class Global
    {
        public static SessionManager SessionMgr { get { return SessionManager.Instance; } }
    }
}
