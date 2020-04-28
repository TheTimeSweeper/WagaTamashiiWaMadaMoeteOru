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

        public EnableDisableDisplayEvent enableDisableDisplayevent;
        public UpdateItemDisplayEvent updateItemDisplayEvent;

        private List<CharacterSwooceManager> _swooceManagers = new List<CharacterSwooceManager>();

        //test spawn items
        private int _currentRandomIndex;
        private int _spawnedRandomItems;
        private int _currentRedIndex;
        private int _spawnedRedItems;

        private string[] _engiMinionNames = new string[] {
            "EngiTurretBody",
            "EngiBeamTurretBody",
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
        }
        
        #region config

        private void InitConfig()
        {
            string sectionName = "hope youre having a lovely day";

            Utils.Cfg_Int_ItemStackMax =
                Config.Wrap<int>(sectionName,
                            "ItemStackMax",
                            "Maximum item displays that can be spawned (-1 for infinite).",
                            -1).Value;

            Utils.Cfg_Float_ItemSwooceDistanceMultiplier =
                Config.Wrap(sectionName,
                            "ItemDistanceMultiplier",
                            "The distance between extra displays that spawns.",
                            0.0420f).Value;

            Utils.Cfg_Float_EngiTurretItemDistanceMultiplier =
                Config.Wrap(sectionName,
                            "EngiTurretItemDistanceMultiplier",
                            "Items are a little bigger on Engis Turrets. Spread them out a bit more.",
                            1.5f).Value;

            Utils.Cfg_Float_ScavengerItemDistanceMultiplier =
                Config.Wrap(sectionName,
                            "ScavItemDistanceMultiplier",
                            "Items are a little bigger on Scavengers. Spread them out just a tiny bit maybe.",
                            6f).Value;

            Utils.Cfg_Bool_UtilsLog =
                Config.Wrap(sectionName,
                            "Output Logs",
                            "because I keep forgetting to remove logs from my builds haha woops.",
                            false).Value;

            string cheatSection = "liar liar plants for hire";

            Utils.Cfg_Bool_PlantsForHire =
                Config.Wrap(cheatSection,
                            "Cheats",
                            "Press f2 f3 f6 and f9 to rain items from the sky.",
                            false).Value;

            Utils.Cfg_Int_CheatItem =
                Config.Wrap(cheatSection,
                            "Cheat Item",
                            "Press f7 to spawn this item",
                            7).Value;

            Utils.Cfg_Int_CheatItemBoring =
                Config.Wrap(cheatSection,
                            "Cheat Item2",
                            "Press f11 and f10 to add/remove this item boringly",
                            58).Value;
        }

        #endregion

        private void InvChangedHook(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            _swooceManagers.TrimExcess();

            bool isEngineerTurret = false;
            bool isScavenger = false;

            for (int i = 0; i < _engiMinionNames.Length; i++) {

                if (self.bodyIndex == BodyCatalog.FindBodyIndex(_engiMinionNames[i])) {
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
                if (self.hurtBoxGroup == null)
                {
                    Utils.Log("did it bug?", true);
                    Utils.Log("if not", true);
                    Utils.Log("WE DID IT BOIS", true);
                    Utils.Log("DIO'S IS SAFE", true);
                    Utils.Log(Environment.StackTrace);
                    //there's still a nullref tho i gotta check out
                    return;
                }
                
                if (self.hurtBoxGroup.gameObject.GetComponent<CharacterSwooceManager>() == null)
                {
                    CharacterSwooceManager swooceManager = self.hurtBoxGroup.gameObject.AddComponent<CharacterSwooceManager>();
                    _swooceManagers.Add(swooceManager);
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
            if (Utils.Cfg_Bool_PlantsForHire)
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
                    TestSpawnItem(Utils.Cfg_Int_CheatItem);
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