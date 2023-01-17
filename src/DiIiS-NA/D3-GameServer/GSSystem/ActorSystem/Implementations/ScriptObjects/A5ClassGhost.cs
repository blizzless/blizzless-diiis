//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022 
using System.Drawing;

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
			this.CollFlags = 1;
			this.WalkSpeed = 0;
			this.Attributes[GameAttribute.Invulnerable] = true;
			//this.Attributes[GameAttribute.MinimapIconOverride] = 120356;
		}

		public override bool Reveal(Player player)
		{
			bool showed = false;
			switch (player.Toon.Class)
			{
				case ToonClass.Barbarian:
					showed = this.SNO == ActorSno._x1_fortress_spiritbarbarian;
					break;
				case ToonClass.Crusader:
					showed = (this.SNO == ActorSno._x1_fortress_spiritcrusadermmaster && player.Toon.Gender == 0) || (this.SNO == ActorSno._x1_fortress_spiritcrusaderfmaster && player.Toon.Gender == 1);
					break;
				case ToonClass.DemonHunter:
					showed = this.SNO == ActorSno._x1_fortress_spiritdemonhunter;
					break;
				case ToonClass.Monk:
					showed = this.SNO == ActorSno._x1_fortress_spiritmonkpatriarch;
					break;
				case ToonClass.WitchDoctor:
					showed = this.SNO == ActorSno._x1_fortress_spiritwitchdoctor;
					break;
				case ToonClass.Wizard:
					showed = this.SNO == ActorSno._x1_fortress_spiritwizard;
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
					if (this.World.Game.CurrentQuest != 273408) return;

					if (this.World.Game.CurrentStep == 12)
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
