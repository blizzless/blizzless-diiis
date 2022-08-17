//Blizzless Project 2022
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.AchievementSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.GamesSystem;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using WatsonTcp;

namespace DiIiS_NA.LoginServer.Battle
{
	public class BattleBackend
	{
		private static readonly Logger Logger = LogManager.CreateLogger("BattleNetEmu");

		public WatsonTcpServer GameServerSocket;

		public struct ServerDescriptor
		{
			public string GameIP;
			public int GamePort;
		};

		public Dictionary<string, ServerDescriptor> GameServers = new Dictionary<string, ServerDescriptor>();

		public Dictionary<string, ServerDescriptor> PvPGameServers = new Dictionary<string, ServerDescriptor>();

		public BattleBackend(string BattletHost, int BackPort)
		{
			this.GameServerSocket = new WatsonTcpServer(BattletHost, BackPort, this._receiverClientConnected, this._receiverClientDisconnected, this._receiverMessageReceived, false);
			System.Threading.Thread.Sleep(3000);
		}

		private bool _receiverClientConnected(string ipPort)
		{
			Logger.Info("Game server loaded {0} connecting to BlizzLess.Net...", ipPort);
			return true;
		}

		private bool _receiverClientDisconnected(string ipPort)
		{
			Logger.Warn("GameServer at {0} was disconnected!", ipPort);
			if (this.GameServers.ContainsKey(ipPort)) this.GameServers.Remove(ipPort);
			if (this.PvPGameServers.ContainsKey(ipPort)) this.PvPGameServers.Remove(ipPort);

			if (this.GameServers.Count == 0)
				Logger.Warn("GameServers list is empty! Unable to use PvE game activities atm.");
			if (this.PvPGameServers.Count == 0)
				Logger.Warn("PvPGameServers list is empty! Unable to use PvP game activities atm.");
			return true;
		}

		public void CreateGame(string ipPort, int GameId, int level, int act, int difficulty, int questId, int questStepId, bool isHardcore, int gamemode, bool iSseasoned, int perftest_id = 0)
		{
			GameServerSocket.Send(ipPort, Encoding.UTF8.GetBytes(string.Format("diiiscg|{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}", GameId, level, act, difficulty, questId, questStepId, isHardcore, gamemode, iSseasoned, perftest_id)));
		}

