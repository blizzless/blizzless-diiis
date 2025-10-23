using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.LoginServer.Toons;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(
		ActorSno._x1_fortress_spiritbarbarian,
		ActorSno._x1_fortress_spiritcrusadermmaster,
		ActorSno._x1_fortress_spiritcrusaderfmaster,
		ActorSno._x1_fortress_spiritdemonhunter,
		ActorSno._x1_fortress_spiritmonkpatriarch,
		ActorSno._x1_fortress_spiritwitchdoctor,
		ActorSno._x1_fortress_spiritwizard,
		ActorSno._x1_fortress_spiritnecromancerordan
	)]
	public class A5ClassGhost : InteractiveNPC
	{
		private bool _collapsed = false;

		public A5ClassGhost(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			CollFlags = 1;
			WalkSpeed = 0;
			Attributes[GameAttributes.Invulnerable] = true;
			//this.Attributes[GameAttribute.MinimapIconOverride] = 120356;
		}

		public override bool Reveal(Player player)
		{
			bool showed = false;
			switch (player.Toon.Class)
			{
				case ToonClass.Barbarian:
					showed = SNO == ActorSno._x1_fortress_spiritbarbarian;
					break;
				case ToonClass.Crusader:
					showed = (SNO == ActorSno._x1_fortress_spiritcrusadermmaster && player.Toon.Gender == 0) || (SNO == ActorSno._x1_fortress_spiritcrusaderfmaster && player.Toon.Gender == 1);
					break;
				case ToonClass.DemonHunter:
					showed = SNO == ActorSno._x1_fortress_spiritdemonhunter;
					break;
				case ToonClass.Monk:
					showed = SNO == ActorSno._x1_fortress_spiritmonkpatriarch;
					break;
				case ToonClass.WitchDoctor:
					showed = SNO == ActorSno._x1_fortress_spiritwitchdoctor;
					break;
				case ToonClass.Wizard:
					showed = SNO == ActorSno._x1_fortress_spiritwizard;
					break;
				case ToonClass.Necromancer:
					showed = SNO == ActorSno._x1_fortress_spiritnecromancerordan;
					break;
			}

			if (showed)
				return base.Reveal(player);
			else
				return false;
		}

		public override void OnPlayerApproaching(Player player)
		{
			try
			{
				if (player.Position.DistanceSquared(ref _position) < 144f && !_collapsed)
				{
					if (World.Game.CurrentQuest != 273408) return;

					if (World.Game.CurrentStep == 12)
					{
						_collapsed = true;

						switch (player.Toon.Class)
						{
							case ToonClass.Barbarian:
								player.Conversations.StartConversation(335174);
								break;
							case ToonClass.Crusader:
								if (player.Toon.Gender == 0)
									player.Conversations.StartConversation(336672);
								else
									player.Conversations.StartConversation(336674);
								break;
							case ToonClass.DemonHunter:
								player.Conversations.StartConversation(336676);
								break;
							case ToonClass.Monk:
								player.Conversations.StartConversation(336678);
								break;
							case ToonClass.WitchDoctor:
								player.Conversations.StartConversation(336680);
								break;
							case ToonClass.Wizard:
								player.Conversations.StartConversation(336682);
								break;
						}
					}
				}
			}
			catch { }
		}
	}
}
