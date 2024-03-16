using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiIiS_NA.REST.Networking
{
    public class NetworkThread<TSocketType> where TSocketType : ISocket
    {
        public void Stop()
        {
            _stopped = true;
        }

        public bool Start()
        {
            if (_thread != null)
                return false;

            _thread = new Thread(Run);
            _thread.Start();
            return true;
        }

        public void Wait()
        {
            _thread.Join();
            _thread = null;
        }

        public int GetConnectionCount()
        {
            return _connections;
        }

        public virtual void AddSocket(TSocketType sock)
        {
            Interlocked.Increment(ref _connections);
            _newSockets.Add(sock);
            SocketAdded(sock);
        }

        protected virtual void SocketAdded(TSocketType sock) { }
        protected virtual void SocketRemoved(TSocketType sock) { }

        void AddNewSockets()
        {
            foreach (var socket in _newSockets.ToArray())
            {
                if (!socket.IsOpen())
                {
                    SocketRemoved(socket);

                    Interlocked.Decrement(ref _connections);
                }
                else
                    _Sockets.Add(socket);
            }

            _newSockets.Clear();
        }

        void Run()
        {
       
            int sleepTime = 10;
            while (!_stopped)
            {
                Thread.Sleep(sleepTime);

                AddNewSockets();

                for (var i = 0; i < _Sockets.Count; ++i)
                {
                    TSocketType socket = _Sockets[i];
                    if (!socket.Update())
                    {
                        if (socket.IsOpen())
                            socket.CloseSocket();

                        SocketRemoved(socket);

                        --_connections;
                        _Sockets.Remove(socket);
                    }
                }

                uint diff = 0;
                sleepTime = (int)(diff > 10 ? 0 : 10 - diff);
            }
            _newSockets.Clear();
            _Sockets.Clear();
        }

        int _connections;
        volatile bool _stopped;

        Thread _thread;

        List<TSocketType> _Sockets = new List<TSocketType>();
        List<TSocketType> _newSockets = new List<TSocketType>();
    }
}