		private bool _receiverMessageReceived(string ipPort, byte[] data)
		{
			string msg = "";
			if (data != null && data.Length > 0) msg = Encoding.UTF8.GetString(data);
			Logger.Trace("Message received from {0}: {1}", ipPort, msg);

			var message = msg.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			var args = message[1].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			switch (message[0])
			{
				case "rngsr":
					if (this.GameServers.ContainsKey(ipPort)) this.GameServers.Remove(ipPort);
					string rgs_ip = args[0];
					int rgs_port = int.Parse(args[1].Trim());
					this.GameServers.Add(ipPort, new ServerDescriptor { GameIP = rgs_ip, GamePort = rgs_port });
					Logger.Info("Game server was registred for BlizzLess.Net {0}:{1}.", rgs_ip, rgs_port);
					break;
				case "rnpvpgsr":
					if (this.PvPGameServers.ContainsKey(ipPort)) this.PvPGameServers.Remove(ipPort);
					string rpgs_ip = args[0];
					int rpgs_port = int.Parse(args[1].Trim());
					this.PvPGameServers.Add(ipPort, new ServerDescriptor { GameIP = rpgs_ip, GamePort = rpgs_port });
					Logger.Info("PvP GameServer at {0}:{1} successfully signed and ready to work.", rpgs_ip, rpgs_port);
					break;
				case "grachi":
					ulong gachi_accId = ulong.Parse(args[0].Trim());
					ulong gachi_achId = ulong.Parse(args[1].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var gachi_invokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == gachi_accId))
							AchievementManager.GrantAchievement(gachi_invokerClient, gachi_achId);
					});
					break;
				case "gcrit":
					ulong gc_accId = ulong.Parse(args[0].Trim());
					ulong gc_criId = ulong.Parse(args[1].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var gc_invokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == gc_accId))
							AchievementManager.GrantCriteria(gc_invokerClient, gc_criId);
					});
					break;
				case "upequt":
					ulong uq_accId = ulong.Parse(args[0].Trim());
					ulong uq_achId = ulong.Parse(args[1].Trim());
					uint uq_addCounter = uint.Parse(args[2].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var uq_invokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == uq_accId))
							AchievementManager.UpdateQuantity(uq_invokerClient, uq_achId, uq_addCounter);
					});
					break;
				case "uoacce":
					ulong uac_accId = ulong.Parse(args[0].Trim());
					int uac_typeId = int.Parse(args[1].Trim());
					uint uac_addCounter = uint.Parse(args[2].Trim());
					int uac_comparand = int.Parse(args[3].Trim());
					ulong uac_achi = ulong.Parse(args[4].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						if (uac_achi == 0)
							foreach (var uac_invokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == uac_accId))
								AchievementManager.UpdateAllCounters(uac_invokerClient, uac_typeId, uac_addCounter, uac_comparand);
						else
							foreach (var uac_invokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == uac_accId))
								AchievementManager.UpdateAllCounters(uac_invokerClient, uac_typeId, uac_addCounter, uac_comparand);
					});
					break;
				case "upsnaccr": //UpdateSingleAchievementCounter
					ulong usac_accId = ulong.Parse(args[0].Trim());
					ulong usac_achId = ulong.Parse(args[1].Trim());
					uint usac_addCounter = uint.Parse(args[2].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var usac_invokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == usac_accId))
							AchievementManager.UpdateQuantity(usac_invokerClient, usac_achId, usac_addCounter);
					});
					break;
				case "cqc": //CheckQuestCriteria
					ulong cqc_accId = ulong.Parse(args[0].Trim());
					int cqc_qId = int.Parse(args[1].Trim());
					bool cqc_isCoop = (args[2].Trim() == "True" ? true : false);
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var cqc_invokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == cqc_accId))
							AchievementManager.CheckQuestCriteria(cqc_invokerClient, cqc_qId, cqc_isCoop);
					});
					break;
				case "ckmc": //CheckKillMonsterCriteria
					ulong ckmc_accId = ulong.Parse(args[0].Trim());
					int ckmc_actorId = int.Parse(args[1].Trim());
					int ckmc_type = int.Parse(args[2].Trim());
					bool ckmc_isHardcore = (args[3].Trim() == "True" ? true : false);
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var ckmc_invokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == ckmc_accId))
							AchievementManager.CheckKillMonsterCriteria(ckmc_invokerClient, ckmc_actorId, ckmc_type, ckmc_isHardcore);
					});
					break;
				case "clc": //CheckLevelCap
					ulong clc_accId = ulong.Parse(args[0].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var clc_invokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == clc_accId))
							AchievementManager.CheckLevelCap(clc_invokerClient);
					});
					break;
				case "csic": //CheckSalvageItemCriteria
					ulong csic_accId = ulong.Parse(args[0].Trim());
					int csic_itemId = int.Parse(args[1].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var csic_invokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == csic_accId))
							AchievementManager.CheckSalvageItemCriteria(csic_invokerClient, csic_itemId);
					});
					break;
				case "clac": //CheckLevelAreaCriteria
					ulong clac_accId = ulong.Parse(args[0].Trim());
					int clac_laId = int.Parse(args[1].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var clac_invokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == clac_accId))
							AchievementManager.CheckLevelAreaCriteria(clac_invokerClient, clac_laId);
					});
					break;
				case "ccc": //CheckConversationCriteria
					ulong ccc_accId = ulong.Parse(args[0].Trim());
					int ccc_cId = int.Parse(args[1].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var ccc_invokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == ccc_accId))
							AchievementManager.CheckConversationCriteria(ccc_invokerClient, ccc_cId);
					});
					
					break;
				case "plu": //ParagonLevelUp
					ulong plu_accId = ulong.Parse(args[0].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						var plr_client = PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == plu_accId).FirstOrDefault();
						if (plr_client != null && plr_client.Account.GameAccount. Clan != null)
							plr_client.Account.GameAccount.Clan.ParagonRatings[plr_client.Account.GameAccount] = plr_client.Account.GameAccount.DBGameAccount.ParagonLevel;
					});
					break;
				case "uii": //UniqueItemIdentified
					ulong uii_accId = ulong.Parse(args[0].Trim());
					ulong uii_itemId = ulong.Parse(args[1].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						var plr_client = PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == uii_accId).FirstOrDefault();
						if (plr_client != null && plr_client.Account.GameAccount.Clan != null)
						{
							var dbItem = DBSessions.SessionGet<DBInventory>(uii_itemId);
							if (dbItem != null)
							{
								var generator = D3.Items.Generator.CreateBuilder()
									.SetGbHandle(D3.GameBalance.Handle.CreateBuilder().SetGbid(dbItem.GbId).SetGameBalanceType(2))
									.SetStackSize(1)
									.SetDyeType((uint)dbItem.DyeType)
									.SetItemQualityLevel(dbItem.Quality)
									.SetFlags((uint)((dbItem.Binding > 0) ? 0 : ((dbItem.Version == 1) ? 2147483647 : 10633))) //0x1 - explored
									.SetSeed((uint)ItemGenerator.GetSeed(dbItem.Attributes))
									.SetDurability(509);

								List<string> affixes = dbItem.Affixes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
								foreach (string affix in affixes)
								{
									int result = 0;
									Int32.TryParse(affix, out result);
									generator.AddBaseAffixes(result);
								}

								plr_client.Account.GameAccount.Clan.AddNews(plr_client.Account.GameAccount, 0, generator.Build().ToByteArray());
							}
						}
					});
					break;
				case "uc": //UpdateClient
					ulong uc_accId = ulong.Parse(args[0].Trim());
					int uc_level = int.Parse(args[1].Trim());
					int uc_screen = int.Parse(args[2].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						try
						{
							foreach (var uc_invokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == uc_accId))
							{
								if (!uc_invokerClient.Account.IsOnline) continue;
								uc_invokerClient.Account.GameAccount.ChangedFields.SetPresenceFieldValue(uc_invokerClient.Account.GameAccount.CurrentToon.HeroLevelField);
								uc_invokerClient.Account.GameAccount.ChangedFields.SetPresenceFieldValue(uc_invokerClient.Account.GameAccount.CurrentToon.HeroParagonLevelField);
								if (uc_screen != -1) uc_invokerClient.Account.GameAccount.ScreenStatus = D3.PartyMessage.ScreenStatus.CreateBuilder().SetScreen(uc_screen).SetStatus(0).Build();
								uc_invokerClient.Account.GameAccount.NotifyUpdate();
							}
						}
						catch { }
					});
					break;
				case "gpj": //PlayerJoined
					int gpj_gameId = int.Parse(args[0].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						try
						{
							GameFactoryManager.FindGameByDynamicId((ulong)gpj_gameId).PlayersCount++;
						}
						catch { }
					});
					break;
				case "gpl": //PlayerLeft
					int gpl_gameId = int.Parse(args[0].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						try
						{
							if (GameFactoryManager.FindGameByDynamicId((ulong)gpl_gameId) != null)
								GameFactoryManager.FindGameByDynamicId((ulong)gpl_gameId).PlayersCount--;
							
						}
						catch { }
					});
					break;
				case "gsp": //SetGamePublic
					int gsp_gameId = int.Parse(args[0].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						try
						{
							GameFactoryManager.FindGameByDynamicId((ulong)gsp_gameId).Public = true;
						}
						catch { }
					});
					break;
				case "tsc": //ToonStateChanged
					int tsc_toonId = int.Parse(args[0].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						try
						{
							LoginServer.Toons.ToonManager.GetToonByLowID((ulong)tsc_toonId).StateChanged();
						}
						catch { }
					});
					break;
				case "pvpsp":   //PvPSaveProgress
					ulong pvpsp_gAccId = ulong.Parse(args[0].Trim());
					int pvpsp_kills = int.Parse(args[1].Trim());
					int pvpsp_wins = int.Parse(args[2].Trim());
					int pvpsp_gold = int.Parse(args[3].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						var gAcc = GameAccountManager.GetAccountByPersistentID(pvpsp_gAccId);

						lock (gAcc.DBGameAccount)
						{
							var dbGAcc = gAcc.DBGameAccount;
							dbGAcc.PvPTotalKilled += (ulong)pvpsp_kills;
							dbGAcc.PvPTotalWins += (ulong)pvpsp_wins;
							dbGAcc.PvPTotalGold += (ulong)pvpsp_gold;
							DBSessions.SessionUpdate(dbGAcc);
						}
					});
					break;
				default:
					Logger.Warn("Unknown message type: {0}|{1}", message[0], message[1]);
					break;
			}
			return true;
		}
	}
}
