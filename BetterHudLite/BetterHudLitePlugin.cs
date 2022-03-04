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
    [BepInPlugin("com.TheTimeSweeper.BetterHudLite", "BetterHudLite", "0.1.2")]
    //[R2API.Utils.NetworkCompatibility(R2API.Utils.CompatibilityLevel.NoNeedForSync)]
    public class BetterHudLitePlugin : BaseUnityPlugin {

        public static BetterHudLitePlugin instance;

        void Awake() {

            instance = this;

            Confug.doConfig();
            
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

                //separate sprint and tab screen icons, keeping them in their corner
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

                //move skills over
                skillsScaler.transform.SetParent(self.mainUIPanel.transform.Find("SpringCanvas/BottomCenterCluster"));// barRoots);
                //skillsScaler.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
                skillsScaler.rotation = Quaternion.identity;
                skillsScaler.pivot = new Vector2(0.5f, 0.0f);

                skillsScaler.sizeDelta = new Vector2(-639, -234);
                float barsHeight = Confug.healthBarHeight - 1;
                float skillHeightFactor = 72 * 0.25f;
                skillsScaler.anchoredPosition = new Vector2(60, 80 + barsHeight * skillHeightFactor);
                //presto! I bet you've never seen so much magic in your life!

                //move up the skill slot fields to give the section room to move lower
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
                #region component migration

                GraphicRaycaster graphicRaycasterOld = bottomRightCluster.GetComponent<GraphicRaycaster>();
                GraphicRaycaster graphicRaycasterNew = bottomCenterCluster.gameObject.AddComponent<GraphicRaycaster>();
                graphicRaycasterNew.blockingObjects = graphicRaycasterOld.blockingObjects;
                graphicRaycasterNew.ignoreReversedGraphics = graphicRaycasterOld.ignoreReversedGraphics;
                graphicRaycasterNew.useGUILayout = graphicRaycasterOld.useGUILayout;
                #endregion
                #endregion skills

                //skills are now covering this
                #region notif area (item blurbs)

                RectTransform notifArea = (RectTransform)self.mainUIPanel.transform.parent.Find("NotificationArea");
                notifArea.anchorMin = new Vector2(0.8f, 0.05f);
                notifArea.anchorMax = new Vector2(0.8f, 0.05f);
                #endregion 

                #region spectator label

                RectTransform spec = (RectTransform)bottomCenterCluster.Find("SpectatorLabel");
                spec.anchoredPosition = new Vector2(0 , 150);
                #endregion 

            }

            if (!Confug.doBar && !Confug.doSkills)
            {
                Logger.LogMessage("bruh you just downloaded a mod and disabled the only two things it does");
            }
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
        private static ConfigEntry<bool> _doBar;
        public static bool doBar
        {
            get
            {
                reloadConfig();
                return _doBar.Value;
            }
        }

        private static ConfigEntry<bool> _doSkills;
        public static bool doSkills
        {
            get
            {
                reloadConfig();
                return _doSkills.Value;
            }
        }

        public static float healthBarWidth = 1;
        public static float healthBarHeight = 1;

        private static BetterHudLitePlugin plugin => BetterHudLitePlugin.instance;

        private static void reloadConfig()
        {
            plugin.Config.Reload();
        }

        public static void doConfig()
        {
            string barSection = "Health UI";

            _doBar =
                plugin.Config.Bind<bool>(barSection,
                        "Do the health bar",
                        true,
                        "Brings the health and level bar to the center");

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

            _doSkills =
                plugin.Config.Bind<bool>(skillsSection,
                        "Do the skills",
                        true,
                        "Brings the skills to the center above where the health bar is (if that's also enabled)");

            
        }
    }
}