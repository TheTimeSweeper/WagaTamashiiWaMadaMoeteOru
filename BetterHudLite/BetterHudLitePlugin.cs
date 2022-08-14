using BepInEx;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace BetterHudLite {

    [BepInPlugin("com.TheTimeSweeper.BetterHudLite", "BetterHudLite", "0.1.4")]
    //[R2API.Utils.NetworkCompatibility(R2API.Utils.CompatibilityLevel.NoNeedForSync)] time to add this back?
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

            if (Confug.doBar) {
                self.gameObject.AddComponent<HealthHudHandler>().Init(self);
            }

            if (Confug.doSkills) {
                self.gameObject.AddComponent<SkillsHudHandler>().Init(self);
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
}