//Blizzless Project 2022

using System;
using DiIiS_NA.Core.Logging;
using Nini.Config;

namespace DiIiS_NA.Core.Config
{
	public class Config
	{
		private readonly Logger _logger;
		private readonly string _sectionName;
		private readonly IConfig _section;

		protected Config(string sectionName)
		{
			_sectionName = sectionName;
			_logger = LogManager.CreateLogger($"{GetType().Name}:{sectionName}");
			_section = ConfigurationManager.Section(sectionName) ?? ConfigurationManager.AddSection(sectionName);
		}

		public void Save()
		{
			ConfigurationManager.Save();
		}

		protected bool GetBoolean(string key, bool defaultValue) => _section.GetBoolean(key, defaultValue);
		protected double GetDouble(string key, double defaultValue) => _section.GetDouble(key, defaultValue);
		protected float GetFloat(string key, float defaultValue) => _section.GetFloat(key, defaultValue);
		protected int GetInt(string key, int defaultValue) => _section.GetInt(key, defaultValue);
		protected int GetInt(string key, int defaultValue, bool fromAlias) => _section.GetInt(key, defaultValue, fromAlias);
		protected long GetLong(string key, long defaultValue) => _section.GetLong(key, defaultValue);
		protected string GetString(string key, string defaultValue) { return _section.Get(key, defaultValue); }
		protected string[] GetEntryKeys() { return _section.GetKeys(); }
		protected void Set(string key, object value) { _section.Set(key, value); }
	}
}
