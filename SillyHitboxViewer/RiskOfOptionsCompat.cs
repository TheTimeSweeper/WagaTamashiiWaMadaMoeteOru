using RiskOfOptions;
using RiskOfOptions.Options;
using UnityEngine;

namespace SillyHitboxViewer {

    public static class RiskOfOptionsCompat {

        #region enabled
        private static bool? _enabled;

        public static bool enabled {
            get {
                if (_enabled == null) {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
                }
                return (bool)_enabled;
            }
        }
        #endregion enabled

        public static Sprite icon;

        public static void doOptions() {
            
            ModSettingsManager.SetModIcon(icon);
            ModSettingsManager.AddOption(new CheckBoxOption(Utils.doHitbox));
            ModSettingsManager.AddOption(new CheckBoxOption(Utils.doHurtbox));
            ModSettingsManager.AddOption(new CheckBoxOption(Utils.doKinos));
        }

        public static void hitboxBoolEvent(bool active) {

            HitboxViewerMod.setShowingHitboxes(!active);
        }
        public static void hurtboxBoolEvent(bool active) {

            HitboxViewerMod.setShowingHurtboxes(!active, true);
        }
    }
}
