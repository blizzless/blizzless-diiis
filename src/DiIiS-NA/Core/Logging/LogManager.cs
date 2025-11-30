using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DiIiS_NA.Core.Logging
{
	public static class LogManager
	{
		/// <summary>
		/// Is logging enabled?
		/// </summary>
		public static bool Enabled { get; set; }

		/// <summary>
		/// Available & configured log targets.
		/// </summary>
		internal static readonly List<LogTarget> Targets = new();

		/// <summary>
		/// Available loggers.
		/// </summary>
		internal readonly static Dictionary<string, Logger> Loggers = new();
		
		/// <summary>
		/// Creates and returns a logger with given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns>A <see cref="Logger"/> instance.</returns>
		public static Logger CreateLogger(string? name = null, [CallerFilePath] string filePath = "")
		{
			if (name == null)
			{
				var frame = new StackFrame(1, false); // read stack frame.
				name = frame.GetMethod()?.DeclaringType?.Name ?? "Unknown"; // get declaring type's name.
			}

			if (!Loggers.ContainsKey(name)) // see if we already have instance for the given name.
				Loggers.Add(name, new Logger(name, filePath)); // add it to dictionary of loggers.

			return Loggers[name]; // return the newly created logger.
		}

        public static Logger CreateLogger<T>([CallerFilePath] string filePath = "")
        {
            return CreateLogger(typeof(T).Name, filePath);
        }

		/// <summary>
		/// Attachs a new log target.
		/// </summary>
		/// <param name="target"></param>
		public static void AttachLogTarget(LogTarget target)
		{
			Targets.Add(target);
		}
	}
}
