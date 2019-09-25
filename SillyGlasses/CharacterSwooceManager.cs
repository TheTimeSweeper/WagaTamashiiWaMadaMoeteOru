using System;
using System.Collections.Generic;
using BepInEx;
using RoR2;
using UnityEngine;

namespace SillyGlasses
{
    public class CharacterSwooceManager : MonoBehaviour {
        
        public bool Engi;

        private Dictionary<ItemIndex, int> _instantiatedExtraGlasAmounts = new Dictionary<ItemIndex, int>();
        private Dictionary<ItemIndex, Transform> _instantiatedGlasParents = new Dictionary<ItemIndex, Transform>();
        private Dictionary<string, Transform> _extraGlasParents = new Dictionary<string, Transform>();

        //private Dictionary<string, Transform> _instantiatedGlasParentsExtra = new Dictionary<string, Transform>();
        //check the parents of the parents:
        //displayruleparent != _instantiatedGlasParents[item].parent
        //displayruleparent != _instantiatedGlasParentsExtra[string.format(item+number)].parent
        //how do i find number?

        private CharacterModel _swoocedModel;
        private Inventory _swoocedCurrentInventory;
        private ChildLocator _swooceChildLocator;

        public void Awake() {

            if (_swoocedModel == null)
            {
                _swoocedModel = GetComponent<CharacterModel>();
            }
        }

        public void HookedUpdateItemDisplay(CharacterModel self, Inventory inventory)
        {
            if (_swoocedModel == self && _swoocedCurrentInventory == null)
            {
                _swoocedCurrentInventory = inventory;
            }
        }

        public void HookedEnableDisableDisplay(CharacterModel self, ItemIndex itemIndex)
        {
            if (self.itemDisplayRuleSet)
            {
                DisplayRuleGroup displayRuleGroup = self.itemDisplayRuleSet.GetItemDisplayRuleGroup(itemIndex);

                PseudoInstantiateDisplayRuleGroup(self, displayRuleGroup, itemIndex);
            }
        }

        public void PseudoInstantiateDisplayRuleGroup(CharacterModel self,
                                                      DisplayRuleGroup displayRuleGroup_,
                                                      ItemIndex itemIndex_)
        {
            if (self != _swoocedModel)
                return;

            if (displayRuleGroup_.rules == null)
                return;

            if (itemIndex_ == ItemIndex.None)
                return;
            //if (itemIndex_ == ItemIndex.BoostHp)
            //    return;
            //if (itemIndex_ == ItemIndex.BoostDamage)
            //    return;

            if (_swoocedCurrentInventory == null)
                return;

            if (!_instantiatedExtraGlasAmounts.ContainsKey(itemIndex_))
            {
                _instantiatedExtraGlasAmounts.Add(itemIndex_, 0);
            }

            int currentCount = _swoocedCurrentInventory.GetItemCount(itemIndex_);

            int extraItemDisplays = _instantiatedExtraGlasAmounts[itemIndex_];
            int previousTotalItemDisplays = extraItemDisplays + (currentCount > 0 ? 1 : 0);

            int difference = currentCount - previousTotalItemDisplays;
              
            if (difference < 0) //removing more than one, just do this it's simpler
            {
                Utils.Log($"{itemIndex_} diff: {difference} = current: {currentCount} - orig: {previousTotalItemDisplays}");
                if (_instantiatedGlasParents.ContainsKey(itemIndex_) && _instantiatedGlasParents[itemIndex_] != null)
                {
                    Destroy(_instantiatedGlasParents[itemIndex_].gameObject);
                    _instantiatedGlasParents[itemIndex_] = null;
                    _instantiatedExtraGlasAmounts[itemIndex_] = 0;
                    PseudoInstantiateDisplayRuleGroup(self, displayRuleGroup_, itemIndex_);
                }
                return;
            }

            if(currentCount == 1 && difference >= 0) //this is the first item, let the game spawn it normally
                return;

            //
            //all clear, let's get to swoocing
            //

            if (_swooceChildLocator == null)
            {
                _swooceChildLocator = self.GetComponent<ChildLocator>();                
            }

            GameObject iterInstantiatedItem = null;

            for (int i = 0; i < difference; i++)
            {
                int maxItems = SillyGlasse.CfgInt_ItemStackMax.Value;
                if (maxItems != -1 && extraItemDisplays + 1 >= maxItems)
                    return;
            
                int currentCountIterated = previousTotalItemDisplays + i;

                Utils.Log($"swoocing new prefab {itemIndex_}: {i} ");
                _instantiatedExtraGlasAmounts[itemIndex_]++;

                for (int j = 0; j < displayRuleGroup_.rules.Length; j++)
                {
                    ItemDisplayRule swoocedDisplayRule = displayRuleGroup_.rules[j];

                    if (swoocedDisplayRule.ruleType != ItemDisplayRuleType.ParentedPrefab)
                        continue;

                    Transform displayParent = _swooceChildLocator.FindChild(swoocedDisplayRule.childName);

                    iterInstantiatedItem = InstantiateExtraItem(self, swoocedDisplayRule, _swooceChildLocator, displayParent, currentCountIterated);
                    iterInstantiatedItem.name += currentCountIterated.ToString();

                    if (!_instantiatedGlasParents.ContainsKey(itemIndex_) || _instantiatedGlasParents[itemIndex_] == null)
                    {
                        Transform parentTransform = new GameObject(iterInstantiatedItem.gameObject.name + "Parent").transform;
                        parentTransform.parent = iterInstantiatedItem.transform.parent;
                        parentTransform.localPosition = Vector3.one;
                        parentTransform.localRotation = Quaternion.identity;
                        parentTransform.localScale = Vector3.one;

                        _instantiatedGlasParents[itemIndex_] = parentTransform;
                    }
                        iterInstantiatedItem.transform.parent = _instantiatedGlasParents[itemIndex_];
                }
            }
        }

