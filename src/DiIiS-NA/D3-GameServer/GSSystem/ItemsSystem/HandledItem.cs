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
    public sealed class HandledItemAttribute : Attribute
    {
        public List<string> Names { get; private set; }

        public HandledItemAttribute(params string[] names)
        {
            Names = new List<string>();
            Names.AddRange(names);
        }
    }
}
