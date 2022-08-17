//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	//[ Info] [AttackPayload]: Игрок с индесом: 0 - задамажил: ID: 437089 Name: Barbarian_KKG_Follower_NPC, NumInWorld: 0
	[HandledSNO(437089)] //Barbarian_KKG_Follower_NPC
	public class Barbarian_KKG_Follower_NPC : NPC
	{
		private bool _collapsed = false;
		public Barbarian_KKG_Follower_NPC(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			//{[Actor] [Type: Monster] SNOId:437089 GlobalId: 1017303615 Position: x:348.598 y:853.68604 z:5.41089 Name: Barbarian_KKG_Follower_NPC}
			//437394 - Рык
			//437259 - сидит
			//437260 - встаёт
			//437258 - вышагивает вперёд
			//439753 - исчезновение
			//449259 - Появление на троне со вспышкой
			//324250 - перекат
			//437396 - мертвый

			//	this.Hidden = true;
			//	this.SetVisible(false);
		}

		protected override void ReadTags()
		{
			if (!Tags.ContainsKey(MarkerKeys.ConversationList))
				Tags.Add(MarkerKeys.ConversationList, new TagMapEntry(MarkerKeys.ConversationList.ID, 108832, 2));

			base.ReadTags();
		}
		public override void OnPlayerApproaching(PlayerSystem.Player player)
		{
			try
			{
				if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * this.Scale * this.Scale && !_collapsed)
				{
					if (!player.KanaiUnlocked)
					{
						_collapsed = true;
						this.PlayActionAnimation(439753);

						var Cube = World.GetActorBySNO(437895);
						Cube.PlayActionAnimation(441642);
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
