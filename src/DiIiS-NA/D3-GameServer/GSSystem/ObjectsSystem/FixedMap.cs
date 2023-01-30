using System;
using System.Collections.Generic;
using DiIiS_NA.Core.Logging;

namespace DiIiS_NA.GameServer.GSSystem.ObjectsSystem
{
    public enum FixedAttribute
    {
        Invulnerable,
        Speed,
        Powerful,
        Resourceful,
        AttackSpeed
    }
    
    public class FixedMap
    {
        private static readonly Logger _logger = LogManager.CreateLogger(nameof(FixedMap));
        private readonly Dictionary<FixedAttribute, Action<GameAttributeMap>> _attributeMap = new();

        public void Add(FixedAttribute name, Action<GameAttributeMap> action)
        {
            if (Contains(name))
            {
                _attributeMap[name] += action;
                _logger.Warn($"Fixed attribute {name} already exists. Action will be added.");
                return;
            }
            _attributeMap.Add(name, action);
        }

        public void Remove(FixedAttribute name) => _attributeMap.Remove(name);
        public void Clear() => _attributeMap.Clear();
        public bool Contains(FixedAttribute name) => _attributeMap.ContainsKey(name);
        public void Apply(GameAttributeMap map)
        {
            foreach (var action in _attributeMap.Values)
                action(map);
        }
    }
}