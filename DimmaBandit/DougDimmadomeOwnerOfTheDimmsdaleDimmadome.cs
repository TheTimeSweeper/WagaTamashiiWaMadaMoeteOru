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

#pragma warning disable CS0618 // Type or member is obsolete. sorry I'm lazy
            growAmount =
                Config.Wrap("The Names Doug Dimmadome Owner of the Dimmsdale Dimmadome",
                            "hat grow amount",
                            "multiplier of size gain per kill",
                            0.169f).Value;
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

            On.RoR2.CharacterBody.Start += CharacterBody_Start;

            On.RoR2.CharacterBody.OnDeathStart += CharacterBody_OnDeathStart;
        }        

        private void CharacterBody_OnDeathStart(On.RoR2.CharacterBody.orig_OnDeathStart orig, CharacterBody self) {
            orig(self);

            //config healthscale
            onHatGrow?.Invoke(self.maxHealth/100);
        }

        private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self) {

            orig(self);

            bool doug = _dougs.Contains(self.name);

            if(doug) {
                Chat.AddMessage("The name's Doug Dimmadome,");

                ModelLocator modelLocator = self.modelLocator;

                DimmaBanditHatGrower hatGrower = modelLocator.modelTransform.GetComponent<ChildLocator>()
                    .FindChild("Head").gameObject
                    .AddComponent<DimmaBanditHatGrower>()
                    .init(modelLocator.modelTransform.GetComponent<CharacterModel>());

                Chat.AddMessage("owner of the Dimmsdale immadome");
            }
        }
    }
}
