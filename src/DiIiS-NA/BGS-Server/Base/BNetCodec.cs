using bgs.protocol;
using DotNetty.Codecs;
using DotNetty.Buffers;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using Google.ProtocolBuffers;

namespace DiIiS_NA.LoginServer.Base
{
    public class BNetCodec : MessageToMessageCodec<BinaryWebSocketFrame, BNetPacket>
    {
        protected override void Decode(IChannelHandlerContext ctx, BinaryWebSocketFrame msg, List<object> output)
        {
            if (msg.Content.ReadableBytes < 2)
            {
                return;
            }

            int headerSize = msg.Content.ReadUnsignedShort();
            if (msg.Content.ReadableBytes < headerSize)
            {
                return;
            }
            byte[] headerBuf = new byte[headerSize];
            msg.Content.ReadBytes(headerBuf);
            Header header = Header.ParseFrom(headerBuf);

            int payloadSize = header.HasSize ? (int)header.Size : msg.Content.ReadableBytes;
            if (msg.Content.ReadableBytes < payloadSize)
            {
                return;
            }
            byte[] payload = new byte[payloadSize];
            msg.Content.ReadBytes(payload);

            output.Add(new BNetPacket(header, payload));
        }

        protected override void Encode(IChannelHandlerContext ctx, BNetPacket msg, List<object> output)
        {
            Header header = msg.GetHeader();
            var payload = msg.GetPayload();
            int headerSize = header.SerializedSize;
            int payloadSize = 0;
            if (payload != null)
                payloadSize = (payload as IMessage).SerializedSize;

            IByteBuffer packet = DotNetty.Buffers.Unpooled.Buffer(2 + headerSize + payloadSize);

            packet.WriteShort(headerSize);
            packet.WriteBytes(header.ToByteArray());
            if (payload != null)
                packet.WriteBytes((payload as IMessage).ToByteArray());


            output.Add(new BinaryWebSocketFrame(packet));
            //throw new NotImplementedException();
        }

    }
}
