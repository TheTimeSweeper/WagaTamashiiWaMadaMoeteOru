using RoR2;
using System.Reflection;
using UnityEngine;

namespace SillyGlasses
{
    public class Utils
    {
        public static ConfigEntry<int> Cfg_ItemStackMax;
        public static ConfigEntry<bool> Cfg_OutwardStackType;
        public static ConfigEntry<float> Cfg_ItemDistanceMultiplier;
        public static ConfigEntry<float> Cfg_EngiTurretItemDistanceMultiplier;
        public static ConfigEntry<float> Cfg_ScavengerItemDistanceMultiplier;
        public static ConfigEntry<float> Cfg_BrotherItemDistanceMultiplier;
        public static ConfigEntry<bool> Cfg_NoMaterialUpdate;
        public static ConfigEntry<bool> Cfg_SlightlyUnstable;
        public static ConfigEntry<bool> Cfg_UseLogs;
        public static bool Cfg_PlantsForHire = false;
        public static int Cfg_CheatItem = 7; //glassiese
        public static int Cfg_CheatItemBoring = 58; //magayzenes
        public static bool wegetit;

        public static void Log (string logString, bool chat = false)
        {
            if (Cfg_UseLogs)
            {
                SillyGlassesPlugin.logger.LogMessage($"[SillyLog] {logString}");

                if (chat)
                {
                    Chat.AddMessage(logString);
                }
            }
        }
    }
}