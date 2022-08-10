using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SillyEnemyItemDisplays {
    public abstract class ItemDisplaysBase {

        protected abstract string bodyName { get; }
        protected CharacterModel bodyCharacterModel { get; private set; }

        public virtual ItemDisplaysBase Init() {

            bodyCharacterModel = BodyCatalog.FindBodyPrefab(bodyName).GetComponentInChildren<CharacterModel>();
            SetItemDisplays();

            return this;
        }

        private void SetItemDisplays() {

            ItemDisplayRuleSet characterItemDisplayRuleSet = bodyCharacterModel.itemDisplayRuleSet;
            List<ItemDisplayRuleSet.KeyAssetRuleGroup> newItemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            //grab rob's old displays - not gonna waste time doing all that again
            LegacyConvertRules(characterItemDisplayRuleSet);

            //grab pre-existing displays (namely elite displays) and rob's displays from above
            RecoverExistingItemDisplays(characterItemDisplayRuleSet, newItemDisplayRules);

            //set new display rules here
            SetItemDisplayRules(newItemDisplayRules);

            bodyCharacterModel.itemDisplayRuleSet.keyAssetRuleGroups = newItemDisplayRules.ToArray();
        }

        public void thefuck() {
            Log.Warning(bodyCharacterModel.itemDisplayRuleSet.keyAssetRuleGroups.Length);
        }

        private void RecoverExistingItemDisplays(ItemDisplayRuleSet characterItemDisplayRuleSet, List<ItemDisplayRuleSet.KeyAssetRuleGroup> newItemDisplayRules) {
            //Log.Warning("attempt recover rules " + characterItemDisplayRuleSet.keyAssetRuleGroups.Length);
            for (int i = 0; i < characterItemDisplayRuleSet.keyAssetRuleGroups.Length; i++) {
                
                ItemDisplayRuleSet.KeyAssetRuleGroup group = characterItemDisplayRuleSet.keyAssetRuleGroups[i];
                if (group.displayRuleGroup.rules == null) {
                    Log.Warning($"{bodyCharacterModel.name}: keyasset {group.keyAsset.name} null");
                    continue;
                }

                newItemDisplayRules.Add(group);
                //Log.Warning($"{bodyCharacterModel.name}: recovering display rules for {group.keyAsset.name}");
            }
        }
        
        private void LegacyConvertRules(ItemDisplayRuleSet characterItemDisplayRuleSet) {

            List<ItemDisplayRuleSet.NamedRuleGroup> itemRules = new List<ItemDisplayRuleSet.NamedRuleGroup>();
            List<ItemDisplayRuleSet.NamedRuleGroup> equipmentRules = new List<ItemDisplayRuleSet.NamedRuleGroup>();

            SetLegacyItemDisplays(itemRules, equipmentRules);

            if (itemRules.Count != 0) {
                characterItemDisplayRuleSet.namedItemRuleGroups = itemRules.ToArray();
            }
            if(equipmentRules.Count != 0) {
                characterItemDisplayRuleSet.namedEquipmentRuleGroups = equipmentRules.ToArray();
            }

            if (itemRules.Count == 0 && equipmentRules.Count == 0)
                return;

            characterItemDisplayRuleSet.UpdgradeToAssetKeying();
        }

        protected abstract void SetLegacyItemDisplays(List<ItemDisplayRuleSet.NamedRuleGroup> itemList, List<ItemDisplayRuleSet.NamedRuleGroup> equipmentList);

        protected abstract void SetItemDisplayRules(List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules);
    }
}