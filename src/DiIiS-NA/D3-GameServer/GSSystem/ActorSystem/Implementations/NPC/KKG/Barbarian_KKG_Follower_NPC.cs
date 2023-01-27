using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    //[ Info] [AttackPayload]: Игрок с индесом: 0 - задамажил: ID: 437089 Name: Barbarian_KKG_Follower_NPC, NumInWorld: 0
    [HandledSNO(ActorSno._barbarian_kkg_follower_npc)] //Barbarian_KKG_Follower_NPC
	public class Barbarian_KKG_Follower_NPC : NPC
	{
		private bool _collapsed = false;
		public Barbarian_KKG_Follower_NPC(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			//{[Actor] [Type: Monster] SNOId:437089 GlobalId: 1017303615 Position: x:348.598 y:853.68604 z:5.41089 Name: Barbarian_KKG_Follower_NPC}
			//437394 - Roar
			//437259 - Sit
			//437260 - Stand
			//437258 - Walk Forward
			//439753 - Disappear
			//449259 - Appear on Throne with Flash
			//324250 - Roll
			//437396 - Dead

			//	this.Hidden = true;
			//	this.SetVisible(false);
		}

		protected override void ReadTags()
		{
			if (!Tags.ContainsKey(MarkerKeys.ConversationList))
				Tags.Add(MarkerKeys.ConversationList, new TagMapEntry(MarkerKeys.ConversationList.ID, 108832, 2));

			base.ReadTags();
		}
		public override void OnPlayerApproaching(Player player)
		{
			try
			{
				if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * Scale * Scale && !_collapsed)
				{
					if (!player.KanaiUnlocked)
					{
						_collapsed = true;
						this.PlayActionAnimation(AnimationSno.barbarian_male_hth_kkgevent_point_01);

						var Cube = World.GetActorBySNO(ActorSno._p4_ruins_frost_kanaicube_altar);
						Cube.PlayActionAnimation(AnimationSno.p4_ruins_frost_kanaicube_altar_active);
						//{[Actor] [Type: Gizmo] SNOId:437895 GlobalId: 1017303610 Position: x:331.9304 y:867.761 z:5.41071 Name: p4_Ruins_Frost_KanaiCube_Altar}
						foreach (var plr in player.InGameClient.Game.Players.Values)
							plr.GrantCriteria(74987252674266);
					}
				}
			}
			catch { }
		}
	}
}
