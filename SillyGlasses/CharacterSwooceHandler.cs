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
        
        private SillyGlassesPlugin _sillyGlasses;

        private bool _subscribedToEvents;

        #region StackingDisplays
        private Dictionary<ItemIndex, int> _instantiatedSillyGlassAmounts = new Dictionary<ItemIndex, int>();
        private Dictionary<ItemIndex, Transform> _instantiatedSillyGlassParents = new Dictionary<ItemIndex, Transform>();
        private Dictionary<ItemIndex, List<Transform>> _extraGlassParentsLists = new Dictionary<ItemIndex, List<Transform>>();

        private CharacterModel _swoocedCharacterModel;
        public CharacterModel swoocedCharacterModel {
            get => _swoocedCharacterModel;
        }

        private Inventory _swoocedCurrentInventory;
        private ChildLocator _swoocedChildLocator;

        private float _specialItemDistance;
        #endregion

        #region UpdatingMaterials
        private List<ItemDisplay> _allSillyItemDisplays = new List<ItemDisplay>();

        private MaterialPropertyBlock _propertyStorage = new MaterialPropertyBlock();

        #region hopereflectionisasperformantasthiscausegoddamn
        private float _pseudoFade = 1f;
        public float pseudoFade {
            get => _pseudoFade;
            set => _pseudoFade = value;
        }

        private float _pseudoFirstPersonFade = 1f;
        #endregion
        private static readonly Color hitFlashBaseColor = new Color32(193, 108, 51, byte.MaxValue);
        private static readonly Color hitFlashShieldColor = new Color32(132, 159, byte.MaxValue, byte.MaxValue);
        private static readonly Color healFlashColor = new Color32(104, 196, 49, byte.MaxValue);

        private static readonly float hitFlashDuration = 0.15f;
        private static readonly float healFlashDuration = 0.35f;
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

        void Awake() {

            if (_swoocedCharacterModel == null)
            {
                _swoocedCharacterModel = GetComponent<CharacterModel>();
            }

            FieldInfo fadeField = typeof(CharacterModel).GetField("fade", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public void Init(SillyGlassesPlugin sillyGlasses_, float distance_)
        {
            _sillyGlasses = sillyGlasses_;

            _specialItemDistance = distance_;
            
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
                _sillyGlasses.enableDisableDisplayEvent += onHookedEnableDisableDisplay;

                _sillyGlasses.updateMaterialsEvent += onUpdateMaterials;
                _sillyGlasses.updateCameraEvent += onUpdateCamera;

            }
            else
            {
                _sillyGlasses.updateItemDisplayEvent -= onHookedUpdateItemDisplay;
                _sillyGlasses.enableDisableDisplayEvent -= onHookedEnableDisableDisplay;

                _sillyGlasses.updateMaterialsEvent -= onUpdateMaterials;
                _sillyGlasses.updateCameraEvent -= onUpdateCamera;
            }
        }

        private void onHookedUpdateItemDisplay(CharacterModel characterModel_, Inventory inventory_)
        {
            if (_swoocedCharacterModel == characterModel_ && _swoocedCurrentInventory == null)
            {
                _swoocedCurrentInventory = inventory_;
            }
        }

        public void onHookedEnableDisableDisplay(CharacterModel characterModel_, ItemIndex itemIndex_)
        {
            if (characterModel_ && characterModel_.gameObject != gameObject)
                return;

            if (characterModel_.itemDisplayRuleSet)
            {
                DisplayRuleGroup displayRuleGroup = characterModel_.itemDisplayRuleSet.GetItemDisplayRuleGroup(itemIndex_);

                PseudoInstantiateDisplayRuleGroup(characterModel_, displayRuleGroup, itemIndex_);
            }
        }

        #region StackingDisplays
        private void PseudoInstantiateDisplayRuleGroup(CharacterModel CharacterModel_,
                                                       DisplayRuleGroup displayRuleGroup_,
                                                       ItemIndex itemIndex_)
        {
            //check if it breaks
            //pls don't break
            if (_swoocedCurrentInventory == null)
                return;
            if (CharacterModel_ != _swoocedCharacterModel)
                return;
            if (displayRuleGroup_.rules == null || displayRuleGroup_.rules.Length == 0) 
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

            //we're removing. destroy all the displays and spawn the right amount 
            //(sadly i'm not rad enough to only destroy what is needed to be destroyed. i did the more retard friendly method)
            if (difference < 0) 
            {
                //find the glassparent and destroy it
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
            
            if (_swoocedChildLocator == null)
            {
                _swoocedChildLocator = CharacterModel_.GetComponent<ChildLocator>();                
            }

            //
            //all clear, we're adding, let's get to swoocing
            //

            for (int i = 0; i < difference; i++) {

                int currentCountIterated = previousTotalItemDisplays + i;

                if (_cfgMaxItems != -1 && currentCountIterated + 1 >= _cfgMaxItems)
                    return;
            

                //Utils.Log($"swoocing new prefab {itemIndex_}: {i} of {extraItemDisplays} ");
                _instantiatedSillyGlassAmounts[itemIndex_]++;

                for (int j = 0; j < displayRuleGroup_.rules.Length; j++)
                {
                    ItemDisplayRule swoocedDisplayRule = displayRuleGroup_.rules[j];
                    if (swoocedDisplayRule.ruleType != ItemDisplayRuleType.ParentedPrefab)
                        continue;

                    Transform bodyDisplayParent = _swoocedChildLocator.FindChild(swoocedDisplayRule.childName);
                    if (bodyDisplayParent == null)
                        continue;

                    //create item
                    GameObject iterInstantiatedItem = InstantiateSillyItem(swoocedDisplayRule, _swoocedChildLocator, bodyDisplayParent, currentCountIterated, itemIndex_ == ItemCatalog.FindItemIndex("CritGlasses"));
                    if (iterInstantiatedItem == null)
                        continue;

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
        private GameObject InstantiateSillyItem(ItemDisplayRule displayRule_, ChildLocator childLocator_, Transform bodyDisplayParent_, int instanceMult_, bool glas)
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

            if (glas || Utils.Cfg_ClassicStackType) {
                if (glas) {
                    forwardDistance = Mathf.Abs(forwardDistance);
                }

                instantiatedDisplay.transform.position += instantiatedDisplay.transform.forward * forwardDistance;
            } else {

                if (displayRule_.childName != "Head") {
                    Vector3 outwardVect = (instantiatedDisplay.transform.position - bodyDisplayParent_.position).normalized;

                    Vector3 perpVec = Vector3.Cross(bodyDisplayParent_.transform.up, outwardVect).normalized;

                    perpVec = Vector3.Cross(bodyDisplayParent_.transform.up, perpVec).normalized;

                    Vector3 adjustedDirection = ((outwardVect - perpVec) * 0.5f).normalized;

                    instantiatedDisplay.transform.position += adjustedDirection * forwardDistance;
                } else {
                    instantiatedDisplay.transform.position += (instantiatedDisplay.transform.position - bodyDisplayParent_.position).normalized * forwardDistance;
                }
            }

            if (Utils.Cfg_UseLogs) {
                instantiatedDisplay.name = $"{instantiatedDisplay.name} {instanceMult_}";
            }

            LimbMatcher limbMatcher = instantiatedDisplay.GetComponent<LimbMatcher>();
            if (limbMatcher && childLocator_)
            {
                limbMatcher.SetChildLocator(childLocator_);
            }
            
            ItemDisplay itemDisplay = instantiatedDisplay.GetComponent<ItemDisplay>();
            _allSillyItemDisplays.Add(itemDisplay);

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

        #region UpdatingMaterials
        private void onUpdateMaterials(CharacterModel characterModel_) 
        {
            if (characterModel_ != _swoocedCharacterModel)
                return;

            pseudoUpdateMaterials(characterModel_);
        }

        #region shouldJustUsedReflectionStupid
        private void onUpdateCamera(CharacterModel characterModel, CameraRigController camera) {

            if (characterModel != _swoocedCharacterModel)
                return;

            pseudoUpdateForCamera(camera);
        }

        private void pseudoUpdateForCamera(CameraRigController cameraRig) {

            _swoocedCharacterModel.visibility = VisibilityLevel.Visible;
            float target = 1f;
            if (_swoocedCharacterModel.body) {
                if (cameraRig.firstPersonTarget == _swoocedCharacterModel.body.gameObject) {
                    target = 0f;
                }
                _swoocedCharacterModel.visibility = _swoocedCharacterModel.body.GetVisibilityLevel(cameraRig.targetTeamIndex);
            }
            _pseudoFirstPersonFade = Mathf.MoveTowards(_pseudoFirstPersonFade, target, Time.deltaTime / 0.25f);
            _pseudoFade *= _pseudoFirstPersonFade;
        }

        private void pseudoRefreshCameraObstructors(CameraRigController cameraRig) {

            Vector3 position = cameraRig.transform.position;
            foreach (CharacterModel characterModel in InstanceTracker.GetInstancesList<CharacterModel>()) {
                if (cameraRig.enableFading) {
                    float nearestHurtBoxDistance = (position - _swoocedCharacterModel.transform.position).magnitude;
                    _pseudoFade = Mathf.Clamp01(Util.Remap(nearestHurtBoxDistance, cameraRig.fadeStartDistance, cameraRig.fadeEndDistance, 0f, 1f));
                } else {
                    _pseudoFade = 1f;
                }
            }
        }
        #endregion

        private void pseudoUpdateMaterials(CharacterModel characterModel_) {

            _allSillyItemDisplays.RemoveAll((item) => {
                return item == null;
            });

            if(_allSillyItemDisplays.Count <= 0) {
                return;
            }

            //float fade = characterModel_.GetFieldValue<float>("fade");

            float fade = (float)Utils.materialFadeField.GetValue(characterModel_);

            for (int i = 0; i < _allSillyItemDisplays.Count; i++) {

                ItemDisplay itemDisplay = _allSillyItemDisplays[i];
                if(itemDisplay == null) {

                    //Utils.Log($"itemDisplay {i} is null");
                    continue;
                }

                itemDisplay.SetVisibilityLevel(characterModel_.visibility);

                for (int l = 0; l < itemDisplay.rendererInfos.Length; l++) {
                    Renderer renderer2 = itemDisplay.rendererInfos[l].renderer;
                    renderer2.GetPropertyBlock(_propertyStorage);
                    _propertyStorage.SetColor(CommonShaderProperties._FlashColor, getDisplayColor(characterModel_));
                    _propertyStorage.SetFloat(CommonShaderProperties._Fade, fade);
                    renderer2.SetPropertyBlock(_propertyStorage);
                }
            }
        }

        private Color getDisplayColor(CharacterModel characterModel_) {

            Color color = Color.black;

            if (characterModel_.body && characterModel_.body.healthComponent) {

                float remainingHitFlash = Mathf.Clamp01(1f - characterModel_.body.healthComponent.timeSinceLastHit / hitFlashDuration);
                float remainingHealFlash = Mathf.Pow(Mathf.Clamp01(1f - characterModel_.body.healthComponent.timeSinceLastHeal / healFlashDuration), 0.5f);
                if (remainingHealFlash > remainingHitFlash) {
                    color = healFlashColor * remainingHealFlash;
                } else {
                    color = ((characterModel_.body.healthComponent.shield > 0f) ? hitFlashShieldColor : hitFlashBaseColor) * remainingHitFlash;
                }
            }
            return color;
        }


        #endregion

        #region cheatsvol2
        void Update()
        {
            if (Utils.Cfg_PlantsForHire)
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