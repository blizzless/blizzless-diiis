using System;
using System.Collections.Generic;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ObjectsSystem
{
    public enum FixedAttribute
    {
        Invulnerable,
        Speed,
        Powerful,
        Resourceful,
        AttackSpeed,
        Dev,
        Betrayal
    }

    public class FixedMap
    {
        private static readonly Logger _logger = LogManager.CreateLogger(nameof(FixedMap));
        private readonly Dictionary<FixedAttribute, Action<GameAttributeMap>> _attributeMap = new();
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly Dictionary<FixedAttribute, Action<GameAttributeMap>> _removedAttributeMap = new();

        public void Add(FixedAttribute name, Action<GameAttributeMap> action,
            Action<GameAttributeMap> removedAction = null)
        {
            _attributeMap.Add(name, action);
            if (removedAction != null)
            {
                _removedAttributeMap.Add(name, removedAction);
            }

            if (Contains(name))
            {
                _attributeMap[name] = action;
                _logger.Warn($"Overwrite attribute {name}");
            }
            else
                _attributeMap.Add(name, action);
        }

        public void Remove(FixedAttribute name)
        {
            try 
            { 
                _attributeMap.Remove(name);
                _removedAttributeMap.Remove(name);
            }
            catch (Exception ex)
            {
                _logger.WarnException(ex, $"Cannot remove {name} fixed attribute.");
            }
        }

        public void Clear() => _attributeMap.Clear();
        public bool Contains(FixedAttribute name) => _attributeMap.ContainsKey(name);
        public void Apply(GameAttributeMap map)
        {
            foreach (var action in _attributeMap.Values)
                action(map);
        }
    }
}