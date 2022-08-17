//Blizzless Project 2022
//Blizzless Project 2022 
using System.Runtime.Serialization;

namespace DiIiS_NA.LoginServer.Objects
{
	public class PersistentRPCObject : RPCObject
	{
		
		[DataMemberAttribute]
		public ulong PersistentID { get; private set; }
		protected PersistentRPCObject(ulong persistentId)
		{
			this.PersistentID = persistentId;
		}
	}
}
