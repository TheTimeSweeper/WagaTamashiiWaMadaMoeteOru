using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace SillyGlasses
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.SillyItem", "Silly Items", "1.0.0")]
    public class SillyGlassesPlugin : BaseUnityPlugin
    {
        public delegate void UpdateItemDisplayEvent(CharacterModel self, Inventory inventory);
        public delegate void EnableDisableDisplayEvent(CharacterModel self, ItemIndex itemIndex);

        public delegate void UpdateMaterialsEvent(CharacterModel self);
        public delegate void UpdateCameraEvent(CharacterModel self, CameraRigController cameraRig);

        public EnableDisableDisplayEvent enableDisableDisplayEvent;
        public UpdateItemDisplayEvent updateItemDisplayEvent;

        public UpdateMaterialsEvent updateMaterialsEvent;
        public UpdateCameraEvent updateCameraEvent;

        private List<CharacterSwooceHandler> _swooceHandlers = new List<CharacterSwooceHandler>();

        //test spawn items
        private int _currentRandomIndex;
        private int _spawnedRandomItems;
        private int _currentRedIndex;
        private int _spawnedRedItems;

        private string[] _engiMinionNames = new string[] {
            "EngiTurretBody",
            "EngiWalkerTurretBody",
            };
        private string[] _scavGuyNames = new string[] {
            "ScavBody",
            "ScavLunar1Body",
            "ScavLunar2Body",
            "ScavLunar3Body",
            "ScavLunar4Body",
            };

        public void Awake()
        {
            InitConfig();

            On.RoR2.CharacterBody.OnInventoryChanged += InvChangedHook;

            On.RoR2.CharacterModel.UpdateItemDisplay += UpdateItemDisplayHook;

            On.RoR2.CharacterModel.EnableItemDisplay += EnableItemDisplayHook;

            On.RoR2.CharacterModel.DisableItemDisplay += DisableItemDisplayHook;

            On.RoR2.CharacterModel.UpdateMaterials += UpdateMaterialsHook;

            On.RoR2.CharacterModel.UpdateForCamera += UpdateForCameraHook;

            On.RoR2.CharacterModel.RefreshObstructorsForCamera += RefreshObstructorsForCameraHook;
        }

        #region config

        private void InitConfig() {
            string sectionName = "hope youre having a lovely day";

            //ConfigDefinition moveSpeedDef = new ConfigDefinition(sectionName, "Set Move Speed");
            //ConfigDescription moveSpeedDesc = new ConfigDescription("Damage to Set");
            ///ConfigEntry<float> MultAttackSpeed;

            //SetDamage = Config.Bind<float>(AttackSpeedDef, 8.5f, AttackSpeedDesc);
            ///Utils.Cfg_Int_ItemStackMax = SetMoveSpeed.Value;
            ///

            Utils.Cfg_ItemStackMax = AbboosBrandBindConfig(sectionName,
                                                           "ItemStackMax",
                                                           "Maximum item displays that can be spawned (-1 for infinite).", 
                                                           2);

#pragma warning disable CS0618 // Type or member is obsolete
            Utils.Cfg_ItemDistanceMultiplier =
                Config.Wrap(sectionName,
                            "ItemDistanceMultiplier",
                            "The distance between extra displays that spawns.",
                            0.0420f).Value;

            Utils.Cfg_EngiTurretItemDistanceMultiplier =
                Config.Wrap(sectionName,
                            "EngiTurretItemDistanceMultiplier",
                            "Items are a little bigger on Engis Turrets. Spread them out a bit more.",
                            1.5f).Value;

            Utils.Cfg_ScavengerItemDistanceMultiplier =
                Config.Wrap(sectionName,
                            "ScavItemDistanceMultiplier",
                            "Items are a little bigger on Scavengers. Spread them out just a tiny bit maybe.",
                            6f).Value;

            Utils.Cfg_UseLogs =
                Config.Wrap(sectionName,
                            "Output Logs",
                            "because I keep forgetting to remove logs from my builds haha woops.",
                            false).Value;

            string cheatSection = "liar liar plants for hire";

            Utils.Cfg_PlantsForHire =
                Config.Wrap(cheatSection,
                            "Cheats",
                            "Press f2 f3 f6 and f9 to rain items from the sky.",
                            false).Value;

            Utils.Cfg_CheatItem =
                Config.Wrap(cheatSection,
                            "Cheat Item",
                            "Press f7 to spawn this item (glasses are 7)",
                            7).Value;

            Utils.Cfg_CheatItemBoring =
                Config.Wrap(cheatSection,
                            "Cheat Item2",
                            "Press f11 and f10 to add/remove this item boringly (58 for magazines)",
                            58).Value;
        }

        private T AbboosBrandBindConfig<T>(string sectionName, string keyname, string description, T def) {

            ConfigDefinition itemStackMaxDef = new ConfigDefinition(sectionName, keyname);
            ConfigDescription itemstackMaxDesc = new ConfigDescription(description);

            ConfigEntry<T> setItemStackMax = Config.Bind<T>(itemStackMaxDef, def, itemstackMaxDesc);

            return setItemStackMax.Value;
        }

        #endregion

        private void InvChangedHook(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            _swooceHandlers.TrimExcess();

            bool isEngineerTurret = false;
            bool isScavenger = false;

            for (int i = 0; i < _engiMinionNames.Length; i++) {

                int bodyIndex = self.bodyIndex;
                int engiBodyIndex = BodyCatalog.FindBodyIndex(_engiMinionNames[i]);

                if (bodyIndex == engiBodyIndex) {
                    isEngineerTurret = true;
                }
            }
            for (int i = 0; i < _scavGuyNames.Length; i++) {

                if (self.bodyIndex == BodyCatalog.FindBodyIndex(_scavGuyNames[i])) {
                    isScavenger = true;
                }
            }

            if (self.isPlayerControlled || isEngineerTurret || isScavenger)
            {
                if (self.characterMotor == null)
                {
                    Utils.Log("did it bug?", true);
                    Utils.Log("if not", true);
                    Utils.Log("WE DID IT BOIS", true);
                    Utils.Log("DIO'S IS SAFE", true);
                    Utils.Log(Environment.StackTrace);
                    //there's still a nullref tho i gotta check out
                    return;
                }
                
                if (self.characterMotor.gameObject.GetComponent<CharacterSwooceHandler>() == null)
                {
                    CharacterSwooceHandler swooceManager = self.hurtBoxGroup.gameObject.AddComponent<CharacterSwooceHandler>();
                    _swooceHandlers.Add(swooceManager);
                    swooceManager.Init(this, isEngineerTurret, isScavenger);
                }
            }

            orig(self);
        }

        public void UpdateItemDisplayHook(On.RoR2.CharacterModel.orig_UpdateItemDisplay orig,
                                          CharacterModel self,
                                          Inventory inventory)
        {
            updateItemDisplayEvent?.Invoke(self, inventory);
            
            orig(self, inventory);
        }

        private void DisableItemDisplayHook(On.RoR2.CharacterModel.orig_DisableItemDisplay orig, 
                                            CharacterModel self, 
                                            ItemIndex itemIndex)
        {
            orig(self, itemIndex);

            enableDisableDisplayEvent?.Invoke(self, itemIndex);
        }

        private void EnableItemDisplayHook(On.RoR2.CharacterModel.orig_EnableItemDisplay orig, 
                                           CharacterModel self, 
                                           ItemIndex itemIndex)
        {
            orig(self, itemIndex);

            enableDisableDisplayEvent?.Invoke(self, itemIndex);
        }

        private void UpdateMaterialsHook(On.RoR2.CharacterModel.orig_UpdateMaterials orig, CharacterModel self) {

            orig(self);

            updateMaterialsEvent?.Invoke(self);
        }

        private void UpdateForCameraHook(On.RoR2.CharacterModel.orig_UpdateForCamera orig, CharacterModel self, CameraRigController cameraRigController) {

            orig(self, cameraRigController);

            updateCameraEvent?.Invoke(self, cameraRigController);
        }

        private void RefreshObstructorsForCameraHook(On.RoR2.CharacterModel.orig_RefreshObstructorsForCamera orig, CameraRigController cameraRigController) {

            orig(cameraRigController);

            Vector3 position = cameraRigController.transform.position;

            _swooceHandlers.TrimExcess();

            for (int i = 0; i < _swooceHandlers.Count; i++) {

                if (_swooceHandlers[i] == null)
                    continue;

                CharacterSwooceHandler swooceHandler = _swooceHandlers[i];

                if (cameraRigController.enableFading) {
                    float nearestHurtBoxDistance = (position - swooceHandler.swoocedCharacterModel.transform.position).magnitude;
                    swooceHandler.pseudoFade = Mathf.Clamp01(Util.Remap(nearestHurtBoxDistance, cameraRigController.fadeStartDistance, cameraRigController.fadeEndDistance, 0f, 1f));
                } else {
                    swooceHandler.pseudoFade = 1f;
                }
            }
        }

        #region cheats
        public void Update()
        {
            if (Utils.Cfg_PlantsForHire)
            {
                if (Input.GetKeyDown(KeyCode.F2))
                {
                    TestSpawnItem((int)ItemIndex.CritGlasses);
                }

                if (Input.GetKeyDown(KeyCode.F3))
                {
                    Chat.AddMessage("kek");
                    TestSpawnItem((int)ItemIndex.TreasureCache);
                }

                if (Input.GetKeyDown(KeyCode.F6))
                {
                    TestSpawnItem();
                }

                if (Input.GetKeyDown(KeyCode.F7))
                {
                    TestSpawnItem(Utils.Cfg_CheatItem);
                }

                if (Input.GetKeyDown(KeyCode.F9))
                {
                    TestSpawnItemRed();
                }
            }
        }

        private void TestSpawnItem(int item = -1)
        {
            List<PickupIndex> dropList = Run.instance.availableTier1DropList;
            TestSpawnItem(dropList, item, true);
        }

        private void TestSpawnItemRed()
        {
            List<PickupIndex> dropList = Run.instance.availableTier3DropList;
            TestSpawnItem(dropList, -1, false);
        }

        private void TestSpawnItem(List<PickupIndex> dropList, int item = -1, bool random = true)
        {
            Transform transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

            PickupIndex pickupIndex = new PickupIndex();

            if (item > -1)
            {
                pickupIndex = new PickupIndex((ItemIndex)item);
            }
            else
            {
                if (dropList.Count < 0)
                    return;

                if (random)
                {
                    if (_spawnedRandomItems >= 3)
                    {
                        //Randomly get the next item:
                        _currentRandomIndex = Run.instance.treasureRng.RangeInt(0, dropList.Count);
                        _spawnedRandomItems = 1;
                    }
                    else
                    {
                        _spawnedRandomItems++;
                    }

                    pickupIndex = dropList[_currentRandomIndex];
                }
                else
                {

                    _spawnedRedItems++;
                    if (_spawnedRedItems >= 3)
                    {
                        _currentRedIndex++;
                        _spawnedRedItems = 0;
                    }

                    if (_currentRedIndex >= dropList.Count)
                    {
                        _currentRedIndex = 1;
                    }

                    pickupIndex = dropList[_currentRedIndex];
                }
            }

            Vector3 dir = UnityEngine.Random.insideUnitSphere;
            dir.y = Mathf.Abs(dir.y * 3);
            dir /= 3;

            PickupDropletController.CreatePickupDroplet(pickupIndex, transform.position + Vector3.up * 69, dir * 20f);
        }
        #endregion

    }
}