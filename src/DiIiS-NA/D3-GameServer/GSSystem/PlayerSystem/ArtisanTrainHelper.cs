using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.D3_GameServer.GSSystem.ActorSystem.Implementations.Artisans;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiIiS_NA.D3_GameServer.GSSystem.PlayerSystem
{
    internal class ArtisanTrainHelper
    {
        private const int maxLevel = 12;
        private static readonly ArtisanType[] canBeTrained = new[] { ArtisanType.Blacksmith, ArtisanType.Jeweler, ArtisanType.Mystic };
        private static readonly Dictionary<ArtisanType, string> recipeTemplates = new()
        {
            [ArtisanType.Blacksmith] = "BlackSmith_Train_Level{0}",
            [ArtisanType.Jeweler] = "Jeweler_Train_Level{0}",
            [ArtisanType.Mystic] = "Mystic_Train_Level{0}"
        };
        private static readonly Dictionary<ArtisanType, long[]> achievements = new()
        {
            [ArtisanType.Blacksmith] = new[] { 74987243307767, 74987243307768, 74987243307769, 74987251817289 },
            [ArtisanType.Jeweler] = new[] { 74987243307781, 74987243307782, 74987243307783, 74987257153995 },
            [ArtisanType.Mystic] = new[] { 74987253584575, 74987256660015, 74987248802163, 74987251397159 }
        };
        private static readonly Dictionary<ArtisanType, long> criteriaForLevel10 = new()
        {
            [ArtisanType.Blacksmith] = 74987249071497,
            [ArtisanType.Jeweler] = 74987245845978,
            [ArtisanType.Mystic] = 74987259424359
        };
        private static readonly int[] animationTags = new[] {
            0x00011500,
            0x00011510,
            0x00011520,
            0x00011530,
            0x00011540,
            0x00011550,
            0x00011560,
            0x00011570,
            0x00011580,
            0x00011590,
            // fixme no animation
            0x00011600,
            // fixme no animation
            0x00011610,
        };
        private static readonly int[] idleAnimationTags = new[] {
            0x00011210,
            0x00011220,
            0x00011230,
            0x00011240,
            0x00011250,
            0x00011260,
            0x00011270,
            0x00011280,
            0x00011290,
            0x00011300,
            // fixme no animation
            0x00011310,
            // fixme no animation
            0x00011320
        };


        private readonly ArtisanType artisanType;
        internal ArtisanTrainHelper(DBCraft dBCraft, ArtisanType type)
        {
            if (!canBeTrained.Contains(type))
                throw new ArgumentException("Unsupported artisan type", nameof(type));
            DbRef = dBCraft ?? throw new ArgumentNullException(nameof(dBCraft));
            artisanType = type;
        }
        internal DBCraft DbRef { get; }

        internal string TrainRecipeName => string.Format(recipeTemplates[artisanType], Math.Min(DbRef.Level, maxLevel - 1));

        internal bool HasMaxLevel => DbRef.Level >= maxLevel;

        internal ulong? Achievement => DbRef.Level switch
        {
            // index by level: 2 -> 0, 5 -> 1, 10 -> 2, 12 -> 3
            2 or 5 or 10 or 12 => (ulong)achievements[artisanType][DbRef.Level / 4],
            _ => null,
        };

        internal ulong? Criteria => DbRef.Level == 10 ? (ulong)criteriaForLevel10[artisanType] : null;

        internal int AnimationTag => animationTags[DbRef.Level - 1];
        internal int IdleAnimationTag => idleAnimationTags[DbRef.Level - 1];

        internal int Type => Array.IndexOf(canBeTrained, artisanType);
    }
}
