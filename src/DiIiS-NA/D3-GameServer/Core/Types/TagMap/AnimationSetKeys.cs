using System.Collections.Generic;
using System.Reflection;

namespace DiIiS_NA.GameServer.Core.Types.TagMap
{
	public class AnimationSetKeys
	{
		#region compile a dictionary to access keys from ids. If you need a readable name for a TagID, look up its key and get its name
		private static Dictionary<int, TagKey> tags = new Dictionary<int, TagKey>();

		public static TagKey GetKey(int index)
		{
			return tags.ContainsKey(index) ? tags[index] : null;
		}

		static AnimationSetKeys()
		{
			foreach (FieldInfo field in typeof(AnimationSetKeys).GetFields())
			{
				TagKey key = field.GetValue(null) as TagKey;
				key.Name = field.Name;
				tags.Add(key.ID, key);
			}
		}
		#endregion


		public static TagKeyInt GenericCast = new TagKeyInt(262144);
		public static TagKeyInt IdleDefault = new TagKeyInt(69632);
		public static TagKeyInt Idle = new TagKeyInt(69968);

		public static TagKeyInt Flee = new TagKeyInt(70768);
		public static TagKeyInt Spawn = new TagKeyInt(70097);
		public static TagKeyInt KnockBackLand = new TagKeyInt(71176);
		public static TagKeyInt KnockBackMegaOuttro = new TagKeyInt(71218);
		public static TagKeyInt KnockBack = new TagKeyInt(71168);
		public static TagKeyInt KnockBackMegaIntro = new TagKeyInt(71216);
		public static TagKeyInt Ambush = new TagKeyInt(70144);
		public static TagKeyInt RangedAttack = new TagKeyInt(69840);
		public static TagKeyInt DeathDefault = new TagKeyInt(69648);
		public static TagKeyInt GetHit = new TagKeyInt(69664);
		public static TagKeyInt Dead1 = new TagKeyInt(79168);
		public static TagKeyInt Dead2 = new TagKeyInt(79152);
		public static TagKeyInt Dead3 = new TagKeyInt(77920);
		public static TagKeyInt Dead4 = new TagKeyInt(77888);
		public static TagKeyInt Dead5 = new TagKeyInt(77904);
		public static TagKeyInt Dead6 = new TagKeyInt(77872);
		public static TagKeyInt Dead7 = new TagKeyInt(77856);
		public static TagKeyInt Dead8 = new TagKeyInt(77840);
		public static TagKeyInt SpecialDead = new TagKeyInt(71440);
		public static TagKeyInt Run = new TagKeyInt(69728);
		public static TagKeyInt TownRun = new TagKeyInt(69736);
		public static TagKeyInt Walk = new TagKeyInt(69744);
		public static TagKeyInt Attack = new TagKeyInt(69776);
		public static TagKeyInt Attack2 = new TagKeyInt(69792);
		public static TagKeyInt SpecialAttack = new TagKeyInt(69904);
		public static TagKeyInt Explode = new TagKeyInt(69920);

		public static TagKeyInt GizmoState1 = new TagKeyInt(70160);
		public static TagKeyInt UndeadEating = new TagKeyInt(270336);

		public static TagKeyInt DeathLightning = new TagKeyInt(73760);
		public static TagKeyInt DeathPoison = new TagKeyInt(73792);
		public static TagKeyInt DeathDisintegration = new TagKeyInt(73808);
		public static TagKeyInt DeathDecap = new TagKeyInt(73840);
		public static TagKeyInt DeathAcid = new TagKeyInt(73984);
		public static TagKeyInt DeathArcane = new TagKeyInt(73776);
		public static TagKeyInt DeathFire = new TagKeyInt(73744);
		public static TagKeyInt DeathPlague = new TagKeyInt(73856);
		public static TagKeyInt DeathDismember = new TagKeyInt(73872);
		public static TagKeyInt DeadDefault = new TagKeyInt(69712);
		public static TagKeyInt DeathPulverise = new TagKeyInt(73824);
		public static TagKeyInt DeathCold = new TagKeyInt(74016);
		public static TagKeyInt DeathLava = new TagKeyInt(74032);
		public static TagKeyInt DeathHoly = new TagKeyInt(74048);
		public static TagKeyInt DeathSpirit = new TagKeyInt(74064);
		public static TagKeyInt DeathFlyingOrDefault = new TagKeyInt(71424);
		public static TagKeyInt Spawn2 = new TagKeyInt(291072);
		public static TagKeyInt Despawn = new TagKeyInt(410369);
		public static TagKeyInt Stunned = new TagKeyInt(69680);

		public static TagKeyInt EmoteCheer = new TagKeyInt(410112);
		public static TagKeyInt EmoteShrugQuestion = new TagKeyInt(410113);
		public static TagKeyInt EmoteCower = new TagKeyInt(410114);
		public static TagKeyInt EmoteExclamationShout = new TagKeyInt(410115);
		public static TagKeyInt EmoteLaugh = new TagKeyInt(410116);
		public static TagKeyInt EmotePoint = new TagKeyInt(410117);
		public static TagKeyInt EmoteSad = new TagKeyInt(410118);
		public static TagKeyInt EmoteTalk = new TagKeyInt(410119);
		public static TagKeyInt EmoteIdle = new TagKeyInt(410120);
		public static TagKeyInt EmoteUse = new TagKeyInt(410121);
		public static TagKeyInt EmoteGreet = new TagKeyInt(410128);
		public static TagKeyInt EmoteUseLoop = new TagKeyInt(410129);


		public static TagKeyInt EmoteNo = new TagKeyInt(86272);
		public static TagKeyInt EmoteWave = new TagKeyInt(86274);
		public static TagKeyInt EmoteYes = new TagKeyInt(86275);



		public static TagKeyInt Opening = new TagKeyInt(70416);
		public static TagKeyInt Open = new TagKeyInt(70432);
		public static TagKeyInt Closing = new TagKeyInt(70448);

		public static TagKeyInt RootBreak = new TagKeyInt(196608);




		public static TagKeyInt HTHParry = new TagKeyInt(70037);


	}
}
