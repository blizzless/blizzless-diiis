using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
