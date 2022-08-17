//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base
{
    [Message(new[] { //Opcodes.GenericBlobMessage, //Opcodes.GenericBlobMessage2, //Opcodes.GenericBlobMessage3,
        Opcodes.TieredRiftRewardMessage, Opcodes.CraftingDataBlacksmith
                     , Opcodes.CraftingDataJeweler, Opcodes.CraftingDataMystic, Opcodes.CraftingDataHoradrim, Opcodes.CraftingDataTest, Opcodes.CraftingDataTransmog
                     , Opcodes.CraftingDataBlacksmithInitialMessage, Opcodes.CraftingDataJewelerInitialMessage, Opcodes.CraftingDataMysticInitialMessage
                     , Opcodes.CraftingDataHoradrimInitialMessage, Opcodes.CraftingDataTestInitialMessage
                     , Opcodes.CraftingDataTransmogInitialMessage, Opcodes.CurrencyDataFull, Opcodes.CurrencyDataPartial, Opcodes.ExtractedLegendariedDataFull, Opcodes.ExtractedLegendariedDataPartial
                     , Opcodes.RiftStartPreloadingLeaderboardMessage, Opcodes.UnlockedDyesRequestMessage, Opcodes.UnlockedDyesResultsMessage //Opcodes.MailContentsMessage
                     , Opcodes.TutorialStateMessage 
                     , Opcodes.ConsoleSetCameraDefaults, Opcodes.ConsoleValidateAccount, Opcodes.ConsoleSyncHeroTimePlayed
                     , Opcodes.AchievementsGetSnapshot, Opcodes.AchievementsSnapshot, Opcodes.AchievementAwarded, Opcodes.CosmeticItemsBlobMessage, Opcodes.RiftEndScreenAddPaticipantsBlobMessage
                     , Opcodes.RiftEndScreenInfoBlobMessage, Opcodes.GenericBlobMessage37, Opcodes.GenericBlobMessage38 })]
    public class GenericBlobMessage : GameMessage
    {
        public byte[] Data;
        public Opcodes SelectOpcode;

        public GenericBlobMessage(Opcodes opcode) : base(opcode) { SelectOpcode = opcode; }

        public override void Parse(GameBitBuffer buffer)
        {
            Data = buffer.ReadBlob(32);

        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBlob(32, Data);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("GenericBlobMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Length: 0x" + Data.Length.ToString("X8") + " (" + Data.Length + ")");
            if (SelectOpcode == Opcodes.CurrencyDataFull)
            {
                var BlobData = D3.Items.CurrencySavedData.ParseFrom(Data);
            }
            else if (SelectOpcode == Opcodes.RiftEndScreenInfoBlobMessage)
            {
                var BlobData = D3.GameMessage.RiftEndScreenInfo.ParseFrom(Data);
            }
            /*for (int i = 0; i < Data.Length; i += 16)
            {

                b.Append(' ', pad);
                b.Append(i.ToString("X4"));
                b.Append(' ');

                int off = i;
                for (int j = 0; j < 8; j++, off++)
                {
                    if (off < Data.Length)
                    {
                        b.Append(Data[off].ToString("X2"));
                        b.Append(' ');
                    }
                    else
                    {
                        b.Append(' '); b.Append(' '); b.Append(' ');
                    }
                }
                b.Append(' ');
                off = i + 8;
                for (int j = 0; j < 8; j++, off++)
                {
                    if (off < Data.Length)
                    {
                        b.Append(Data[off].ToString("X2"));
                        b.Append(' ');
                    }
                    else
                    {
                        b.Append(' '); b.Append(' '); b.Append(' ');
                    }
                }

                b.Append(' ');

                off = i;
                for (int j = 0; j < 8; j++, off++)
                {
                    if (off < Data.Length)
                    {
                        if (Data[off] >= 20 && Data[off] < 128)
                            b.Append((char)Data[off]);
                        else
                            b.Append('.');
                    }
                    else
                        b.Append(' ');
                }
                b.Append(' ');
                off = i + 8;
                for (int j = 0; j < 8; j++, off++)
                {
                    if (off < Data.Length)
                    {
                        if (Data[off] >= 20 && Data[off] < 128)
                            b.Append((char)Data[off]);
                        else
                            b.Append('.');
                    }
                    else
                        b.Append(' ');
                }
                b.AppendLine();
            }*/
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}
