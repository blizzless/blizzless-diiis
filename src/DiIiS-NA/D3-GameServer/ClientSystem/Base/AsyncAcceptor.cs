//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Net;
//Blizzless Project 2022 
using System.Net.Sockets;

namespace DiIiS_NA.GameServer.ClientSystem.Base
{
    public delegate void SocketAcceptDelegate(Socket newSocket);
    public class AsyncAcceptor
    {
        protected static readonly Logger Logger = LogManager.CreateLogger("As");
        public bool Start(string ip, int port)
        {
            IPAddress bindIP;
            if (!IPAddress.TryParse(ip, out bindIP))
            {
                Logger.Fatal("Server can't be started: Invalid IP-Address ({0})", ip);
                return false;
            }

            try
            {
                _listener = new TcpListener(bindIP, port);

                _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                _listener.Server.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
                _listener.Server.LingerState = new LingerOption(enable: false, seconds: 0);
                _listener.Start();
            }
            catch (SocketException ex)
            {
                Logger.WarnException(ex, "Exception:");
                return false;
            }

            return true;
        }

        public async void AsyncAcceptSocket(SocketAcceptDelegate mgrHandler)
        {
            try
            {
                var _socket = await _listener.AcceptSocketAsync();
                if (_socket != null)
                {
                    mgrHandler(_socket);

                    if (!_closed)
                        AsyncAcceptSocket(mgrHandler);
                }
            }
            catch (ObjectDisposedException)
            { }
        }

        public void Close()
        {
            if (_closed)
                return;

            _closed = true;
            _listener.Stop();
        }

        TcpListener _listener;
        volatile bool _closed;
    }
}
