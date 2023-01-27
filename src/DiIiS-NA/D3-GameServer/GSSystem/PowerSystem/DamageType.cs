using DiIiS_NA.GameServer.Core.Types.TagMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public class DamageType
	{
		public enum HitEffectType : int
		{
			Physical = 0,
			Fire = 1,
			Lightning = 2,
			Cold = 3,
			Poison = 4,
			Arcane = 5,
			Holy = 6,
			UnknownFlicker = 7
		}

		public HitEffectType HitEffect;
		public int AttributeKey;  // GameAttributeMap key for a given damage type
		public TagKeyInt DeathAnimationTag;

		public static readonly DamageType Physical = new DamageType
		{
			HitEffect = HitEffectType.Physical,
			AttributeKey = 0,
			DeathAnimationTag = AnimationSetKeys.DeathDefault,
		};
		public static readonly DamageType Arcane = new DamageType
		{
			HitEffect = HitEffectType.Arcane,
			AttributeKey = 5,
			DeathAnimationTag = AnimationSetKeys.DeathArcane,
		};
		public static readonly DamageType Cold = new DamageType
		{
			HitEffect = HitEffectType.Cold,
			AttributeKey = 3,
			DeathAnimationTag = AnimationSetKeys.DeathCold,
		};
		public static readonly DamageType Fire = new DamageType
		{
			HitEffect = HitEffectType.Fire,
			AttributeKey = 1,
			DeathAnimationTag = AnimationSetKeys.DeathFire,
		};
		public static readonly DamageType Lightning = new DamageType
		{
			HitEffect = HitEffectType.Lightning,
			AttributeKey = 2,
			DeathAnimationTag = AnimationSetKeys.DeathLightning,
		};
		public static readonly DamageType Poison = new DamageType
		{
			HitEffect = HitEffectType.Poison,
			AttributeKey = 4,
			DeathAnimationTag = AnimationSetKeys.DeathPoison,
		};
		public static readonly DamageType Holy = new DamageType
		{
			HitEffect = HitEffectType.Holy,
			AttributeKey = 6,
			DeathAnimationTag = AnimationSetKeys.DeathHoly,
		};

		public static readonly DamageType[] AllTypes = new DamageType[]
		{
			Physical,
			Arcane,
			Cold,
			Fire,
			Lightning,
			Poison,
			Holy
		};
	}
}
