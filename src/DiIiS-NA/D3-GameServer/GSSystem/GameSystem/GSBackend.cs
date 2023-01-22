//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;
//Blizzless Project 2022 
using WatsonTcp;

namespace DiIiS_NA.GameServer.GSSystem.GameSystem
{
	public class GSBackend
	{
		private static readonly Logger Logger = LogManager.CreateLogger("GSBackend");

		private static Object _globalSocketLock = new object();

		public WatsonTcpClient BattleNetSocket;

		public GSBackend(string BattletHost, int BattlePort)
		{
			BattleNetSocket = new WatsonTcpClient(BattletHost, BattlePort, _senderServerConnected, _senderServerDisconnected, _senderMessageReceived, false);
		}


		private bool _senderMessageReceived(byte[] data)
		{
			string msg = "";
			if (data != null && data.Length > 0) msg = Encoding.UTF8.GetString(data);
			Logger.Trace("Сообщение от Battle.net: {0}", msg);

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
							Logger.WarnException(e, "Ошибка создания игры: ");
						}
					});
					break;
				default:
					Logger.Warn("Неизвестное сообщение: {0}|{1}", message[0], message[1]);
					break;
			}
			return true;
		}

		private bool _senderServerConnected()
		{
			//Logger.Info("GameServer подключен к BattleNet.");
			System.Threading.Thread.Sleep(3000);
			string BackEndIP = Config.Instance.BindIP;
			int BackEndPort = Config.Instance.Port;
			bool PVP = false;
			if (!PVP)
				RegisterGameServer(BackEndIP, BackEndPort);
			else
				RegisterPvPGameServer(BackEndIP, BackEndPort);
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

		private bool _senderServerDisconnected()
		{
			Logger.Warn("MooNetServer was disconnected!");
			return true;
		}

		public void RegisterGameServer(string ip, int port)
		{
			Logger.Trace("RegisterGameServer(): ip {0}, port {1}", ip, port);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("rngsr|{0}/{1}", ip, port)));
		}

		public void RegisterPvPGameServer(string ip, int port)
		{
			Logger.Trace("RegisterPvPGameServer(): ip {0}, port {1}", ip, port);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("rnpvpgsr|{0}/{1}", ip, port)));
		}

		public void GrantAchievement(ulong gameAccountId, ulong achievementId)
		{
			Logger.Trace("GrantAchievement(): gameAccountId {0}, achievementId {1}", gameAccountId, achievementId);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("grachi|{0}/{1}", gameAccountId, achievementId)));
		}

		public void GrantCriteria(ulong gameAccountId, ulong criteriaId)
		{
			Logger.Trace("GrantCriteria(): gameAccountId {0}, criteriaId {1}", gameAccountId, criteriaId);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("gcrit|{0}/{1}", gameAccountId, criteriaId)));
		}

		public void UpdateAchievementCounter(ulong gameAccountId, int type, uint addCounter, int comparand, ulong achievement = 0)
		{
			Logger.Trace("UpdateAchievementCounter(): type {0}, addCounter {1}, comparand {2}", type, addCounter, comparand);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("uoacce|{0}/{1}/{2}/{3}/{4}", gameAccountId, type, addCounter, comparand, achievement)));
		}

		public void UpdateSingleAchievementCounter(ulong gameAccountId, ulong achId, uint addCounter)
		{
			Logger.Trace("UpdateSingleAchievementCounter(): type {0}, addCounter {1}", achId, addCounter);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("upsnaccr|{0}/{1}/{2}", gameAccountId, achId, addCounter)));
		}

		public void UpdateQuantity(ulong gameAccountId, ulong achievementId, uint addCounter)
		{
			Logger.Trace("UpdateQuantity(): achievementId {0}, addCounter {1}", achievementId, addCounter);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("upequt|{0}/{1}/{2}", gameAccountId, achievementId, addCounter)));
		}

		public void CheckQuestCriteria(ulong gameAccountId, int questId, bool isCoop)
		{
			Logger.Trace("CheckQuestCriteria(): gameAccountId {0}, questId {1}, coop {2}", gameAccountId, questId, isCoop);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("cqc|{0}/{1}/{2}", gameAccountId, questId, (isCoop ? "True" : "False"))));
		}

		public void CheckKillMonsterCriteria(ulong gameAccountId, int actorId, int type, bool isHardcore)
		{
			Logger.Trace("CheckKillMonsterCriteria(): gameAccountId {0}, actorId {1}, type {2}, hc {3}", gameAccountId, actorId, type, isHardcore);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("ckmc|{0}/{1}/{2}/{3}", gameAccountId, actorId, type, (isHardcore ? "True" : "False"))));
		}

		public void CheckSalvageItemCriteria(ulong gameAccountId, int itemId)
		{
			Logger.Trace("CheckSalvageItemCriteria(): gameAccountId {0}, itemId {1}", gameAccountId, itemId);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("csic|{0}/{1}", gameAccountId, itemId)));
		}

		public void CheckLevelCap(ulong gameAccountId)
		{
			Logger.Trace("CheckLevelCap(): gameAccountId {0}", gameAccountId);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("clc|{0}", gameAccountId)));
		}

		public void CheckConversationCriteria(ulong gameAccountId, int convId)
		{
			Logger.Trace("CheckConversationCriteria(): gameAccountId {0}, convId {1}", gameAccountId, convId);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("ccc|{0}/{1}", gameAccountId, convId)));
		}

		public void CheckLevelAreaCriteria(ulong gameAccountId, int laId)
		{
			Logger.Trace("CheckLevelAreaCriteria(): gameAccountId {0}, laId {1}", gameAccountId, laId);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("clac|{0}/{1}", gameAccountId, laId)));
		}

		public void UpdateClient(ulong gameAccountId, int level, int screen)
		{
			Logger.Trace("UpdateClient(): gameAccountId {0}", gameAccountId);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("uc|{0}/{1}/{2}", gameAccountId, level, screen)));
		}

		public void PlayerJoined(int gameId)
		{
			Logger.Trace("PlayerJoined(): gameId {0}", gameId);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("gpj|{0}", gameId)));
		}

		public void PlayerLeft(int gameId)
		{
			Logger.Trace("PlayerLeft(): gameId {0}", gameId);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("gpl|{0}", gameId)));
		}

		public void SetGamePublic(int gameId)
		{
			Logger.Trace("SetGamePublic(): gameId {0}", gameId);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("gsp|{0}", gameId)));
		}

		public void PvPSaveProgress(ulong gameAccountId, int kills, int wins, int gold)
		{
			Logger.Trace("PvPSaveProgress(): gameAccountId {0}", gameAccountId);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("pvpsp|{0}/{1}/{2}/{3}", gameAccountId, kills, wins, gold)));
		}

		public void ParagonLevelUp(ulong gameAccountId)
		{
			Logger.Trace("ParagonLevelUp(): gameAccountId {0}", gameAccountId);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("plu|{0}", gameAccountId)));
		}

		public void ToonStateChanged(ulong toonID)
		{
			Logger.Trace("ToonStateChanged(): toonID {0}", toonID);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("tsc|{0}", toonID)));
		}

		public void UniqueItemIdentified(ulong gameAccountId, ulong itemId)
		{
			Logger.Trace("UniqueItemIdentified(): gameAccountId {0}, itemId {1}", gameAccountId, itemId);
			BattleNetSocketSend(Encoding.UTF8.GetBytes(string.Format("uii|{0}/{1}", gameAccountId, itemId)));
		}
	}
}
