﻿//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
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

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads
{
	public class AttackPayload : Payload
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		// list of targets to try and hit with this payload, must be set before calling Apply()
		public TargetList Targets;
		public float chcBonus = 0f;

		// list of each amount and type of damage the attack will contain
		public class DamageEntry
		{
			public DamageType DamageType;

			public float MinDamage;
			public float DamageDelta;

			public bool IsWeaponBasedDamage;
			public float WeaponDamageMultiplier;
		}
		public List<DamageEntry> DamageEntries = new List<DamageEntry>();

		public Action<HitPayload> OnHit = null;
		public Action<DeathPayload> OnDeath = null;

		// some power's use custom hit effects, but they're not specified correctly in the tagmaps it seems
		// so this can be set to false to force off all hit effect generation from the payload
		public bool AutomaticHitEffects = true;

		private List<Func<Buff>> _hitBuffs = new List<Func<Buff>>();

		public AttackPayload(PowerContext context)
			: base(context, context.User)
		{
		}

		public void AddDamage(float minDamage, float damageDelta, DamageType damageType)
		{
			DamageEntries.Add(new DamageEntry
			{
				DamageType = damageType,
				MinDamage = minDamage,
				DamageDelta = damageDelta,
				IsWeaponBasedDamage = false,
			});
		}

		public void AddWeaponDamage(float damageMultiplier, DamageType damageType)
		{
			DamageEntries.Add(new DamageEntry
			{
				DamageType = damageType,
				IsWeaponBasedDamage = true,
				WeaponDamageMultiplier = damageMultiplier,
			});
		}

		public void SetSingleTarget(Actor target)
		{
			Targets = new TargetList();
			Targets.Actors.Add(target);
		}

		public void AddBuffOnHit<T>() where T : Buff, new()
		{
			_hitBuffs.Add(() => new T());
		}

		public void Apply()
		{
			if (Targets == null) Targets = new TargetList();
			if (Target.World != null)
			{
				if (!Target.World.Game.Working) return;
				Target.World.BuffManager.SendTargetPayload(Target, this);
				
				if (Context.User != null) Target.World.BuffManager.SendTargetPayload(Context.User, this);
			}
			if (new System.Diagnostics.StackTrace().FrameCount > 35)
			{
				return;
			}
			if (Target is Player && DamageEntries.Count > 0)
			{
				Player player = (Player)Target;
				foreach (Actor extra in Targets.ExtraActors)
					if (extra is DesctructibleLootContainer)
						extra.OnTargeted(player, null);

			}
            if (Context.User is Player && Context.Target is Monster && Context.Target.GBHandle.Type == 1)
            {
                (Context.User as Player).ExpBonusData.MonsterAttacked((Context.User as Player).InGameClient.Game.TickCounter);
                ((Context.Target as Monster).Brain as AISystem.Brains.MonsterBrain).AttackedBy = Context.User;
            }
            
			foreach (Actor target in Targets.Actors)
			{
				if (target == null || target.World == null || target.World != null && target.World.PowerManager.IsDeletingActor(target))
					continue;

				var payload = new HitPayload(this, _DoCriticalHit(Context.User, target, chcBonus)
					, target);
				payload.AutomaticHitEffects = AutomaticHitEffects;
				payload.OnDeath = OnDeath;

				foreach (Func<Buff> buffFactory in _hitBuffs)
					Context.AddBuff(target, buffFactory());
				if (payload.Successful)
				{
					try
					{
						if (OnHit != null && AutomaticHitEffects)
							OnHit(payload);
					}
					catch { }
					payload.Apply();
				}
			}
		}

		private bool _DoCriticalHit(Actor user, Actor target, float chcBonus = 0f)
		{
			if (target.Attributes[GameAttribute.Ignores_Critical_Hits]) return false;

			//Monk -> Exploding Palm
			if (Context.PowerSNO == 97328 && Context.Rune_E <= 0) return false;

			float additionalCritChance = chcBonus;

			if (user is Player && (user as Player).SkillSet.HasPassive(338859)) //Single Out
				if (target.GetMonstersInRange(20f).Where(m => m != target).Count() == 0)
					additionalCritChance += 0.25f;

			//Wizard -> Spectral Blade -> Ice Blades
			if (target.World.BuffManager.HasBuff<Implementations.WizardSpectralBlade.BladesChcDebuff>(target))
				additionalCritChance += target.World.BuffManager.GetFirstBuff<Implementations.WizardSpectralBlade.BladesChcDebuff>(target).Percentage;

			//Crusader -> Judgment -> Resolved
			if (target.World.BuffManager.HasBuff<Implementations.CrusaderJudgment.JudgedDebuffRooted>(target))
				additionalCritChance += target.World.BuffManager.GetFirstBuff<Implementations.CrusaderJudgment.JudgedDebuffRooted>(target).bonusChC;

			//Crusader -> Punish -> Fury
			if (target.World.BuffManager.HasBuff<Implementations.CrusaderPunish.FuryChCBuff>(target))
			{
				additionalCritChance += target.World.BuffManager.GetFirstBuff<Implementations.CrusaderPunish.FuryChCBuff>(target).Percentage;
				target.World.BuffManager.RemoveBuffs(target, SkillsSystem.Skills.Crusader.FaithGenerators.Punish);
			}

			var totalCritChance = user.Attributes[GameAttribute.Weapon_Crit_Chance] + user.Attributes[GameAttribute.Crit_Percent_Bonus_Capped] + user.Attributes[GameAttribute.Crit_Percent_Bonus_Uncapped] + user.Attributes[GameAttribute.Power_Crit_Percent_Bonus, Context.PowerSNO] + target.Attributes[GameAttribute.Bonus_Chance_To_Be_Crit_Hit] + additionalCritChance;
			if (totalCritChance > 0.85f) totalCritChance = 0.85f;
			return PowerContext.Rand.NextDouble() < totalCritChance;
		}
	}
}
