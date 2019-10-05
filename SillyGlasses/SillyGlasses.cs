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
    [BepInPlugin("com.TheTimeSweeper.SillyItems", "Silly Items", "0.3.2")]
    public class SillyGlasses : BaseUnityPlugin
    {
        public static ConfigWrapper<int> CfgInt_ItemStackMax;
        public static ConfigWrapper<float> CfgFloat_ItemSwooceDistanceMultiplier;
        public static ConfigWrapper<bool> CfgBool_UtilsLog;
        public static ConfigWrapper<bool> CfgBool_PlantsForHire;
        public static ConfigWrapper<int> CfgBool_CheatItem;

        public delegate void UpdateItemDisplayEvent(CharacterModel self, Inventory inventory);
        public delegate void EnableDisableDisplayEvent(CharacterModel self, ItemIndex itemIndex);

        public EnableDisableDisplayEvent enableDisableDisplayevent;
        public UpdateItemDisplayEvent updateItemDisplayEvent;

        private List<CharacterSwooceManager> _swooceManagers = new List<CharacterSwooceManager>();

        private Inventory _copiedItemsInventory;

        //test spawn items
        private int _currentRandomIndex;
        private int _spawnedRandomItems;
        private int _currentRedIndex;
        private int _spawnedRedItems;

        public void Awake()
        {
            InitConfig();

            On.RoR2.Inventory.CopyItemsFrom += CopyItemsHook;

            On.RoR2.CharacterBody.OnInventoryChanged += InvChangedHook;

            On.RoR2.CharacterModel.UpdateItemDisplay += UpdateItemDisplayHook;

            On.RoR2.CharacterModel.EnableItemDisplay += EnableItemDisplayHook;

            On.RoR2.CharacterModel.DisableItemDisplay += DisableItemDisplayHook;
        }

        private void InitConfig()
        {
            string sectionName = "hope you're having a lovely day";

            CfgInt_ItemStackMax = 
                Config.Wrap(sectionName,
                            "ItemStackMax",
                            "Maximum item displays that can be spawned (-1 for infinite).",
                            -1);

            CfgFloat_ItemSwooceDistanceMultiplier = 
                Config.Wrap(sectionName,
                            "ItemDistanceMultiplier",
                            "The distance between extra displays that spawns.",
                            0.0420f);

            CfgBool_UtilsLog = 
                Config.Wrap(sectionName,
                            "Output Logs",
                            "because I keep forgetting to remove logs from my builds haha woops.",
                            false);

            string cheatSection = "liar liar plants for hire";

            CfgBool_PlantsForHire = 
                Config.Wrap(cheatSection,
                            "Cheats",
                            "Press f2 f3 f6 and f9 to rain items from the sky.",
                            false);

            CfgBool_CheatItem = 
                Config.Wrap(cheatSection,
                            "Cheat Item",
                            "Press f7 to spawn this item",
                            7);
        }

        private void CopyItemsHook(On.RoR2.Inventory.orig_CopyItemsFrom orig, Inventory self, Inventory other)
        {
            CharacterBody copiedItemsBody = null;

            for (int i = 0; i < _swooceManagers.Count; i++)
            {
                if (_swooceManagers[i] != null && _swooceManagers[i].GetComponent<CharacterModel>() != null)
                {
                    copiedItemsBody = _swooceManagers[i].GetComponent<CharacterModel>().body;
                    if (copiedItemsBody.inventory == other)
                    {
                        _copiedItemsInventory = self;
                        break;
                    }
                }
            }

            orig(self, other);
        }

        private void InvChangedHook(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            _swooceManagers.TrimExcess();

            if (self.isPlayerControlled || self.inventory == _copiedItemsInventory)
            {
                if (self.hurtBoxGroup == null)
                {
                    Utils.Log("did it bug?", true);
                    Utils.Log("if not", true);
                    Utils.Log("WE DID IT BOIS", true);
                    Utils.Log("DIO'S IS SAFE", true);
                    //there's still a nullref tho i gotta check out
                    return;
                }

                if (self.hurtBoxGroup.gameObject.GetComponent<CharacterSwooceManager>() == null)
                {
                    CharacterSwooceManager swooceManager = self.hurtBoxGroup.gameObject.AddComponent<CharacterSwooceManager>();
                    _swooceManagers.Add(swooceManager);
                    swooceManager.Init(this, self.inventory == _copiedItemsInventory);
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

            enableDisableDisplayevent?.Invoke(self, itemIndex);
        }

        private void EnableItemDisplayHook(On.RoR2.CharacterModel.orig_EnableItemDisplay orig, 
                                           CharacterModel self, 
                                           ItemIndex itemIndex)
        {
            orig(self, itemIndex);

            enableDisableDisplayevent?.Invoke(self, itemIndex);
        }

        #region cheats
        public void Update()
        {
            if (CfgBool_PlantsForHire.Value)
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
                    TestSpawnItem(CfgBool_CheatItem.Value);
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