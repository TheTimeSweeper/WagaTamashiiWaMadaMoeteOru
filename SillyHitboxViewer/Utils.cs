using System;
using System.Collections.Generic;
using UnityEngine;

namespace SillyHitboxViewer {

    public class Utils {

        //debug config
        public static bool cfg_doHurtbox;
        public static bool cfg_useDebug;
        public static bool cfg_showLogsVerbose;

        public static KeyCode cfg_toggleKey;
        public static KeyCode cfg_bulletModeKey;

        //hitbox config
        public static string defaultSoftenedCharacters = "MercBody, MinerBody, MiniMushroomBody, NemesisEnforcerBody, NemesisEnforcerBossBody, UrsaBody, ";
        public static string cfg_softenedCharactersString;
        public static List<int> cfg_softenedCharacters = new List<int>();

        //hurt config
        public static bool cfg_unDynamicHurtboxes;

        #region logging

        public static void Log(object log, bool chat, bool overrideDebug = true) {

            if (cfg_useDebug || overrideDebug) {
                HitboxViewerMod.log.LogMessage(log);
                if (chat) {
                    RoR2.Chat.AddMessage(log.ToString());
                }
            }
        }

        public static void LogError(object log) {
            HitboxViewerMod.log.LogError(log);
        }

        /// <summary>
        /// only with verbose setting
        /// </summary>
        public static void LogReadout(object log) {

            if (cfg_showLogsVerbose) {
                Log(log, false, false);
            }
        }

        /// <summary>
        /// only with verbose setting
        /// </summary>
        public static void LogWarning(object log) {

            if (cfg_showLogsVerbose) {
                HitboxViewerMod.log.LogWarning(log);
            }
        }

        #endregion

        #region config
        public static void doConfig() {

            //box
            const string sectionBox = "1. OverlapAttack hitboxes (melee and some projectiles)";

            HitboxRevealer.cfg_BoxAlpha =
                HitboxViewerMod.instance.Config.Bind( 
                            sectionBox,
                            "OverlapAttack hitbox alpha",
                            0.22f,
                            "0-1. Around 0.22 is ok. don't make it higher if you have epilepsy\n").Value;

            HitboxRevealer.cfg_MercSoften =
                HitboxViewerMod.instance.Config.Bind( 
                            sectionBox,
                            "Tone down merc",
                            true,
                            "Make merc's hitboxes lighter cause he's a crazy fool (and might actually hurt your eyes)\n - overrides alpha brightness to 0.1 and keeps colors cool blue-ish range\n").Value;

            Utils.cfg_softenedCharactersString =
                HitboxViewerMod.instance.Config.Bind( 
                            sectionBox,
                            "Toned-down characters",
                            "MercBody, MinerBody, MiniMushroomBody",
                            $"The wacky characters who need softening, separated by commas.\n - In addition to these, the following characters are always on the list: {Utils.defaultSoftenedCharacters}\n - Character's internal names are: CommandoBody, HuntressBody, ToolbotBody, EngiBody, MageBody, MercBody, TreebotBody, LoaderBody, CrocoBody, Captainbody\n - Use the DebugToolkit mod's body_list command to see a complete list (including enemies and moddeds)\n").Value;

            //blast
            const string sectionBlast = "2. Blastattack hitboxes (hitspheres?)";

            HitboxRevealer.cfg_BlastShowTime =
                HitboxViewerMod.instance.Config.Bind( 
                            sectionBlast,
                            "BlastAttack visual time",
                            0.2f,
                            "the amount of time blast hitboxes show up (their actual damage happens in one frame)\n").Value;

            //gun
            const string sectionGun = "3. BulletAttack hitboxes (hitcylinders?)";

            HitboxRevealer.cfg_BulletAlpha =
                HitboxViewerMod.instance.Config.Bind( 
                            sectionGun,
                            "BulletAttack hitbox alpha",
                            0.14f,
                            "0=1. lil brighter than the others, usually\nif a bullet has 0 radius, this gets increased a bit\n point of contact of the bullet is a little brighter\n").Value;

            HitboxRevealer.cfg_BulletShowTime =
                HitboxViewerMod.instance.Config.Bind( 
                            sectionGun,
                            "BulletAttack visual time",
                            0.4f,
                            "the amount of time bullet rays show (their actual damage happens in one frame)\n").Value;

            Utils.cfg_bulletModeKey =
                HitboxViewerMod.instance.Config.Bind(
                            sectionGun,
                            "Bullet point linger mode keybind",
                            KeyCode.Slash,
                            $"key to toggle bullet point mode\nin this mode, bullet hit points will linger indefinitely\ntoggle it off to clear them\n").Value;

            HitboxRevealer.cfg_ColorfulBullets =
                HitboxViewerMod.instance.Config.Bind( 
                            sectionGun,
                            "Colorful bullets!",
                            false,
                            "if true, each individual bullet cylinder will be a random color.\nWhen false bullets fired on the same frame will be the same color\n").Value;

            HitboxRevealer.cfg_UniformBullets =
                HitboxViewerMod.instance.Config.Bind(
                            sectionGun,
                            "Not colorful bullets!",
                            false,
                            "Overrides colorful Bullets! config. all bullet attacks will be the same color so they don't look like Dippin Dots everywhere\n").Value;

            //hurt me more dawg
            const string sectionPain = "4. Hurtboxes";

            HitboxRevealer.cfg_HurtAlpha =
                HitboxViewerMod.instance.Config.Bind( 
                            sectionPain,
                            "Hurtbox capsule/box alpha",
                            0.169f,
                            "0-1. Around 0.16 is ok.\n").Value;

            //maybe make a setting or command to toggle this at runtime
            Utils.cfg_unDynamicHurtboxes =
                HitboxViewerMod.instance.Config.Bind( 
                            sectionPain,
                            "optimized Hurtboxes",
                            false,
                            "tl;dr: Set to false for development, as it's more useful. Set to true for casual play, as it's more optimized.\n - When set to false, when characters spawn, hurtboxes and their objects will always be initialized for them, so they can be revealed and hidden at will.\n - When set to true, when characters spawn, hurtbox objects will only be initialized if hurtboxes are enabled.\n   - Avoids creating unnecessary hurtboxes, but can't reveal hurtboxes on previously spawned enemies, if that makes sense.\n").Value;

            //i'm not crazy
            const string sectionRetard = "5. be safe";

            Utils.cfg_toggleKey =
                HitboxViewerMod.instance.Config.Bind( 
                            sectionRetard,
                            "Hitbox toggle Keybind",
                            KeyCode.Semicolon,
                            "press this key to toggle disabling hitbox viewer on and off in game. Use to override current settings\n").Value;

            Utils.cfg_useDebug =
                HitboxViewerMod.instance.Config.Bind( 
                            sectionRetard,
                            "debug",
                            false,
                            "welcom 2m y twisted mind\ntimescale hotkeys on I, K, O, and L. press quote key to disable\n").Value;

            Utils.cfg_showLogsVerbose =
                HitboxViewerMod.instance.Config.Bind( 
                            sectionRetard,
                            "extra logs",
                            false,
                            "like a lot of extra logs\n").Value;
        }
        #endregion

