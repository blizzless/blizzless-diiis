﻿//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	//5747
	[HandledSNO(
		ActorSno._trdun_cath_chandelier_trap, ActorSno._trdun_cath_braizer_trap // trDun_cath_Chandilier_Trap.acr 
	)]
	class CathedralLamp : Gizmo
	{
		public CathedralLamp(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			
		}

		public void ReceiveDamage(Actor source, float damage)
		{
			if (SNO == ActorSno._trout_highlands_goatman_totem_gharbad && World.Game.CurrentSideQuest != 225253) return;

			World.BroadcastIfRevealed(plr => new FloatingNumberMessage
			{
				Number = damage,
				ActorID = DynamicID(plr),
				Type = FloatingNumberMessage.FloatType.White
			}, this);

			Attributes[GameAttribute.Hitpoints_Cur] = Math.Max(Attributes[GameAttribute.Hitpoints_Cur] - damage, 0);
			Attributes.BroadcastChangedIfRevealed();

			if (Attributes[GameAttribute.Hitpoints_Cur] == 0 && !SNO.IsUndestroyable())
				Die(source);
		}

		public void Die(Actor source = null)
		{
			base.OnTargeted(null, null);

			Logger.Trace("Breaked barricade, id: {0}", SNO);

			if (AnimationSet.Animations.ContainsKey(AnimationSetKeys.DeathDefault.ID))
				World.BroadcastIfRevealed(plr => new PlayAnimationMessage
				{
					ActorID = DynamicID(plr),
					AnimReason = 11,
					UnitAniimStartTime = 0,
					tAnim = new PlayAnimationMessageSpec[]
					{
						new PlayAnimationMessageSpec()
						{
							Duration = 10,
							AnimationSNO = (int)AnimationSet.Animations[AnimationSetKeys.DeathDefault.ID], //{DeathDefault = 10217}
							PermutationIndex = 0,
							AnimationTag = 0,
							Speed = 1
						}
					}

				}, this);

			Attributes[GameAttribute.Deleted_On_Server] = true;
			Attributes[GameAttribute.Could_Have_Ragdolled] = true;
			Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;
			Attributes[GameAttribute.Damage_Weapon_Min, 0] = 5f;
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 5f;
			Attributes.BroadcastChangedIfRevealed();

			Task.Delay(1500).ContinueWith(delegate
			{
				World.PowerManager.RunPower(this, 30209);
				var DataOfSkill = DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.Anim][10217].Data;

				Destroy();
			});
			
		}


		public override void OnTargeted(Player player, TargetMessage message)
		{
			base.OnTargeted(player, message);
			ReceiveDamage(player, 100);
		}
	}
}