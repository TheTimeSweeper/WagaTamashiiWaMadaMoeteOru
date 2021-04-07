using RiskOfOptions;

namespace SillyHitboxViewer {
    public static class RiskOfOptionsCompat {
        private static bool? _enabled;

        public static bool enabled {
            get {
                if (_enabled == null) {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
                }
                return (bool)_enabled;
            }
        }

        public static void doOptions() {

            ModSettingsManager.setPanelTitle("Hitbox Viewer");
            ModSettingsManager.setPanelDescription("Enable/disable hitbox or hurtbox viewer");

            ModSettingsManager.addOption(new ModOption(ModOption.OptionType.Bool, "Enable Hitboxes", "", "1"));
            ModSettingsManager.addListener(ModSettingsManager.getOption("Enable Hitboxes"), new UnityEngine.Events.UnityAction<bool>(hitboxBoolEvent));

            ModSettingsManager.addOption(new ModOption(ModOption.OptionType.Bool, "Enable Hurtboxes", "", "0"));
            ModSettingsManager.addListener(ModSettingsManager.getOption("Enable Hurtboxes"), new UnityEngine.Events.UnityAction<bool>(hurtboxBoolEvent));

        }

        public static void hitboxBoolEvent(bool active) {

            HitboxRevealer.showingHitBoxes = !active;
        }
        public static void hurtboxBoolEvent(bool active) {

            HitboxRevealer.showingHurtBoxes = !active;
            HitboxViewerMod.instance.showAllHurtboxes();
        }

        public static void readOptions() {

            string disableHit = ModSettingsManager.getOptionValue("Enable Hitboxes");
            if (!string.IsNullOrEmpty(disableHit)) {
                HitboxRevealer.showingHitBoxes = disableHit == "1";
            }

            string disableHurt = ModSettingsManager.getOptionValue("Enable Hurtboxes");
            if (!string.IsNullOrEmpty(disableHurt)) {
                HitboxRevealer.showingHurtBoxes = disableHurt == "1";
            }
        }

    }
}
