//Blizzless Project 2022 
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
	[HandledSNO(308737, 314802, 319402, 314804, 314806, 314817, 314792)]
	public class A5ClassGhost : InteractiveNPC
	{
		private bool _collapsed = false;

		public A5ClassGhost(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
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
					showed = (this.ActorSNO.Id == 308737);
					break;
				case ToonClass.Crusader:
					showed = ((this.ActorSNO.Id == 314802 && player.Toon.Gender == 0) || (this.ActorSNO.Id == 319402 && player.Toon.Gender == 1));
					break;
				case ToonClass.DemonHunter:
					showed = (this.ActorSNO.Id == 314804);
					break;
				case ToonClass.Monk:
					showed = (this.ActorSNO.Id == 314806);
					break;
				case ToonClass.WitchDoctor:
					showed = (this.ActorSNO.Id == 314817);
					break;
				case ToonClass.Wizard:
					showed = (this.ActorSNO.Id == 314792);
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
