using R2API.Utils;
using RoR2;
using UnityEngine;
using FireFireBolt = EntityStates.Mage.Weapon.FireFireBolt;

namespace FastArtiBolts {

    public class FastArtiBoltsHandler : MonoBehaviour {

        private FastBoltsMod _fastBoltsMod;

        private bool _subscribedToEvents;

        private CharacterBody _baseCharacterbody;
        private EntityStates.Mage.Weapon.FireFireBolt _selfBolt;

        private float _origDamage;
        private float _origDuration;
        private float _origProc;

        private float _fastBoltDamage;
        private float _fastBoltDuration;
        private float _firingBoltsEndTime;

        private float _fastBoltDamageDivi = 2f;
        private float _fastBoltAttackSpeedMulti = 3f;
        private float _firingBoltsDurationMulti = 0.51f;

        private float _firingBoltsTimer = 0;
        private float _pseudoFixedAge = 0;

        private bool potato;

        private int fired = 1;

        public void init(CharacterBody body) {
            _baseCharacterbody = body;

            //punch that like button to events
            SubscribeToEvents(true);
        }
        #region events
        void OnDestroy() {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool subscribing) {
            if (subscribing == _subscribedToEvents)
                return;

            _subscribedToEvents = subscribing;

            if (subscribing) {
                _fastBoltsMod.baseOnEnterEvent += BaseState_OnEnter;
                _fastBoltsMod.boltOnEnterEvent += FireFireBolt_OnEnter
            } else {
            }

        }
        #endregion

        private void BaseState_OnEnter(CharacterBody body, FireFireBolt self) {
            if (body != _baseCharacterbody)
                return;

            _selfBolt = self;

            _firingBoltsTimer = 0;
            _pseudoFixedAge = 0;

            fired = 0;

            fired++;

            _origDuration = _selfBolt.baseDuration;
            _fastBoltDuration = _origDuration / (_baseCharacterbody.attackSpeed * _fastBoltAttackSpeedMulti);
            _firingBoltsEndTime = _origDuration * _firingBoltsDurationMulti;

            _origDamage = _selfBolt.damageCoefficient;
            _fastBoltDamage = _origDamage / _fastBoltDamageDivi;
            _selfBolt.damageCoefficient = _fastBoltDamage;

            _selfBolt.procCoefficient /= _fastBoltDamageDivi;
        }

        private void FireFireBolt_OnEnter(FireFireBolt self) {
            if (self != _selfBolt)
                return;

            _selfBolt.SetFieldValue<float>("duration", _origDuration);
        }

        private void FixedUpdate() {
            if (this._selfBolt == null)
                return;


            _firingBoltsTimer += Time.fixedDeltaTime;
            _pseudoFixedAge += Time.fixedDeltaTime;

            if (_firingBoltsTimer > _fastBoltDuration && _pseudoFixedAge < _firingBoltsEndTime) {

                if (!potato) {

                    while (_firingBoltsTimer > _fastBoltDuration) {
                        _firingBoltsTimer -= _fastBoltDuration;

                        PseudoFireGauntlet(_selfBolt);
                    }
                } else {

                    int multi = 0;
                    while (_firingBoltsTimer > _fastBoltDuration) {
                        _firingBoltsTimer -= _fastBoltDuration;

                        multi++;
                    }

                    _selfBolt.damageCoefficient = _fastBoltDamage * multi;
                    _selfBolt.procCoefficient = Mathf.Min(_selfBolt.procCoefficient * multi, 1);

                    PseudoFireGauntlet(_selfBolt);
                }

            }
        }

        private void PseudoFireGauntlet(EntityStates.Mage.Weapon.FireFireBolt self) {
            self.SetFieldValue("hasFiredGauntlet", false);
            self.InvokeMethod("FireGauntlet");
            fired++;
        }


        private void FireFireBolt_OnExit(EntityStates.Mage.Weapon.FireFireBolt self) {
            if (self != _selfBolt)
                return;

            Chat.AddMessage($"i'ma firin mah {fired} layzors");
            _selfBolt = null;
        }
    }
}
