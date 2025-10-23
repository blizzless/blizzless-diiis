using D3.GameMessage;
using System.Text;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(Opcodes.PlayerBannerMessage)]
    public class PlayerBannerMessage : GameMessage
    {
        public PlayerBanner PlayerBanner;

        public PlayerBannerMessage() : base(Opcodes.PlayerBannerMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            PlayerBanner = PlayerBanner.ParseFrom(buffer.ReadBlob(32));
        }
        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBlob(32, PlayerBanner.ToByteArray());
        }
        public override void AsText(StringBuilder b, int pad)
        {
            b.AppendLine("PlayerBannerMessage:");
            b.AppendLine(PlayerBanner.ToString());
        }
    }
}
