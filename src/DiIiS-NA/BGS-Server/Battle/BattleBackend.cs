using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.GameServer.AchievementSystem;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.GamesSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatsonTcp;

namespace DiIiS_NA.LoginServer.Battle
{
	public class BattleBackend
	{
		private static readonly Logger Logger = LogManager.CreateLogger("BattleBackend");

		public readonly WatsonTcpServer GameServerSocket;

		public struct ServerDescriptor
		{
			public string GameIp;
			public int GamePort;
		};

		public readonly Dictionary<string, ServerDescriptor> GameServers = new();

		private readonly Dictionary<string, ServerDescriptor> _pvPGameServers = new();

		public BattleBackend(string battleHost, int port)
		{
			GameServerSocket = new WatsonTcpServer(battleHost, port, ReceiverClientConnected, ReceiverClientDisconnected, ReceiverMessageReceived, false);
			System.Threading.Thread.Sleep(3000);
		}

		private bool ReceiverClientConnected(string ipPort)
		{
			Logger.Info($"Blizzless client connected {ipPort}...");
			return true;
		}

		private bool ReceiverClientDisconnected(string ipPort)
		{
			Logger.Warn("Blizzless client disconnected $[white]${0}$[/]$!", ipPort);
			if (GameServers.ContainsKey(ipPort)) GameServers.Remove(ipPort);
			if (_pvPGameServers.ContainsKey(ipPort)) _pvPGameServers.Remove(ipPort);

			if (GameServers.Count == 0)
				Logger.Warn("GameServers list is empty! Unable to use PvE game activities atm.");
			if (_pvPGameServers.Count == 0)
				Logger.Warn("PvPGameServers list is empty! Unable to use PvP game activities atm.");
			return true;
		}

		public void CreateGame(string ipPort, int gameId, int level, int act, int difficulty, int questId, int questStepId, bool isHardcore, int gameMode, bool isSeasoned, int perftestId = 0)
		{
			Send(ipPort, $"diiiscg|{gameId}/{level}/{act}/{difficulty}/{questId}/{questStepId}/{isHardcore}/{gameMode}/{isSeasoned}");
		}
		
		private void Send(string ipPort, string data)
			=> GameServerSocket.Send(ipPort, Encoding.UTF8.GetBytes(data));

