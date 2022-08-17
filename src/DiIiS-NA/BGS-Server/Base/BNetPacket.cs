//Blizzless Project 2022
//Blizzless Project 2022 
using bgs.protocol;
//Blizzless Project 2022 
using System;

namespace DiIiS_NA.LoginServer.Base
{
    public class BNetPacket
    {
        private Header header;

        private Object payload;

        public BNetPacket(Header h, Object p)
        {
            header = h;
            payload = p;
        }

        public Header GetHeader()
        {
            return header;
        }

        public Object GetPayload()
        {
            return payload;
        }
    }
}
