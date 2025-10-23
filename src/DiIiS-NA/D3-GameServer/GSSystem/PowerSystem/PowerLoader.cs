using DiIiS_NA.Core.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public class PowerLoader
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		private static readonly Dictionary<int, Type> _implementations = new();

		private static PowerScript TryActivate(int powerSno)
		{
			try
			{
				return (PowerScript)Activator.CreateInstance(_implementations[powerSno]);
			}
			catch(Exception ex)
			{
				Logger.FatalException(ex, $"Failed to activate power {powerSno}");
				return null;
			}
		}
		public static PowerScript CreateImplementationForPowerSNO(int powerSNO)
		{
			if (_implementations.ContainsKey(powerSNO))
			{
				PowerScript script = TryActivate(powerSNO);
				if (script != null)
				{
					script.PowerSNO = powerSNO;
					return script;
				}
			}
			#if DEBUG
			if (powerSNO != 30021 && powerSNO != 30022 && powerSNO != -1) //for hiding annoying messages
				Logger.Info($"$[underline red]$Unimplemented power:$[/]$ $[underline]${powerSNO}$[/]$");
			#else
			if (powerSNO != 30021 && powerSNO != 30022 && powerSNO != -1) //for hiding annoying messages
				Logger.Info($"$[underline red]$Unimplemented power:$[/]$ $[underline]${powerSNO}$[/]$");
			#endif
			return null;
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
