//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Net;
//Blizzless Project 2022 
using System.Net.Sockets;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.REST.Networking
{
    public delegate void SocketAcceptDelegate(Socket newSocket);

    public class AsyncAcceptor
    {
        public bool Start(string ip, int port)
        {
            IPAddress bindIP;
            if (!IPAddress.TryParse(ip, out bindIP))
            {
                //Log.outError(LogFilter.Network, "Server can't be started: Invalid IP-Address ({0})", ip);
                return false;
            }

            try
            {
                _listener = new TcpListener(bindIP, port);
                _listener.Start();
            }
            catch (SocketException ex)
            {
                //Log.outException(ex);
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
