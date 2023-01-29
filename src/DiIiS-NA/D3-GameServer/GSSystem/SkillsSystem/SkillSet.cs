using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using DiIiS_NA.LoginServer.Toons;
using System;
using System.Linq;

namespace DiIiS_NA.GameServer.GSSystem.SkillsSystem
{
	public class SkillSet
	{
		public ToonClass @Class;
		public Toon Toon { get; private set; }
		public Player Player { get; private set; }

		public ActiveSkillSavedData[] ActiveSkills;
		public HotbarButtonData[] HotBarSkills;
		public int[] PassiveSkills;

		protected static readonly Logger Logger = LogManager.CreateLogger();

		public SkillSet(Player player, ToonClass @class, Toon toon)
		{
			@Class = @class;
			Player = player;
			var dbToon = player.Toon.DBToon;
			var dbActiveSkills = player.Toon.DBActiveSkills;
			ActiveSkills = new ActiveSkillSavedData[6] {
				new ActiveSkillSavedData {  snoSkill = dbActiveSkills.Skill0, 
											snoRune  = dbActiveSkills.Rune0 },
				new ActiveSkillSavedData {  snoSkill = dbActiveSkills.Skill1, 
											snoRune  = dbActiveSkills.Rune1 },
				new ActiveSkillSavedData {  snoSkill = dbActiveSkills.Skill2, 
											snoRune  = dbActiveSkills.Rune2 },
				new ActiveSkillSavedData {  snoSkill = dbActiveSkills.Skill3, 
											snoRune  = dbActiveSkills.Rune3 },
				new ActiveSkillSavedData {  snoSkill = dbActiveSkills.Skill4, 
											snoRune  = dbActiveSkills.Rune4 },
				new ActiveSkillSavedData {  snoSkill = dbActiveSkills.Skill5, 
											snoRune  = dbActiveSkills.Rune5 },
			};

			PassiveSkills = new int[4] {
				dbActiveSkills.Passive0,
				dbActiveSkills.Passive1,
				dbActiveSkills.Passive2,
				dbActiveSkills.Passive3
			};
			//}

			HotBarSkills = new HotbarButtonData[6] {
				new HotbarButtonData { SNOSkill = ActiveSkills[0].snoSkill, ItemAnn = ActiveSkills[0].snoRune, ItemGBId = -1, RuneType = -1 }, // left-click
				new HotbarButtonData { SNOSkill = ActiveSkills[1].snoSkill, ItemAnn = ActiveSkills[1].snoRune, ItemGBId = -1, RuneType = -1 }, // right-click
				new HotbarButtonData { SNOSkill = ActiveSkills[2].snoSkill, ItemAnn = ActiveSkills[2].snoRune, ItemGBId = -1, RuneType = -1 }, // bar-1
				new HotbarButtonData { SNOSkill = ActiveSkills[3].snoSkill, ItemAnn = ActiveSkills[3].snoRune, ItemGBId = -1, RuneType = -1 }, // bar-2
				new HotbarButtonData { SNOSkill = ActiveSkills[4].snoSkill, ItemAnn = ActiveSkills[4].snoRune, ItemGBId = -1, RuneType = -1 }, // bar-3
				new HotbarButtonData { SNOSkill = ActiveSkills[5].snoSkill, ItemAnn = ActiveSkills[5].snoRune, ItemGBId = -1, RuneType = -1 }, // bar-4
			};
		}

		public void UpdateSkills(int hotBarIndex, int SNOSkill, int SNORune, Toon toon)
		{
			Logger.Debug("Update index {0} skill {1} rune {2}", hotBarIndex, SNOSkill, SNORune);
			var dbActiveSkills = Player.Toon.DBActiveSkills;
			switch (hotBarIndex)
			{
				case 0:
					dbActiveSkills.Skill0 = SNOSkill;
					dbActiveSkills.Rune0 = SNORune;
					break;
				case 1:
					dbActiveSkills.Skill1 = SNOSkill;
					dbActiveSkills.Rune1 = SNORune;
					break;
				case 2:
					dbActiveSkills.Skill2 = SNOSkill;
					dbActiveSkills.Rune2 = SNORune;
					break;
				case 3:
					dbActiveSkills.Skill3 = SNOSkill;
					dbActiveSkills.Rune3 = SNORune;
					break;
				case 4:
					dbActiveSkills.Skill4 = SNOSkill;
					dbActiveSkills.Rune4 = SNORune;
					break;
				case 5:
					dbActiveSkills.Skill5 = SNOSkill;
					dbActiveSkills.Rune5 = SNORune;
					break;
			}
			if (!Player.World.Game.PvP)
			{
				Player.World.Game.GameDbSession.SessionUpdate(dbActiveSkills);
			}

		}

		public void SwitchUpdateSkills(int SkillIndex, int SNOSkill, int SNORune, Toon toon)
		{
			Logger.Debug("SkillSet: SwitchUpdateSkill skillindex {0} Newskill {1}", SkillIndex, SNOSkill);
			HotBarSkills[SkillIndex].SNOSkill = SNOSkill;
			UpdateSkills(SkillIndex, SNOSkill, SNORune, toon);
		}

		public void UpdatePassiveSkills(Toon toon)
		{
			Logger.Debug("Update passive to {0} {1} {2} {3}", PassiveSkills[0], PassiveSkills[1], PassiveSkills[2], PassiveSkills[3]);
			var dbActiveSkills = Player.Toon.DBActiveSkills;
			dbActiveSkills.Passive0 = PassiveSkills[0];
			dbActiveSkills.Passive1 = PassiveSkills[1];
			dbActiveSkills.Passive2 = PassiveSkills[2];
			dbActiveSkills.Passive3 = PassiveSkills[3];
			if (!Player.World.Game.PvP)
			{
				Player.World.Game.GameDbSession.SessionUpdate(dbActiveSkills);
			}
		}

		public bool HasPassive(int passiveId)
		{
			if (PassiveSkills.Contains(passiveId))
				return true;
			else
				return false;
		}

		public bool HasSkill(int skillId)
		{
			return ActiveSkills.Any(s => s.snoSkill == skillId);
		}

		public bool HasSkillWithRune(int skillId, int runeId)
		{
			return ActiveSkills.Any(s => s.snoSkill == skillId && s.snoRune == runeId);
		}


		public bool HasItemPassiveProc(int passiveId)
		{
			if ((float)FastRandom.Instance.NextDouble() < Player.Attributes[GameAttribute.Item_Power_Passive, passiveId])
				return true;
			else
				return false;
		}
	}
}
