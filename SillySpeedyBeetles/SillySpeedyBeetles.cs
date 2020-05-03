using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;

namespace SillySpeedyBeetles {

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimesweeper.SillySpeedyBeetles", "SpeedyBeetlesMutiny", "1.0.6")]
    public class SillySpeedyBeetles : BaseUnityPlugin {

        private GameObject _beetlePrefab;
        public ConfigEntry<float> SetMoveSpeed;
        public ConfigEntry<float> MultAttackSpeed;
        public ConfigEntry<float> MultDamage;

        public void Awake() {

            config();
            
            _beetlePrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/BeetleBody");
            CharacterBody beetleBody = _beetlePrefab.GetComponent<CharacterBody>();

            Debug.Log($"setting movespeed from {beetleBody.baseMoveSpeed} to {SetMoveSpeed.Value}");

            beetleBody.baseMoveSpeed = SetMoveSpeed.Value;
            beetleBody.baseAttackSpeed *= MultAttackSpeed.Value;
            beetleBody.baseDamage *= MultDamage.Value;
        }

        private void config() {

            const string sectionName = "may the day treat you well";

            ConfigDefinition moveSpeedDef = new ConfigDefinition(sectionName, "Set Move Speed");
            ConfigDescription moveSpeedDesc = new ConfigDescription("Move Speed to Set (base is 6)");

            SetMoveSpeed = Config.Bind<float>(moveSpeedDef, 9.5f, moveSpeedDesc);

            ConfigDefinition attackSpeedDef = new ConfigDefinition(sectionName, "Attack Speed Multiplier");
            ConfigDescription attackSpeedDesc = new ConfigDescription("Multiplier to set to Attack Speed");

            MultAttackSpeed = Config.Bind<float>(attackSpeedDef, 2f, attackSpeedDesc);

            ConfigDefinition damageDef = new ConfigDefinition(sectionName, "Damage Multipier");
            ConfigDescription damageDesc = new ConfigDescription("Multiplier to set to Damage");

            MultDamage = Config.Bind<float>(damageDef, 0.8f, damageDesc);
        }
    }
}
