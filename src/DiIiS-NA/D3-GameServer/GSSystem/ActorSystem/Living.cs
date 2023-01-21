//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class Living : Actor
	{
		public override ActorType ActorType { get { return ActorType.Monster; } }

		public SNOHandle Monster { get; private set; }

		/// <summary>
		/// The brain for 
		/// </summary>
		public AISystem.Brain Brain { get; set; }

		public Living(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Monster = new SNOHandle(SNOGroup.Monster, (ActorData.MonsterSNO));
			this.Field2 = 0x9;//16;

			// FIXME: This is hardcoded crap
			this.SetFacingRotation((float)(RandomHelper.NextDouble() * 2.0f * Math.PI));
			this.GBHandle.Type = -1; this.GBHandle.GBID = -1;
			this.Field7 = 0x00000001;
			this.Field10 = 0x0;

			//scripted //this.Attributes[GameAttribute.Hitpoints_Max_Total] = 4.546875f;
			this.Attributes[GameAttribute.Hitpoints_Max] = 4.546875f;
			//scripted //this.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
			this.Attributes[GameAttribute.Hitpoints_Cur] = 4.546875f;

			this.Attributes[GameAttribute.Level] = 1;
		}

		public override bool Reveal(Player player)
		{
			PlayEffect(MessageSystem.Message.Definitions.Effect.Effect.Hit);
			if (!base.Reveal(player))
				return false;
			if (AnimationSet != null)
			{
				if (this.AnimationSet.GetAnimationTag(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Idle) != -1)
				{
					player.InGameClient.SendMessage(new SetIdleAnimationMessage
					{
						ActorID = this.DynamicID(player),
						AnimationSNO = this.AnimationSet.GetAnimationTag(DiIiS_NA.Core.MPQ.FileFormats.AnimationTags.Idle)
					});
				}
			}
			return true;
		}

		public void Kill(PowerContext context = null, bool lootAndExp = false)
		{
			var deathload = new DeathPayload(context, DamageType.Physical, this, lootAndExp);
			deathload.Apply();
		}
	}
}
