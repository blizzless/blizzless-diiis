//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Hash;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings
{
	[HandledSNO(ActorSno._enchantress /* Enchantress.acr */)]
	public class Enchantress : Hireling
	{
		public Enchantress(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			mainSNO = ActorSno._enchantress;
			hirelingSNO = ActorSno._hireling_enchantress;
			proxySNO = ActorSno._hireling_enchantress_proxy;
			skillKit = 484938;
			hirelingGBID = StringHashHelper.HashItemName("Enchantress");
			Attributes[GameAttribute.Hireling_Class] = 3;
		}

		public override Hireling CreateHireling(MapSystem.World world, ActorSno sno, TagMap tags)
		{
			return new Enchantress(world, sno, tags);
		}

		public void SetSkill(Player player, int SkillSNOId)
		{
			var dbhireling = player.World.Game.GameDbSession.SessionQueryWhere<DBHireling>(dbh => dbh.DBToon.Id == player.Toon.PersistentID && dbh.Class == 3).ToList().First();
			switch (SkillSNOId)
			{
				case 102057:
				case 101969:
					player.HirelingInfo[3].Skill1SNOId = SkillSNOId;
					Attributes[GameAttribute.Skill, SkillSNOId] = 1;
					Attributes[GameAttribute.Skill, (SkillSNOId == 102057 ? 101969 : 102057)] = 0;
					Attributes.BroadcastChangedIfRevealed();

					dbhireling.Skill1SNOId = SkillSNOId;
					player.World.Game.GameDbSession.SessionUpdate(dbhireling);
					break;
				case 102133:
				case 101461:
					player.HirelingInfo[3].Skill2SNOId = SkillSNOId;
					Attributes[GameAttribute.Skill, SkillSNOId] = 1;
					Attributes[GameAttribute.Skill, (SkillSNOId == 102133 ? 101461 : 102133)] = 0;
					Attributes.BroadcastChangedIfRevealed();

					dbhireling.Skill2SNOId = SkillSNOId;
					player.World.Game.GameDbSession.SessionUpdate(dbhireling);
					break;
				case 101990:
				case 220872:
					player.HirelingInfo[3].Skill3SNOId = SkillSNOId;
					Attributes[GameAttribute.Skill, SkillSNOId] = 1;
					Attributes[GameAttribute.Skill, (SkillSNOId == 101990 ? 220872 : 101990)] = 0;
					Attributes.BroadcastChangedIfRevealed();

					dbhireling.Skill3SNOId = SkillSNOId;
					player.World.Game.GameDbSession.SessionUpdate(dbhireling);
					break;
				case 101425:
				case 201524:
					player.HirelingInfo[3].Skill4SNOId = SkillSNOId;
					Attributes[GameAttribute.Skill, SkillSNOId] = 1;
					Attributes[GameAttribute.Skill, (SkillSNOId == 101425 ? 201524 : 101425)] = 0;
					Attributes.BroadcastChangedIfRevealed();

					dbhireling.Skill4SNOId = SkillSNOId;
					player.World.Game.GameDbSession.SessionUpdate(dbhireling);
					break;
				default:
					return;
			}
		}

		public void Retrain(Player player)
		{
			var dbhireling = player.World.Game.GameDbSession.SessionQueryWhere<DBHireling>(dbh => dbh.DBToon.Id == player.Toon.PersistentID && dbh.Class == 3).ToList().First();
			dbhireling.Skill1SNOId = -1;
			dbhireling.Skill2SNOId = -1;
			dbhireling.Skill3SNOId = -1;
			dbhireling.Skill4SNOId = -1;
			player.World.Game.GameDbSession.SessionUpdate(dbhireling);

			player.HirelingInfo[3].Skill1SNOId = -1;
			player.HirelingInfo[3].Skill2SNOId = -1;
			player.HirelingInfo[3].Skill3SNOId = -1;
			player.HirelingInfo[3].Skill4SNOId = -1;

			Attributes[GameAttribute.Skill, 102057] = 0;
			Attributes[GameAttribute.Skill, 101969] = 0;
			Attributes[GameAttribute.Skill, 102133] = 0;
			Attributes[GameAttribute.Skill, 101461] = 0;
			Attributes[GameAttribute.Skill, 101990] = 0;
			Attributes[GameAttribute.Skill, 220872] = 0;
			Attributes[GameAttribute.Skill, 101425] = 0;
			Attributes[GameAttribute.Skill, 201524] = 0;
			Attributes.SendChangedMessage(player.InGameClient);
		}


		public override bool Reveal(Player player)
		{
			if (!player.HirelingEnchantressUnlocked)
				return false;

			return base.Reveal(player);
		}
	}
}
