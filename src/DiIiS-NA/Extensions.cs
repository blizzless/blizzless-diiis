using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DiIiS_NA
{
    internal static class Globals
    {
        public const float FLOAT_TOLERANCE = 0.0001f;
    }

    internal class Mapping
    {
        private readonly Dictionary<string, object> _map = new();
        public Mapping Map(string map, object value)
        {
            _map.Add(map, value.ToString());
            return this;
        }

        public static Mapping From(string map, object value)
        {
            return (new Mapping()).Map(map, value);
        }

        public string GetString(string template)
        {
            return _map.Aggregate(template, (current, map) => current.Replace("{" + map.Key + "}", map.Value.ToString()));
        }
    }
}