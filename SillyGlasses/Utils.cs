using RoR2;
using UnityEngine;

namespace SillyGlasses
{
    public class Utils
    {
        public static void Log (string logString, bool chat = false)
        {
            if (SillyGlasse.CfgBool_UtilsLog.Value)
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