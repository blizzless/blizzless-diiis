//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Reflection;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public class PowerLoader
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		private static Dictionary<int, Type> _implementations = new Dictionary<int, Type>();

		public static PowerScript CreateImplementationForPowerSNO(int powerSNO)
		{
			if (_implementations.ContainsKey(powerSNO))
			{
				PowerScript script = (PowerScript)Activator.CreateInstance(_implementations[powerSNO]);
				script.PowerSNO = powerSNO;
				return script;
			}
			else
			{
				#if DEBUG
				if (powerSNO != 30021 && powerSNO != 30022 && powerSNO != -1)
					Logger.Info("Unimplemented power: {0}", powerSNO); //for hiding annoying messages
				#endif
				return null;
			}
		}

		public static bool HasImplementationForPowerSNO(int powerSNO)
		{
			return _implementations.ContainsKey(powerSNO);
		}

		static PowerLoader()
		{
			// Find all subclasses of PowerImplementation and index them by the PowerSNO they are attributed with.
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.IsSubclassOf(typeof(PowerScript)))
				{
					var attributes = (ImplementsPowerSNO[])type.GetCustomAttributes(typeof(ImplementsPowerSNO), true);
					foreach (var powerAttribute in attributes)
					{
						_implementations[powerAttribute.PowerSNO] = type;
					}
				}
			}
		}
	}
}
