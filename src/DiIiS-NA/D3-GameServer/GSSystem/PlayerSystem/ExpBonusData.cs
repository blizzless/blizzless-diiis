//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Combat;
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

namespace DiIiS_NA.GameServer.GSSystem.PlayerSystem
{
	public class ExpBonusData
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		/// <summary>
		/// The referenced player.
		/// </summary>
		private Player _player;

		/// <summary>
		/// The time between two kills to still count as a killstreak.
		/// </summary>
		private int _killstreakTickTime;

		/// <summary>
		/// The player's killcounter in a killstreak.
		/// </summary>
		private int _killstreakPlayer;

		/// <summary>
		/// The environment's killcounter in a killstreak.
		/// </summary>
		private int _killstreakEnvironment;

		/// <summary>
		/// The last tick in which the player killed any monster.
		/// </summary>
		private int _lastMonsterKillTick;

		/// <summary>
		/// The last tick in which the player attacked any monster.
		/// </summary>
		private int _lastMonsterAttackTick;

		/// <summary>
		/// The number of monster-kills of the player's latest monster-attack.
		/// </summary>
		private int _lastMonsterAttackKills;

		/// <summary>
		/// The last tick in which environement got destroyed by the player.
		/// </summary>
		private int _lastEnvironmentDestroyTick;

		/// <summary>
		/// The number of monster-kills of the last environment-destruction.
		/// </summary>
		private int _lastEnvironmentDestroyMonsterKills;

		/// <summary>
		/// The last tick in which destroyed environment killed a monster.
		/// </summary>
		private int _lastEnvironmentDestroyMonsterKillTick;

		public ExpBonusData(Player player)
		{
			this._player = player;
			this._killstreakTickTime = 240;
			this._killstreakPlayer = 0;
			this._killstreakEnvironment = 0;
			this._lastMonsterKillTick = 0;
			this._lastMonsterAttackTick = 0;
			this._lastMonsterAttackKills = 0;
			this._lastEnvironmentDestroyTick = 0;
			this._lastEnvironmentDestroyMonsterKills = 0;
			this._lastEnvironmentDestroyMonsterKillTick = 0;
		}

		public void Update(int attackerActorType, int defeatedActorType)
		{
			if (attackerActorType == 7) // Player
			{
				if (defeatedActorType == 1) // Monster
				{
					// Massacre
					if (this._lastMonsterKillTick + this._killstreakTickTime > this._player.InGameClient.Game.TickCounter)
					{
						this._killstreakPlayer++;
						
					}
					else
					{
						this._killstreakPlayer = 1;
					}

					// MightyBlow
					if (Math.Abs(this._lastMonsterAttackTick - this._player.InGameClient.Game.TickCounter) <= 20)
					{
						this._lastMonsterAttackKills++;
						
					}
					else
					{
						this._lastMonsterAttackKills = 1;
					}

					this._lastMonsterKillTick = this._player.InGameClient.Game.TickCounter;
				}
				else if (defeatedActorType == 5) // Environment
				{
					// Destruction
					if (this._lastEnvironmentDestroyTick + this._killstreakTickTime > this._player.InGameClient.Game.TickCounter)
					{
						this._killstreakEnvironment++;
					}
					else
					{
						this._killstreakEnvironment = 1;
					}

					this._lastEnvironmentDestroyTick = this._player.InGameClient.Game.TickCounter;
				}
			}
			else if (attackerActorType == 5) // Environment
			{
				// Pulverized
				if (Math.Abs(this._lastEnvironmentDestroyMonsterKillTick - this._player.InGameClient.Game.TickCounter) <= 20)
				{
					this._lastEnvironmentDestroyMonsterKills++;
				}
				else
				{
					this._lastEnvironmentDestroyMonsterKills = 1;
				}

				this._lastEnvironmentDestroyMonsterKillTick = this._player.InGameClient.Game.TickCounter;
			}
		}

		public void Check(byte bonusType)
		{
			int defeated = 0;
			int expBonus = 0;

			switch (bonusType)
			{
				case 0: // Massacre
					{
						if ((this._killstreakPlayer > 10) && (this._lastMonsterKillTick + this._killstreakTickTime <= this._player.InGameClient.Game.TickCounter))
						{
							defeated = this._killstreakPlayer;
							expBonus = (this._killstreakPlayer - 10) * 10;

							this._killstreakPlayer = 0;
						}
						break;
					}
				case 1: // Destruction
					{
						if ((this._killstreakEnvironment > 5) && (this._lastEnvironmentDestroyTick + this._killstreakTickTime <= this._player.InGameClient.Game.TickCounter))
						{
							defeated = this._killstreakEnvironment;
							expBonus = (this._killstreakEnvironment - 5) * 5;

							this._killstreakEnvironment = 0;
						}
						break;
					}
				case 2: // Mighty Blow
					{
						if (this._lastMonsterAttackKills > 10)
						{
							defeated = this._lastMonsterAttackKills;
							expBonus = (this._lastMonsterAttackKills - 10) * 5;
						}
						this._lastMonsterAttackKills = 0;
						break;
					}
				case 3: // Pulverized
					{
						if (this._lastEnvironmentDestroyMonsterKills > 9)
						{
							defeated = this._lastEnvironmentDestroyMonsterKills;
							expBonus = (this._lastEnvironmentDestroyMonsterKills - 9) * 10;
						}
						this._lastEnvironmentDestroyMonsterKills = 0;
						break;
					}
				default:
					{
						Logger.Warn("Invalid Exp-Bonus-Type was checked.");
						return;
					}
			}

			if (expBonus > 0)
			{
				expBonus = (int)(expBonus * this._player.World.Game.XPModifier);

				this._player.InGameClient.SendMessage(new KillCounterUpdateMessage()
				{
					BonusType = bonusType,
					KilledCount = defeated,
					XPMultiplier = 1.00f + (0.01f * defeated),
					TotalTime = expBonus,
					Expired = true,
				});

				this._player.UpdateExp(expBonus);
				this._player.Conversations.StartConversation(0x0002A73F);
			}
		}

		public void MonsterAttacked(int monsterAttackTick)
		{
			this._lastMonsterAttackTick = monsterAttackTick;
			if (this._killstreakPlayer > 10)
				this._player.InGameClient.SendMessage(new KillCounterUpdateMessage()
				{
					BonusType = 0,
					KilledCount = _killstreakPlayer,
					XPMultiplier = 1.00f + (0.01f * _killstreakPlayer),
					Expired = false,
				});
		}
	}
}
