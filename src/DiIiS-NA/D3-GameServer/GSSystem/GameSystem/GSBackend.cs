using DiIiS_NA.Core.Extensions;
using DiIiS_NA.Core.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WatsonTcp;

namespace DiIiS_NA.GameServer.GSSystem.GameSystem
{
	public class GsBackend
	{
		private static readonly Logger Logger = LogManager.CreateLogger("GSBackend");

		private static Object _globalSocketLock = new object();

		public WatsonTcpClient BattleNetSocket;

		public GsBackend(string battleHost, int battlePort)
		{
			BattleNetSocket = new WatsonTcpClient(battleHost, battlePort, SenderServerConnected, SenderServerDisconnected, SenderMessageReceived, false);
		}


		private bool SenderMessageReceived(byte[] data)
		{
			string msg = "";
			if (data != null && data.Length > 0) msg = Encoding.UTF8.GetString(data);
			Logger.Debug("Message from Battle.net: {0}", msg);

			var message = msg.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
			var args = message[1].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			switch (message[0])
			{
				case "diiiscg":
					Task.Run(() =>
					{
						try
						{
							var game = GameManager.CreateGame(int.Parse(args[0].Trim()), int.Parse(args[1].Trim()));
							lock (game.Players)
							{
								game.SetAct(int.Parse(args[2].Trim()));
								game.SetGameMode((Game.Mode)int.Parse(args[7].Trim()));
								game.IsHardcore = (args[6].Trim() == "True" ? true : false);
								game.IsSeasoned = (args[8].Trim() == "True" ? true : false);
								game.SetDifficulty(int.Parse(args[3].Trim()));
								if (game.GameMode != Game.Mode.Portals)
									game.SetQuestProgress(int.Parse(args[4].Trim()), int.Parse(args[5].Trim()));
								if (args.Length > 9)
									if (int.Parse(args[9].Trim()) > 0) game.EnablePerfTest(int.Parse(args[9].Trim()));
								
								//Good job, you reverse God, AiDIEvE =)
							}
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "Error creating game: ");
						}
					});
					break;
				default:
					Logger.Warn("Unknown message: {0}|{1}", message[0], message[1]);
					break;
			}
			return true;
		}

		private bool SenderServerConnected()
		{
			Logger.Info("GameServer connected to BattleNet.");
			System.Threading.Thread.Sleep(3000);
			string backEndIp = Config.Instance.BindIP;
			int backEndPort = Config.Instance.Port;
			bool pvp = false;
			if (!pvp)
				RegisterGameServer(backEndIp, backEndPort);
			else
				RegisterPvPGameServer(backEndIp, backEndPort);
			return true;
		}

		private void BattleNetSocketSend(byte[] data)
		{
			Task.Run(() =>
			{
				lock (_globalSocketLock)
				{
					BattleNetSocket.Send(data);
				}
			});
		}

		private void BattleNetSocketSend(string data) => BattleNetSocketSend(Encoding.UTF8.GetBytes(data));

		private bool SenderServerDisconnected()
		{
			Logger.Warn("MooNetServer was disconnected!");
			return true;
		}

