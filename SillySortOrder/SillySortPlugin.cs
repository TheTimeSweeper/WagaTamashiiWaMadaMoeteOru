﻿using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Security;
using System.Security.Permissions;
using BepInEx.Logging;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace SillyMod {
    [BepInPlugin("com.TheTimeSweeper.SurvivorSortOrder", "SurvivorSortOrder", "0.1.2")]
    public class SillySortPlugin : BaseUnityPlugin {            //there's really no reason to call this one silly. just branding at this point

        public static Dictionary<string, float> NewSurivorSortings;
        public static Dictionary<string, float> VanillaSurivorSortings;
        public static List<string> forceOutWhitelist;

        void Awake() {

            ConFag.DoConfig(this);

            SetSurvivorSortings();

            On.RoR2.SurvivorCatalog.SetSurvivorDefs += SurvivorCatalog_SetSurvivorDefs; ;
        }

        /*
         CommandoBody sort position: 1
         HuntressBody sort position: 2
         Bandit2Body sort position: 3
         ToolbotBody sort position: 4
         EngiBody sort position: 5
            EnforcerBody sort position: 4.005
         MageBody sort position: 6
         MercBody sort position: 7
            SniperClassicBody sort position: 5.5
         TreebotBody sort position: 8
            HANDOverclockedBody sort position: 100
            CHEF sort position: 99
         LoaderBody sort position: 9
            MinerBody sort position: 17
         CrocoBody sort position: 10
         CaptainBody sort position: 11
         HereticBody sort position: 13
         RailgunnerBody sort position: 14
         VoidSurvivorBody sort position: 15

            NemesisEnforcerBody sort position: 4.010
         JoeBody sort position: 69
         V1Body sort position: 101
        */

        private void SetSurvivorSortings() {

            VanillaSurivorSortings = new Dictionary<string, float>();

            VanillaSurivorSortings["CommandoBody"] = 1f;
            VanillaSurivorSortings["HuntressBody"] = 2f;
            VanillaSurivorSortings["Bandit2Body"] = 3f;
            VanillaSurivorSortings["ToolbotBody"] = 4f;
            VanillaSurivorSortings["EngiBody"] = 5f;
            VanillaSurivorSortings["MageBody"] = 6f;

            VanillaSurivorSortings["MercBody"] = 7f;
            VanillaSurivorSortings["TreebotBody"] = 8f;
            VanillaSurivorSortings["LoaderBody"] = 9f;
            VanillaSurivorSortings["CrocoBody"] = 10f;
            VanillaSurivorSortings["CaptainBody"] = 11f;
            VanillaSurivorSortings["HereticBody"] = 13f;
            VanillaSurivorSortings["RailgunnerBody"] = 14f;
            VanillaSurivorSortings["VoidSurvivorBody"] = 15f;
            //todo: check vanilla content packs if you wanna be legit about it

            NewSurivorSortings = new Dictionary<string, float>();

            float afterVanillaIndex = 20.0f;

            if (ConFag.MixRor1Survivors) {
                NewSurivorSortings["EnforcerBody"] = 5.1f;
                NewSurivorSortings["SniperClassicBody"] = 7.1f;
                NewSurivorSortings["HANDOverclockedBody"] = 8.1f;
                NewSurivorSortings["CHEF"] = 8.2f;
                NewSurivorSortings["MinerBody"] = 9.1f;
            } else {
                NewSurivorSortings["EnforcerBody"] = afterVanillaIndex + 0.1f;
                NewSurivorSortings["SniperClassicBody"] = afterVanillaIndex + 0.2f;
                NewSurivorSortings["HANDOverclockedBody"] = afterVanillaIndex + 0.3f;
                NewSurivorSortings["CHEF"] = afterVanillaIndex + 0.4f;
                NewSurivorSortings["MinerBody"] = afterVanillaIndex + 0.5f;
            }

            if (ConFag.NemesesSeparate) {
                NewSurivorSortings["NemmandoBody"] = afterVanillaIndex + 1 + 0.1f;
                NewSurivorSortings["NemesisEnforcerBody"] = afterVanillaIndex + 1 + 0.2f;
            } else {
                NewSurivorSortings["NemmandoBody"] = VanillaSurivorSortings["CommandoBody"] + 0.001f;
                NewSurivorSortings["NemesisEnforcerBody"] = NewSurivorSortings["EnforcerBody"] + 0.001f;
            }

        }

        private void SurvivorCatalog_SetSurvivorDefs(On.RoR2.SurvivorCatalog.orig_SetSurvivorDefs orig, SurvivorDef[] newSurvivorDefs) {

            PrintOrder(newSurvivorDefs);

            for (int i = newSurvivorDefs.Length - 1; i >= 0; i--) {
                //is new survivor (ror1 or nemesis)
                if (NewSurivorSortings.ContainsKey(newSurvivorDefs[i].bodyPrefab.name)) {
                    newSurvivorDefs[i].desiredSortPosition = NewSurivorSortings[newSurvivorDefs[i].bodyPrefab.name];
                    continue;
                }

                float afterEndIndex = 25.0f;
                //force other modded characters
                if (ConFag.ForceModdedCharactersOut) {
                    if (!NewSurivorSortings.ContainsKey(newSurvivorDefs[i].bodyPrefab.name) && !VanillaSurivorSortings.ContainsKey(newSurvivorDefs[i].bodyPrefab.name)) {
                        newSurvivorDefs[i].desiredSortPosition = afterEndIndex + newSurvivorDefs[i].desiredSortPosition / 1000;
                    }
                }
            }

            if (ConFag.Debug)
                Logger.LogInfo("sorted");

            PrintOrder(newSurvivorDefs);

            orig(newSurvivorDefs);
        }

        private void PrintOrder(SurvivorDef[] newSurvivorDefs) {

            if (!ConFag.Debug)
                return;

            List<SurvivorDef> defs = newSurvivorDefs.ToList();
            defs.Sort((def1, def2) => {
                return def1.desiredSortPosition >= def2.desiredSortPosition ? 1 : -1;
            });

            for (int i = 0; i < defs.Count; i++) {
                //Language.GetString((newSurvivorDefs[i].displayNameToken), "EN_US")
                Logger.LogMessage(defs[i].bodyPrefab.name + " sort position: " + defs[i].desiredSortPosition);
            }
        }
    }

    public class ConFag {

        public static bool MixRor1Survivors;
        public static Ror1MixType Ror1Mix;
        public static bool NemesesSeparate;
        public static bool ForceModdedCharactersOut;
        public static string ForceOutWhiteList;

        public static bool Debug;

        public enum Ror1MixType {
            Default,
            Risky1First,
            Risky1ButMixed
        }

        public static void DoConfig(BaseUnityPlugin plugin) {

            MixRor1Survivors =
                plugin.Config.Bind<bool>("love ya",
                            "Mix ror1 surivors",
                            true,
                            "Modded ror1 survivors will be mixed in with the rest of the cast, loosely based on their unlock condition.\n"
                            + "Set to false to separate them out and neatly place them together right after Vanilla\n"
                            + "Overrides those mods' original configs").Value;

            //Ror1Mix =
            //    plugin.Config.Bind<Ror1MixType>("love ya",
            //                "How ror1 survivors are mixed",
            //                Ror1MixType.Default,
            //                "Default is to order them as if they were risky 2 characters, i.e. relatively in order of unlock difficulty.\n"
            //                + "Risky1Style is to order them the same order as ror1, with ror2 newcomers at the end\n"
            //                + "Risky1ButMixed idk I think risky 1 order with ror2 mixed in\n"
            //                + "does nothing if MixRor1Survivors is set to false, obviously").Value;
            //                 //the fuck does banditclassic go
            NemesesSeparate =
                plugin.Config.Bind<bool>("love ya",
                            "Separate Nemesis",
                            false,
                            "Set to true to separate out nemesis survivors from the main lineup, grouped up after ror1 and ror2 survivors.\n"
                            + "Set to false to place next to their counterpart").Value;

            ForceModdedCharactersOut =
                plugin.Config.Bind<bool>("love ya",
                            "Force Modded Characters Out",
                            true,
                            "Set to true to separate out any other non-ror1 modded characters.\n"
                            + "Set to false to leave them where their mod placed them").Value;

            Debug =
                plugin.Config.Bind<bool>("love ya",
                            "Print Sorting",
                            true,
                            "Show in console original sort positions and new sortings after this mod takes effect").Value;

            //ForcedModdedCharacters =
            //    plugin.Config.Bind<string>("love ya",
            //                "Forced out whitelist",
            //                "none",
            //                "Enter names of modded survivors, comma separated, to exclude them from force-out config above.\n"
            //                //be a chad and allow any format of character names:nemforcerbody,mdlnemforcer,Nemesis Enforcer
            //                + "ex: enter [executioner,paladin] to leave these characters where their mod put them\n"

            //set your own custom order

            //customOrder =
            //    plugin.Config.Bind("love ya",
            //                "apply custom order",
            //                ":P",
            //                "dictionary of name and number? EngiBody:69,HereticBody:2,etc:X,"
            //                + "").Value;
            string defaultStort = "CommandoBody: 1\n" +
                                  "HuntressBody: 2\n" + 
                                  "Bandit2Body: 3\n" + 
                                  "ToolbotBody: 4\n" + 
                                  "EngiBody: 5\n" + 
                                  "   EnforcerBody: 5.1\n" + 
                                  "MageBody: 6\n" + 
                                  "MercBody: 7\n" + 
                                  "   SniperClassicBody: 7.1\n" + 
                                  "TreebotBody: 8\n" + 
                                  "   HANDOverclockedBody: 8.1\n" + 
                                  "   CHEF: 8.2\n" + 
                                  "LoaderBody: 9\n" + 
                                  "   MinerBody: 9.1\n" + 
                                  "CrocoBody: 10\n" + 
                                  "CaptainBody: 11\n" + 
                                  "HereticBody: 13"; 
        }
    }
}