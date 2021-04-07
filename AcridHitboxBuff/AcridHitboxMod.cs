using BepInEx;
using EntityStates;
using RoR2;
using RoR2.Skills;
using System;
using System.Linq;
using UnityEngine;

namespace AcridHitbox {

    //TODO: see if only host needs it
    //[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod)] 
    //[BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.AcridHitboxBuff", "Acrid Slash Hitbox Buff", "1.1.0")]
    public class AcridHitboxMod : BaseUnityPlugin {

        //45, 42, 38,, 0, 13, 11
        private Vector3 cfg_newHitboxScale = new Vector3(45, 42, 38);
        private Vector3 cfg_newHtboxPosition = new Vector3(0, 13, 11);
        private bool cfg_keepVisuals;
        BodyIndex _crocoBod;

        public void Awake() {

            doConfig();
            
            //if i could do this directly to the prefab in resources that'd be swell
            //wheresthebeef(Resources.Load<GameObject>("prefabs/characterbodies/crocobody").GetComponent<CharacterBody>());

            On.RoR2.CharacterBody.Start += CharacterBody_Start;

            On.RoR2.BodyCatalog.Init += BodyCatalog_Init;
        }

        private void BodyCatalog_Init(On.RoR2.BodyCatalog.orig_Init orig) {
            orig();
            _crocoBod = BodyCatalog.FindBodyIndex("CrocoBody");
        }

        private void doConfig() {

            string sectionName = "if youre reading this ur cool";

            float scaleX = Config.Bind(sectionName,
                                       "hitbox local scale X (left/right)",
                                       45.0f,
                                       "orig is 34.8").Value;
            float scaleY = Config.Bind(sectionName,
                                       "hitbox local scale Y (forward/backward)",
                                       45.0f,
                                       "orig is 27.0").Value;
            float scaleZ = Config.Bind(sectionName,
                                       "hitbox local scale Z (up/down)",
                                       45.0f,
                                       "orig is 34.4").Value;
            //somehow testing and feeling out and trying different things landed on perfect cube without realizing

            cfg_newHitboxScale = new Vector3(scaleX, scaleY, scaleZ);

            float positX = Config.Bind(sectionName,
                                       "hitbox local position X (left/right)",
                                       0f,
                                       "left/right. orig is 0").Value;
            float positY = Config.Bind(sectionName,
                                       "local position Y (up/down)",
                                       11.0f,
                                       "orig is 13").Value;
            float positZ = Config.Bind(sectionName,
                                       "hitbox local position Z (forwards/backwards)",
                                       12.5f,
                                       "orig is 17.8").Value;

            cfg_newHtboxPosition = new Vector3(positX, positY, positZ);
             
            cfg_keepVisuals = Config.Bind(sectionName,
                                      "disable beef visuals",
                                      false,
                                      "keep original visuals rather than slightly pumped up versions to match bigger hitbox").Value;

        }

        private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self) { 

            orig(self);

            if (self.bodyIndex != _crocoBod) 
                return;

            wheresthebeef(self);
        }

        private void wheresthebeef(CharacterBody bod) {

            HitBoxGroup hitboxGroup = bod.modelLocator.modelTransform.GetComponent<HitBoxGroup>();

            if (hitboxGroup == null || hitboxGroup.groupName != "Slash") {
                Debug.LogWarning("couldn't get croco's slashHitbox. probably got changed?. aborting");
            }

            Transform hitboxTransform = hitboxGroup.hitBoxes[0].transform;
            hitboxTransform.localScale = cfg_newHitboxScale;
            hitboxTransform.localPosition = cfg_newHtboxPosition;

            if (cfg_keepVisuals)
                return;

            ChildLocator childLocator = bod.modelLocator.modelTransform.GetComponent<ChildLocator>();

            Transform slash1 = childLocator.FindChild("Slash1");
            slash1.localPosition = new Vector3(0, 10.26f, 1);
            slash1.localRotation = Quaternion.Euler(60, 300, 3.4f);
            slash1.localScale = new Vector3(1.2f, 1, 1);

            Transform slash2 = childLocator.FindChild("Slash2");
            slash2.localPosition = new Vector3(0, 10.26f, 1);
            slash2.localRotation = Quaternion.Euler(300, 247, 3.4f);
            slash2.localScale = new Vector3(1.2f, 1, 1);

            Transform slash3 = childLocator.FindChild("Slash3");
            slash3.localPosition = new Vector3(0, 10.26f, 6);
            slash3.localRotation = Quaternion.Euler(282, 180, 90);

            Transform mouth = childLocator.FindChild("MouthMuzzle");
            mouth.localScale = new Vector3(1.2f, 1, 1.2f);
        }
    }
}
