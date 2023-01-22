//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Misc;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public class PowerManager
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		// list of all actively channeled skills
		private List<ChanneledSkill> _channeledSkills = new List<ChanneledSkill>();

		// list of all executing power scripts
		private class ExecutingScript
		{
			public IEnumerator<TickTimer> PowerEnumerator;
			public PowerScript Script;
		}
		private List<ExecutingScript> _executingScripts = new List<ExecutingScript>();

		// list of actors that were killed and are waiting to be deleted
		// rather ugly hack needed because deleting actors immediatly when they have visual buff effects
		// applied causes the effects to stay around forever.
		private Dictionary<Actor, TickTimer> _deletingActors = new Dictionary<Actor, TickTimer>();

		public PowerManager()
		{
		}

		public void Update()
		{
			_UpdateDeletingActors();
			_UpdateExecutingScripts();
		}

		private void CheckItemProcs(Player user)
		{
			if (user.SkillSet.HasItemPassiveProc(248776))
			{
				user.PlayEffectGroup(249956);
			}
			if (user.Attributes[GameAttribute.Item_Power_Passive, 246116] == 1 && FastRandom.Instance.NextDouble() < 0.2)
			{
				user.PlayEffectGroup(246117);
			}
		}

		private int cheatCounter = 0;

		public bool RunPower(Actor user, PowerScript power, Actor target = null, Vector3D targetPosition = null, TargetMessage targetMessage = null)
		{
			if (power.PowerSNO == 168344 || power.PowerSNO == 167648) //teleport
			{
				if (!user.World.CheckLocationForFlag(PowerMath.TranslateDirection2D(user.Position, targetPosition, user.Position, Math.Min(PowerMath.Distance2D(user.Position, targetPosition), 35f)), DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
					return false;
			}

			if (user.Attributes[GameAttribute.Disabled] == true) return false;

			if (user is Player && targetPosition != null)
				CheckItemProcs(user as Player);

			//break stun if possible
			if (PowerTagHelper.FindTagMapWithKey(power.PowerSNO, PowerKeys.BreaksStun) != null)
				if (user.Attributes[GameAttribute.Stunned] == true || user.Attributes[GameAttribute.Frozen] == true)
				{
					float result;
					if (ScriptFormulaEvaluator.Evaluate(power.PowerSNO, PowerKeys.BreaksStun, user.Attributes, PowerContext.Rand, out result) && result > 0)
					{
						user.World.BuffManager.RemoveBuffs(user, 101000);
						user.Attributes[GameAttribute.Frozen] = false;
						user.Attributes.BroadcastChangedIfRevealed();
					}
				}
			//break fear if possible
			if (PowerTagHelper.FindTagMapWithKey(power.PowerSNO, PowerKeys.BreaksFear) != null)
				if (user.Attributes[GameAttribute.Feared] == true)
				{
					float result;
					if (ScriptFormulaEvaluator.Evaluate(power.PowerSNO, PowerKeys.BreaksFear, user.Attributes, PowerContext.Rand, out result) && result > 0)
						user.World.BuffManager.RemoveBuffs(user, 101002);
				}
			//break root if possible
			if (PowerTagHelper.FindTagMapWithKey(power.PowerSNO, PowerKeys.BreaksRoot) != null)
				if (user.Attributes[GameAttribute.IsRooted] == true)
				{
					float result;
					if (ScriptFormulaEvaluator.Evaluate(power.PowerSNO, PowerKeys.BreaksRoot, user.Attributes, PowerContext.Rand, out result) && result > 0)
						user.World.BuffManager.RemoveBuffs(user, 101003);
				}
			// replace power with existing channel instance if one exists
			if (power is ChanneledSkill)
			{
				var existingChannel = _FindChannelingSkill(user, power.PowerSNO);
				if (existingChannel != null)
				{
					power = existingChannel;
				}
				else  // new channeled skill, add it to the list
				{
					_channeledSkills.Add((ChanneledSkill)power);
				}
			}
			else
			{
				user.TranslateFacing(targetPosition, true);
			}

			// copy in context params
			power.User = user;
			power.Target = target;
			power.World = user.World;
			power.TargetPosition = targetPosition;
			power.TargetMessage = targetMessage;

			user.LastSecondCasts++;

			if (user is Player && !(power is ChanneledSkill) && power.PowerSNO != 109344 && user.LastSecondCasts > user.Attributes[GameAttribute.Attacks_Per_Second_Total] + 1f)
			{
				//fix for ApS cheating
				user.Attributes[GameAttribute.Attacks_Per_Second] -= 0.00000001f;
				user.Attributes.BroadcastChangedIfRevealed();
				cheatCounter++;
				if (cheatCounter > 5)
				{
					//Logger.Warn("Player {0}, skill {1} - possible attack speed cheat!", (power.User as Player).Toon.Name, power.PowerSNO);
					cheatCounter = 0;
				}

			}

			_StartScript(power);
			return true;
		}

		public bool RunPower(Actor user, int powerSNO, uint targetId = uint.MaxValue, Vector3D targetPosition = null, TargetMessage targetMessage = null)
		{
			Actor target;
			if (powerSNO == -1) return false;
			
			if (targetId == uint.MaxValue)
			{
				target = null;
			}
			else
			{
				target = user.World.GetActorByGlobalId(targetId);
				if (user is Player)
					foreach (var obj in (user as Player).RevealedObjects)
						if (obj.Value == targetId)
							target = user.World.GetActorByGlobalId(obj.Key);



				if (target == null)
					return false;

				targetPosition = target.Position;
			}

			// find and run a power implementation
			var implementation = PowerLoader.CreateImplementationForPowerSNO(powerSNO);
			if (implementation != null)
			{
				return RunPower(user, implementation, target, targetPosition, targetMessage);
			}
			else
			{
				return false;
			}
		}

		private void _UpdateExecutingScripts()
		{
			// process all powers, removing from the list the ones that expire
			try
			{
				_executingScripts.RemoveAll(script =>
				{
					try
					{
						if (script.PowerEnumerator.Current.TimedOut)
						{
							if (script.PowerEnumerator.MoveNext())
								return script.PowerEnumerator.Current == PowerScript.StopExecution;
							else
								return true;
						}
						else
						{
							return false;
						}
					}
					catch
					{
						return true;
					}

				});
			}
			catch
			{ }
		}

		public void CancelChanneledSkill(Actor user, int powerSNO)
		{
			var channeledSkill = _FindChannelingSkill(user, powerSNO);
			if (channeledSkill != null)
			{
				channeledSkill.CloseChannel();
				_channeledSkills.Remove(channeledSkill);
			}
			else
			{
				Logger.Debug("cancel channel for power {0}, but it doesn't have an open channel to cancel", powerSNO);
			}
		}

		private ChanneledSkill _FindChannelingSkill(Actor user, int powerSNO)
		{
			return _channeledSkills.FirstOrDefault(impl => impl.User == user &&
														   impl.PowerSNO == powerSNO &&
														   impl.IsChannelOpen);
		}

		private void _StartScript(PowerScript script)
		{
			var powerEnum = script.Run().GetEnumerator();
			if (powerEnum.MoveNext() && powerEnum.Current != PowerScript.StopExecution)
			{
				_executingScripts.Add(new ExecutingScript
				{
					PowerEnumerator = powerEnum,
					Script = script
				});
			}
		}

		private void _UpdateDeletingActors()
		{
			foreach (var key in _deletingActors.Keys.ToArray())
			{
				if (_deletingActors[key].TimedOut)
				{
					key.Destroy();
					_deletingActors.Remove(key);
				}
			}
		}

		public void AddDeletingActor(Actor actor)
		{
			try
			{
				_deletingActors.Add(actor, new SecondsTickTimer(actor.World.Game, 10f));
			}
			catch (ArgumentException) { }
		}

		public bool IsDeletingActor(Actor actor)
		{
			return _deletingActors.ContainsKey(actor);
		}

		public void CancelAllPowers(Actor user)
		{
			try
			{
				_channeledSkills.RemoveAll(impl =>
				{
					if (impl.User == user && impl.IsChannelOpen)
					{
						impl.CloseChannel();
						return true;
					}
					return false;
				});

				_executingScripts.RemoveAll((script) => script.Script.User == user);
			}
			catch { }
		}
	}
}
