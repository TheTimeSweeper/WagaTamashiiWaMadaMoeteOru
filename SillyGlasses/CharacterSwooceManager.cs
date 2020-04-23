using System;
using System.Collections.Generic;
using BepInEx;
using RoR2;
using UnityEngine;

namespace SillyGlasses
{
    public class CharacterSwooceManager : MonoBehaviour {
        

        private Dictionary<ItemIndex, int> _instantiatedSillyGlassAmounts = new Dictionary<ItemIndex, int>();
        private Dictionary<ItemIndex, Transform> _instantiatedSillyGlassParents = new Dictionary<ItemIndex, Transform>();
        private Dictionary<ItemIndex, List<Transform>> _extraGlassParentsLists = new Dictionary<ItemIndex, List<Transform>> ();

        private SillyGlassesPlugin _sillyGlasses;
        private CharacterModel _swoocedModel;
        private Inventory _swoocedCurrentInventory;
        private ChildLocator _swooceChildLocator;

        private bool _subscribedToEvents;

        private bool _engiMinion;
        private bool _scavGuy;

        private int _cfgMaxItems {
            get {
                return Utils.Cfg_Int_ItemStackMax;
            }
        }
        private float _cfgDistanceMultiplier {
            get {
                return Utils.Cfg_Float_ItemSwooceDistanceMultiplier;
            }
        }

        private float _specialCharacterDistanceMultiplier {
            get {
                float engiDistance = Utils.Cfg_Float_EngiTurretItemDistanceMultiplier;
                float scavDistance = Utils.Cfg_Float_ScavengerItemDistanceMultiplier;

                return _engiMinion ? engiDistance : _scavGuy ? scavDistance : 1;
            }
        }

        public void Awake() {

            if (_swoocedModel == null)
            {
                _swoocedModel = GetComponent<CharacterModel>();
            }
        }

        public void Init(SillyGlassesPlugin sillyGlasses_, bool engi_, bool scav_)
        {
            _sillyGlasses = sillyGlasses_;

            _engiMinion = engi_;
            _scavGuy = scav_;
            
            SubscribeToEvents(true);
        }

        void OnDestroy()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool subscribing)
        {
            if (subscribing == _subscribedToEvents)
                return;

            _subscribedToEvents = subscribing;

            if (subscribing)
            {
                _sillyGlasses.updateItemDisplayEvent += onHookedUpdateItemDisplay;
                _sillyGlasses.enableDisableDisplayevent += onHookedEnableDisableDisplay;
            }
            else
            {
                _sillyGlasses.updateItemDisplayEvent -= onHookedUpdateItemDisplay;
                _sillyGlasses.enableDisableDisplayevent -= onHookedEnableDisableDisplay;
            }
        }

        private void onHookedUpdateItemDisplay(CharacterModel characterModel_, Inventory inventory_)
        {
            if (_swoocedModel == characterModel_ && _swoocedCurrentInventory == null)
            {
                _swoocedCurrentInventory = inventory_;
            }
        }

        public void onHookedEnableDisableDisplay(CharacterModel characterModel_, ItemIndex itemIndex_)
        {
            if (characterModel_.gameObject != gameObject)
                return;

            if (characterModel_.itemDisplayRuleSet)
            {
                DisplayRuleGroup displayRuleGroup = characterModel_.itemDisplayRuleSet.GetItemDisplayRuleGroup(itemIndex_);

                PseudoInstantiateDisplayRuleGroup(characterModel_, displayRuleGroup, itemIndex_);
            }
        }

