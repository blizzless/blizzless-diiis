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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings
{
	[HandledSNO(4644 /* Scoundrel.acr */)]
	public class Scoundrel : Hireling
	{
		public Scoundrel(MapSystem.World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			mainSNO = 4644;
			hirelingSNO = 52694;
			proxySNO = 192941;
			skillKit = 484937;
			hirelingGBID = StringHashHelper.HashItemName("Scoundrel");
			Attributes[GameAttribute.Hireling_Class] = 2;
		}

		public override Hireling CreateHireling(MapSystem.World world, int snoId, TagMap tags)
		{
			return new Scoundrel(world, snoId, tags);
		}

		public void SetSkill(Player player, int SkillSNOId)
		{
			var dbhireling = player.World.Game.GameDBSession.SessionQueryWhere<DBHireling>(dbh => dbh.DBToon.Id == player.Toon.PersistentID && dbh.Class == 2).ToList().First();
			switch (SkillSNOId)
			{
				case 95675:
				case 30460:
					player.HirelingInfo[2].Skill1SNOId = SkillSNOId;
					this.Attributes[GameAttribute.Skill, SkillSNOId] = 1;
					this.Attributes[GameAttribute.Skill, (SkillSNOId == 95675 ? 30460 : 95675)] = 0;
					this.Attributes.SendChangedMessage(player.InGameClient);

					dbhireling.Skill1SNOId = SkillSNOId;
					player.World.Game.GameDBSession.SessionUpdate(dbhireling);
					break;
				case 97436:
				case 30464:
					player.HirelingInfo[2].Skill2SNOId = SkillSNOId;
					this.Attributes[GameAttribute.Skill, SkillSNOId] = 1;
					this.Attributes[GameAttribute.Skill, (SkillSNOId == 97436 ? 30464 : 97436)] = 0;
					this.Attributes.SendChangedMessage(player.InGameClient);

					dbhireling.Skill2SNOId = SkillSNOId;
					player.World.Game.GameDBSession.SessionUpdate(dbhireling);
					break;
				case 95690:
				case 30458:
					player.HirelingInfo[2].Skill3SNOId = SkillSNOId;
					this.Attributes[GameAttribute.Skill, SkillSNOId] = 1;
					this.Attributes[GameAttribute.Skill, (SkillSNOId == 95690 ? 30458 : 95690)] = 0;
					this.Attributes.SendChangedMessage(player.InGameClient);

					dbhireling.Skill3SNOId = SkillSNOId;
					player.World.Game.GameDBSession.SessionUpdate(dbhireling);
					break;
				case 200169:
				case 30454:
					player.HirelingInfo[2].Skill4SNOId = SkillSNOId;
					this.Attributes[GameAttribute.Skill, SkillSNOId] = 1;
					this.Attributes[GameAttribute.Skill, (SkillSNOId == 200169 ? 30454 : 200169)] = 0;
					this.Attributes.SendChangedMessage(player.InGameClient);

					dbhireling.Skill4SNOId = SkillSNOId;
					player.World.Game.GameDBSession.SessionUpdate(dbhireling);
					break;
				default:
					return;
			}
		}

		public void Retrain(Player player)
		{
			var dbhireling = player.World.Game.GameDBSession.SessionQueryWhere<DBHireling>(dbh => dbh.DBToon.Id == player.Toon.PersistentID && dbh.Class == 2).ToList().First();
			dbhireling.Skill1SNOId = -1;
			dbhireling.Skill2SNOId = -1;
			dbhireling.Skill3SNOId = -1;
			dbhireling.Skill4SNOId = -1;
			player.World.Game.GameDBSession.SessionUpdate(dbhireling);

			player.HirelingInfo[2].Skill1SNOId = -1;
			player.HirelingInfo[2].Skill2SNOId = -1;
			player.HirelingInfo[2].Skill3SNOId = -1;
			player.HirelingInfo[2].Skill4SNOId = -1;

			this.Attributes[GameAttribute.Skill, 95675] = 0;
			this.Attributes[GameAttribute.Skill, 30460] = 0;
			this.Attributes[GameAttribute.Skill, 97436] = 0;
			this.Attributes[GameAttribute.Skill, 30464] = 0;
			this.Attributes[GameAttribute.Skill, 95690] = 0;
			this.Attributes[GameAttribute.Skill, 30458] = 0;
			this.Attributes[GameAttribute.Skill, 200169] = 0;
			this.Attributes[GameAttribute.Skill, 30454] = 0;
			this.Attributes.SendChangedMessage(player.InGameClient);
		}


		public override bool Reveal(Player player)
		{
			if (!player.HirelingScoundrelUnlocked)
				return false;

			return base.Reveal(player);
		}
	}
}
