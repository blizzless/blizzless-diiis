//Blizzless Project 2022
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Net;
//Blizzless Project 2022 
using System.Net.Security;
//Blizzless Project 2022 
using System.Net.Sockets;
//Blizzless Project 2022 
using System.Security.Cryptography.X509Certificates;

namespace DiIiS_NA.REST
{
    public abstract class SSLSocket : ISocket
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        protected SSLSocket(Socket socket)
        {
            _socket = socket;
            _remoteAddress = ((IPEndPoint)_socket.RemoteEndPoint).Address;
            _remotePort = (ushort)((IPEndPoint)_socket.RemoteEndPoint).Port;
            _receiveBuffer = new byte[ushort.MaxValue];

            _stream = new SslStream(new NetworkStream(socket), false);
        }

        public abstract void Start();

        public virtual bool Update()
        {
            return IsOpen();
        }

        public IPAddress GetRemoteIpAddress()
        {
            return _remoteAddress;
        }

        public ushort GetRemotePort()
        {
            return _remotePort;
        }

        public void AsyncRead()
        {
            if (!IsOpen())
                return;

            try
            {
                _stream.BeginRead(_receiveBuffer, 0, _receiveBuffer.Length, ReadHandlerInternal, _stream);
            }
            catch (Exception ex)
            {
                Logger.ErrorException(ex, "");
            }
        }

        void ReadHandlerInternal(IAsyncResult result)
        {
            int bytes = 0;
            try
            {
                bytes = _stream.EndRead(result);
            }
            catch (Exception ex)
            {
                Logger.ErrorException(ex, "");
            }

            ReadHandler(bytes);
        }

        public abstract void ReadHandler(int transferredBytes);

        public void AsyncHandshake(X509Certificate2 certificate)
        {
            
            try
            {
                if(certificate == null)
                    certificate = new X509Certificate2("bnetserver.p12", "123");

                _stream.BeginAuthenticateAsServer(certificate, false, true, OnTLSAuthentication, this._stream);
            }
            catch (Exception ex)
            {
                Logger.ErrorException(ex, "");
                CloseSocket();
                return;
            }
            
        }

        void OnTLSAuthentication(IAsyncResult result)
        {
            try
            {
                _stream.EndAuthenticateAsServer(result);
                if (_stream.IsEncrypted)
                {
                    Logger.Info("Connection Established.");
                    AsyncRead();
                }
            }
            catch (Exception e)
            {
                Logger.FatalException(e, "OnTLSAuthentication() exception: ");
            }
        }

        public void AsyncWrite(byte[] data)
        {
            if (!IsOpen())
                return;

            try
            {
                _stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Logger.ErrorException(ex, "");
            }
        }

        public void CloseSocket()
        {
            try
            {
                _closed = true;
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
            catch (Exception ex)
            {
                Logger.ErrorException(ex, GetRemoteIpAddress().ToString());
                //Log.outDebug(LogFilter.Network, "WorldSocket.CloseSocket: {0} errored when shutting down socket: {1}", GetRemoteIpAddress().ToString(), ex.Message);
            }

            OnClose();
        }

        public void SetNoDelay(bool enable)
        {
            _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, enable);
        }

        public virtual void OnClose() { }

        public bool IsOpen() { return !_closed; }

        public byte[] GetReceiveBuffer()
        {
            return _receiveBuffer;
        }

        Socket _socket;
        internal SslStream _stream;
        byte[] _receiveBuffer;

        volatile bool _closed;

        IPAddress _remoteAddress;
        ushort _remotePort;
    }
}
