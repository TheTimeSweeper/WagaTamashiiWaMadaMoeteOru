using BepInEx.Configuration;
using System.Reflection;
using UnityEngine;

namespace FastArtiBolts {
    public static class Utils {

        #region asse
        public static AssetBundle MainAssetBundle = null;

        // icons
        public static Sprite FastBoltIcon = null;

        public static void PopulateAssets() {
            if (MainAssetBundle == null) {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FastArtiBolts.arti")) {
                    MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }

            // gather assets
            FastBoltIcon = MainAssetBundle.LoadAsset<Sprite>("texMageSkillIcon");
        }

        #endregion

            #region conf
        public static float Cfg_DamageMulti = 0.4f;
        public static float Cfg_ProcMulti = 0.6f;
        public static float Cfg_AttackSpeedMulti = 3f;
        public static float Cfg_DurationMulti = 0.51f;
        public static bool Cfg_potato = false;

        public static void InitConfig(ConfigFile Config) {
            string sectionName = "Do your best, I believe in you";

            Cfg_DamageMulti = 
                Config.Bind(sectionName,
                            "FastBoltsDamageMulti",
                            0.5f,
                            "The damage of Fast Bolts is the damage of original Flame Bolts * this multiplier").Value;

            Cfg_ProcMulti = 
                Config.Bind(sectionName,
                            "FastBoltsProcMulti",
                            0.5f,
                            "The Proc Coefficient of Fast Bolts is the Proc Coefficient of original Flame Bolts * this multiplier").Value;

            Cfg_AttackSpeedMulti = 
                Config.Bind(sectionName,
                            "FastBoltAttackSpeedMulti",
                            3f,
                            "Multiplier of attack speed that decides how fast you shoot flame bolts (which decides how many bolts you shoot within the window of the move)").Value;

            Cfg_DurationMulti =   
                Config.Bind(sectionName,
                            "FiringBoltsDurationMulti",
                            0.52f,
                            "The duration of the skill during which extra bolts will fire").Value;

            Cfg_potato = 
                Config.Bind(sectionName,
                            "potato",
                            false,
                            "If true, when multiple bolts are spawned on the same frame, it will instead spawn one bolt with combined damage\nThis also combines proc coefficient, which will lead to proc coefficients higher than 1, and I don't know what hilarity will ensue").Value;
        }
        #endregion
    }


    public class help : MonoBehaviour {
        void OnDestroy() {
            Debug.LogWarning("rip " + gameObject.name);
        }

        void OnDisable() {
            Debug.LogWarning("uh disable " + gameObject.name);
        }
        void OnEnable() {
            Debug.LogWarning("uh enable " + gameObject.name);
        }
    }
}
