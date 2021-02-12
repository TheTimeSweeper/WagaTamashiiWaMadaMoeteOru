using System;
using System.Reflection;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using FireFireBolt = EntityStates.Mage.Weapon.FireFireBolt;

namespace FastArtiBolts {
    public class FireFastBolt : FireFireBolt {

        public static MethodInfo fireGauntletMethod;

        private float _fastBoltDamageMulti { get { return Utils.Cfg_DamageMulti; } }
        private float _fastBoltProcMulti { get { return Utils.Cfg_ProcMulti; } }
        private float _fastBoltAttackSpeedMulti { get { return Utils.Cfg_AttackSpeedMulti; } }
        private float _firingBoltsDurationMulti { get { return Utils.Cfg_DurationMulti; } }

        private bool potato { get { return Utils.Cfg_potato; } }

        private FireFireBolt _fireFireBolt {
            get { return this; }
        }

        private float _fastBoltDuration;
        private float _firingBoltsEndTime;
        
        private float _firingBoltsTimer = 0;
        private float _pseudoFixedAge = 0;

        private int _boltsFired = 1;

        private Transform _aimOrigin;
        private Vector3 _origOrigin;
        public static float _originShift = 0.3f;

        private Gauntlet _jauntlet;

        public override void OnEnter() {

            #region ref
            //printed from current FlameBolt
            //more properly grab flame bolt's SerializedFields from (i assume) its scriptableobject;
            //plugging in the values for now
            //bless you for making these public

            //self.projectilePrefab,        MageFireboltBasic  
            //self.muzzleflashEffectPrefab, MuzzleflashMageFire 
            //self.procCoefficient,         1 
            //self.damageCoefficient,       2.2 
            //self.force,                   300
            //self.baseDuration,            0.25
            //self.attackSoundString,       Play_mage_m1_shoot
            //self.attackSoundPitch         1.3 
            #endregion

            base.projectilePrefab = FastBoltsMod.fastFireBoltPrefab;
            base.muzzleflashEffectPrefab = Resources.Load<GameObject>("prefabs/effects/resources/muzzleflashes/MuzzleFlashMageFire");

            base.damageCoefficient = 2.2f * _fastBoltDamageMulti;
            base.procCoefficient = 1 * _fastBoltProcMulti;
            base.force = 300;
            base.baseDuration = 0.25f;
            base.attackSoundString = "Play_mage_m1_shoot";
            base.attackSoundPitch = 1.3f;

            _fastBoltDuration = base.baseDuration / (base.characterBody.attackSpeed * _fastBoltAttackSpeedMulti);
            _firingBoltsEndTime = base.baseDuration * _firingBoltsDurationMulti;

            _firingBoltsTimer = 0;
            _pseudoFixedAge = 0;

            _aimOrigin = base.characterBody.aimOriginTransform;
            _origOrigin = _aimOrigin.localPosition;

            base.OnEnter();

            //one bolt is fired OnEnter
            _boltsFired = 1;

            _fireFireBolt.SetFieldValue<float>("duration", base.baseDuration);

            _originShift *= _jauntlet == Gauntlet.Right ? 1 : -1;
        }
        

        public override void FixedUpdate() {
            base.FixedUpdate();

            _firingBoltsTimer += Time.fixedDeltaTime;
            _pseudoFixedAge += Time.fixedDeltaTime;

            if (_firingBoltsTimer > _fastBoltDuration && _pseudoFixedAge < _firingBoltsEndTime) {

                if (!potato) {
                    float sameFrameBolts = 0;
                    while (_firingBoltsTimer > _fastBoltDuration) {
                        _firingBoltsTimer -= _fastBoltDuration;

                        Vector3 aimRight = -Vector3.Cross(base.GetAimRay().direction, Vector3.up);
                        _aimOrigin.position += aimRight.normalized * sameFrameBolts * _originShift;

                        PseudoFireGauntlet();

                        sameFrameBolts++;
                        _aimOrigin.localPosition = _origOrigin;
                    }

                    _aimOrigin.localPosition = _origOrigin;
                } else {

                    int multi = 0;
                    while (_firingBoltsTimer > _fastBoltDuration) {
                        _firingBoltsTimer -= _fastBoltDuration;

                        multi++;
                    }

                    base.damageCoefficient = base.damageCoefficient * multi;
                    base.procCoefficient = base.procCoefficient * multi;

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

            _boltsFired++;
        }

        public override void OnExit() {
            base.OnExit();
            _aimOrigin.localPosition = _origOrigin;
        }

        public override void OnSerialize(NetworkWriter writer) {
            base.OnSerialize(writer);
            writer.Write((byte)this._jauntlet);
        }

        // Token: 0x0600393A RID: 14650 RVA: 0x000E9E6F File Offset: 0x000E806F
        public override void OnDeserialize(NetworkReader reader) {
            base.OnDeserialize(reader);
            this._jauntlet = (FireFireBolt.Gauntlet)reader.ReadByte();
        }
    }
}
