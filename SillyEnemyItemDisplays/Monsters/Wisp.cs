using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace SillyEnemyItemDisplays.Monsters {
    public class Wisp : ItemDisplaysBase {

		protected override string bodyName => "WispBody";

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
							childName = "Muzzle",
							localPos = new Vector3(0f, 0.15f, 0.1f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(1.25f, 1.25f, 1.25f),
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
							childName = "Muzzle",
							localPos = new Vector3(0f, 0.25f, -0.5f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(3f, 3f, 3f),
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