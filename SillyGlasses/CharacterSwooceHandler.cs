using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
//using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngineInternal;

namespace SillyGlasses
{
    public class CharacterSwooceHandler : MonoBehaviour {

        private bool _subscribedToEvents;

        #region StackingDisplays
        private Dictionary<ItemIndex, int> _instantiatedSillyAmounts = new Dictionary<ItemIndex, int>();
        private Dictionary<ItemIndex, Transform> _instantiatedSillyParents = new Dictionary<ItemIndex, Transform>();
        private Dictionary<ItemIndex, List<Transform>> _extraParentsLists = new Dictionary<ItemIndex, List<Transform>>();

        private CharacterModel _swoocedCharacterModel;
        public CharacterModel swoocedCharacterModel {
            get => _swoocedCharacterModel;
        }

        private Inventory _swoocedCurrentInventory;
        private ChildLocator _swoocedChildLocator;

        private float _specialItemDistance;
        #endregion

        private int _cfgMaxItems {
            get {
                return Utils.Cfg_ItemStackMax;
            }
        }
        private float _cfgDistanceMultiplier {
            get {
                return Utils.Cfg_ItemDistanceMultiplier;
            }
        }

        public void Init(float distance_)
        {
            _specialItemDistance = distance_;

            _swoocedCharacterModel = GetComponent<CharacterModel>();
            _swoocedCurrentInventory = _swoocedCharacterModel.body.inventory;

            SillyGlassesPlugin.instance.swooceHandlers.Add(this);            
            SubscribeToEvents(true);
        }

        void OnDestroy()
        {
            SillyGlassesPlugin.instance.swooceHandlers.Remove(this);
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool subscribing)
        {
            if (subscribing == _subscribedToEvents)
                return;

            _subscribedToEvents = subscribing;

            if (subscribing)
            {
                //SillyGlassesPlugin.instance.updateItemDisplayEvent += onHookedUpdateItemDisplay;
                SillyGlassesPlugin.instance.enableDisableDisplayEvent += onHookedEnableDisableDisplay;
            }
            else
            {
                //SillyGlassesPlugin.instance.updateItemDisplayEvent -= onHookedUpdateItemDisplay;
                SillyGlassesPlugin.instance.enableDisableDisplayEvent -= onHookedEnableDisableDisplay;
            }
        }

        //private void onHookedUpdateItemDisplay(CharacterModel characterModel_, Inventory inventory_)
        //{
        //    if (_swoocedCharacterModel == characterModel_ && _swoocedCurrentInventory == null)
        //    {
        //        _swoocedCurrentInventory = inventory_;
        //    }
        //}

        public void onHookedEnableDisableDisplay(CharacterModel characterModel_, ItemIndex itemIndex_)
        {
            if (characterModel_ == null)
                return;
            if (characterModel_.gameObject != gameObject)
                return;

            if (characterModel_.itemDisplayRuleSet)
            {
                DisplayRuleGroup displayRuleGroup = characterModel_.itemDisplayRuleSet.GetItemDisplayRuleGroup(itemIndex_);

                PseudoInstantiateDisplayRuleGroup(characterModel_, displayRuleGroup, itemIndex_);
            }
        }