		public void RegisterGameServer(string ip, int port)
		{
			Logger.MethodTrace($"ip {ip}, port {port}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"rngsr|{ip}/{port}"));
		}

		public void RegisterPvPGameServer(string ip, int port)
		{
			Logger.MethodTrace($"ip {ip}, port {port}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"rnpvpgsr|{ip}/{port}"));
		}

		public void GrantAchievement(ulong gameAccountId, ulong achievementId)
		{
			Logger.MethodTrace($"gameAccountId {gameAccountId}, achievementId {achievementId}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"grachi|{gameAccountId}/{achievementId}"));
		}

		public void GrantCriteria(ulong gameAccountId, ulong criteriaId)
		{
			Logger.MethodTrace($"gameAccountId {gameAccountId}, achievementId {criteriaId}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"gcrit|{gameAccountId}/{criteriaId}"));
		}

		public void UpdateAchievementCounter(ulong gameAccountId, int type, uint addCounter, int comparand, ulong achievement = 0)
		{
			Logger.MethodTrace($"type {type}, addCounter {addCounter}, comparand {comparand}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes(
				$"uoacce|{gameAccountId}/{type}/{addCounter}/{comparand}/{achievement}"));
		}

		public void UpdateSingleAchievementCounter(ulong gameAccountId, ulong achId, uint addCounter)
		{
			Logger.MethodTrace($"type {achId}, addCounter {addCounter}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"upsnaccr|{gameAccountId}/{achId}/{addCounter}"));
		}

		public void UpdateQuantity(ulong gameAccountId, ulong achievementId, uint addCounter)
		{
			Logger.MethodTrace($"achievementId {achievementId}, addCounter {addCounter}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"upequt|{gameAccountId}/{achievementId}/{addCounter}"));
		}

		public void CheckQuestCriteria(ulong gameAccountId, int questId, bool isCoop)
		{
			Logger.MethodTrace($"gameAccountId {gameAccountId}, questId {questId}, coop {isCoop}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"cqc|{gameAccountId}/{questId}/{(isCoop ? "True" : "False")}"));
		}

		public void CheckKillMonsterCriteria(ulong gameAccountId, int actorId, int type, bool isHardcore)
		{
			Logger.MethodTrace($"gameAccountId {gameAccountId}, actorId {actorId}, type {type}, hc {isHardcore}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes(
				$"ckmc|{gameAccountId}/{actorId}/{type}/{(isHardcore ? "True" : "False")}"));
		}

		public void CheckSalvageItemCriteria(ulong gameAccountId, int itemId)
		{
			Logger.MethodTrace($"gameAccountId {gameAccountId}, itemId {itemId}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"csic|{gameAccountId}/{itemId}"));
		}

		public void CheckLevelCap(ulong gameAccountId)
		{
			Logger.MethodTrace($"gameAccountId {gameAccountId}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"clc|{gameAccountId}"));
		}

		public void CheckConversationCriteria(ulong gameAccountId, int convId)
		{
			Logger.MethodTrace($"gameAccountId {gameAccountId}, convId {convId}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"ccc|{gameAccountId}/{convId}"));
		}

		public void CheckLevelAreaCriteria(ulong gameAccountId, int laId)
		{
			Logger.MethodTrace($"gameAccountId {gameAccountId}, laId {laId}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"clac|{gameAccountId}/{laId}"));
		}

		public void UpdateClient(ulong gameAccountId, int level, int screen)
		{
			Logger.MethodTrace($"gameAccountId {gameAccountId}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"uc|{gameAccountId}/{level}/{screen}"));
		}

		public void PlayerJoined(int gameId)
		{
			Logger.MethodTrace($"gameId {gameId}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"gpj|{gameId}"));
		}

		public void PlayerLeft(int gameId)
		{
			Logger.MethodTrace($"gameId {gameId}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"gpl|{gameId}"));
		}

		public void SetGamePublic(int gameId)
		{
			Logger.MethodTrace($"gameId {gameId}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"gsp|{gameId}"));
		}

		public void PvPSaveProgress(ulong gameAccountId, int kills, int wins, int gold)
		{
			Logger.MethodTrace($"game account id {gameAccountId}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"pvpsp|{gameAccountId}/{kills}/{wins}/{gold}"));
		}

		public void ParagonLevelUp(ulong gameAccountId)
		{
			Logger.MethodTrace($"game account id {gameAccountId}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"plu|{gameAccountId}"));
		}

		public void ToonStateChanged(ulong toonId)
		{
			Logger.MethodTrace($"game account id {toonId}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"tsc|{toonId}"));
		}

		public void UniqueItemIdentified(ulong gameAccountId, ulong itemId)
		{
			Logger.MethodTrace($"gameAccountId {gameAccountId}, itemId {itemId}");
			BattleNetSocketSend(Encoding.UTF8.GetBytes($"uii|{gameAccountId}/{itemId}"));
		}
	}
}
