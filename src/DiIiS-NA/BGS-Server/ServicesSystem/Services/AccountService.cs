//Blizzless Project 2022
using System;
using Google.ProtocolBuffers;
using bgs.protocol;
using bgs.protocol.account.v1;
using DiIiS_NA.LoginServer.Base;


namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0x2, serviceHash: 0x62DA0891)]

    public class AccountService : bgs.protocol.account.v1.AccountService, IServerService
    {
        

        public override void GetAccountState(IRpcController controller, GetAccountStateRequest request, Action<GetAccountStateResponse> done)
        {

            GetAccountStateResponse.Builder builder = GetAccountStateResponse.CreateBuilder();
            var AccState = AccountState.CreateBuilder();

            if (request.EntityId.Low == (controller as HandlerController).Client.Account.BnetEntityId.Low)
            {
                if (request.Options.FieldPrivacyInfo)
                {
                    var prv = PrivacyInfo.CreateBuilder();
                    prv.SetIsUsingRid(true);
                    prv.SetIsVisibleForViewFriends(true);
                    prv.SetIsHiddenFromFriendFinder(false);
                    AccState.SetPrivacyInfo(prv);
                    prv.SetGameInfoPrivacy(PrivacyInfo.Types.GameInfoPrivacy.PRIVACY_EVERYONE);
                }
                if (request.Options.FieldAccountLevelInfo)
                {
                    AccountLevelInfo.Builder level = AccountLevelInfo.CreateBuilder();
                   
                    level.AddLicenses(new AccountLicense.Builder().SetId(167));
                    level.AddLicenses(new AccountLicense.Builder().SetId(168));
                    level.AddLicenses(new AccountLicense.Builder().SetId(0));
                    level.AddLicenses(new AccountLicense.Builder().SetId(1));
                    level.AddLicenses(new AccountLicense.Builder().SetId(2));
                    level.AddLicenses(new AccountLicense.Builder().SetId(4));
                    level.AddLicenses(new AccountLicense.Builder().SetId(10));
                    level.AddLicenses(new AccountLicense.Builder().SetId(15));
                    level.AddLicenses(new AccountLicense.Builder().SetId(20));

                    level.SetDefaultCurrency(5395778);
                    level.SetCountry("RUS");
                    level.SetPreferredRegion(1);
                    level.SetFullName("Name LastName");
                    level.SetBattleTag((controller as HandlerController).Client.Account.BattleTag);
                    level.SetAccountPaidAny(true);
                    level.SetEmail((controller as HandlerController).Client.Account.Email).SetHeadlessAccount(false);

                    AccState.SetAccountLevelInfo(level);

                    builder.SetTags(AccountFieldTags.CreateBuilder().SetAccountLevelInfoTag(3827081107));
                }
            }

            builder.SetState(AccState);
            done(builder.Build());
        }

        public override void GetAuthorizedData(IRpcController controller, GetAuthorizedDataRequest request, Action<GetAuthorizedDataResponse> done)
        {
            var Data = AuthorizedData.CreateBuilder();//.SetData("a");
            //Data.AddLicense(17459); // Diablo 3
            //Data.AddLicense(21298); // Starcraft 2 Wings of Liberty
            //Data.AddLicense(5730135); // World of Warcraft without last update
            var builder = GetAuthorizedDataResponse.CreateBuilder().AddData(Data);
            done(builder.Build());
        }

        public override void GetCAISInfo(IRpcController controller, GetCAISInfoRequest request, Action<GetCAISInfoResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetGameAccountState(IRpcController controller, GetGameAccountStateRequest request, Action<GetGameAccountStateResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetGameSessionInfo(IRpcController controller, GetGameSessionInfoRequest request, Action<GetGameSessionInfoResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetGameTimeRemainingInfo(IRpcController controller, GetGameTimeRemainingInfoRequest request, Action<GetGameTimeRemainingInfoResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetLicenses(IRpcController controller, GetLicensesRequest request, Action<GetLicensesResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetSignedAccountState(IRpcController controller, GetSignedAccountStateRequest request, Action<GetSignedAccountStateResponse> done)
        {
            done(GetSignedAccountStateResponse.CreateBuilder().SetToken("eyJ0eXAiOiJKV1QiLCJlbnYiOiJwcm9kLmV1IiwiYWxnIjoiUlMyNTYiLCJraWQiOiJmMDE5NzgzMi0zMWMwLTQzN2MtOTc2NC1iMzliOTM5MDJlNWMiLCJrdHkiOiJSU0EifQ").Build());
            //throw new NotImplementedException();
        }

        

        public override void ResolveAccount(IRpcController controller, ResolveAccountRequest request, Action<ResolveAccountResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void Subscribe(IRpcController controller, SubscriptionUpdateRequest request, Action<SubscriptionUpdateResponse> done)
        {
            ;
            var builder = SubscriptionUpdateResponse.CreateBuilder()
                    .AddRef(request.GetRef(0));//.AddRef(request.GetRef(41));
            done(builder.Build());
        }

        public override void Unsubscribe(IRpcController controller, SubscriptionUpdateRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

    }
}
