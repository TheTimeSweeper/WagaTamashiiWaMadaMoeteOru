using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using RoR2;
using RoR2.Skills;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AcridHitbox {
    //TODO: see if only host needs it
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod)] 
    //[BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.3rdRowCSS", "3rdRowCSS", "1.0.0")]
    public class BiggerCSSPlugin : BaseUnityPlugin {

        private static ConfigEntry<bool> cfg_donothing;


        public void Start() {

            doConfig();



            On.RoR2.UI.CharacterSelectController.Awake += CharacterSelectController_Awake;
        }

        private static int GetVisibleSurvivorCount() {

            List<SurvivorDef> list = new List<SurvivorDef>();
            foreach (SurvivorDef survivorDef in SurvivorCatalog.orderedSurvivorDefs) {

                if (ShouldDisplaySurvivor(survivorDef)) {
                    list.Add(survivorDef);
                }
            }
            return list.Count;
        }

        private static bool ShouldDisplaySurvivor(SurvivorDef survivorDef) {
            return survivorDef && !survivorDef.hidden;
        }

        private void CharacterSelectController_Awake(On.RoR2.UI.CharacterSelectController.orig_Awake orig, RoR2.UI.CharacterSelectController self) {

            orig(self);

            //ShowBoxesGodDang(LeftPanel);

            if (GetVisibleSurvivorCount() < 17)
                return;

            Config.Reload();

            if (!cfg_donothing.Value) {
                DoYerThing(self.transform);
            }

        }

        private static void DoYerThing(Transform CSSUIRoot) {

            //give us a litlte more room
            RectTransform LeftPanel = (RectTransform)CSSUIRoot.Find("SafeArea/LeftHandPanel (Layer: Main)");
            if (LeftPanel) {
                LeftPanel.anchorMin = new Vector2(LeftPanel.anchorMin.x, 0.115f);
                LeftPanel.anchorMax = new Vector2(LeftPanel.anchorMax.x, 1.015f);
            }

            LeftPanel.GetComponent<VerticalLayoutGroup>().spacing = 0;

            //move chatbox out of the way
            RectTransform ChatboxPanel = (RectTransform)CSSUIRoot.Find("SafeArea/ChatboxPanel");

            if (ChatboxPanel) {
                ChatboxPanel.anchorMin = new Vector2(0, -0.02f);
                ChatboxPanel.anchorMax = new Vector2(0.35f, 0.115f);
            }

            //expand CSS border to fit a third row (which moves the rest down
            LayoutElement SurvivorGridElement = CSSUIRoot.GetComponentInChildren<CharacterSelectBarController>()?.GetComponent<LayoutElement>();
            if (SurvivorGridElement) {
                SurvivorGridElement.minHeight = 230;
            }

            //name's a lil small now. make lil bigger
            VerticalLayoutGroup SurvivorInfoPanel = CSSUIRoot.Find("SafeArea/LeftHandPanel (Layer: Main)/SurvivorInfoPanel, Active (Layer: Secondary)")?.GetComponent<VerticalLayoutGroup>();
            if (SurvivorInfoPanel) {
                SurvivorInfoPanel.spacing = 3;
            }

            //name's a lil small now. make lil bigger
            LayoutElement NameElement = CSSUIRoot.Find("SafeArea/LeftHandPanel (Layer: Main)/SurvivorInfoPanel, Active (Layer: Secondary)/SurvivorNamePanel")?.GetComponent<LayoutElement>();
            if (NameElement) {
                NameElement.preferredHeight = 36;
            }
        }

        private void ShowBoxesGodDang(Transform CSS) {
            //grab a Border Image from some ui element
            Sprite bg = CSS.transform
                .Find("SafeArea/LeftHandPanel (Layer: Main)/BorderImage")
                .GetComponent<Image>().sprite;

            Transform root = CSS.transform.Find("SafeArea/LeftHandPanel (Layer: Main)");

            Image mig;

            foreach (RectTransform rect in root.GetComponentsInChildren<RectTransform>()) {
                if (rect.GetComponent<Graphic>() == null) {
                    mig = rect.gameObject.AddComponent<Image>();
                    mig.sprite = bg;
                    mig.type = Image.Type.Sliced;
                    mig.raycastTarget = false;
                }

                //mig = rect.GetComponent<Image>();
                //if (mig != null && mig.sprite != null)
                //{
                //    mig.enabled = true;
                //}
            }
        }

        private void doConfig() {

            cfg_donothing =
                Config.Bind("Good Afterevening",
                            "Do Nothing",
                            false,
                            "In case you want to disable the mod live");
        }
    }
}
