//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Threading;

namespace DiIiS_NA.GameServer.ClientSystem.Base
{
    public class NetworkThread
    {
        public static readonly DateTime ApplicationStartTime = DateTime.Now;
        protected static readonly Logger Logger = LogManager.CreateLogger("NT");
        int _connections;
        volatile bool _stopped;

        Thread _thread;

        List<Connection> _Sockets = new List<Connection>();
        List<Connection> _newSockets = new List<Connection>();

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

        public virtual void AddSocket(Connection sock)
        {
            Interlocked.Increment(ref _connections);
            _newSockets.Add(sock);
        }

        void AddNewSockets()
        {
            try
            {
                if (_newSockets.Count() == 0)
                    return;

                lock (_newSockets)
                {
                    foreach (var socket in _newSockets)
                    {
                        if (!socket.IsOpen())
                        {
                            Interlocked.Decrement(ref _connections);
                        }
                        else
                            _Sockets.Add(socket);
                    }

                    _newSockets.Clear();
                }
            }
            catch { }
        }

        void Run()
        {
            //Logger.Info("Network Thread Starting");

            int sleepTime = 10;
            while (!_stopped)
            {
                Thread.Sleep(sleepTime);

                uint tickStart = (uint)(DateTime.Now - ApplicationStartTime).TotalMilliseconds;

                try
                {
                    AddNewSockets();

                    for (var i = _Sockets.Count; i > 0; i--)
                    {
                        Connection socket = _Sockets[i - 1];
                        if (!socket.IsOpen())
                        {
                            try
                            {
                                socket.Close();

                                _connections--;
                                _Sockets.Remove(socket);
                            }
                            catch { }
                        }
                    }
                }
                catch { }

                uint diff = (uint)(DateTime.Now - ApplicationStartTime).TotalMilliseconds - tickStart;
                sleepTime = (int)(diff > 10 ? 1 : 10 - diff);
            }

            Logger.Warn("Network Thread exits");
            _newSockets.Clear();
            _Sockets.Clear();
        }

    }
}
