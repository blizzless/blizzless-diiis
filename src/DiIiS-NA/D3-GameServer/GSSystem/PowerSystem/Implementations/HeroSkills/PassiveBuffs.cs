using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
	#region ArcaneDynamo
	[ImplementsPowerSNO(208823)]
	[ImplementsPowerBuff(1, true)]
	public class DynamoBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(300f);
			MaxStackCount = 5;
		}

		public override bool Apply()
		{
			return base.Apply();
		}

		public override void Remove()
		{
			base.Remove();
		}
	}
	#endregion
	#region PowerHungry
	[ImplementsPowerSNO(208478)]
	[ImplementsPowerBuff(1, true)]
	public class HungryBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(300f);
			MaxStackCount = 10;
		}

		public override bool Apply()
		{
			return base.Apply();
		}

		public override void Remove()
		{
			base.Remove();
		}
	}
	#endregion
	#region GalvanizingWard
	[ImplementsPowerSNO(208541)]
	[ImplementsPowerBuff(1)]
	public class GalvanizingBuff : PowerBuff
	{
		TickTimer CheckTimer = null;
		bool WasHit = false;
		int SecondsPassed = 0;
		public override bool Apply()
		{
			return base.Apply();
		}

		public override bool Update()
		{
			if (base.Update())
				return true;

			if (CheckTimer == null || CheckTimer.TimedOut)
			{
				CheckTimer = WaitSeconds(1f);
				if (WasHit)
				{
					SecondsPassed = 0;
					WasHit = false;
					return false;
				}
				if (HasBuff<GalvShieldBuff>(User)) return false;

				SecondsPassed++;
				if (SecondsPassed >= 5)
				{
					AddBuff(User, new GalvShieldBuff());
					SecondsPassed = 0;
				}
			}
			return false;
		}
		public override void OnPayload(Payload payload)
		{
			if (payload is HitPayload && payload.Target == User && (payload as HitPayload).IsWeaponDamage)
			{
				WasHit = true;
			}
		}
		public override void Remove()
		{
			base.Remove();
		}
	}
	[ImplementsPowerSNO(208541)]
	[ImplementsPowerBuff(2)]
	class GalvShieldBuff : PowerBuff
	{
		float HPTreshold = 0f;

		public override void Init()
		{
			Timeout = WaitSeconds(300f);
			HPTreshold = User.Attributes[GameAttributes.Hitpoints_Max_Total] * 0.6f;
		}
		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			return true;
		}

		public override void OnPayload(Payload payload)
		{
			if (payload.Target == Target && payload is HitPayload &&
				(payload as HitPayload).IsWeaponDamage && HPTreshold > 0)
			{
				float dmg = (payload as HitPayload).TotalDamage;
				(payload as HitPayload).TotalDamage -= Math.Min((payload as HitPayload).TotalDamage, HPTreshold);
				HPTreshold -= dmg;
				if (HPTreshold <= 0)
					User.World.BuffManager.RemoveBuff(User, this);
			}
		}

		public override void Remove()
		{
			base.Remove();
		}
	}
	#endregion
	#region Dominance
	[ImplementsPowerSNO(341344)]
	[ImplementsPowerBuff(1, true)]
	public class DominanceBuff : PowerBuff
	{
		float HPTreshold = 0f;

		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(ScriptFormula(0));
			MaxStackCount = 10;
			HPTreshold = User.Attributes[GameAttributes.Hitpoints_Max_Total] * 0.02f;
		}

		public override bool Apply()
		{
			return base.Apply();
		}

		public override bool Stack(Buff buff)
		{
			bool stacked = StackCount < MaxStackCount;
			base.Stack(buff);

			if (!stacked) return true;
			HPTreshold = User.Attributes[GameAttributes.Hitpoints_Max_Total] * 0.02f * StackCount;
			Extend(30);

			return true;
		}

		public override void OnPayload(Payload payload)
		{
			if (payload.Target == Target && payload is HitPayload &&
				(payload as HitPayload).IsWeaponDamage && HPTreshold > 0)
			{
				float dmg = (payload as HitPayload).TotalDamage;
				(payload as HitPayload).TotalDamage -= Math.Min((payload as HitPayload).TotalDamage, HPTreshold);
				HPTreshold -= dmg;
				if (HPTreshold <= 0)
					User.World.BuffManager.RemoveBuff(User, this);
			}
		}

		public override void Remove()
		{
			base.Remove();
		}
	}
	#endregion
	#region Rampage
	[ImplementsPowerSNO(296572)]
	[ImplementsPowerBuff(1, true)]
	public class RampageBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(ScriptFormula(2));
			MaxStackCount = 25;
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			_currentBonus = 0.01f * StackCount;
			Target.Attributes[GameAttributes.Strength_Bonus_Percent] += _currentBonus;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		private float _currentBonus = 0f;

		public override bool Stack(Buff buff)
		{
			bool stacked = StackCount < MaxStackCount;

			base.Stack(buff);

			if (!stacked) return true;

			Target.Attributes[GameAttributes.Strength_Bonus_Percent] -= _currentBonus;

			_currentBonus = 0.01f * StackCount;
			Target.Attributes[GameAttributes.Strength_Bonus_Percent] += _currentBonus;
			User.Attributes.BroadcastChangedIfRevealed();

			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Strength_Bonus_Percent] -= _currentBonus;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion

	#region Elemental Exposure
	[ImplementsPowerSNO(342326)]
	[ImplementsPowerBuff(1, true)]
	public class ElementalExposureBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(ScriptFormula(3));
			MaxStackCount = 4;
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			_currentBonus = 0.05f * StackCount;
			Target.Attributes[GameAttributes.Amplify_Damage_Percent] += _currentBonus;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public int LastDamageType = -1;

		private float _currentBonus = 0f;

		public override bool Stack(Buff buff)
		{
			bool stacked = StackCount < MaxStackCount;

			base.Stack(buff);

			if (!stacked) return true;

			Target.Attributes[GameAttributes.Amplify_Damage_Percent] -= _currentBonus;

			_currentBonus = 0.05f * StackCount;
			Target.Attributes[GameAttributes.Amplify_Damage_Percent] += _currentBonus;
			User.Attributes.BroadcastChangedIfRevealed();

			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Amplify_Damage_Percent] -= _currentBonus;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
	#region HotPursuit
	[ImplementsPowerSNO(155725)]
	[ImplementsPowerBuff(0)]
	public class HotPursuitBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(100f);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			Target.Attributes[GameAttributes.Movement_Scalar_Uncapped_Bonus] += 0.15f;
			//Target.Attributes[GameAttribute.Running_Rate] = Target.Attributes[GameAttribute.Running_Rate] * 1.15f;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Movement_Scalar_Uncapped_Bonus] -= 0.15f;
			//Target.Attributes[GameAttribute.Running_Rate] = Target.Attributes[GameAttribute.Running_Rate] / 1.15f;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
	#region Fervor
	[ImplementsPowerSNO(357218)]
	[ImplementsPowerBuff(2)]
	public class FervorBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(3f);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			Target.Attributes[GameAttributes.Attacks_Per_Second_Percent] += 0.1f;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Attacks_Per_Second_Percent] -= 0.1f;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
	#region Insurmountable
	[ImplementsPowerSNO(310640)]
	[ImplementsPowerBuff(1)]
	public class InsurmountableBuff : PowerBuff
	{
		TickTimer CheckTimer = null;
		private bool Applied = false;

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			return true;
		}
		public override bool Update()
		{
			if (base.Update())
				return true;

			if (CheckTimer == null || CheckTimer.TimedOut)
			{
				CheckTimer = WaitSeconds(1f);
				if (Target.GetObjectsInRange<Monster>(12f).Count >= 4)
				{
					if (!Applied)
					{
						Target.Attributes[GameAttributes.Block_Amount_Bonus_Percent] += ScriptFormula(2);
						Target.Attributes.BroadcastChangedIfRevealed();
						Applied = true;
					}
				}
				else if (Applied)
				{
					Target.Attributes[GameAttributes.Block_Amount_Bonus_Percent] -= ScriptFormula(2);
					Target.Attributes.BroadcastChangedIfRevealed();
					Applied = false;
				}
			}
			return false;
		}
		public override void Remove()
		{
			base.Remove();
			if (Applied)
			{
				Target.Attributes[GameAttributes.Block_Amount_Bonus_Percent] -= ScriptFormula(2);
				Target.Attributes.BroadcastChangedIfRevealed();
				Applied = false;
			}
		}
	}
	#endregion
	#region Indestructible
	[ImplementsPowerSNO(309830)]
	[ImplementsPowerBuff(1)]
	public class IndestructibleBuff : PowerBuff
	{
		TickTimer CheckTimer = null;
		private float LastBonusPercent = 0f;

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			return true;
		}
		public override bool Update()
		{
			if (base.Update())
				return true;

			if (CheckTimer == null || CheckTimer.TimedOut)
			{
				CheckTimer = WaitSeconds(1f);

				Target.Attributes[GameAttributes.Armor_Bonus_Percent] -= LastBonusPercent;
				Target.Attributes.BroadcastChangedIfRevealed();

				LastBonusPercent = ((Target.Attributes[GameAttributes.Hitpoints_Max_Total] - Target.Attributes[GameAttributes.Hitpoints_Cur]) / Target.Attributes[GameAttributes.Hitpoints_Max_Total]) * 0.3f;
				Target.Attributes[GameAttributes.Armor_Bonus_Percent] += LastBonusPercent;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
			return false;
		}
		public override void Remove()
		{
			base.Remove();
			if (LastBonusPercent > 0f)
			{
				Target.Attributes[GameAttributes.Armor_Bonus_Percent] -= LastBonusPercent;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion
	#region DivineFortress
	[ImplementsPowerSNO(356176)]
	[ImplementsPowerBuff(1)]
	public class DivineFortressBuff : PowerBuff
	{
		TickTimer CheckTimer = null;
		private float LastBonusPercent = 0f;

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			return true;
		}
		public override bool Update()
		{
			if (base.Update())
				return true;

			if (CheckTimer == null || CheckTimer.TimedOut)
			{
				CheckTimer = WaitSeconds(1f);

				Target.Attributes[GameAttributes.Armor_Bonus_Percent] -= LastBonusPercent;
				Target.Attributes.BroadcastChangedIfRevealed();

				if ((Target as Player).Inventory.GetEquippedItems().Any(i => i.ItemType.Name.Contains("Shield")))
					LastBonusPercent = (Target as Player).Inventory.GetEquippedItems().Find(i => i.ItemType.Name.Contains("Shield")).Attributes[GameAttributes.Block_Chance_Item];
				else LastBonusPercent = 0f;

				Target.Attributes[GameAttributes.Armor_Bonus_Percent] += LastBonusPercent;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
			return false;
		}
		public override void Remove()
		{
			base.Remove();
			if (LastBonusPercent > 0f)
			{
				Target.Attributes[GameAttributes.Armor_Bonus_Percent] -= LastBonusPercent;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion
	#region Renewal
	[ImplementsPowerSNO(356173)]
	[ImplementsPowerBuff(1)]
	public class RenewalBuff : PowerBuff
	{
		private float Heal = 0f;
		public override void Init()
		{
			base.Init();
			Heal = 5 + 0.0006f * (float)Math.Pow(User.Attributes[GameAttributes.Level], 4);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			return true;
		}
		public override void OnPayload(Payload payload)
		{
			if (payload is HitPayload && payload.Target == User && (payload as HitPayload).Blocked)
				(User as Player).AddHP(Heal);
		}

		public override void Remove()
		{
			base.Remove();
		}
	}
	#endregion
	#region Finery
	[ImplementsPowerSNO(311629)]
	[ImplementsPowerBuff(1)]
	public class FineryBuff : PowerBuff
	{
		TickTimer CheckTimer = null;
		private float LastBonus = 0f;

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			return true;
		}
		public override bool Update()
		{
			if (base.Update())
				return true;

			if (CheckTimer == null || CheckTimer.TimedOut)
			{
				CheckTimer = WaitSeconds(1f);

				Target.Attributes[GameAttributes.Strength_Bonus] -= LastBonus;
				Target.Attributes.BroadcastChangedIfRevealed();

				LastBonus = 70f * (Target as Player).Inventory.GetEquippedItems().Sum(i => i.Gems.Count);
				Target.Attributes[GameAttributes.Strength_Bonus] += LastBonus;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
			return false;
		}
		public override void Remove()
		{
			base.Remove();
			if (LastBonus > 0f)
			{
				Target.Attributes[GameAttributes.Strength_Bonus] -= LastBonus;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion
	#region ToweringShield
	[ImplementsPowerSNO(356052)]
	[ImplementsPowerBuff(1)]
	public class ToweringShieldBuff : PowerBuff
	{
		TickTimer CheckTimer = null;
		private float InitialBonus = 0f;

		public override void Init()
		{
			InitialBonus = Target.Attributes[GameAttributes.Block_Chance];
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			return true;
		}
		public override bool Update()
		{
			if (base.Update())
				return true;

			if (CheckTimer == null || CheckTimer.TimedOut)
			{
				CheckTimer = WaitSeconds(1f);

				if (Target.Attributes[GameAttributes.Block_Chance] < 0.75f)
				{
					Target.Attributes[GameAttributes.Block_Chance] += 0.025f;
					Target.Attributes.BroadcastChangedIfRevealed();
				}
			}
			return false;
		}
		public override void OnPayload(Payload payload)
		{
			if (payload is HitPayload && payload.Target == User && (payload as HitPayload).Blocked)
				Target.Attributes[GameAttributes.Block_Chance] -= (Target.Attributes[GameAttributes.Block_Chance] - InitialBonus);
		}
		public override void Remove()
		{
			base.Remove();
			if (Target.Attributes[GameAttributes.Block_Chance] > InitialBonus)
				Target.Attributes[GameAttributes.Block_Chance] -= (Target.Attributes[GameAttributes.Block_Chance] - InitialBonus);
		}
	}
	#endregion
	#region Unwavering Will
	[ImplementsPowerSNO(298038)]
	[ImplementsPowerBuff(1)]
	public class UnwaveringWillBuff : PowerBuff
	{
		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			Target.Attributes[GameAttributes.Damage_Weapon_Percent_Bonus] += ScriptFormula(8);
			Target.Attributes[GameAttributes.Damage_Percent_All_From_Skills] += ScriptFormula(8);
			Target.Attributes[GameAttributes.Armor_Bonus_Percent] += ScriptFormula(2);
			Target.Attributes[GameAttributes.Resistance_Percent_All] += ScriptFormula(3);
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Damage_Weapon_Percent_Bonus] -= ScriptFormula(8);
			Target.Attributes[GameAttributes.Damage_Percent_All_From_Skills] -= ScriptFormula(8);
			Target.Attributes[GameAttributes.Armor_Bonus_Percent] -= ScriptFormula(2);
			Target.Attributes[GameAttributes.Resistance_Percent_All] -= ScriptFormula(3);
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
	#region InspiringPresence
	[ImplementsPowerSNO(205707)]
	[ImplementsPowerBuff(0)]
	public class InspiringPresenceBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(60f);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			Target.Attributes[GameAttributes.Hitpoints_Regen_Per_Second] += (Target.Attributes[GameAttributes.Hitpoints_Max_Total]) / 100;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Hitpoints_Regen_Per_Second] -= (Target.Attributes[GameAttributes.Hitpoints_Max_Total]) / 100;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
	#region SpiritVessel
	[ImplementsPowerSNO(218501)]
	[ImplementsPowerBuff(1)]
	public class SpiritVesselCooldownBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(90f);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			return true;
		}

		public override void Remove()
		{
			base.Remove();
		}
	}
	#endregion
	#region NearDeathExperience
	[ImplementsPowerSNO(156484)]
	[ImplementsPowerBuff(1)]
	public class NearDeathExperienceCooldownBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(60f);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			Target.Attributes[GameAttributes.Hitpoints_Regen_Bonus_Percent] += 0.35f;
			//random fixed value here, since no bonus_percent attribute for LoH stat, and multiplying base attribute by 35% is unsafe
			Target.Attributes[GameAttributes.Hitpoints_On_Hit] += 1000;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Hitpoints_Regen_Bonus_Percent] -= 0.35f;
			Target.Attributes[GameAttributes.Hitpoints_On_Hit] -= 1000;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
	#region ThrillOfTheHunt
	[ImplementsPowerSNO(211225)]
	[ImplementsPowerBuff(0)]
	public class ThrillOfTheHuntCooldownBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(10f);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			return true;
		}

		public override void Remove()
		{
			base.Remove();
		}
	}
	#endregion
	#region Brooding
	[ImplementsPowerSNO(210801)]
	[ImplementsPowerBuff(0)]
	public class BroodingCooldownBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(3f);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			Target.Attributes[GameAttributes.Hitpoints_Regen_Per_Second] -= Target.Attributes[GameAttributes.Hitpoints_Max_Total] / 100;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Hitpoints_Regen_Per_Second] += Target.Attributes[GameAttributes.Hitpoints_Max_Total] / 100;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
	#region UnstableAnomaly
	[ImplementsPowerSNO(208474)]
	[ImplementsPowerBuff(1)]
	public class UnstableAnomalyCooldownBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(60f);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			return true;
		}

		public override void Remove()
		{
			base.Remove();
		}
	}
	#endregion
	#region GruesomeFeast
	[ImplementsPowerSNO(208594)]
	[ImplementsPowerBuff(0, true)]
	public class GruesomeFeastIntBuff : PowerBuff
	{
		float IntBonus = 0f;
		float _currentBonus = 0f;

		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(15f);
			MaxStackCount = 5;
			IntBonus = (Target as Player).TotalIntelligence / 10f;
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			_currentBonus = IntBonus * StackCount;
			Target.Attributes[GameAttributes.Intelligence] += _currentBonus;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override bool Stack(Buff buff)
		{
			if (!base.Stack(buff))
				return false;

			Target.Attributes[GameAttributes.Intelligence] -= _currentBonus;
			_currentBonus = IntBonus * StackCount;
			Target.Attributes[GameAttributes.Intelligence] += _currentBonus;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Intelligence] -= IntBonus * StackCount;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
	#region VisionQuest
	[ImplementsPowerSNO(209041)]
	[ImplementsPowerBuff(1)]
	public class VisionQuestBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(5f);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			Target.Attributes[GameAttributes.Resource_Regen_Bonus_Percent, 0] += 0.3f;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Resource_Regen_Bonus_Percent, 0] -= 0.3f;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
	#region Provocation
	[ImplementsPowerSNO(209813)]
	[ImplementsPowerBuff(1)]
	public class ProvocationBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(10f);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			Target.Attributes[GameAttributes.Damage_Weapon_Percent_Bonus] += 0.15f;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Damage_Weapon_Percent_Bonus] -= 0.15f;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
	#region CombinationStrike
	/*[ImplementsPowerSNO(218415)]
	[ImplementsPowerBuff(1)]
	public class CombinationStrikeBuff : PowerBuff	//UI icon placeholder. EDIT: icon is broken anyway, commenting this out
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(3f);
		}
	}
	[ImplementsPowerSNO(218415)]*/
	[ImplementsPowerBuff(2)]
	public class CombinationFistsOfThunderBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(3f);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			//if (!HasBuff<CombinationStrikeBuff>(User))
			//AddBuff(User, new CombinationStrikeBuff());

			return true;
		}

		public override void OnPayload(Payload payload)
		{
			if (payload is HitPayload && Target.Equals(payload.Context.User) && (payload as HitPayload).IsWeaponDamage)
				(payload as HitPayload).TotalDamage *= 1.1f;
		}
	}
	[ImplementsPowerSNO(218415)]
	[ImplementsPowerBuff(3)]
	public class CombinationDeadlyReachBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(3f);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			//if (!HasBuff<CombinationStrikeBuff>(User))
			//AddBuff(User, new CombinationStrikeBuff());

			return true;
		}

		public override void OnPayload(Payload payload)
		{
			if (payload is HitPayload && Target.Equals(payload.Context.User) && (payload as HitPayload).IsWeaponDamage)
				(payload as HitPayload).TotalDamage *= 1.1f;
		}
	}
	[ImplementsPowerSNO(218415)]
	[ImplementsPowerBuff(4)]
	public class CombinationCripplingWaveBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(3f);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			//if (!HasBuff<CombinationStrikeBuff>(User))
			//AddBuff(User, new CombinationStrikeBuff());

			return true;
		}

		public override void OnPayload(Payload payload)
		{
			if (payload is HitPayload && Target.Equals(payload.Context.User) && (payload as HitPayload).IsWeaponDamage)
				(payload as HitPayload).TotalDamage *= 1.1f;
		}
	}
	[ImplementsPowerSNO(218415)]
	[ImplementsPowerBuff(5)]
	public class CombinationWayOf100FistBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(3f);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			//if (!HasBuff<CombinationStrikeBuff>(User))
			//AddBuff(User, new CombinationStrikeBuff());

			return true;
		}
		public override void OnPayload(Payload payload)
		{
			if (payload is HitPayload && Target.Equals(payload.Context.User) && (payload as HitPayload).IsWeaponDamage)
				(payload as HitPayload).TotalDamage *= 1.1f;
		}
	}
	#endregion
	#region Sharpshooter
	[ImplementsPowerSNO(155715)]
	[ImplementsPowerBuff(0, true)]
	public class SharpshooterBuff : PowerBuff
	{
		TickTimer _damageTimer = null;
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(5f);
			MaxStackCount = 25;
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			_currentBonus = 0.04f * StackCount;
			Target.Attributes[GameAttributes.Weapon_Crit_Chance] += _currentBonus;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		private float _currentBonus = 0f;

		public override bool Update()
		{
			if (base.Update())
				return true;

			if (_damageTimer == null || _damageTimer.TimedOut)
			{
				_damageTimer = WaitSeconds(1f);
				if ((Target as Player).SkillSet.HasPassive(155715) && HasBuff<SharpshooterBuff>(Target))
					AddBuff(Target, new SharpshooterBuff());
				if (!(Target as Player).SkillSet.HasPassive(155715))
					RemoveBuffs(Target, 155715);
			}
			return false;
		}

		public override bool Stack(Buff buff)
		{
			bool stacked = StackCount < MaxStackCount;

			base.Stack(buff);

			if (!stacked) return true;

			Target.Attributes[GameAttributes.Weapon_Crit_Chance] -= _currentBonus;

			_currentBonus = 0.04f * StackCount;
			Target.Attributes[GameAttributes.Weapon_Crit_Chance] += _currentBonus;
			User.Attributes.BroadcastChangedIfRevealed();

			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Weapon_Crit_Chance] -= _currentBonus;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
	#region Awareness
	[ImplementsPowerSNO(324770)]
	[ImplementsPowerBuff(0, true)]
	public class AwarenessBuff : PowerBuff
	{
		TickTimer _damageTimer = null;
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(5f);
			MaxStackCount = 20;
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			_currentBonus = 0.06f * StackCount;
			Target.Attributes[GameAttributes.Dodge_Chance_Bonus] += _currentBonus;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		private float _currentBonus = 0f;

		public override bool Update()
		{
			if (base.Update())
				return true;

			if (_damageTimer == null || _damageTimer.TimedOut)
			{
				_damageTimer = WaitSeconds(1f);
				if ((Target as Player).SkillSet.HasPassive(324770) && HasBuff<AwarenessBuff>(Target))
					AddBuff(Target, new AwarenessBuff());
				if (!(Target as Player).SkillSet.HasPassive(324770))
					RemoveBuffs(Target, 324770);
			}
			return false;
		}

		public override bool Stack(Buff buff)
		{
			bool stacked = StackCount < MaxStackCount;

			base.Stack(buff);

			if (!stacked) return true;

			Target.Attributes[GameAttributes.Dodge_Chance_Bonus] -= _currentBonus;

			_currentBonus = 0.06f * StackCount;
			Target.Attributes[GameAttributes.Dodge_Chance_Bonus] += _currentBonus;
			Target.Attributes.BroadcastChangedIfRevealed();

			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Dodge_Chance_Bonus] -= _currentBonus;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
	#region Grenadier
	[ImplementsPowerSNO(208779)]
	public class GrenadierBlow : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			SpawnEffect(ActorSno._grenadeproxy_obsidian, User.Position); //big grenade blow

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(User.Position, 10f);
			attack.AddWeaponDamage(10f, DamageType.Fire);
			attack.Apply();

			yield break;
		}
	}
	#endregion
	#region Physical Attunement
	[ImplementsPowerSNO(340910)]
	[ImplementsPowerBuff(1)]
	public class PhysicalAttunementBuff : PowerBuff
	{
		TickTimer _damageTimer = null;

		private float _currentBonus = 0f;

		public override bool Update()
		{
			if (base.Update())
				return true;

			if (_damageTimer == null || _damageTimer.TimedOut)
			{
				_damageTimer = WaitSeconds(1f);

				Target.Attributes[GameAttributes.Resistance, 0] -= _currentBonus;

				_currentBonus = 70f * GetEnemiesInRadius(Target.Position, 20f + Target.Attributes[GameAttributes.Gold_PickUp_Radius]).Actors.Count;
				Target.Attributes[GameAttributes.Resistance, 0] += _currentBonus;
				Target.Attributes.BroadcastChangedIfRevealed();

				if (!(Target as Player).SkillSet.HasPassive(340910))
					RemoveBuffs(Target, 340910);
			}
			return false;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Resistance, 0] -= _currentBonus;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
	#region Mythic Rhythm
	[ImplementsPowerSNO(315271)]
	[ImplementsPowerBuff(0)]
	public class MythicRhythmBuff : PowerBuff
	{
		public override void OnPayload(Payload payload)
		{
			if (payload is HitPayload && payload.Context.User == Target)
			{
				int powerSNO = payload.Context.PowerSNO;
				if (SkillsSystem.Skills.Monk.SpiritSpenders.List.Contains(PowerSNO)) //spirit spender
				{
					(payload as HitPayload).TotalDamage *= 1.4f;
					Target.World.BuffManager.RemoveBuff(Target, this);
				}
			}
		}
	}
	#endregion
	#region The Guardians Path
	[ImplementsPowerSNO(209812)]
	[ImplementsPowerBuff(1)]
	public class GuardiansPathBuff : PowerBuff
	{
		public override void Init() //Placeholder buff, effect done in Player
		{
			base.Init();
		}
	}
	#endregion
	#region Momentum
	[ImplementsPowerSNO(341559)]
	[ImplementsPowerBuff(0)]
	public class MomentumCheckBuff : PowerBuff
	{
		TickTimer CheckTimer = null;
		int Momentum = 0;
		public override void Init()
		{
			base.Init();
		}
		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			return true;
		}
		public override bool Update()
		{
			if (base.Update())
				return true;

			if (CheckTimer == null || CheckTimer.TimedOut)
			{
				CheckTimer = WaitSeconds(0.5f);

				if ((Target as Player).InGameClient.Game.TickCounter - (Target as Player).LastMovementTick <= 30) Momentum++;
				else Momentum = 0;

				if (Momentum > 4)
				{
					Momentum = 0;
					if (!HasBuff<MomentumDamageBuff>(Target))
						AddBuff(Target, new MomentumDamageBuff());
				}
			}
			return false;
		}

		public override void Remove()
		{
			base.Remove();
		}
	}
	[ImplementsPowerSNO(341559)]
	[ImplementsPowerBuff(1)]
	public class MomentumDamageBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(4f);
		}
		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			User.Attributes[GameAttributes.Damage_Weapon_Percent_Bonus] += 0.15f;
			User.Attributes[GameAttributes.Damage_Percent_All_From_Skills] += 0.15f;
			User.Attributes.BroadcastChangedIfRevealed();

			return true;
		}

		public override void Remove()
		{
			base.Remove();

			User.Attributes[GameAttributes.Damage_Weapon_Percent_Bonus] -= 0.15f;
			User.Attributes[GameAttributes.Damage_Percent_All_From_Skills] -= 0.15f;
			User.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
	#region Epiphany
	[ImplementsPowerSNO(312307)]
	[ImplementsPowerBuff(7)]
	class EpiphanyDashMoverBuff : PowerBuff
	{
		private Vector3D _destination;
		private ActorMover _mover;

		public EpiphanyDashMoverBuff(Vector3D destination)
		{
			_destination = destination;
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			Logger.Debug("DashMover Called");
			// dash speed seems to always be actor speed * 10
			float speed = Target.Attributes[GameAttributes.Running_Rate_Total] * 9f;

			Target.TranslateFacing(_destination, true);
			_mover = new ActorMover(Target);
			_mover.Move(_destination, speed, new ACDTranslateNormalMessage
			{
				SnapFacing = true,
				MoveFlags = 0x9206, // alt: 0x920e, not sure what this param is for.
				AnimationTag = 90160,
				WalkInPlaceTicks = 6, // ticks to wait before playing attack animation
			});

			// make sure buff timeout is big enough otherwise the client will sometimes ignore the visual effects.
			Timeout = _mover.ArrivalTime;

			Target.Attributes[GameAttributes.Hidden] = true;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Hidden] = false;
			Target.Attributes.BroadcastChangedIfRevealed();
		}

		public override bool Update()
		{
			_mover.Update();

			return base.Update();
		}
	}
	#endregion

	#region OnlyOne
	[ImplementsPowerSNO(470725)]
	[ImplementsPowerBuff(1)]
	public class OnlyOne : PowerBuff
	{
		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			Target.Attributes[GameAttributes.Armor_Bonus_Percent] += 1;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Armor_Bonus_Percent] -= 1;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
	#endregion
}
