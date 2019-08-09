using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace SillyGlasses
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.SillyItems", "Silly Items", "0.2.2")]
    public class MainGlas : BaseUnityPlugin
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
        private bool _engi;

        //test spawn items
        private int _currentRandomIndex;
        private int _spawnedRandomItems;
        private int _currentRedIndex;
        private int _spawnedRedItems;

        private bool _buildDropTable;

        public void Awake()
        {
            InitConfig();
            
            On.RoR2.Run.BuildDropTable += BuildDropTableHook;

            //On.RoR2.GenericPickupController.GrantItem += (orig, self, body, inventory) =>
            //{
            //    orig(self, body, inventory);
            //};

            //On.RoR2.Inventory.GiveItem += (orig, self, itemIndex, count) =>
            //{
            //    orig(self, itemIndex, count);
            //};

            //On.RoR2.Inventory.RemoveItem += (orig, self, itemIndex, count) =>
            //{
            //    orig(self, itemIndex, count);
            //};

            //item amount has been applied

            On.RoR2.Inventory.CopyItemsFrom += CopyItemsHook;

            On.RoR2.CharacterBody.OnInventoryChanged += InvChangedHook;

            On.RoR2.CharacterModel.UpdateItemDisplay += UpdateItemDisplayHook;

            On.RoR2.ItemMask.HasItem += HasItemHook;

            On.RoR2.CharacterModel.InstantiateDisplayRuleGroup += InstantiateDisplayRuleGroupHook;
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

        private void BuildDropTableHook(On.RoR2.Run.orig_BuildDropTable orig, Run self)
        {
            _buildDropTable = true;
            orig(self);
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
            //Utils.Log($"player {self.isPlayerControlled}");

            _swooceManagers.TrimExcess();

            if (self.isPlayerControlled || self.inventory == _copiedItemsInventory)
            {
                if (self.hurtBoxGroup.gameObject.GetComponent<CharacterSwooceManager>() == null)
                {
                    CharacterSwooceManager man = self.hurtBoxGroup.gameObject.AddComponent<CharacterSwooceManager>();
                    _swooceManagers.Add(man);
                    man.Engi = _engi;
                    if (_engi)
                    {
                        _engi = false;
                    }
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

        public bool HasItemHook(On.RoR2.ItemMask.orig_HasItem orig, ref ItemMask self, ItemIndex itemIndex)
        {
            if (_buildDropTable)
            {
                return orig(ref self, itemIndex);
            }

            _swooceRightIn = orig(ref self, itemIndex);

            return false;
        }

        private void InstantiateDisplayRuleGroupHook(On.RoR2.CharacterModel.orig_InstantiateDisplayRuleGroup orig, 
                                                     CharacterModel self, 
                                                     DisplayRuleGroup displayRuleGroup_, 
                                                     ItemIndex itemIndex_, 
                                                     EquipmentIndex equipmentIndex_)
        {
            //Utils.Log($"{itemIndex_}: orig, swooce {_swooceRightIn}, build {_buildDropTable}");
            for (int i = 0; i < _swooceManagers.Count; i++)
            {
                _swooceManagers[i].HookedInstantiateDisplayRuleGroup(self, displayRuleGroup_, itemIndex_);
            }
            if (_swooceRightIn)
            {
                _swooceRightIn = false;
                return;
            }
            orig(self, displayRuleGroup_, itemIndex_, equipmentIndex_);
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
                    TestSpawnItem(7);
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
            TestSpawnItemm(dropList, item, true);

            //Transform transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

            //PickupIndex pickupIndex = new PickupIndex();

            //if(item == -1)
            //{
            //    List<PickupIndex> dropList = Run.instance.availableTier1DropList;
            //    if (dropList.Count < 0)
            //        return;

            //    if (_spawnedRandomItems >= 3)
            //    {
            //        //Randomly get the next item:
            //        _currentRandomIndex = Run.instance.treasureRng.RangeInt(0, dropList.Count);
            //        _spawnedRandomItems = 0;
            //    }
            //    else
            //    {
            //        _spawnedRandomItems++;
            //    }

            //    pickupIndex = dropList[_currentRandomIndex];
            //}
            //else
            //{
            //    pickupIndex = new PickupIndex((ItemIndex)item);
            //}

            //Vector3 dir = UnityEngine.Random.insideUnitSphere;
            //dir.y = Mathf.Abs(dir.y * 3);
            //dir /= 3;

            //PickupDropletController.CreatePickupDroplet(pickupIndex, transform.position + Vector3.up * 69, dir * 20f);
        }

        private void TestSpawnItemRed()
        {
            List<PickupIndex> dropList = Run.instance.availableTier3DropList;
            TestSpawnItemm(dropList, -1, false);

            //Transform transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

            //PickupIndex pickupIndex = new PickupIndex();
            
            //List<PickupIndex> dropList = Run.instance.availableTier3DropList;
            //if (dropList.Count < 0)
            //    return;

            //pickupIndex = dropList[_currentRedIndex];

            //_spawnedRedItems++;
            //if (_spawnedRedItems >= 3)
            //{
            //    _spawnedRedItems = 0;
            //    _currentRedIndex++;
            //}
            //if(_currentRedIndex >= dropList.Count)
            //{
            //    _currentRedIndex = 0;
            //}

            //Vector3 dir = UnityEngine.Random.insideUnitSphere;
            //dir.y = Mathf.Abs(dir.y * 3);
            //dir /= 3;

            //PickupDropletController.CreatePickupDroplet(pickupIndex, transform.position + Vector3.up * 69, dir * 20f);
        }

        private void TestSpawnItemm(List<PickupIndex> dropList, int item = -1, bool random = true)
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

//NullReferenceException
//  at(wrapper managed-to-native) UnityEngine.Component.get_gameObject(UnityEngine.Component)
// at SillyGlasses.MainGlas.InvChangedHook(On.RoR2.CharacterBody+orig_OnInventoryChanged orig, RoR2.CharacterBody self) [0x00034] in <1db70481bff74a93896578b58ada899f>:0 
//  at DMD<>?1945817088._Hook<RoR2_CharacterBody::OnInventoryChanged>?-1710622976 (RoR2.CharacterBody )[0x00014] in <a1a17e560e4d4bc2aa2ab6f1b55ce402>:0 
//  at (wrapper delegate-invoke) <Module>.invoke_void()
//  at RoR2.Inventory.GiveItem(RoR2.ItemIndex itemIndex, System.Int32 count) [0x0005f] in <8ec438c5119444dda414344ec5746409>:0 
//  at RoR2.CharacterMaster.RespawnExtraLife() [0x00006] in <8ec438c5119444dda414344ec5746409>:0 