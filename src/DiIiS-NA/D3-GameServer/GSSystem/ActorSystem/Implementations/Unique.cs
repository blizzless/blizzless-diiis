//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GeneratorsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
//Blizzless Project 2022 
using System;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(
	341760, //Sartor
	342355, //Balata	
	290453, //Lurk	
	353443, //Trejiak
	156738, //Moontooth Dreadshark	
	156763, //Raziel	
	219583, //Flesh of Nar Gulle	
	218947, //Shaitan the Broodmother	
	219832, //Ernutet	
	140424, //Mundunogo	
	140947, //Dervish Lord
	144400, //Graveljaw the Devourer	
	147155, //Hurax
	105959, //Cultist Grand Inquisitor
	111580, //Cadhul the Deathcaller
	218362, //Firestarter
	4340, //Dataminer
	156801, //captain Daltyn
	115403, //Cain Intro Skeleton
	85900,  //Mira Imon
	156353, //Imon advisor
	131131, //prophet Urik
	174013, //graverobber Nigel
	178619, //Wortham cultist leader
	188400, //a2_swr_Adria snakeman
	338681, //Thilor
	358946 //Mordrath
	)]
	public class Unique : Monster
	{
		public bool CanDropKey = false;

		public Unique(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			//this.Attributes[GameAttribute.Hitpoints_Max] *= 6.0f;
			//this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max];
			//this.Attributes[GameAttribute.Immune_To_Charm] = true;
			//this.Attributes[GameAttribute.Damage_Weapon_Min, 0] *= 3.5f;
			//this.Attributes[GameAttribute.Damage_Weapon_Delta, 0] *= 3.5f;

			MonsterAffixGenerator.Generate(this, Math.Min(this.World.Game.Difficulty + 1, 5));
		}

		public override int Quality
		{
			get
			{
				return (int)DiIiS_NA.Core.MPQ.FileFormats.SpawnType.Unique;
			}
			set
			{
				// TODO MonsterQuality setter not implemented. Throwing a NotImplementedError is catched as message not beeing implemented and nothing works anymore...
			}
		}
	}
}