        public static void setSoftenedCharacters() {

            if (!HitboxRevealer.cfg_MercSoften)
                return;

            cfg_softenedCharactersString = defaultSoftenedCharacters + cfg_softenedCharactersString;
            cfg_softenedCharactersString = cfg_softenedCharactersString.Replace(" ", "");
            string[] strings = cfg_softenedCharactersString.Split(',');

            string parsedBodiesLog = "";

            for (int i = 0; i < strings.Length; i++) {
                string parsedString = strings[i];

                int bodIndex = (int)RoR2.BodyCatalog.FindBodyIndex(parsedString);
                if(bodIndex == -1) {
                    //HitboxViewerMod.log.LogWarning($"Could not find Characterbody for '{parsedString}'. This character's hitboxes will not be toned down. be careful");
                    continue;
                }
                if (cfg_softenedCharacters.Contains(bodIndex))
                    continue;

                cfg_softenedCharacters.Add(bodIndex);
                parsedBodiesLog += $"\nbodyIndex: {bodIndex}: {parsedString}";
            }

            if (!string.IsNullOrEmpty(parsedBodiesLog)) {
                HitboxViewerMod.log.LogMessage($"Toning down hitboxes for" + parsedBodiesLog);
            } else {
                HitboxViewerMod.log.LogWarning($"No characters found to tone down hitboxes, make sure you've typed their names right in the config");
            }
        }

        //don't think I'll ever get the hang of this
        //please laugh at me I deserve it
        public static Vector3 retardTransformTest(Transform transform, Vector3 normal, Vector3 transformwhatever) {

            if (Input.GetKey(KeyCode.Keypad7)) {
                transformwhatever = transform.TransformDirection(normal);
            }
            if (Input.GetKey(KeyCode.Keypad8)) {
                transformwhatever = transform.InverseTransformDirection(normal);
            }
            if (Input.GetKey(KeyCode.Keypad4)) {
                transformwhatever = transform.TransformVector(normal);
            }
            if (Input.GetKey(KeyCode.Keypad5)) {
                transformwhatever = transform.InverseTransformVector(normal);
            }
            if (Input.GetKey(KeyCode.Keypad1)) {
                transformwhatever = transform.TransformPoint(normal);
            }
            if (Input.GetKey(KeyCode.Keypad2)) {
                transformwhatever = transform.InverseTransformVector(normal);
            }

            return transformwhatever;
        }
    }
}
