using RoR2;
using UnityEngine;

namespace SillyGlasses
{
    public class Utils
    {
        public static int Cfg_ItemStackMax = -1;
        public static float Cfg_ItemDistanceMultiplier = 0.0420f;
        public static float Cfg_EngiTurretItemDistanceMultiplier = 1.5f;
        public static float Cfg_ScavengerItemDistanceMultiplier = 6;
        public static float Cfg_BrotherItemDistanceMultiplier = 3;
        public static bool Cfg_UseLogs = false;
        public static bool Cfg_PlantsForHire = false;
        public static int Cfg_CheatItem = 7; //glassiese
        public static int Cfg_CheatItemBoring = 58; //magayzenes

        public static void Log (string logString, bool chat = false)
        {
            if (Cfg_UseLogs)
            {
                Debug.Log(logString);

                if (chat)
                {
                    Chat.AddMessage(logString);
                }
            }
        }
    }
}