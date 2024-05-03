﻿using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
//using R2API.Utils;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace SillyGlasses
{
    //[NetworkCompatibility(CompatibilityLevel.NoNeedForSync)]
    [BepInPlugin("com.TheTimeSweeper.SillyItem", "Silly Items", "1.3.2")]
    public class SillyGlassesPlugin : BaseUnityPlugin
    {
        public delegate void UpdateItemDisplayEvent(CharacterModel self, Inventory inventory);
        public delegate void EnableDisableDisplayEvent(CharacterModel self, ItemIndex itemIndex);

        public delegate void UpdateMaterialsEvent(CharacterModel self);
        public delegate void UpdateCameraEvent(CharacterModel self, CameraRigController cameraRig);

        public EnableDisableDisplayEvent enableDisableDisplayEvent;

        //private List<CharacterSwooceHandler> _swooceHandlers = new List<CharacterSwooceHandler>();
        //public List<CharacterSwooceHandler> swooceHandlers => _swooceHandlers;

        private string[] _turretGuyNames = new string[] {
            "EngiBeamTurretBody",
            "EngiTurretBody",
            "EngiWalkerTurretBody",
            "TeslaTowerBody",
        };

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

        public static SillyGlassesPlugin instance;

        public void Awake()
        {
            instance = this;

            InitConfig();

            //On.RoR2.Inventory.CopyItemsFrom_Inventory += Inventory_CopyItemsFrom_Inventory; ;

            //On.RoR2.CharacterBody.Awake += CharacterBody_Awake;
            //On.RoR2.CharacterBody.Start += CharacterBody_Start;

            On.RoR2.CharacterBody.OnInventoryChanged += InvChangedHook;

            On.RoR2.CharacterModel.EnableItemDisplay += EnableItemDisplayHook;
            
            On.RoR2.CharacterModel.DisableItemDisplay += DisableItemDisplayHook;
        }

        //dont work. something else needs to awake first idk what
        private void CharacterBody_Awake(On.RoR2.CharacterBody.orig_Awake orig, CharacterBody self)
        {
            orig(self);

            float specialItemDistance = getSpecialItemDistance(self);

            //make this happen once on init rather than using GetComponent every time an inventory changes
            if (self.modelLocator?.modelTransform == null)
            {
                return;
            }

            CharacterSwooceHandler swooceHandler = self.modelLocator.modelTransform.gameObject.AddComponent<CharacterSwooceHandler>();
            //_swooceHandlers.Add(swooceHandler);
            swooceHandler.Init(specialItemDistance);
        }

        #region config

        private void InitConfig()
        {
            string sectionName = "hope youre having a lovely day";

            Utils.Cfg_ItemStackMax =
                Config.Bind(sectionName,
                            "ItemStacksMax",
                            -1,
                            "Maximum item displays that can be spawned (-1 for infinite).").Value;

            Utils.Cfg_OutwardStackType =
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

        private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);

            float specialItemDistance = getSpecialItemDistance(self);

            //make this happen once on init rather than using GetComponent every time an inventory changes
            //nvm this broke everything
                //getcomponentphobia is pretty bad sometimes
            if (self.modelLocator?.modelTransform == null)
            {
                return;
            }

            CharacterSwooceHandler swooceHandler = self.modelLocator.modelTransform.gameObject.AddComponent<CharacterSwooceHandler>();
            //_swooceHandlers.Add(swooceHandler);
            swooceHandler.Init(specialItemDistance);
        }

        private void InvChangedHook(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            float specialItemDistance = getSpecialItemDistance(self);

            //make this happen once on init rather than using GetComponent every time an inventory changes
            if (self.modelLocator.modelTransform == null)
            {
                return;
            }

            if (self.modelLocator.modelTransform.gameObject.GetComponent<CharacterSwooceHandler>() == null)
            {
                CharacterSwooceHandler swooceHandler = self.modelLocator.modelTransform.gameObject.AddComponent<CharacterSwooceHandler>();
                //_swooceHandlers.Add(swooceHandler);
                swooceHandler.Init(specialItemDistance);
            }

            orig(self);
        }

        //move this to swoocehandler.
        private float getSpecialItemDistance(CharacterBody self)
        {
            BodyIndex index = self.bodyIndex;

            if (CheckNames(index, _turretGuyNames))
            {
                return Utils.Cfg_EngiTurretItemDistanceMultiplier;
            }

            if (CheckNames(index, _scavGuyNames))
            {
                return Utils.Cfg_ScavengerItemDistanceMultiplier;
            }

            if (CheckNames(index, _moonGuyNames))
            {
                return Utils.Cfg_BrotherItemDistanceMultiplier;
            }

            return 1;
        }

        private static bool CheckNames(BodyIndex index, string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                if (index == BodyCatalog.FindBodyIndex(names[i]))
                {
                    return true;
                }
            }
            return false;
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

    #region sillydisplayrules
    public class SillyItemDisplayRules : List<SillyItemDisplayRule>
    {

    }

    public enum SillyItemDisplayBehavior
    {
        DEFAULT,
        DEFAULT_BOTH_WAYS,
        OUTWARD,
        SCATTER,
        //SPIRAL
        //GROW
    }

    public class SillyItemDisplayRule
    {
        public string character;

        public ItemIndex item;

        public Vector3 defaultStackDirection = Vector3.forward;
        //public Vector3 rotationShift;
        public float distanceMult = 1;
        public SillyItemDisplayBehavior stackBehavior;
    }

    #endregion

}