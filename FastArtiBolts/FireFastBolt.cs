using System;
using System.Reflection;
using RoR2;
using R2API.Utils;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using FireFireBolt = EntityStates.Mage.Weapon.FireFireBolt;

namespace FastArtiBolts {
    public class FireFastBolt : FireFireBolt, SteppedSkillDef.IStepSetter {

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
        private float _originShift = 0.12f;
        private float _originShiftMax = 0.4f;
        private float _originShiftForward = 0.2f;

        private Gauntlet _jauntlet;

        public new void SetStep(int i) {
            base.SetStep(i);
            _jauntlet = (FireFireBolt.Gauntlet)i;
        }

        public override void OnEnter() {

            #region ref
            //printed from current FlameBolt
            //more properly grab flame bolt's SerializedFields from (i assume) its scriptableobject;
            //plugging in the values for now

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
            base.muzzleflashEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashMageFire");

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

            _fireFireBolt.duration = base.baseDuration;
        }
         

        public override void FixedUpdate() {
            base.FixedUpdate();

            _firingBoltsTimer += Time.fixedDeltaTime;
            _pseudoFixedAge += Time.fixedDeltaTime;

            if (_firingBoltsTimer > _fastBoltDuration && _pseudoFixedAge < _firingBoltsEndTime) {

                if (!potato) {

                    Vector3 aimRight = -Vector3.Cross(base.GetAimRay().direction, Vector3.up);
                    int sameFrameBolts = 1;
                    while (_firingBoltsTimer > _fastBoltDuration) {

                        _firingBoltsTimer -= _fastBoltDuration;

                        float fuckinMath = doFuckinMathOffset(_boltsFired);
                        Vector3 fuckinForwardSimple = base.GetAimRay().direction.normalized * _originShiftForward * _fastBoltDuration * sameFrameBolts * 80;
                        
                        _aimOrigin.position += aimRight.normalized * fuckinMath - fuckinForwardSimple;

                        PseudoFireGauntlet();
                        sameFrameBolts++;

                        _aimOrigin.localPosition = _origOrigin;
                    }

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

        //makes the position pingpong back and forth from a left and right threshold
        private float doFuckinMathOffset(int boltsFired) {

            int jauntletMult = _jauntlet == Gauntlet.Right ? 1 : -1;

            float fuckinMath = _originShift * boltsFired * jauntletMult;

            while (Mathf.Clamp(fuckinMath, -_originShiftMax, _originShiftMax) != fuckinMath) {

                if (fuckinMath >= _originShiftMax) {

                    fuckinMath = 2 * _originShiftMax - fuckinMath;
                }

                if (fuckinMath <= -_originShiftMax) {

                    fuckinMath = -2 * _originShiftMax - fuckinMath;
                }
            }

            fuckinMath += 0.1f * jauntletMult;

            return fuckinMath;
        }

        private void PseudoFireGauntlet() {

            _fireFireBolt.hasFiredGauntlet = false;

            _fireFireBolt.FireGauntlet();

            _boltsFired++;
        }

        public override void OnExit() {
            base.OnExit();
            _aimOrigin.localPosition = _origOrigin;
        }

        public override void OnSerialize(NetworkWriter writer) {
            base.OnSerialize(writer);
            writer.Write((byte)_jauntlet);
        }

        public override void OnDeserialize(NetworkReader reader) {
            base.OnDeserialize(reader);
            _jauntlet = (FireFireBolt.Gauntlet)reader.ReadByte();
        }
    }
}
