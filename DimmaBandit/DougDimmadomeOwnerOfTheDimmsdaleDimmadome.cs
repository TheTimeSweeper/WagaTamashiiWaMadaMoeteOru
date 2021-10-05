using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using System.Reflection;
using R2API.Utils;

namespace DimmaBandit {
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.DimmaBandit", "The Names Doug Dimmadome Owner of the Dimmsdale Dimmadome", "0.3.1")]
    public class DougDimmadomeOwnerOfTheDimmsdaleDimmadome : BaseUnityPlugin {

        public static DougDimmadomeOwnerOfTheDimmsdaleDimmadome Instance;

        public static float growAmount = 0.169f;

        public static bool moveCamera = true;

        public static GameObject dimmadomePrefab;
        public static GameObject dimmadomePrefab2;

        public delegate void hatGrowEvent(float amountScale);
        public hatGrowEvent onHatGrow;

        public delegate void ragdollEvent(GameObject model);
        public ragdollEvent onRagdoll;

        private void populateAss() {
            AssetBundle MainAss = null;
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DimmaBandit.dimmabandit")) {
                MainAss = AssetBundle.LoadFromStream(assetStream);
            }
             
            dimmadomePrefab = MainAss.LoadAsset<GameObject>("the name's doug dimmadome");
            dimmadomePrefab2 = MainAss.LoadAsset<GameObject>("the name's doug dimmadome2");
        }

        private void doConfig() {

            growAmount =
                Config.Bind("The Names Doug Dimmadome Owner of the Dimmsdale Dimmadome",
                            "hat grow amount",
                            0.169f,
                            "multiplier of size gain per kill (scales with enemy health)").Value;
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

            On.RoR2.RagdollController.BeginRagdoll += RagdollController_BeginRagdoll;
        }

        private void RagdollController_BeginRagdoll(On.RoR2.RagdollController.orig_BeginRagdoll orig, RagdollController self, Vector3 force) {
            orig(self, force);
            onRagdoll?.Invoke(self.gameObject);
        }

        private void CharacterBody_OnDeathStart(On.RoR2.CharacterBody.orig_OnDeathStart orig, CharacterBody self) {
            orig(self);

            onHatGrow?.Invoke(self.maxHealth * 0.012f);
        }

        private void ModelSkinController_ApplySkin(On.RoR2.ModelSkinController.orig_ApplySkin orig, ModelSkinController self, int skinIndex) {

            orig(self, skinIndex);

            CharacterModel model = self.GetComponent<CharacterModel>();

            bool bandit1 = model.name == "mdlBandit";
            bool bandit2 = model.name == "mdlBandit2";
            if (bandit1 || bandit2) {

                Chat.AddMessage("<color=#fd2>The name's Doug Dimmadome,</color>");

                DimmaBanditHatGrower hatGrower = model.GetComponent<ChildLocator>()
                    .FindChild("Head").GetChild(0).gameObject
                    .AddComponent<DimmaBanditHatGrower>()
                    .init(model, bandit1);

                Chat.AddMessage("<color=#fd2>owner of the Dimmsdale dimmadome</color>");
            }
        }
    }
}
