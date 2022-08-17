//Blizzless Project 2022
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;
//Blizzless Project 2022 
using System.Net.Sockets;
//Blizzless Project 2022 
using System.Security.Cryptography;
//Blizzless Project 2022 
using System.Security.Cryptography.X509Certificates;
//Blizzless Project 2022 
using DiIiS_NA.REST.Http;
//Blizzless Project 2022 
using DiIiS_NA.REST.Extensions;
//Blizzless Project 2022 
using DiIiS_NA.REST.Data.Authentication;
//Blizzless Project 2022 
using DiIiS_NA.REST.JSON;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using System.IO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.REST
{
    public enum LoginStatements
    {
        SEL_REALMLIST,
        DEL_EXPIRED_IP_BANS,
        UPD_EXPIRED_ACCOUNT_BANS,
        SEL_IP_INFO,
        INS_IP_AUTO_BANNED,
        SEL_ACCOUNT_BANNED_ALL,
        SEL_ACCOUNT_BANNED_BY_USERNAME,
        DEL_ACCOUNT_BANNED,
        UPD_ACCOUNT_INFO_CONTINUED_SESSION,
        SEL_ACCOUNT_INFO_CONTINUED_SESSION,
        UPD_VS,
        SEL_ACCOUNT_ID_BY_NAME,
        SEL_ACCOUNT_LIST_BY_NAME,
        SEL_ACCOUNT_INFO_BY_NAME,
        SEL_ACCOUNT_LIST_BY_EMAIL,
        SEL_ACCOUNT_BY_IP,
        INS_IP_BANNED,
        DEL_IP_NOT_BANNED,
        SEL_IP_BANNED_ALL,
        SEL_IP_BANNED_BY_IP,
        SEL_ACCOUNT_BY_ID,
        INS_ACCOUNT_BANNED,
        UPD_ACCOUNT_NOT_BANNED,
        DEL_REALM_CHARACTERS_BY_REALM,
        DEL_REALM_CHARACTERS,
        INS_REALM_CHARACTERS,
        SEL_SUM_REALM_CHARACTERS,
        INS_ACCOUNT,
        INS_REALM_CHARACTERS_INIT,
        UPD_EXPANSION,
        UPD_ACCOUNT_LOCK,
        UPD_ACCOUNT_LOCK_COUNTRY,
        INS_LOG,
        UPD_USERNAME,
        UPD_PASSWORD,
        UPD_EMAIL,
        UPD_REG_EMAIL,
        UPD_MUTE_TIME,
        UPD_MUTE_TIME_LOGIN,
        UPD_LAST_IP,
        UPD_LAST_ATTEMPT_IP,
        UPD_ACCOUNT_ONLINE,
        UPD_UPTIME_PLAYERS,
        DEL_OLD_LOGS,
        DEL_ACCOUNT_ACCESS,
        DEL_ACCOUNT_ACCESS_BY_REALM,
        INS_ACCOUNT_ACCESS,
        GET_ACCOUNT_ID_BY_USERNAME,
        GET_ACCOUNT_ACCESS_GMLEVEL,
        GET_GMLEVEL_BY_REALMID,
        GET_USERNAME_BY_ID,
        SEL_CHECK_PASSWORD,
        SEL_CHECK_PASSWORD_BY_NAME,
        SEL_PINFO,
        SEL_PINFO_BANS,
        SEL_GM_ACCOUNTS,
        SEL_ACCOUNT_INFO,
        SEL_ACCOUNT_ACCESS_GMLEVEL_TEST,
        SEL_ACCOUNT_ACCESS,
        SEL_ACCOUNT_RECRUITER,
        SEL_BANS,
        SEL_ACCOUNT_WHOIS,
        SEL_REALMLIST_SECURITY_LEVEL,
        DEL_ACCOUNT,
        SEL_IP2NATION_COUNTRY,
        SEL_AUTOBROADCAST,
        SEL_LAST_ATTEMPT_IP,
        SEL_LAST_IP,
        GET_EMAIL_BY_ID,
        INS_ALDL_IP_LOGGING,
        INS_FACL_IP_LOGGING,
        INS_CHAR_IP_LOGGING,
        INS_FALP_IP_LOGGING,

        SEL_ACCOUNT_ACCESS_BY_ID,
        SEL_RBAC_ACCOUNT_PERMISSIONS,
        INS_RBAC_ACCOUNT_PERMISSION,
        DEL_RBAC_ACCOUNT_PERMISSION,

        INS_ACCOUNT_MUTE,
        SEL_ACCOUNT_MUTE_INFO,
        DEL_ACCOUNT_MUTED,

        SEL_BNET_AUTHENTICATION,
        UPD_BNET_AUTHENTICATION,
        SEL_BNET_ACCOUNT_INFO,
        UPD_BNET_LAST_LOGIN_INFO,
        UPD_BNET_GAME_ACCOUNT_LOGIN_INFO,
        SEL_BNET_CHARACTER_COUNTS_BY_ACCOUNT_ID,
        SEL_BNET_CHARACTER_COUNTS_BY_BNET_ID,
        SEL_BNET_LAST_PLAYER_CHARACTERS,
        DEL_BNET_LAST_PLAYER_CHARACTERS,
        INS_BNET_LAST_PLAYER_CHARACTERS,
        INS_BNET_ACCOUNT,
        SEL_BNET_ACCOUNT_EMAIL_BY_ID,
        SEL_BNET_ACCOUNT_ID_BY_EMAIL,
        UPD_BNET_PASSWORD,
        SEL_BNET_ACCOUNT_SALT_BY_ID,
        SEL_BNET_CHECK_PASSWORD,
        UPD_BNET_ACCOUNT_LOCK,
        UPD_BNET_ACCOUNT_LOCK_CONTRY,
        SEL_BNET_ACCOUNT_ID_BY_GAME_ACCOUNT,
        UPD_BNET_GAME_ACCOUNT_LINK,
        SEL_BNET_MAX_ACCOUNT_INDEX,
        SEL_BNET_GAME_ACCOUNT_LIST,

        UPD_BNET_FAILED_LOGINS,
        INS_BNET_ACCOUNT_AUTO_BANNED,
        DEL_BNET_EXPIRED_ACCOUNT_BANNED,
        UPD_BNET_RESET_FAILED_LOGINS,

        SEL_LAST_CHAR_UNDELETE,
        UPD_LAST_CHAR_UNDELETE,

        SEL_ACCOUNT_TOYS,
        REP_ACCOUNT_TOYS,

        SEL_BATTLE_PETS,
        INS_BATTLE_PETS,
        DEL_BATTLE_PETS,
        UPD_BATTLE_PETS,
        SEL_BATTLE_PET_SLOTS,
        INS_BATTLE_PET_SLOTS,
        DEL_BATTLE_PET_SLOTS,

        SEL_ACCOUNT_HEIRLOOMS,
        REP_ACCOUNT_HEIRLOOMS,

        SEL_ACCOUNT_MOUNTS,
        REP_ACCOUNT_MOUNTS,

        SEL_BNET_ITEM_APPEARANCES,
        INS_BNET_ITEM_APPEARANCES,
        SEL_BNET_ITEM_FAVORITE_APPEARANCES,
        INS_BNET_ITEM_FAVORITE_APPEARANCE,
        DEL_BNET_ITEM_FAVORITE_APPEARANCE,

        MAX_LOGINDATABASE_STATEMENTS
    }
    public class RestSession : SocketBase//SSLSocket
    {
        public static bool ToGet = false;
        public static int b = 0;
        private static readonly Core.Logging.Logger Logger = Core.Logging.LogManager.CreateLogger();
        private readonly GameBitBuffer _incomingBuffer = new GameBitBuffer(ushort.MaxValue);
        private object _bufferLock = new object(); 


        public RestSession(Socket socket) : base(socket) { }

        public override void ReadHandler(int transferredBytes)
        {
            byte[] a = GetReceiveBuffer();
            var httpRequest = HttpHelper.ParseRequest(GetReceiveBuffer(), transferredBytes);
            if (httpRequest == null)
            {
                return;
            }
            else
            {
                if (httpRequest.Path == "200")
                {
                    //HandleLoginRequest(httpRequest);
                    //SendResponse(HttpCode.OK, Global.Global.SessionMgr.GetFormInput());
                }
                else if (httpRequest.Path == "/fakeclient/")
                {
                    switch (httpRequest.Method)
                    {
                        case "GET":
                        default:
                            SendResponseAlt(HttpCode.OK);
                            break;

                    }
                }
                else if (httpRequest.Path == "/client/alert?targetRegion=ruRU")
                {
                    switch (httpRequest.Method)
                    {
                        case "GET":
                        default:
                            HandleInfoRequest(httpRequest);
                            break;
                    }
                }
                else if (httpRequest.Path == "/D3/ruRU/client/alert?targetRegion=127")
                {
                    switch (httpRequest.Method)
                    {
                        case "GET":
                        default:
                            HandleInfoRequest(httpRequest);
                            break;
                    }
                }
                else if (httpRequest.Path == "/shop" || httpRequest.Path == "/D3/ruRU/client/alert?targetRegion=US" || httpRequest.Path == "/D3/ruRU/client/alert?targetRegion=EU")
                {
                    switch (httpRequest.Method)
                    {
                        case "GET":
                        default:
                            Shop(httpRequest);
                            break;
                    }
                }
                else if (httpRequest.Path == "/key" || httpRequest.Path == "/key/" || httpRequest.Path == "/bgs-key-fingerprint"
                    || httpRequest.Path == "/Bnet/zxx/client/bgs-key-fingerprint")
                {
                    switch (httpRequest.Method)
                    {
                        case "GET":
                        default:
                            Logger.Info("...return bgs-key-fingerprint.");
                            Key(httpRequest);
                            break;
                    }
                }
                else
                {
                    switch (httpRequest.Method)
                    {
                        case "GET":
                        default:
                            HandleConnectRequest(httpRequest);
                            break;
                        case "POST":
                            HandleLoginRequest(httpRequest);
                            return;
                    }
                }
            }
            AsyncRead();
        }

        public void HandleConnectRequest(HttpHeader request)
        {
            // Login form is the same for all clients...
            SendResponse(HttpCode.OK, Global.Global.SessionMgr.GetFormInput());
        }

        public void HandleInfoRequest(HttpHeader request)
        {
            SendResponseHtml(HttpCode.OK, "     Welcome to BlizzLess.Net" + //System.Environment.NewLine +
                                          "\nBuild " + Program.Build +// + System.Environment.NewLine +
                                          "\nSupport: 2.7.1.77744, PTR: 2.7.2.78988");
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }


        public void Key(HttpHeader request)
        {
            //SendBytes(StringToByteArray("485454502f312e3120323030204f4b0d0a5365727665723a204170616368652f322e322e3135202843656e744f53290d0a436f6e74656e742d547970653a20746578742f706c61696e3b20636861727365743d5554462d380d0a582d5661726e6973683a203434393539303836380d0a5669613a20312e31207661726e6973682d76340d0a4163636570742d52616e6765733a2062797465730d0a582d4c4c49443a2032346361623335333637316439643831333564356163346536346561363766640d0a4167653a2032343434390d0a446174653a205361742c2031302041707220323032312030313a31333a343320474d540d0a4c6173742d4d6f6469666965643a205475652c2030322046656220323032312030313a35333a343020474d540d0a436f6e74656e742d4c656e6774683a2033323736310d0a436f6e6e656374696f6e3a206b6565702d616c6976650d0a0d0a"));
            //SendResponseAlt(HttpCode.OK);
            //SendResponseAlt(HttpCode.OK);
            //SendResponseHtmlAlt(HttpCode.OK, System.IO.File.ReadAllText("bgs-key-fingerprint"));
            SendResponseHtmlAlt(HttpCode.OK, "");
        }


        public void Shop(HttpHeader request)
        {

            SendResponseHtml(HttpCode.OK, "     Welcome to BlizzLess.Net" + //System.Environment.NewLine +
                                          "\nBuild " + Program.Build +// + System.Environment.NewLine +
                                          "\nSupport: 2.7.1.77744, PTR: 2.7.2.78988");
        }

        void SendResponse<T>(HttpCode code, T response)
        {
            AsyncWrite(HttpHelper.CreateResponse(code, JSON.Json.CreateString(response)));
        }

        void SendBytes(byte[] array)
        {
            AsyncWrite(array);
        }

        void SendResponseAlt(HttpCode code)
        {
            AsyncWrite(HttpHelper.CreateResponse2(code));
        }

        void SendResponseByte(HttpCode code, byte[] array)
        {
            AsyncWrite(HttpHelper.CreateResponse1(code, array));
        }

        void SendResponseHtml(HttpCode code, string response)
        {
            AsyncWrite(HttpHelper.CreateResponse(code, response));
        }

        void SendResponseHtmlAlt(HttpCode code, string response)
        {
            AsyncWrite(HttpHelper.CreateResponseAlt(code, response));
            //CloseSocket();
            
        }

        public override void Start()
        {
            AsyncRead();
        }

        string CalculateShaPassHash(string name, string password)
        {
            SHA256 sha256 = SHA256.Create();
            var i = sha256.ComputeHash(Encoding.UTF8.GetBytes(name));
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(i.ToHexString() + ":" + password)).ToHexString();
        }

        public void HandleLoginRequest(HttpHeader request)
        {
            LogonData loginForm = Json.CreateObject<LogonData>(request.Content);
            LogonResult loginResult = new LogonResult();
            if (loginForm == null)
            {
                loginResult.AuthenticationState = "LOGIN";
                loginResult.ErrorCode = "UNABLE_TO_DECODE";
                loginResult.ErrorMessage = "There was an internal error while connecting to Battle.net. Please try again later.";
                SendResponse(HttpCode.BadRequest, loginResult);
                return;
            }

            string login = "";
            string password = "";

            for (int i = 0; i < loginForm.Inputs.Count; ++i)
            {
                switch (loginForm.Inputs[i].Id)
                {
                    case "account_name":
                        login = loginForm.Inputs[i].Value;
                        break;
                    case "password":
                        password = loginForm.Inputs[i].Value;
                        break;
                }
            }

            //PreparedStatement stmt = DB.Login.GetPreparedStatement(LoginStatements.SEL_BNET_AUTHENTICATION);
            //stmt.AddValue(0, login);

            //SQLResult result = DB.Login.Query(stmt);
            bool result = false;
            
            if(AccountManager.GetAccountBySaltTicket(password + " asa " + login.ToLower()) != null)
            {
                loginResult.LoginTicket = password + " asa " + login.ToLower();// "AiDiE";
                result = true;
            }

            //result = true;
            if (result)// || loginForm.Inputs.Count == 0)
            {
                #region
                /*
                uint accountId = result.Read<uint>(0);
                string pass_hash = result.Read<string>(1);
                uint failedLogins = result.Read<uint>(2);
                string loginTicket = result.Read<string>(3);
                uint loginTicketExpiry = result.Read<uint>(4);
                bool isBanned = result.Read<ulong>(5) != 0;

                if (CalculateShaPassHash(login, password) == pass_hash)
                {
                    if (loginTicket.IsEmpty() || loginTicketExpiry < Time.UnixTime)
                    {
                        byte[] ticket = new byte[0].GenerateRandomKey(20);
                        loginTicket = "TC-" + ticket.ToHexString();
                    }

                    stmt = DB.Login.GetPreparedStatement(LoginStatements.UPD_BNET_AUTHENTICATION);
                    stmt.AddValue(0, loginTicket);
                    stmt.AddValue(1, Time.UnixTime + 3600);
                    stmt.AddValue(2, accountId);

                    DB.Login.Execute(stmt);
                    loginResult.LoginTicket = loginTicket;
                }
                else if (!isBanned)
                {
                    uint maxWrongPassword = ConfigMgr.GetDefaultValue("WrongPass.MaxCount", 0u);

                    if (ConfigMgr.GetDefaultValue("WrongPass.Logging", false))
                        Log.outDebug(LogFilter.Network, "[{0}, Account {1}, Id {2}] Attempted to connect with wrong password!", request.Host, login, accountId);

                    if (maxWrongPassword != 0)
                    {
                        SQLTransaction trans = new SQLTransaction();
                        stmt = DB.Login.GetPreparedStatement(LoginStatements.UPD_BNET_FAILED_LOGINS);
                        stmt.AddValue(0, accountId);
                        trans.Append(stmt);

                        ++failedLogins;

                        Log.outDebug(LogFilter.Network, "MaxWrongPass : {0}, failed_login : {1}", maxWrongPassword, accountId);

                        if (failedLogins >= maxWrongPassword)
                        {
                            BanMode banType = ConfigMgr.GetDefaultValue("WrongPass.BanType", BanMode.Ip);
                            int banTime = ConfigMgr.GetDefaultValue("WrongPass.BanTime", 600);

                            if (banType == BanMode.Account)
                            {
                                stmt = DB.Login.GetPreparedStatement(LoginStatements.INS_BNET_ACCOUNT_AUTO_BANNED);
                                stmt.AddValue(0, accountId);
                            }
                            else
                            {
                                stmt = DB.Login.GetPreparedStatement(LoginStatements.INS_IP_AUTO_BANNED);
                                stmt.AddValue(0, request.Host);
                            }

                            stmt.AddValue(1, banTime);
                            trans.Append(stmt);

                            stmt = DB.Login.GetPreparedStatement(LoginStatements.UPD_BNET_RESET_FAILED_LOGINS);
                            stmt.AddValue(0, accountId);
                            trans.Append(stmt);
                        }

                        DB.Login.CommitTransaction(trans);
                    }
                }
                */
                #endregion

                //TODO: Передача уникального тикета

                loginResult.AuthenticationState = "DONE";
                SendResponse(HttpCode.OK, loginResult);
                Logger.Warn("Аутентификация завершена: Логин - {0}. Cоединение с REST разорвано.", login);
            }
            else
            {
                loginResult.AuthenticationState = "LOGIN";
                loginResult.ErrorCode = "UNABLE_TO_DECODE";
                loginResult.ErrorMessage = "HEBEPHO! WRONG! English do you speak IT?!";
                SendResponse(HttpCode.BadRequest, loginResult);
                Logger.Error("Аутентификация неудалась: Логин - {0}. Cоединение с REST разорвано.", login);
            }

            //Аутентификация завершена 
            CloseSocket();
            
        }
    }
}
