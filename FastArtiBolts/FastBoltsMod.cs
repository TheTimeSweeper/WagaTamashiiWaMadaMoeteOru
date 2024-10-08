﻿using BepInEx;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using UnityEngine;
using System.Security;
using System.Security.Permissions;
using R2API;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace FastArtiBolts {

    [BepInPlugin(MODUID, "Fast Artificer Bolts", "2.2.2")]
    public class FastBoltsMod : BaseUnityPlugin {

        public static FastBoltsMod instance;
        public const string MODUID = "com.TheTimeSweeper.FastArtiBolts";

        public delegate void BaseOnEnterEvent();

        public BaseOnEnterEvent baseOnEnterEvent;

        void Awake() {
            instance = this; 

            Utils.PopulateAssets();

            Utils.InitConfig(Config);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.cwmlolzlz.skills")) {
                SkillsPlusCompat.Init();
            }

            createFastBoltProjectile();

            AddNewFireboltSkill();

            new Modules.ContentPacks().Initialize();
        }

        private void createFastBoltProjectile() {

            GameObject fastFireBoltPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageFireboltBasic"), "FastFireBolt", true);
            GameObject fastBoltGhost = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageFireboltBasic").GetComponent<ProjectileController>().ghostPrefab, "FastFireBoltGhost", false);
            fastBoltGhost.transform.localScale = new Vector3(0.72f, 0.72f, 1f);
            fastBoltGhost.transform.Find("Spinner").localScale = Vector3.one * 0.87f;

            var rends = fastBoltGhost.GetComponentsInChildren<TrailRenderer>();
            for (int i = 0; i < rends.Length; i++) {
                rends[i].time = 0.14f;
            }
            fastFireBoltPrefab.GetComponent<ProjectileController>().ghostPrefab = fastBoltGhost;

            //Debug.LogWarning("created" + (fastFireBoltPrefab != null));

            //ContentAddition.AddProjectile(fastFireBoltPrefab);
            Modules.ContentPacks.projectilePrefabs.Add(fastFireBoltPrefab);
            FireFastBolt.fastProjectilePrefab = fastFireBoltPrefab;
        }

        public void AddNewFireboltSkill() {

            GameObject mageCharacterBody = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/MageBody");

            SkillLocator skillLocator = mageCharacterBody.GetComponent<SkillLocator>();

            SkillFamily skillFamily = skillLocator.primary.skillFamily;

            SteppedSkillDef mySkillDef = spoofFireBoltSkillDef(skillFamily.variants[0].skillDef as SteppedSkillDef);

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);

            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant {
                skillDef = mySkillDef,
                unlockableDef= null,
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };

            Modules.ContentPacks.skillDefs.Add(mySkillDef);
            Modules.ContentPacks.entityStates.Add(typeof(FireFastBolt));

            //ContentAddition.AddSkillDef(mySkillDef);
            //ContentAddition.AddEntityState<FireFastBolt>(out _);
        }

        private static SteppedSkillDef spoofFireBoltSkillDef(SteppedSkillDef firefireboltskill) {

            LanguageAPI.Add("ARTI_PRIMARY_FASTBOLTS_NAME", "Fast Flame Bolts");
            //TODO
            string damage = $"<style=cIsDamage>2x{2.2f * Utils.Cfg_DamageMulti * 100}%</style>";
            string flameBolts = $"<style=cIsDamage>Flame Bolts</style>";

            string descriptionText = $"Fire smaller {flameBolts} for {damage}. Fires <style=cIsUtility>more bolts</style> with <style=cIsUtility>attack speed</style>";
            LanguageAPI.Add("ARTI_PRIMARY_FASTBOLTS_DESCRIPTION", descriptionText);

            SteppedSkillDef mySkillDef = ScriptableObject.CreateInstance<SteppedSkillDef>();
            mySkillDef.icon = Utils.FastBoltIcon;
            mySkillDef.skillDescriptionToken = "ARTI_PRIMARY_FASTBOLTS_DESCRIPTION";
            string name = "ARTI_PRIMARY_FASTBOLTS_NAME";
            mySkillDef.skillName = name;
            mySkillDef.skillNameToken = name;
            (mySkillDef as ScriptableObject).name = name;

            mySkillDef.keywordTokens = new string[] { "KEYWORD_IGNITE" };
            mySkillDef.activationState = new SerializableEntityStateType(typeof(FireFastBolt));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.beginSkillCooldownOnSkillEnd = false;
            mySkillDef.resetCooldownTimerOnUse = false;
            mySkillDef.dontAllowPastMaxStocks = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.interruptPriority = InterruptPriority.Any;
            mySkillDef.rechargeStock = 1;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = false;
            mySkillDef.cancelSprintingOnActivation = true;

            mySkillDef.baseMaxStock = 4;
            mySkillDef.baseRechargeInterval = 1.3f;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.stockToConsume = 1;
            mySkillDef.stepCount = 2;  

            //mySkillDef.keywordTokens = firefireboltskill.keywordTokens;
            //mySkillDef.activationState = new SerializableEntityStateType(typeof(FireFastBolt));
            //mySkillDef.activationStateMachineName = firefireboltskill.activationStateMachineName;
            //mySkillDef.baseMaxStock = firefireboltskill.baseMaxStock;
            //mySkillDef.baseRechargeInterval = firefireboltskill.baseRechargeInterval;
            //mySkillDef.beginSkillCooldownOnSkillEnd = firefireboltskill.beginSkillCooldownOnSkillEnd;
            //mySkillDef.resetCooldownTimerOnUse = firefireboltskill.resetCooldownTimerOnUse;
            //mySkillDef.dontAllowPastMaxStocks = firefireboltskill.dontAllowPastMaxStocks;
            //mySkillDef.fullRestockOnAssign = firefireboltskill.fullRestockOnAssign;
            //mySkillDef.canceledFromSprinting = firefireboltskill.canceledFromSprinting;
            //mySkillDef.interruptPriority = firefireboltskill.interruptPriority;
            //mySkillDef.rechargeStock = firefireboltskill.rechargeStock;
            //mySkillDef.isCombatSkill = firefireboltskill.isCombatSkill;
            //mySkillDef.mustKeyPress = firefireboltskill.mustKeyPress;
            //mySkillDef.cancelSprintingOnActivation = firefireboltskill.cancelSprintingOnActivation;
            //mySkillDef.rechargeStock = firefireboltskill.rechargeStock;
            //mySkillDef.requiredStock = firefireboltskill.requiredStock;
            //mySkillDef.stockToConsume = firefireboltskill.stockToConsume;
            //mySkillDef.stepCount = firefireboltskill.stepCount;
            return mySkillDef;
        }

        //void Update() {
            //if (Input.GetKeyDown(KeyCode.U)) {

            //    FireFastBolt._originShiftForward += 0.1f;
            //    Chat.AddMessage(FireFastBolt._originShiftForward.ToString("0.00"));
            //}
            //if (Input.GetKeyDown(KeyCode.J)) {

            //    FireFastBolt._originShiftForward -= 0.1f;
            //    Chat.AddMessage(FireFastBolt._originShiftForward.ToString("0.00"));
            //}
        //}


    }
}
