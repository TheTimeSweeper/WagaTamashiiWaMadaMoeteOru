﻿using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
//using R2API.Utils;

using System.Security;
using System.Security.Permissions;

//can't believe I didn't have this til now
//can I do a bunch of shit i didn't know i could do with this?
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//[assembly: ManualNetworkRegistration]
namespace SillyGlasses {
#region sillydisplayrules
    public class SillyItemDisplayRules : List<SillyItemDisplayRule> {
        
    }

    public enum SillyItemDisplayBehavior {
        DEFAULT,
        DEFAULT_BOTH_WAYS,
        OUTWARD,
        SCATTER,
        //SPIRAL
        //GROW
    }

    public class SillyItemDisplayRule {
        public string character;

        public ItemIndex item; 

        public Vector3 defaultStackDirection = Vector3.forward;
        //public Vector3 rotationShift;
        public float distanceMult = 1;
        public SillyItemDisplayBehavior stackBehavior;
    }
    #endregion

    //[NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    //[BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.SillyItem", "Silly Items", "1.2.3")]
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

        private Inventory _copiedItemsInventory;

        private string[] _scavGuyNames = new string[] {
            "ScavBody",
            "ScavLunar1Body",
            "ScavLunar2Body",
            "ScavLunar3Body",
            "ScavLunar4Body",
        };

        private string[] _moonGuyNames = new string[] {
            "BrotherBody",
            "BrotherGlassBody",
            "BrotherHauntBody",
            "BrotherHurtBody",
            "MithrixBody",
        };

        #region cheats
        //test spawn items
        private int _currentRandomIndex;
        private int _spawnedRandomItems;
        private int _currentRedIndex;
        private int _spawnedRedItems;
        #endregion

        public void Awake() {

            InitConfig();

            Utils.CacheReflection();

            On.RoR2.Inventory.CopyItemsFrom_Inventory += Inventory_CopyItemsFrom_Inventory; ;

            On.RoR2.CharacterBody.OnInventoryChanged += InvChangedHook;

            On.RoR2.CharacterModel.UpdateItemDisplay += UpdateItemDisplayHook;

            On.RoR2.CharacterModel.EnableItemDisplay += EnableItemDisplayHook;

            On.RoR2.CharacterModel.DisableItemDisplay += DisableItemDisplayHook;

            On.RoR2.CharacterModel.UpdateMaterials += UpdateMaterialsHook;
        }

        //check if this inventory is a turret
        private void Inventory_CopyItemsFrom_Inventory(On.RoR2.Inventory.orig_CopyItemsFrom_Inventory orig, Inventory self, Inventory other) {

            CharacterBody copiedItemsBody;

            for (int i = 0; i < _swooceHandlers.Count; i++) {

                if (_swooceHandlers[i] != null && _swooceHandlers[i].swoocedCharacterModel != null) {

                    copiedItemsBody = _swooceHandlers[i].swoocedCharacterModel.body;

                    if (copiedItemsBody.inventory == other) {

                        _copiedItemsInventory = self;
                        break;
                    }
                }
            }

            orig(self, other);
        }

        #region config

        private void InitConfig() {
            string sectionName = "hope youre having a lovely day";

            Utils.Cfg_ItemStackMax = 
                Config.Bind(sectionName,
                            "ItemStacksMax",
                            -1, 
                            "Maximum item displays that can be spawned (-1 for infinite).").Value;

            Utils.Cfg_ClassicStackType =
                Config.Bind(sectionName,
                            "Outward Stacking",
                            false,
                            "Makes item stacks outward from the bone it's attached to, rather than their object's forward facing direction.").Value;

            Utils.Cfg_ItemDistanceMultiplier =
                Config.Bind(sectionName,
                            "ItemDistanceMultiplier",
                            0.0480f,
                            "The distance between extra displays that spawns.").Value;
            Utils.Cfg_EngiTurretItemDistanceMultiplier =
                Config.Bind(sectionName,
                            "EngiTurretItemDistanceMultiplier",
                            1.5f,
                            "Items are a little bigger on Engis Turrets. Spread them out a bit more.").Value;

            Utils.Cfg_ScavengerItemDistanceMultiplier =
                Config.Bind(sectionName,
                            "ScavItemDistanceMultiplier",
                            6f,
                            "Items are a also bigger on Scavengers I think").Value;

            Utils.Cfg_BrotherItemDistanceMultiplier =
                Config.Bind(sectionName,
                            "BrotherItemDistanceMultiplier",
                            2f,
                            "Big Spikes.").Value;

            Utils.Cfg_UseLogs =
                Config.Bind(sectionName,
                            "Output Logs",
                            false,
                            "because I keep forgetting to remove logs from my builds haha woops.").Value;

            string cheatSection = "liar liar plants for hire";

            Utils.Cfg_PlantsForHire =
                Config.Bind(cheatSection,
                            "Cheats",
                            false,
                            "Press f2 f3 f6 and f9 to rain items from the sky.").Value;

            Utils.Cfg_CheatItem =
                Config.Bind(cheatSection,
                            "Cheat Item",
                            7,
                            "Press f7 to spawn this item (glasses are 7)").Value;

            Utils.Cfg_CheatItemBoring =
                Config.Bind(cheatSection,
                            "Cheat Item2",
                            58,
                            "Press f11 and f10 to add/remove this item boringly (58 for magazines)").Value;
        }

