using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SillySortOrder
{
    [BepInPlugin("com.TheTimeSweeper.SillySortOrder", "SurvivorSortOrder", "1.0.0")]
    public class SillySortPlugin : BaseUnityPlugin
    {            //there's really no reason to call this one silly. just branding at this point
        void Awake()
        {
            //ConFag.DoConfig(this);

            On.RoR2.SurvivorCatalog.SetSurvivorDefs += SurvivorCatalog_SetSurvivorDefs; ;
        }

        private void SurvivorCatalog_SetSurvivorDefs(On.RoR2.SurvivorCatalog.orig_SetSurvivorDefs orig, SurvivorDef[] newSurvivorDefs)
        {

            List<SurvivorDef> defs = newSurvivorDefs.ToList();
            defs.Sort();

            for (int i = 0; i < defs.Count; i++)
            {
                          //Language.GetString((newSurvivorDefs[i].displayNameToken), "EN_US")
                Debug.Log(newSurvivorDefs[i].bodyPrefab.name + " sort position: " + newSurvivorDefs[i].desiredSortPosition);
                if(newSurvivorDefs[i].bodyPrefab.name == "EnforcerBody")
                {
                    newSurvivorDefs[i].desiredSortPosition = 0;
                }
            }

            orig(newSurvivorDefs);
        }
    }

    public class ConFag
    {
        private static bool MixRor1Survivors;
        private static Ror1MixType Ror1Mix;
        private static bool NemesesSeparate;
        private static bool ForceModdedCharactersOut;
        private static string ForcedModdedCharacters;

        enum Ror1MixType
        {
            Default,
            Risky1Style,
            Risky1ButMixed
        }

        public static void DoConfig(BaseUnityPlugin plugin)
        {
            MixRor1Survivors =
                plugin.Config.Bind<bool>("love ya",
                            "Mix ror1 surivors",
                            true,
                            "Modded ror1 survivors will be mixed in with the rest of the cast.\n" 
                            + "Set to false to separate them all out after Captain\n"
                            + "Overrides those mods' original configs").Value;

            Ror1Mix =
                plugin.Config.Bind<Ror1MixType>("love ya",
                            "How ror1 survivors are mixed",
                            Ror1MixType.Default,
                            "Default is order them as if they were risky 2 characters, i.e. relatively in order of unlock difficulty.\n"
                            + "Risky1Style is to order them the same order as ror1, with ror2 newcomers at the end\n"
                            + "Risky1ButMixed idk I think risky 1 order with ror2 mixed in").Value;

            NemesesSeparate =
                plugin.Config.Bind<bool>("love ya",
                            "Separate Nemesis",
                            false,
                            "Set to true to separate out nemesis survivors from the main lineup, group up next to each other after captain.\n"
                            + "Set to false to leave them where their mod placed them (usually next to their counterpart)").Value;

            ForceModdedCharactersOut =
                plugin.Config.Bind<bool>("love ya",
                            "Force Modded Characters Out",
                            false,
                            "Set to true to separate out any other non-ror1 modded characters.\n"
                            + "Set to false to leave them where their mod placed them").Value;

            ForcedModdedCharacters =
                plugin.Config.Bind<string>("love ya",
                            "Force Modded Characters Out",
                            "none",
                            "Enter names of modded survivors, comma separated (no spaces), to force them out of the vanilla lineup.\n"
                            + "Accepts 'none', and 'all'.\n"
                            + "set '-' before a character to whitelist them from 'all'\n"
                            //make sure these examples work
                            //be a chad and allow any format of character names:nemforcerbody,mdlnemforcer,Nemesis Enforcer
                            + "ex: enter [executioner,ursa] to force these two characters after captain\n"
                            + "ex: enter [all,-paladin] would separate all modded characters, but keep paladin where he is").Value;

            //set your own custom order

            //customOrder =
            //    plugin.Config.Bind("love ya",
            //                "Force Modded Characters Out",
            //                false,
            //                "whatevs.\n"
            //                + "for example: whatevs").Value;

        }
    }
}

/*
[Info   : Unity Log] [RoR2.SurvivorCatalog] ENFORCER_NAME sort position: 4.005
[Info   : Unity Log] [RoR2.SurvivorCatalog] NEMFORCER_NAME sort position: 4.01
[Info   : Unity Log] [RoR2.SurvivorCatalog] CHEF_NAME sort position: 99
[Info   : Unity Log] [RoR2.SurvivorCatalog] PALADIN_NAME sort position: 7.001
[Info   : Unity Log] [RoR2.SurvivorCatalog] WYATT_BODY_NAME sort position: 11.5
[Info   : Unity Log] [RoR2.SurvivorCatalog] HABIBI_JOE_BODY_NAME sort position: 69
[Info   : Unity Log] [RoR2.SurvivorCatalog] ZARKO_V1_BODY_NAME sort position: 101
[Info   : Unity Log] [RoR2.SurvivorCatalog] BANDIT2_BODY_NAME sort position: 3
[Info   : Unity Log] [RoR2.SurvivorCatalog] CAPTAIN_BODY_NAME sort position: 11
[Info   : Unity Log] [RoR2.SurvivorCatalog] COMMANDO_BODY_NAME sort position: 1
[Info   : Unity Log] [RoR2.SurvivorCatalog] CROCO_BODY_NAME sort position: 10
[Info   : Unity Log] [RoR2.SurvivorCatalog] ENGI_BODY_NAME sort position: 5
[Info   : Unity Log] [RoR2.SurvivorCatalog] HERETIC_BODY_NAME sort position: 13
[Info   : Unity Log] [RoR2.SurvivorCatalog] HUNTRESS_BODY_NAME sort position: 2
[Info   : Unity Log] [RoR2.SurvivorCatalog] LOADER_BODY_NAME sort position: 9
[Info   : Unity Log] [RoR2.SurvivorCatalog] MAGE_BODY_NAME sort position: 6
[Info   : Unity Log] [RoR2.SurvivorCatalog] MERC_BODY_NAME sort position: 7
[Info   : Unity Log] [RoR2.SurvivorCatalog] TOOLBOT_BODY_NAME sort position: 4
[Info   : Unity Log] [RoR2.SurvivorCatalog] TREEBOT_BODY_NAME sort position: 8
*/