        private void PseudoInstantiateDisplayRuleGroup(CharacterModel CharacterModel_,
                                                      DisplayRuleGroup displayRuleGroup_,
                                                      ItemIndex itemIndex_)
        {
            //pls don't break pls don't break
            if (_swoocedCurrentInventory == null)
                return;
            if (CharacterModel_ != _swoocedModel)
                return;
            if (displayRuleGroup_.rules == null)
                return;
            if (itemIndex_ == ItemIndex.None)
                return;

            //keep track of items
            if (!_instantiatedSillyGlassAmounts.ContainsKey(itemIndex_))
            {
                _instantiatedSillyGlassAmounts.Add(itemIndex_, 0);
            }

            int currentCount = _swoocedCurrentInventory.GetItemCount(itemIndex_);

            int sillyItemDisplays = _instantiatedSillyGlassAmounts[itemIndex_];
            int previousTotalItemDisplays = sillyItemDisplays + (currentCount > 0 ? 1 : 0);

            //find how many we're adding/removing
            int difference = currentCount - previousTotalItemDisplays;

            //this is the first item, let the game spawn it normally
            if (currentCount == 1 && difference >= 0)
                return;

            //we're removing. destroy all the displays and spawn the right amount (sadly i'm not rad enough to only destroy what is needed to be destroyed. i did the more retard friendly method)
            if (difference < 0) 
            {
                //Utils.Log($"{itemIndex_} diff: {difference} = current: {currentCount} - orig: {previousTotalItemDisplays}");

                //find the glass parent and destroy it
                if (_instantiatedSillyGlassParents.ContainsKey(itemIndex_) && _instantiatedSillyGlassParents[itemIndex_] != null)
                {
                    Destroy(_instantiatedSillyGlassParents[itemIndex_].gameObject);
                    _instantiatedSillyGlassParents[itemIndex_] = null;
                    _instantiatedSillyGlassAmounts[itemIndex_] = 0;

                    //find any subparents and destory them
                    if (_extraGlassParentsLists.ContainsKey(itemIndex_) && _extraGlassParentsLists[itemIndex_] != null)
                    {
                        List<Transform> extraParents = _extraGlassParentsLists[itemIndex_];
                        for (int i = extraParents.Count - 1; i >= 0; i--)
                        {
                            if (extraParents[i])
                            {
                                Destroy(extraParents[i].gameObject);
                            }
                            extraParents.RemoveAt(i);
                        }
                    }

                    //restart
                    PseudoInstantiateDisplayRuleGroup(CharacterModel_, displayRuleGroup_, itemIndex_);
                }
                return;
            }
            
            if (_swooceChildLocator == null)
            {
                _swooceChildLocator = CharacterModel_.GetComponent<ChildLocator>();                
            }

            //
            //all clear, we're adding, let's get to swoocing
            //

            for (int i = 0; i < difference; i++)
            {
                if (_cfgMaxItems != -1 && sillyItemDisplays + 1 >= _cfgMaxItems)
                    return;
            
                int currentCountIterated = previousTotalItemDisplays + i;

                //Utils.Log($"swoocing new prefab {itemIndex_}: {i} of {extraItemDisplays} ");
                _instantiatedSillyGlassAmounts[itemIndex_]++;

                for (int j = 0; j < displayRuleGroup_.rules.Length; j++)
                {
                    ItemDisplayRule swoocedDisplayRule = displayRuleGroup_.rules[j];
                    if (swoocedDisplayRule.ruleType != ItemDisplayRuleType.ParentedPrefab)
                        continue;

                    Transform bodyDisplayParent = _swooceChildLocator.FindChild(swoocedDisplayRule.childName);
                    if (bodyDisplayParent == null)
                        continue;

                    //create item
                    GameObject iterInstantiatedItem = InstantiateSillyItem(swoocedDisplayRule, _swooceChildLocator, bodyDisplayParent, currentCountIterated);
                    if (iterInstantiatedItem == null)
                        continue;

                    iterInstantiatedItem.name += currentCountIterated.ToString();

                    //parent item to correct parent transform
                    Transform sillyGlassParent = GetSillyGlassParent(itemIndex_, bodyDisplayParent);

                    iterInstantiatedItem.transform.parent = sillyGlassParent;

                    if (sillyGlassParent == null)
                    {
                        Utils.Log("sillyGlassParent is null \nwait how \nmake that big ol' function good enough to eliminate this as a possibility. I should be pretty much covering all the bases there");
                    }
                }
            }
        }

        //copied from CharacterModel.ParentedPrefabDisplay.Apply
        private GameObject InstantiateSillyItem(ItemDisplayRule displayRule_, ChildLocator childLocator_, Transform bodyDisplayParent_, int moveMult_)
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

