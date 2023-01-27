using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(ActorSno._belialfirebomb)]
	class A2BelialBomb : ProximityTriggeredGizmo
    {
		private bool _collapsed = false;

		public A2BelialBomb(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			//this.Field2 = 0x9;//16;
			//this.Field7 = 0x00000001;
			CollFlags = 0;
			float DamageMin = World.Game.MonsterLevel * 10f;

			if (World.Game.MonsterLevel > 30)
				DamageMin = World.Game.MonsterLevel * 50f;

			if (World.Game.MonsterLevel > 60)
				DamageMin = World.Game.MonsterLevel * 120f;

			float DamageDelta = DamageMin * 0.3f * World.Game.DmgModifier;
			Attributes[GameAttribute.Damage_Weapon_Min, 0] = DamageMin * World.Game.DmgModifier;
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = DamageDelta;

			Attributes[GameAttribute.Team_Override] = 2;
			Attributes[GameAttribute.Untargetable] = true;
			Attributes[GameAttribute.NPC_Is_Operatable] = false;
			Attributes[GameAttribute.Operatable] = false;
			Attributes[GameAttribute.Operatable_Story_Gizmo] = false;
			Attributes[GameAttribute.Immunity] = true;
			Attributes.BroadcastChangedIfRevealed();
		}

		public override void OnPlayerApproaching(PlayerSystem.Player player)
		{
			try
			{
			 	if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * Scale * Scale && !_collapsed)
				{
					               
					_collapsed = true;

					this.PlayActionAnimation(AnimationSno.trdun_cath_lever_type2_closing);
					this.World.PowerManager.RunPower(this, 153000);

					Task.Delay(RandomHelper.Next(5,10) * 1000).ContinueWith(delegate { _collapsed = false; });
				}
			}
			catch { }
		}
	}
}
