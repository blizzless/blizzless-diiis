using System.Collections.Generic;
using System.Linq;

namespace DiIiS_NA;

public class Mapping
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