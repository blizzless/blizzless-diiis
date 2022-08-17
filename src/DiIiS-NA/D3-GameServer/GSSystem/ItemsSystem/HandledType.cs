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

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class HandledTypeAttribute : Attribute
    {
        public List<string> Types { get; private set; }

        public HandledTypeAttribute(params string[] types)
        {
            this.Types = new List<string>();
            this.Types.AddRange(types);
        }
    }
}
