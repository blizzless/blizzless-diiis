//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Runtime.CompilerServices;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.REST.Manager
{
    public class Cypher
    {
        public static void Assert(bool value, string message = "", [CallerMemberName]string memberName = "")
        {
            if (!value)
            {
               // if (!message.IsEmpty())
                //    Log.outFatal(LogFilter.Server, message);

                throw new Exception(memberName);
            }
        }
    }

}
