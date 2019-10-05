using System;
using System.Collections.Generic;
using BepInEx;
using RoR2;
using UnityEngine;

namespace SillyGlasses
{
        public class ExtraGlassParentsDict : Dictionary<ItemIndex, List<Transform>>
        {
            
        }

    public class CharacterSwooceManager : MonoBehaviour {
        

        private Dictionary<ItemIndex, int> _instantiatedSillyGlassAmounts = new Dictionary<ItemIndex, int>();
        private Dictionary<ItemIndex, Transform> _instantiatedSillyGlassParents = new Dictionary<ItemIndex, Transform>();
        private Dictionary<ItemIndex, List<Transform>> _extraGlassParentsLists = new Dictionary<ItemIndex, List<Transform>> ();

        private ExtraGlassParentsDict _extras = new ExtraGlassParentsDict();


        //private Dictionary<string, Transform> _instantiatedGlasParentsExtra = new Dictionary<string, Transform>();
        //check the parents of the parents:
        //displayruleparent != _instantiatedGlasParents[item].parent
        //displayruleparent != _instantiatedGlasParentsExtra[string.format(item+number)].parent
        //how do i find number?

        private SillyGlasses _sillyGlasses;
        private CharacterModel _swoocedModel;
        private Inventory _swoocedCurrentInventory;
        private ChildLocator _swooceChildLocator;

        private int _cfgMaxItems;
        private float _cfgDistanceMultiplier;

        private bool _engi;

        public void Awake() {

            if (_swoocedModel == null)
            {
                _swoocedModel = GetComponent<CharacterModel>();
            }
            _cfgDistanceMultiplier = SillyGlasses.CfgFloat_ItemSwooceDistanceMultiplier.Value;
            _cfgMaxItems = SillyGlasses.CfgInt_ItemStackMax.Value;
        }

        public void Init(SillyGlasses sillyGlasses_, bool engi_)
        {
            _sillyGlasses = sillyGlasses_;
            _engi = engi_;

            SubscribeToEvents(true);
        }

        void OnDestroy()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool subscribing)
        {
            if (subscribing)
            {
                _sillyGlasses.updateItemDisplayEvent += onHookedUpdateItemDisplay;
                _sillyGlasses.enableDisableDisplayevent += onHookedEnableDisableDisplay;
                Utils.Log("events Added");
            }
            else
            {
                _sillyGlasses.updateItemDisplayEvent -= onHookedUpdateItemDisplay;
                _sillyGlasses.enableDisableDisplayevent -= onHookedEnableDisableDisplay;
                Utils.Log("events Removed");
            }
        }

        public void onHookedUpdateItemDisplay(CharacterModel characterModel_, Inventory inventory_)
        {
            if (_swoocedModel == characterModel_ && _swoocedCurrentInventory == null)
            {
                _swoocedCurrentInventory = inventory_;
            }

            string name = characterModel_ ? characterModel_.gameObject.name : "";
            Utils.Log($"event UpdateItem Called {name}");
        }

        public void onHookedEnableDisableDisplay(CharacterModel characterModel_, ItemIndex itemIndex_)
        {
            if (characterModel_.itemDisplayRuleSet)
            {
                DisplayRuleGroup displayRuleGroup = characterModel_.itemDisplayRuleSet.GetItemDisplayRuleGroup(itemIndex_);

                PseudoInstantiateDisplayRuleGroup(characterModel_, displayRuleGroup, itemIndex_);
            }

            string name = characterModel_ ? characterModel_.gameObject.name : "";
            Utils.Log($"event EnableDisable called {name}");
        }

