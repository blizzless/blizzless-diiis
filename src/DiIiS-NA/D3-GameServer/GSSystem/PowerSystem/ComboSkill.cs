using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public abstract class ComboSkill : Skill
	{
		public int ComboIndex => TargetMessage.ComboLevel;

		public override AnimationSno GetActionAnimationSNO()
		{
			int tag;
			switch (ComboIndex)
			{
				case 0:
					tag = EvalTag(PowerKeys.ComboAnimation1);
					break;
				case 1:
					tag = EvalTag(PowerKeys.ComboAnimation2); 
					break;
				case 2:
					tag = EvalTag(PowerKeys.ComboAnimation3);
					break;
				default: 
					return AnimationSno._NONE;
			}

			if (User.AnimationSet.Animations.ContainsKey(tag))
				return User.AnimationSet.Animations[tag];
			else
				return AnimationSno._NONE;
		}

		public override float GetActionSpeed()
		{
			switch (ComboIndex)
			{
				case 0: return EvalTag(PowerKeys.ComboAttackSpeed1);
				case 1: return EvalTag(PowerKeys.ComboAttackSpeed2);
				case 2: return EvalTag(PowerKeys.ComboAttackSpeed3);
				default: return 0f;
			}
		}

		public override int GetCastEffectSNO()
		{
			if (IsUserFemale)
			{
				switch (ComboIndex)
				{
					case 0: return EvalTag(PowerKeys.Combo0CastingEffectGroup_Female);
					case 1: return EvalTag(PowerKeys.Combo1CastingEffectGroup_Female);
					case 2: return EvalTag(PowerKeys.Combo2CastingEffectGroup_Female);
					default: return -1;
				}
			}
			else
			{
				switch (ComboIndex)
				{
					case 0: return EvalTag(PowerKeys.Combo0CastingEffectGroup_Male);
					case 1: return EvalTag(PowerKeys.Combo1CastingEffectGroup_Male);
					case 2: return EvalTag(PowerKeys.Combo2CastingEffectGroup_Male);
					default: return -1;
				}
			}
		}

		public override int GetContactEffectSNO()
		{
			if (IsUserFemale)
			{
				switch (ComboIndex)
				{
					case 0: return EvalTag(PowerKeys.Combo0ContactFrameEffectGroup_Female);
					case 1: return EvalTag(PowerKeys.Combo1ContactFrameEffectGroup_Female);
					case 2: return EvalTag(PowerKeys.Combo2ContactFrameEffectGroup_Female);
					default: return -1;
				}
			}
			else
			{
				switch (ComboIndex)
				{
					case 0: return EvalTag(PowerKeys.Combo0ContactFrameEffectGroup_Male);
					case 1: return EvalTag(PowerKeys.Combo1ContactFrameEffectGroup_Male);
					case 2: return EvalTag(PowerKeys.Combo2ContactFrameEffectGroup_Male);
					default: return -1;
				}
			}
		}

		public override float GetContactDelay()
		{
			// only have a contact delay if the action speed is >0 and there is a contact effect specified
			float actionSpeed = GetActionSpeed();
			if (actionSpeed > 0f && GetContactEffectSNO() != -1)
				return 0.5f / actionSpeed;
			else
				return 0f;
		}
	}
}
