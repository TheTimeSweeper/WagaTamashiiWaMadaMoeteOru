using System;
using UnityEngine;

namespace SillyHitboxViewer {
    public class Utils {

        public static bool useDebug;

        public static void Log(string log) {
            if (useDebug) {
                Debug.Log(log);
            }
        }
    }
}
