using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using FireFireBolt = EntityStates.Mage.Weapon.FireFireBolt;

namespace FastArtiBolts {

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.FastArtiBolts", "Fast Artificer Bolts", "0.1.0")]
    public class FastBoltsMod : BaseUnityPlugin {

        public delegate void BaseOnEnterEvent(CharacterBody body, FireFireBolt self);
        public delegate void FireFireBoltOnEnterEvent(FireFireBolt self);
        public delegate void FireFireBoltOnExitEvent(FireFireBolt self);

        public BaseOnEnterEvent baseOnEnterEvent;
        public FireFireBoltOnEnterEvent boltOnEnterEvent;
        public FireFireBoltOnExitEvent boltOnExitEvent;

        private List<FastArtiBoltsHandler> _boltHandlers = new List<FastArtiBoltsHandler>();

        public void Awake() {

            On.EntityStates.BaseState.OnEnter += BaseState_OnEnter;
            On.EntityStates.Mage.Weapon.FireFireBolt.OnEnter += FireFireBolt_OnEnter;
            On.EntityStates.Mage.Weapon.FireFireBolt.FireGauntlet += FireFireBolt_FireGauntlet;
            On.EntityStates.Mage.Weapon.FireFireBolt.FixedUpdate += FireFireBolt_FixedUpdate;
            On.EntityStates.Mage.Weapon.FireFireBolt.OnExit += FireFireBolt_OnExit;
        }

        private void BaseState_OnEnter(On.EntityStates.BaseState.orig_OnEnter orig, EntityStates.BaseState self) {
            
            orig(self);

            if (!(self is EntityStates.Mage.Weapon.FireFireBolt)) 
                return;
            
            CharacterBody selfBody = self.outer.commonComponents.characterBody;
            if (selfBody.GetComponent<FastArtiBoltsHandler>() == null) {

                FastArtiBoltsHandler handler = selfBody.gameObject.AddComponent<FastArtiBoltsHandler>();
                _boltHandlers.Add(handler);
                handler.init(selfBody);
            }

            baseOnEnterEvent?.Invoke(selfBody, (FireFireBolt)self);
        }

        private void FireFireBolt_OnEnter(On.EntityStates.Mage.Weapon.FireFireBolt.orig_OnEnter orig, EntityStates.Mage.Weapon.FireFireBolt self) {
            
            orig(self);

            boltOnEnterEvent?.Invoke(self);
        }

        private void FireFireBolt_FireGauntlet(On.EntityStates.Mage.Weapon.FireFireBolt.orig_FireGauntlet orig, EntityStates.Mage.Weapon.FireFireBolt self) {
            
            orig(self);
            fired++;
        }

        private void FireFireBolt_FixedUpdate(On.EntityStates.Mage.Weapon.FireFireBolt.orig_FixedUpdate orig, EntityStates.Mage.Weapon.FireFireBolt self) {

            orig(self);

            //_firingBoltsTimer += Time.fixedDeltaTime;
            //_pseudoFixedAge += Time.fixedDeltaTime;

            //if (_firingBoltsTimer > _fastBoltDuration && _pseudoFixedAge < _firingBoltsEndTime) {

            //    if (!potato) {

            //        while (_firingBoltsTimer > _fastBoltDuration) {
            //            _firingBoltsTimer -= _fastBoltDuration;

            //            PseudoFireGauntlet(self);
            //        }
            //    } else {

            //        int multi = 0;
            //        while (_firingBoltsTimer > _fastBoltDuration) {
            //            _firingBoltsTimer -= _fastBoltDuration;

            //            multi++;
            //        }

            //        self.damageCoefficient = _fastBoltDamage * multi;
            //        self.procCoefficient = Mathf.Min(self.procCoefficient *multi, 1);

            //        PseudoFireGauntlet(self);
            //    }
            //}
        }

        private static void PseudoFireGauntlet(EntityStates.Mage.Weapon.FireFireBolt self) {
            self.SetFieldValue("hasFiredGauntlet", false);
            self.InvokeMethod("FireGauntlet");
        }

        private void FireFireBolt_OnExit(On.EntityStates.Mage.Weapon.FireFireBolt.orig_OnExit orig, EntityStates.Mage.Weapon.FireFireBolt self) {
            orig(self);

            boltOnExitEvent?.Invoke(self);
        }

        void Update() {

            //if (Input.GetKeyDown(KeyCode.I)) {
            //    _fastBoltDamageDivi += 0.5f;
            //    Chat.AddMessage($"set _fastBoltDamageDivi: {_fastBoltDamageDivi}");
            //}
            //if (Input.GetKeyDown(KeyCode.K)) {
            //    _fastBoltDamageDivi -= 0.5f;
            //    Chat.AddMessage($"set _fastBoltDamageDivi: {_fastBoltDamageDivi}");
            //}

            //if (Input.GetKeyDown(KeyCode.O)) {
            //    _fastBoltAttackSpeedMulti += 0.5f;

            //    Chat.AddMessage($"set _fastBoltAttackSpeedMulti: {_fastBoltAttackSpeedMulti}");
            //}
            //if (Input.GetKeyDown(KeyCode.L)) {
            //    _fastBoltAttackSpeedMulti -= 0.5f;
            //    Chat.AddMessage($"set _fastBoltAttackSpeedMulti: {_fastBoltAttackSpeedMulti}");
            //}

            //if (Input.GetKeyDown(KeyCode.P)) {
            //    _firingBoltsDurationMulti += 0.05f;
            //    Chat.AddMessage($"set _fastBoltsDurationMulti: {_firingBoltsDurationMulti}");
            //}
            //if (Input.GetKeyDown(KeyCode.Semicolon)) {
            //    _firingBoltsDurationMulti -= 0.05f;
            //    Chat.AddMessage($"set _fastBoltsDurationMulti: {_firingBoltsDurationMulti}");
            //}

            //if (Input.GetKeyDown(KeyCode.LeftBracket)) {
            //    potato = !potato;
            //    Chat.AddMessage($"setting potato: {potato}");
            //}
        }
    }
}
