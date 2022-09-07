//Blizzless Project 2022 
using DiIiS_NA.Core.Discord;
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.ClientSystem.Base;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.ClientSystem
{
	public sealed class GameServer : Server
	{
		private new static readonly Logger Logger = LogManager.CreateLogger("GS"); // hide the Server.Logger so that tiny-logger can show the actual server as log source.

		public static GSBackend GSBackend { get; set; }

		public static int MaintenanceTime = -1;
		public Bot DiscordBot { get; set; }
		public GameServer()
		{
			this.OnConnect += ClientManager.Instance.OnConnect;
			this.OnDisconnect += ClientManager.Instance.OnDisconnect;
			this.DataReceived += GameServer_DataReceived;
		}

		void GameServer_DataReceived(object sender, ConnectionDataEventArgs e)
		{
			var connection = (Connection)e.Connection;
			((GameClient)connection.Client).Parse(e);
		}

		public override void Run()
		{
			int Port = 2001;
			 
			if (!this.Listen(Program.GAMESERVERIP, Port)) return;
			Logger.Info("Game Server Started - {0}:{1}...", Program.GAMESERVERIP, Port);
		}
	}
}
