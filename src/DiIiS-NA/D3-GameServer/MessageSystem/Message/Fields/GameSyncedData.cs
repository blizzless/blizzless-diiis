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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class GameSyncedData
    {
        public int GameSyncedFlags;
        public int Act;
        public int InitialMonsterLevel;
        public int MonsterLevel;
        public int RandomWeatherSeed;
        public int OpenWorldMode;
        public int OpenWorldModeAct;
        public int OpenWorldModeParam;
        public int OpenWorldTransitionTime;
        public int OpenWorldDefaultAct;
        public int OpenWorldBonusAct;
        public int SNODungeonFinderLevelArea;
        public int LootRunOpen;
        public int OpenLootRunLevel;
        public int LootRunBossDead;
        public int SetDungeonActive;
        public int HunterPlayerIdx;
        public int LootRunBossActive;
        public int TieredLootRunFailed;
        public int LootRunChallengeCompleted;
        public int PregameEnd;
        public int RoundStart;
        public int RoundEnd;
        public int Pregame;
        public int PVPGameOver;
        public int field_v273;
        // MaxLength = 2
        public int[] TeamWins;
        // MaxLength = 2
        public int[] TeamScore;
        public int[] PVPGameResult;
        public long PartyGuideHeroId;
        // MaxLength = 4
        public long[] TiredRiftPaticipatingHeroID;

        public void Parse(GameBitBuffer buffer)
        {
            GameSyncedFlags = buffer.ReadInt(4);
            Act = buffer.ReadInt(32);
            InitialMonsterLevel = buffer.ReadInt(32);
            MonsterLevel = buffer.ReadInt(32);
            RandomWeatherSeed = buffer.ReadInt(32);
            OpenWorldMode = buffer.ReadInt(32);
            OpenWorldModeAct = buffer.ReadInt(32);
            OpenWorldModeParam = buffer.ReadInt(32);
            OpenWorldTransitionTime = buffer.ReadInt(32);
            OpenWorldDefaultAct = buffer.ReadInt(32);
            OpenWorldBonusAct = buffer.ReadInt(32);
            SNODungeonFinderLevelArea = buffer.ReadInt(32);
            LootRunOpen = buffer.ReadInt(32);
            OpenLootRunLevel = buffer.ReadInt(32);
            LootRunBossDead = buffer.ReadInt(32);
            HunterPlayerIdx = buffer.ReadInt(32);
            LootRunBossActive = buffer.ReadInt(32);
            TieredLootRunFailed = buffer.ReadInt(32);
            LootRunChallengeCompleted = buffer.ReadInt(32);
            SetDungeonActive = buffer.ReadInt(32);
            Pregame = buffer.ReadInt(32);
            PregameEnd = buffer.ReadInt(32);
            RoundStart = buffer.ReadInt(32);
            RoundEnd = buffer.ReadInt(32);
            PVPGameOver = buffer.ReadInt(32);
            field_v273 = buffer.ReadInt(32);
            TeamWins = new int[2];
            for (int i = 0; i < TeamWins.Length; i++) TeamWins[i] = buffer.ReadInt(32);
            TeamScore = new int[2];
            for (int i = 0; i < TeamScore.Length; i++) TeamScore[i] = buffer.ReadInt(32);
            PVPGameResult = new int[2];
            for (int i = 0; i < PVPGameResult.Length; i++) PVPGameResult[i] = buffer.ReadInt(32);
            PartyGuideHeroId = buffer.ReadInt64(64);
            TiredRiftPaticipatingHeroID = new long[4];
            for (int i = 0; i < TiredRiftPaticipatingHeroID.Length; i++) TiredRiftPaticipatingHeroID[i] = buffer.ReadInt64(64);

        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(4, GameSyncedFlags);
            buffer.WriteInt(32, Act);
            buffer.WriteInt(32, InitialMonsterLevel);
            buffer.WriteInt(32, MonsterLevel);
            buffer.WriteInt(32, RandomWeatherSeed);
            buffer.WriteInt(32, OpenWorldMode);
            buffer.WriteInt(32, OpenWorldModeAct);
            buffer.WriteInt(32, OpenWorldModeParam);
            buffer.WriteInt(32, OpenWorldTransitionTime);
            buffer.WriteInt(32, OpenWorldDefaultAct);
            buffer.WriteInt(32, OpenWorldBonusAct);
            buffer.WriteInt(32, SNODungeonFinderLevelArea);
            buffer.WriteInt(32, LootRunOpen);
            buffer.WriteInt(32, OpenLootRunLevel);
            buffer.WriteInt(32, LootRunBossDead);
            buffer.WriteInt(32, HunterPlayerIdx);
            buffer.WriteInt(32, LootRunBossActive);
            buffer.WriteInt(32, TieredLootRunFailed);
            buffer.WriteInt(32, LootRunChallengeCompleted);
            buffer.WriteInt(32, SetDungeonActive);
            buffer.WriteInt(32, Pregame);
            buffer.WriteInt(32, PregameEnd);
            buffer.WriteInt(32, RoundStart);
            buffer.WriteInt(32, RoundEnd);
            buffer.WriteInt(32, PVPGameOver);
            buffer.WriteInt(32, field_v273);
            for (int i = 0; i < TeamWins.Length; i++) buffer.WriteInt(32, TeamWins[i]);
            for (int i = 0; i < TeamScore.Length; i++) buffer.WriteInt(32, TeamScore[i]);
            for (int i = 0; i < PVPGameResult.Length; i++) buffer.WriteInt(32, PVPGameResult[i]);
            buffer.WriteInt64(64, PartyGuideHeroId);
            for (int i = 0; i < TiredRiftPaticipatingHeroID.Length; i++) buffer.WriteInt64(64, TiredRiftPaticipatingHeroID[i]);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("GameSyncedData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("GameSyncedFlags: 0x" + GameSyncedFlags.ToString("X8") + " (" + GameSyncedFlags + ")");
            b.Append(' ', pad);
            b.AppendLine("InitialMonsterLevel: 0x" + InitialMonsterLevel.ToString("X8") + " (" + InitialMonsterLevel + ")");
            b.Append(' ', pad);
            b.AppendLine("Act: 0x" + Act);
            b.Append(' ', pad);
            b.AppendLine("MonsterLevel: 0x" + MonsterLevel.ToString("X8") + " (" + MonsterLevel + ")");
            b.Append(' ', pad);
            b.AppendLine("RandomWeatherSeed: 0x" + RandomWeatherSeed.ToString("X8") + " (" + RandomWeatherSeed + ")");
            b.Append(' ', pad);
            b.AppendLine("OpenWorldMode: 0x" + OpenWorldMode.ToString("X8") + " (" + OpenWorldMode + ")");
            b.Append(' ', pad);
            b.AppendLine("OpenWorldModeAct: 0x" + OpenWorldModeAct.ToString("X8") + " (" + OpenWorldModeAct + ")");
            b.Append(' ', pad);
            b.AppendLine("OpenWorldModeParam: 0x" + OpenWorldModeParam.ToString("X8") + " (" + OpenWorldModeParam + ")");
            b.Append(' ', pad);
            b.AppendLine("OpenWorldTransitionTime: 0x" + OpenWorldTransitionTime.ToString("X8") + " (" + OpenWorldTransitionTime + ")");
            b.Append(' ', pad);
            b.AppendLine("OpenWorldDefaultAct: 0x" + OpenWorldDefaultAct.ToString("X8") + " (" + OpenWorldDefaultAct + ")");
            b.Append(' ', pad);
            b.AppendLine("OpenWorldBonusAct: 0x" + OpenWorldBonusAct.ToString("X8") + " (" + OpenWorldBonusAct + ")");
            b.Append(' ', pad);
            b.AppendLine("SNODungeonFinderLevelArea: 0x" + SNODungeonFinderLevelArea.ToString("X8") + " (" + SNODungeonFinderLevelArea + ")");
            b.Append(' ', pad);
            b.AppendLine("LootRunOpen: 0x" + LootRunOpen.ToString("X8") + " (" + LootRunOpen + ")");
            b.Append(' ', pad);
            b.AppendLine("OpenLootRunLevel: 0x" + OpenLootRunLevel.ToString("X8") + " (" + OpenLootRunLevel + ")");
            b.Append(' ', pad);
            b.AppendLine("LootRunBossDead: 0x" + LootRunBossDead.ToString("X8") + " (" + LootRunBossDead + ")");
            b.Append(' ', pad);
            b.AppendLine("SetDungeonActive: 0x" + SetDungeonActive.ToString("X8") + " (" + SetDungeonActive + ")");
            b.Append(' ', pad);
            b.AppendLine("HunterPlayerIdx: 0x" + HunterPlayerIdx.ToString("X8") + " (" + HunterPlayerIdx + ")");
            b.Append(' ', pad);
            b.AppendLine("LootRunBossActive: 0x" + LootRunBossActive.ToString("X8") + " (" + LootRunBossActive + ")");
            b.Append(' ', pad);
            b.AppendLine("TieredLootRunFailed: 0x" + TieredLootRunFailed.ToString("X8") + " (" + TieredLootRunFailed + ")");
            b.Append(' ', pad);
            b.AppendLine("LootRunChallengeCompleted: 0x" + LootRunChallengeCompleted.ToString("X8") + " (" + LootRunChallengeCompleted + ")");
            b.Append(' ', pad);
            b.AppendLine("PregameEnd: 0x" + PregameEnd.ToString("X8") + " (" + PregameEnd + ")");
            b.Append(' ', pad);
            b.AppendLine("RoundStart: 0x" + RoundStart.ToString("X8") + " (" + RoundStart + ")");
            b.Append(' ', pad);
            b.AppendLine("RoundEnd: 0x" + RoundEnd.ToString("X8") + " (" + RoundEnd + ")");
            b.Append(' ', pad);
            b.AppendLine("Pregame: 0x" + Pregame.ToString("X8") + " (" + Pregame + ")");
            b.Append(' ', pad);
            b.AppendLine("PVPGameOver: 0x" + PVPGameOver.ToString("X8") + " (" + PVPGameOver + ")");
            b.Append(' ', pad);
            b.AppendLine("PartyGuideHeroId: 0x" + PartyGuideHeroId.ToString("X8") + " (" + PartyGuideHeroId + ")");
            b.Append(' ', pad);
            
            b.Append(' ', pad);
            b.AppendLine("TeamWins:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < TeamWins.Length;)
            {
                b.Append(' ', pad + 1);
                for (int j = 0; j < 8 && i < TeamWins.Length; j++, i++)
                {
                    b.Append("0x" + TeamWins[i].ToString("X8") + ", ");
                }
                b.AppendLine();
            }
            b.Append(' ', pad);
            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', pad);
            b.AppendLine("TeamScore:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < TeamScore.Length;)
            {
                b.Append(' ', pad + 1);
                for (int j = 0; j < 8 && i < TeamScore.Length; j++, i++)
                {
                    b.Append("0x" + TeamScore[i].ToString("X8") + ", ");
                }
                b.AppendLine();
            }
            b.Append(' ', pad);
            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', pad);
            b.AppendLine("PVPGameResult:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < PVPGameResult.Length;)
            {
                b.Append(' ', pad + 1);
                for (int j = 0; j < 8 && i < PVPGameResult.Length; j++, i++)
                {
                    b.Append("0x" + PVPGameResult[i].ToString("X8") + ", ");
                }
                b.AppendLine();
            }
            b.Append(' ', pad);
            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', pad);
            b.AppendLine("PartyGuideHeroId: 0x" + PartyGuideHeroId.ToString("X8") + " (" + PartyGuideHeroId + ")");
            b.Append(' ', pad);
            b.AppendLine("TiredRiftPaticipatingHeroID:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < TiredRiftPaticipatingHeroID.Length;)
            {
                b.Append(' ', pad + 1);
                for (int j = 0; j < 8 && i < TiredRiftPaticipatingHeroID.Length; j++, i++)
                {
                    b.Append("0x" + TiredRiftPaticipatingHeroID[i].ToString("X16") + ", ");
                }
                b.AppendLine();
            }
            b.Append(' ', pad);
            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }

    }
}
