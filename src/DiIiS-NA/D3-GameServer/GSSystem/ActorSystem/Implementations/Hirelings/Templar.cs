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
	[HandledSNO(ActorSno._templar /* Templar.acr */)]
	public class Templar : Hireling
	{
		public Templar(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			mainSNO = ActorSno._templar;
			hirelingSNO = ActorSno._hireling_templar;
			proxySNO = ActorSno._hireling_templar_proxy;
			skillKit = 484941;
			hirelingGBID = StringHashHelper.HashItemName("Templar");
			Attributes[GameAttribute.Hireling_Class] = 1;
		}

		public override Hireling CreateHireling(MapSystem.World world, ActorSno sno, TagMap tags)
		{
			return new Templar(world, sno, tags);
		}

		public void SetSkill(Player player, int SkillSNOId)
		{
			var dbhireling = player.World.Game.GameDBSession.SessionQueryWhere<DBHireling>(dbh => dbh.DBToon.Id == player.Toon.PersistentID && dbh.Class == 1).ToList().First();
			switch (SkillSNOId)
			{
				case 1747:
				case 93938:
					player.HirelingInfo[1].Skill1SNOId = SkillSNOId;
					Attributes[GameAttribute.Skill, SkillSNOId] = 1;
					Attributes[GameAttribute.Skill, (SkillSNOId == 1747 ? 93938 : 1747)] = 0;
					Attributes.SendChangedMessage(player.InGameClient);

					dbhireling.Skill1SNOId = SkillSNOId;
					player.World.Game.GameDBSession.SessionUpdate(dbhireling);
					break;
				case 30357:
				case 93901:
					player.HirelingInfo[1].Skill2SNOId = SkillSNOId;
					Attributes[GameAttribute.Skill, SkillSNOId] = 1;
					Attributes[GameAttribute.Skill, (SkillSNOId == 30357 ? 93901 : 30357)] = 0;
					Attributes.SendChangedMessage(player.InGameClient);

					dbhireling.Skill2SNOId = SkillSNOId;
					player.World.Game.GameDBSession.SessionUpdate(dbhireling);
					break;
				case 30360:
				case 93888:
					player.HirelingInfo[1].Skill3SNOId = SkillSNOId;
					Attributes[GameAttribute.Skill, SkillSNOId] = 1;
					Attributes[GameAttribute.Skill, (SkillSNOId == 30360 ? 93888 : 30360)] = 0;
					Attributes.SendChangedMessage(player.InGameClient);

					dbhireling.Skill3SNOId = SkillSNOId;
					player.World.Game.GameDBSession.SessionUpdate(dbhireling);
					break;
				case 30356:
				case 30359:
					player.HirelingInfo[1].Skill4SNOId = SkillSNOId;
					Attributes[GameAttribute.Skill, SkillSNOId] = 1;
					Attributes[GameAttribute.Skill, (SkillSNOId == 30356 ? 30359 : 30356)] = 0;
					Attributes.SendChangedMessage(player.InGameClient);

					dbhireling.Skill4SNOId = SkillSNOId;
					player.World.Game.GameDBSession.SessionUpdate(dbhireling);
					break;
				default:
					return;
			}
		}

		public void Retrain(Player player)
		{
			var dbhireling = player.World.Game.GameDBSession.SessionQueryWhere<DBHireling>(dbh => dbh.DBToon.Id == player.Toon.PersistentID && dbh.Class == 1).ToList().First();
			dbhireling.Skill1SNOId = -1;
			dbhireling.Skill2SNOId = -1;
			dbhireling.Skill3SNOId = -1;
			dbhireling.Skill4SNOId = -1;
			player.World.Game.GameDBSession.SessionUpdate(dbhireling);

			player.HirelingInfo[1].Skill1SNOId = -1;
			player.HirelingInfo[1].Skill2SNOId = -1;
			player.HirelingInfo[1].Skill3SNOId = -1;
			player.HirelingInfo[1].Skill4SNOId = -1;

			Attributes[GameAttribute.Skill, 1747] = 0;
			Attributes[GameAttribute.Skill, 93938] = 0;
			Attributes[GameAttribute.Skill, 30357] = 0;
			Attributes[GameAttribute.Skill, 93901] = 0;
			Attributes[GameAttribute.Skill, 30360] = 0;
			Attributes[GameAttribute.Skill, 93888] = 0;
			Attributes[GameAttribute.Skill, 30356] = 0;
			Attributes[GameAttribute.Skill, 30359] = 0;
			Attributes.SendChangedMessage(player.InGameClient);
		}


		public override bool Reveal(Player player)
		{
			if (!player.HirelingTemplarUnlocked)
				return false;

			return base.Reveal(player);
		}
	}
}
