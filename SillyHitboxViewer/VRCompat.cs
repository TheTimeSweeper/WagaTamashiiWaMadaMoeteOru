using RoR2;

namespace SillyHitboxViewer {
    public static class VRCompat {

        #region enabled
        private static bool? _enabled;

        public static bool enabled {
            get {
                if (_enabled == null) {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DrBibop.VRAPI");
                }
                return (bool)_enabled;
            }
        }
        #endregion enabled

        public static bool IsLocalVRPlayer(CharacterBody body) {
            return enabled && body == LocalUserManager.GetFirstLocalUser().cachedBody;
        }
    }
}
