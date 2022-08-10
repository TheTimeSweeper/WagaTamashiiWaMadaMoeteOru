using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace SillyEnemyItemDisplays.Monsters {


    public class Aurelionite : StoneTitan {
        protected override string bodyName => "TitanGoldBody";
    }

    public class StoneTitan : ItemDisplaysBase {
		protected override string bodyName => "TitanBody";

		protected override void SetItemDisplayRules(List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules) {

			itemDisplayRules.Add(ItemDisplays.CreateGenericDisplayRule(RoR2Content.Items.CritGlasses,
																	   ItemDisplays.LoadDisplay("DisplayGlasses"),
																	   "Head",
																	   new Vector3(0.01005F, 1.27674F, 1.71633F),
																	   new Vector3(0F, 0F, 0F),
																	   new Vector3(3.02237F, 2.98254F, 2.88232F)));
		}

		protected override void SetLegacyItemDisplays(List<ItemDisplayRuleSet.NamedRuleGroup> itemList, List<ItemDisplayRuleSet.NamedRuleGroup> equipmentList) { }
	}
}