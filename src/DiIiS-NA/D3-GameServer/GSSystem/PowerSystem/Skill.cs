using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public abstract class Skill : PowerScript
	{
		// main handler called to generate effects for skill
		public abstract IEnumerable<TickTimer> Main();

		static new readonly Logger Logger = LogManager.CreateLogger();
		public Power DataOfSkill;
		public sealed override IEnumerable<TickTimer> Run()
		{
			DataOfSkill = (Power)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.Power][PowerSNO].Data;
			
			// play starting animation and effects
			PlayActionAnimation();
			PlayCastEffect();

			float contactDelay = GetContactDelay();
			if (contactDelay > 0f)
				yield return WaitSeconds(contactDelay);

			PlayContactEffect();

			// run main effects script
			foreach (TickTimer timer in Main())
				yield return timer;

			float cooldown = GetCooldown();
			if (cooldown > 0f)
				yield return WaitSeconds(cooldown);
		}

		public bool IsUserFemale
		{
			get
			{
				return User is Player && (User as Player).Toon.Gender == 2;  // 2 = female
			}
		}
		public virtual AnimationSno GetActionAnimationSNO()
		{
			try
			{
				int tag = EvalTag(PowerKeys.AnimationTag);
				if (User.AnimationSet != null)
                {
	                try
	                {
		                if (User.AnimationSet.Animations.TryGetValue(tag, out AnimationSno value))
			                return value;
		                return (AnimationSno)User.AnimationSet.GetAnimationTag(AnimationTags.Attack2);
	                }
	                catch (Exception ex)
	                {
		                #if DEBUG
		                Logger.ErrorException(ex, "GetActionAnimationSNO throws error");
		                #endif
		                return AnimationSno._NONE;
	                }
                }
			}
			catch (Exception e)
			{
				Logger.Error("GetActionAnimationSNO throws error {0}", e.Message);
			}
			return AnimationSno._NONE;

		}

		public virtual float GetActionSpeed() => EvalTag(PowerKeys.AttackSpeed);

		public virtual int GetCastEffectSNO() => EvalTag(IsUserFemale ? PowerKeys.CastingEffectGroup_Female : PowerKeys.CastingEffectGroup_Male);

		public virtual int GetContactEffectSNO() => EvalTag(IsUserFemale ? PowerKeys.ContactFrameEffectGroup_Female : PowerKeys.ContactFrameEffectGroup_Male);

		public virtual float GetContactDelay() => 0f;

		public float GetCooldown()
		{
			var tag = EvalTag(PowerKeys.CooldownTime);
			return tag > 0f ? tag : 1f;
		}

		private void PlayActionAnimation()
		{
			float speed = GetActionSpeed();
			var animationSNO = GetActionAnimationSNO();
			#region Патч анимаций
			if(animationSNO == AnimationSno._NONE)
				switch (User.SNO)
				{
						case ActorSno._x1_skeletonarcher_westmarch_a: //x1_SkeletonArcher_Westmarch_A
							if (PowerSNO == 30334)
								animationSNO = AnimationSno.x1_skeletonarcher_westmarch_attack_01;
							break;
						case ActorSno._p6_necro_skeletonmage_f_archer: //p6_necro_skeletonMage_F_archer
							animationSNO = AnimationSno.x1_skeletonarcher_westmarch_attack_01;
							speed = 2f;
							break;
				}
			#endregion
			
			if (animationSNO != AnimationSno._NONE && speed > 0f)
			{
				if (User is Player)
				{
					User.World.BroadcastExclusive(plr => new PlayAnimationMessage
					{
						ActorID = User.DynamicID(plr),
						AnimReason = 3,
						UnitAniimStartTime = 0,
						tAnim = new PlayAnimationMessageSpec[]
						{
							new PlayAnimationMessageSpec
							{
								Duration = (int)(60f / speed),  // ticks
								AnimationSNO = (int)animationSNO,
								PermutationIndex = 0x0,
								AnimationTag = 0,
								Speed = 1,
							}
						}
					}, User);
					
				}
				else
				{
					if (User is ActorSystem.Minion)
						speed = 1;
					User.World.BroadcastIfRevealed(plr => new PlayAnimationMessage
					{
						ActorID = User.DynamicID(plr),
						AnimReason = User.SNO == ActorSno._pt_blacksmith_nonvendor ? 3 : 3,
						UnitAniimStartTime = 0,
						tAnim = new PlayAnimationMessageSpec[]
					{
						new PlayAnimationMessageSpec
						{
							Duration = (int)(60f / speed),  // ticks
							AnimationSNO = (int)animationSNO,
							PermutationIndex = 0x0,
							AnimationTag = 0,
							Speed = User.SNO == ActorSno._pt_blacksmith_nonvendor || User.SNO == ActorSno._leah ? 1 : speed,
						}
					}
					}, User);
				}
			}
		}

		private void PlayCastEffect()
		{
			int sno = GetCastEffectSNO();
			if (sno != -1)
				User.PlayEffectGroup(sno);
		}

		private void PlayContactEffect()
		{
			int sno = GetContactEffectSNO();
			if (sno != -1)
				User.PlayEffectGroup(sno);
		}
	}
}
