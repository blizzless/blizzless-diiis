//Blizzless Project 2022
using System;
using System.Collections.Generic;
using DiIiS_NA.Core.Logging;

namespace DiIiS_NA.LoginServer.Objects
{
    public static class RPCObjectManager
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private static ulong _nextId = 10000;
        public static readonly Dictionary<ulong, RPCObject> Objects = new Dictionary<ulong, RPCObject>();

		static RPCObjectManager()
		{ }

		public static void Init(RPCObject obj)
		{
			if (Objects.ContainsKey(obj.DynamicId))
				throw new Exception("Given object was already initialized");
			ulong id = Next();
				obj.DynamicId = id;

			Objects.Add(id, obj);
		}

		public static void Release(RPCObject obj)
		{
			Logger.Trace("Releasing object {0}", obj.DynamicId);
			Objects.Remove(obj.DynamicId);
		}

		public static ulong Next()
		{
			while (Objects.ContainsKey(++_nextId)) ;
			return _nextId;
		}
	}
}
