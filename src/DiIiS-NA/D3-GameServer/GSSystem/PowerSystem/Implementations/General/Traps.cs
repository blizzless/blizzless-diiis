//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
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

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
    
    [ImplementsPowerSNO(102874)] //{ActivationPower = [Power] 102874 - caOut_Oasis_Attack_Plant_attack}
    public class PlantAttack : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            var Targets = User.GetActorsInRange(20f);
            foreach (var target in Targets)
            {
                WeaponDamage(Target, 50f, DamageType.Poison);
            }
            yield break;
        }
    }

    [ImplementsPowerSNO(153000)] //{[153000] [Power] A2_Evacuation_BelialBomb
    public class BelialBomb : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            var Targets = User.GetActorsInRange(20f);
            TickTimer waitForImpact = WaitSeconds(1.3f - 0.1f);
            SpawnEffect(161822, this.User.Position, 0, WaitSeconds(5f)); //[161822] [Actor] Belial_GroundBomb_Pending
            yield return waitForImpact;
            SpawnEffect(185108, this.User.Position, 0, WaitSeconds(3f)); //[185108] [Actor] Belial_GroundMeteor
            yield return waitForImpact;
            var Fire = SpawnEffect(161833, this.User.Position, 0, WaitSeconds(3f));
            Fire.UpdateDelay = 1f;
            Fire.OnUpdate = () =>
            {
                AttackPayload Attack = new AttackPayload(this);
                Attack.Targets = GetEnemiesInRadius(this.User.Position, 15f);
                Attack.AddWeaponDamage(10f, DamageType.Poison);
                Attack.OnHit = hitPayload =>
                {
                    //                    hitPayload.Target.Kill
                    if (hitPayload.Target is ActorSystem.Monster)
                    {
                        if (!hitPayload.Target.Dead)
                        {
                            var deathload = new DeathPayload(hitPayload.Context, hitPayload.DominantDamageType, hitPayload.Target, hitPayload.Target.HasLoot);
                            deathload.AutomaticHitEffects = hitPayload.AutomaticHitEffects;

                            if (deathload.Successful)
                            {
                                hitPayload.Target.Dead = true;
                                try
                                {
                                    if (hitPayload.OnDeath != null && hitPayload.AutomaticHitEffects)
                                        hitPayload.OnDeath(deathload);
                                }
                                catch { }
                                deathload.Apply();
                            }
                        }
                    }
                };
                Attack.Apply();
            };
            yield break;
        }
    }

    [ImplementsPowerSNO(186216)] //trDun_Cath_WallCollapse_Damage
    public class WallCollapse : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            //WeaponDamage(GetEnemiesInRadius(User.Position, 20f), 1.0f, DamageType.Physical);
            var Targets = User.GetActorsInRange(20f);
            foreach (var target in Targets)
            {
                AddBuff(target, new DebuffStunned(WaitSeconds(2f)));
                WeaponDamage(Target, 20f, DamageType.Physical);
            }
            yield break;
        }
    }

    [ImplementsPowerSNO(30209)] //DestructableObjectChandelier_AOE
    public class CathendralTrap : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            var Targets = User.GetActorsInRange(20f);
            foreach (var target in Targets)
            {
                AddBuff(target, new DebuffStunned(WaitSeconds(2f)));
                WeaponDamage(Target, 20f, DamageType.Physical);
            }
            yield break;
        }
    }
    //223284
    [ImplementsPowerSNO(223284)] //a4dun_spire_firewall
    public class a4dun_spire_firewall : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            yield return WaitSeconds(8f);
            bool inCombat = false;
            if (this.World.GetActorBySNO(3526) != null)
                inCombat = true;

            if (inCombat == false)
            {
                (this.User as ActorSystem.Monster).Brain.DeActivate();
                yield break;
            }
            AddBuff(User, new Burn());
            var PowerData = (DiIiS_NA.Core.MPQ.FileFormats.Power)MPQStorage.Data.Assets[SNOGroup.Power][this.PowerSNO].Data;

            //yield return WaitSeconds(18f);
            //yield break;
        }
        [ImplementsPowerBuff(0)]
        public class Burn : PowerBuff
        {
            const float _damageRate = 1f;
            TickTimer _damageTimer = null;
            List<ActorSystem.Actor> Targets = null;
            EffectActor proxy = null;

            public override void Init()
            {
                Timeout = WaitSeconds(5f);

                proxy = SpawnProxy(User.Position, WaitSeconds(8.5f));
                proxy.PlayEffectGroup(224754);
                //*/
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                return true;
            }

            public override void OnPayload(Payload payload)
            {
                if (payload.Target == Target && payload is HitPayload)
                {

                }
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;

                Targets = proxy.GetActorsInRange(18f);
                foreach (var target in Targets)
                    if (target is PlayerSystem.Player || target is ActorSystem.Monster)
                    {
                        this.User.Attributes[GameAttribute.Damage_Min] = target.Attributes[GameAttribute.Hitpoints_Max] / 20f;
                        WeaponDamage(target, 1.5f, DamageType.Fire); 
                    }
                //*/
                return false;
            }

            public override void Remove()
            {
                base.Remove();
                if (proxy != null)
                    proxy.Destroy();
            }
        }

        [ImplementsPowerBuff(1)]
        public class fst : PowerBuff
        {
            const float _damageRate = 1f;
            TickTimer _damageTimer = null;
            List<ActorSystem.Actor> Targets = null;
            EffectActor proxy = null;

            public override void Init()
            {
                Timeout = WaitSeconds(5f);

            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                return true;
            }

            public override void OnPayload(Payload payload)
            {
                if (payload.Target == Target && payload is HitPayload)
                {

                }
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;

                return false;
            }

            public override void Remove()
            {
                base.Remove();

            }
        }
        [ImplementsPowerBuff(2)]
        public class sec : PowerBuff
        {
            const float _damageRate = 1f;
            TickTimer _damageTimer = null;
            List<ActorSystem.Actor> Targets = null;
            EffectActor proxy = null;

            public override void Init()
            {
                Timeout = WaitSeconds(5f);

            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                return true;
            }

            public override void OnPayload(Payload payload)
            {
                if (payload.Target == Target && payload is HitPayload)
                {

                }
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;

                return false;
            }

            public override void Remove()
            {
                base.Remove();

            }
        }
        [ImplementsPowerBuff(3)]
        public class thd : PowerBuff
        {
            const float _damageRate = 1f;
            TickTimer _damageTimer = null;
            List<ActorSystem.Actor> Targets = null;
            EffectActor proxy = null;

            public override void Init()
            {
                Timeout = WaitSeconds(5f);

            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                return true;
            }

            public override void OnPayload(Payload payload)
            {
                if (payload.Target == Target && payload is HitPayload)
                {

                }
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;

                return false;
            }

            public override void Remove()
            {
                base.Remove();

            }
        }
    }

    [ImplementsPowerSNO(96925)] //Butcher_FloorPanelFire
    public class Butcher_FloorPanelFire : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            yield return WaitSeconds(8f);
            bool inCombat = false;
            if(this.World.GetActorBySNO(3526) != null)
                inCombat = true;
            
            if (inCombat == false)
            {
                (this.User as ActorSystem.Monster).Brain.DeActivate();
                yield break;
            }
            AddBuff(User, new Burn());
            var PowerData = (DiIiS_NA.Core.MPQ.FileFormats.Power)MPQStorage.Data.Assets[SNOGroup.Power][this.PowerSNO].Data;

            //yield return WaitSeconds(18f);
            //yield break;
        }
        [ImplementsPowerBuff(0)]
        public class Burn : PowerBuff
        {
            const float _damageRate = 1f;
            TickTimer _damageTimer = null;
            List<ActorSystem.Actor> Targets = null;
            EffectActor proxy = null;

            public override void Init()
            {
                Timeout = WaitSeconds(5f);
                
                proxy = SpawnProxy(User.Position, WaitSeconds(8.5f));
                #region Выборка анимации
                int EffectOfActivate = 0;
                switch (this.User.ActorSNO.Id)
                {
                    case 201423: EffectOfActivate = 201257; break;//[201423] [Actor] ButcherLair_FloorPanel_UpperLeft_Base
                    case 201438: EffectOfActivate = 201444; break;//[201438][Actor] ButcherLair_FloorPanel_UpperMid_Base
                    case 201454: EffectOfActivate = 201459; break;//[201454][Actor] ButcherLair_FloorPanel_UpperRight_Base

                    case 201426: EffectOfActivate = 201432; break;//[201426][Actor] ButcherLair_FloorPanel_MidMiddle_Base

                    case 201242: EffectOfActivate = 201247; break;//[201242][Actor] ButcherLair_FloorPanel_LowerLeft_Base
                    case 200969: EffectOfActivate = 201011; break;//[200969][Actor] ButcherLair_FloorPanel_LowerMid_Base
                    case 201464: EffectOfActivate = 201471; break;//[201464][Actor] ButcherLair_FloorPanel_LowerRight_Base
                }
                #endregion
                proxy.PlayEffectGroup(EffectOfActivate);
                //*/
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                return true;
            }

            public override void OnPayload(Payload payload)
            {
                if (payload.Target == Target && payload is HitPayload)
                {

                }
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;
                
                Targets = proxy.GetActorsInRange(18f);
                foreach (var target in Targets)
                    if (target is PlayerSystem.Player)
                        WeaponDamage(target, 1.5f, DamageType.Fire);
                //*/
                return false;
            }

            public override void Remove()
            {
                base.Remove();
                if (proxy != null)
                    proxy.Destroy();
            }
        }

        [ImplementsPowerBuff(1)]
        public class fst : PowerBuff
        {
            const float _damageRate = 1f;
            TickTimer _damageTimer = null;
            List<ActorSystem.Actor> Targets = null;
            EffectActor proxy = null;

            public override void Init()
            {
                Timeout = WaitSeconds(5f);

            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                return true;
            }

            public override void OnPayload(Payload payload)
            {
                if (payload.Target == Target && payload is HitPayload)
                {

                }
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;

                return false;
            }

            public override void Remove()
            {
                base.Remove();

            }
        }
        [ImplementsPowerBuff(2)]
        public class sec : PowerBuff
        {
            const float _damageRate = 1f;
            TickTimer _damageTimer = null;
            List<ActorSystem.Actor> Targets = null;
            EffectActor proxy = null;

            public override void Init()
            {
                Timeout = WaitSeconds(5f);
                
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                return true;
            }

            public override void OnPayload(Payload payload)
            {
                if (payload.Target == Target && payload is HitPayload)
                {

                }
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;
                
                return false;
            }

            public override void Remove()
            {
                base.Remove();

            }
        }
        [ImplementsPowerBuff(3)]
        public class thd : PowerBuff
        {
            const float _damageRate = 1f;
            TickTimer _damageTimer = null;
            List<ActorSystem.Actor> Targets = null;
            EffectActor proxy = null;

            public override void Init()
            {
                Timeout = WaitSeconds(5f);

            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                return true;
            }

            public override void OnPayload(Payload payload)
            {
                if (payload.Target == Target && payload is HitPayload)
                {

                }
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;

                return false;
            }

            public override void Remove()
            {
                base.Remove();

            }
        }
    }

    [ImplementsPowerSNO(108017)] //a1dun_leor_BigFireGrate_Fire
    public class a1dun_leor_BigFireGrate_Fire : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            AddBuff(User, new Burn());
            yield return WaitSeconds(18f);
            //yield break;
        }
        [ImplementsPowerBuff(0)]
        public class Burn : PowerBuff
        {
            const float _damageRate = 1f;
            TickTimer _damageTimer = null;
            List<ActorSystem.Actor> Targets = null;
            EffectActor proxy = null;

            public override void Init()
            {
                Timeout = WaitSeconds(8f);
                proxy = SpawnProxy(User.Position, WaitSeconds(8f));
                
                proxy.PlayEffectGroup(108018);
            }

            public override bool Apply()
            {
                if (!base.Apply())
                    return false;

                return true;
            }

            public override void OnPayload(Payload payload)
            {
                if (payload.Target == Target && payload is HitPayload)
                {

                }
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;

                Targets = proxy.GetActorsInRange(18f);
                foreach (var target in Targets)
                    if (target is PlayerSystem.Player)
                        WeaponDamage(target, 1.5f, DamageType.Fire);

                return false;
            }

            public override void Remove()
            {
                base.Remove();

            }
        }
    }
}
