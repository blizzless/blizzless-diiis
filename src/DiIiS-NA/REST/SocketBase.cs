//Blizzless Project 2022
using DiIiS_NA.Core.Logging;
using System;
using System.Net;
using System.Net.Sockets;

namespace DiIiS_NA.REST
{
    public abstract class SocketBase : ISocket, IDisposable
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        protected SocketBase(Socket socket)
        {
            _socket = socket;
            _remoteAddress = ((IPEndPoint)_socket.RemoteEndPoint).Address;
            _remotePort = (ushort)((IPEndPoint)_socket.RemoteEndPoint).Port;
            _receiveBuffer = new byte[ushort.MaxValue];
        }

        public virtual void Dispose()
        {
            _receiveBuffer = null;
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
                                using (var socketEventargs = new SocketAsyncEventArgs())
                {
                    socketEventargs.SetBuffer(_receiveBuffer, 0, _receiveBuffer.Length);
                    socketEventargs.Completed += (sender, args) => ReadHandlerInternal(args);
                    socketEventargs.SocketFlags = SocketFlags.None;
                    socketEventargs.RemoteEndPoint = _socket.RemoteEndPoint;

                    if (!_socket.ReceiveAsync(socketEventargs))
                        ReadHandlerInternal(socketEventargs);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException(ex, "");
            }
        }

        public delegate void SocketReadCallback(SocketAsyncEventArgs args);
        public void AsyncReadWithCallback(SocketReadCallback callback)
        {
            if (!IsOpen())
                return;

            try
            {
                                using (var socketEventargs = new SocketAsyncEventArgs())
                {
                    socketEventargs.SetBuffer(_receiveBuffer, 0, _receiveBuffer.Length);
                    socketEventargs.Completed += (sender, args) => callback(args);
                    socketEventargs.UserToken = _socket;
                    socketEventargs.SocketFlags = SocketFlags.None;
                    if (!_socket.ReceiveAsync(socketEventargs))
                        callback(socketEventargs);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException(ex, "");
            }
        }

        void ReadHandlerInternal(SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
            {
                CloseSocket();
                return;
            }

            if (args.BytesTransferred == 0)
            {
                CloseSocket();
                return;
            }

            ReadHandler(args.BytesTransferred);
        }

        public abstract void ReadHandler(int transferredBytes);

        public void SendtoSocket(byte[] data)
        {
            if (!IsOpen())
                return;
            _socket.Send(data);
        }

        public void AsyncWrite(byte[] data)
        {
            if (!IsOpen())
                return;
                        using (var socketEventargs = new SocketAsyncEventArgs())
            {
                socketEventargs.SetBuffer(data, 0, data.Length);
                socketEventargs.Completed += WriteHandlerInternal;
                socketEventargs.RemoteEndPoint = _socket.RemoteEndPoint;
                socketEventargs.UserToken = _socket;
                socketEventargs.SocketFlags = SocketFlags.None;
                _socket.SendAsync(socketEventargs);
            }
        }

        void WriteHandlerInternal(object sender, SocketAsyncEventArgs args)
        {
            args.Completed -= WriteHandlerInternal;
        }

        public void CloseSocket()
        {
            if (_socket == null)
                return;

            try
            {
                _closed = true;
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
            catch (Exception ex)
            {
                Logger.ErrorException(ex, GetRemoteIpAddress().ToString());
            }

            OnClose();
        }

        public void SetNoDelay(bool enable)
        {
            _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, enable);
        }

        public virtual void OnClose() { Dispose(); }

        public bool IsOpen() { return !_closed; }

        public byte[] GetReceiveBuffer()
        {
            return _receiveBuffer;
        }



        Socket _socket;
        byte[] _receiveBuffer;

        volatile bool _closed;

        IPAddress _remoteAddress;
        ushort _remotePort;
    }

    public interface ISocket
    {
        void Start();
        bool Update();
        bool IsOpen();
        void CloseSocket();
    }
}
