using DiIiS_NA.GameServer.ClientSystem;
using System.Text;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(Opcodes.LoadCompleteMessage)]
    public class LoadCompleteMessage : GameMessage, ISelfHandler
    {

        public void Handle(GameClient client)
        {
            client.Player.Attributes[GameAttributes.Banter_Cooldown, 0xFFFFF] = 0x000007C9;
            client.Player.Attributes[GameAttributes.Buff_Visual_Effect, 0x20CBE] = true;
            client.Player.Attributes[GameAttributes.Buff_Visual_Effect, 0x33C40] = false;
            client.Player.Attributes[GameAttributes.Immobolize] = false;
            client.Player.Attributes[GameAttributes.Untargetable] = false;
            client.Player.Attributes[GameAttributes.CantStartDisplayedPowers] = false;
            client.Player.Attributes[GameAttributes.Buff_Icon_Start_Tick0, 0x20CBE] = 0xC1;
            client.Player.Attributes[GameAttributes.Disabled] = false;
            client.Player.Attributes[GameAttributes.Hidden] = false;
            client.Player.Attributes[GameAttributes.Buff_Icon_Count0, 0x33C40] = 0;
            client.Player.Attributes[GameAttributes.Buff_Icon_End_Tick0, 0x20CBE] = 0x7C9;
            client.Player.Attributes[GameAttributes.Loading] = false;
            client.Player.Attributes[GameAttributes.Buff_Icon_End_Tick0, 0x33C40] = 0;
            client.Player.Attributes[GameAttributes.Invulnerable] = false;
            client.Player.Attributes[GameAttributes.Buff_Icon_Count0, 0x20CBE] = 1;
            client.Player.Attributes[GameAttributes.Buff_Icon_Start_Tick0, 0x33C40] = 0;

            client.Player.Attributes.BroadcastChangedIfRevealed();
        }

        public override void Parse(GameBitBuffer buffer)
        {
        }

        public override void Encode(GameBitBuffer buffer)
        {
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("LoadCompleteMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}
