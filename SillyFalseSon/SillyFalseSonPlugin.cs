using BepInEx;
using BepInEx.Configuration;
using RoR2;
using System;
using UnityEngine;

namespace SillyFalseSon
{
    [BepInPlugin("com.TheTimeSweeper.SillyFalseSon", "Silly False Son", "1.0.0")]
    public class SillyFalseSonPlugin : BaseUnityPlugin
    {
        private const string Section = "hi 1.0.1";
        private ConfigEntry<float> funnySize;
        private ConfigEntry<float> hitboxSize;
        private ConfigEntry<float> CameraBack;
        private ConfigEntry<float> CameraHigh;
        internal static SillyFalseSonPlugin instance;

        void Awake()
        {
            instance = this;

            funnySize = Configuration.BindAndOptions(Section, "size", 1.12f, "go nuts", true);
            hitboxSize = Configuration.BindAndOptions(Section, "hitbox size", 9f, "if you make him really small you should crank this up. swing effects will not be affected", true);
            CameraBack = Configuration.BindAndOptions(Section, "camera back", 14f, "how far back the camera is pulled dout. default false son is 13", true);
            CameraHigh = Configuration.BindAndOptions(Section, "camera high", 1.2f, "how far higher the camera is raised", true);

            RoR2.RoR2Application.onLoad += nip;
        }

        private void nip()
        {
            DoEverything();
        }

        private void DoEverything()
        {
            CharacterBody funnyGuy = BodyCatalog.FindBodyPrefab("FalseSonBody").GetComponent<CharacterBody>();
            Transform modelTransform = funnyGuy.GetComponentInChildren<CharacterModel>().transform;
            modelTransform.localScale = Vector3.one * funnySize.Value;
            modelTransform.Find("ClubHitBox").localScale = new Vector3(1,1.1f,1) * hitboxSize.Value;
            modelTransform.Find("ClubHitBox").localPosition = new Vector3(0, 2, 3);

            var camera = funnyGuy.GetComponent<CameraTargetParams>().cameraParams;
            camera.data.idealLocalCameraPos = new Vector3(0, CameraHigh.Value, -CameraBack.Value);
            camera.data.pivotVerticalOffset = 1f;

            SurvivorCatalog.FindSurvivorDefFromBody(funnyGuy.gameObject).displayPrefab.transform.localScale *= funnySize.Value * 1.2f;

            EntityStates.FalseSon.ClubSwing? clubSwing = EntityStateCatalog.InstantiateState(typeof(EntityStates.FalseSon.ClubSwing)) as EntityStates.FalseSon.ClubSwing;
            var nip = clubSwing.swingEffectPrefab;
            nip.transform.GetChild(0).localScale = Vector3.one * 0.8f;
            nip.transform.GetChild(0).localPosition = new Vector3(0, 0, -2.5f);
            nip = EntityStates.FalseSon.ClubSwing.secondarySwingEffectPrefab;
            nip.transform.GetChild(0).localScale = Vector3.one * 0.8f;
            nip.transform.GetChild(0).localPosition = new Vector3(0, 1, -2.5f);
        }
    }
}