        #region StackingDisplays
        private void PseudoInstantiateDisplayRuleGroup(CharacterModel characterModel_, DisplayRuleGroup displayRuleGroup_, ItemIndex itemIndex_)
        {
            //check if it breaks
            //pls don't break
            if (_swoocedCurrentInventory == null)
                return;
            if (characterModel_ != _swoocedCharacterModel)
                return;
            if (displayRuleGroup_.rules == null || displayRuleGroup_.rules.Length == 0) 
                return;
            if (itemIndex_ == ItemIndex.None)
                return;

            //keep track of items
            if (!_instantiatedSillyAmounts.ContainsKey(itemIndex_))
            {
                _instantiatedSillyAmounts.Add(itemIndex_, 0);
            }

            int currentCount = _swoocedCurrentInventory.GetItemCount(itemIndex_);

            int sillyItemDisplays = _instantiatedSillyAmounts[itemIndex_];
            int previousTotalItemDisplays = sillyItemDisplays + (currentCount > 0 ? 1 : 0);

            //find how many we're adding/removing
            int difference = currentCount - previousTotalItemDisplays;

            //this is the first item, let the game spawn it normally
            if (currentCount == 1 && difference >= 0)
                return;

            //we're removing. destroy all the displays and spawn the right amount 
            //(sadly i'm not rad enough to only destroy what is needed to be destroyed. i did the more retard friendly method)
            if (difference < 0) 
            {
                //find the parent and destroy it
                if (_instantiatedSillyParents.ContainsKey(itemIndex_) && _instantiatedSillyParents[itemIndex_] != null)
                {
                    Destroy(_instantiatedSillyParents[itemIndex_].gameObject);
                    _instantiatedSillyParents[itemIndex_] = null;
                    _instantiatedSillyAmounts[itemIndex_] = 0;

                    //find any subparents and destory them
                    if (_extraParentsLists.ContainsKey(itemIndex_) && _extraParentsLists[itemIndex_] != null)
                    {
                        List<Transform> extraParents = _extraParentsLists[itemIndex_];
                        for (int i = extraParents.Count - 1; i >= 0; i--)
                        {
                            if (extraParents[i])
                            {
                                Destroy(extraParents[i].gameObject);
                            }
                            extraParents.RemoveAt(i);
                        }
                    }

                    for (int i = characterModel_.parentedPrefabDisplays.Count - 1; i >= 0; i--)
                    {
                        if (characterModel_.parentedPrefabDisplays[i].instance == null)
                        {
                            characterModel_.parentedPrefabDisplays.RemoveAt(i);
                        }
                    }

                    //restart
                    PseudoInstantiateDisplayRuleGroup(characterModel_, displayRuleGroup_, itemIndex_);
                }
                return;
            }
            
            if (_swoocedChildLocator == null)
            {
                _swoocedChildLocator = characterModel_.GetComponent<ChildLocator>();                
            }

            //
            //all clear, we're adding, let's get to swoocing
            //

            for (int i = 0; i < difference; i++) {

                int currentCountIterated = previousTotalItemDisplays + i;

                if (_cfgMaxItems != -1 && currentCountIterated + 1 >= _cfgMaxItems)
                    return;
            

                //Utils.Log($"swoocing new prefab {itemIndex_}: {i} of {extraItemDisplays} ");
                _instantiatedSillyAmounts[itemIndex_]++;

                for (int j = 0; j < displayRuleGroup_.rules.Length; j++)
                {
                    ItemDisplayRule swoocedDisplayRule = displayRuleGroup_.rules[j];
                    if (swoocedDisplayRule.ruleType != ItemDisplayRuleType.ParentedPrefab)
                        continue;

                    Transform bodyDisplayParent = _swoocedChildLocator.FindChild(swoocedDisplayRule.childName);
                    if (bodyDisplayParent == null)
                        continue;

                    GameObject iterInstantiatedItem = InstantiateSillyItem(swoocedDisplayRule, _swoocedChildLocator, bodyDisplayParent, currentCountIterated, itemIndex_);
                    if (iterInstantiatedItem == null)
                        continue;

                    characterModel_.parentedPrefabDisplays.Add(new CharacterModel.ParentedPrefabDisplay {
                        instance = iterInstantiatedItem,
                        itemDisplay = iterInstantiatedItem.GetComponent<ItemDisplay>(),
                        itemIndex = itemIndex_,
                    });

                    //parent item to correct parent transform
                    Transform sillyParent = GetSillyParent(itemIndex_, bodyDisplayParent);
                    iterInstantiatedItem.transform.parent = sillyParent;
                    if (sillyParent == null)
                    {
                        Utils.Log("sillyParent is null \nwait how \nI should be pretty much covering all the bases there");
                    }
                }
            }
        }

        //copied from CharacterModel.ParentedPrefabDisplay.Apply
        private GameObject InstantiateSillyItem(ItemDisplayRule displayRule_, ChildLocator childLocator_, Transform bodyDisplayParent_, int instanceMult_, ItemIndex itemIndex_)
        {
            GameObject prefab = displayRule_.followerPrefab;
            if (prefab == null)
                return null;

            Vector3 displayRuleLocalPosition = displayRule_.localPos;
            Quaternion displayRuleLocalRotation = Quaternion.Euler(displayRule_.localAngles);
            Vector3 displayRuleLocalScale = displayRule_.localScale;

            GameObject instantiatedDisplay = Instantiate<GameObject>(prefab.gameObject, bodyDisplayParent_);

            instantiatedDisplay.transform.localPosition = displayRuleLocalPosition;
            instantiatedDisplay.transform.localRotation = displayRuleLocalRotation;
            instantiatedDisplay.transform.localScale = displayRuleLocalScale;

            float forwardDistance = instanceMult_ * _cfgDistanceMultiplier * _specialItemDistance;

            Vector3 sillyOffset;

            if (!Utils.Cfg_OutwardStackType) {

                sillyOffset = instantiatedDisplay.transform.forward * forwardDistance;

            } else {

                if (displayRule_.childName != "Head") {
                    Vector3 outwardVector = (instantiatedDisplay.transform.position - bodyDisplayParent_.position).normalized;

                    Vector3 perpendicularVector = Vector3.Cross(bodyDisplayParent_.transform.up, outwardVector).normalized;

                    perpendicularVector = Vector3.Cross(bodyDisplayParent_.transform.up, perpendicularVector).normalized;

                    Vector3 adjustedDirection = ((outwardVector - perpendicularVector) * 0.5f).normalized;

                    sillyOffset = adjustedDirection * forwardDistance;
                } else {
                    sillyOffset = (instantiatedDisplay.transform.position - bodyDisplayParent_.position).normalized * forwardDistance;
                }
            }

            instantiatedDisplay.transform.position += sillyOffset;

            if (Utils.Cfg_UseLogs) {
                instantiatedDisplay.name = $"{instantiatedDisplay.name} {instanceMult_}";
            }

            LimbMatcher limbMatcher = instantiatedDisplay.GetComponent<LimbMatcher>();
            if (limbMatcher && childLocator_)
            {
                instantiatedDisplay.AddComponent<BootlegLimbMatcher>().Init(limbMatcher, childLocator_, forwardDistance);
            }

            //ItemDisplay itemDisplay = instantiatedDisplay.GetComponent<ItemDisplay>();
            //_allSillyItemDisplays.Add(itemDisplay);

            return instantiatedDisplay;
        }

