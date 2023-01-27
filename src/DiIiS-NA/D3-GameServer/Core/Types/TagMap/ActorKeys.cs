using System.Collections.Generic;
using System.Reflection;

namespace DiIiS_NA.GameServer.Core.Types.TagMap
{
	class ActorKeys
	{
		#region compile a dictionary to access keys from ids. If you need a readable name for a TagID, look up its key and get its name
		private static Dictionary<int, TagKey> tags = new Dictionary<int, TagKey>();

		public static TagKey GetKey(int index)
		{
			return tags.ContainsKey(index) ? tags[index] : null;
		}

		static ActorKeys()
		{
			foreach (FieldInfo field in typeof(ActorKeys).GetFields())
			{
				TagKey key = field.GetValue(null) as TagKey;
				key.Name = field.Name;
				tags.Add(key.ID, key);
			}
		}
		#endregion

		public static TagKeyFloat Scale = new TagKeyFloat(65543);
		public static TagKeyInt TeamID = new TagKeyInt(65556);  // TODO this is not team id
		public static TagKeySNO FlippyParticle = new TagKeySNO(65655);
		public static TagKeySNO Flippy = new TagKeySNO(65688);

		public static TagKeySNO Script = new TagKeySNO(65907);

		public static TagKeySNO SoundFootstepGrass = new TagKeySNO(66048);
		public static TagKeySNO SoundFootstepDirt1 = new TagKeySNO(66049);
		public static TagKeySNO SoundFootstepStone1 = new TagKeySNO(66050);
		public static TagKeySNO SoundFootstepMetal1 = new TagKeySNO(66051);
		public static TagKeySNO SoundFootstepWater1 = new TagKeySNO(66052);
		public static TagKeySNO SoundFootstepStone2 = new TagKeySNO(66053);
		public static TagKeySNO SoundFootstepDirt2 = new TagKeySNO(66054);
		public static TagKeySNO SoundFootstepWater2 = new TagKeySNO(66055);
		public static TagKeySNO SoundFootstepMetal2 = new TagKeySNO(66056);
		public static TagKeySNO SoundFootstepStone3 = new TagKeySNO(66057);
		public static TagKeySNO SoundFootstepWater3 = new TagKeySNO(66058);
		public static TagKeySNO SoundFootstepWood = new TagKeySNO(66059);
		public static TagKeySNO SoundFootstepDirt3 = new TagKeySNO(66060);
		public static TagKeySNO SoundFootstepBone = new TagKeySNO(66061);
		public static TagKeySNO SoundFootstepSnow = new TagKeySNO(66062);
		public static TagKeySNO SoundFootstepWater4 = new TagKeySNO(66063);

		public static TagKeySNO SoundFootstepCarpet = new TagKeySNO(69644);


		public static TagKeySNO Projectile = new TagKeySNO(66138);



		public static TagKeyGizmoGroup GizmoGroup = new TagKeyGizmoGroup(66305);
		public static TagKeyInt DeathAnimationTag = new TagKeyInt(66308); // this is probably not the correct name since it is used only on ravens, that have a 'monster' attached to it. it may be something more general

		public static TagKeySNO LootTreasureClass = new TagKeySNO(66384);
		public static TagKeySNO ActivationPower = new TagKeySNO(66400);
		public static TagKeySNO WarpProxy = new TagKeySNO(66472);


		public static TagKeySNO Lore = new TagKeySNO(67331);

		public static TagKeySNO MinimapMarker = new TagKeySNO(458752);

		public static TagKeySNO FireEffectGroup = new TagKeySNO(74064);
		public static TagKeySNO ColdEffectGroup = new TagKeySNO(74065);
		public static TagKeySNO LightningEffectGroup = new TagKeySNO(74066);
		public static TagKeySNO PoisonEffectGroup = new TagKeySNO(74067);
		public static TagKeySNO ArcaneEffectGroup = new TagKeySNO(74068);

		public static TagKeySNO LifeStealEffectGroup = new TagKeySNO(74070);
		public static TagKeySNO ManaStealEffectGroup = new TagKeySNO(74071);
		public static TagKeySNO MagicFindEffectGroup = new TagKeySNO(74072);
		public static TagKeySNO GoldFindEffectGroup = new TagKeySNO(74073);
		public static TagKeySNO AttackEffectGroup = new TagKeySNO(74074);
		public static TagKeySNO CastEffectGroup = new TagKeySNO(74075);
		public static TagKeySNO HolyEffectGroup = new TagKeySNO(74076);
		public static TagKeySNO Spell1EffectGroup = new TagKeySNO(74077);
		public static TagKeySNO Spell2EffectGroup = new TagKeySNO(74078);

