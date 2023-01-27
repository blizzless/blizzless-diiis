using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.REST.Manager
{
    public class Cypher
    {
        public static void Assert(bool value, string message = "", [CallerMemberName]string memberName = "")
        {
            if (!value)
            {
                 throw new Exception(memberName);
            }
        }
    }

}
