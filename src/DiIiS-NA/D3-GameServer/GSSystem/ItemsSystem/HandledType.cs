using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ItemsSystem
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class HandledTypeAttribute : Attribute
    {
        public List<string> Types { get; private set; }

        public HandledTypeAttribute(params string[] types)
        {
            Types = new List<string>();
            Types.AddRange(types);
        }
    }
}