		public static TagKeySNO SoundImpactSword = new TagKeySNO(90112);
		public static TagKeySNO SoundImpactBlow = new TagKeySNO(90113);
		public static TagKeySNO SoundImpactHtH = new TagKeySNO(90114);
		public static TagKeySNO SoundImpactArrow = new TagKeySNO(90115);


	}

	public class TagKeyGizmoGroup : TagKey { public TagKeyGizmoGroup(int id) : base(id) { } public GizmoGroup GetValue(TagMapEntry entry) { return (GizmoGroup)entry.Int; } }

	public enum GizmoGroup
	{
		Passive = -1,
		Door = 0,
		LootContainer = 1,
		Portal = 2,  // whichdoctor_fetisharmy also has this set despite beeing client effects
		Waypoint = 4,
		CheckPoint = 7,
		Sign = 8,
		Healthwell = 9,
		Shrine = 10,    // and actor\MinimapIconStairs_Switch.acr
		TownPortal = 11,
		HearthPortal = 12,
		Headstone = 18,
		ServerProp = 19,    // mostly set for server props and for actors that have a controling function (conductorproxymaster, markerlocation, nospawn20feet, etc)
		StartLocations = 20, // and exit locations
		CathedralIdol = 22, // only one actor with that name
		DestructibleLootContainer = 23,
		PlayerSharedStash = 25,
		Spawner = 28,
		Trigger = 44,
		Destructible = 48,
		Barricade = 56,
		ScriptObject = 57,  // Actor\TEMP_SkeletonPortal_Center.acr, Gizmo Actor\SkeletonKingGizmo.acr, Gizmo, Actor\TEMP_GoatPortal_Center.acr, Gizmo, Actor\Temp_Story_Trigger_Enabled.acr, Gizmo, Actor\trOut_fields_Cart_Fixable.acr, Gizmo, Actor\Temp_FesteringWoodsAmbush_Switch.acr, Gizmo, Actor\trOut_Wilderness_Skeleton_Chair_Switch.acr, 
		GateGizmo = 59,
		ProximityTriggered = 60,  // raven pecking, wall collapse... triggered when player approaches
		ActChangeTempObject = 62, // only one actor with that name
		Unknown = 63,
		Banner = 64,
		Readable = 65,
		BossPortal = 66,
		QuestLoot = 67, // only Actor\PlacedGold.acr, Gizmo and Actor\Scoundrel_LoreLoot.acr, Gizmo
		Savepoint = 68,
		DungeonStonePortal = 70, // only one actor with that name
		NephalemAltar = 71,
		LootRunObelisk = 78,
		ExpPool = 79,
		NecroCorpse = 84,
	}
	class DRLGCommandKeys
	{
		public static class Group
		{
			//	[0]: {851986 = -1}
			//	[1]: {1015841 = 1}
			//	[2]: {851987 = -1}
			//	[3]: {851993 = -1}
			//	[4]: {1015822 = 0}
			//	[5]: {851983 = 19780} //19780 LevelArea A1_trDun_Level01
			public static TagKeySNO Level = new TagKeySNO(851983);
		}

		public static class AddExit
		{
			//[0]: {852000 = -1}	Type SNO (2)
			//[1]: {851984 = 60713} Type SNO (2) [20:16] (snobot) [1] 60713 Worlds trDun_Cain_Intro, 
			//[2]: {1020032 = 1}	(0)
			//[3]: {852050 = 0}	 //Starting location? ID (7)
			//[4]: {1015841 = 1}	(0)
			//[5]: {852051 = 172}   //Starting location? ID (7)
			//[6]: {1015814 = 0}	(0)
			//[7]: {854612 = -1}	Type SNO (2)
			//[8]: {1015813 = 300}  (0) tiletype (exit)
			//[9]: {1020416 = 1}	(0)
			//[10]: {854613 = -1}   (2)
			//[11]: {1015821 = -1}  (0)
			public static TagKeySNO ExitWorld = new TagKeySNO(851984);
			public static TagKeySNO CoordinateX = new TagKeySNO(852050); //??
			public static TagKeySNO CoordinateY = new TagKeySNO(852051); //??
			public static TagKeySNO TileType = new TagKeySNO(1015813);

		}
	}
}
