using BepInEx;
using EntityStates;
using R2API;
using R2API.AssetPlus;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;

namespace FastArtiBolts {

    [R2APISubmoduleDependency(nameof(LoadoutAPI),
                              nameof(LanguageAPI))]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.FastArtiBolts", "Fast Artificer Bolts", "1.0.1")]
    public class FastBoltsMod : BaseUnityPlugin {

        public static FastBoltsMod instance;

        public delegate void BaseOnEnterEvent();

        public BaseOnEnterEvent baseOnEnterEvent;

        public void Awake() {

            instance = this;

            Utils.PopulateAssets();

            AddNewFireboltSkill();

            On.EntityStates.BaseState.OnEnter += BaseState_OnEnter;
        }

        public void AddNewFireboltSkill() {
            // myCharacter should either be
            // Resources.Load<GameObject>("prefabs/characterbodies/BanditBody");
            // or BodyCatalog.FindBodyIndex("BanditBody");
            GameObject mageCharacterBody = Resources.Load<GameObject>("prefabs/characterbodies/MageBody");

            // If you're confused about the language tokens, they're the proper way to add any strings used by the game.
            // We use AssetPlus API for that
            LanguageAPI.Add("ARTI_PRIMARY_FASTBOLTS_NAME", "Fast Flame Bolts");

            string descriptionText = $"Fire Flame Bolts for {colorText("110% damage", "#E5C962")} each. Shoots {colorText("more bolts", "#95CDE5")} with {colorText("higher attack speed", "#95CDE5")}";
            LanguageAPI.Add("ARTI_PRIMARY_FASTBOLTS_DESCRIPTION", descriptionText);

            SkillLocator skillLocator = mageCharacterBody.GetComponent<SkillLocator>();

            SkillFamily skillFamily = skillLocator.primary.skillFamily;

            SkillDef fireFireBoltSkill = skillFamily.variants[0].skillDef;

            SkillDef mySkillDef = cloneFireBoltSkillDef(fireFireBoltSkill);

            //This adds our skilldef. If you don't do this, the skill will not work.
            LoadoutAPI.AddSkillDef(mySkillDef);

            //If this is an alternate skill, use this code.
            // Here, we add our skill as a variant to the exisiting Skill Family.
            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);

            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant {
                skillDef = mySkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            };

            //If this is the default/first skill, copy this code and remove the //,
            //skillFamily.variants[0] = new SkillFamily.Variant
            //{
            //    skillDef = mySkillDef,
            //    unlockableName = "",
            //    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
            //};
        }

        private string colorText(string text, string hex) {
            string thanks = "thanks kinggrinyov for the idea (yes I made this a string instead of a comment so it gets compiled)";
            return $"<color={hex}>{text}</color>";
        }

        private static SkillDef cloneFireBoltSkillDef(SkillDef firefireboltskill) {

            SkillDef mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
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
            return mySkillDef;

            #region Ref
            //var mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            //mySkillDef.activationState = new SerializableEntityStateType(typeof(MyNameSpace.EntityStates.ExampleState));
            //mySkillDef.activationStateMachineName = "Weapon";
            //mySkillDef.baseMaxStock = 1;
            //mySkillDef.baseRechargeInterval = 0f;
            //mySkillDef.beginSkillCooldownOnSkillEnd = true;
            //mySkillDef.canceledFromSprinting = true;
            //mySkillDef.fullRestockOnAssign = true;
            //mySkillDef.interruptPriority = InterruptPriority.Any;
            //mySkillDef.isBullets = true;
            //mySkillDef.isCombatSkill = false;
            //mySkillDef.mustKeyPress = false;
            //mySkillDef.noSprint = true;
            //mySkillDef.rechargeStock = 1;
            //mySkillDef.requiredStock = 1;
            //mySkillDef.shootDelay = 0.5f;
            //mySkillDef.stockToConsume = 1;
            //mySkillDef.icon = Resources.Load<Sprite>("NotAnActualPath");
            //mySkillDef.skillDescriptionToken = "CHARACTERNAME_SKILLSLOT_SKILLNAME_DESCRIPTION";
            //mySkillDef.skillName = "CHARACTERNAME_SKILLSLOT_SKILLNAME_NAME";
            //mySkillDef.skillNameToken = "CHARACTERNAME_SKILLSLOT_SKILLNAME_NAME";

            //Note; if your character does not originally have a skill family for this, use the following:
            //skillLocator.special = gameObject.AddComponent<GenericSkill>();
            //var newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            //LoadoutAPI.AddSkillFamily(newFamily);
            //skillLocator.special.SetFieldValue("_skillFamily", newFamily);
            //var specialSkillFamily = skillLocator.special.skillFamily;

            //Note; you can change component.primary to component.secondary , component.utility and component.special
            #endregion
        }

        private void BaseState_OnEnter(On.EntityStates.BaseState.orig_OnEnter orig, EntityStates.BaseState self) {

            orig(self);

            if (!(self is FireFastBolt)) 
                return;

            baseOnEnterEvent?.Invoke();
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
