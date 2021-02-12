using BepInEx;
using EntityStates;
using R2API;
using R2API.AssetPlus;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Linq;
using UnityEngine;

namespace FastArtiBolts {

    [R2APISubmoduleDependency(new string[]
    {
        "LoadoutAPI",
        "LanguageAPI",
        "ResourcesAPI"
        //"EffectAPI",
        //"SurvivorAPI",
        //"UnlockablesAPI",
    })]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod)]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.FastArtiBolts", "Fast Artificer Bolts", "2.0.0")]
    public class FastBoltsMod : BaseUnityPlugin {

        public static FastBoltsMod instance;

        public static GameObject fastFireBoltPrefab;

        public delegate void BaseOnEnterEvent();

        public BaseOnEnterEvent baseOnEnterEvent;

        public void Awake() {

            instance = this;

            Utils.PopulateAssets();

            Utils.InitConfig(Config);

            RegisterFastBoltProjectile();

            AddNewFireboltSkill();
        }

        private void RegisterFastBoltProjectile() {

            fastFireBoltPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/MageFireboltBasic"), "FastFireBolt", true);
            GameObject fastBoltGhost = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/MageFireboltBasic").GetComponent<ProjectileController>().ghostPrefab, "FastFireBoltGhost", false);
            fastBoltGhost.transform.localScale = new Vector3(0.69f, 0.69f, 0.8f);
            fastBoltGhost.transform.Find("Spinner").localScale = Vector3.one * 0.87f;

            var rends = fastBoltGhost.GetComponentsInChildren<TrailRenderer>();
            for (int i = 0; i < rends.Length; i++) {
                rends[i].time = 0.12f;
            }
            //even i'm like this is too much 69 right now, but they're actually workin alright
            fastFireBoltPrefab.GetComponent<ProjectileController>().ghostPrefab = fastBoltGhost;

            ProjectileCatalog.getAdditionalEntries += list => {
                list.Add(fastFireBoltPrefab);
            };
        }

        public void AddNewFireboltSkill() {

            GameObject mageCharacterBody = Resources.Load<GameObject>("prefabs/characterbodies/MageBody");

            SkillLocator skillLocator = mageCharacterBody.GetComponent<SkillLocator>();

            SkillFamily skillFamily = skillLocator.primary.skillFamily;

            SteppedSkillDef mySkillDef = spoofFireBoltSkillDef(skillFamily.variants[0].skillDef as SteppedSkillDef);

            LoadoutAPI.AddSkillDef(mySkillDef);
            LoadoutAPI.AddSkill(typeof(FireFastBolt));

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);

            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };
        }

        private static SteppedSkillDef spoofFireBoltSkillDef(SteppedSkillDef firefireboltskill) {

            LanguageAPI.Add("ARTI_PRIMARY_FASTBOLTS_NAME", "Fast Flame Bolts");
            //TODO
            string damage = $"<style=cIsDamage> {2.2f * Utils.Cfg_DamageMulti * 100}%</style>";
            string descriptionText = $"Fire Flame Bolts for {damage} each. Shoots <style=cIsUtility>more bolts</style> with <style=cIsUtility>higher attack speed</style>";
            LanguageAPI.Add("ARTI_PRIMARY_FASTBOLTS_DESCRIPTION", descriptionText);

            SteppedSkillDef mySkillDef = ScriptableObject.CreateInstance<SteppedSkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(FireFastBolt));
            mySkillDef.activationStateMachineName = firefireboltskill.activationStateMachineName;
            mySkillDef.baseMaxStock = firefireboltskill.baseMaxStock;
            mySkillDef.baseRechargeInterval = firefireboltskill.baseRechargeInterval;
            mySkillDef.beginSkillCooldownOnSkillEnd = firefireboltskill.beginSkillCooldownOnSkillEnd;
            mySkillDef.canceledFromSprinting = firefireboltskill.canceledFromSprinting;
            mySkillDef.fullRestockOnAssign = firefireboltskill.fullRestockOnAssign;
            mySkillDef.interruptPriority = firefireboltskill.interruptPriority;
            mySkillDef.isBullets = firefireboltskill.isBullets;
            mySkillDef.isCombatSkill = firefireboltskill.isCombatSkill;
            mySkillDef.mustKeyPress = firefireboltskill.mustKeyPress;
            mySkillDef.noSprint = firefireboltskill.noSprint;
            mySkillDef.rechargeStock = firefireboltskill.rechargeStock;
            mySkillDef.requiredStock = firefireboltskill.requiredStock;
            mySkillDef.shootDelay = firefireboltskill.shootDelay;
            mySkillDef.stockToConsume = firefireboltskill.stockToConsume;
            mySkillDef.icon = Utils.FastBoltIcon;
            mySkillDef.skillDescriptionToken = "ARTI_PRIMARY_FASTBOLTS_DESCRIPTION";
            mySkillDef.skillName = "ARTI_PRIMARY_FASTBOLTS_NAME";
            mySkillDef.skillNameToken = "ARTI_PRIMARY_FASTBOLTS_NAME";
            mySkillDef.resetStepsOnIdle = firefireboltskill.resetStepsOnIdle;
            mySkillDef.stepCount = firefireboltskill.stepCount;
            return mySkillDef;
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.U)) {

                FireFastBolt._originShift += 0.1f;
                Chat.AddMessage(FireFastBolt._originShift.ToString());
            }
            if (Input.GetKeyDown(KeyCode.J)) {

                FireFastBolt._originShift -= 0.1f;
                Chat.AddMessage(FireFastBolt._originShift.ToString());
            }
        }


    }
}
