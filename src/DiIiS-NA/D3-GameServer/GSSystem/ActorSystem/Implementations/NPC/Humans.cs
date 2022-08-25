//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    [HandledSNO(308474, //X1_WestM_Intro_Human_Male
               309191,  //X1_WestM_Intro_Human_Male2
               181563,  //vizjereiMale_A_Town
               210087,  //Zakarum_Female_Wealthy_Gates
               190390,  //A3_Hub_SacrificeLadyNew
               378376   //x1_WestmHub_Guard_NoLoS_KnownWithScene
               )]
    class Humans : NPC
    {
        private bool _collapsed = false;
        public Humans(World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            this.Field7 = 1;
            this.Attributes[GameAttribute.TeamID] = 2;
            this.Attributes[GameAttribute.NPC_Has_Interact_Options, 0] = false;
        }
        
		public override bool Reveal(PlayerSystem.Player player)
		{
			if (!base.Reveal(player))
				return false;
            
			return true;
		}
        //*/

        public override void OnPlayerApproaching(PlayerSystem.Player player)
        {
            try
            {
                if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * this.Scale * this.Scale && ! _collapsed)
                {
                    _collapsed = true;

                    if (this.World.WorldSNO.Id == 306549)
                        switch (this.ActorSNO.Id)
                        {
                            case 308474:
                                if (this.Position.X > 1440) 
                                    StartConversation(this.World, 311433);
                                else
                                {
                                    foreach (var man in this.World.GetActorsBySNO(308474)) if (man.CurrentScene.SceneSNO.Id == this.CurrentScene.SceneSNO.Id) man.PlayActionAnimation(306544);
                                    foreach (var man in this.World.GetActorsBySNO(309191)) if (man.CurrentScene.SceneSNO.Id == this.CurrentScene.SceneSNO.Id) man.PlayActionAnimation(306544);
                                    foreach (var man in this.World.GetActorsBySNO(310653)) if (man.CurrentScene.SceneSNO.Id == this.CurrentScene.SceneSNO.Id) man.PlayActionAnimation(306544);
                                    foreach (var man in this.World.GetActorsBySNO(310631)) if (man.CurrentScene.SceneSNO.Id == this.CurrentScene.SceneSNO.Id) man.PlayActionAnimation(306544);
                                }
                                break;
                            case 309191:
                                if (this.Position.X > 1300 & this.Position.Y > 440)
                                {
                                    StartConversation(this.World, 311435);
                                    //foreach(var 309191 )
                                    /*
                                     * +		[0]	{Ambush = 306544}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry
                                       +		[1]	{Run = 341519}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry
                                    +		[0]	{Ambush = 306544}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry
                                    +		[1]	{70912 = 328577}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry
                                    +		[2]	{70928 = 328575}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry
                                    +		[3]	{70944 = 328578}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry
                                    +		[4]	{70960 = 328576}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry
                                    +		[5]	{70976 = 328782}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry
                                    +		[6]	{98304 = 330015}	DiIiS_NA.GameServer.Core.Types.TagMap.TagMapEntry
                                    */
                                    foreach (var man in this.World.GetActorsBySNO(308474)) if (man.CurrentScene.SceneSNO.Id == this.CurrentScene.SceneSNO.Id) man.PlayActionAnimation(306544);
                                    foreach (var man in this.World.GetActorsBySNO(309191)) if (man.CurrentScene.SceneSNO.Id == this.CurrentScene.SceneSNO.Id) man.PlayActionAnimation(306544);
                                    foreach (var man in this.World.GetActorsBySNO(310653)) if (man.CurrentScene.SceneSNO.Id == this.CurrentScene.SceneSNO.Id) man.PlayActionAnimation(306544);
                                    
                                    
                                }
                                break; //X1_WestM_Intro_Human_Male2

                        }

                }
            }
            catch { }
        }

        protected bool StartConversation(World world, int conversationId)
        {
            foreach (var plr in world.Players)
                plr.Value.Conversations.StartConversation(conversationId);
            return true;
        }
    }
}
