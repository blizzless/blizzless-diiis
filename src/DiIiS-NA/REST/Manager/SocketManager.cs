//Blizzless Project 2022 
using DiIiS_NA.REST.Networking;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Net.Sockets;

namespace DiIiS_NA.REST.Manager
{
    public class SocketManager<TSocketType> where TSocketType : ISocket
    {
        public virtual bool StartNetwork(string bindIp, int port, int threadCount = 1)
        {
            Cypher.Assert(threadCount > 0);

            Acceptor = new AsyncAcceptor();
            if (!Acceptor.Start(bindIp, port))
            {
                return false;
            }

            _threadCount = threadCount;
            _threads = new NetworkThread<TSocketType>[GetNetworkThreadCount()];

            for (int i = 0; i < _threadCount; ++i)
            {
                _threads[i] = new NetworkThread<TSocketType>();
                _threads[i].Start();
            }

            Acceptor.AsyncAcceptSocket(OnSocketOpen);

            return true;
        }

        public virtual void StopNetwork()
        {
            Acceptor.Close();

            if (_threadCount != 0)
                for (int i = 0; i < _threadCount; ++i)
                    _threads[i].Stop();

            Wait();

            Acceptor = null;
            _threads = null;
            _threadCount = 0;
        }

        void Wait()
        {
            if (_threadCount != 0)
                for (int i = 0; i < _threadCount; ++i)
                    _threads[i].Wait();
        }

        public virtual void OnSocketOpen(Socket sock)
        {
            try
            {
                TSocketType newSocket = (TSocketType)Activator.CreateInstance(typeof(TSocketType), sock);
                newSocket.Start();

                _threads[SelectThreadWithMinConnections()].AddSocket(newSocket);
            }
            catch (Exception err)
            {
      
            }
        }

        public int GetNetworkThreadCount() { return _threadCount; }

        uint SelectThreadWithMinConnections()
        {
            uint min = 0;

            for (uint i = 1; i < _threadCount; ++i)
                if (_threads[i].GetConnectionCount() < _threads[min].GetConnectionCount())
                    min = i;

            return min;
        }

        public AsyncAcceptor Acceptor;
        NetworkThread<TSocketType>[] _threads;
        int _threadCount;
    }
}
