//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(211835)]
	class A2BelialBomb : ProximityTriggeredGizmo
    {
		private bool _collapsed = false;

		public A2BelialBomb(MapSystem.World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			//this.Field2 = 0x9;//16;
			//this.Field7 = 0x00000001;
			this.CollFlags = 0;
			float DamageMin = this.World.Game.MonsterLevel * 10f;

			if (this.World.Game.MonsterLevel > 30)
				DamageMin = this.World.Game.MonsterLevel * 50f;

			if (this.World.Game.MonsterLevel > 60)
				DamageMin = this.World.Game.MonsterLevel * 120f;

			float DamageDelta = DamageMin * 0.3f * this.World.Game.DmgModifier;
			this.Attributes[GameAttribute.Damage_Weapon_Min, 0] = DamageMin * this.World.Game.DmgModifier;
			this.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = DamageDelta;

			this.Attributes[GameAttribute.Team_Override] = 2;
			this.Attributes[GameAttribute.Untargetable] = true;
			this.Attributes[GameAttribute.NPC_Is_Operatable] = false;
			this.Attributes[GameAttribute.Operatable] = false;
			this.Attributes[GameAttribute.Operatable_Story_Gizmo] = false;
			this.Attributes[GameAttribute.Immunity] = true;
			this.Attributes.BroadcastChangedIfRevealed();
		}

		public override void OnPlayerApproaching(PlayerSystem.Player player)
		{
			try
			{
			 	if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * this.Scale * this.Scale && !_collapsed)
				{
					               
					_collapsed = true;

					this.PlayActionAnimation(10264);
					this.World.PowerManager.RunPower(this, 153000);

					Task.Delay(RandomHelper.Next(5,10) * 1000).ContinueWith(delegate { _collapsed = false; });
				}
			}
			catch { }
		}
	}
}
