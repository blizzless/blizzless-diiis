//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class ServerProp : Actor
	{
		public override ActorType ActorType
		{
			get { return ActorType.ServerProp; }
		}
		//a2dun_Zolt_Hall_NS_480_02 - 1784
		//a2dun_Zolt_Portalroom_A - 31076

		public ServerProp(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Field2 = 0x9;
			this.Field7 = 0x00000001;
			this.CollFlags = 1; // this.CollFlags = 0; a hack for passing through blockers /fasbat
								//this.Attributes[GameAttribute.MinimapActive] = true;

		}

		private bool triggered = false;

		public override bool Reveal(Player player)
		{
			if (this.ActorSNO.Id == 197138 ||   //MouthOfAzmodan
				this.ActorSNO.Id == 172645 ||   //Leoric Throne_Gate
				this.ActorSNO.Id == 220260 ||   //Gluttony_Block_Collision
				this.ActorSNO.Id == 3660 ||     //MilitaryWallB (wtf is that thing?)
				this.ActorSNO.Id == 225300 ||       //a2 caldeum MilitaryWallB
				this.ActorSNO.Id == 122346 ||       //a3_barricade_solid
				this.ActorSNO.Id == 210419 ||       //demonHeart_shield
				this.ActorSNO.Id == 1168333 ||      //ZKNavBlocker
				this.ActorSNO.Id == 167272 ||       //AdriaCover
				this.ActorSNO.Id == 209103 ||   //MilitaryWallB again
				this.ActorSNO.Id == 91162 ||    //TownAttack_ChapelLoc
				this.ActorSNO.Id == 185443 ||   //caOutStingingWinds_StingingWinds_mine_blocker
				this.ActorSNO.Id == 375094 ||       //invisBoxCollision_flippy
				this.ActorSNO.Id == 365472 ||       //_x1_westm_Urzael_Fire_Event_Flash
				(this.ActorSNO.Id == 316495 && this.World.Game.CurrentQuest != 251355) ||   //A5_closedDoor
				((this.ActorSNO.Id == 112131 || this.ActorSNO.Id == 196224) && this.World.Game.CurrentQuest != 87700))          //Tristram invis wall
				return false;

			if (!this.triggered)
			{
				this.triggered = true;
				if (this.ActorSNO.Id == 229290 && FastRandom.Instance.Next(100) < 30) //invisBoxCollision_LeorLogs
				{
					this.World.SpawnMonster(213905, this.Position);
				}
			}

			return base.Reveal(player);
		}
	}
}
