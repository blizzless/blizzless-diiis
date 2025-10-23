using DiIiS_NA.Core.Discord;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.ClientSystem.Base;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.ClientSystem
{
	public sealed class GameServer : Server
	{
		private new static readonly Logger Logger = LogManager.CreateLogger("GS"); // hide the Server.Logger so that tiny-logger can show the actual server as log source.

		public static GsBackend GSBackend { get; set; }

		public static int MaintenanceTime = -1;
		public Bot DiscordBot { get; set; }
		public GameServer()
		{
			OnConnect += ClientManager.Instance.OnConnect;
			OnDisconnect += ClientManager.Instance.OnDisconnect;
			DataReceived += GameServer_DataReceived;
		}

		void GameServer_DataReceived(object sender, ConnectionDataEventArgs e)
		{
			var connection = (Connection)e.Connection;
			((GameClient)connection.Client).Parse(e);
		}

		public override void Run()
		{
			int Port = 2001;
			 
			if (!Listen(Program.GameServerIp, Port)) return;
			Logger.Info("Game Server Started - {0}:{1}...", Program.GameServerIp, Port);
		}
	}
}
