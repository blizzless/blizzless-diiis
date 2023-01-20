//Blizzless Project 2022
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Act;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Connection;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Portal;
//Blizzless Project 2022
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022
using System;
//Blizzless Project 2022
using System.Linq;

namespace DiIiS_NA.GameServer.ClientSystem
{
    public class ClientManager : IMessageConsumer
    {
        protected static readonly Logger Logger = LogManager.CreateLogger("C");
        private static readonly ClientManager _instance = new ClientManager();
        public static ClientManager Instance { get { return _instance; } }
        public static int falsecounter = 0;

        public void OnConnect(object sender, Base.ConnectionEventArgs e)
        {
            var gameClient = new GameClient(e.Connection);
            e.Connection.Client = gameClient;
        }

        public void OnDisconnect(object sender, Base.ConnectionEventArgs e)
        {
            GameManager.RemovePlayerFromGame((GameClient)e.Connection.Client);
		}

        public void Consume(GameClient client, GameMessage message)
        {
            if (message is JoinBNetGameMessage) OnJoinGame(client, (JoinBNetGameMessage)message);
        }

		public void OnJoinGame(GameClient client, JoinBNetGameMessage message)
		{
			//System.Threading.Tasks.Task.Delay(1000).Wait();
			var game = GameManager.GetGameById(message.SGameId);
			Toon toon = null;
			if (game != null)
				toon = ToonManager.GetToonByLowID((ulong)message.HeroID, game.GameDBSession);
			bool PVP = false;
			if (PVP)
				toon = new Toon(ToonManager.CreateFakeDBToon(toon.GameAccount.Owner.BattleTag, toon.GameAccount.DBGameAccount), game.GameDBSession);

			if (game == null)
			{
				if (PVP)
				{
					game = GameManager.CreateGame(message.SGameId, 1);
					toon = ToonManager.GetToonByLowID((ulong)message.HeroID, game.GameDBSession);
					game.SetAct(0);
				}
				else
				{
					Logger.Warn("Client provided message.GameId doesnt exists, dropping client...");
					client.Connection.Disconnect();
					return;
				}
			}

			//Task.Run(() =>
			//{
			lock (game)
			{

				client.Game = game;

				/*if (toon.GameAccount.LoggedInClient == null)
				{
					Logger.Warn("Client doesn't seem to be connected to moonet, dropping him..");
					client.Connection.Disconnect();
					return; // if moonet connection is lost, don't allow him to get in.
				}*/

				// Set references between MooNetClient and GameClient.
				if (//Mooege.Net.MooNet.Config.Instance.Enabled &&
					toon.GameAccount.LoggedInClient != null)
				{
					client.BnetClient = toon.GameAccount.LoggedInClient;
					client.BnetClient.InGameClient = client;
				}

				lock (game.StartingWorld)
					client.Player = new Player(game.StartingWorld, client, toon);

				Logger.Debug("Player {0}[PlayerIndex: {1}, PlayerGroupIndex: {2}] connected.", client.Player.Toon.Name, client.Player.PlayerIndex, client.Player.PlayerGroupIndex);

				client.SendMessage(new VersionsMessage((uint)message.SNOPackHash, (uint)message.ProtocolHash));
				client.SendMessage(new ConnectionEstablishedMessage
				{
					PlayerIndex = client.Player.PlayerIndex,
					AnimSyncedSeed = 0x4BB91A16,
					SNOPackHash = message.SNOPackHash,
				});
				client.SendMessage(new GameSetupMessage // should be the current tick for the game /raist.
				{
					FirstHeartBeat = game.TickCounter,
				});
				client.SendMessage(new HearthPortalInfoMessage
				{
					snoLevelArea = -1,
					snoUnknown = -1,
					Field1 = -1,
				});
				client.SendMessage(new SavePointInfoMessage
				{
					snoLevelArea = -1,
				});

				// transition player to act so client can load act related data? /raist
				client.SendMessage(new ActTransitionMessage
				{
					Act = game.CurrentAct,
					OnJoin = true, //without cutscenes
				});

                if (client.Player.PlayerIndex > 0)
                {
                    //make sure toons Difficulty is set
                    toon.CurrentDifficulty = game.Difficulty;
                    client.SendMessage(new HandicapMessage(Opcodes.HandicapMessage)
                    {
                        Difficulty = (uint)game.Difficulty
                    });
                }


                toon.LoginTime = DateTimeExtensions.ToUnixTime(DateTime.UtcNow);
				Logger.Debug("Log in time:" + toon.LoginTime.ToString());

				game.Enter(client.Player);


			}
		}

		public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

    }
}
