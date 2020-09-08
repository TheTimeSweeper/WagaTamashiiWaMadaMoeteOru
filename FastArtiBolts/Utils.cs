using BepInEx.Configuration;
using R2API;
using R2API.AssetPlus;
using System.Reflection;
using UnityEngine;

namespace FastArtiBolts {
    public static class Utils {
        #region as
        public static AssetBundle MainAssetBundle = null;
        public static AssetBundleResourcesProvider Provider;

        // FX
        //public static GameObject JoeFireball = null;

        // icons
        public static Sprite FastBoltIcon = null;

        public static void PopulateAssets() {
            // populate ASSets
            if (MainAssetBundle == null) {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FastArtiBolts.arti")) {

                    MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                    Provider = new AssetBundleResourcesProvider("@FastArtiBolts", MainAssetBundle);
                }
            }
            // gather assets
            FastBoltIcon = MainAssetBundle.LoadAsset<Sprite>("texMageSkillIcon");
        }
        #endregion

        #region conf
        public static float Cfg_DamageMulti = 0.5f;
        public static float Cfg_AttackSpeedMulti = 3f;
        public static float Cfg_DurationMulti = 0.51f;
        public static bool Cfg_potato = false;

        public static void InitConfig() {
            string sectionName = "Dont give up, I believe in you!";

            Cfg_DamageMulti = AbboosBrandBindConfig(sectionName,
                                                    "FastBoltDamageMulti",
                                                    "The damage (and proccoefficient) of Fast Bolts is the damage of original Flame Bolts * this multiplier",
                                                    0.5f);

            Cfg_AttackSpeedMulti = AbboosBrandBindConfig(sectionName,
                                                    "FastBoltAttackSpeedMulti",
                                                    "Multiplier of attack speed that decides how fast you shoot flame bolts (which decides how many bolts you shoot within the window of the move)",
                                                    3f);

            Cfg_DurationMulti = AbboosBrandBindConfig(sectionName,
                                                    "FiringBoltsDurationMulti",
                                                    "The duration of the skill during which extra bolts will fire",
                                                    0.52f);

            Cfg_potato = AbboosBrandBindConfig(sectionName,
                                                    "potato",
                                                    "If true, when multiple bolts are spawned on the same frame, it will instead spawn one bolt with combined damage",
                                                    false);
        }

        private static T AbboosBrandBindConfig<T>(string sectionName, string keyname, string description, T def) {

            ConfigDefinition itemStackMaxDef = new ConfigDefinition(sectionName, keyname);
            ConfigDescription itemstackMaxDesc = new ConfigDescription(description);

            ConfigEntry<T> setItemStackMax = FastBoltsMod.instance.Config.Bind<T>(itemStackMaxDef, def, itemstackMaxDesc);

            return setItemStackMax.Value;
        }
        #endregion
    }
}