		private bool ReceiverMessageReceived(string ipPort, byte[] data)
		{
			string msg = "";
			if (data != null && data.Length > 0) msg = Encoding.UTF8.GetString(data);
			Logger.Trace("Message received from $[grey69]${0}$[/]$: $[white]${1}$[/]$", ipPort, msg);

			var message = msg.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			var args = message[1].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			switch (message[0])
			{
				case "rngsr":
					if (GameServers.ContainsKey(ipPort)) GameServers.Remove(ipPort);
					string rgsIp = args[0];
					int rgsPort = int.Parse(args[1].Trim());
					GameServers.Add(ipPort, new ServerDescriptor { GameIp = rgsIp, GamePort = rgsPort });
					Logger.Info("Game server was registered for Blizzless {0}:{1}.", rgsIp, rgsPort);
					break;
				case "rnpvpgsr":
					if (_pvPGameServers.ContainsKey(ipPort)) _pvPGameServers.Remove(ipPort);
					string rpgsIp = args[0];
					int rpgsPort = int.Parse(args[1].Trim());
					_pvPGameServers.Add(ipPort, new ServerDescriptor { GameIp = rpgsIp, GamePort = rpgsPort });
					Logger.Info("PvP GameServer at {0}:{1} successfully signed and ready to work.", rpgsIp, rpgsPort);
					break;
				case "grachi":
					ulong gachiAccId = ulong.Parse(args[0].Trim());
					ulong gachiAchId = ulong.Parse(args[1].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var gachiInvokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == gachiAccId))
							AchievementManager.GrantAchievement(gachiInvokerClient, gachiAchId);
					});
					break;
				case "gcrit":
					ulong gcAccId = ulong.Parse(args[0].Trim());
					ulong gcCriId = ulong.Parse(args[1].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var gcInvokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == gcAccId))
							AchievementManager.GrantCriteria(gcInvokerClient, gcCriId);
					});
					break;
				case "upequt":
					ulong uqAccId = ulong.Parse(args[0].Trim());
					ulong uqAchId = ulong.Parse(args[1].Trim());
					uint uqAddCounter = uint.Parse(args[2].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var uqInvokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == uqAccId))
							AchievementManager.UpdateQuantity(uqInvokerClient, uqAchId, uqAddCounter);
					});
					break;
				case "uoacce":
					ulong uacAccId = ulong.Parse(args[0].Trim());
					int uacTypeId = int.Parse(args[1].Trim());
					uint uacAddCounter = uint.Parse(args[2].Trim());
					int uacComparand = int.Parse(args[3].Trim());
					ulong uacAchi = ulong.Parse(args[4].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						if (uacAchi == 0)
							foreach (var uacInvokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == uacAccId))
								AchievementManager.UpdateAllCounters(uacInvokerClient, uacTypeId, uacAddCounter, uacComparand);
						else
							foreach (var uacInvokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == uacAccId))
								AchievementManager.UpdateAllCounters(uacInvokerClient, uacTypeId, uacAddCounter, uacComparand);
					});
					break;
				case "upsnaccr": //UpdateSingleAchievementCounter
					ulong usacAccId = ulong.Parse(args[0].Trim());
					ulong usacAchId = ulong.Parse(args[1].Trim());
					uint usacAddCounter = uint.Parse(args[2].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var usacInvokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == usacAccId))
							AchievementManager.UpdateQuantity(usacInvokerClient, usacAchId, usacAddCounter);
					});
					break;
				case "cqc": //CheckQuestCriteria
					ulong cqcAccId = ulong.Parse(args[0].Trim());
					int cqcQId = int.Parse(args[1].Trim());
					bool cqcIsCoop = (args[2].Trim() == "True" ? true : false);
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var cqcInvokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == cqcAccId))
							AchievementManager.CheckQuestCriteria(cqcInvokerClient, cqcQId, cqcIsCoop);
					});
					break;
				case "ckmc": //CheckKillMonsterCriteria
					ulong ckmcAccId = ulong.Parse(args[0].Trim());
					int ckmcActorId = int.Parse(args[1].Trim());
					int ckmcType = int.Parse(args[2].Trim());
					bool ckmcIsHardcore = (args[3].Trim() == "True" ? true : false);
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var ckmcInvokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == ckmcAccId))
							AchievementManager.CheckKillMonsterCriteria(ckmcInvokerClient, ckmcActorId, ckmcType, ckmcIsHardcore);
					});
					break;
				case "clc": //CheckLevelCap
					ulong clcAccId = ulong.Parse(args[0].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var clcInvokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == clcAccId))
							AchievementManager.CheckLevelCap(clcInvokerClient);
					});
					break;
				case "csic": //CheckSalvageItemCriteria
					ulong csicAccId = ulong.Parse(args[0].Trim());
					int csicItemId = int.Parse(args[1].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var csicInvokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == csicAccId))
							AchievementManager.CheckSalvageItemCriteria(csicInvokerClient, csicItemId);
					});
					break;
				case "clac": //CheckLevelAreaCriteria
					ulong clacAccId = ulong.Parse(args[0].Trim());
					int clacLaId = int.Parse(args[1].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var clacInvokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == clacAccId))
							AchievementManager.CheckLevelAreaCriteria(clacInvokerClient, clacLaId);
					});
					break;
				case "ccc": //CheckConversationCriteria
					ulong cccAccId = ulong.Parse(args[0].Trim());
					int cccCId = int.Parse(args[1].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						foreach (var cccInvokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == cccAccId))
							AchievementManager.CheckConversationCriteria(cccInvokerClient, cccCId);
					});
					
					break;
				case "plu": //ParagonLevelUp
					ulong pluAccId = ulong.Parse(args[0].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						var plrClient = PlayerManager.OnlinePlayers.FirstOrDefault(c => c.Account.GameAccount.PersistentID == pluAccId);
						if (plrClient != null && plrClient.Account.GameAccount. Clan != null)
							plrClient.Account.GameAccount.Clan.ParagonRatings[plrClient.Account.GameAccount] = plrClient.Account.GameAccount.DBGameAccount.ParagonLevel;
					});
					break;
				case "uii": //UniqueItemIdentified
					ulong uiiAccId = ulong.Parse(args[0].Trim());
					ulong uiiItemId = ulong.Parse(args[1].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						var plrClient = PlayerManager.OnlinePlayers.FirstOrDefault(c => c.Account.GameAccount.PersistentID == uiiAccId);
						if (plrClient != null && plrClient.Account.GameAccount.Clan != null)
						{
							var dbItem = DBSessions.SessionGet<DBInventory>(uiiItemId);
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

								plrClient.Account.GameAccount.Clan.AddNews(plrClient.Account.GameAccount, 0, generator.Build().ToByteArray());
							}
						}
					});
					break;
				case "uc": //UpdateClient
					ulong ucAccId = ulong.Parse(args[0].Trim());
					int ucLevel = int.Parse(args[1].Trim());
					int ucScreen = int.Parse(args[2].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						try
						{
							foreach (var ucInvokerClient in PlayerManager.OnlinePlayers.Where(c => c.Account.GameAccount.PersistentID == ucAccId))
							{
								if (!ucInvokerClient.Account.IsOnline) continue;
								ucInvokerClient.Account.GameAccount.ChangedFields.SetPresenceFieldValue(ucInvokerClient.Account.GameAccount.CurrentToon.HeroLevelField);
								ucInvokerClient.Account.GameAccount.ChangedFields.SetPresenceFieldValue(ucInvokerClient.Account.GameAccount.CurrentToon.HeroParagonLevelField);
								if (ucScreen != -1) ucInvokerClient.Account.GameAccount.ScreenStatus = D3.PartyMessage.ScreenStatus.CreateBuilder().SetScreen(ucScreen).SetStatus(0).Build();
								ucInvokerClient.Account.GameAccount.NotifyUpdate();
							}
						}
						catch { }
					});
					break;
				case "gpj": //PlayerJoined
					int gpjGameId = int.Parse(args[0].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						try
						{
							GameFactoryManager.FindGameByDynamicId((ulong)gpjGameId).PlayersCount++;
						}
						catch { }
					});
					break;
				case "gpl": //PlayerLeft
					int gplGameId = int.Parse(args[0].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						try
						{
							if (GameFactoryManager.FindGameByDynamicId((ulong)gplGameId) != null)
								GameFactoryManager.FindGameByDynamicId((ulong)gplGameId).PlayersCount--;
							
						}
						catch { }
					});
					break;
				case "gsp": //SetGamePublic
					int gspGameId = int.Parse(args[0].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						try
						{
							GameFactoryManager.FindGameByDynamicId((ulong)gspGameId).Public = true;
						}
						catch { }
					});
					break;
				case "tsc": //ToonStateChanged
					int tscToonId = int.Parse(args[0].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						try
						{
							Toons.ToonManager.GetToonByLowId((ulong)tscToonId).StateChanged();
						}
						catch { }
					});
					break;
				case "pvpsp":   //PvPSaveProgress
					ulong pvpspGAccId = ulong.Parse(args[0].Trim());
					int pvpspKills = int.Parse(args[1].Trim());
					int pvpspWins = int.Parse(args[2].Trim());
					int pvpspGold = int.Parse(args[3].Trim());
					System.Threading.Tasks.Task.Delay(1).ContinueWith((a) => {
						var gAcc = GameAccountManager.GetAccountByPersistentID(pvpspGAccId);

						lock (gAcc.DBGameAccount)
						{
							var dbGAcc = gAcc.DBGameAccount;
							dbGAcc.PvPTotalKilled += (ulong)pvpspKills;
							dbGAcc.PvPTotalWins += (ulong)pvpspWins;
							dbGAcc.PvPTotalGold += (ulong)pvpspGold;
							DBSessions.SessionUpdate(dbGAcc);
						}
					});
					break;
				default:
					Logger.Warn("Unknown message type: $[white]${0}|{1}$[/]$", message[0], message[1]);
					break;
			}
			return true;
		}
	}
}
