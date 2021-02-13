using System;
using System.Collections.Generic;
using UnityEngine;

namespace SillyHitboxViewer {
    public class Utils {

        public static bool cfg_doHurtbox;
        public static bool cfg_useDebug;
        public static bool cfg_showLogsVerbose;

        public static KeyCode cfg_toggleKey;

        public static string cfg_softenedCharactersString;
        public static List<int> cfg_softenedCharacters = new List<int>();

        public static void LogReadout(object log) {

            if (cfg_showLogsVerbose) {
                Log(log, false);
            }
        }

        public static void LogWarning(object log) {

            if (cfg_showLogsVerbose) {
                HitboxViewerMod.log.LogWarning(log);
            }
        }

        public static void Log(object log, bool chat) {
            if (cfg_useDebug) {
                HitboxViewerMod.log.LogMessage(log);
                if (chat) {
                    RoR2.Chat.AddMessage(log.ToString());
                }
            }
        }

        public static void setSoftenedCharacters() {

            if (!HitboxRevealer.cfg_MercSoften)
                return;

            cfg_softenedCharactersString = cfg_softenedCharactersString.Replace(" ", "");
            string[] strings = cfg_softenedCharactersString.Split(',');

            string parsedBodiesLog = "";

            for (int i = 0; i < strings.Length; i++) {
                string parsedString = strings[i];

                int bodIndex = RoR2.BodyCatalog.FindBodyIndex(parsedString);
                if(bodIndex == -1) {
                    HitboxViewerMod.log.LogWarning($"Could not find Characterbody for '{parsedString}'. This character's hitboxes will not be toned down. be careful");
                    continue;
                }
                cfg_softenedCharacters.Add(bodIndex);
                parsedBodiesLog += $"\nbodyIndex: {bodIndex}: {parsedString}";
            }

            if (!string.IsNullOrEmpty(parsedBodiesLog)) {
                HitboxViewerMod.log.LogMessage($"Toning down hitboxes for" + parsedBodiesLog);
            } else {
                HitboxViewerMod.log.LogWarning($"No characters found to tone down hitboxes, make sure you've typed their names right in the config");
            }
        }
    }
}
