//Blizzless Project 2022
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using DiIiS_NA.REST.Http;
using DiIiS_NA.REST.Extensions;
using DiIiS_NA.REST.Data.Authentication;
using DiIiS_NA.REST.JSON;
using DiIiS_NA.LoginServer.AccountsSystem;
using System.IO;
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
                Logger.Info($"$[yellow]$REST Request: $[/]$ {httpRequest.Method} {httpRequest.Path}");
                if (httpRequest.Path == "200")
                {

                }
                else if (httpRequest.Path.Contains("/client/alert"))
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
            if (loginForm == default(LogonData))
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
                Logger.Warn("Authentication completed: Login - {0}.", login);
            }
            else
            {
                loginResult.AuthenticationState = "LOGIN";
                loginResult.ErrorCode = "UNABLE_TO_DECODE";
                loginResult.ErrorMessage = "The information you have entered is not valid.";
                SendResponse(HttpCode.BadRequest, loginResult);
                Logger.Error("Authentication failed: Login - {0}.", login);
            }
            CloseSocket();
            
        }
    }
}