        //copied from CharacterModel.ParentedPrefabDisplay.Apply
        private GameObject InstantiateExtraItem(CharacterModel characterModel_, ItemDisplayRule displayRule_, ChildLocator childLocator_, Transform parent_, int moveMult_)
        {
            GameObject prefab = displayRule_.followerPrefab;

            Vector3 displayRuleLocalPosition = displayRule_.localPos;
            Quaternion displayRuleLocalRotation = Quaternion.Euler(displayRule_.localAngles);
            Vector3 displayRuleLocalScale = displayRule_.localScale;

            GameObject instantiatedDisplay = Instantiate<GameObject>(prefab.gameObject, parent_);

            instantiatedDisplay.transform.localPosition = displayRuleLocalPosition;
            instantiatedDisplay.transform.localRotation = displayRuleLocalRotation;
            instantiatedDisplay.transform.localScale = displayRuleLocalScale;
            instantiatedDisplay.transform.position += instantiatedDisplay.transform.forward * moveMult_ * SillyGlasse.CfgFloat_ItemSwooceDistanceMultiplier.Value * (Engi ? 1.5f : 1);

            LimbMatcher component = instantiatedDisplay.GetComponent<LimbMatcher>();
            if (component && childLocator_)
            {
                component.SetChildLocator(childLocator_);
            }
            //this.itemDisplay = parentedDisplay.GetComponent<ItemDisplay>();

            return instantiatedDisplay;
        }

        private static void ShowFunnyCube(Transform parent, Vector3 DisplayRuleLocalPos, Quaternion DisplayRuleLocalRotation)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            Type[] cubeComponents = new Type[] { typeof(MeshRenderer), typeof(MeshFilter) };

            GameObject bruh = new GameObject("bruh", cubeComponents);
            bruh.GetComponent<MeshFilter>().mesh = cube.GetComponent<MeshFilter>().mesh;
            bruh.GetComponent<MeshRenderer>().material = new Material(cube.GetComponent<MeshRenderer>().material);

            bruh.transform.parent = parent;
            bruh.transform.localScale = new Vector3(0.169f, 0.01f, 0.1f);
            bruh.transform.localPosition = DisplayRuleLocalPos;
            bruh.transform.localRotation = DisplayRuleLocalRotation;

            Destroy(cube);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F10))
            {
                TestRemoveGlasses();
            }
        }

        private void TestRemoveGlasses()
        {
            if(_swoocedCurrentInventory != null)
            {
                _swoocedCurrentInventory.RemoveItem(ItemIndex.CritGlasses, 1);
            }
        }
    }
}