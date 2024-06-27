using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace LoadoutSkillTitles
{
    //[NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    [BepInPlugin("com.TheTimeSweeper.LoadoutSkillTitles", "Loadout Skill Titles", "1.0.0")]
    public class LoadoutSkillTitlesPlugin : BaseUnityPlugin
    {
        public static LoadoutSkillTitlesPlugin instance;

        private Dictionary<string, Dictionary<int, string>> titleTokens = new Dictionary<string, Dictionary<int, string>>();

        private ConfigEntry<bool> firstSkillPassive;
        private ConfigEntry<bool> fallbackToSkillName;
        
        public static void AddTitleToken(string bodyName, int skillSlotIndex, string token)
        {
            if (!instance.titleTokens.ContainsKey(bodyName))
            {
                instance.titleTokens[bodyName] = new Dictionary<int, string>();
            }
            instance.titleTokens[bodyName][skillSlotIndex] = token;
        }

        void Awake()
        {
            instance = this;

            InitConfig();
            InitDefaultTitles();

            IL.RoR2.UI.LoadoutPanelController.Row.FromSkillSlot += Row_FromSkillSlot;
        }

        private void InitDefaultTitles()
        {
            AddTitleToken("ToolbotBody", 1, "LOADOUT_SKILL_PRIMARY");
            AddTitleToken("CaptainBody", 4, "LOADOUT_SKILL_CAPTAIN_BEACON1");
            AddTitleToken("CaptainBody", 5, "LOADOUT_SKILL_CAPTAIN_BEACON2");
            //AddTitleToken("RailgunnerBody", 0, "LOADOUT_SKILL_PASSIVE");
            //AddTitleToken("VoidSurvivorBody", 0, "LOADOUT_SKILL_PASSIVE");

            if (Modules.Language.printingEnabled)
            {
                Modules.Language.Add("LOADOUT_SKILL_PASSIVE", "Passive");
                Modules.Language.Add("LOADOUT_SKILL_CAPTAIN_BEACON1", "Beacon 1");
                Modules.Language.Add("LOADOUT_SKILL_CAPTAIN_BEACON2", "Beacon 2");
                Modules.Language.PrintOutput("SkillTitles.language");
            }
        }

        private void InitConfig()
        {
            string section = "hope life is treating you well";

            firstSkillPassive = Modules.Config.BindAndOptions(
                section,
                "First Skill Passive",
                true,
                "Non-primary generic skill in skillslot 0 will be titled 'Passive'",
                false);

            fallbackToSkillName = Modules.Config.BindAndOptions(
                section,
                "Fallback to skillName",
                false,
                "if a proper title token isn't found, use the skillName of the GenericSkill component for that slot",
                false);
        }
        
        private void Row_FromSkillSlot(MonoMod.Cil.ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            cursor.GotoNext(MoveType.After,
                instruction => instruction.MatchLdstr("LOADOUT_SKILL_MISC")
                );
            cursor.Emit(OpCodes.Ldarg_1);
            cursor.Emit(OpCodes.Ldarg_2);
            cursor.Emit(OpCodes.Ldarg_3);
            cursor.EmitDelegate<Func<string, BodyIndex, int, GenericSkill, string>>((originalString, bodyIndex, skillSlotIndex, genericSkill) =>
            {
                string bodyName = BodyCatalog.GetBodyName(bodyIndex);

                if (titleTokens.ContainsKey(bodyName) && titleTokens[bodyName].ContainsKey(skillSlotIndex))
                {
                    return titleTokens[bodyName][skillSlotIndex];
                }

                if (genericSkill == null)
                    return originalString;

                if (!string.IsNullOrEmpty(genericSkill.skillName) && genericSkill.skillName.StartsWith("LOADOUT"))
                {
                    return genericSkill.skillName;
                }

                if (firstSkillPassive.Value && skillSlotIndex == 0 && genericSkill.GetComponent<SkillLocator>().primary != genericSkill)
                {
                    return "LOADOUT_SKILL_PASSIVE";
                }

                if (!string.IsNullOrEmpty(genericSkill.skillName) && fallbackToSkillName.Value) {
                    return genericSkill.skillName;
                }

                return originalString;
            });
        }
    }
}
