using BepInEx;
using EntityStates;
//using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using R2API;
using UnityEngine;
using System.Security;
using System.Security.Permissions;
using SkillsPlusPlus;
using SkillsPlusPlus.Modifiers;

namespace FastArtiBolts {

    public class SkillsPlusCompat {

        public static void Init() {
            SkillModifierManager.LoadSkillModifiers();

            doLanguage();
        }

        private static void doLanguage() {
            //"SkillDef.skillName".toupper() + "NAME_UPGRADE_DESCRIPTION"
            LanguageAPI.Add("ARTI_PRIMARY_FASTBOLTS_NAME_UPGRADE_DESCRIPTION", "<style=cIsUtility>+2</style> stock and <style=cIsUtility>-10%</style> stock recharge time");
        }

        [SkillLevelModifier("ARTI_PRIMARY_FASTBOLTS_NAME", typeof(FireFastBolt))]
        class CommandoFirePistolSkillModifier : SimpleSkillModifier<FireFastBolt> {

            public override void OnSkillLeveledUp(int level, CharacterBody characterBody, SkillDef skillDef) {
                base.OnSkillLeveledUp(level, characterBody, skillDef);
                skillDef.baseMaxStock = AdditiveScaling(4, 2, level);
                skillDef.baseRechargeInterval = MultScaling(1.3f, -0.1f, level);
            }
        }
    }
}