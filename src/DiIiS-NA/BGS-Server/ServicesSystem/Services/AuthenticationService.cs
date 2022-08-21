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
            //Error 28 - Появилось обновление Diablo III, клиент закрывается.
            //Error 33 - Профилактические работы
            //Error 35 - Служба Battle.net - Отключена
            //Error 36 - Не удалось загрузить модуль аутентификации
            //Error 37 - Служба аутентификации получает слишком много обращений.
            //Error 38 - Для игры требуется получить BattleTag
            //Error 42 - Вы подключаетесь к неверному серверу (Неверная последовательность действий)
            //Error 43 - Вы заблокировали свою учетную запись с мобильного телефона.
            //Error 44 - Невозможно выполнить это действие. Учетная запись лишена функции голосового общения.
            //Error 50 - Предоплаченное для учетной записи время игры истекло.
            //Error 51 - Подписка для данной учетной записи истекла.
            //Error 52 - Данная учетная запись была заблокирована в связи с многочисленными нарушениями условий использования службы Battle.net
            //Error 53 - Действие данной учетной записи было приостановлено в связи с нарушениями условий использования службы Batle.net.

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
                var Parts = request.Version.Split('"');
                if (Parts.Length > 1)
                    version = Parts[1];
            }
            else
                version = request.ApplicationVersion.ToString();
            Logger.Info("Game: {0} | Version: {1} | Platform: {2} | Locale: {3}", game, version, request.Platform, request.Locale);
            if (request.Program.ToLower() == "d3")
                if (request.ApplicationVersion != VersionRetail & request.ApplicationVersion != VersionPTR)
                {
                    //Logger.Error("Подключение не правильной версии клиента!");
                    var ercomplete = LogonResult.CreateBuilder().SetErrorCode(28);
                    //(controller as HandlerController).Client.MakeRPC((lid) => AuthenticationListener.CreateStub((controller as HandlerController).Client).OnLogonComplete(controller, ercomplete.Build(), callback => { }));
                }
            switch (request.Locale)
            {
                case "deDE": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.deDE; break;
                case "enGB": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.enGB; break;
                case "enSG": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.enSG; break;
                case "enUS": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.enUS; break;
                case "esES": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.esES; break;
                case "esMX": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.esMX; break;
                case "frFR": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.frFR; break;
                case "itIT": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.itIT; break;
                case "koKR": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.koKR; break;
                case "plPL": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.plPL; break;
                case "ptBR": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.ptBR; break;
                case "ptPT": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.ptPT; break;
                case "ruRU": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.ruRU; break;
                case "trTR": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.trTR; break;
                case "zhCN": (controller as HandlerController).Client.ClientLanguage = Battle.BattleClient.ClientLocale.zhCN; break;
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
                        #region Процедура аутентификации через WEB
                        if (request.HasCachedWebCredentials)
                            VerifyWebCredentials(controller, VerifyWebCredentialsRequest.CreateBuilder().SetWebCredentials(request.CachedWebCredentials).Build(), callback => { });
                        builder.SetPayloadType("web_auth_url");
                        if (REST.Config.Instance.Public)
                            builder.SetPayload(ByteString.CopyFromUtf8(String.Format("http://{0}:{1}/battlenet/login", REST.Config.Instance.PublicIP, REST.Config.Instance.PORT)));
                        else
                            builder.SetPayload(ByteString.CopyFromUtf8(String.Format("http://{0}:{1}/battlenet/login", Program.RESTSERVERIP, REST.Config.Instance.PORT)));

                        (controller as HandlerController).Client.MakeRPC((lid) => ChallengeListener.CreateStub((controller as HandlerController).Client).OnExternalChallenge(controller, builder.Build(), callback => { }));
                        #endregion
                    }
                    break;
                default:
                    Logger.Error("Authorization not implemeted for Game: {0}", game);
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
            #region Завершение аутентификации
            if (request.WebCredentials.ToStringUtf8().ToLower().Contains("eu-"))
            {
                (controller as HandlerController).Client.Account = AccountManager.GetAccountByPersistentID(1);

                var comple = LogonResult.CreateBuilder()
               .SetAccountId((controller as HandlerController).Client.Account.BnetEntityId)
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
                comple.AddGameAccountId((controller as HandlerController).Client.Account.GameAccount.BnetEntityId);
                (controller as HandlerController).Client.Account.GameAccount.LoggedInClient = (controller as HandlerController).Client;
                (controller as HandlerController).Client.MakeRPC((lid) => AuthenticationListener.CreateStub((controller as HandlerController).Client).OnLogonComplete(controller, comple.Build(), callback => { }));
                (controller as HandlerController).Client.Account.GameAccount.ProgramField.Value = "FEN";
                PlayerManager.PlayerConnected((controller as HandlerController).Client);
                var ga1selected = GameAccountSelectedRequest.CreateBuilder().SetResult(0).SetGameAccountId((controller as HandlerController).Client.Account.GameAccount.BnetEntityId);
                (controller as HandlerController).Client.MakeRPC((lid) =>
                    AuthenticationListener.CreateStub((controller as HandlerController).Client).OnGameAccountSelected(new HandlerController() { ListenerId = lid }, ga1selected.Build(), callback => { }));

            }
            else
            {
                (controller as HandlerController).Client.Account = AccountManager.GetAccountBySaltTicket(request.WebCredentials.ToStringUtf8());

                if ((controller as HandlerController).Client.Account == null)
                {
                    var complete = LogonResult.CreateBuilder().SetErrorCode(2);
                    (controller as HandlerController).Client.MakeRPC((lid) => AuthenticationListener.CreateStub((controller as HandlerController).Client).OnLogonComplete(controller, complete.Build(), callback => { }));
                    (controller as HandlerController).Client.SocketConnection.CloseAsync();
                    (controller as HandlerController).Client.Connect.CloseAsync();
                }
                else
                {
                    Logger.Info("Client connected - {0}#{1}", (controller as HandlerController).Client.Account.DBAccount.BattleTagName, (controller as HandlerController).Client.Account.HashCode);
                    Logger.Info("----------------------------------------------------------------");
                    var complete = LogonResult.CreateBuilder()
                        .SetAccountId((controller as HandlerController).Client.Account.BnetEntityId)
                        .SetEmail((controller as HandlerController).Client.Account.Email)
                        .SetBattleTag((controller as HandlerController).Client.Account.BattleTag)
                        .AddAvailableRegion(1)
                        .SetConnectedRegion(1)
                        .SetGeoipCountry("RU")
                        .SetRestrictedMode(false)
                        .SetErrorCode(0);
                    complete.AddGameAccountId((controller as HandlerController).Client.Account.GameAccount.BnetEntityId); //D3
                    (controller as HandlerController).Client.Account.GameAccount.LoggedInClient = (controller as HandlerController).Client;
                    (controller as HandlerController).Client.MakeRPC((lid) => AuthenticationListener.CreateStub((controller as HandlerController).Client).OnLogonComplete(controller, complete.Build(), callback => { }));

                    PlayerManager.PlayerConnected((controller as HandlerController).Client);

                    var gaselected = GameAccountSelectedRequest.CreateBuilder().SetResult(0).SetGameAccountId((controller as HandlerController).Client.Account.GameAccount.BnetEntityId);
                    (controller as HandlerController).Client.MakeRPC((lid) =>
                        AuthenticationListener.CreateStub((controller as HandlerController).Client).OnGameAccountSelected(new HandlerController() { ListenerId = lid }, gaselected.Build(), callback => { }));
                }
            }
            #endregion
        }
    }
}
