using BepInEx;
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
    [BepInPlugin("com.TheTimeSweeper.SurvivorSortOrder", "SurvivorSortOrder", "0.2.1")]
    public class SillySortPlugin : BaseUnityPlugin {            //there's really no reason to call this one silly. just branding at this point

        public static Dictionary<string, float> NewSurivorSortings = new Dictionary<string, float>();
        public static Dictionary<string, float> VanillaSurivorSortings = new Dictionary<string, float>();
        //public static List<string> forceOutWhitelist;

        public static ManualLogSource Log;

        public const float AFTER_VANILLA_INDEX = 20f;
        public const float AFTER_END_INDEX = 25f;

        void Awake() {

            Log = Logger;

            ConFag.DoConfig(this);

            SetSurvivorSortings();

            On.RoR2.SurvivorCatalog.SetSurvivorDefs += SurvivorCatalog_SetSurvivorDefs; ;
        }

        /*
         CommandoBody sort position: 1
         HuntressBody sort position: 2
         Bandit2Body sort position: 3
         ToolbotBody sort position: 4
            HANDOverclockedBody sort position: 100
         EngiBody sort position: 5
            EnforcerBody sort position: 4.005
         MageBody sort position: 6
         MercBody sort position: 7
            SniperClassicBody sort position: 5.5
         TreebotBody sort position: 8
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

            if (ConFag.MixRor1Survivors) {
                NewSurivorSortings["HANDOverclockedBody"] = 4.1f;
                NewSurivorSortings["EnforcerBody"] = 5.1f;
                NewSurivorSortings["SniperClassicBody"] = 7.1f;
                NewSurivorSortings["CHEF"] = 8.2f;
                NewSurivorSortings["MinerBody"] = 9.1f;
            } else {
                NewSurivorSortings["EnforcerBody"] = AFTER_VANILLA_INDEX + 0.1f;
                NewSurivorSortings["SniperClassicBody"] = AFTER_VANILLA_INDEX + 0.2f;
                NewSurivorSortings["HANDOverclockedBody"] = AFTER_VANILLA_INDEX + 0.3f;
                NewSurivorSortings["CHEF"] = AFTER_VANILLA_INDEX + 0.4f;
                NewSurivorSortings["MinerBody"] = AFTER_VANILLA_INDEX + 0.5f;
            }

            if (ConFag.NemesesSeparate) {
                NewSurivorSortings["NemmandoBody"] = AFTER_VANILLA_INDEX + 1 + 0.1f;
                NewSurivorSortings["SS2UNemmandoBody"] = AFTER_VANILLA_INDEX + 1 + 0.1f;
                NewSurivorSortings["NemCommandoBody"] = AFTER_VANILLA_INDEX + 1 + 0.15f;
                NewSurivorSortings["NemesisEnforcerBody"] = AFTER_VANILLA_INDEX + 1 + 0.2f;
            } else {
                NewSurivorSortings["NemmandoBody"] = VanillaSurivorSortings["CommandoBody"] + 0.001f;
                NewSurivorSortings["SS2UNemmandoBody"] = VanillaSurivorSortings["CommandoBody"] + 0.001f;
                NewSurivorSortings["NemCommandoBody"] = VanillaSurivorSortings["CommandoBody"] + 0.0015f;
                NewSurivorSortings["NemesisEnforcerBody"] = NewSurivorSortings["EnforcerBody"] + 0.001f;
            }

        }

        private void SurvivorCatalog_SetSurvivorDefs(On.RoR2.SurvivorCatalog.orig_SetSurvivorDefs orig, SurvivorDef[] newSurvivorDefs) {

            Log.LogMessage("[Before Sort]");
            PrintOrder(newSurvivorDefs);

            for (int i = newSurvivorDefs.Length - 1; i >= 0; i--) {
                string BodyPrefabName = newSurvivorDefs[i].bodyPrefab.name;

                //force other modded characters
                if (ConFag.ForceModdedCharactersOut) {
                    if (!NewSurivorSortings.ContainsKey(BodyPrefabName) && !VanillaSurivorSortings.ContainsKey(BodyPrefabName)) {
                        newSurvivorDefs[i].desiredSortPosition = AFTER_END_INDEX + newSurvivorDefs[i].desiredSortPosition / 10000;
                    }
                }

                //is new survivor (ror1 or nemesis)
                if (NewSurivorSortings.ContainsKey(BodyPrefabName)) {
                    newSurvivorDefs[i].desiredSortPosition = NewSurivorSortings[BodyPrefabName];
                }

                //finally, set custom order
                if(ConFag.CustomOrderSortings.ContainsKey(BodyPrefabName)) {
                    newSurvivorDefs[i].desiredSortPosition = ConFag.CustomOrderSortings[BodyPrefabName];
                    ConFag.CustomOrderSortings.Remove(BodyPrefabName);
                }
            }
            
            //sort is no longer a word
            Log.LogMessage("[After Sort]");
            PrintOrder(newSurvivorDefs, true);

            string log = "";
            foreach (KeyValuePair<string, float> remainingEntry in ConFag.CustomOrderSortings) {
                log += $"{remainingEntry.Key}, ";
            }
            if (!string.IsNullOrEmpty(log)) {
                Log.LogWarning($"Could not find body to sort for: {log}\nEither these characters don't exist or you messed up typing lol.");
            }

            for (int i = 0; i < ConFag.ParseErrorLog.Count; i++) {
                Log.LogError(ConFag.ParseErrorLog[i]);
            }

            orig(newSurvivorDefs);
        }

        private void PrintOrder(SurvivorDef[] newSurvivorDefs, bool toConfig = false) {

            List<SurvivorDef> sortedDefs = newSurvivorDefs.ToList();
            sortedDefs.Sort((def1, def2) => {
                return def1.desiredSortPosition >= def2.desiredSortPosition ? 1 : -1;
            });
            string configString = "Printed Sort Order:\n";
            for (int i = 0; i < sortedDefs.Count; i++) {
                //Language.GetString((newSurvivorDefs[i].displayNameToken), "EN_US")
                Log.LogMessage(sortedDefs[i].bodyPrefab.name + " sort position: " + sortedDefs[i].desiredSortPosition);
                string comma = i == 0 ? "" : ", ";
                configString += $"{comma}{sortedDefs[i].bodyPrefab.name}:{sortedDefs[i].desiredSortPosition}";
            }

            if (toConfig) {
                Log.LogMessage(configString);
                ConFag.PrintSortingConfig(configString);
            }

            //PrintStats(sortedDefs);
        }

        private void PrintStats(List<SurvivorDef> newSurvivorDefs) {

            for (int i = 0; i < newSurvivorDefs.Count; i++) {
                CharacterBody survivor = newSurvivorDefs[i].bodyPrefab.GetComponent<CharacterBody>();

                Log.LogInfo($"{newSurvivorDefs[i]._cachedName}: Health {survivor.baseMaxHealth}, Regen: {survivor.baseRegen}, Armor {survivor.baseArmor}");
            }
        }
    }
}