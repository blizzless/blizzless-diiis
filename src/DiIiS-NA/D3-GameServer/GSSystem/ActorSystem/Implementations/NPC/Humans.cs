using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    [HandledSNO(
        ActorSno._x1_westm_intro_human_male, //X1_WestM_Intro_Human_Male
        ActorSno._x1_westm_intro_human_male2,  //X1_WestM_Intro_Human_Male2
        ActorSno._vizjereimale_a_town,  //vizjereiMale_A_Town
        ActorSno._zakarum_female_wealthy_gates,  //Zakarum_Female_Wealthy_Gates
        ActorSno._a3_hub_sacrificeladynew,  //A3_Hub_SacrificeLadyNew
        ActorSno._x1_westmhub_guard_nolos_knownwithscene   //x1_WestmHub_Guard_NoLoS_KnownWithScene
    )]
    class Humans : NPC
    {
        private bool _collapsed = false;
        public Humans(World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            Field7 = 1;
            Attributes[GameAttributes.TeamID] = 2;
            Attributes[GameAttributes.NPC_Has_Interact_Options, 0] = false;
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
                if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * Scale * Scale && ! _collapsed)
                {
                    _collapsed = true;

                    if (World.SNO == WorldSno.x1_westm_intro)
                        switch (SNO)
                        {
                            case ActorSno._x1_westm_intro_human_male:
                                if (Position.X > 1440) 
                                    StartConversation(World, 311433);
                                else
                                {
                                    foreach (var man in World.GetActorsBySNO(
                                        ActorSno._x1_westm_intro_human_male,
                                        ActorSno._x1_westm_intro_human_male2,
                                        ActorSno._x1_westm_intro_human_female,
                                        ActorSno._x1_westmarchfemale_deathmaidenkill
                                        ))
                                    {
                                        if (man.CurrentScene.SceneSNO.Id == this.CurrentScene.SceneSNO.Id) man.PlayActionAnimation(AnimationSno.x1_westmhub_guard_wispkilled_transform_01);
                                    }
                                }
                                break;
                            case ActorSno._x1_westm_intro_human_male2:
                                if (Position.X > 1300 & Position.Y > 440)
                                {
                                    StartConversation(World, 311435);
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
                                    foreach (var man in World.GetActorsBySNO(
                                        ActorSno._x1_westm_intro_human_male,
                                        ActorSno._x1_westm_intro_human_male2,
                                        ActorSno._x1_westm_intro_human_female
                                        ))
                                    {
                                        if (man.CurrentScene.SceneSNO.Id == this.CurrentScene.SceneSNO.Id) man.PlayActionAnimation(AnimationSno.x1_westmhub_guard_wispkilled_transform_01);
                                    }
                                    
                                    
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
