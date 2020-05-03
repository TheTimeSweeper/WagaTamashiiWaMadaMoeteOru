using RoR2;
using UnityEngine;

namespace HyperTurbine {

    public class Utils {
        public static bool Cfg_Bool_UtilsLog = true;

        public static void Log(string logString, bool chat = false) {
            if (Cfg_Bool_UtilsLog) {
                Debug.Log(logString);

                if (chat) {
                    Chat.AddMessage(logString);
                }
            }
        }
    }
}

