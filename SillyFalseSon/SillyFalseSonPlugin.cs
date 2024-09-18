using BepInEx;
using BepInEx.Configuration;
using RoR2.UI;
using RoR2;
using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using EntityStates.BrotherMonster;
using System.Security;
using System.Security.Permissions;


[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace SillyFalseSon
{
    [BepInPlugin("com.TheTimeSweeper.SillyFalseSon", "Silly False Son", "2.0.0")]
    public class SillyFalseSonPlugin : BaseUnityPlugin
    {
        private const string configSection = "hi";
        private ConfigEntry<float> cfg_funnySize;
        private ConfigEntry<float> cfg_displaySizeMulti;
        private ConfigEntry<float> cfg_hitboxSize;
        private ConfigEntry<float> cfg_cameraBack;
        private ConfigEntry<float> cfg_cameraHigh;
        private ConfigEntry<bool> cfg_dashOnBody;
        private ConfigEntry<int> cfg_extraStockDisplays;
        private ConfigEntry<bool> cfg_slamMovementTweaks;


        internal static SillyFalseSonPlugin instance;

        private float _originalClubGroundSlamMinimumDuration;

        void Awake()
        {
            instance = this;

            cfg_funnySize = Configuration.BindAndOptions(configSection, "In-game size", 1.12f, "go nuts", true);
            cfg_displaySizeMulti = Configuration.BindAndOptions(configSection, "CSS size multiplier", 1f, "vanilla is 0.7 * his in-game size, which is why I thought he was small and made this mod but he's actually kinda big. keeping the big funny size anyways lol", true);
            cfg_hitboxSize = Configuration.BindAndOptions(configSection, "hitbox size", 9f, "relative to size. if you make him really small you should crank this up. swing effects will not be affected cause I can't be arsed", true);
            cfg_cameraBack = Configuration.BindAndOptions(configSection, "camera back", 13f, "how far back the camera is pulled dout. default false son is 13. pull it back or forward if you're making him larger or smaller respectively", true);
            cfg_cameraHigh = Configuration.BindAndOptions(configSection, "camera high", 1f, "how far higher the camera is raised. I forget what default is.", true);
            cfg_dashOnBody = Configuration.BindAndOptions(configSection, "dash while attacking", true, "puts dash on the body state machine. allowing you to dash while using other abilities", true);
            cfg_extraStockDisplays = Configuration.BindAndOptions(
                configSection,
                "Extra Stock Displays",
                20, 0, 100,
                "added stock displays to crosshair for secondary stocks. if it's not a multiple of 4 you're a felon",
                true);
            cfg_slamMovementTweaks = Configuration.BindAndOptions(
                configSection,
                "Slam movement Tweaks",
                true,
                "jump arc of the m1+m2 changes:\ninstantly activate when landing on the ground consistently\ninitial jump arc adjusted\nninitial jump arc no longer affected by attack speed",
                true);

            RoR2Application.onLoad += OnLoad;

            if (cfg_slamMovementTweaks.Value)
            {
                On.EntityStates.FalseSon.ClubGroundSlam.OnEnter += ClubGroundSlam_OnEnter;
                On.EntityStates.FalseSon.ClubGroundSlam.OnExit += ClubGroundSlam_OnExit;

                On.EntityStates.FalseSon.PreClubGroundSlam.OnEnter += PreClubGroundSlam_OnEnter;
                On.EntityStates.FalseSon.PreClubGroundSlam.FixedUpdate += PreClubGroundSlam_FixedUpdate;
            }
        }

        private void PreClubGroundSlam_OnEnter(On.EntityStates.FalseSon.PreClubGroundSlam.orig_OnEnter orig, EntityStates.FalseSon.PreClubGroundSlam self)
        {
            EntityStates.FalseSon.PreClubGroundSlam.upwardVelocity = 22f;//magic numbers from testing that don't need to be in config
            Logger.LogWarning(EntityStates.FalseSon.PreClubGroundSlam.baseDuration);
            orig(self);
            self.duration = 0.3f;
        }

        private void PreClubGroundSlam_FixedUpdate(On.EntityStates.FalseSon.PreClubGroundSlam.orig_FixedUpdate orig, EntityStates.FalseSon.PreClubGroundSlam self)
        {
            orig(self);

            ref float ySpeed = ref self.characterMotor.velocity.y;
            ySpeed += Physics.gravity.y * 2.5f * Time.deltaTime;

        }

        private void ClubGroundSlam_OnEnter(On.EntityStates.FalseSon.ClubGroundSlam.orig_OnEnter orig, EntityStates.FalseSon.ClubGroundSlam self)
        {
            _originalClubGroundSlamMinimumDuration = EntityStates.FalseSon.ClubGroundSlam.minimumDuration;
            if (!self.characterMotor.isGrounded)
            {
                EntityStates.FalseSon.ClubGroundSlam.minimumDuration = 0;
            }
            orig(self);
        }

        private void ClubGroundSlam_OnExit(On.EntityStates.FalseSon.ClubGroundSlam.orig_OnExit orig, EntityStates.FalseSon.ClubGroundSlam self)
        {
            orig(self);
            EntityStates.FalseSon.ClubGroundSlam.minimumDuration = _originalClubGroundSlamMinimumDuration;
        }

        private void OnLoad()
        {
            DoEverything();
        }

        private void DoEverything()
        {
            CharacterBody funnyGuy = BodyCatalog.FindBodyPrefab("FalseSonBody").GetComponent<CharacterBody>();
            Transform modelTransform = funnyGuy.GetComponentInChildren<CharacterModel>().transform;
            modelTransform.localScale = Vector3.one * cfg_funnySize.Value;
            modelTransform.Find("ClubHitBox").localScale = new Vector3(1, 1.1f, 1) * cfg_hitboxSize.Value;
            modelTransform.Find("ClubHitBox").localPosition = new Vector3(0, 2, 3);

            SurvivorCatalog.FindSurvivorDefFromBody(funnyGuy.gameObject).displayPrefab.transform.GetChild(0).localScale = Vector3.one * cfg_funnySize.Value * cfg_displaySizeMulti.Value;

            var camera = funnyGuy.GetComponent<CameraTargetParams>().cameraParams;
            camera.data.idealLocalCameraPos = new Vector3(0, cfg_cameraHigh.Value, -cfg_cameraBack.Value);
            camera.data.pivotVerticalOffset = 1f;

            //why didn't i just addressables this lol
            EntityStates.FalseSon.ClubSwing? clubSwing = EntityStateCatalog.InstantiateState(typeof(EntityStates.FalseSon.ClubSwing)) as EntityStates.FalseSon.ClubSwing;
            var nip = clubSwing.swingEffectPrefab;
            nip.transform.GetChild(0).localScale = Vector3.one * 0.8f;
            nip.transform.GetChild(0).localPosition = new Vector3(0, 0, -2.5f);
            nip = EntityStates.FalseSon.ClubSwing.secondarySwingEffectPrefab;
            nip.transform.GetChild(0).localScale = Vector3.one * 0.8f;
            nip.transform.GetChild(0).localPosition = new Vector3(0, 1, -2.5f);

            GameObject falseSonGroundSlaem = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/FalseSon/FalseSonGroundSlam.prefab").WaitForCompletion();
            falseSonGroundSlaem.transform.Find("Debris/Spikes, Small").localScale = new Vector3(3.5f, 3.5f, 2);
            falseSonGroundSlaem.transform.Find("Debris/Particle System").localScale = new Vector3(2, 2, 2);
            falseSonGroundSlaem.GetComponent<DestroyOnTimer>().duration = 2;

            if (cfg_dashOnBody.Value)
            {
                funnyGuy.GetComponents<GenericSkill>().First((skill) => { return skill.skillName == "StepBrothers"; }).skillFamily.variants[0].skillDef.activationStateMachineName = "Body";
            }

            AddStockStoCrosshair(funnyGuy);
        }

        private void AddStockStoCrosshair(CharacterBody funnyGuy)
        {
            if (cfg_extraStockDisplays.Value <= 0)
                return;

            Transform crosshair = R2API.PrefabAPI.InstantiateClone(funnyGuy.defaultCrosshairPrefab, funnyGuy.defaultCrosshairPrefab.name, false).transform;

            CrosshairController crosshairController = crosshair.GetComponent<CrosshairController>();
            CrosshairController.SkillStockSpriteDisplay stockDisplay = crosshairController.skillStockSpriteDisplays[0];

            Queue<Transform> fillQueue = new Queue<Transform>();
            fillQueue.Enqueue(crosshair.Find("Holder/TR/TRFill"));
            fillQueue.Enqueue(crosshair.Find("Holder/BR/BRFill"));
            fillQueue.Enqueue(crosshair.Find("Holder/BL/BLFill"));
            fillQueue.Enqueue(crosshair.Find("Holder/TL/TLFill"));

            List<CrosshairController.SkillStockSpriteDisplay> displays = new List<CrosshairController.SkillStockSpriteDisplay>();
            displays.AddRange(crosshairController.skillStockSpriteDisplays);

            for (int i = 0; i < cfg_extraStockDisplays.Value; i++)
            {
                Transform stockFillQueued = fillQueue.Dequeue();
                Transform stockFill = Instantiate(stockFillQueued.gameObject, stockFillQueued.parent).transform;
                stockFill.localPosition = new Vector3(0, stockFillQueued.localPosition.y + 20, 0);

                displays.Add(new CrosshairController.SkillStockSpriteDisplay
                {
                    target = stockFill.gameObject,
                    skillSlot = stockDisplay.skillSlot,
                    requiredSkillDef = stockDisplay.requiredSkillDef
                });

                fillQueue.Enqueue(stockFill);
            }

            for (int i = 0; i < displays.Count; i++)
            {
                CrosshairController.SkillStockSpriteDisplay display = displays[i];
                display.minimumStockCountToBeValid = i + 1;
                display.maximumStockCountToBeValid = 100;
                displays[i] = display;
            }
            crosshairController.skillStockSpriteDisplays = displays.ToArray();

            funnyGuy._defaultCrosshairPrefab = crosshair.gameObject;
        }
    }
}
