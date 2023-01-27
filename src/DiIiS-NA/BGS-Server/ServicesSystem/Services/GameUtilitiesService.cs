//Blizzless Project 2022
//Blizzless Project 2022 
using bgs.protocol;
//Blizzless Project 2022 
using bgs.protocol.game_utilities.v1;
//Blizzless Project 2022 
using bgs.protocol.notification.v1;
//Blizzless Project 2022 
using D3.GameMessage;
//Blizzless Project 2022 
using D3.Notification;
//Blizzless Project 2022 
using D3.OnlineService;
//Blizzless Project 2022 
using D3.Profile;
//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.AchievementSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Base;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Battle;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.GuildSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022 
using Google.ProtocolBuffers;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    public class InitialLoginTask
    {
        private BattleClient Client;
        public InitialLoginTask(BattleClient inClient)
        {
            Client = inClient;
        }

        public void run()
        {
            InitialLoginData.Builder Init = InitialLoginData.CreateBuilder();
            Init.SetOutstandingOrder(D3.Store.Order.CreateBuilder().SetAcknowledged(true).SetErrorCode(0).SetStatus(0).SetTransactionId(0));

            var GAS = D3.Client.GameAccountSettings.CreateBuilder()
                .SetShowDifficultySelector(false)
                .SetUseGameHandicapDeprecated(true)
                .SetSeasonJourneySeasonNumber(10)
                .SetViewedAnniversaryScreenYear(1)
                .SetAccountFlags(0)
                .SetAccountFlags((uint)D3.Account.Digest.Types.Flags.MASTER_DIFFICULTY_UNLOCKED)
                .SetAchievementsTimeLastViewed(DateTimeExtensions.ToUnixTime(DateTime.UtcNow))
                //.SetViewedAnniversaryScreenYear(1)
                .SetViewedWhatsNewVersion(20)
                .SetViewedWhatsNewSeason(20)
                .SetRmtLastUsedCurrency("PLATINUM")
                .SetRmtPreferredCurrency("PLATINUM")

                ;

            //Client.Account.GameAccount.DBGameAccount.ViewedNewVersion = 8;

            Init.SetChatRestrictionContentLicenseId(0);

            Init.SetAchievementsContentHandle(D3.OnlineService.ContentHandle.CreateBuilder().SetHash("20375546335DA13E31554A104FE036B5BCC878D715108F1FCEB50AB85BD87478").SetRegion("EU").SetUsage(".achu"));
            HeroDigestListResponse.Builder d = HeroDigestListResponse.CreateBuilder();
            foreach (Toon t in Client.Account.GameAccount.Toons)
            {
                d.AddDigestList(t.Digest);
                GAS.AddHeroListOrder(t.D3EntityID);
            }
            Init.SetGameAccountSettings(GAS);
            Init.SetHeroDigests(d);
            Init.SetAccountDigest(Client.Account.GameAccount.Digest);
            Init.SetSyncedVars(
                " OnlineService.Season.Num=1" + //Season number
                " OnlineService.Season.State=1" + //Season status, 1 - Activated, 0 - Deactivated
                " OnlineService.Leaderboard.Era=1" +
                " OnlineService.AnniversaryEvent.Status=1" + //Anniversary Event, 1st Old Tristam
                " ChallengeRift.ChallengeNumber=1" + //Rift portal number.
                " OnlineService.FreeToPlay=true" + //Shop for Platinum
                " OnlineService.Store.Status=0" + //Store Status, 0 - Enabled, 1 - Disabled
                " OnlineService.Store.ProductCatalogDigest=C42DC6117A7008EDA2006542D6C07EAD096DAD90" + //China
                " OnlineService.Store.ProductCatalogVersion=633565800390338000" + //Chinese Catalog
                                                                                  //" OnlineService.Store.ProductCatalogDigest=79162283AFACCBA5DA989BD341F7B782860AC1F4" + //Euro
                                                                                  //" OnlineService.Store.ProductCatalogVersion=635984100169931000" + //Euro
                " OnlineService.Region.Id=1"); //Region

            Init.SetSeenTutorials(ByteString.CopyFrom(Client.Account.GameAccount.DBGameAccount.SeenTutorials));
            Init.SetMatchmakingPool("Default");
            
            var guildInfo = D3.Guild.GuildInfoList.CreateBuilder();
            if (Client.Account.GameAccount.Clan != null || Client.Account.GameAccount.Communities.Count > 0)
            {
                //*
                if (Client.Account.GameAccount.Clan != null)
                {
                    var clan = Client.Account.GameAccount.Clan;
                    var clanInfo = D3.Guild.GuildInfo.CreateBuilder()
                        .SetGuildId(clan.PersistentId)
                        .SetGuildCategory(0)
                        .SetGuildLeaderId(clan.Owner.PersistentID)
                        .SetName(clan.FullName)
                        .SetSearchable(clan.IsLFM)
                        .SetMemberNewsTime(clan.NewsTime)
                        .AddValidatedMemberIds(Client.Account.GameAccount.PersistentID)
                        .SetRankId(clan.GetRank(Client.Account.GameAccount.PersistentID))
                        .SetTotalMembers((uint)clan.Members.Count);
                    guildInfo.AddGuilds(clanInfo);
                }
                foreach (var community in Client.Account.GameAccount.Communities)
                {
                    var communityInfo = D3.Guild.GuildInfo.CreateBuilder()
                        .SetGuildId(community.PersistentId)
                        .SetGuildLeaderId(community.Owner.PersistentID)
                        .SetGuildCategory(community.Category)
                        .SetName(community.Name)
                        .SetSearchable(community.IsLFM)
                        .SetMemberNewsTime(community.NewsTime)
                        .AddValidatedMemberIds(Client.Account.GameAccount.PersistentID)
                        .SetRankId(community.GetRank(Client.Account.GameAccount.PersistentID))
                        .SetTotalMembers((uint)community.Members.Count);
                    guildInfo.AddGuilds(communityInfo);
                }
                //*/
            }
            Init.SetGuilds(guildInfo);
            
            Init.SetGuildInvites(D3.Guild.InviteInfoList.CreateBuilder().AddRangeInvites(Client.Account.GameAccount.GuildInvites));


            Init.AddEras(EraInfo.CreateBuilder().SetId(0).SetNameDeprecated("TestEra"));
            Init.SetLogonTime(DateTimeExtensions.ToUnixTime(DateTime.UtcNow));
            Init.SetMissingEntitlements(D3.Store.MissingEntitlements.CreateBuilder().AddEntitlement(D3.Store.MissingEntitlement.CreateBuilder()));

            ContentLicenses.Builder licences = ContentLicenses.CreateBuilder();
            licences.AddLicenses(ContentLicense.CreateBuilder().SetId(0).SetQuantity(1));  // Diablo III
            licences.AddLicenses(ContentLicense.CreateBuilder().SetId(1).SetQuantity(1));  // Diablo III Reaper Of Souls - MENU
            licences.AddLicenses(ContentLicense.CreateBuilder().SetId(2).SetQuantity(1));  // Crusader
            licences.AddLicenses(ContentLicense.CreateBuilder().SetId(3).SetQuantity(1));  // ?
            licences.AddLicenses(ContentLicense.CreateBuilder().SetId(4).SetQuantity(1));  // Diablo III Reaper Of Souls - ACT V
            licences.AddLicenses(ContentLicense.CreateBuilder().SetId(5).SetQuantity(1));  // ?
            licences.AddLicenses(ContentLicense.CreateBuilder().SetId(6).SetQuantity(1));  // ?
            //licences.AddLicenses(ContentLicense.CreateBuilder().SetId(7).SetQuantity(1));  // + 6 Heroes Slots
            //licences.AddLicenses(ContentLicense.CreateBuilder().SetId(8).SetQuantity(1));  // + 2 Heroes Slots
            //licences.AddLicenses(ContentLicense.CreateBuilder().SetId(9).SetQuantity(1));  // + 3 Heroes Slots
            licences.AddLicenses(ContentLicense.CreateBuilder().SetId(10).SetQuantity(1));  // + 8 Heroes Slots
            //licences.AddLicenses(ContentLicense.CreateBuilder().SetId(11).SetQuantity(1));  // + 1 Heroes Slots
            licences.AddLicenses(ContentLicense.CreateBuilder().SetId(15).SetQuantity(1)); // Elite Edition
            licences.AddLicenses(ContentLicense.CreateBuilder().SetId(20).SetQuantity(1)); // Necromancer

            //licences.AddLicenses(ContentLicense.CreateBuilder().SetId(16).SetQuantity(1));  // Blue Booster
            //licences.AddLicenses(ContentLicense.CreateBuilder().SetId(17).SetQuantity(1));  // Gold Booster
            //licences.AddLicenses(ContentLicense.CreateBuilder().SetId(18).SetQuantity(5));  // Red Booster
            //.SetExpireTime(900000) - Delay in milliseconds)

            Init.SetContentLicenses(licences);

            // Build response
            InitialLoginDataResponse.Builder res = InitialLoginDataResponse.CreateBuilder();
            res.SetErrorCode(0U)
            .SetServiceId(Client.GuildChannelsRevealed ? 0U : 1U)
            .SetLoginData(Init);

            // Build notification
            bgs.protocol.notification.v1.Notification.Builder builder = bgs.protocol.notification.v1.Notification.CreateBuilder();
            builder.SetSenderId(bgs.protocol.EntityId.CreateBuilder().SetHigh(0).SetLow(0));
            builder.SetTargetAccountId(Client.Account.BnetEntityId);
            builder.SetTargetId(Client.Account.GameAccount.BnetEntityId);
            
            builder.SetType("D3.NotificationMessage");
            bgs.protocol.Attribute.Builder messageId = bgs.protocol.Attribute.CreateBuilder();
            messageId.SetName("D3.NotificationMessage.MessageId")
                    .SetValue(Variant.CreateBuilder().SetIntValue(1));  // InitialLoginDataResponse
            bgs.protocol.Attribute.Builder payload = bgs.protocol.Attribute.CreateBuilder();
            payload.SetName("D3.NotificationMessage.Payload")
                .SetValue(Variant.CreateBuilder().SetMessageValue(res.Build().ToByteString()));
            builder.AddAttribute(messageId);
            builder.AddAttribute(payload);

            Client.MakeRPC((lid) => NotificationListener.CreateStub(Client).OnNotificationReceived(new HandlerController() { ListenerId = lid }, builder.Build(), callback => { }));

            if (!Client.GuildChannelsRevealed)
            {
                Client.GuildChannelsRevealed = true;
                GuildManager.ReplicateGuilds(Client.Account.GameAccount);
            }
        }


    }

  
    [Service(serviceID: 0x38, serviceName: "bnet.protocol.game_utilities.GameUtilities"), ]
    public class GameUtilitiesService : bgs.protocol.game_utilities.v1.GameUtilitiesService, IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public static ulong counter { get; set; }

        public override void ProcessClientRequest(IRpcController controller, ClientRequest request, Action<ClientResponse> done)
        {
            ClientResponse.Builder builder = ClientResponse.CreateBuilder();
            var attr = bgs.protocol.Attribute.CreateBuilder();
            int messageId = (int)request.GetAttribute(1).Value.IntValue;


#if DEBUG
            if (messageId != 270)
                Logger.Debug("ProcessClientRequest() ID: {0}", messageId);
#endif
            switch (((HandlerController)controller).Client.Account.GameAccount.ProgramField.Value)
            {
                case "D3":
                    switch (messageId)
                    {
                        case 0:  // HeroDigestListRequest
                            ByteString digest = OnHeroDigestListRequest(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(digest));
                            break;
                        case 1:  // CreateHero
                            ByteString hero = OnHeroCreateParams(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            if (hero != null)
                                attr.SetValue(Variant.CreateBuilder().SetMessageValue(hero));
                            else
                                ((HandlerController)controller).Status = 17;
                            break;
                        case 2:  // DeleteHero
                            ByteString hero1 = OnHeroDeleteParams(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(hero1));
                            break;
                        case 3:  // Hero Select
                            ByteString switchHero = SwitchCharRequest(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(switchHero));
                            break;
                        case 4: //D3.GameMessages.SaveBannerConfiguration -> return MessageId with no Message
                            SaveBanner(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            var attrId = bgs.protocol.Attribute.CreateBuilder()
                                .SetName("CustomMessageId")
                                .SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue(5).Build())
                                .Build();
                            builder.AddAttribute(attrId);
                            break;
                        case 6:  // InitialLoginDataRequest -> InitialLoginDataQueuedResponse
                            ByteString loginData = OnInitialLoginDataRequest(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(loginData));
                            break;
                        case 7:
                            var getAccountSettings = GetGameAccountSettings(((HandlerController)controller).Client);
                            attr.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(getAccountSettings).Build());
                            break;
                        case 8:  
                            var setAccountSettings = SetGameAccountSettings(D3.GameMessage.SetGameAccountSettings.ParseFrom(request.GetAttribute(2).Value.MessageValue), ((HandlerController)controller).Client);
                            attr.SetValue(bgs.protocol.Variant.CreateBuilder().SetMessageValue(setAccountSettings).Build());
                            break;
                        case 9:  // GetToonSettings -> ToonSettings
                            ByteString toonSettings = OnGetToonSettings(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(toonSettings));
                            break;
                        case 10: // SetToonSettings -> Empty Message???
                            ByteString Current = GetAchievements(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(Current));
                            // attr.SetValue(Variant.CreateBuilder().SetMessageValue(ByteString.Empty));
                            break;
                        case 14: //D3.GameMessage.GetAccountProfiles
                            var getprofile1 = CollectProfiles(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(getprofile1));
                            //CollectHeroesProfiles
                            break;
                        case 15: //D3.GameMessage.GetHeroProfiles -> D3.Profile.HeroProfileList
                            //var gettoon = SelectToon(Client.Connect, request.GetAttribute(2).Value.MessageValue);
                            var heroprofs = GetHeroProfs(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(heroprofs));
                            break;
                        case 16:  // GetAccountPrefs -> (D3.Client.)Preferences
                            ByteString prefs = OnGetAccountPrefs(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(prefs));
                            break;
                        case 18: // 
                            //ByteString TestNB = TestRequest(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            //attr.SetValue(Variant.CreateBuilder().SetMessageValue(TestNB));
                            break;
                        case 19: // Getting all cosmetics
                            ByteString CurrentToon = GetCollectionAccout(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(CurrentToon));
                            break;
                        case 23: //Information about dead characters
                            var herodeadprofs = GetDeadedHeroProfs(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(herodeadprofs));

                            var attr2 = bgs.protocol.Attribute.CreateBuilder();
                            var heroAddedDigests = GetDeadedHeroDigests(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr2.SetValue(Variant.CreateBuilder().SetMessageValue(heroAddedDigests));
                            attr2.SetName("CustomMessage2");
                            builder.AddAttribute(attr2);
                            break;
                        case 27: //GetAchievementsInfo
                            //ByteString ClearActsResponse = ClearMissions(Client.Connect, request.GetAttribute(2).Value.MessageValue);
                            //attr.SetValue(Variant.CreateBuilder().SetMessageValue(ClearActsResponse));
                            break;
                        case 29: //Set Cosmetic
                            ByteString Current1 = SetCollection(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(Current1));
                            break;
                        case 32: //ChallengeRift Fetch HeroData
                            ByteString ChallengeRift = GetChallengeRift(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(ChallengeRift));
                            break;
                        case 33: //Rebirth
                            ByteString RebirthResponse = RebirthMethod(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(RebirthResponse));
                            break;
                        case 120: //Creation of a guild
                            ByteString CreatedGuild = CreateGuild(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(CreatedGuild));
                            break;
                        case 121: //TODO: Clan invitation
                            ByteString GSuggest = GuildInvit(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(GSuggest).Build());
                            break;
                        case 122: //Joining a Guild
                            ByteString InvitedToGuild = AcceptInviteToGuild(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(InvitedToGuild));
                            break;
                        case 124: //TODO: Clan message of the day
                            GuildSetMOTD(((HandlerController)controller).Client, D3.GameMessage.GuildSetMotd.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                            break;
                        case 125: //TODO: Add clan news
                            GuildSetNews(((HandlerController)controller).Client, D3.GameMessage.GuildSetNews.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                            break;
                        case 127: //Rank changes
                            break;
                        case 129: //Rank promotion
                            var promote = GuildPromoteMember(((HandlerController)controller).Client, D3.GameMessage.GuildPromoteMember.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(promote).Build());
                            break;
                        case 130: //Rank demotion
                            var demote = GuildDemoteMember(((HandlerController)controller).Client, D3.GameMessage.GuildDemoteMember.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(demote).Build());
                            break;
                        case 132: //Guild kick
                            ByteString ExitFrGuild = ExitFromGuild(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(ExitFrGuild));
                            break;
                        case 133: //Kick from the clan
                            GuildKickMemberP(((HandlerController)controller).Client, D3.GameMessage.GuildKickMember.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                            break;
                        case 134: //Disbanding a clan
                            GuildDisband(((HandlerController)controller).Client, D3.GameMessage.GuildId.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                            break;
                        case 138: //Guild creation
                            ByteString createCommunity = CreateCommunity(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(createCommunity));
                            break;
                        case 144: //TODO: Guild information
                            GuildSetDescription(((HandlerController)controller).Client, D3.GameMessage.GuildSetDescription.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                            break;
                        case 146: //TODO: 
                            var inviteList = GetInviteList(((HandlerController)controller).Client, D3.GameMessage.GuildId.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(inviteList).Build());
                            break;
                        case 148: //Request to join the guild
                            ByteString guildSuggest = GuildSuggest(((HandlerController)controller).Client, D3.GameMessage.GuildSuggest.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(guildSuggest));
                            //ByteString Invite = InviteToGuild(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            //attr.SetValue(Variant.CreateBuilder().SetMessageValue(Invite));
                            break;
                        case 149:
                            GuildSuggestionResponse(((HandlerController)controller).Client, D3.GameMessage.GuildSuggestionResponse.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                            break;
                        case 150:// Clan news
                            ByteString news = GuildFetchNews(((HandlerController)controller).Client, D3.GameMessage.GuildFetchNews.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(news).Build());
                            //GetInviteList(((HandlerController)controller).Client, D3.GameMessage.GuildId.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                            break;
                        case 152: //Cancel request to join the guild
                            GuildCancelInvite(((HandlerController)controller).Client, D3.GameMessage.GuildCancelInvite.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                            break;
                        case 156: //Guild settings change
                            GuildLFM(((HandlerController)controller).Client, D3.GameMessage.GuildSetLFM.ParseFrom(request.GetAttribute(2).Value.MessageValue));
                            break;
                        case 190: //Search Guild
                            ByteString guildSearch = SearchGuilds(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(guildSearch));
                            break;
                        case 200: //Rating
                            ByteString rating = GetRating(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(rating));
                            break;
                        case 201: //Rating1
                            ByteString ratingAlt = GetRatingAlt(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(ratingAlt));
                            break;
                        case 202: //personal rating
                            ByteString personalRating = GetRatingPersonal(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(personalRating));
                            break;
                        case 210:
                            ByteString storeResponse = CurrentStore(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(storeResponse));
                            break;
                        case 211:
                            ByteString storeResponse1 = CurrentStore1(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(storeResponse1));
                            break;
                        case 212: //Purchase
                            ByteString currentWalletResponse = CurrentWallet(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(currentWalletResponse));
                            break;
                        case 213: //Real currency
                            ByteString currentPrimaryWalletResponse = CurrentPrimaryWallet(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(currentPrimaryWalletResponse));
                            break;
                        case 216: //Collection
                            ByteString test = TestRequest(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(test));
                            break;
                        case 230: //GetAchievementSnapshot
                            var GetAchievementsSnapshot = CollectAchivementsSnapshot(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(GetAchievementsSnapshot));
                            break;
                        case 250: //Purchase
                            ByteString SwitchParams = SwitchParameters(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(SwitchParams));
                            break;
                        case 270: //Purchase
                            ByteString TestResponse = SendWarden3Custom(((HandlerController)controller).Client, request.GetAttribute(2).Value.MessageValue);
                            attr.SetValue(Variant.CreateBuilder().SetMessageValue(TestResponse));
                            break;
                    }
                    if (attr.HasValue)
                    {
                        attr.SetName("CustomMessage");
                        builder.AddAttribute(attr);
                    }
                    break;
            }
                    
            done(builder.Build());

            if (messageId == 6)
            {
                var LogTask = new InitialLoginTask(((HandlerController)controller).Client);
                LogTask.run();
            }

        }

        #region Service Methods
        public override void GetAchievementsFile(IRpcController controller, GetAchievementsFileRequest request, Action<GetAchievementsFileResponse> done)
        {
            throw new NotImplementedException();
        }
        public override void GetAllValuesForAttribute(IRpcController controller, GetAllValuesForAttributeRequest request, Action<GetAllValuesForAttributeResponse> done)
        {
            throw new NotImplementedException();
        }
        public override void GetPlayerVariables(IRpcController controller, GetPlayerVariablesRequest request, Action<GetPlayerVariablesResponse> done)
        {
            throw new NotImplementedException();
        }
        public override void OnGameAccountOffline(IRpcController controller, GameAccountOfflineNotification request, Action<NO_RESPONSE> done)
        {
            throw new NotImplementedException();
        }
        public override void OnGameAccountOnline(IRpcController controller, GameAccountOnlineNotification request, Action<NO_RESPONSE> done)
        {
            throw new NotImplementedException();
        }
        public override void PresenceChannelCreated(IRpcController controller, PresenceChannelCreatedRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }
        public override void ProcessServerRequest(IRpcController controller, ServerRequest request, Action<ServerResponse> done)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Diablo 3
        #region Rating system
        private ByteString GetRatingPersonal(BattleClient client, ByteString data)
        {
            var request = D3.GameMessage.LeaderboardGetHeroSnapshot.ParseFrom(data);
            var response = D3.GameMessage.LeaderboardGetHeroSnapshotResponse.CreateBuilder();
            bool season = false;
            bool hardcore = false;
            ToonClass neededClass = 0;
            switch (request.LeaderboardId)
            {
                case 1:
                    break;
                case 2: neededClass = ToonClass.Barbarian; break; // Barbarian
                case 3: neededClass = ToonClass.Crusader; break; // Crusader
                case 4: neededClass = ToonClass.DemonHunter; break; // Demon Hunter
                case 5: neededClass = ToonClass.Monk; break; // Monk
                case 6: neededClass = ToonClass.WitchDoctor; break; // Witch Doctor
                case 7: neededClass = ToonClass.Wizard; break; // Wizard
                case 8: neededClass = ToonClass.Necromancer; break; // Necromancer

                case 10: // deuces
                    break;
                case 11: // threes
                    break;
                case 12: // Fours
                    break;
                case 30: //Portal of Daring - 1 Player
                    break;
                case 31: //Portal of Daring - 2 Players
                    break;
                case 32: //Portal of Daring - 3 Players
                    break;
                case 33: //Portal of Daring - 4 Players
                    break;
                default:
                    break;
            }
            switch (request.ScopeId)
            {
                case 3: break; //Normal
                case 2: hardcore = true; break; //Hardcore
                case 5: season = true; break; //Seasonal
                case 4: season = true; hardcore = true; break; //Hardcore and Seasonal
            }

            List<DBGameAccount> gameAccounts = DBSessions.SessionQuery<DBGameAccount>().Where(a => a.Id == request.GameAccountId).ToList();
            var heroes = ToonManager.GetToonsForGameAccount(GameAccountManager.GetGameAccountByDBGameAccount(gameAccounts[0]));
            Toon hero = null;
            byte upLevel = 0;
            int idx = -1;
            if (heroes.Count > 0)
            {
                for (int i = 0; i < heroes.Count; i++)
                {
                    if (season && !heroes[i].isSeassoned) continue;
                    if (hardcore && !heroes[i].IsHardcore) continue;
                    if (heroes[i].Class != neededClass && (uint)neededClass != 0) continue;
                    if (upLevel > heroes[i].Level) continue;
                    upLevel = heroes[i].Level;
                    idx = i;
                }
                if (idx > -1)
                    hero = heroes[idx];
            }
            if (hero != null)
            {
                var Snapshot = D3.Leaderboard.HeroSnapshot.CreateBuilder()
                        .SetHeroId(hero.D3EntityID)
                        .AddCosmeticItems(D3.Leaderboard.HeroCosmeticItem.CreateBuilder().SetCosmeticVisualInventorySlot(1).SetGbid(hero.Cosmetic1))
                        .AddCosmeticItems(D3.Leaderboard.HeroCosmeticItem.CreateBuilder().SetCosmeticVisualInventorySlot(2).SetGbid(hero.Cosmetic2))
                        .AddCosmeticItems(D3.Leaderboard.HeroCosmeticItem.CreateBuilder().SetCosmeticVisualInventorySlot(3).SetGbid(hero.Cosmetic3))
                        .AddCosmeticItems(D3.Leaderboard.HeroCosmeticItem.CreateBuilder().SetCosmeticVisualInventorySlot(4).SetGbid(hero.Cosmetic4))
                        .SetActiveSkills(SkillsWithRunes.CreateBuilder()
                            .AddRunes(SkillWithRune.CreateBuilder().SetSkill(hero.DBActiveSkills.Skill0).SetRuneType(hero.DBActiveSkills.Rune0))
                            .AddRunes(SkillWithRune.CreateBuilder().SetSkill(hero.DBActiveSkills.Skill1).SetRuneType(hero.DBActiveSkills.Rune1))
                            .AddRunes(SkillWithRune.CreateBuilder().SetSkill(hero.DBActiveSkills.Skill2).SetRuneType(hero.DBActiveSkills.Rune2))
                            .AddRunes(SkillWithRune.CreateBuilder().SetSkill(hero.DBActiveSkills.Skill3).SetRuneType(hero.DBActiveSkills.Rune3))
                            .AddRunes(SkillWithRune.CreateBuilder().SetSkill(hero.DBActiveSkills.Skill4).SetRuneType(hero.DBActiveSkills.Rune4))
                            .AddRunes(SkillWithRune.CreateBuilder().SetSkill(hero.DBActiveSkills.Skill5).SetRuneType(hero.DBActiveSkills.Rune5)))
                        .SetActiveTraits(PassiveSkills.CreateBuilder().AddSnoTraits(hero.DBActiveSkills.Passive0).AddSnoTraits(hero.DBActiveSkills.Passive1).AddSnoTraits(hero.DBActiveSkills.Passive2).AddSnoTraits(hero.DBActiveSkills.Passive3));

                foreach (var item in hero.Profile.Equipment.ItemsList)
                {
                    int pos = 0;
                    switch ((item.ItemSlot - 272) / 16)
                    {
                        case 1: pos = 0; break; //0 - Helmet
                        case 2: pos = 1; break; //1 - Armor
                        case 3: pos = 5; break; //5 - Shield
                        case 4: pos = 4; break; //4 - Main hand
                        case 5: pos = 3; break; //3 - Gloves
                        //case 6: pos = 8; break; // - belt
                        case 7: pos = 2; break; //2 - Boots
                        case 8: pos = 6; break; //6 - Shoulders
                        case 9: pos = 7; break; //7 - Pants
                         default:
                            int s = (item.ItemSlot - 272) / 16;
                            pos = 9;
                            break;
                    }
                    Snapshot.AddEquippedItems(D3.Leaderboard.HeroEquippedItem.CreateBuilder()
                        .SetGenerator(item.Generator)
                        .SetVisualInventorySlot(pos)
                        );
                    
                }  
                ;
                //Snapshot.AddEquippedItems(D3.Leaderboard.HeroEquippedItem.CreateBuilder());

                response.SetSnapshot(Snapshot);
            }
            return response.Build().ToByteString();
        }
        private ByteString GetRatingAlt(BattleClient client, ByteString data)
        {
            var request = D3.GameMessage.LeaderboardFetchScores.ParseFrom(data);
            var response = LeaderboardFetchScoresResponse.CreateBuilder();
            bool season = false;
            bool hardcore = false;
            ToonClass neededClass = ToonClass.Unknown;
            switch (request.LeaderboardId)
            {
                case 1:
                    break;
                case 2: neededClass = ToonClass.Barbarian; break; // Barbarian.
                case 3: neededClass = ToonClass.Crusader; break; // Crusader.
                case 4: neededClass = ToonClass.DemonHunter; break; // Demon Hunter.
                case 5: neededClass = ToonClass.Monk; break; // Monk.
                case 6: neededClass = ToonClass.WitchDoctor; break; // Warlock.
                case 7: neededClass = ToonClass.Wizard; break; // Wizard.
                case 8: neededClass = ToonClass.Necromancer; break; // Necromancer.

                case 10: // ToonClass.
                    break;
                case 11: // Fours.
                    break;
                case 12: // Fours.
                    break;
                case 30: //Daring Portal - 1 Player
                case 31: //Daring Portal - 2 Players
                case 32: //Daring Portal - 3 Players
                case 33: //Daring Portal - 4 Players
                default:
                    break;
            }
            switch (request.ScopeId)
            {
                case 3: break; //Normal
                case 2: hardcore = true; break; //Hardcore
                case 5: season = true; break; //Seasonal
                case 4: season = true; hardcore = true; break; //Seasonal Hardcore
            }
            //foreach (var gameaccount in request.GameAccountIdsList)
            
            //var DBGA = DBSessions.SessionQuery<DBGameAccount>().Where(a => a.Id == gameaccount).First();
            List<DBGameAccount> preGameAccounts = DBSessions.SessionQuery<DBGameAccount>().ToList();
            
            
            foreach (var gameAccountId in request.GameAccountIdsList)
            {

                DBGameAccount gameAccount = DBSessions.SessionQuery<DBGameAccount>().First(a => a.Id == gameAccountId);

                var Heroes = ToonManager.GetToonsForGameAccount(GameAccountManager.GetGameAccountByDBGameAccount(gameAccount));
                Toon Hero = null;
                byte upLevel = 0;
                int idx = -1;
                if (Heroes.Count > 0)
                {
                    for (int i = 0; i < Heroes.Count; i++)
                    {
                        if (season && !Heroes[i].isSeassoned) continue;
                        if (hardcore && !Heroes[i].IsHardcore) continue;
                        if (Heroes[i].Class != neededClass && neededClass !=  ToonClass.Unknown) continue;
                        if (upLevel > Heroes[i].Level) continue;
                        upLevel = Heroes[i].Level;
                        idx = i;
                    }
                    if (idx > -1)
                        Hero = Heroes[idx];
                }
                if (Hero != null)
                    try
                    {
                        GameAccount gameAccount2 = GameAccountManager.GetGameAccountByDBGameAccount(gameAccount);
                        Account account = AccountManager.GetAccountByPersistentID(gameAccount2.AccountId);
                        var member = D3.Leaderboard.Member.CreateBuilder()
                                                .SetAccountId(gameAccount2.AccountId)
                                                .SetHeroSeasonCreated((uint)Hero.SeasonCreated)
                                                .SetBattleTag(account.BattleTagName)
                                                .SetHeroAltLevel((uint)gameAccount2.DBGameAccount.ParagonLevel)
                                                .SetHeroFlags((uint)Hero.Flags)
                                                .SetHeroLevel((uint)Hero.Level)
                                                .SetHeroGbidClass((uint)Hero.ClassID)
                                                .SetHeroName(Hero.Name)
                                                .SetHeroSnapshotAvailable(true)
                                                .SetHeroVisualEquipment(gameAccount2.Toons[0].Digest.VisualEquipment);
                        if (gameAccount2.Clan != null)
                        {
                            member.SetClanId(gameAccount2.Clan.GuildId.GuildId_).SetClanTag(gameAccount2.Clan.Prefix).SetClanName(gameAccount2.Clan.Name);
                        }
                        response
                            .AddEntry(D3.Leaderboard.Score.CreateBuilder()
                            .SetGameAccountId(gameAccount2.AccountId)
                            .SetScore_((ulong)Hero.Level + (ushort)gameAccount2.DBGameAccount.ParagonLevel) // Temporary Separation.
                            .SetScoreBand(5)
                            .SetLeaderboardId(5)
                            .SetScopeId(5)
                            .SetTimestamp(DateTimeExtensions.ToUnixTime(DateTime.UtcNow))
                            .SetMetadata(D3.Leaderboard.Metadata.CreateBuilder()
                                .SetAct1TimeMs(0)
                                .SetAct2TimeMs(0)
                                .SetAct3TimeMs(0)
                                .SetAct4TimeMs(0)
                                .SetAct5TimeMs(0)
                                .SetLevelSeed(0)
                                .SetCheated(false)
                                .AddTeamMember(member)
                                .SetChallengeData(D3.Leaderboard.WeeklyChallengeData.CreateBuilder()
                                    .SetBnetAccountId(unchecked((uint)account.BnetEntityId.Low))
                                    .SetGameAccountId(GameAccountHandle.CreateBuilder().SetId(unchecked((uint)gameAccount2.BnetEntityId.Low)).SetProgram(17459).SetRegion(1))
                                    .SetHeroSnapshot(D3.Hero.SavedDefinition.CreateBuilder().SetVersion(905)
                                            .SetDigest(Hero.Digest)
                                            .SetSavedAttributes(D3.AttributeSerializer.SavedAttributes.CreateBuilder()))
                                    .SetAccountSnapshot(D3.Account.SavedDefinition.CreateBuilder().SetVersion(905)
                                        .SetDigest(gameAccount2.Digest))
                                    .SetRiftSnapshot(D3.Leaderboard.RiftSnapshot.CreateBuilder()
                                        .SetRiftSeed(2342341)
                                        .SetRiftTier(1)
                                        .SetSnoDungeonFinder(1)
                                        .SetSnoBoss(1)
                                        .SetDeprecatedCompletionSeconds(10)
                                        .SetNumDeaths(1)))
                                ));


                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorException(ex, "Error while creating leaderboard entry");
                    }
            }
            return response.Build().ToByteString();
        }
        private ByteString GetRating(BattleClient client, ByteString data)
        {
            LeaderboardList request = LeaderboardList.ParseFrom(data);
            bool season = false;
            bool hardcore = false;
            ToonClass neededClass = ToonClass.Unknown;
            switch (request.LeaderboardId)
            {
                case 1: 
                    break;
                case 2: neededClass = ToonClass.Barbarian; break; // Barbarian
                case 3: neededClass = ToonClass.Crusader; break; // Crusader
                case 4: neededClass = ToonClass.DemonHunter; break; // Demon Hunter
                case 5: neededClass = ToonClass.Monk; break; // Monk
                case 6: neededClass = ToonClass.WitchDoctor; break; // Witch Doctor
                case 7: neededClass = ToonClass.Wizard; break; // Wizard
                case 8: neededClass = ToonClass.Necromancer; break; // Necromancer

                case 10: // Duples
                    break;
                case 11: // Tripples
                    break;
                case 12: // Quads
                    break;
                case 30: //Daring portal - 1 Player
                case 31: //Daring portal - 2 Players
                case 32: //Daring portal - 3 Players
                case 33: //Daring portal - 4 Players
                default:
                    break;
            }
            switch (request.ScopeId)
            {
                case 3: break; //Normal
                case 2: hardcore = true; break; //Hardcore
                case 5: season = true; break; //Seasonal
                case 4: season = true; hardcore = true; break; //Seasonal + Harcore
            }

            var result = LeaderboardListResponse.CreateBuilder()
                .SetLimit(request.Limit)
                .SetOffset(request.Offset)
                .SetVersion(request.Version)
                ;

            List<DBGameAccount> gameAccounts = DBSessions.SessionQuery<DBGameAccount>().ToList();
            foreach (var gameAccountId in gameAccounts)
            {
                var toons = ToonManager.GetToonsForGameAccount(
                    GameAccountManager.GetGameAccountByDBGameAccount(gameAccountId));
                Toon hero = null;
                byte upLevel = 0;
                int idx = -1;
                if (toons.Count > 0)
                {
                    for (int i = 0; i < toons.Count; i++)
                    {
                        if (season && !toons[i].isSeassoned) continue;
                        if (hardcore && !toons[i].IsHardcore) continue;
                        if (toons[i].Class != neededClass && neededClass != ToonClass.Unknown) continue;
                        if (upLevel > toons[i].Level) continue;
                        upLevel = toons[i].Level;
                        idx = i;
                    }

                    if (idx > -1)
                        hero = toons[idx];
                }

                if (hero != null)
                    try
                    {
                        GameAccount gameAccount = GameAccountManager.GetGameAccountByDBGameAccount(gameAccountId);
                        Account account = AccountManager.GetAccountByPersistentID(gameAccount.AccountId);
                        var member = D3.Leaderboard.Member.CreateBuilder()
                            .SetAccountId((uint)gameAccount.D3GameAccountId.IdLow)
                            .SetHeroSeasonCreated((uint)hero.SeasonCreated)
                            .SetBattleTag(account.BattleTagName)
                            .SetHeroAltLevel((uint)gameAccount.DBGameAccount.ParagonLevel)
                            .SetHeroFlags((uint)hero.Flags)
                            .SetHeroLevel((uint)hero.Level)
                            .SetHeroGbidClass((uint)hero.ClassID)
                            .SetHeroName(hero.Name)
                            .SetHeroSnapshotAvailable(true)
                            .SetHeroVisualEquipment(gameAccount.Toons[0].Digest.VisualEquipment);
                        if (gameAccount.Clan != null)
                        {
                            member.SetClanId(gameAccount.Clan.GuildId.GuildId_).SetClanTag(gameAccount.Clan.Prefix)
                                .SetClanName(gameAccount.Clan.Name);
                        }

                        result
                            .AddEntry(D3.Leaderboard.Slot.CreateBuilder()
                                .SetGameAccountId(gameAccount.AccountId)
                                //TODO: Need to implement the calculation from the time of passage of the portal!
                                .SetScore((ulong)hero.Level +
                                          (ulong)gameAccount.DBGameAccount.ParagonLevel) //temporary separation
                                .SetTimestamp(DateTime.UtcNow.ToUnixTime())
                                .SetMetadata(D3.Leaderboard.Metadata.CreateBuilder()
                                    .SetAct1TimeMs(0)
                                    .SetAct2TimeMs(0)
                                    .SetAct3TimeMs(0)
                                    .SetAct4TimeMs(0)
                                    .SetAct5TimeMs(0)
                                    .SetLevelSeed(1)

                                    .SetChallengeData(D3.Leaderboard.WeeklyChallengeData.CreateBuilder()
                                        .SetBnetAccountId(unchecked((uint)account.BnetEntityId.Low))
                                        .SetGameAccountId(GameAccountHandle.CreateBuilder()
                                            .SetId(unchecked((uint)gameAccount.BnetEntityId.Low)).SetProgram(17459)
                                            .SetRegion(1))
                                        .SetHeroSnapshot(D3.Hero.SavedDefinition.CreateBuilder().SetVersion(905)
                                            .SetDigest(hero.Digest)
                                            .SetSavedAttributes(D3.AttributeSerializer.SavedAttributes.CreateBuilder()))
                                        .SetAccountSnapshot(D3.Account.SavedDefinition.CreateBuilder().SetVersion(905)
                                            .SetDigest(gameAccount.Digest)
                                        )
                                        .SetRiftSnapshot(D3.Leaderboard.RiftSnapshot.CreateBuilder()
                                            .SetRiftSeed(2342341)
                                            .SetRiftTier(1)
                                            .SetSnoDungeonFinder(1)
                                            .SetSnoBoss(1)
                                            .SetNumDeaths(1)
                                            .SetCompletionMilliseconds(5000)
                                            .SetDeprecatedCompletionSeconds(500)
                                            .AddFloors(D3.Leaderboard.RiftFloor.CreateBuilder().SetSnoWorld(-1)
                                                .SetPopulationHash(10))
                                        )
                                    )
                                    //*/
                                    .SetCheated(false)
                                    .AddTeamMember(member)
                                ));
                    }
                    catch
                    {
                    }
            }


            result.SetLimit(1004).SetTotalLeaderboardEntries((uint)result.EntryCount);

            result.SetTotalLeaderboardEntries((uint)gameAccounts.Count);
            return result.Build().ToByteString();
        }
        #endregion
        #region Character List Request
        private ByteString OnHeroDigestListRequest(BattleClient client, ByteString data)
        {
            HeroDigestListRequest request = HeroDigestListRequest.ParseFrom(data);
            HeroDigestListResponse.Builder builder = HeroDigestListResponse.CreateBuilder();
            foreach (var toon in request.ToonIdList)
            {
                builder.AddDigestList(ToonManager.GetToonByLowID(toon).Digest);
            }
            return builder.Build().ToByteString();
        }
        #endregion
        #region Create/Delete/Switch Character
        private ByteString OnHeroCreateParams(BattleClient client, ByteString data)
        {
            HeroCreateParams createParams = HeroCreateParams.ParseFrom(data);

            //for (int i = 0; i < Client.Account.GameAccount.DBGameAccount.DBToons.Count; i++)
            //    if (Client.Account.GameAccount.DBGameAccount.DBToons[i].Name.ToLower() == createPrams.Name.ToLower())
            //        return null;

            //var newToon = ToonManager.CreateNewToon(createPrams.Name, createPrams.GbidClass, createPrams.IsHardcore, createPrams.IsFemale ? 0x02 : (uint)0x00, 1, Client.Account.GameAccount, createPrams.IsSeason ? (uint)1 : (uint)0);
            var newToon = ToonManager. CreateNewToon(createParams.Name, createParams.GbidClass, createParams.IsFemale ? ToonFlags.Female : ToonFlags.Male, 1, createParams.IsHardcore, client.Account.GameAccount, createParams.IsSeason ? 1 : 0);
            return CreateHeroResponse.CreateBuilder().SetHeroId(newToon.D3EntityID.IdLow).Build().ToByteString();
        }
        private ByteString OnHeroDeleteParams(BattleClient client, ByteString data)
        {
            var deleteParams = DeleteHero.ParseFrom(data);
            var toon = ToonManager.GetToonByLowID(deleteParams.HeroId);
            ToonManager.DeleteToon(toon);
            return ByteString.Empty;
        }
        private ByteString SwitchCharRequest(BattleClient client, ByteString data)
        {
            var request = D3.GameMessage.GetToonSettings.ParseFrom(data);

            var oldToon = client.Account.GameAccount.CurrentToon;
            var newToon = ToonManager.GetToonByLowID(request.HeroId);

            if (oldToon != newToon)
            {
                client.Account.GameAccount.CurrentToon = newToon;
                client.Account.GameAccount.NotifyUpdate();
                //AccountManager.SaveToDB(Client.Account);
                //Client.Account.GameAccount.Setted = true;
            }

            var response = D3.GameMessage.SelectHero.CreateBuilder().SetHeroId(request.HeroId);
            return response.Build().ToByteString();
        }
        #endregion
        #region Login initialization
        private ByteString OnInitialLoginDataRequest(BattleClient client, ByteString data)
        {
            var req = InitialLoginDataRequest.ParseFrom(data);

            InitialLoginDataQueuedResponse.Builder res = InitialLoginDataQueuedResponse.CreateBuilder();
            res.SetServiceId(1)
                .SetTimeoutTickInterval(2000);

            return res.Build().ToByteString();
        }
        #endregion
        #region Banner editing
        private bool SaveBanner(BattleClient client, ByteString data)
        {
            Logger.Debug("SaveBannerConfiguration()");
            //var bannerConfig = HeroDigestBanner.ParseFrom(data);
            var bannerConfig = SaveBannerConfiguration.ParseFrom(data);

            if (client.Account.GameAccount.BannerConfigurationField.Value == bannerConfig.Banner)
                return false;
            else
            {
                client.Account.GameAccount.BannerConfiguration = bannerConfig.Banner;
                client.Account.GameAccount.NotifyUpdate();
            }

            return true;

        }
        #endregion
        #region Guild system

        private ByteString CreateGuild(BattleClient client, ByteString data)
        {
            GuildCreate request = GuildCreate.ParseFrom(data);
            var guild = GuildManager.CreateNewGuild(client.Account.GameAccount, request.Name, request.Tag, true, 0, request.LookingForMembers, request.Language);
            if (guild != null)
            {
                return guild.GuildId.ToByteString();
            }
            else
                return ByteString.Empty;
        }
        private ByteString SearchGuilds(BattleClient client, ByteString data)
        {
            GuildSearch request = GuildSearch.ParseFrom(data);
            Logger.Debug("GuildSearch(): {0}", request.ToString());
            var builder = D3.Guild.GuildSearchResultList.CreateBuilder();

            List<Guild> allGuilds = request.ClanOrGroup == 1 ? GuildManager.GetCommunities() : GuildManager.GetClans();
            foreach (var guild in allGuilds)
            {
                if (guild.Disbanded) continue;
                if (request.HasName && !guild.Name.ToLower().Contains(request.Name.ToLower())) continue;

                var guildData = D3.Guild.GuildSearchResult.CreateBuilder()
                    .SetGuildId(guild.PersistentId)
                    .SetGuildName(guild.FullName)
                    .SetCategory(guild.Category)
                    .SetRequiresInvite(guild.IsInviteRequired)
                    .SetTotalMembers((uint)guild.Members.Count)
                    .SetLastActivity(guild.NewsTime)
                    .SetLastOfficerActivity(guild.NewsTime)
                    .SetLanguage(guild.Language);

                if (guild.Prefix != "")
                    guildData.SetGuildTag(guild.Prefix);

                builder.AddResults(guildData);
            }

            return builder.Build().ToByteString();
        }

        private ByteString AcceptInviteToGuild(BattleClient client, ByteString data)
        {
            GuildInviteResponse request = GuildInviteResponse.ParseFrom(data);

            var guild = GuildManager.GetGuildById(request.GuildId);
            var gameAccount = client.Account.GameAccount;
            if (guild != null)
            {
                if (request.Result == false)
                {
                    gameAccount.GuildInvites.RemoveAll(i => i.GuildId == guild.PersistentId);
                }
                else
                {
                    if (gameAccount.GuildInvites.Any(i => i.GuildId == guild.PersistentId) || !guild.IsClan)
                    {
                        gameAccount.GuildInvites.RemoveAll(i => i.GuildId == guild.PersistentId);
                        guild.AddMember(gameAccount);
                    }
                }
            }

            return ByteString.Empty;
        }
        private void AcceptInviteToGuild(BattleClient client, GuildInviteResponse request)
        {
            var guild = GuildManager.GetGuildById(request.GuildId);
            var gameAccount = client.Account.GameAccount;
            if (guild != null)
            {
                if (request.Result == false)
                {
                    gameAccount.GuildInvites.RemoveAll(i => i.GuildId == guild.PersistentId);
                }
                else
                {
                    if (gameAccount.GuildInvites.Any(i => i.GuildId == guild.PersistentId) || !guild.IsClan)
                    {
                        gameAccount.GuildInvites.RemoveAll(i => i.GuildId == guild.PersistentId);
                        guild.AddMember(gameAccount);
                    }
                }
            }
        }
        private void GuildKickMemberP(BattleClient client, D3.GameMessage.GuildKickMember request)
        {
            Logger.Debug("GuildKickMember(): {0}", request.ToString());

            var guild = GuildManager.GetGuildById(request.GuildId);
            if (guild != null && client.Account.GameAccount.PersistentID == guild.Owner.PersistentID)
            {
                var gameAccount = GameAccountManager.GetAccountByPersistentID(request.MemberId);
                guild.RemoveMember(gameAccount);
            }

        }
        private void GuildDisband(BattleClient client, D3.GameMessage.GuildId request)
        {
            Logger.Debug("GuildDisband(): {0}", request.ToString());

            var guild = GuildManager.GetGuildById(request.GuildId_);
            if (guild != null && client.Account.GameAccount.PersistentID == guild.Owner.PersistentID)
            {
                guild.Disband();
            }
        }
        private ByteString ExitFromGuild(BattleClient client, ByteString data)
        {
            
            return ByteString.Empty;
        }
        private ByteString GuildFetchNews(BattleClient client, D3.GameMessage.GuildFetchNews request)
        {
            Logger.Debug("GuildFetchNews(): {0}", request.ToString());
            var builder = D3.Guild.NewsList.CreateBuilder();

            /* news types:
			0 - item looted + D3.Items.Generator
			1 - achievement + D3.Guild.AchievementNews
			2 - joined
			3 - left
			4 - promoted
			5 - changed MOTD
			6 - changed info
			7 - changed leader
			8 - manual post + D3.Guild.NewsPost
			*/
            var guild = GuildManager.GetGuildById(request.GuildId);
            if (guild != null)
            {
                var guild_news = DBSessions.SessionQueryWhere<DBGuildNews>(n => n.DBGuild.Id == guild.PersistentId);
                foreach (var news in guild_news)
                {
                    if (news.Time < request.NewsTime) continue;
                    var post = D3.Guild.News.CreateBuilder().SetNewsId(news.Id).SetAccountId(news.DBGameAccount.Id).SetNewsType((uint)news.Type).SetNewsTime(news.Time);
                    if (news.Type < 2 || news.Type == 8)
                        post.SetNewsData(ByteString.CopyFrom(news.Data));
                    builder.AddNewsProp(post);
                    //builder.AddPosts(post);
                }
                return builder.Build().ToByteString();
            }
            else
                return ByteString.Empty;
        }

        private ByteString GuildPromoteMember(BattleClient client, D3.GameMessage.GuildPromoteMember request)
        {
            Logger.Debug("GuildPromoteMember(): {0}", request.ToString());

            var guild = GuildManager.GetGuildById(request.GuildId);
            var account = GameAccountManager.GetAccountByPersistentID(request.MemberId);
            if (guild != null && guild.HasMember(account) && guild.HasMember(client.Account.GameAccount))
            {
                guild.PromoteMember(account);
                var builder = D3.Guild.Member.CreateBuilder()
                    .SetAccountId(account.PersistentID)
                    .SetRankId(guild.GetRank(account.PersistentID))
                    .SetNote(guild.GetMemberNote(account.PersistentID))
                    .SetNewsTime(guild.NewsTime)
                    .SetAchievementPoints(account.AchievementPoints);
                return builder.Build().ToByteString();
            }
            else
                return ByteString.Empty;
        }

        private ByteString GuildDemoteMember(BattleClient client, D3.GameMessage.GuildDemoteMember request)
        {
            Logger.Debug("GuildDemoteMember(): {0}", request.ToString());

            var guild = GuildManager.GetGuildById(request.GuildId);
            var account = GameAccountManager.GetAccountByPersistentID(request.MemberId);
            if (guild != null && guild.HasMember(account) && guild.HasMember(client.Account.GameAccount))
            {
                guild.DemoteMember(account);
                var builder = D3.Guild.Member.CreateBuilder()
                    .SetAccountId(account.PersistentID)
                    .SetRankId(guild.GetRank(account.PersistentID))
                    .SetNote(guild.GetMemberNote(account.PersistentID))
                    .SetNewsTime(guild.NewsTime)
                    .SetAchievementPoints(account.AchievementPoints);
                return builder.Build().ToByteString();
            }
            else
                return ByteString.Empty;
        }

        private ByteString InviteToGuild(BattleClient client, ByteString data)
        {
            GuildInvite request = GuildInvite.ParseFrom(data);
            
            ;
            var guild = GuildManager.GetGuildById(request.GuildId);
            if (guild != null)
            {
                var gameAccount = GameAccountManager.GetAccountByPersistentID(request.InviteeId);
                gameAccount.InviteToGuild(guild, client.Account.GameAccount);
            }

            return ByteString.Empty;
        }
        private void GuildSetMOTD(BattleClient client, D3.GameMessage.GuildSetMotd request)
        {
            var guild = GuildManager.GetGuildById(request.GuildId);
            if (guild != null && guild.HasMember(client.Account.GameAccount))
            {
                guild.MOTD = request.Motd;
                var dbGuild = guild.DBGuild;
                dbGuild.MOTD = request.Motd;
                DBSessions.SessionUpdate(dbGuild);
                guild.AddNews(client.Account.GameAccount, 5);
            }
        }

        private void GuildSetNews(BattleClient client, D3.GameMessage.GuildSetNews request)
        {
             var guild = GuildManager.GetGuildById(request.GuildId);
            if (guild != null && guild.HasMember(client.Account.GameAccount))
            {
                guild.AddNews(client.Account.GameAccount, 8, request.NewsData.ToByteArray());
            }
            
        }
        private ByteString GuildInvit(BattleClient client, ByteString data)
        {
            var test = GuildInvite.ParseFrom(data);
            var guild = GuildManager.GetGuildById(test.GuildId);
            if (guild != null)
            {
                var acc = AccountManager.GetAccountByBattletag(test.BattleTagOrEmail);
                if (acc == null)
                    acc = AccountManager.GetAccountByEmail(test.BattleTagOrEmail);

                var gameAccount = GameAccountManager.GetAccountByPersistentID(acc.CurrentGameAccountId);
                gameAccount.InviteToGuild(guild, client.Account.GameAccount);
            }
            return ByteString.Empty;
        }
        private ByteString GuildSuggest(BattleClient client, D3.GameMessage.GuildSuggest request)
        {
            var gameAccount = GameAccountManager.GetAccountByPersistentID(request.OtherAccountId);

            var guild = GuildManager.GetGuildById(request.GuildId);
            if (guild != null && guild.IsLFM)
            {
                guild.AddSuggestion(gameAccount, client.Account.GameAccount);
                gameAccount.InviteToGuild(guild, client.Account.GameAccount);
                return D3.Guild.InviteInfo.CreateBuilder()
                    .SetGuildId(guild.PersistentId)
                    .SetGuildName(guild.Name)
                    .SetInviterId(client.Account.GameAccount.PersistentID)
                    .SetCategory(guild.Category)
                    .SetInviteType(1U)
                    .SetExpireTime(3600).Build().ToByteString();
            }
            return ByteString.Empty;
        }
        private void GuildSetDescription(BattleClient client, D3.GameMessage.GuildSetDescription request)
        {
      
            var guild = GuildManager.GetGuildById(request.GuildId);
            if (guild != null && guild.HasMember(client.Account.GameAccount))
            {
                guild.Description = request.Description;
                var dbGuild = guild.DBGuild;
                dbGuild.Description = request.Description;
                DBSessions.SessionUpdate(dbGuild);
                guild.AddNews(client.Account.GameAccount, 6);
            }
        }
        private void GuildSuggestionResponse(BattleClient client, D3.GameMessage.GuildSuggestionResponse request)
        {
            var builder = D3.GameMessage.GuildSuggestionResponse.CreateBuilder();
            var gameAccount = GameAccountManager.GetAccountByPersistentID(request.InviteeId);

            var guild = GuildManager.GetGuildById(request.GuildId);
            if (guild != null)
            {
                guild.RemoveSuggestion(gameAccount);
                gameAccount.GuildInvites.RemoveAll(i => i.GuildId == guild.PersistentId);
                if (request.Result)
                    gameAccount.InviteToGuild(guild, client.Account.GameAccount);
            }
        }
        private ByteString GetInviteList(BattleClient client, D3.GameMessage.GuildId guild_id)
        {
            var builder = D3.Guild.InviteList.CreateBuilder();

            var guild = GuildManager.GetGuildById(guild_id.GuildId_);
            if (guild != null && guild.HasMember(client.Account.GameAccount))
            {
                foreach (var invite in guild.GuildSuggestions)
                    builder.AddInvites(invite);
                return builder.Build().ToByteString();
            }
            else
                return ByteString.Empty;
        }
        private void GuildCancelInvite(BattleClient client, D3.GameMessage.GuildCancelInvite request)
        {
            
            var guild = GuildManager.GetGuildById(request.GuildId);
            if (guild != null)
            {
                var gameAccount = GameAccountManager.GetAccountByPersistentID(request.AccountId);
                gameAccount.GuildInvites.RemoveAll(i => i.GuildId == guild.PersistentId);
                guild.GuildSuggestions.RemoveAll(i => i.AccountId == request.AccountId);
            }
        }
        private void GuildLFM(BattleClient client, D3.GameMessage.GuildSetLFM request)
        {
           
            var guild = GuildManager.GetGuildById(request.GuildId);
            if (guild != null && guild.HasMember(client.Account.GameAccount))
            {
                guild.IsLFM = request.Lfm;
                var dbGuild = guild.DBGuild;
                dbGuild.IsLFM = request.Lfm;
                DBSessions.SessionUpdate(dbGuild);
                guild.UpdateChannelAttributes();
            }
            
        }
        private ByteString CreateCommunity(BattleClient client, ByteString data)
        {
            var request = GroupCreate.ParseFrom(data);
            Logger.Debug("CreateCommunity(): {0}", request.ToString());

            var guild = GuildManager.CreateNewGuild(client.Account.GameAccount, request.Name, "", false, request.SearchCategory, false, request.Language);
            if (guild != null)
            {
                return guild.GuildId.ToByteString();
            }
            else
                return ByteString.Empty;
        }
        #endregion
        #region Account statistics
        private ByteString CollectProfiles(BattleClient client, ByteString data)
        {
            var request = D3.GameMessage.GetAccountProfile.ParseFrom(data);
            var account = GameAccountManager.GetAccountByPersistentID(request.AccountId.Id);
            if (request.SeasonId != 0)
            {
                return account.Profile.ToByteString(); //return account.SeasonProfile.ToByteString();
            }
            else
            {

                return account.Profile.ToByteString();
            }

        }
        private ByteString CollectAchivementsSnapshot(BattleClient client, ByteString data)
        {
            var request = D3.GameMessage.AchievementsGetSnapshot.ParseFrom(data);
            var snapshot = D3.Achievements.Snapshot.CreateBuilder();

            foreach (var achievement in client.Account.GameAccount.Achievements)
            {
                if (achievement.AchievementId == 1)
                {
                    /*
                    AchievementCriteria.Add(CriteriaUpdateRecord.CreateBuilder()
                            .SetCriteriaId32AndFlags8(3367569)
                            .SetQuantity32((uint)ach.Criteria.Count())
                            .Build()
                            );
                    //*/
                }
                snapshot.AddAchievementSnapshot(achievement);
            }
            foreach (var criteria in client.Account.GameAccount.AchievementCriteria)
            {
                uint countOfTravels = 0;
                if (criteria.CriteriaId32AndFlags8 == 3367569)
                    countOfTravels++;
                snapshot.AddCriteriaSnapshot(criteria);
            }


            return AchievementsSnapshot.CreateBuilder().SetErrorCode(0).SetGameAccountId(request.GameAccountId).SetSnapshot(snapshot).Build().ToByteString();
        }

        #endregion
        #region Collection and store
        private ByteString GetCollectionAccout(BattleClient client, ByteString data)
        {
            var C = D3.CosmeticItems.CosmeticItems.CreateBuilder();
            #region All items
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587061))); // Angelic
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587101))); // Aranea
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2586361111))); // Barbarian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2350702305))); // Base Frame
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2608693639))); // Blood Shard
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2608693641))); // Blood Shard))); // Champion
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2608693643))); // Blood Shard))); // Conqueror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2608693642))); // Blood Shard))); // Destroyer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2608693644))); // Blood Shard))); // Guardian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2608693640))); // Blood Shard))); // Slayer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587063))); // Bone
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587102))); // Caldeum
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)789315589))); // Call to Adventure
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)789315590))); // Call to Adventure))); // Slayer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)789315591))); // Call to Adventure))); // Champion
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)789315593))); // Call to Adventure))); // Conqueror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)789315592))); // Call to Adventure))); // Destroyer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)789315594))); // Call to Adventure))); // Guardian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3111787208))); // Classic Angel
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3115028404))); // Classic Demon
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1666320302))); // Crusader
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587062))); // Deathshead
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1834718654))); // Demon Hunter
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)949147387))); // Eternal Conflict
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)949147388))); // Eternal Conflict))); // Slayer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)949147389))); // Eternal Conflict))); // Champion
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)949147391))); // Eternal Conflict))); // Conqueror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)949147390))); // Eternal Conflict))); // Destroyer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)949147392))); // Eternal Conflict))); // Guardian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)466824271))); // Eternal Woods
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)466824272))); // Eternal Woods))); // Slayer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)466824273))); // Eternal Woods))); // Champion
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)466824275))); // Eternal Woods))); // Conqueror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)466824274))); // Eternal Woods))); // Destroyer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)466824276))); // Eternal Woods))); // Guardian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2914281823))); // Greyhollow
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2914281824))); // Greyhollow))); // Slayer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2914281825))); // Greyhollow))); // Champion
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2914281827))); // Greyhollow))); // Conqueror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2914281826))); // Greyhollow))); // Destroyer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2914281828))); // Greyhollow))); // Guardian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587099))); // Heart of Darkness
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)409592463))); // Heaven
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)409592464))); // Heaven))); // Slayer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)409592465))); // Heaven))); // Champion
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)409592467))); // Heaven))); // Conqueror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)409592466))); // Heaven))); // Destroyer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)409592468))); // Heaven))); // Guardian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)586020310))); // Hell
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)586020311))); // Hell))); // Slayer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)586020312))); // Hell))); // Champion
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)586020313))); // Hell))); // Destroyer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)586020314))); // Hell))); // Conqueror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)586020315))); // Hell))); // Guardian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2174954790))); // Imperius
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2174954791))); // Imperius))); // Slayer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2174954792))); // Imperius))); // Champion
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2174954794))); // Imperius))); // Conqueror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2174954793))); // Imperius))); // Destroyer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2174954795))); // Imperius))); // Guardian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)4111980878))); // Industrial
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587128))); // Jade Serpent
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587066))); // Molten
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)423580970))); // Monk
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1094085036))); // Necromancer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)604200072))); // Overwatch
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2129557709))); // Pandemonium
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2129557710))); // Pandemonium))); // Slayer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2129557711))); // Pandemonium))); // Champion
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2129557713))); // Pandemonium))); // Conqueror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2129557712))); // Pandemonium))); // Destroyer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2129557714))); // Pandemonium))); // Guardian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2350702306))); // Paragon 1
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)263764818))); // Paragon 10
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)114304450))); // Paragon 100
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)263764851))); // Paragon 20
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)114305539))); // Paragon 200
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)263764884))); // Paragon 30
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)114306628))); // Paragon 300
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)263764917))); // Paragon 40
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)114307717))); // Paragon 400
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)263764950))); // Paragon 50
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)114308806))); // Paragon 500
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)263764983))); // Paragon 60
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)114309895))); // Paragon 600
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)263765016))); // Paragon 70
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)114310984))); // Paragon 700
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)263765049))); // Paragon 80
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)114312073))); // Paragon 800
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)263765082))); // Paragon 90
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1103438361))); // Portrait of Valor
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587097))); // Season 3
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2914245886))); // Season 4
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2914245888))); // Season 4))); // Champion
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2914245890))); // Season 4))); // Conqueror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2914245889))); // Season 4))); // Destroyer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2914245891))); // Season 4))); // Guardian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2914245887))); // Season 4))); // Slayer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587127))); // Sescheron
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)360887075))); // Soulstone
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)360887077))); // Soulstone))); // Champion
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)360887079))); // Soulstone))); // Conqueror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)360887078))); // Soulstone))); // Destroyer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)360887080))); // Soulstone))); // Guardian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)360887076))); // Soulstone))); // Slayer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587068))); // Stained Glass))); // Azure
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587094))); // Stained Glass))); // Orange
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587095))); // Stained Glass))); // Purple
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587069))); // Stained Glass))); // Red
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587067))); // Stained Glass))); // Teal
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587093))); // Stained Glass))); // Yellow
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587098))); // Storm
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1898911879))); // Tal Rasha
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1898911880))); // Tal Rasha))); // Slayer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1898911881))); // Tal Rasha))); // Champion
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1898911883))); // Tal Rasha))); // Conqueror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1898911882))); // Tal Rasha))); // Destroyer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1898911884))); // Tal Rasha))); // Guardian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1079851844))); // Teganze Warrior
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587126))); // The Sign of Rakkis
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587064))); // Thorns
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3661137118))); // Treasure Goblin
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3661137119))); // Treasure Goblin))); // Slayer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3661137120))); // Treasure Goblin))); // Champion
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3661137122))); // Treasure Goblin))); // Conqueror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3661137121))); // Treasure Goblin))); // Destroyer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3661137123))); // Treasure Goblin))); // Guardian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)4140821779))); // Triforce
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2001444264))); // Twitch
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587100))); // Viper's Seal
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587096))); // Whimsyshire Portrait Frame
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2819867039))); // Witch Doctor
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2102838278))); // Wizard
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3526887646))); // Zandalari
                                                                                                                    //Pets
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2923223637))); // Bat
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)567166426))); // Angelic Goblin
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3358761128))); // Az-Lo
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3620943091))); // Belphegor
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)291388178))); // Bile Boy
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2767458423))); // Blaine's Bear
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1336361309))); // Blaze
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)988742338))); // Bones
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)291388177))); // Buddy
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)167459531))); // Butcher
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1497375554))); // Captain Maraca
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1209305989))); // Charlotte
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)937125523))); // Cucco
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1369935753))); // Diablo
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3922872439))); // Dominion's Revenge
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)471039389))); // Dream of Piers
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1404174869))); // Emerald Serpent
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3620943090))); // Friendly Gauntlet
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)291388175))); // Frost Hound
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2327409917))); // Galthrak the Unhinged
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3153829279))); // Garluth, Destroyer of Kneecaps
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)909568905))); // Grunkk
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3258626648))); // Half-formed Golem
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)4084520547))); // Haunting Hannah
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1336361310))); // Humbart Wessel
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1404174868))); // Iron Serpent
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1404174867))); // Jade Serpent
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3620943089))); // Knight Hand
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1508058349))); // Lady Morthanlu
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1967180567))); // Lamb
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3132731524))); // Liv Moore
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2767458419))); // Lord Kek La Mort
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1508058348))); // Lord Nedly
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)347688656))); // Malfeasance
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1514497706))); // Mal'Ganis
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1377380860))); // Minaca the Feared
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2767458418))); // Mr. Bear
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3430307999))); // Ms. Madeleine
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2401672920))); // Murkgoblin
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3870322527))); // Old Growth
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2767458421))); // Overseer Lady Josephine
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)517654224))); // Probe
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3094528011))); // Queen of the Succubi
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1168709051))); // Rocky
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1729774130))); // Royal Calf
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3919201454))); // Shadow Diablo
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)291388174))); // Spike
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2431587065))); // Steadfast
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1920516661))); // Stupendous Contraption
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3536793764))); // Tal'darim Probe
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2647152737))); // Ten Pounder
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2768450096))); // That Which Must Not be Named
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)291388179))); // The Black Dog
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3475456761))); // The Bumble
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)95923716))); // The Stomach
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)291388176))); // Tiger Hound
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)471039390))); // Unihorn
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2767458422))); // Xiansai Bear
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3620943088))); // Zayl's Loss
                                                                                                                    //Wings
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3372877841))); // Knights of Westmarch
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1464422747))); // Blade Wings
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)567892925))); // Anguish's Grasp        
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2222542490))); // Auriel's Favor
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)702834904))); // Blue Horror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)702834901))); // Brimstone Ascendant
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1905890384))); // Cosmic Wings
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)702834903))); // Dark Bat
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1464422100))); // Demon Soaring
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)702834905))); // Dread
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2653514096))); // Echoes of the Mask
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)208405024))); // Eldritch Embrace
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3890992984))); // Embrace of the Pure One
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1464439640))); // Eternal Flame
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1081301157))); // Eternal Light
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1081297790))); // Falcon's Wings
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)550271606))); // Fiacla-Géar
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)262300867))); // Fingers of Flame
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2479192435))); // Fingers of Terror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)546207131))); // Galactic
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)211305012))); // Harbinger of Destruction
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1905890379))); // Lady Gaki's Wings
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3273442495))); // Lilith's Embrace
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1905890378))); // Lord Culsu's Wings
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1335990173))); // Mercy's Gaze
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)702834906))); // Osseous Grasp
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)208454467))); // Pieces of Hatred
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2552747379))); // Prime Evil Wings
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2764892181))); // Skeletal Wings
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2029657407))); // Star Wings
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)702834902))); // Sulfuric Tide
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2840660586))); // The Light of Heaven
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3297867693))); // The Mimic
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2222725144))); // The Pillars of Heaven
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)721772994))); // Trag'Oul Wings
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1905890383))); // Winds of Aldinach
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1905890380))); // Wings of Kokabiel
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1905890381))); // Wings of Lempo
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2036418836))); // Wings of Mastery
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)550271607))); // Wings of Northern Skies
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1905890382))); // Wings of Semyaz
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1185806158))); // Wings of Terror
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2590476058))); // Wings of the Betrayer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)726768249))); // Wings of the Crypt Guardian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)671128753))); // Wings of the Dedicated
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)4188235898))); // Wings of the Ghost Queen
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1030142027))); // Wings of Valor
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2015178386))); // Heaven's Might

            //1514497706

            //Flags
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1428615110))); // Barbarian
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)895083523))); // Barbarian Ascendant
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3194995136))); // BlizzCon 2015
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3484863475))); // Blood Master
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)522011784))); // Crusader
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)938178597))); // Crusader Ascendant
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)566077307))); // Demon Hunter
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)28429688))); // Demon Hunter Ascendant
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)177825848))); // Diablo
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)177825849))); // Diablo II
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)177825850))); // Diablo III
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2113242414))); // Dog
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)218434287))); // Dragon
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)590649887))); // Goat
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)4211285172))); // Harvest
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)127020830))); // Heroes of the Storm
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2644729685))); // Horse
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)177825852))); // Lord of Destruction
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1156357301))); // Loremaster
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3269693700))); // Monk
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)895493953))); // Monk Ascendant
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1375359175))); // Monkey
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3829241798))); // Necromancer
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)671111717))); // Necromancer Ascendant
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1769352763))); // Ox
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2575751604))); // Pig
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)4233838312))); // Rabbit
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2645002203))); // Rat
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)177825851))); // Reaper of Souls
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)455845058))); // Rooster
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3124785508))); // Samhain
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2732665546))); // Season 3
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1329910150))); // Snake
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3359438927))); // Tiger
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2885511819))); // Tyrael's Justice
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)3532272868))); // Warsong Pennant
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)2626988266))); // Witch Doctor
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)28430311))); // Witch Doctor Ascendant
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)1114884937))); // Wizard
            C.AddCosmeticItems_(D3.CosmeticItems.CosmeticItem.CreateBuilder().SetGbid(unchecked((int)938200550))); // Wizard Ascendant
            //*/
            #endregion 
            return C.Build().ToByteString();
        }

        private ByteString CurrentPrimaryWallet(BattleClient client, ByteString data)
        {
            var test = D3.Store.GetPrimaryCurrency.ParseFrom(data);
            var testresp = D3.Store.GetPrimaryCurrencyResponse.CreateBuilder().SetCurrency(3600);
            var test1 = D3.Store.GetPaymentMethods.ParseFrom(data);

            var testresp2 = D3.Store.GetPaymentMethodsResponse.CreateBuilder()
                .AddWallets(D3.Store.Wallet.CreateBuilder().SetPrimary(true).SetWalletId(840).SetWalletName("US Dollar").SetFixedPointBalance(50))
                //.AddWallets(D3.Store.Wallet.CreateBuilder().SetPrimary(true).SetWalletId(826).SetWalletName("Pound Streling").SetFixedPointBalance(50))
                ;
                /*
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("GBP").SetId(826).SetSymbol("GBP").SetName("Pound Sterling"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("NZD").SetId(554).SetSymbol("NZD").SetName("New Zealand Dollar"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("CLP").SetId(152).SetSymbol("CLP").SetName("Chilean Peso"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("TPT").SetId(16).SetSymbol("NT$").SetName("TPT"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("CAD").SetId(124).SetSymbol("CAD").SetName("Canadian Dollar"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("KRW").SetId(410).SetSymbol("KRW").SetName("Won"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("USD").SetId(840).SetSymbol("USD").SetName("US Dollar"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("JPY").SetId(392).SetSymbol("JPY").SetName("Yen"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("MXN").SetId(484).SetSymbol("MXN").SetName("Mexican Peso"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("BRL").SetId(986).SetSymbol("BRL").SetName("Brazilian Real"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("AUD").SetId(36).SetSymbol("AUD").SetName("Australian Dollar"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("CAD").SetId(124).SetSymbol("CAD").SetName("Canadian Dollar"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("EUR").SetId(978).SetSymbol("EUR").SetName("Euro"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("ARS").SetId(32).SetSymbol("ARS").SetName("Argentine Peso"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("RUB").SetId(643).SetSymbol("RUB").SetName("Russian Ruble"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("CPT").SetId(15).SetSymbol("¥").SetName("CPT"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("XDC").SetId(26).SetSymbol("$").SetName("D3 Platinum"))
                //*/
            return testresp2.Build().ToByteString();
        }

        private ByteString CurrentWallet(BattleClient client, ByteString data)
        {
            var test = D3.Store.GetPaymentMethods.ParseFrom(data);
            var request = D3.Store.GetCurrency.ParseFrom(data);

            var testresp = D3.Store.GetPrimaryCurrencyResponse.CreateBuilder().SetCurrency(3600);

            return testresp.Build().ToByteString();
        }
        private ByteString CurrentStore(BattleClient client, ByteString data)
        {

            var test = D3.Store.GetProductCategories.ParseFrom(data);
            var responseCategoriese = D3.Store.GetProductCategoriesResponse.CreateBuilder();
            responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(0).SetNewestProductTime(0x6E1199DE92C0000)); //Popular
            //responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(1).SetNewestProductTime(0)); //Platinum
            //responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(2).SetNewestProductTime(0)); //?
            //responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(3).SetNewestProductTime(0)); //Addition
            //responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(4).SetNewestProductTime(0x72A0D8B7AD90000)); //Necromancer
            //responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(5).SetNewestProductTime(0)); //?
            //responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(6).SetNewestProductTime(0)); //?
            //responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(7).SetNewestProductTime(0)); //Elite kits
            //responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(8).SetNewestProductTime(0)); //Crusader
            //responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(9).SetNewestProductTime(0)); //Adventurer's Pass
            responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(10).SetNewestProductTime(0x72A0D8B7AD90000));//Wings
            responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(11).SetNewestProductTime(0x725C16B88870000));//Pets
            responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(12).SetNewestProductTime(0));//Framework
            responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(13).SetNewestProductTime(0));//Flags
            responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(14).SetNewestProductTime(0));//Strengthening the Hero
            responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(15).SetNewestProductTime(0));//Hero Cells
            responseCategoriese.AddCategories(D3.Store.ProductCategory.CreateBuilder().SetCategory(16).SetNewestProductTime(0));//Stash Tabs

            return responseCategoriese.Build().ToByteString();
        }
        private ByteString CurrentStore1(BattleClient client, ByteString data)
        {
            var request = D3.Store.GetProductList.ParseFrom(data);
            var store = D3.Store.GetProductListResponse.CreateBuilder();
            if (request.Category == 0)
            {
                store
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)2036418836))) // Wings of Mastery
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(50000000).SetFixedPointRetailPrice(50000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(4))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)3194995136))) // Blizzcon 2015
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(1).SetBundleLabel("")
                            .SetProductId(130002))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)1514497706))) // MalGanis
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(70000000).SetFixedPointRetailPrice(80000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(11).SetFeatured(2).SetBundleLabel("")
                                .SetProductId(11000))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)2431587061))) // Angelic
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(2000000).SetFixedPointRetailPrice(5000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(12).SetFeatured(3).SetBundleLabel("")
                                .SetProductId(12000))
                ;
            }
            #region Platinum
            if (request.Category == 1)
            {
                store
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetPlatinumAmount(500))
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(15).SetFixedPointCost(50000).SetFixedPointRetailPrice(50000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(1).SetFeatured(0).SetBundleLabel("")
                                .SetProductId(11383))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetPlatinumAmount(1000))
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(15).SetFixedPointCost(80000).SetFixedPointRetailPrice(80000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(1).SetFeatured(1).SetBundleLabel("")
                                .SetProductId(11384))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetPlatinumAmount(5000))
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(15).SetFixedPointCost(299900).SetFixedPointRetailPrice(350000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(1).SetFeatured(1).SetBundleLabel("")
                                .SetProductId(11385))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetPlatinumAmount(10000))
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(15).SetFixedPointCost(499900).SetFixedPointRetailPrice(499900)) //Value Type, Discount Price, Regular Price
                                .SetCategory(1).SetFeatured(1).SetBundleLabel("")
                                .SetProductId(11386))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetPlatinumAmount(50000))
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(15).SetFixedPointCost(0).SetFixedPointRetailPrice(990000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(1).SetFeatured(1).SetBundleLabel("")
                                .SetProductId(11387))
                                ;
            }
            #endregion
            #region Wings
            if (request.Category == 10)
            {
                store
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(1905890383))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(1))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(1905890380))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(20000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(2))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(1905890381))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(20000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(3))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(2036418836)) // Wings of Mastery
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(50000000).SetFixedPointRetailPrice(50000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(4))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(550271607))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(20000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(5))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(1185806158))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(20000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(1).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(6))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)2590476058)))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(7))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(671128753))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(8))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)4188235898)))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(9))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)1030142027)))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(10))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(721772994))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(11))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(567892925))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(12))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(1464422747))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(13))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(1905890384))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(14))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(1081297790))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(15))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(1905890379))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(16))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(1905890378))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(17))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)2764892181)))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(18))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(2029657407))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(19))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(721772994))
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(10).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(20));
            }

            #endregion
            #region Pets
            if (request.Category == 11)
            {
                store
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)1514497706))) // MalGanis
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(70000000).SetFixedPointRetailPrice(80000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(11).SetBundleLabel("")
                                .SetProductId(11000));
            }
            #endregion
            #region Framework
            if (request.Category == 12)
            {
                store
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)2431587061))) // Angelic
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(2000000).SetFixedPointRetailPrice(5000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(12).SetBundleLabel("")
                                .SetProductId(12000))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)604200072))) // OverWatch
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(3000000).SetFixedPointRetailPrice(5000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(12).SetBundleLabel("")
                                .SetProductId(12001))
                                ;

            }
            #endregion
            #region Flags
            if (request.Category == 13)
            {
                store
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(1428615110)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130000))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(895083523)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130001))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)3194995136))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130002))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)3484863475))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130003))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(522011784)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130004))
                         .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(938178597)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130005))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(566077307)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130006))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(28429688)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130007))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(177825848)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130008))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(177825849)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130009))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(177825850)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130010))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(2113242414)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130011))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(218434287)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130012))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(590649887)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130013))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(895083523)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130014))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)4211285172))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130015))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(127020830)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130016))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)2644729685))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130017))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(177825852)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130018))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(1156357301)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130019))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)3269693700))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130020))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(895493953)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130021))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(1375359175)) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130022))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)3269693700))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130023))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)3829241798))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130024))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)671111717))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130025))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)1769352763))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130026))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)2575751604))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130027))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)4233838312))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130028))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)2645002203))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130029))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)177825851))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130030))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)455845058))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130031))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)3124785508))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130032))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)2732665546))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130033))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)1329910150))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130034))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)3359438927))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130035))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)2885511819))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130036))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)3532272868))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130037))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)2626988266))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130038))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)28430311))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130039))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)1114884937))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130040))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetGbid(unchecked((int)938200550))) // 
                            .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(15000000).SetFixedPointRetailPrice(15000000)) //Value Type, Discount Price, Regular Price
                            .SetCategory(13).SetFeatured(0).SetBundleLabel("")
                            .SetProductId(130041))

                            ;
            }
            #endregion
            #region Boosts
            if (request.Category == 14)
            {
                store
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetContentLicenseId(16)) // 
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(60000000).SetFixedPointRetailPrice(60000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(14).SetDurationSecs(2592000)
                                .SetProductId(12101))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetContentLicenseId(16)) // 
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(0).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(14).SetDurationSecs(604800)
                                .SetProductId(12102))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetContentLicenseId(17)) // 
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(60000000).SetFixedPointRetailPrice(60000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(14).SetDurationSecs(2592000)
                                .SetProductId(12103))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetContentLicenseId(17)) // 
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(20000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(14).SetDurationSecs(604800)
                                .SetProductId(12104))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetContentLicenseId(18)) // 
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(60000000).SetFixedPointRetailPrice(60000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(14).SetDurationSecs(2592000)
                                .SetProductId(12105))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetContentLicenseId(18)) // 
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(20000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(14).SetDurationSecs(604800)
                                .SetProductId(12106))
                        ;
            }
            #endregion
            #region Hero Cells
            if (request.Category == 15)
            {
                store
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetContentLicenseId(11).SetMaxStackCount(10))
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(100000000).SetFixedPointRetailPrice(100000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(15)
                                .SetProductId(15000))
                                ;
            }
            #endregion
            #region Stash Tabs
            if (request.Category == 16)
            {
                store
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetContentLicenseId(14).SetMaxStackCount(5)).AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetPlatinumAmount(0))
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(20000000).SetFixedPointRetailPrice(20000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(16)
                                .AddMaxContentlicensesAllowed(D3.Store.ContentLicenseRestriction.CreateBuilder().SetContentLicenseId(14).SetCount(0))
                                .SetProductId(160))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetContentLicenseId(14).SetMaxStackCount(5))
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(40000000).SetFixedPointRetailPrice(40000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(16)
                                .AddMaxContentlicensesAllowed(D3.Store.ContentLicenseRestriction.CreateBuilder().SetContentLicenseId(14).SetCount(1))
                                .AddMinContentlicensesRequired(D3.Store.ContentLicenseRestriction.CreateBuilder().SetContentLicenseId(14).SetCount(1))
                                .SetProductId(161))
                        .AddProducts(D3.Store.Product.CreateBuilder().AddEntitlements(D3.Store.ProductEntitlement.CreateBuilder().SetContentLicenseId(14).SetMaxStackCount(5))
                                .AddPrices(D3.Store.ProductPrice.CreateBuilder().SetCurrency(26).SetFixedPointCost(80000000).SetFixedPointRetailPrice(80000000)) //Value Type, Discount Price, Regular Price
                                .SetCategory(16)
                                .AddMinContentlicensesRequired(D3.Store.ContentLicenseRestriction.CreateBuilder().SetContentLicenseId(14).SetCount(2))
                                .SetProductId(162))
                                
                                ;
            }
            #endregion

            //Currencies
            store
                
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("GBP").SetId(826).SetSymbol("GBP").SetName("Pound Sterling"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("NZD").SetId(554).SetSymbol("NZD").SetName("New Zealand Dollar"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("CLP").SetId(152).SetSymbol("CLP").SetName("Chilean Peso"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("TPT").SetId(16).SetSymbol("NT$").SetName("TPT"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("CAD").SetId(124).SetSymbol("CAD").SetName("Canadian Dollar"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("KRW").SetId(410).SetSymbol("KRW").SetName("Won"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("USD").SetId(840).SetSymbol("USD").SetName("US Dollar"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("JPY").SetId(392).SetSymbol("JPY").SetName("Yen"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("MXN").SetId(484).SetSymbol("MXN").SetName("Mexican Peso"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("BRL").SetId(986).SetSymbol("BRL").SetName("Brazilian Real"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("AUD").SetId(36).SetSymbol("AUD").SetName("Australian Dollar"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("CAD").SetId(124).SetSymbol("CAD").SetName("Canadian Dollar"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("EUR").SetId(978).SetSymbol("EUR").SetName("Euro"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("ARS").SetId(32).SetSymbol("ARS").SetName("Argentine Peso"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("RUB").SetId(643).SetSymbol("RUB").SetName("Russian Ruble"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("CPT").SetId(15).SetSymbol("¥").SetName("CPT"))
                .AddCurrencies(D3.Store.Currency.CreateBuilder().SetCode("XDC").SetId(26).SetSymbol("$").SetName("D3 Platinum"))
                ;
            return store.Build().ToByteString();
        }
        private ByteString SetCollection(BattleClient client, ByteString data)
        {
            var request = D3.GameMessage.EquipCosmeticItem.ParseFrom(data);
            
            
            switch (request.CosmeticItemType)
            {
                case 1:
                    client.Account.GameAccount.CurrentToon.Cosmetic1 = request.Gbid;
                    break;
                case 2:
                    client.Account.GameAccount.CurrentToon.Cosmetic2 = request.Gbid;
                    break;
                case 3:
                    client.Account.GameAccount.CurrentToon.Cosmetic3 = request.Gbid;
                    break;
                case 4:
                    client.Account.GameAccount.CurrentToon.Cosmetic4 = request.Gbid;
                    break;
            }

            var RangeCosmetic = new[]
            {
                D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(client.Account.GameAccount.CurrentToon.Cosmetic1).Build(), // Wings
                D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(client.Account.GameAccount.CurrentToon.Cosmetic2).Build(), // Flag
                D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(client.Account.GameAccount.CurrentToon.Cosmetic3).Build(), // Pet
                D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(client.Account.GameAccount.CurrentToon.Cosmetic4).Build(), // Frame
            };

            client.Account.GameAccount.CurrentToon.StateChanged();

            var NewVisual = D3.Hero.VisualEquipment.CreateBuilder()
                .AddRangeVisualItem(client.Account.GameAccount.CurrentToon.HeroVisualEquipmentField.Value.VisualItemList).AddRangeCosmeticItem(RangeCosmetic).Build();

            client.Account.GameAccount.CurrentToon.HeroVisualEquipmentField.Value = NewVisual;
            client.Account.GameAccount.ChangedFields.SetPresenceFieldValue(client.Account.GameAccount.CurrentToon.HeroVisualEquipmentField);
            client.Account.GameAccount.NotifyUpdate();

            return NewVisual.ToByteString();
        }
        private ByteString StoreRequest(BattleClient client, ByteString data)
        {
            var req = GetHeroIds.ParseFrom(data);

            var Status = CheatModifyStoreState.CreateBuilder().SetEnable(false);
            return Status.Build().ToByteString();
        }
        private ByteString TestRequest(BattleClient client, ByteString data)
        {
            var req = CSPullSnapshot.ParseFrom(data);

            var HeroDigestList = D3.Hero.DigestList.CreateBuilder();
            foreach (Toon t in client.Account.GameAccount.Toons) HeroDigestList.AddDigests(t.Digest);
            var Snap = D3.CS.HeroesList.CreateBuilder().SetCurrentSeasonNum(1).SetCurrentSeasonState(1).SetDigests(HeroDigestList);
            return Snap.Build().ToByteString();
        }
        #endregion
        #region Changing session parameters
        private ByteString SwitchParameters(BattleClient client, ByteString data)
        {

            var request = D3.GameMessage.MatchmakingGetStats.ParseFrom(data);

            var back = GamesSystem.GameFactoryManager.GetStatsBucketWithFilter(request);

            var response = MatchmakingGetStatsResponse.CreateBuilder().AddStatsBucket(back);
            return response.Build().ToByteString();
        }
        #endregion

        private ByteString OnGetToonSettings(BattleClient client, ByteString data)
        {
            var request = GetToonSettings.ParseFrom(data);
            var response = D3.Client.ToonSettings.CreateBuilder();
            return response.Build().ToByteString();
        }
        private ByteString CurrentToon(BattleClient client, ByteString data)
        {
            SelectHero req = SelectHero.ParseFrom(data);
            D3.Client.ToonSettings.Builder res = D3.Client.ToonSettings.CreateBuilder();
            return req.ToByteString();
        }
        private ByteString GetHeroProfs(BattleClient client, ByteString data)
        {
            var testRequest = GetHeroProfiles.ParseFrom(data);

            var profileList = D3.Profile.HeroProfileList.CreateBuilder();
            if (testRequest.HeroIdsCount > 0)
            {
                foreach (var hero in testRequest.HeroIdsList)
                {
                    var toon = ToonManager.GetToonByLowID(hero);
                    if (toon.Dead == false)
                        profileList.AddHeros(toon.Profile);
                }
            }
            else
            {
                var heroList = GameAccountManager.GetAccountByPersistentID(testRequest.AccountId.Id).Toons;
                foreach (var hero in heroList)
                {
                    if (hero.Dead == false)
                        profileList.AddHeros(hero.Profile);
                }
            }

            return profileList.Build().ToByteString();
        }
        private ByteString GetDeadedHeroDigests(BattleClient client, ByteString data)
        {
            var HeroDigestList = D3.Hero.DigestList.CreateBuilder();
            foreach (Toon t in client.Account.GameAccount.Toons)
            {
                if (t.IsHardcore)
                    if (t.Dead)
                        HeroDigestList.AddDigests(t.Digest);
            }
            return HeroDigestList.Build().ToByteString();
        }
        private ByteString GetDeadedHeroProfs(BattleClient client, ByteString data)
        {
            var testRequest = GetFallenHeros.ParseFrom(data);

            var test = D3.CS.FallenHero.CreateBuilder();
            var t1 = ArchiveHardcoreResponse.CreateBuilder();

            var profileList = HeroProfileList.CreateBuilder();

            /*if (testRequest.HeroIdsCount > 0)
            {
                foreach (var hero in testRequest.HeroIdsList)
                {
                    var toon = ToonManager.GetToonByLowID(hero);
                    if (toon.Dead 
            === true)
                        profileList.AddHeros(toon.Profile);
                }
            }
            else*/
            {
                var heroList = GameAccountManager.GetAccountByPersistentID(testRequest.AccountId.Id).Toons;
                foreach (var hero in heroList)
                {
                    //if (hero.Hardcore == true)
                        // if (hero.Dead == true)
                        profileList.AddHeros(hero.Profile);
                }
            }

            return profileList.Build().ToByteString();
        }
        private ByteString SelectToon(BattleClient Client, ByteString data)
        {
            var request = D3.GameMessage.HeroDigestListRequest.ParseFrom(data);
            var builder = HeroDigestListResponse.CreateBuilder();
            foreach (var toon in request.ToonIdList)
                builder.AddDigestList(ToonManager.GetToonByLowID(toon).Digest);

            return builder.Build().ToByteString();
        }

        private ByteString SendWarden3Custom(BattleClient client, ByteString data)
        {
            byte[] response270 = { 8, 137, 249, 159, 185, 12, 16, 136, 14 };
            return ByteString.CopyFrom(response270);
        }
        private ByteString GetAchievements(BattleClient client, ByteString data)
        {
            var HeroDigestList = D3.Hero.DigestList.CreateBuilder();
            foreach (Toon t in client.Account.GameAccount.Toons) HeroDigestList.AddDigests(t.Digest);
            var Snap = D3.CS.HeroesList.CreateBuilder().SetCurrentSeasonNum(1).SetCurrentSeasonState(1).SetDigests(HeroDigestList);

            return Snap.Build().ToByteString();
        }
        private ByteString RebirthMethod(BattleClient client, ByteString data)
        {
            RebirthHeroRequest Request = RebirthHeroRequest.ParseFrom(data);
            var Response = RebirthHeroResponse.CreateBuilder();
            foreach (Toon t in client.Account.GameAccount.Toons)
            {
                if (t.D3EntityID.IdLow == Request.HeroId)
                {
                    //t.SetSeason(1);

                    Response.SetHeroDigest(t.Digest);
                }
            }

            return Response.Build().ToByteString();
        }
        private ByteString GetChallengeRift(BattleClient client, ByteString data)
        {
            var request = ChallengeRiftFetchHeroData.ParseFrom(data);

            var response = ChallengeRiftFetchHeroDataResponse.CreateBuilder();

            response.SetAltLevel(1)
                .SetBattleTag("TestForChallenge#0000")
                .SetHeroDigest(D3.Hero.Digest.CreateBuilder()
                    .SetVersion(905)
                    .SetHeroId(D3.OnlineService.EntityId.CreateBuilder().SetIdHigh(0).SetIdLow(68056897))
                    .SetHeroName("TestForChallenge")
                    .SetGbidClass(54772266)
                    .SetLevel(70)
                    .SetPlayerFlags(8388624)
                    .SetVisualEquipment(D3.Hero.VisualEquipment.CreateBuilder()
                        .AddVisualItem(D3.Hero.VisualItem.CreateBuilder().SetGbid(1104405863))
                        .AddVisualItem(D3.Hero.VisualItem.CreateBuilder().SetGbid(868571512))
                        .AddVisualItem(D3.Hero.VisualItem.CreateBuilder().SetGbid(1493121096))
                        .AddVisualItem(D3.Hero.VisualItem.CreateBuilder().SetGbid(-1385479119))
                        .AddVisualItem(D3.Hero.VisualItem.CreateBuilder().SetGbid(-638474348))
                        .AddVisualItem(D3.Hero.VisualItem.CreateBuilder().SetGbid(1003477242))
                        .AddVisualItem(D3.Hero.VisualItem.CreateBuilder().SetGbid(-1499119737))
                        .AddVisualItem(D3.Hero.VisualItem.CreateBuilder().SetGbid(-1221469977))
                        .AddCosmeticItem(D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(567892925))
                        .AddCosmeticItem(D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(-83682124))
                        .AddCosmeticItem(D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(-1200439285))
                        .AddCosmeticItem(D3.Hero.VisualCosmeticItem.CreateBuilder().SetGbid(949147389))
                        )
                    .SetLastPlayedAct(0)
                    .SetHighestUnlockedAct(3000)
                    .SetLastPlayedQuest(87700)
                    .SetLastPlayedQuestStep(-1)
                    .SetTimePlayed(119301)
                    .SetPvpRank(1)
                    .SetSeasonCreated(0)
                    .SetHighestSoloRiftCompleted(57)
                )
                .SetRewardGbid(456652071)
                .SetTargetMillisecond(318566)
                .SetChallengeEndTimeUnixSeconds(1586832804)
                ;

            response.SetHeroProfile(HeroProfile.CreateBuilder()
                .SetEquipment(D3.Items.ItemList.CreateBuilder()
                    .AddItems(D3.Items.SavedItem.CreateBuilder()
                        .SetId(ItemId.CreateBuilder().SetIdHigh(0).SetIdLow(54043195984020046))
                        .SetOwnerEntityId(D3.OnlineService.EntityId.CreateBuilder().SetIdHigh(0).SetIdLow(130817604))
                        .SetHirelingClass(0)
                        .SetItemSlot(288)
                        .SetSquareIndex(0)
                        .SetUsedSocketCount(1)
                        .SetGenerator(D3.Items.Generator.CreateBuilder()
                            .SetSeed(3437070505)
                            .SetGbHandle(D3.GameBalance.Handle.CreateBuilder().SetGameBalanceType(2).SetGbid(1104405863))
                            .AddBaseAffixes(-1392021261)
                            .AddBaseAffixes(1342579558)
                            .AddBaseAffixes(-889190757)
                            .AddBaseAffixes(1151246290)
                            .AddBaseAffixes(1274656075)
                            .AddBaseAffixes(-260600975)
                            .SetFlags(436491)
                            .SetDurability(257)
                            .SetStackSize(0)
                            .SetItemQualityLevel(9)
                            .SetItemBindingLevel(2)
                            .SetMaxDurability(257)
                            .AddContents(D3.Items.EmbeddedGenerator.CreateBuilder()
                                .SetId(ItemId.CreateBuilder().SetIdHigh(0).SetIdLow(4323455643408519424))
                                .SetGenerator(D3.Items.Generator.CreateBuilder()
                                    .SetSeed(2696508178)
                                    .SetGbHandle(D3.GameBalance.Handle.CreateBuilder().SetGameBalanceType(2).SetGbid(-1038303580))
                                    .SetFlags(436491)
                                    .SetDurability(0)
                                    .SetStackSize(1)
                                    .SetItemBindingLevel(2)
                                    .SetSeasonCreated(20)
                                    )
                                )
                            .SetLegendaryItemLevel(70)
                            .SetSeasonCreated(20)
                            .SetLegendaryBaseItemGbid(620036249)
                            )
                    )

                )
                .SetSnoActiveSkills(SkillsWithRunes.CreateBuilder()
                    .AddRunes(SkillWithRune.CreateBuilder().SetSkill(108506).SetRuneType(4))
                    .AddRunes(SkillWithRune.CreateBuilder().SetSkill(69867).SetRuneType(1))
                    .AddRunes(SkillWithRune.CreateBuilder().SetSkill(67668).SetRuneType(4))
                    .AddRunes(SkillWithRune.CreateBuilder().SetSkill(106237).SetRuneType(0))
                    .AddRunes(SkillWithRune.CreateBuilder().SetSkill(117402).SetRuneType(4))
                    .AddRunes(SkillWithRune.CreateBuilder().SetSkill(67616).SetRuneType(1))
                    )
                .SetSnoTraits(PassiveSkills.CreateBuilder()
                    .AddSnoTraits(208628)
                    .AddSnoTraits(208639)
                    .AddSnoTraits(208594)
                    .AddSnoTraits(218191)
                    )
                .SetLegendaryPowers(LegendaryPowers.CreateBuilder()
                    .AddGbidLegendaryPowers(1889450717)
                    .AddGbidLegendaryPowers(-2073208480)
                    .AddGbidLegendaryPowers(1160204002)
                    )
                );

            #region  Etalon
            /*
             alt_level: 446
hero_digest {
  version: 905
  hero_id {
    id_high: 0
    id_low: 68056897
  }
  hero_name: "\350\275\273\346\205\242\350\200\214\350\265\267\350\210\236\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000\000"
  gbid_class: -1104684007
  level: 70
  player_flags: 1585446930
  visual_equipment {
    visual_item {
      gbid: -1992164625
    }
    visual_item {
      gbid: 259933632
    }
    visual_item {
      gbid: -800755056
    }
    visual_item {
      gbid: 2059399737
    }
    visual_item {
      gbid: 237815937
    }
    visual_item {
      gbid: 841509030
    }
    visual_item {
      gbid: -1980761457
    }
    visual_item {
      gbid: -774345512
    }
    cosmetic_item {
      gbid: 1030142027
    }
    cosmetic_item {
    }
    cosmetic_item {
    }
    cosmetic_item {
      gbid: 114305539
    }
  }
  last_played_act: 400
  highest_unlocked_act: 3000
  last_played_quest: -1
  last_played_quest_step: -1
  time_played: 527881
  pvp_rank: 1
  season_created: 0
  highest_solo_rift_completed: 54
}
hero_profile {
  equipment {
    items {
      id {
        id_high: 0
        id_low: 2395915002591459005
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 288
      square_index: 0
      used_socket_count: 1
      generator {
        seed: 860017891
        gb_handle {
          game_balance_type: 2
          gbid: -1993314873
        }
        base_affixes: -260600975
        base_affixes: 2066794450
        base_affixes: -1900569383
        base_affixes: -889190756
        base_affixes: 1342579558
        base_affixes: -993623678
        flags: 3339
        durability: 347
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 347
        contents {
          id {
            id_high: 0
            id_low: 2395915002591459006
          }
          generator {
            seed: 3676334038
            gb_handle {
              game_balance_type: 2
              gbid: -848028873
            }
            flags: 2319
            durability: 0
            stack_size: 1
            item_binding_level: 2
            season_created: 0
          }
        }
        legendary_item_level: 70
        transmog_gbid: -1992164625
        season_created: 0
        enchanted_affix_old: -1900569383
        enchanted_affix_new: 432187391
        legendary_base_item_gbid: 1565456767
        enchanted_affix_seed: 3177548108
        enchanted_affix_count: 22
      }
    }
    items {
      id {
        id_high: 0
        id_low: 3152519740255919235
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 304
      square_index: 0
      used_socket_count: 3
      generator {
        seed: 813905168
        gb_handle {
          game_balance_type: 2
          gbid: 258783384
        }
        base_affixes: -997350627
        base_affixes: -522962331
        base_affixes: 1444204450
        base_affixes: 82091320
        base_affixes: 1342579571
        base_affixes: -1170857815
        flags: 2315
        durability: 332
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 332
        contents {
          id {
            id_high: 0
            id_low: 2089670228029338625
          }
          generator {
            seed: 3610959166
            gb_handle {
              game_balance_type: 2
              gbid: 1019190666
            }
            flags: 43275
            durability: 0
            stack_size: 1
            item_binding_level: 2
            season_created: 0
          }
        }
        contents {
          id {
            id_high: 0
            id_low: 2089670228029338632
          }
          generator {
            seed: 3588885479
            gb_handle {
              game_balance_type: 2
              gbid: 1019190666
            }
            flags: 43275
            durability: 0
            stack_size: 1
            item_binding_level: 2
            season_created: 0
          }
        }
        contents {
          id {
            id_high: 0
            id_low: 2089670228029338633
          }
          generator {
            seed: 3132810486
            gb_handle {
              game_balance_type: 2
              gbid: 1019190666
            }
            flags: 43275
            durability: 0
            stack_size: 1
            item_binding_level: 2
            season_created: 0
          }
        }
        legendary_item_level: 70
        transmog_gbid: 259933632
        season_created: 0
        enchanted_affix_old: 1444204450
        enchanted_affix_new: -454279577
        legendary_base_item_gbid: 1612259889
        enchanted_affix_seed: 2592320242
        enchanted_affix_count: 82
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458988
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 320
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 2119670951
        gb_handle {
          game_balance_type: 2
          gbid: 841509030
        }
        base_affixes: -708791752
        base_affixes: 82091320
        base_affixes: 1444204450
        base_affixes: -454279577
        base_affixes: -993623679
        flags: 2315
        durability: 263
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 263
        legendary_item_level: 70
        season_created: 0
        enchanted_affix_old: 1444204450
        enchanted_affix_new: 783505022
        legendary_base_item_gbid: 1815809043
        enchanted_affix_seed: 2984547919
        enchanted_affix_count: 37
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2089670228029332721
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 336
      square_index: 0
      used_socket_count: 1
      generator {
        seed: 1999325384
        gb_handle {
          game_balance_type: 2
          gbid: 237815937
        }
        base_affixes: 955554965
        base_affixes: -620982138
        base_affixes: -415821016
        base_affixes: 1646833580
        base_affixes: -303435822
        flags: 43275
        durability: 300
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 300
        contents {
          id {
            id_high: 0
            id_low: 2395915002591459002
          }
          generator {
            seed: 2512707236
            gb_handle {
              game_balance_type: 2
              gbid: -1456001726
            }
            flags: 2315
            durability: 0
            stack_size: 1
            item_binding_level: 2
            season_created: 0
          }
        }
        legendary_item_level: 70
        season_created: 0
        enchanted_affix_old: -415821016
        enchanted_affix_new: 1342579558
        legendary_base_item_gbid: -873319305
        enchanted_affix_seed: 1612936527
        enchanted_affix_count: 1
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591459007
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 352
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 2545353357
        gb_handle {
          game_balance_type: 2
          gbid: 2058249489
        }
        base_affixes: -993623679
        base_affixes: 783505022
        base_affixes: -889190757
        base_affixes: 611264315
        base_affixes: 1406385324
        base_affixes: 555585138
        flags: 2315
        durability: 309
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 309
        legendary_item_level: 70
        transmog_gbid: 2059399737
        season_created: 0
        enchanted_affix_old: 611264315
        enchanted_affix_new: -1064392808
        legendary_base_item_gbid: -1533912119
        enchanted_affix_seed: 2967401546
        enchanted_affix_count: 37
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2485986995225715174
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 368
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 2285458808
        gb_handle {
          game_balance_type: 2
          gbid: 924103246
        }
        base_affixes: -1170857815
        base_affixes: 1363430979
        base_affixes: -1650911650
        flags: 43275
        durability: 308
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 308
        legendary_item_level: 70
        season_created: 0
        legendary_base_item_gbid: 2112157589
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458975
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 384
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 2064410481
        gb_handle {
          game_balance_type: 2
          gbid: -801905304
        }
        base_affixes: -1170857815
        base_affixes: 1363430979
        base_affixes: 1936411023
        base_affixes: 98886379
        base_affixes: 966382508
        base_affixes: -708791752
        flags: 2315
        durability: 433
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 433
        legendary_item_level: 70
        transmog_gbid: -800755056
        season_created: 0
        enchanted_affix_old: 1936411023
        enchanted_affix_new: 432187391
        legendary_base_item_gbid: 2140882336
        enchanted_affix_seed: 543031971
        enchanted_affix_count: 39
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458973
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 400
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 3768965997
        gb_handle {
          game_balance_type: 2
          gbid: -1981911705
        }
        base_affixes: -1170857815
        base_affixes: 611264315
        base_affixes: 1363430979
        base_affixes: 1889120614
        base_affixes: -670436801
        base_affixes: 555585138
        flags: 2315
        durability: 389
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 389
        legendary_item_level: 70
        transmog_gbid: -1980761457
        season_created: 0
        enchanted_affix_old: 1889120614
        enchanted_affix_new: -454279577
        legendary_base_item_gbid: 365492434
        enchanted_affix_seed: 3942732396
        enchanted_affix_count: 14
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458984
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 416
      square_index: 0
      used_socket_count: 2
      generator {
        seed: 2359142568
        gb_handle {
          game_balance_type: 2
          gbid: 825865639
        }
        base_affixes: -1170857815
        base_affixes: 1355452559
        base_affixes: 611264315
        base_affixes: 1444204450
        base_affixes: 555585138
        base_affixes: -1613479975
        flags: 2315
        durability: 444
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 444
        contents {
          id {
            id_high: 0
            id_low: 2089670228029338466
          }
          generator {
            seed: 1542136307
            gb_handle {
              game_balance_type: 2
              gbid: 1019190666
            }
            flags: 43275
            durability: 0
            stack_size: 1
            item_binding_level: 2
            season_created: 0
          }
        }
        contents {
          id {
            id_high: 0
            id_low: 2089670228029338437
          }
          generator {
            seed: 3259546689
            gb_handle {
              game_balance_type: 2
              gbid: 1019190666
            }
            flags: 43279
            durability: 0
            stack_size: 1
            item_binding_level: 2
            season_created: 0
          }
        }
        legendary_item_level: 70
        transmog_gbid: -774345512
        season_created: 0
        enchanted_affix_old: 1444204450
        enchanted_affix_new: 1363430979
        legendary_base_item_gbid: -1512729953
        enchanted_affix_seed: 2578006689
        enchanted_affix_count: 9
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591459004
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 432
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 2557074620
        gb_handle {
          game_balance_type: 2
          gbid: 962282719
        }
        base_affixes: -574194875
        base_affixes: 1363430979
        base_affixes: 1412926752
        base_affixes: -1428872617
        base_affixes: -1170857815
        flags: 2315
        durability: 360
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 360
        legendary_item_level: 70
        season_created: 0
        enchanted_affix_old: -1428872617
        enchanted_affix_new: 91845950
        legendary_base_item_gbid: -875942693
        enchanted_affix_seed: 1610014882
        enchanted_affix_count: 3
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458960
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 448
      square_index: 0
      used_socket_count: 1
      generator {
        seed: 858994610
        gb_handle {
          game_balance_type: 2
          gbid: 1978116233
        }
        base_affixes: -1613479975
        base_affixes: 966382508
        base_affixes: -1064392808
        base_affixes: -1588234626
        base_affixes: -1170857815
        base_affixes: 1342579558
        flags: 67851
        durability: 350
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 350
        contents {
          id {
            id_high: 0
            id_low: 2395915002591458961
          }
          generator {
            seed: 1974443180
            gb_handle {
              game_balance_type: 2
              gbid: -1046348118
            }
            flags: 67851
            durability: 0
            stack_size: 0
            item_quality_level: 9
            item_binding_level: 2
            season_created: 0
            jewel_rank: 50
          }
        }
        legendary_item_level: 70
        season_created: 0
        enchanted_affix_old: -1588234626
        enchanted_affix_new: 232461798
        legendary_base_item_gbid: 1146967350
        enchanted_affix_seed: 944591465
        enchanted_affix_count: 15
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458826
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 464
      square_index: 0
      used_socket_count: 1
      generator {
        seed: 3351932141
        gb_handle {
          game_balance_type: 2
          gbid: 1978152170
        }
        base_affixes: -670436801
        base_affixes: -1322426367
        base_affixes: 1363430979
        base_affixes: 232461798
        base_affixes: -1170857815
        base_affixes: 1342579558
        flags: 67851
        durability: 269
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 269
        contents {
          id {
            id_high: 0
            id_low: 2395915002591458827
          }
          generator {
            seed: 1498634001
            gb_handle {
              game_balance_type: 2
              gbid: -1045090323
            }
            flags: 67851
            durability: 0
            stack_size: 0
            item_quality_level: 9
            item_binding_level: 2
            season_created: 0
            jewel_rank: 50
          }
        }
        legendary_item_level: 70
        season_created: 0
        enchanted_affix_old: 1363430979
        enchanted_affix_new: -1064392808
        legendary_base_item_gbid: 1146967350
        enchanted_affix_seed: 2940133443
        enchanted_affix_count: 32
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458802
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 480
      square_index: 0
      used_socket_count: 1
      generator {
        seed: 1872323009
        gb_handle {
          game_balance_type: 2
          gbid: 1528310934
        }
        base_affixes: -1254226428
        base_affixes: 1342579558
        base_affixes: -889190756
        base_affixes: 1913242032
        base_affixes: -993623678
        base_affixes: -627228777
        flags: 68875
        durability: 337
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 337
        contents {
          id {
            id_high: 0
            id_low: 2395915002591458817
          }
          generator {
            seed: 1439668742
            gb_handle {
              game_balance_type: 2
              gbid: -1046204370
            }
            flags: 67851
            durability: 0
            stack_size: 0
            item_quality_level: 9
            item_binding_level: 2
            season_created: 0
            jewel_rank: 34
          }
        }
        legendary_item_level: 70
        season_created: 0
        enchanted_affix_old: -889190756
        enchanted_affix_new: 783505022
        legendary_base_item_gbid: 1682228656
        enchanted_affix_seed: 2536073229
        enchanted_affix_count: 3
      }
    }
    items {
      id {
        id_high: 0
        id_low: 738590339582893671
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 592
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 3927525989
        gb_handle {
          game_balance_type: 2
          gbid: -2018707798
        }
        flags: 43275
        durability: 357
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 357
        season_created: 0
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458974
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 1
      item_slot: 1296
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 3413238101
        gb_handle {
          game_balance_type: 2
          gbid: 161312714
        }
        base_affixes: -744661497
        base_affixes: -436436258
        base_affixes: -746302019
        base_affixes: -889190756
        base_affixes: -993623678
        flags: 3339
        durability: 365
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 365
        legendary_item_level: 70
        season_created: 0
        legendary_base_item_gbid: 1815809043
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458998
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 1
      item_slot: 1312
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 1990428352
        gb_handle {
          game_balance_type: 2
          gbid: 1350507102
        }
        base_affixes: 1154440475
        base_affixes: 996645142
        base_affixes: -889190756
        base_affixes: -993623678
        base_affixes: -1034448274
        flags: 3339
        durability: 444
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 444
        legendary_item_level: 70
        season_created: 0
        legendary_base_item_gbid: -270935653
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591459003
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 1
      item_slot: 1328
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 3638447227
        gb_handle {
          game_balance_type: 2
          gbid: -765770609
        }
        base_affixes: -708791752
        base_affixes: -362198678
        base_affixes: 1014533493
        base_affixes: 1913242032
        base_affixes: -993623679
        flags: 2315
        durability: 307
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 307
        legendary_item_level: 70
        season_created: 0
        legendary_base_item_gbid: 1147341804
      }
    }
    items {
      id {
        id_high: 0
        id_low: 1423137484191371416
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 1
      item_slot: 1344
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 168474578
        gb_handle {
          game_balance_type: 2
          gbid: 1566296343
        }
        base_affixes: -993623678
        base_affixes: 783505022
        base_affixes: -942168372
        base_affixes: -1985526412
        base_affixes: -522962331
        flags: 44299
        durability: 286
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 286
        legendary_item_level: 70
        season_created: 0
        legendary_base_item_gbid: 1682228656
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458996
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 1
      item_slot: 1360
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 1448599879
        gb_handle {
          game_balance_type: 2
          gbid: 1978152170
        }
        base_affixes: -670436801
        base_affixes: -1322426367
        base_affixes: 717893392
        base_affixes: -1064392808
        base_affixes: -1170857815
        base_affixes: 1342579558
        flags: 2315
        durability: 351
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 351
        legendary_item_level: 70
        season_created: 0
        legendary_base_item_gbid: 1146967350
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458987
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 1
      item_slot: 1376
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 840216012
        gb_handle {
          game_balance_type: 2
          gbid: -1187794594
        }
        base_affixes: 966382508
        base_affixes: 1363430979
        base_affixes: 232461798
        base_affixes: -1170857815
        flags: 2315
        durability: 384
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 384
        legendary_item_level: 70
        season_created: 0
        legendary_base_item_gbid: 1146967350
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458972
      }
      owner_entity_id {
        id_high: 0
        id_low: 0
      }
      hireling_class: 2
      item_slot: 1312
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 1960495955
        gb_handle {
          game_balance_type: 2
          gbid: -2091504069
        }
        base_affixes: -2113755303
        base_affixes: 1656971390
        flags: 2313
        durability: 499
        stack_size: 0
        item_quality_level: 4
        max_durability: 499
        season_created: 0
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591459008
      }
      owner_entity_id {
        id_high: 0
        id_low: 0
      }
      hireling_class: 2
      item_slot: 1328
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 554456963
        gb_handle {
          game_balance_type: 2
          gbid: 978821514
        }
        base_affixes: 555585138
        base_affixes: -362198678
        base_affixes: 611264315
        base_affixes: -889190757
        base_affixes: -1598227767
        flags: 2315
        durability: 443
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 443
        legendary_item_level: 70
        season_created: 0
        legendary_base_item_gbid: -229899866
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458980
      }
      owner_entity_id {
        id_high: 0
        id_low: 0
      }
      hireling_class: 3
      item_slot: 1312
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 1372590090
        gb_handle {
          game_balance_type: 2
          gbid: -1170762813
        }
        base_affixes: 1661455571
        base_affixes: -595374178
        base_affixes: -1765925748
        base_affixes: 1342579558
        base_affixes: -303435822
        flags: 2315
        durability: 374
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 374
        legendary_item_level: 70
        season_created: 0
        legendary_base_item_gbid: -2115688088
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458997
      }
      owner_entity_id {
        id_high: 0
        id_low: 0
      }
      hireling_class: 3
      item_slot: 1328
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 2470846102
        gb_handle {
          game_balance_type: 2
          gbid: -450681608
        }
        base_affixes: -863531393
        base_affixes: -889190757
        base_affixes: -362198678
        base_affixes: 1913242032
        base_affixes: -1392021261
        flags: 2315
        durability: 402
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 402
        legendary_item_level: 70
        season_created: 0
        legendary_base_item_gbid: 761439029
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458981
      }
      owner_entity_id {
        id_high: 0
        id_low: 0
      }
      hireling_class: 3
      item_slot: 1344
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 3934740857
        gb_handle {
          game_balance_type: 2
          gbid: 1682228656
        }
        base_affixes: -993623680
        base_affixes: -889190758
        base_affixes: -1519552049
        base_affixes: 71995443
        base_affixes: 512347501
        rare_item_name {
          item_name_is_prefix: false
          sno_affix_string_list: 130729
          affix_string_list_index: 2
          item_string_list_index: 5
        }
        flags: 2313
        durability: 405
        stack_size: 0
        item_quality_level: 7
        max_durability: 405
        season_created: 0
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458993
      }
      owner_entity_id {
        id_high: 0
        id_low: 0
      }
      hireling_class: 3
      item_slot: 1360
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 1165105980
        gb_handle {
          game_balance_type: 2
          gbid: -1188729220
        }
        base_affixes: -1307815087
        base_affixes: -1588234626
        base_affixes: 1311365575
        base_affixes: -1985526412
        base_affixes: 886664040
        flags: 2315
        durability: 368
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 368
        legendary_item_level: 67
        season_created: 0
        legendary_base_item_gbid: 1146967349
      }
    }
    items {
      id {
        id_high: 0
        id_low: 2395915002591458994
      }
      owner_entity_id {
        id_high: 0
        id_low: 0
      }
      hireling_class: 3
      item_slot: 1376
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 3751822759
        gb_handle {
          game_balance_type: 2
          gbid: -1149737311
        }
        base_affixes: 966382508
        base_affixes: 1363430979
        base_affixes: 611264315
        base_affixes: 1151246290
        base_affixes: -1170857815
        flags: 2315
        durability: 303
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 303
        legendary_item_level: 70
        season_created: 0
        legendary_base_item_gbid: 1146967350
      }
    }
    items {
      id {
        id_high: 0
        id_low: 738590339582893671
      }
      owner_entity_id {
        id_high: 0
        id_low: 68056897
      }
      hireling_class: 0
      item_slot: 592
      square_index: 0
      used_socket_count: 0
      generator {
        seed: 3927525989
        gb_handle {
          game_balance_type: 2
          gbid: -2018707798
        }
        flags: 43275
        durability: 357
        stack_size: 0
        item_quality_level: 9
        item_binding_level: 2
        max_durability: 357
        season_created: 0
      }
    }
  }
  sno_active_skills {
    runes {
      skill: 286510
      rune_type: 4
    }
    runes {
      skill: 239042
      rune_type: 3
    }
    runes {
      skill: 291804
      rune_type: 1
    }
    runes {
      skill: 243853
      rune_type: 1
    }
    runes {
      skill: 342281
      rune_type: 2
    }
    runes {
      skill: 269032
      rune_type: 4
    }
  }
  sno_traits {
    sno_traits: 286177
    sno_traits: 311629
    sno_traits: 310804
    sno_traits: 309830
  }
  legendary_powers {
    gbid_legendary_powers: -990363367
    gbid_legendary_powers: -1046338483
    gbid_legendary_powers: 417069418
  }
}
reward_gbid: 456652071
target_millisecond: 362116
challenge_end_time_unix_seconds: 1583200800

             */
            #endregion

            return response.Build().ToByteString();
        }
        private ByteString ClearMissions(BattleClient client, ByteString data)
        {
            var request = ResetHeroStoryProgress.ParseFrom(data);
            return request.ToByteString();
        }
        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        private ByteString GetGameAccountSettings(BattleClient client)
        {
            Logger.Debug("GetGameAccountSettings()");

            var gameAccount = client.Account.GameAccount;
            return gameAccount.Settings.ToByteString();
        }
        private ByteString SetGameAccountSettings(SetGameAccountSettings settings, BattleClient client)
        {
            Logger.Debug("SetGameAccountSettings()");

            client.Account.GameAccount.Settings = settings.Settings;
            return ByteString.Empty;
        }
        private static BNetPacket GetPacketFromHexByteArray(byte[] bytemessage)
        {
            DotNetty.Buffers.IByteBuffer BB = DotNetty.Buffers.Unpooled.WrappedBuffer(bytemessage);
            DotNetty.Codecs.Http.WebSockets.BinaryWebSocketFrame msg = new DotNetty.Codecs.Http.WebSockets.BinaryWebSocketFrame(BB);
            if (msg.Content.ReadableBytes < 2)
            {
                return null;
            }

            int headerSize = msg.Content.ReadUnsignedShort();
            if (msg.Content.ReadableBytes < headerSize)
            {
                return null;
            }
            byte[] headerBuf = new byte[headerSize];
            msg.Content.ReadBytes(headerBuf);
            Header header = Header.ParseFrom(headerBuf);

            int payloadSize = header.HasSize ? (int)header.Size : msg.Content.ReadableBytes;
            if (msg.Content.ReadableBytes < payloadSize)
            {
                return null;
            }
            byte[] payload = new byte[payloadSize];
            msg.Content.ReadBytes(payload);

            return new BNetPacket(header, payload);
        }
        private ByteString OnGetAccountPrefs(BattleClient client, ByteString data) => ByteString.Empty;

        #endregion

    }
}