        #endregion

        //old hook
        private void CopyItemsHook(Inventory self, Inventory other) 
        {
            CharacterBody copiedItemsBody = null;

            for (int i = 0; i < _swooceHandlers.Count; i++) {

                if (_swooceHandlers[i] != null && _swooceHandlers[i].swoocedCharacterModel != null) {

                    copiedItemsBody = _swooceHandlers[i].swoocedCharacterModel.body;

                    if (copiedItemsBody.inventory == other) {

                        _copiedItemsInventory = self;
                        break;
                    }
                }
            }

            //orig(self, other);
        }

        private void InvChangedHook(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self) {
            _swooceHandlers.RemoveAll((handler) => {
                return handler == null;
            });

            float specialItemDistance = getSpecialItemDistance(self);

            //make this happen once on init rather than using GetComponent every time an inventory changes
            if (self.modelLocator.modelTransform == null) {
                return;
            }

            if (self.modelLocator.modelTransform.gameObject.GetComponent<CharacterSwooceHandler>() == null) {
                CharacterSwooceHandler swooceHandler = self.modelLocator.modelTransform.gameObject.AddComponent<CharacterSwooceHandler>();
                _swooceHandlers.Add(swooceHandler);
                swooceHandler.Init(this, specialItemDistance);
            }

            orig(self);
        }

        //move this to swoocehandler. 
        private float getSpecialItemDistance(CharacterBody self) {

            bool isCopiedInventory = self.inventory == _copiedItemsInventory;
            _copiedItemsInventory = null;

            if (isCopiedInventory) {
                return Utils.Cfg_EngiTurretItemDistanceMultiplier;
            }

            bool isScavenger = checkScavengerNames(self);
            if (isScavenger) {
                return Utils.Cfg_ScavengerItemDistanceMultiplier;
            }

            bool isMoon = checkMoonNames(self);
            if (isMoon) {
                return Utils.Cfg_BrotherItemDistanceMultiplier;
            }

            return 1;
        }

        private bool checkScavengerNames(CharacterBody self) 
        {
            bool isScavenger = false;

            for (int i = 0; i < _scavGuyNames.Length; i++) {

                if (self.bodyIndex == BodyCatalog.FindBodyIndex(_scavGuyNames[i])) {
                    isScavenger = true;
                }
            }

            return isScavenger;
        }

        private bool checkMoonNames(CharacterBody self) {
            bool isMoon = false;

            for (int i = 0; i < _moonGuyNames.Length; i++) {

                if (self.bodyIndex == BodyCatalog.FindBodyIndex(_moonGuyNames[i])) {
                    isMoon = true;
                }
            }

            return isMoon;
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

        private void UpdateMaterialsHook(On.RoR2.CharacterModel.orig_UpdateMaterials orig, CharacterModel self) 
        {
            orig(self);

            updateMaterialsEvent?.Invoke(self);
        }

        #region cheats
        public void Update()
        {
            //if (Utils.Cfg_PlantsForHire)
            //{
            //    if (Input.GetKeyDown(KeyCode.F2))
            //    {
            //        TestSpawnItem((int)ItemCatalog.FindItemIndex("CritGlasses"));//ItemIndex.CritGlasses);
            //    }

            //    //if (Input.GetKeyDown(KeyCode.F3))
            //    //{
            //    //    Chat.AddMessage("kek");
            //    //    TestSpawnItem((int)ItemIndex.TreasureCache);
            //    //}

            //    if (Input.GetKeyDown(KeyCode.F6))
            //    {
            //        TestSpawnItem();
            //    }

            //    if (Input.GetKeyDown(KeyCode.F7))
            //    {
            //        TestSpawnItem(Utils.Cfg_CheatItem);
            //    }

            //    if (Input.GetKeyDown(KeyCode.F9))
            //    {
            //        TestSpawnItemRed();
            //    }
            //}
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

            PickupIndex pickupIndex;

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
                        Utils.Log("treasureRNG broken", true);
                        //_currentRandomIndex = Run.instance.treasureRng.RangeInt(0, dropList.Count);
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