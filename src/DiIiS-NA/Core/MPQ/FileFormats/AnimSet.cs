//Blizzless Project 2022
using CrystalMpq;
using DiIiS_NA.GameServer.Core.Types.SNO;
using Gibbed.IO;
using System.Collections.Generic;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using System.Linq;
using System;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.AnimSet)]
    public class AnimSet : FileFormat
    {
        private static readonly AnimationTags[] deathTags = new AnimationTags[]
        {
            AnimationTags.DeathArcane,
            AnimationTags.DeathFire,
            AnimationTags.DeathLightning,
            AnimationTags.DeathPoison,
            AnimationTags.DeathPlague,
            AnimationTags.DeathDismember,
            AnimationTags.DeathDefault,
            AnimationTags.DeathPulverise,
            AnimationTags.DeathCold,
            AnimationTags.DeathLava,
            AnimationTags.DeathHoly,
            AnimationTags.DeathSpirit,
            AnimationTags.DeathFlyingOrDefault
        };
        public Header Header { get; private set; }
        public int SNOParentAnimSet { get; private set; }
        public TagMap TagMapAnimDefault { get; private set; }
        public TagMap[] AnimSetTagMaps;


        private Dictionary<int, AnimationSno> _animations;
        public Dictionary<int, AnimationSno> Animations
        {
            get
            {
                return _animations ??= InitAnimations();
            }
        }

        private Dictionary<int, AnimationSno> InitAnimations()
        {
            var defaultAnimations = TagMapAnimDefault.TagMapEntries.ToDictionary(x => x.TagID, x => (AnimationSno)x.Int);

            //not sure how better to do this, cant load parents anims on init as they may not be loaded first. - DarkLotus
            if (SNOParentAnimSet != -1)
            {
                var ani = (AnimSet)MPQStorage.Data.Assets[SNOGroup.AnimSet][SNOParentAnimSet].Data;
                return defaultAnimations.Union(ani.Animations.Where(x => !defaultAnimations.ContainsKey(x.Key))).ToDictionary(x => x.Key, x => x.Value);
            }
            return defaultAnimations;
        }

        public AnimSet(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);
            this.SNOParentAnimSet = stream.ReadValueS32();
            TagMapAnimDefault = stream.ReadSerializedItem<TagMap>();
            stream.Position += 8;
            AnimSetTagMaps = new TagMap[28];
            for (int i = 0; i < AnimSetTagMaps.Length; i++)
            {
                AnimSetTagMaps[i] = stream.ReadSerializedItem<TagMap>();
                stream.Position += 8;
            }

            stream.Close();
        }

        public AnimationSno GetAniSNO(AnimationTags type)
        {
            if (Animations.Keys.Contains((int)type))
            {
                return Animations[(int)type];
            }
            return AnimationSno._NONE;
        }
        public bool TagExists(AnimationTags type)
        {
            return Animations.Keys.Contains((int)type);
        }
        public int GetAnimationTag(AnimationTags type)
        {
            if (Animations.Keys.Contains((int)type))
            {
                return (int)type;
            }
            return -1;
        }
        public AnimationSno GetRandomDeath()
        {
            if (!TagExists(AnimationTags.DeathDefault))
            {
                return AnimationSno._NONE;
            }
            return deathTags.Select(x => GetAniSNO(x)).Where(x => x != AnimationSno._NONE).OrderBy(x => RandomHelper.Next()).First();
        }
    }
    public enum AnimationTags
    {
        GenericCast = 262144,
        Idle2 = 69632,
        Idle = 69968,
        Spawn = 70097,

        KnockBackLand = 71176,
        KnockBackMegaOuttro = 71218,
        KnockBack = 71168,
        KnockBackMegaIntro = 71216,
        RangedAttack = 69840,
        Stunned = 69648,
        GetHit = 69664,
        Dead1 = 79168,
        Dead2 = 79152,
        Dead3 = 77920,
        Dead4 = 77888,
        Dead5 = 77904,
        Dead6 = 77872,
        Dead7 = 77856,
        Dead8 = 77840,
        SpecialDead = 71440,
        Run = 69728,
        Walk = 69744,
        Attack = 69776,
        Attack2 = 69792,
        SpecialAttack = 69904,
        DeathArcane = 73776,
        DeathFire = 73744,
        DeathLightning = 73760,
        DeathPoison = 73792,
        DeathPlague = 73856,
        DeathDismember = 73872,
        DeathDefault = 69712,
        DeathPulverise = 73824,
        DeathCold = 74016,
        DeathLava = 74032,
        DeathHoly = 74048,
        DeathSpirit = 74064,
        DeathFlyingOrDefault = 71424
    }
}