            float forwardDistance = _cfgDistanceMultiplier * moveMult_ * _specialCharacterDistanceMultiplier;
            instantiatedDisplay.transform.position += instantiatedDisplay.transform.forward * forwardDistance;

            LimbMatcher limbMatcher = instantiatedDisplay.GetComponent<LimbMatcher>();
            if (limbMatcher && childLocator_)
            {
                limbMatcher.SetChildLocator(childLocator_);
            }

            //we can't access 'this.itemDisplay'. hope we don't need it
            //this.itemDisplay = parentedDisplay.GetComponent<ItemDisplay>();

            return instantiatedDisplay;
        }

        private Transform GetSillyGlassParent(ItemIndex itemIndex_, Transform bodyDisplayParent_)
        {
            Transform sillyGlassParent = _instantiatedSillyGlassParents.ContainsKey(itemIndex_) ? _instantiatedSillyGlassParents[itemIndex_] : null;

            bool hasNoSillyGlassParent = sillyGlassParent == null;

            //if there's no parent, create one and return it
            if (hasNoSillyGlassParent)
            {
                Transform parentTransform = CreateNewParentedTransform($"SillyItem_{itemIndex_.ToString()}_Parent", bodyDisplayParent_);

                _instantiatedSillyGlassParents[itemIndex_] = parentTransform;

                return parentTransform;
            }

            bool hasSillyGlassParent = sillyGlassParent != null && sillyGlassParent.parent == bodyDisplayParent_;

            //if there is an appropriate parent, return this one
            if (hasSillyGlassParent)
            {
                return _instantiatedSillyGlassParents[itemIndex_];
            }

            bool sillyGlassParentIsDifferent = sillyGlassParent != null && sillyGlassParent.parent != bodyDisplayParent_;

            //if there is a parent but it's not on the right limb, look for an extra
            if (sillyGlassParentIsDifferent)
            {
                List<Transform> extraParentList = null;

                // if there are no list for these extras yet, create a list for them
                if (!_extraGlassParentsLists.ContainsKey(itemIndex_))
                {
                    _extraGlassParentsLists[itemIndex_] = new List<Transform>();
                }

                extraParentList = _extraGlassParentsLists[itemIndex_];

                //search extras list and try to find one to return
                Transform extraParent = extraParentList.Find((extra) => {
                    return bodyDisplayParent_ == extra.parent;
                });

                //if there's no good boy in the list, create one and return it
                if (extraParent == null)
                {
                    extraParent = CreateNewParentedTransform($"SillyItem_{itemIndex_.ToString()}_ExtraParent", bodyDisplayParent_);
                    _extraGlassParentsLists[itemIndex_].Add(extraParent);
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
        //not currently used. may be outdaded
        private void ShowFunnyCube(Transform parent_, Vector3 displayRuleLocalPos_, Quaternion displayRuleLocalRotation_, float forwardDistance_)
        {
            GameObject cubeReference = GameObject.CreatePrimitive(PrimitiveType.Cube);

            //I forgot why, but simply creating a primitive wasn't working, so the ugly solution was to create a new gameobject from scratch and copy its properties
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

        #region cheatsvol2
        void Update()
        {
            if (Utils.Cfg_Bool_PlantsForHire)
            {
                if (Input.GetKeyDown(KeyCode.F10))
                {
                    TestRemoveGlasses();
                }
                if (Input.GetKeyDown(KeyCode.F11))
                {
                    TestAddGlasses();
                }
            }
        }

        private void TestAddGlasses()
        {
            if (_swoocedCurrentInventory != null)
            {
                _swoocedCurrentInventory.GiveItem((ItemIndex)Utils.Cfg_Int_CheatItemBoring, 1);
            }
        }

        private void TestRemoveGlasses()
        {
            if(_swoocedCurrentInventory != null)
            {
                _swoocedCurrentInventory.RemoveItem((ItemIndex)Utils.Cfg_Int_CheatItemBoring, 1);
            }
        }
        #endregion
    }
}