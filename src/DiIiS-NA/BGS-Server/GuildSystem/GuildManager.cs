//Blizzless Project 2022
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.LoginServer.AccountsSystem;
using System.Collections.Generic;
using System.Linq;

namespace DiIiS_NA.LoginServer.GuildSystem
{
	public static class GuildManager
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public readonly static Dictionary<ulong, Guild> Guilds =
			new Dictionary<ulong, Guild>();

		public static void PreLoadGuilds()
		{
			//Logger.Info("Loading Diablo III guilds system...");
			List<DBGuild> all_guilds = DBSessions.SessionQuery<DBGuild>();
			List<DBGuildMember> all_guild_members = DBSessions.SessionQuery<DBGuildMember>();
			List<DBGuildNews> all_guild_news = DBSessions.SessionQuery<DBGuildNews>();
			foreach (var dbGuild in all_guilds)
			{
				var guild = new Guild(dbGuild);
				Guilds.Add(guild.PersistentId, guild);
				foreach (var dbGuildMember in all_guild_members)
					if (dbGuildMember.DBGuild.Id == guild.PersistentId)
					{
						var gacc = GameAccountManager.GetGameAccountByDBGameAccount(dbGuildMember.DBGameAccount);
						if (guild.IsClan)
							guild.ParagonRatings.Add(gacc, gacc.DBGameAccount.ParagonLevel);
						guild.Members.Add(gacc, dbGuildMember);
					}
				foreach (var dbGuildNews in all_guild_news)
					if (dbGuildNews.DBGuild.Id == guild.PersistentId)
					{
						if (dbGuildNews.Time > guild.NewsTime)
							guild.NewsTime = dbGuildNews.Time;
					}
			}
		}

		public static Guild CreateNewGuild(GameAccount owner, string name, string tag, bool isClan, uint category, bool isLFM, uint language)
		{
			if (DBSessions.SessionQueryWhere<DBGuild>(g => g.Name == name || (g.Tag != "" && g.Tag == tag)).Count > 0) return null;
			var newDBGuild = new DBGuild
			{
				Name = name,
				Tag = tag,
				Description = "",
				MOTD = "",
				Category = (int)category,
				Language = (int)language,
				IsLFM = isLFM,
				IsInviteRequired = isClan,
				Rating = 0,
				Creator = owner.DBGameAccount,
				Ranks = D3.Guild.RankList.CreateBuilder()
					.AddRanks(D3.Guild.Rank.CreateBuilder().SetRankId(1).SetRankOrder(1).SetName("Leader").SetPermissions(4294967295))
					.AddRanks(D3.Guild.Rank.CreateBuilder().SetRankId(2).SetRankOrder(2).SetName("Officer").SetPermissions(212943))
					.AddRanks(D3.Guild.Rank.CreateBuilder().SetRankId(4).SetRankOrder(4).SetName("Member").SetPermissions(4163))
					.Build().ToByteString().ToByteArray(),
				Disbanded = false
			};
			DBSessions.SessionSave(newDBGuild);

			var guild = new Guild(newDBGuild);
			Guilds.Add(guild.PersistentId, guild);
			guild.AddMember(owner);
			return guild;
		}

		public static Guild GetGuildById(ulong id)
		{
			return Guilds.ContainsKey(id) ? Guilds[id] : null;
		}

		public static Guild GetGuildByName(string name)
		{
			return Guilds.Values.Where(g => g.Name == name).FirstOrDefault();
		}

		public static List<Guild> GetClans()
		{
			return Guilds.Values.Where(g => g.IsClan).ToList();
		}

		public static List<Guild> GetCommunities()
		{
			return Guilds.Values.Where(g => !g.IsClan).ToList();
		}

		public static void ReplicateGuilds(GameAccount account)
		{
			if (account.Clan != null)
			{
				account.Clan.NotifyChannels(account);
			}

			foreach (var guild in account.Communities)
				guild.NotifyChannels(account);
		}
	}
}