        private Transform GetSillyParent(ItemIndex itemIndex_, Transform bodyDisplayParent_)
        {
            Transform sillyParent = _instantiatedSillyParents.ContainsKey(itemIndex_) ? _instantiatedSillyParents[itemIndex_] : null;

            bool hasNoSillyParent = sillyParent == null;

            //if there's no parent, create one and return it
            if (hasNoSillyParent)
            {
                Transform parentTransform = CreateNewParentedTransform($"SillyItem_{itemIndex_.ToString()}_Parent", bodyDisplayParent_);

                _instantiatedSillyParents[itemIndex_] = parentTransform;

                return parentTransform;
            }

            //if there is an appropriate parent, return this one
            if (sillyParent != null && sillyParent.parent == bodyDisplayParent_)
            {
                return _instantiatedSillyParents[itemIndex_];
            }

            //if there is a parent but it's not on the right limb, look for an extra
            if (sillyParent != null && sillyParent.parent != bodyDisplayParent_)
            {
                List<Transform> extraParentList = null;

                // if there are no list for these extras yet, create a list for them
                if (!_extraParentsLists.ContainsKey(itemIndex_))
                {
                    _extraParentsLists[itemIndex_] = new List<Transform>();
                }

                extraParentList = _extraParentsLists[itemIndex_];

                //search extras list and try to find one to return
                Transform extraParent = extraParentList.Find((extra) => {
                    return bodyDisplayParent_ == extra.parent;
                });

                //if there's no good boy in the list, create one and return it
                if (extraParent == null)
                {
                    extraParent = CreateNewParentedTransform($"SillyItem_{itemIndex_.ToString()}_ExtraParent", bodyDisplayParent_);
                    _extraParentsLists[itemIndex_].Add(extraParent);
                }

                return extraParent;
            }

            return null;
        }

        private Transform CreateNewParentedTransform(string name, Transform parent)
        {
            Transform newTransform = new GameObject(name).transform;
            newTransform.parent = parent;
            newTransform.localPosition = Vector3.one;
            newTransform.localRotation = Quaternion.identity;
            newTransform.localScale = Vector3.one;

            return newTransform;
        }

        //creates a cube in the place of where an item will spawn, so I can see in what direction its local transform is oriented
        //not currently used. basically just keeping for nostalgia
        private void ShowFunnyCube(Transform parent_, Vector3 displayRuleLocalPos_, Quaternion displayRuleLocalRotation_, float forwardDistance_) {
            GameObject cubeReference = GameObject.CreatePrimitive(PrimitiveType.Cube);

            //I forgot why, but simply creating a primitive wasn't working, so the ugly solution was to create a new gameobject from scratch and copy its properties
            //wait I think I was a retard and tried creating a blank object and adding these components instead of just making a primitive and removing the collider
            Type[] cubeComponents = new Type[] { typeof(MeshRenderer), typeof(MeshFilter) };

            GameObject bruh = new GameObject("bruh", cubeComponents);
            bruh.GetComponent<MeshFilter>().mesh = cubeReference.GetComponent<MeshFilter>().mesh;
            bruh.GetComponent<MeshRenderer>().material = new Material(cubeReference.GetComponent<MeshRenderer>().material);

            bruh.transform.parent = parent_;

            bruh.transform.localPosition = displayRuleLocalPos_;
            bruh.transform.localRotation = displayRuleLocalRotation_;
            bruh.transform.localScale = new Vector3(0.169f, 0.01f, 0.1f);
            bruh.transform.position += bruh.transform.forward * forwardDistance_;

            Destroy(cubeReference);
        }
        #endregion

        #region cheatsvol2
        void Update()
        {
            if (Utils.Cfg_PlantsForHire)
            {
                //if (Input.GetKeyDown(KeyCode.F10))
                //{
                //    TestRemoveGlasses();
                //}
                //if (Input.GetKeyDown(KeyCode.F11))
                //{
                //    TestAddGlasses();
                //}
            }
        }

        private void TestAddGlasses()
        {
            if (_swoocedCurrentInventory != null)
            {
                _swoocedCurrentInventory.GiveItem((ItemIndex)Utils.Cfg_CheatItemBoring, 1);
            }
        }

        private void TestRemoveGlasses()
        {
            if(_swoocedCurrentInventory != null)
            {
                _swoocedCurrentInventory.RemoveItem((ItemIndex)Utils.Cfg_CheatItemBoring, 1);
            }
        }
        #endregion
    }
}