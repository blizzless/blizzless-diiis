//Blizzless Project 2022
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using bgs.protocol;
//Blizzless Project 2022 
using bgs.protocol.authentication.v1;
//Blizzless Project 2022 
using bgs.protocol.challenge.v1;
//Blizzless Project 2022 
using DiIiS_NA.Core.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Base;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Battle;
//Blizzless Project 2022 
using Google.ProtocolBuffers;


namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0x1, serviceHash: 233634817)]
    public class AuthenticationService : bgs.protocol.authentication.v1.AuthenticationService, IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        public static int CounterLogged = 0;
        public static bool switcher = false;


        public override void GenerateSSOToken(IRpcController controller, GenerateSSOTokenRequest request, Action<GenerateSSOTokenResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GenerateWebCredentials(IRpcController controller, GenerateWebCredentialsRequest request, Action<GenerateWebCredentialsResponse> done)
        {
            uint Product = request.Program;
            string a = ByteString.CopyFromUtf8("CN-97e2280792852a25e356fc9897f8bcd5-525139407").ToStringUtf8();

            var Response = GenerateWebCredentialsResponse.CreateBuilder().SetWebCredentials(ByteString.CopyFromUtf8(a));
            done(Response.Build());
        }

        public override void Logon(IRpcController controller, LogonRequest request, Action<NoData> done)
        {
            //Error 28 - There is an update for Diablo III, the client is closing.
            //Error 33 - Maintenance
            //Error 35 - Battle.net service - Disabled
            //Error 36 - Failed to load authentication module
            //Error 37 - Authentication service is receiving too many requests.
            //Error 38 - To play you need to get BattleTag
            //Error 42 - You are connecting to the wrong server (Wrong sequence of actions)
            //Error 43 - You blocked your account from your mobile phone.
            //Error 44 - Unable to perform this action. The account is deprived of the function of voice communication.
            //Error 50 - Prepaid time for the account has expired.
            //Error 51 - Subscription for this account has expired.
            //Error 52 - This account has been blocked due to numerous violations of the terms of use of the Battle.net service
            //Error 53: Action of this account has been suspended due to violations of the terms of use of the Batle.net service

            int VersionRetail = 81850; //74291 - 2.7.0, 76761 - 2.7.1, 79575 - 2.7.2;
            int VersionPTR = 79151;
            string version = "";
            int a = request.ApplicationVersion;
            Logger.Info("----------------------------------------------------------------");
            string game = "-----";
            switch (request.Program.ToLower())
            {
                case "d3": game = "Diablo 3"; break;
                case "osi": game = "Diablo 2 Ressurected"; break;
                case "odin": game = "Call of Duty: Warzone"; break;
                case "pro": game = "Overwatch"; break;
                case "proc": game = "Overwatch Tournament"; break;
                case "app": game = "Battle.net Launcher"; break;
                case "s2": game = "Starcraft 2"; break;
                case "hero": game = "Heroes of the Storm"; break;
                case "fen": game = "Diablo IV"; break;
            }
            if (request.ApplicationVersion == 0)
            {
                var parts = request.Version.Split('"');
                if (parts.Length > 1)
                    version = parts[1];
            }
            else
                version = request.ApplicationVersion.ToString();
            Logger.Info("Game: {0} | Version: {1} | Platform: {2} | Locale: {3}", game, version, request.Platform, request.Locale);
            if (request.Program.ToLower() == "d3")
                if (request.ApplicationVersion != VersionRetail & request.ApplicationVersion != VersionPTR)
                {
                    //Logger.Error("Connecting the wrong client version!");
                    var ercomplete = LogonResult.CreateBuilder().SetErrorCode(28);
                    //((HandlerController)controller).Client.MakeRPC((lid) => AuthenticationListener.CreateStub(((HandlerController)controller).Client).OnLogonComplete(controller, ercomplete.Build(), callback => { }));
                }
            switch (request.Locale)
            {
                case "deDE": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.deDE; break;
                case "enGB": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.enGB; break;
                case "enSG": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.enSG; break;
                case "enUS": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.enUS; break;
                case "esES": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.esES; break;
                case "esMX": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.esMX; break;
                case "frFR": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.frFR; break;
                case "itIT": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.itIT; break;
                case "koKR": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.koKR; break;
                case "plPL": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.plPL; break;
                case "ptBR": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.ptBR; break;
                case "ptPT": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.ptPT; break;
                case "ruRU": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.ruRU; break;
                case "trTR": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.trTR; break;
                case "zhCN": ((HandlerController)controller).Client.ClientLanguage = Battle.BattleClient.ClientLocale.zhCN; break;
            }
            done(NoData.CreateBuilder().Build());
            Logger.Info("----------------------------------------------------------------");
            var builder = ChallengeExternalRequest.CreateBuilder();
            var complete = LogonResult.CreateBuilder();
            switch (request.Program.ToLower())
            {
                case "d3":
                    //if (!request.HasCachedWebCredentials)
                    {
                        #region Authentication procedure through WEB
                        if (request.HasCachedWebCredentials)
                            VerifyWebCredentials(controller, VerifyWebCredentialsRequest.CreateBuilder().SetWebCredentials(request.CachedWebCredentials).Build(), callback => { });
                        builder.SetPayloadType("web_auth_url");
                        if (REST.Config.Instance.Public)
                            builder.SetPayload(ByteString.CopyFromUtf8(
                                $"http://{REST.Config.Instance.PublicIP}:{REST.Config.Instance.PORT}/battlenet/login"));
                        else
                            builder.SetPayload(ByteString.CopyFromUtf8(
                                $"http://{Program.RESTSERVERIP}:{REST.Config.Instance.PORT}/battlenet/login"));

                        ((HandlerController)controller).Client.MakeRPC((lid) => ChallengeListener.CreateStub(((HandlerController)controller).Client).OnExternalChallenge(controller, builder.Build(), callback => { }));
                        #endregion
                    }
                    break;
                default:
                    Logger.Error("Authorization not implemented for Game: {0}", game);
                    Logger.Info("----------------------------------------------------------------");
                    break;
            }

        }

        public override void ModuleMessage(IRpcController controller, ModuleMessageRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void ModuleNotify(IRpcController controller, ModuleNotification request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void SelectGameAccount(IRpcController controller, SelectGameAccountRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void SelectGameAccountDEPRECATED(IRpcController controller, EntityId request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void VerifyWebCredentials(IRpcController controller, VerifyWebCredentialsRequest request, Action<NoData> done)
        {
            done(NoData.CreateBuilder().Build());
            #region Authentication complete
            if (request.WebCredentials.ToStringUtf8().ToLower().Contains("eu-"))
            {
                ((HandlerController)controller).Client.Account = AccountManager.GetAccountByPersistentID(1);

                var comple = LogonResult.CreateBuilder()
               .SetAccountId(((HandlerController)controller).Client.Account.BnetEntityId)
               .SetEmail("TEST@MAIL.DU")
               .SetBattleTag("Test#0000")
               .SetSessionKey(ByteString.CopyFrom("7CB18EDA470F96A4DD70C70B9307CBBA2A4131043075648D8B2F55EE0E383132025D3CC3BA43406DC0740D776B1E5C366BD1123D16E6D6759075B475C28C4022".ToByteArray()))
               .AddAvailableRegion(1)
               .AddAvailableRegion(2)
               .AddAvailableRegion(3)
               .SetConnectedRegion(1)
               .SetGeoipCountry("RU")
               .SetRestrictedMode(false)
               .SetErrorCode(0);
                comple.AddGameAccountId(((HandlerController)controller).Client.Account.GameAccount.BnetEntityId);
                ((HandlerController)controller).Client.Account.GameAccount.LoggedInClient = ((HandlerController)controller).Client;
                ((HandlerController)controller).Client.MakeRPC((lid) => AuthenticationListener.CreateStub(((HandlerController)controller).Client).OnLogonComplete(controller, comple.Build(), callback => { }));
                ((HandlerController)controller).Client.Account.GameAccount.ProgramField.Value = "FEN";
                PlayerManager.PlayerConnected(((HandlerController)controller).Client);
                var ga1selected = GameAccountSelectedRequest.CreateBuilder().SetResult(0).SetGameAccountId(((HandlerController)controller).Client.Account.GameAccount.BnetEntityId);
                ((HandlerController)controller).Client.MakeRPC((lid) =>
                    AuthenticationListener.CreateStub(((HandlerController)controller).Client).OnGameAccountSelected(new HandlerController() { ListenerId = lid }, ga1selected.Build(), callback => { }));

            }
            else
            {
                ((HandlerController)controller).Client.Account = AccountManager.GetAccountBySaltTicket(request.WebCredentials.ToStringUtf8());

                if (((HandlerController)controller).Client.Account == null)
                {
                    var complete = LogonResult.CreateBuilder().SetErrorCode(2);
                    ((HandlerController)controller).Client.MakeRPC((lid) => AuthenticationListener.CreateStub(((HandlerController)controller).Client).OnLogonComplete(controller, complete.Build(), callback => { }));
                    ((HandlerController)controller).Client.SocketConnection.CloseAsync();
                    ((HandlerController)controller).Client.Connect.CloseAsync();
                }
                else
                {
                    Logger.Info("Client connected - {0}#{1}", ((HandlerController)controller).Client.Account.DBAccount.BattleTagName, ((HandlerController)controller).Client.Account.HashCode);
                    Logger.Info("----------------------------------------------------------------");
                    var complete = LogonResult.CreateBuilder()
                        .SetAccountId(((HandlerController)controller).Client.Account.BnetEntityId)
                        .SetEmail(((HandlerController)controller).Client.Account.Email)
                        .SetBattleTag(((HandlerController)controller).Client.Account.BattleTag)
                        .AddAvailableRegion(1)
                        .SetConnectedRegion(1)
                        .SetGeoipCountry("RU")
                        .SetRestrictedMode(false)
                        .SetErrorCode(0);
                    complete.AddGameAccountId(((HandlerController)controller).Client.Account.GameAccount.BnetEntityId); //D3
                    ((HandlerController)controller).Client.Account.GameAccount.LoggedInClient = ((HandlerController)controller).Client;
                    ((HandlerController)controller).Client.MakeRPC((lid) => AuthenticationListener.CreateStub(((HandlerController)controller).Client).OnLogonComplete(controller, complete.Build(), callback => { }));

                    PlayerManager.PlayerConnected(((HandlerController)controller).Client);

                    var gaselected = GameAccountSelectedRequest.CreateBuilder().SetResult(0).SetGameAccountId(((HandlerController)controller).Client.Account.GameAccount.BnetEntityId);
                    ((HandlerController)controller).Client.MakeRPC((lid) =>
                        AuthenticationListener.CreateStub(((HandlerController)controller).Client).OnGameAccountSelected(new HandlerController() { ListenerId = lid }, gaselected.Build(), callback => { }));
                }
            }
            #endregion
        }
    }
}
