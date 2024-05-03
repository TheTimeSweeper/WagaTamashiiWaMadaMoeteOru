using BepInEx;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace SillyMod {
    public class ConFag {

        public static bool MixRor1Survivors;
        public static Ror1MixType Ror1Mix;
        public static bool NemesesSeparate;
        public static bool ForceModdedCharactersOut;
        public static string ForceOutWhiteList;
        public static Dictionary<string, float> CustomOrderSortings;
        public static List<string> ParseErrorLog = new List<string>();
        public static BaseUnityPlugin plugin;

        public static bool Debug;

        public enum Ror1MixType {
            Default,
            Risky1First,
            Risky1ButMixed
        }

        public static void DoConfig(BaseUnityPlugin plugin) {

            ConFag.plugin = plugin;

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
            
            //set your own custom order
            string customOrder =
                plugin.Config.Bind("love ya",
                                   "Custom Order",
                                   "",
                                   "List of characters to apply a custom order, comma separated. ."
                                   + "\nFormat is BodyName:Number. Number can be decimal between two values to place your character in between."
                                   + "\nExample: EngiBody:69,HereticBody:2,EnforcerBody:6.5,HANDOverclockedBody:8.1"
                                   + "\nYou can copy from Print Sorting config below.").Value;

            CustomOrderSortings = ParseCustomOrder(customOrder);
        }

        public static void PrintSortingConfig(string log) {

            Debug =
                plugin.Config.Bind<bool>("love ya",
                            "Print Sorting",
                            true,
                            $"This config does nothing. just shows what the game's current order is when you run the game so you can copy paste\n{log}").Value;
        }

        private static Dictionary<string, float> ParseCustomOrder(string customOrder) {
            Dictionary<string, float> CustomOrderDictionary = new Dictionary<string, float>();

            string[] entries = customOrder.Split(',');

            for (int i = 0; i < entries.Length; i++) {

                string entry = entries[i];
                if (string.IsNullOrEmpty(entry))
                    continue;

                string[] entrySet = entry.Split(':');

                if (entrySet.Length >= 2) {
                    
                    try {
                        string body = entrySet[0].TrimStart().TrimEnd();
                        CustomOrderDictionary[body] = float.Parse(entrySet[1].TrimStart().TrimEnd());
                    } catch (Exception e) {
                        ParseErrorLog.Add($"Custom Sort could not find sort position for entry: {entry}\n{e}");
                    }
                } else {
                    ParseErrorLog.Add($"Custom Sort Entry Invalid: {entry}. Should be BodyName:Number");
                    if (entrySet.Length > 2)
                    {
                        ParseErrorLog.Add($"If someone put : in their bodyname there's nothing I can do. Reach out to the mod creator, tell them Timesweeper sent you.");
                    }
                }
            }

            return CustomOrderDictionary;
        }
    }
}