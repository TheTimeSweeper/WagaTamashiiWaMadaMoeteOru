using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace SillyEnemyItemDisplays.Monsters {
    public class BeetleQueen : ItemDisplaysBase {

		protected override string bodyName => "BeetleQueen2Body";

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
							childName = "Head",
							localPos = new Vector3(0f, 1.5f, -1.5f),
							localAngles = new Vector3(0f, 180f, 0f),
							localScale = new Vector3(4f, 4f, 4f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Behemoth",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBehemoth"),
							childName = "Stomach",
							localPos = new Vector3(0f, -2.5f, 5f),
							localAngles = new Vector3(345f, 0f, 180f),
							localScale = new Vector3(1.5f, 1.5f, 1.5f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "ArmorPlate",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayRepulsionArmorPlate"),
							childName = "Butt",
							localPos = new Vector3(0.8f, -2.2f, -1.5f),
							localAngles = new Vector3(0f, 180f, 0f),
							localScale = new Vector3(7f, 5f, 5f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Bear",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBear"),
							childName = "Stomach",
							localPos = new Vector3(3f, 0f, 1f),
							localAngles = new Vector3(0f, 90f, 180f),
							localScale = new Vector3(5f, 5f, 5f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Medkit",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayMedkit"),
							childName = "Butt",
							localPos = new Vector3(0f, -3f, -2f),
							localAngles = new Vector3(0f, 180f, 0f),
							localScale = new Vector3(9f, 9f, 9f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Dagger",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayDagger"),
							childName = "Stomach",
							localPos = new Vector3(1f, -2f, 1f),
							localAngles = new Vector3(60f, 0f, 0f),
							localScale = new Vector3(8f, 8f, 8f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "BeetleGland",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBeetleGland"),
							childName = "Stomach",
							localPos = new Vector3(-3f, -2f, 0f),
							localAngles = new Vector3(0f, 180f, 90f),
							localScale = new Vector3(1.5f, 1.5f, 1.5f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "ShockNearby",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayTeslaCoil"),
							childName = "Stomach",
							localPos = new Vector3(0f, -3f, 1f),
							localAngles = new Vector3(315f, 0f, 180f),
							localScale = new Vector3(8f, 8f, 8f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Mushroom",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayMushroom"),
							childName = "Stomach",
							localPos = new Vector3(0f, -2f, 0f),
							localAngles = new Vector3(90f, 0f, 0f),
							localScale = new Vector3(2f, 2f, 2f),
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