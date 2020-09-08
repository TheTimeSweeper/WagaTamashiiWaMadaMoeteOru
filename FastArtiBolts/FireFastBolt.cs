using System;
using System.Reflection;
using R2API.Utils;
using RoR2;
using UnityEngine;
using FireFireBolt = EntityStates.Mage.Weapon.FireFireBolt;

namespace FastArtiBolts {
    public class FireFastBolt : FireFireBolt {

        private FastBoltsMod _fastBoltsMod {
            get {
                return FastBoltsMod.instance;
            }
        }
        private CharacterBody _seflBody {
            get {
                return outer.commonComponents.characterBody;
            }
        }
        private FireFireBolt _fireFireBolt {
            get {
                return this;
            }
        }

        public static MethodInfo fireGauntletMethod;

        private bool _subscribedToEvents;
        
        private float _fastBoltDamageMulti { get { return Utils.Cfg_DamageMulti; } }
        private float _fastBoltAttackSpeedMulti { get { return Utils.Cfg_AttackSpeedMulti; } }
        private float _firingBoltsDurationMulti { get { return Utils.Cfg_DurationMulti; } }

        private bool potato { get { return Utils.Cfg_potato; } }

        private float _origDamage;
        private float _origDuration;

        private float _fastBoltDamage;
        private float _fastBoltDuration;
        private float _firingBoltsEndTime;
        
        private float _firingBoltsTimer = 0;
        private float _pseudoFixedAge = 0;

        private int fired = 1;

        #region events

        private void SubscribeToEvents(bool subscribing) {
            if (subscribing == _subscribedToEvents)
                return;

            _subscribedToEvents = subscribing;

            if (subscribing) {
                _fastBoltsMod.baseOnEnterEvent += BaseOnEnterHook;
            } else {
                _fastBoltsMod.baseOnEnterEvent -= BaseOnEnterHook;
            }
        }

        public override void OnExit() {
            base.OnExit();
            SubscribeToEvents(false);
        }
        #endregion

        //this is called by hooking on to BaseState.OnEnter, so that FireFireBolt.OnEnter will call its base.OnEnter and this will happen right after, essentially being inserted into the FireFireBolt.OnEnter function
        public void BaseOnEnterHook() {

            #region ref
            //printed from current FlameBolt
            //maybe there's a better way to do this programmatically, but for now just manually placing the values;
            //self.projectilePrefab,        MageFireboltBasic  
            //self.muzzleflashEffectPrefab, MuzzleflashMageFire 
            //self.procCoefficient,        1 
            //self.damageCoefficient,      2.2 
            //self.force,                  300
            //self.baseDuration,           0.25
            //self.attackSoundString,      Play_mage_m1_shoot
            //self.attackSoundPitch));     1.3 
            #endregion

            base.projectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/MageFireBoltBasic");
            base.muzzleflashEffectPrefab = Resources.Load<GameObject>("prefabs/effects/resources/muzzleflashes/MuzzleFlashMageFire");

            base.procCoefficient = 1f * _fastBoltDamageMulti;
            base.damageCoefficient = 2.2f * _fastBoltDamageMulti;
            base.force = 300;
            base.baseDuration = 0.25f;
            base.attackSoundString = "Play_mage_m1_shoot";
            base.attackSoundPitch = 1.3f;

            _origDuration = base.baseDuration;
            _fastBoltDuration = _origDuration / (_seflBody.attackSpeed * _fastBoltAttackSpeedMulti);
            _firingBoltsEndTime = _origDuration * _firingBoltsDurationMulti;

            _origDamage = base.damageCoefficient;
            _fastBoltDamage = _origDamage * _fastBoltDamageMulti;
            base.damageCoefficient = _fastBoltDamage;

            _firingBoltsTimer = 0;
            _pseudoFixedAge = 0;

            fired = 1;
        }

        //this is called on the highest(lowest?) override level of OnEnter, so that the code I need is called absolutely before and after the entire function runs
        public override void OnEnter() {

            SubscribeToEvents(true);

            base.OnEnter();
            
            _fireFireBolt.SetFieldValue<float>("duration", _origDuration);
        }
        

        public override void FixedUpdate() {
            base.FixedUpdate();

            _firingBoltsTimer += Time.fixedDeltaTime;
            _pseudoFixedAge += Time.fixedDeltaTime;

            if (_firingBoltsTimer > _fastBoltDuration && _pseudoFixedAge < _firingBoltsEndTime) {

                if (!potato) {

                    while (_firingBoltsTimer > _fastBoltDuration) {
                        _firingBoltsTimer -= _fastBoltDuration;

                        PseudoFireGauntlet();
                    }
                } else {

                    int multi = 0;
                    while (_firingBoltsTimer > _fastBoltDuration) {
                        _firingBoltsTimer -= _fastBoltDuration;

                        multi++;
                    }

                    base.damageCoefficient = _fastBoltDamage * multi;
                    base.procCoefficient = Mathf.Min(base.procCoefficient * multi, 1);

                    PseudoFireGauntlet();
                }
            }
        }

        private void PseudoFireGauntlet() {

            _fireFireBolt.SetFieldValue("hasFiredGauntlet", false);

            if (fireGauntletMethod == null) {
                fireGauntletMethod = typeof(FireFireBolt).GetMethod("FireGauntlet", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            fireGauntletMethod.Invoke(_fireFireBolt, null);
            //_fireFireBolt.InvokeMethod("FireGauntlet");

            fired++;
        }
    }
}
