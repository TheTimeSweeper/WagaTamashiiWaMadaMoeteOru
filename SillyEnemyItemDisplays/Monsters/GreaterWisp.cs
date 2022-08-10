using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace SillyEnemyItemDisplays.Monsters {
    public class GreaterWisp : ItemDisplaysBase {

		protected override string bodyName => "GreaterWispBody";

		protected override void SetLegacyItemDisplays(List<ItemDisplayRuleSet.NamedRuleGroup> list, List<ItemDisplayRuleSet.NamedRuleGroup> list2) {
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "CritGlasses",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayGlasses"),
							childName = "MaskBase",
							localPos = new Vector3(0f, 0.2f, 0.7f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(2f, 2f, 2f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "GhostOnKill",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayMask"),
							childName = "MaskBase",
							localPos = new Vector3(0f, 0.4f, 0.2f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(5f, 4f, 4f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
		}

		protected override void SetItemDisplayRules(List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules) {

		}
	}
}