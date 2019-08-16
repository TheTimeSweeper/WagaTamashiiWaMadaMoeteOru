using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace SillyGlasses
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.SillyItems", "Silly Items", "0.2.5")]
    public class SillyGlasse : BaseUnityPlugin
    {
        public static ConfigWrapper<float> ItemDistanceMultiplier;
        private float _defaultSwooceDistanceMultiplier = 0.0420f;

        public static ConfigWrapper<int> ItemStackMax;

        public static ConfigWrapper<bool> PlantsForHire;

        public static ConfigWrapper<int> CheatItem;

        public static ConfigWrapper<bool> UtilsLog;

        private List<CharacterSwooceManager> _swooceManagers = new List<CharacterSwooceManager>();

        private Inventory _copiedItemsInventory;

        private bool _swooceRightIn;

        //test spawn items
        private int _currentRandomIndex;
        private int _spawnedRandomItems;
        private int _currentRedIndex;
        private int _spawnedRedItems;

        private bool _buildDropTable;

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

            ItemStackMax = Config.Wrap(sectionName,
                                       "ItemStackMax",
                                       "Maximum item displays that can be spawned (-1 for infinite).",
                                       -1);

            ItemDistanceMultiplier = Config.Wrap(sectionName,
                                                 "ItemDistanceMultiplier",
                                                 "The distance between extra displays that spawns.",
                                                 _defaultSwooceDistanceMultiplier);

            PlantsForHire = Config.Wrap(sectionName,
                                        "Cheats",
                                        "Press f2 f3 f6 and f9 to rain items from the sky.",
                                        false);

            UtilsLog = Config.Wrap(sectionName,
                                   "Output Logs",
                                   "because I keep forgetting to remove logs from my builds haha woops.",
                                   false);

            CheatItem = Config.Wrap(sectionName,
                                   "Cheat Item",
                                   "The item to spawn when you press f7",
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
                    Utils.Log("[ ~1] IT WAS FUCKIN SELF.HURTBOXGROUP");
                    Utils.Log("did it bug?", true);
                    Utils.Log("if not", true);
                    Utils.Log("WE DID IT BOIS", true);
                    Utils.Log("DIO'S IS SAFE", true);
                    return;
                }

                if (self.hurtBoxGroup.gameObject.GetComponent<CharacterSwooceManager>() == null)
                {
                    CharacterSwooceManager man = self.hurtBoxGroup.gameObject.AddComponent<CharacterSwooceManager>();
                    _swooceManagers.Add(man);
                    man.Engi = self.inventory == _copiedItemsInventory;
                }
            }

            orig(self);
            _buildDropTable = false;
        }

        public void UpdateItemDisplayHook(On.RoR2.CharacterModel.orig_UpdateItemDisplay orig,
                                          CharacterModel self,
                                          Inventory inventory)
        {
            for (int i = 0; i < _swooceManagers.Count; i++)
            {
                _swooceManagers[i].HookedUpdateItemDisplay(self,inventory);
            }
            orig(self, inventory);
        }

        private void DisableItemDisplayHook(On.RoR2.CharacterModel.orig_DisableItemDisplay orig, CharacterModel self, ItemIndex itemIndex)
        {
            orig(self, itemIndex);

            UpdateSwooceManagerDisplays(self, itemIndex);
        }

        private void EnableItemDisplayHook(On.RoR2.CharacterModel.orig_EnableItemDisplay orig, CharacterModel self, ItemIndex itemIndex)
        {
            orig(self, itemIndex);

            UpdateSwooceManagerDisplays(self, itemIndex);
        }

        private void UpdateSwooceManagerDisplays(CharacterModel self, ItemIndex itemIndex)
        {
            for (int i = 0; i < _swooceManagers.Count; i++)
            {
                _swooceManagers[i].HookedEnableDisableDisplay(self, itemIndex);
            }
        }

        public void Update()
        {
            if (PlantsForHire.Value)
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
                    TestSpawnItem(CheatItem.Value);
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
    }
}