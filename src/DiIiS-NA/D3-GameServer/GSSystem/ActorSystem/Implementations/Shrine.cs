using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	class Shrine : Gizmo
	{
		public Shrine(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttributes.MinimapActive] = true;
		}

		private bool Activated = false;
		private object taskLock = new object();

		public override void OnTargeted(Player player, TargetMessage message)
		{
			lock (taskLock)
			{
				if (Activated) return;
				Activated = true;
				World.BroadcastIfRevealed(plr => new ANNDataMessage(Opcodes.ShrineActivatedMessage) { ActorID = DynamicID(plr) }, this);
				switch (SNO)
				{
					case ActorSno._shrine_global_blessed: //blessed
						foreach (var plr in GetPlayersInRange(100f))
						{
							World.BuffManager.AddBuff(this, plr, new PowerSystem.Implementations.ShrineBlessedBuff(TickTimer.WaitSeconds(World.Game, 120.0f)));
							plr.GrantCriteria(74987243307423);
						}
						break;
					case ActorSno._shrine_global_enlightened: //enlightened
						foreach (var plr in GetPlayersInRange(100f))
						{
							World.BuffManager.AddBuff(this, plr, new PowerSystem.Implementations.ShrineEnlightenedBuff(TickTimer.WaitSeconds(World.Game, 120.0f)));
							plr.GrantCriteria(74987243307424);
						}
						break;
					case ActorSno._shrine_global_fortune: //fortune
						foreach (var plr in GetPlayersInRange(100f))
						{
							World.BuffManager.AddBuff(this, plr, new PowerSystem.Implementations.ShrineFortuneBuff(TickTimer.WaitSeconds(World.Game, 120.0f)));
							plr.GrantCriteria(74987243307425);
						}
						break;
					case ActorSno._shrine_global_frenzied: //frenzied
						foreach (var plr in GetPlayersInRange(100f))
						{
							World.BuffManager.AddBuff(this, plr, new PowerSystem.Implementations.ShrineFrenziedBuff(TickTimer.WaitSeconds(World.Game, 120.0f)));
							plr.GrantCriteria(74987243307426);
						}
						break;
					default:
						foreach (var plr in GetPlayersInRange(100f))
						{
							World.BuffManager.AddBuff(this, plr, new PowerSystem.Implementations.ShrineEnlightenedBuff(TickTimer.WaitSeconds(World.Game, 120.0f)));
						}
						break;
				}

				Attributes[GameAttributes.Gizmo_Has_Been_Operated] = true;
				//this.Attributes[GameAttribute.Gizmo_Operator_ACDID] = unchecked((int)player.DynamicID);
				Attributes[GameAttributes.Gizmo_State] = 1;
				Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
}
