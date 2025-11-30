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
using System.Net;
using System.Net.Security;
using System.Web;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.REST.Data.Forms;
using DiIiS_NA.REST.Manager;

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
                Logger.Debug($"$[yellow]$REST Request: $[/]$ {httpRequest.Method.SafeAnsi()} {httpRequest.Path.SafeAnsi()}");
                if (httpRequest.Path == "200")
                {

                }
                else if (httpRequest.Path.Contains("/client/alert"))
                {
                    HandleInfoRequest(httpRequest);
                }
                else if (httpRequest.Path.Contains("/battlenet/login"))
                {
                    switch (httpRequest.Method)
                    {
                        default:
                            HandleConnectRequest(httpRequest);
                            break;
                        case "POST":
                            HandleLoginRequest(httpRequest);
                            return;
                    }
                }
                else
                {
                    #if DEBUG
                    Logger.Info($"$[red]$404 - REST Request: $[/]$ {httpRequest.Method.SafeAnsi()} {httpRequest.Path.SafeAnsi()}");
                    SendResponseHtml(HttpCode.NotFound, "404 Not Found");
                    #else
                    // sends 502 Bad Gateway to the client to prevent the client from trying to connect to the server again - in case it's a crawler or bad bot.
                    Logger.Info($"$[red]$[404/502] REST Request: $[/]$ {httpRequest.Method.SafeAnsi()} {httpRequest.Path.SafeAnsi()}");
                    SendResponseHtml(HttpCode.BadGateway, "502 Bad Gateway");
                    return;
                    #endif
                }
            }
            AsyncRead();
        }

        void HandleConnectRequest(HttpHeader request)
        {
            SendResponse(HttpCode.OK, SessionManager.Instance.GetFormInput());
        }

        void HandleInfoRequest(HttpHeader request)
        {
            SendResponseHtml(HttpCode.OK, "Welcome to BlizzLess.Net" + 
                                          "\nBuild " + Program.BUILD +
                                          "\nSupport: 2.7.4");
        }

        void SendResponse<T>(HttpCode code, T response)
        {
            AsyncWrite(HttpHelper.CreateResponse(code, JSON.Json.CreateString(response)));
        }

        void SendResponseHtml(HttpCode code, string response)
        {
            AsyncWrite(HttpHelper.CreateResponse(code, response, contentType: "text/html"));
        }

        public override void Start()
        {
            AsyncRead();
        }

        void HandleLoginRequest(HttpHeader request)
        {
            LogonData loginForm = Json.CreateObject<LogonData>(request.Content);
            LogonResult loginResult = new LogonResult();
            if (loginForm?.Inputs is null or {Count: 0})
            {
                loginResult.AuthenticationState = "LOGIN";
                loginResult.ErrorCode = "UNABLE_TO_DECODE";
                loginResult.ErrorMessage = "There was an internal error while connecting to Battle.net. Please try again later.";
                SendResponse(HttpCode.BadRequest, loginResult);
                return;
            }

            string login = "";
            string password = "";

            foreach (var input in loginForm.Inputs)
            {
                switch (input.Id)
                {
                    case "account_name":
                        login = input.Value;
                        break;
                    case "password":
                        password = input.Value;
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
