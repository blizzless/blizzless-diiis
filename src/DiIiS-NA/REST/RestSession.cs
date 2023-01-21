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
    public class RestSession : SocketBase
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
            SendResponse(HttpCode.OK, Global.Global.SessionMgr.GetFormInput());
        }

        public void HandleInfoRequest(HttpHeader request)
        {
            SendResponseHtml(HttpCode.OK, "Welcome to BlizzLess.Net" + 
                                          "\nBuild " + Program.Build +
                                          "\nSupport: 2.7.4");
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        void SendResponse<T>(HttpCode code, T response)
        {
            AsyncWrite(HttpHelper.CreateResponse(code, JSON.Json.CreateString(response)));
        }

        void SendResponseHtml(HttpCode code, string response)
        {
            AsyncWrite(HttpHelper.CreateResponse(code, response));
        }

        public override void Start()
        {
            AsyncRead();
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

            bool result = false;
            
            if(AccountManager.GetAccountBySaltTicket(password + " asa " + login.ToLower()) != null)
            {
                loginResult.LoginTicket = password + " asa " + login.ToLower();// "AiDiE";
                result = true;
            }

            if (result)
            {
                loginResult.AuthenticationState = "DONE";
                SendResponse(HttpCode.OK, loginResult);
                Logger.Warn("Аутентификация завершена: Логин - {0}. Cоединение с REST разорвано.", login);
            }
            else
            {
                loginResult.AuthenticationState = "LOGIN";
                loginResult.ErrorCode = "UNABLE_TO_DECODE";
                loginResult.ErrorMessage = "The information you have entered is not valid.";
                SendResponse(HttpCode.BadRequest, loginResult);
                Logger.Error("Аутентификация неудалась: Логин - {0}. Cоединение с REST разорвано.", login);
            }
            CloseSocket();
            
        }
    }
}
