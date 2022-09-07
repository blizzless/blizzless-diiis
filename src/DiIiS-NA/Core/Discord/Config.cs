/*
 * Copyright (C) 2011 - 2012 DiIiS_NA project - http://www.DiIiS_NA.org
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
  *
 */

namespace DiIiS_NA.Core.Discord
{
	public sealed class Config : Core.Config.Config
	{
		public bool Enabled { get { return this.GetBoolean("Enabled", false); } set { this.Set("Enabled", value); } }
		public bool MonitorEnabled { get { return this.GetBoolean("MonitorEnabled", true); } set { this.Set("MonitorEnabled", value); } }
		public string Token { get { return this.GetString("Token", ""); } set { this.Set("Token", value); } }
		public long GuildId { get { return this.GetLong("GuildId", 0); } set { this.Set("GuildId", value); } }
		public long AnnounceChannelId { get { return this.GetLong("AnnounceChannelId", 0); } set { this.Set("AnnounceChannelId", value); } }
		public long StatsChannelId { get { return this.GetLong("StatsChannelId", 0); } set { this.Set("StatsChannelId", value); } }
		public long EventsChannelId { get { return this.GetLong("EventsChannelId", 0); } set { this.Set("EventsChannelId", value); } }
		public long BaseRoleId { get { return this.GetLong("BaseRoleId", 0); } set { this.Set("BaseRoleId", value); } }
		public long PremiumRoleId { get { return this.GetLong("PremiumRoleId", 0); } set { this.Set("PremiumRoleId", value); } }
		public long CollectorRoleId { get { return this.GetLong("CollectorRoleId", 0); } set { this.Set("CollectorRoleId", value); } }

		private static readonly Config _instance = new Config();
		public static Config Instance { get { return _instance; } }
		private Config() : base("Discord") { }
	}
}
