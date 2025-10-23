using Google.ProtocolBuffers;
using System;

namespace DiIiS_NA.LoginServer.Base
{
    public class RPCCallBack
    {
        public Action<IMessage> Action { get; private set; }
        public IBuilder Builder { get; private set; }

        public RPCCallBack(Action<IMessage> action, IBuilder builder)
        {
            this.Action = action;
            this.Builder = builder;
        }
    }
}
