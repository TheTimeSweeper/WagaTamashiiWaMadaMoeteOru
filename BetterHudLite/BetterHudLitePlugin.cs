using BepInEx;
using BepInEx.Configuration;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace BetterHudLite {
    [BepInPlugin("com.TheTimeSweeper.BetterHudLite", "BetterHudLite", "0.1.0")]
    public class BetterHudLitePlugin : BaseUnityPlugin {

        void Awake() {
            Confug.doConfig(this);

            On.RoR2.UI.HUD.Awake += HUD_Awake;
        }

        private void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
        {
            orig(self);

            //ShowBoxesGodDang(self);

            RectTransform bottomRightCluster = (RectTransform)self.mainUIPanel.transform.Find("SpringCanvas/BottomRightCluster");
            RectTransform bottomCenterCluster = (RectTransform)self.mainUIPanel.transform.Find("SpringCanvas/BottomCenterCluster");

            if (Confug.doBar)
            {
                #region health bar
                //health bar
                RectTransform barRoots = (RectTransform)self.mainUIPanel.transform.Find("SpringCanvas/BottomLeftCluster/BarRoots");

                Transform buffs = barRoots.Find("LevelDisplayCluster/BuffDisplayRoot");
                buffs.SetParent(barRoots.parent);

                barRoots.SetParent(self.mainUIPanel.transform.Find("SpringCanvas/BottomCenterCluster"));

                //cuts down on ugly copypaste but it's a lot less readable
                //ArrangeRect(barRoots,
                //            new Vector2(0.5f, 0.25f),
                //            Vector2.zero,
                //            new Vector2(-432f + -432 * (-(Confag.healthBarWidth - 1)), Confag.healthBarHeight * 72f));

                barRoots.rotation = Quaternion.identity;
                barRoots.pivot = new Vector2(0.5f, 0.25f);
                barRoots.anchoredPosition = Vector2.zero;

                barRoots.sizeDelta = new Vector2(-432f + -432 * (-(Confug.healthBarWidth - 1)), Confug.healthBarHeight * 72f);

                //Transform lev = barRoots.Find("LevelDisplayCluster");
                //lev.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
                #endregion health bar
            }

            if (Confug.doSkills)
            {
                #region skills
                //skills
                RectTransform skillsScaler = (RectTransform)self.mainUIPanel.transform.Find("SpringCanvas/BottomRightCluster/Scaler");

                //separate sprint and tab icons, keeping them in their corner
                RectTransform sprint = (RectTransform)skillsScaler.Find("SprintCluster");
                sprint.SetParent(skillsScaler.parent);
                sprint.anchoredPosition = new Vector2(-100, 0);
                sprint.anchorMin = new Vector2(1, 0.5f);
                sprint.anchorMax = new Vector2(1, 0.5f);

                RectTransform inve = (RectTransform)skillsScaler.Find("InventoryCluster");
                inve.SetParent(skillsScaler.parent);
                inve.anchoredPosition = new Vector2(-146, 0);
                inve.anchorMin = new Vector2(1, 0.5f);
                inve.anchorMax = new Vector2(1, 0.5f);

                skillsScaler.transform.SetParent(self.mainUIPanel.transform.Find("SpringCanvas/BottomCenterCluster"));// barRoots);

                //skillsScaler.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;

                skillsScaler.rotation = Quaternion.identity;
                skillsScaler.pivot = new Vector2(0.5f, 0.0f);

                skillsScaler.sizeDelta = new Vector2(-639, -234);

                float barsHeight = Confug.healthBarHeight - 1;
                float skillHeightFactor = 72 * 0.25f;

                skillsScaler.anchoredPosition = new Vector2(60, 80 + barsHeight * skillHeightFactor);
                //presto! I bet you've never seen so much magic in your life!

                //move up the "M1" "M2" etc fields to give the section room to move lower
                Transform rect;
                Image im;
                foreach (InputBindingDisplayController bindingDisplay in skillsScaler.GetComponentsInChildren<InputBindingDisplayController>())
                {
                    rect = bindingDisplay.transform.parent;
                    Vector3 anch = rect.localPosition;
                    anch.y += 9;
                    rect.localPosition = anch;

                    im = rect.GetComponent<Image>();
                    Color col = im.color;
                    col.a = 0.76f;
                    im.color = col;
                }

                //Fixing tooltips
                // dunno what's required so im just copying everything
                #region component migration
                
                //IDK if this part is required
                var cOld = bottomRightCluster.GetComponent<Canvas>();
                var cNew = bottomCenterCluster.gameObject.AddComponent<Canvas>();
                cNew.additionalShaderChannels = cOld.additionalShaderChannels;
                //cNew.name = cOld.name;
                cNew.normalizedSortingGridSize = cOld.normalizedSortingGridSize;
                cNew.overridePixelPerfect = cOld.overridePixelPerfect;
                cNew.overrideSorting = cOld.overrideSorting;
                cNew.pixelPerfect = cOld.pixelPerfect;
                cNew.planeDistance = cOld.planeDistance;
                cNew.referencePixelsPerUnit = cOld.referencePixelsPerUnit;
                cNew.renderMode = cOld.renderMode;
                cNew.scaleFactor = cOld.scaleFactor;
                cNew.sortingLayerID = cOld.sortingLayerID;
                cNew.sortingLayerName = cOld.sortingLayerName;
                cNew.sortingOrder = cOld.sortingOrder;
                cNew.tag = cOld.tag;
                cNew.targetDisplay = cOld.targetDisplay;
                cNew.worldCamera = cOld.worldCamera;


                //this one definitely is
                var grOld = bottomRightCluster.GetComponent<GraphicRaycaster>();
                var grNew = bottomCenterCluster.gameObject.AddComponent<GraphicRaycaster>();
                grNew.blockingObjects = grOld.blockingObjects;
                grNew.hideFlags = grOld.hideFlags;
                grNew.ignoreReversedGraphics = grOld.ignoreReversedGraphics;
                //grNew.name = grOld.name;
                grNew.tag = grOld.tag;
                grNew.useGUILayout = grOld.useGUILayout;
                #endregion component migration

                #region move notif
                var notifArea = (RectTransform)self.mainUIPanel.transform.parent.Find("NotificationArea");
                //fix this
                notifArea.position += Vector3.up * 2f;
                #endregion move notif

                #region movespec
                var spec = (RectTransform)bottomCenterCluster.Find("SpectatorLabel");
                //fix this too
                spec.position += Vector3.up * 0.4f;
                #endregion movespec

                #endregion skills
            }

            if (!Confug.doBar && !Confug.doSkills)
            {
                Logger.LogMessage("bruh you just downloaded a mod and disabled the only two things it does");
            }
        }

        private void ArrangeRect(RectTransform rect, Vector2 pivot, Vector2 AnchoredPosition, Vector2 sizeDelta)
        {
            rect.rotation = Quaternion.identity;
            rect.pivot = pivot;
            rect.anchoredPosition = AnchoredPosition;
            rect.sizeDelta = sizeDelta;
        }

        //debog
        private void ShowBoxesGodDang(HUD hud)
        {
            Sprite bg = hud.mainUIPanel.transform
                .Find("SpringCanvas/BottomRightCluster/Scaler/Outline")
                .GetComponent<Image>().sprite;

            Image mig;

            foreach (RectTransform rect in hud.mainUIPanel.GetComponentsInChildren<RectTransform>())
            {
                if (rect.GetComponent<Graphic>() == null)
                {
                    mig = rect.gameObject.AddComponent<Image>();
                    mig.sprite = bg;
                    mig.type = Image.Type.Sliced;
                }

                //mig = rect.GetComponent<Image>();
                //if (mig != null && mig.sprite != null)
                //{
                //    mig.enabled = true;
                //}
            }
        }
    }

    public class Confug
    {
        public static float healthBarWidth = 1;
        public static float healthBarHeight = 1;

        public static bool doBar;
        public static bool doSkills;

        public static void doConfig(BaseUnityPlugin plugin)
        {
            string barSection = "Health UI";

            doBar =
                plugin.Config.Bind<bool>(barSection,
                        "Do the health bar",
                        true,
                        "Brings the health and level bar to the center").Value;

            healthBarWidth =
                plugin.Config.Bind<float>(barSection,
                        "Health Bar Width Factor",
                        1f,
                        "size of health bar, range 0 to 4.5, 0 making it nonexistent, 4.5 making it cover the screen\n"
                        + "less than 0 just shifts it to the right, higher than 4.5 goes off screen obviously").Value;

            healthBarHeight =
                plugin.Config.Bind<float>(barSection,
                        "Health Bar Height Multiplier",
                        1f,
                        "multiply health bar height by this value\n"
                        + "expands both up and down. attempts to move skills with it").Value;

            string skillsSection = "Skills UI";

            doSkills =
                plugin.Config.Bind<bool>(skillsSection,
                        "Do the skills",
                        true,
                        "Brings the skills to the center above where the health bar is (if that's also enabled)").Value;
        }
    }
}