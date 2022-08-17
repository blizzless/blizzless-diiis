//Blizzless Project 2022
//Blizzless Project 2022 
using bgs.protocol;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Battle;
//Blizzless Project 2022 
using Google.ProtocolBuffers;
//Blizzless Project 2022 
using System;

namespace DiIiS_NA.LoginServer.Base
{
    public class HandlerController : IRpcController
    {
        public BattleClient Client { get; set; }
        public Header LastCallHeader { get; set; }
        public uint Status { get; set; }
        public ulong ListenerId { get; set; }

        public string ErrorText { get; }

        public bool Failed { get; }

        public bool IsCanceled()
        {
            return false;
        }

        public void Reset()
        {

        }

        public void StartCancel()
        {

        }

        public void SetFailed(string reason)
        {

        }

        public void NotifyOnCancel(Action<object> callback)
        {

        }
    }
}
