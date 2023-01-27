using DiIiS_NA.REST.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.REST.Global
{
    public static class Global
    {
        public static SessionManager SessionMgr { get { return SessionManager.Instance; } }
    }
}
