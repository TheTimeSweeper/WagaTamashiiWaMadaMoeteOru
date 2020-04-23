using RoR2;
using UnityEngine;

namespace SillyGlasses
{
    public class Utils
    {
        public static int Cfg_Int_ItemStackMax = -1;
        public static float Cfg_Float_ItemSwooceDistanceMultiplier = 0.0420f;
        public static float Cfg_Float_EngiTurretItemDistanceMultiplier = 1.5f;
        public static float Cfg_Float_ScavengerItemDistanceMultiplier = 6;
        public static bool Cfg_Bool_UtilsLog = false;
        public static bool Cfg_Bool_PlantsForHire = false;
        public static int Cfg_Int_CheatItem = 7; //glassiese
        public static int Cfg_Int_CheatItemBoring = 58; //magayzenes

        public static void Log (string logString, bool chat = false)
        {
            if (Cfg_Bool_UtilsLog)
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