using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions
{
    //[471947] [Actor] P6_Necro_Revive_Golem
    //[471619] [Actor] p6_ConsumeFleshGolem
    //[471647] [Actor] p6_IceGolem
    //[465239] [Actor] p6_BoneGolem
    //[471646] [Actor] p6_DecayGolem
    //[460042] [Actor] p6_BloodGolem
    public class BaseGolem : Minion
    {
        public BaseGolem(World world, Actor master)
            : base(world, ActorSno._p6_necro_revive_golem, master, null)
        {
            Scale = 1.35f;
            //TODO: get a proper value for this.
            WalkSpeed *= 3;
            SetBrain(new MinionBrain(this));
            //TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
            Attributes[GameAttributes.Hitpoints_Max] = 3000f * (master as Player).Attributes[GameAttributes.Hitpoints_Max];
            Attributes[GameAttributes.Hitpoints_Cur] = 3000f * (master as Player).Attributes[GameAttributes.Hitpoints_Max];
            Attributes[GameAttributes.Attacks_Per_Second] = 1.0f;

            Attributes[GameAttributes.Damage_Weapon_Min, 0] = 5f * (master as Player).Attributes[GameAttributes.Damage_Weapon_Min, 0];
            Attributes[GameAttributes.Damage_Weapon_Delta, 0] = 3 * master.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0] * (master as Player).Toon.Level;

            
        }
    }
    public class ConsumeFleshGolem : Minion
    {
        public ConsumeFleshGolem(World world, Actor master)
            : base(world, ActorSno._p6_consumefleshgolem, master, null)
        {
            Scale = 1.35f;
            //TODO: get a proper value for this.
            WalkSpeed *= 3;
            SetBrain(new MinionBrain(this));
            //TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
            Attributes[GameAttributes.Hitpoints_Max] = 3000f * (master as Player).Attributes[GameAttributes.Hitpoints_Max];
            Attributes[GameAttributes.Hitpoints_Cur] = 3000f * (master as Player).Attributes[GameAttributes.Hitpoints_Max];
            Attributes[GameAttributes.Attacks_Per_Second] = 1.0f;

            Attributes[GameAttributes.Damage_Weapon_Min, 0] = 5f * (master as Player).Toon.Level;
            Attributes[GameAttributes.Damage_Weapon_Delta, 0] = 3 * master.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0] * (master as Player).Toon.Level;
            //SNOSummons

        }
    }
    public class IceGolem : Minion
    {
        public IceGolem(World world, Actor master)
            : base(world, ActorSno._p6_icegolem, master, null)
        {
            Scale = 1.35f;
            //TODO: get a proper value for this.
            WalkSpeed *= 3;
            SetBrain(new MinionBrain(this));
            //TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
            Attributes[GameAttributes.Hitpoints_Max] = 3000f * (master as Player).Attributes[GameAttributes.Hitpoints_Max];
            Attributes[GameAttributes.Hitpoints_Cur] = 3000f * (master as Player).Attributes[GameAttributes.Hitpoints_Max];
            Attributes[GameAttributes.Attacks_Per_Second] = 1.0f;

            Attributes[GameAttributes.Damage_Weapon_Min, 0] = 5f * (master as Player).Toon.Level;
            Attributes[GameAttributes.Damage_Weapon_Delta, 0] = 3 * master.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0] * (master as Player).Toon.Level;


        }
    }
    public class BoneGolem : Minion
    {
        public BoneGolem(World world, Actor master)
            : base(world, ActorSno._p6_bonegolem, master, null)
        {
            Scale = 1.35f;
            //TODO: get a proper value for this.
            WalkSpeed *= 3;
            SetBrain(new MinionBrain(this));
            //TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
            Attributes[GameAttributes.Hitpoints_Max] = 3000f * (master as Player).Attributes[GameAttributes.Hitpoints_Max];
            Attributes[GameAttributes.Hitpoints_Cur] = 3000f * (master as Player).Attributes[GameAttributes.Hitpoints_Max];
            Attributes[GameAttributes.Attacks_Per_Second] = 1.0f;

            Attributes[GameAttributes.Damage_Weapon_Min, 0] = 5f * (master as Player).Toon.Level;
            Attributes[GameAttributes.Damage_Weapon_Delta, 0] = 3 * master.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0] * (master as Player).Toon.Level;

        }
    }
    public class DecayGolem : Minion
    {
        public DecayGolem(World world, Actor master)
            : base(world, ActorSno._p6_decaygolem, master, null)
        {
            Scale = 1.35f;
            //TODO: get a proper value for this.
            WalkSpeed *= 3;
            SetBrain(new MinionBrain(this));
            //TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
            Attributes[GameAttributes.Hitpoints_Max] = 3000f * (master as Player).Attributes[GameAttributes.Hitpoints_Max];
            Attributes[GameAttributes.Hitpoints_Cur] = 3000f * (master as Player).Attributes[GameAttributes.Hitpoints_Max];
            Attributes[GameAttributes.Attacks_Per_Second] = 1.0f;

            Attributes[GameAttributes.Damage_Weapon_Min, 0] = 5f * (master as Player).Toon.Level;
            Attributes[GameAttributes.Damage_Weapon_Delta, 0] = 3 * master.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0] * (master as Player).Toon.Level;

            
        }
    }
    public class BloodGolem : Minion
    {
        public BloodGolem(World world, Actor master)
            : base(world, ActorSno._p6_bloodgolem, master, null)
        {
            Scale = 1.35f;
            //TODO: get a proper value for this.
            WalkSpeed *= 3;
            SetBrain(new MinionBrain(this));
            //TODO: These values should most likely scale, but we don't know how yet, so just temporary values.
            Attributes[GameAttributes.Hitpoints_Max] = 3000f * (master as Player).Attributes[GameAttributes.Hitpoints_Max];
            Attributes[GameAttributes.Hitpoints_Cur] = 3000f * (master as Player).Attributes[GameAttributes.Hitpoints_Max];
            Attributes[GameAttributes.Attacks_Per_Second] = 1.0f;

            Attributes[GameAttributes.Damage_Weapon_Min, 0] = 5f * (master as Player).Toon.Level;
            Attributes[GameAttributes.Damage_Weapon_Delta, 0] = 3 * master.Attributes[GameAttributes.Damage_Weapon_Delta_Total, 0] * (master as Player).Toon.Level;

           
        }
    }
}
