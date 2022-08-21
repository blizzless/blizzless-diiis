//Blizzless Project 2022
//Blizzless Project 2022 
using System.Runtime.Serialization.Formatters.Binary;
//Blizzless Project 2022 
using System.IO;

namespace DiIiS_NA.Core.Extensions
{
	public static class DeepCopy
	{
		public static T DeepClone<T>(T obj)
		{
			using (var ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(ms, obj);
				ms.Position = 0;

				return (T)formatter.Deserialize(ms);
			}
		}
	}
}
