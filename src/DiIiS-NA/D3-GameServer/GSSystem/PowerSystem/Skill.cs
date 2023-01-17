//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
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

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public abstract class Skill : PowerScript
	{
		// main handler called to generate effects for skill
		public abstract IEnumerable<TickTimer> Main();

		static new readonly Logger Logger = LogManager.CreateLogger();
		public DiIiS_NA.Core.MPQ.FileFormats.Power DataOfSkill;
		public sealed override IEnumerable<TickTimer> Run()
		{
			DataOfSkill = (DiIiS_NA.Core.MPQ.FileFormats.Power)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.Power][PowerSNO].Data;
			
			// play starting animation and effects
			_PlayActionAnimation();
			_PlayCastEffect();

			float contactDelay = GetContactDelay();
			if (contactDelay > 0f)
				yield return WaitSeconds(contactDelay);

			_PlayContactEffect();

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
		public virtual int GetActionAnimationSNO()
		{
			try
			{
				int tag = EvalTag(PowerKeys.AnimationTag);
				if (User.AnimationSet != null && User.AnimationSet.Animations.ContainsKey(tag))
					return User.AnimationSet.Animations[tag];
				else
					if (User.AnimationSet != null)
						return User.AnimationSet.GetAnimationTag(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Attack2);
					else
						return -1;
			}
			catch
			{
				return -1;
			}
			

		}

		public virtual float GetActionSpeed()
		{
			return EvalTag(PowerKeys.AttackSpeed);
		}

		public virtual int GetCastEffectSNO()
		{
			return EvalTag(IsUserFemale ? PowerKeys.CastingEffectGroup_Female : PowerKeys.CastingEffectGroup_Male);
		}

		public virtual int GetContactEffectSNO()
		{
			return EvalTag(IsUserFemale ? PowerKeys.ContactFrameEffectGroup_Female : PowerKeys.ContactFrameEffectGroup_Male);
		}

		public virtual float GetContactDelay()
		{
			return 0f;
		}

		public float GetCooldown()
		{
			if (EvalTag(PowerKeys.CooldownTime) > 0f)
				return EvalTag(PowerKeys.CooldownTime);
			else
				return 1f;
		}

		private void _PlayActionAnimation()
		{
			float speed = GetActionSpeed();
			int animationSNO = GetActionAnimationSNO();
			#region Патч анимаций
			if(animationSNO == -1)
			switch (this.User.SNO)
			{
					case ActorSno._x1_skeletonarcher_westmarch_a: //x1_SkeletonArcher_Westmarch_A
						if (this.PowerSNO == 30334)
							animationSNO = 303905;
						break;
					case ActorSno._p6_necro_skeletonmage_f_archer: //p6_necro_skeletonMage_F_archer
						animationSNO = 303905;
						speed = 2f;
						break;
			}
			#endregion
			
			if (animationSNO != -1 && speed > 0f)
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
								AnimationSNO = animationSNO,
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
							AnimationSNO = animationSNO,
							PermutationIndex = 0x0,
							AnimationTag = 0,
							Speed = User.SNO == ActorSno._pt_blacksmith_nonvendor || User.SNO == ActorSno._leah ? 1 : speed,
						}
					}
					}, User);
				}
			}
		}

		private void _PlayCastEffect()
		{
			int sno = GetCastEffectSNO();
			if (sno != -1)
				User.PlayEffectGroup(sno);
		}

		private void _PlayContactEffect()
		{
			int sno = GetContactEffectSNO();
			if (sno != -1)
				User.PlayEffectGroup(sno);
		}
	}
}
