using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using R2API.Utils;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using System.Reflection;
using EntityStates.Missions.ArtifactWorld.TrialController;

namespace DimmaBandit {
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.DimmaBandit", "The Names Doug Dimmadome Owner of the Dimmsdale Dimmadome", "0.2.0")]
    public class DougDimmadomeOwnerOfTheDimmsdaleDimmadome : BaseUnityPlugin {

        public static DougDimmadomeOwnerOfTheDimmsdaleDimmadome Instance;

        public static float growAmount = 0.169f;

        public static bool moveCamera = true;

        public static GameObject dimmadomePrefab;

        public delegate void hatGrowEvent(float amountScale);
        public hatGrowEvent onHatGrow;

        private string[] _dougs = new string[]{
            "BanditBody(Clone)",
            "BanditReloadedBody(Clone)",
            "WildBody(Clone)"
        };

        private void populateAss() {
            AssetBundle MainAss = null;
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DimmaBandit.dimmabandit")) {
                MainAss = AssetBundle.LoadFromStream(assetStream);
            }
             
            dimmadomePrefab = MainAss.LoadAsset<GameObject>("the name's doug dimmadome");
        }

        private void doConfig() {

            growAmount =
                Config.Bind("The Names Doug Dimmadome Owner of the Dimmsdale Dimmadome",
                            "hat grow amount",
                            0.169f,
                            "multiplier of size gain per kill").Value;
            //moveCamera =
            //    Config.Wrap("The Names Doug Dimmadome Owner of the Dimmsdale Dimmadome",
            //                "move camera",
            //                "shift camera aside so hat doesn't cover crosshair",
            //                true).Value;
        }

        void Awake() {
            Instance = this;

            populateAss();
            doConfig();

            On.RoR2.ModelSkinController.ApplySkin += ModelSkinController_ApplySkin;

            On.RoR2.CharacterBody.OnDeathStart += CharacterBody_OnDeathStart;
        }
        private void CharacterBody_OnDeathStart(On.RoR2.CharacterBody.orig_OnDeathStart orig, CharacterBody self) {
            orig(self);

            onHatGrow?.Invoke(self.maxHealth * 0.01f);
        }

        private void ModelSkinController_ApplySkin(On.RoR2.ModelSkinController.orig_ApplySkin orig, ModelSkinController self, int skinIndex) {

            orig(self, skinIndex);

            CharacterModel model = self.GetComponent<CharacterModel>();

            if (_dougs.Contains(model.body.name) || model.name == "mdlBandit") {

                Chat.AddMessage("<color=#fd2>The name's Doug Dimmadome,</color>");

                DimmaBanditHatGrower hatGrower = model.GetComponent<ChildLocator>()
                    .FindChild("Head").transform.Find("hat").gameObject
                    .AddComponent<DimmaBanditHatGrower>()
                    .init(model);

                Chat.AddMessage("<color=#fd2>owner of the Dimmsdale dimmadome</color>");
            }
        }
    }
}