        public void PseudoInstantiateDisplayRuleGroup(CharacterModel CharacterModel_,
                                                      DisplayRuleGroup displayRuleGroup_,
                                                      ItemIndex itemIndex_)
        {
            if (_swoocedCurrentInventory == null)
                return;
            if (CharacterModel_ != _swoocedModel)
                return;
            if (displayRuleGroup_.rules == null)
                return;
            if (itemIndex_ == ItemIndex.None)
                return;

            if (!_instantiatedSillyGlassAmounts.ContainsKey(itemIndex_))
            {
                _instantiatedSillyGlassAmounts.Add(itemIndex_, 0);
            }

            int currentCount = _swoocedCurrentInventory.GetItemCount(itemIndex_);

            int sillyItemDisplays = _instantiatedSillyGlassAmounts[itemIndex_];
            int previousTotalItemDisplays = sillyItemDisplays + (currentCount > 0 ? 1 : 0);

            int difference = currentCount - previousTotalItemDisplays;

            if (difference < 0) //we're removing. destroy all the displays and spawn the right amount
            {
                //Utils.Log($"{itemIndex_} diff: {difference} = current: {currentCount} - orig: {previousTotalItemDisplays}");

                if (_instantiatedSillyGlassParents.ContainsKey(itemIndex_) && _instantiatedSillyGlassParents[itemIndex_] != null)
                {
                    Destroy(_instantiatedSillyGlassParents[itemIndex_].gameObject);
                    _instantiatedSillyGlassParents[itemIndex_] = null;
                    _instantiatedSillyGlassAmounts[itemIndex_] = 0;
                    PseudoInstantiateDisplayRuleGroup(CharacterModel_, displayRuleGroup_, itemIndex_);
                }
                return;
            }

            if(currentCount == 1 && difference >= 0) //this is the first item, let the game spawn it normally
                return;

            if (_swooceChildLocator == null)
            {
                _swooceChildLocator = CharacterModel_.GetComponent<ChildLocator>();                
            }

            //
            //all clear, let's get to swoocing
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

                    //parent item to glassparent
                    Transform sillyGlassParent = GetSillyGlassParent(itemIndex_, bodyDisplayParent);

                    iterInstantiatedItem.transform.parent = sillyGlassParent;

                    if (sillyGlassParent == null)
                    {
                        Utils.Log("wait how\nmake that function good enough to eliminate this as a possibility. I'm pretty much covering all the bases here");
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
            float forwardDistance = moveMult_ * _cfgDistanceMultiplier * (_engi ? 1.5f : 1);
            instantiatedDisplay.transform.position += instantiatedDisplay.transform.forward * forwardDistance;

            LimbMatcher component = instantiatedDisplay.GetComponent<LimbMatcher>();
            if (component && childLocator_)
            {
                component.SetChildLocator(childLocator_);
            }
            //this.itemDisplay = parentedDisplay.GetComponent<ItemDisplay>();

            return instantiatedDisplay;
        }

        private Transform GetSillyGlassParent(ItemIndex itemIndex_, Transform bodyDisplayParent_)
        {
            Transform sillyGlassParent = _instantiatedSillyGlassParents.ContainsKey(itemIndex_) ? _instantiatedSillyGlassParents[itemIndex_] : null;

            bool hasNoSillyGlassParent = sillyGlassParent == null;

            //if there's no parent, create one
            if (hasNoSillyGlassParent)
            {
                Transform parentTransform = NewParentedTransform($"{itemIndex_.ToString()}_Parent", bodyDisplayParent_);

                _instantiatedSillyGlassParents[itemIndex_] = parentTransform;

                return parentTransform;
            }

            bool hasSillyGlassParent = sillyGlassParent != null && sillyGlassParent.parent == bodyDisplayParent_;

            //if there is an appropriate parent, use this one
            if (hasSillyGlassParent)
            {
                return _instantiatedSillyGlassParents[itemIndex_];
            }

            bool sillyGlassParentIsDifferent = sillyGlassParent != null && sillyGlassParent.parent != bodyDisplayParent_;

            //if there is a parent but it's not on the right limb, look for an extra
            if (sillyGlassParentIsDifferent)
            {
                List<Transform> extraParentList = null;

                // if there are no extras yet, create a list for them
                if (!_extraGlassParentsLists.ContainsKey(itemIndex_))
                {
                    _extraGlassParentsLists[itemIndex_] = new List<Transform>();
                }

                extraParentList = _extraGlassParentsLists[itemIndex_];

                //search extra list
                Transform extraParent = extraParentList.Find((extra) => {
                    return bodyDisplayParent_ == extra.parent;
                });

                //if there's no good boy in the list, create one
                if (extraParent == null)
                {
                    extraParent = NewParentedTransform($"{itemIndex_.ToString()}_ExtraParent", bodyDisplayParent_);
                    _extraGlassParentsLists[itemIndex_].Add(extraParent);
                }

                return extraParent;
            }

            return null;
        }

        private Transform NewParentedTransform(string name, Transform parent)
        {
            Transform parentTransform = new GameObject(name).transform;
            parentTransform.parent = parent;
            parentTransform.localPosition = Vector3.one;
            parentTransform.localRotation = Quaternion.identity;
            parentTransform.localScale = Vector3.one;

            return parentTransform;
        }

        private void ShowFunnyCube(Transform parent_, Vector3 displayRuleLocalPos_, Quaternion displayRuleLocalRotation_, float forwardDistance_)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            Type[] cubeComponents = new Type[] { typeof(MeshRenderer), typeof(MeshFilter) };

            GameObject bruh = new GameObject("bruh", cubeComponents);
            bruh.GetComponent<MeshFilter>().mesh = cube.GetComponent<MeshFilter>().mesh;
            bruh.GetComponent<MeshRenderer>().material = new Material(cube.GetComponent<MeshRenderer>().material);

            bruh.transform.parent = parent_;

            bruh.transform.localPosition = displayRuleLocalPos_;
            bruh.transform.localRotation = displayRuleLocalRotation_;
            bruh.transform.localScale = new Vector3(0.169f, 0.01f, 0.1f);
            bruh.transform.position += bruh.transform.forward * forwardDistance_;

            Destroy(cube);
        }

        void Update()
        {
            if (SillyGlasses.CfgBool_PlantsForHire.Value)
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
                _swoocedCurrentInventory.GiveItem(ItemIndex.CritGlasses, 1);
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