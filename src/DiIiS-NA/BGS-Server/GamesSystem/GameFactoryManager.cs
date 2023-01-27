//Blizzless Project 2022
using bgs.protocol.matchmaking.v1;
using D3.OnlineService;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.LoginServer.Battle;
using DiIiS_NA.LoginServer.ChannelSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiIiS_NA.LoginServer.GamesSystem
{
	public static class GameFactoryManager
	{
		private static readonly Dictionary<ulong, GameDescriptor> GameCreators =
			new Dictionary<ulong, GameDescriptor>();

		public static int GamesOnline
		{
			get
			{
				return GameCreators.Values.Where(game => game.PlayersCount > 0).Count();
			}
			set { }
		}

		public static ulong RequestIdCounter = 1;

		public static GameDescriptor JoinGame(BattleClient client, bgs.protocol.games.v1.JoinGameRequest request, ulong requetId)
		{
			var game = FindGameByEntityId(request.GameHandle.GameId);
			return game;
		}

		public static GameDescriptor CreateGame(BattleClient owner, GameMatchmakingOptions request, ulong requestId)
		{
			var gameDescriptor = new GameDescriptor(owner, request, requestId);
			GameCreators.Add(gameDescriptor.DynamicId, gameDescriptor);
			ChannelManager.AddGameChannel(gameDescriptor);

			return gameDescriptor;
		}

		public static GameDescriptor FindGameByEntityId(bgs.protocol.EntityId entityId)
		{
			foreach (GameDescriptor game in GameCreators.Values)
			{
				if (game.BnetEntityId.Low == entityId.Low)
					return game;
			}
			return null;
		}

		public static GameDescriptor FindGameByDynamicId(ulong gameId)
		{
			foreach (GameDescriptor game in GameCreators.Values)
			{
				if (game.DynamicId == gameId)
					return game;
			}
			return null;
		}

		public static void DeleteGame(ulong dynId)
		{
			GameCreators.Remove(dynId);
		}

		public static GameDescriptor FindGame(BattleClient client, QueueMatchmakingRequest request, ulong requestId)
		{
			string request_type = "";
			string ServerPool = "";
			bgs.protocol.v2.Attribute AttributeOfServer = null;
			GameCreateParams gameCreateParams;
			foreach (var attr in request.Options.CreationProperties.AttributeList)
			{
				switch (attr.Name)
				{
					case "GameCreateParams":
						gameCreateParams = GameCreateParams.ParseFrom(attr.Value.BlobValue);
						AttributeOfServer = attr;
						break;
					case "ServerPool":
						ServerPool = attr.Value.StringValue;
						break;
					case "request_type":
						request_type = attr.Value.StringValue;
						break;

				}
			}

			List<GameDescriptor> matchingGames = FindMatchingGames(request);
			
			GameDescriptor TagGame = null;
			if (client.GameTeamTag != "") TagGame = FindTagGame(client.GameTeamTag);

			var rand = new Random();
			GameDescriptor gameDescriptor = null;
			
			if(TagGame != null)
				gameDescriptor = TagGame;
			else if (request_type == "find" && matchingGames.Count > 0)
				gameDescriptor = matchingGames[rand.Next(matchingGames.Count)];
			else
				gameDescriptor = CreateGame(client, request.Options, requestId);

			return gameDescriptor;
		}

		public static GameDescriptor FindPlayerGame()
		{
			foreach (GameDescriptor game in GameCreators.Values)
			{ 
			
			}
			return null;
		}

		public static GameDescriptor FindPlayerGame(BattleClient ToClient)
		{
			foreach (GameDescriptor game in GameCreators.Values)
			{
				if (game.D3EntityId == ToClient.GameChannel.D3EntityId)
					return game;
				;
			}
			return null;
		}

		private static GameDescriptor FindTagGame(string GameTag)
		{
			GameDescriptor taggame = null;
			foreach (GameDescriptor game in GameCreators.Values)
				if (game.PlayersCount > 0 && game.PlayersCount < 8)
					if (game.Owner.GameTeamTag == GameTag)
						taggame = game;

			return taggame;
		}

		private static List<GameDescriptor> FindMatchingGames(QueueMatchmakingRequest request)
		{
			String version = String.Empty;
			int monster_level = 0;
			int difficulty = 0;
			int currentQuest = 0;
			int currentAct = 0;
			bool ChallengeRift = false;


			D3.OnlineService.GameCreateParams Params = D3.OnlineService.GameCreateParams.ParseFrom(request.Options.CreationProperties.GetAttribute(0).Value.BlobValue);

			foreach (var attribute in request.Options.MatchmakerFilter.AttributeList)
			{
				switch (attribute.Name)
				{
					case "MonsterLevel":
						difficulty = (int)attribute.Value.IntValue;
						break;
					case "version":
						version = attribute.Value.StringValue;
						break;
					case "Game.MonsterLevel":
						monster_level = (int)attribute.Value.IntValue;
						break;
					case "HandicapLevel":
						difficulty = (int)attribute.Value.IntValue;
						break;
					case "Game.CurrentQuest":
						currentQuest = (int)attribute.Value.IntValue;
						break;
					case "Game.CurrentAct":
						currentAct = (int)attribute.Value.IntValue;
						break;
					case "MatchmakingPartition":
						if ((int)attribute.Value.IntValue == 9)
							ChallengeRift = true;
						break;
				}
			}
			List<GameDescriptor> matches = new List<GameDescriptor>();
			foreach (GameDescriptor game in GameCreators.Values)
			{
				if (game.PlayersCount > 0 && game.Public && game.PlayersCount < 50)
				{
					{

						if (ChallengeRift)
						{
							if (game.GameCreateParams.GameType == 9)
								matches.Add(game);
						}
						else
						{
							if (Params.CampaignOrAdventureMode.HandicapLevel == game.GameCreateParams.CampaignOrAdventureMode.HandicapLevel)
								if (Params.CampaignOrAdventureMode.Act == game.GameCreateParams.CampaignOrAdventureMode.Act)
								{
									if (Params.CampaignOrAdventureMode.SnoQuest == game.GameCreateParams.CampaignOrAdventureMode.SnoQuest)
									{
										matches.Add(game);
									}
									else if (Params.CampaignOrAdventureMode.SnoQuest == -1)
									{
										matches.Add(game);
									}
								}
								else if (Params.CampaignOrAdventureMode.Act == -1)
								{
									matches.Add(game);
								}
						}

					}
				}
			}
			return matches;
		}
		public enum Mode
		{
			Campaign = 1,
			Bounties = 2,
			Portals = 9
		}
		public static D3.GameMessage.MatchmakingStatsBucket GetStatsBucketWithFilter(D3.GameMessage.MatchmakingGetStats request)
		{
			String version = String.Empty;
			version = request.Version;
			int GameMode = request.Partition;

			int Difficulty = 0;
			int handicap = 0;
			uint GameQuest = 0;
			int GameAct = 0;
			string GameTag = "";
			var response = D3.GameMessage.MatchmakingStatsBucket.CreateBuilder();
			uint games = 0;
			
			uint players = 0;

			switch (request.Partition)
			{
				case 1: //"Campaign"
					
					handicap = request.HandicapLevel;
					Difficulty = request.MonsterLevel;
					GameTag = request.GameTag;
					GameAct = request.GameAct;
					GameQuest = request.GameQuest;
					break;
				case 2: //"Adventure"
					
					Difficulty = request.HandicapLevel;
					GameTag = request.GameTag;
					break;
				case 3: //"Adventure"
					break;
			}

			foreach (GameDescriptor game in GameCreators.Values)
			{
				if (game != null)
				{
					switch ((Mode)GameMode)
					{
						case Mode.Campaign:
							if (game.GameCreateParams.CampaignOrAdventureMode.HandicapLevel == handicap)
							{
								if (request.HasGameAct)
								{
									if (game.PlayersCount > 0)
										if (game.GameCreateParams.CampaignOrAdventureMode.SnoQuest == GameQuest)
										{
											games++;
											players += (uint)game.PlayersCount;
										}
										else if (!request.HasGameQuest)
										{
											games++;
											players += (uint)game.PlayersCount;
										}
								}
								else
								{
									games++;
									players += (uint)game.PlayersCount;
								}
							}
							break;

						case Mode.Bounties:
							if (game.GameCreateParams.CampaignOrAdventureMode.HandicapLevel == handicap)
							{
								games++;
								players += (uint)game.PlayersCount;
							}
							break;

						case Mode.Portals:
							break;

					}
				}
			}

			response.SetPlayersInOpenGamesTotal(players)
				   .SetOpenGamesTotal(games)
				   .SetWaitingPlayers(0)
				   .SetFormingGames(0)
				   ;

			return response.Build();
		}
		
	}
}
