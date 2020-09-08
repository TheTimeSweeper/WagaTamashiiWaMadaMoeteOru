using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace SillySpeedyBeetles {

    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheDuckDuckGreySweeper.SillySpeedyBeetles", "SpeedyBeetlesMutiny", "2.0.2")]
    public class SillySpeedyBeetles : BaseUnityPlugin {

        public ConfigEntry<float> SetMoveSpeed;
        public ConfigEntry<float> MultAttackSpeed;
        public ConfigEntry<float> SetLevelAttackSpeed;
        public ConfigEntry<float> MultDamage;

        private GameObject _beetlePrefab;

        private float _origBaseMoveSpeed;

        public void Start() {
            
            _beetlePrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/BeetleBody");
            CharacterBody beetleBody = _beetlePrefab.GetComponent<CharacterBody>();

            _origBaseMoveSpeed = beetleBody.baseMoveSpeed;

            config();

            beetleBody.baseMoveSpeed = SetMoveSpeed.Value;
            beetleBody.baseAttackSpeed *= MultAttackSpeed.Value;
            beetleBody.levelAttackSpeed = SetLevelAttackSpeed.Value;
            beetleBody.baseDamage *= MultDamage.Value;

            string log = "Beetles are now Speedy:";
            log += $"\n - Movespeed: {beetleBody.baseMoveSpeed}";
            log += $"\n - Attack Speed: {beetleBody.baseAttackSpeed}";
            log += $"\n - Attack Speed Per Level: {beetleBody.levelAttackSpeed}";
            log += $"\n - Damage: {beetleBody.baseDamage}";

            Logger.LogInfo(log);
        }

        private void config() {

            const string sectionName = "may the day treat you well";

            ConfigDefinition moveSpeedDef = new ConfigDefinition(sectionName, "Set Move Speed");
            ConfigDescription moveSpeedDesc = new ConfigDescription($"Set Beetle Move Speed to this amount (base is {_origBaseMoveSpeed})");

            SetMoveSpeed = Config.Bind<float>(moveSpeedDef, 9.5f, moveSpeedDesc);

            ConfigDefinition attackSpeedDef = new ConfigDefinition(sectionName, "Attack Speed Multiplier");
            ConfigDescription attackSpeedDesc = new ConfigDescription("Multiply Beetle Attack Speed by this amount");

            MultAttackSpeed = Config.Bind<float>(attackSpeedDef, 2f, attackSpeedDesc);

            ConfigDefinition levelAttackSpeedDef = new ConfigDefinition(sectionName, "Set Level Attack speed");
            ConfigDescription levelAttackSpeedDesc = new ConfigDescription("Set Beetle Attack speed gained per level");

            SetLevelAttackSpeed = Config.Bind<float>(levelAttackSpeedDef, 0.1f, levelAttackSpeedDesc);

            ConfigDefinition damageDef = new ConfigDefinition(sectionName, "Damage Multipier");
            ConfigDescription damageDesc = new ConfigDescription("Multiply Beetle Damage by this amount");

            MultDamage = Config.Bind<float>(damageDef, 0.8f, damageDesc);
        }
    }
}
