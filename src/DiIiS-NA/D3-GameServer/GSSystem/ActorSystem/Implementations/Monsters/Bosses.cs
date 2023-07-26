using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.MessageSystem;
using Microsoft.Extensions.Logging;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Monsters
{
	#region Unique_CaptainDaltyn
	[HandledSNO(ActorSno._unique_captaindaltyn)]
	public class Unique_CaptainDaltyn : Monster
	{
		public Unique_CaptainDaltyn(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttributes.MinimapActive] = true;
			Attributes[GameAttributes.Immune_To_Charm] = true;
			Attributes[GameAttributes.using_Bossbar] = true;
			Attributes[GameAttributes.InBossEncounter] = true;

			Attributes[GameAttributes.Hitpoints_Cur] = Attributes[GameAttributes.Hitpoints_Max_Total];
			Attributes[GameAttributes.TeamID] = 10;


			WalkSpeed = 0.2f;

		}

		public override int Quality
		{
			get => (int)DiIiS_NA.Core.MPQ.FileFormats.SpawnType.Boss;
			set
			{
			}
		}


	}
	#endregion

}
