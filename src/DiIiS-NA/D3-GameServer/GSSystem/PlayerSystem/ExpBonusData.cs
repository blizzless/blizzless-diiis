using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			_player = player;
			_killstreakTickTime = 240;
			_killstreakPlayer = 0;
			_killstreakEnvironment = 0;
			_lastMonsterKillTick = 0;
			_lastMonsterAttackTick = 0;
			_lastMonsterAttackKills = 0;
			_lastEnvironmentDestroyTick = 0;
			_lastEnvironmentDestroyMonsterKills = 0;
			_lastEnvironmentDestroyMonsterKillTick = 0;
		}

		public void Update(int attackerActorType, int defeatedActorType)
		{
			if (attackerActorType == 7) // Player
			{
				if (defeatedActorType == 1) // Monster
				{
					// Massacre
					if (_lastMonsterKillTick + _killstreakTickTime > _player.InGameClient.Game.TickCounter)
					{
						_killstreakPlayer++;
						
					}
					else
					{
						_killstreakPlayer = 1;
					}

					// MightyBlow
					if (Math.Abs(_lastMonsterAttackTick - _player.InGameClient.Game.TickCounter) <= 20)
					{
						_lastMonsterAttackKills++;
						
					}
					else
					{
						_lastMonsterAttackKills = 1;
					}

					_lastMonsterKillTick = _player.InGameClient.Game.TickCounter;
				}
				else if (defeatedActorType == 5) // Environment
				{
					// Destruction
					if (_lastEnvironmentDestroyTick + _killstreakTickTime > _player.InGameClient.Game.TickCounter)
					{
						_killstreakEnvironment++;
					}
					else
					{
						_killstreakEnvironment = 1;
					}

					_lastEnvironmentDestroyTick = _player.InGameClient.Game.TickCounter;
				}
			}
			else if (attackerActorType == 5) // Environment
			{
				// Pulverized
				if (Math.Abs(_lastEnvironmentDestroyMonsterKillTick - _player.InGameClient.Game.TickCounter) <= 20)
				{
					_lastEnvironmentDestroyMonsterKills++;
				}
				else
				{
					_lastEnvironmentDestroyMonsterKills = 1;
				}

				_lastEnvironmentDestroyMonsterKillTick = _player.InGameClient.Game.TickCounter;
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
						if ((_killstreakPlayer > 10) && (_lastMonsterKillTick + _killstreakTickTime <= _player.InGameClient.Game.TickCounter))
						{
							defeated = _killstreakPlayer;
							expBonus = (_killstreakPlayer - 10) * 10;

							_killstreakPlayer = 0;
						}
						break;
					}
				case 1: // Destruction
					{
						if ((_killstreakEnvironment > 5) && (_lastEnvironmentDestroyTick + _killstreakTickTime <= _player.InGameClient.Game.TickCounter))
						{
							defeated = _killstreakEnvironment;
							expBonus = (_killstreakEnvironment - 5) * 5;

							_killstreakEnvironment = 0;
						}
						break;
					}
				case 2: // Mighty Blow
					{
						if (_lastMonsterAttackKills > 10)
						{
							defeated = _lastMonsterAttackKills;
							expBonus = (_lastMonsterAttackKills - 10) * 5;
						}
						_lastMonsterAttackKills = 0;
						break;
					}
				case 3: // Pulverized
					{
						if (_lastEnvironmentDestroyMonsterKills > 9)
						{
							defeated = _lastEnvironmentDestroyMonsterKills;
							expBonus = (_lastEnvironmentDestroyMonsterKills - 9) * 10;
						}
						_lastEnvironmentDestroyMonsterKills = 0;
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
				expBonus = (int)(expBonus * _player.World.Game.XpModifier);

				_player.InGameClient.SendMessage(new KillCounterUpdateMessage()
				{
					BonusType = bonusType,
					KilledCount = defeated,
					XPMultiplier = 1.00f + (0.01f * defeated),
					TotalTime = expBonus,
					Expired = true,
				});

				_player.UpdateExp(expBonus);
				_player.Conversations.StartConversation(0x0002A73F);
			}
		}

		public void MonsterAttacked(int monsterAttackTick)
		{
			_lastMonsterAttackTick = monsterAttackTick;
			if (_killstreakPlayer > 10)
				_player.InGameClient.SendMessage(new KillCounterUpdateMessage()
				{
					BonusType = 0,
					KilledCount = _killstreakPlayer,
					XPMultiplier = 1.00f + (0.01f * _killstreakPlayer),
					Expired = false,
				});
		}
	}
}
