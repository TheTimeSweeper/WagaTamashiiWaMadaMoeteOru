using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace SillyEnemyItemDisplays.Monsters {

    public class Beetle : ItemDisplaysBase{

        protected override string bodyName => "BeetleBody";

        protected override void SetLegacyItemDisplays(List<ItemDisplayRuleSet.NamedRuleGroup> list, List<ItemDisplayRuleSet.NamedRuleGroup> list2) {

			#region rob is a fucking madman I miss him
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
							localPos = new Vector3(0f, 0.25f, -0.2f),
							localAngles = new Vector3(320f, 180f, 0f),
							localScale = new Vector3(0.75f, 0.8f, 0.5f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Crowbar",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayCrowbar"),
							childName = "Chest",
							localPos = new Vector3(0.5f, 0f, -0.6f),
							localAngles = new Vector3(0f, 45f, 0f),
							localScale = new Vector3(1f, 1f, 1f),
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
							childName = "Chest",
							localPos = new Vector3(0f, 0.75f, -1.5f),
							localAngles = new Vector3(0f, 180f, 0f),
							localScale = new Vector3(0.5f, 0.5f, 0.5f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "NearbyDamageBonus",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayDiamond"),
							childName = "Head",
							localPos = new Vector3(0f, 0.75f, 0f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(0.5f, 0.5f, 0.5f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "FireRing",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayFireRing"),
							childName = "HandL",
							localPos = new Vector3(0f, 0f, 0f),
							localAngles = new Vector3(90f, 0f, 0f),
							localScale = new Vector3(1f, 1f, 1f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "IceRing",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayIceRing"),
							childName = "HandR",
							localPos = new Vector3(0f, 0f, 0f),
							localAngles = new Vector3(90f, 0f, 0f),
							localScale = new Vector3(1f, 1f, 1f),
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
							childName = "Head",
							localPos = new Vector3(0.25f, 0f, 0.75f),
							localAngles = new Vector3(35f, 0f, 180f),
							localScale = new Vector3(2.5f, 1f, 1f),
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
							childName = "Chest",
							localPos = new Vector3(0.6f, 0.2f, -0.5f),
							localAngles = new Vector3(0f, 110f, 0f),
							localScale = new Vector3(1f, 1f, 1f),
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
							childName = "Chest",
							localPos = new Vector3(0f, 0f, -0.75f),
							localAngles = new Vector3(270f, 180f, 0f),
							localScale = new Vector3(2f, 2f, 2f),
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
							childName = "Chest",
							localPos = new Vector3(0f, 0.8f, 0f),
							localAngles = new Vector3(315f, 0f, 0f),
							localScale = new Vector3(2f, 2f, 2f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "ChainLightning",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayUkulele"),
							childName = "Chest",
							localPos = new Vector3(-0.5f, 0f, -0.6f),
							localAngles = new Vector3(0f, 215f, 0f),
							localScale = new Vector3(2f, 2f, 2f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Syringe",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplaySyringeCluster"),
							childName = "Chest",
							localPos = new Vector3(0f, 0.5f, 0.4f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(0.25f, 0.25f, 0.25f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Hoof",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayHoof"),
							childName = "CalfR",
							localPos = new Vector3(0f, 0.45f, -0.025f),
							localAngles = new Vector3(90f, 180f, 0f),
							localScale = new Vector3(0.1f, 0.125f, 0.1f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "SprintOutOfCombat",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayWhip"),
							childName = "Chest",
							localPos = new Vector3(-0.6f, -0.5f, 0f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(1f, 1f, 1f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Seed",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplaySeed"),
							childName = "Head",
							localPos = new Vector3(0f, 0f, 0.4f),
							localAngles = new Vector3(0f, 0f, 180f),
							localScale = new Vector3(0.25f, 0.25f, 0.25f),
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
							childName = "Chest",
							localPos = new Vector3(0.8f, 0f, -0.25f),
							localAngles = new Vector3(90f, 315f, 0f),
							localScale = new Vector3(0.4f, 0.4f, 0.4f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "WarCryOnMultiKill",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayPauldron"),
							childName = "Chest",
							localPos = new Vector3(0f, 0.5f, -0.7f),
							localAngles = new Vector3(270f, 0f, 0f),
							localScale = new Vector3(3f, 2f, 2f),
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
							childName = "Chest",
							localPos = new Vector3(0f, 0f, -0.6f),
							localAngles = new Vector3(270f, 0f, 0f),
							localScale = new Vector3(2f, 2f, 2f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			#endregion rob i love u
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000022F0 File Offset: 0x000004F0
		protected override void SetItemDisplayRules(List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules) {
			//itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup {
			//	keyAsset = RoR2Content.Items.AlienHead,
			//	displayRuleGroup = new DisplayRuleGroup {
			//		rules = new ItemDisplayRule[] {
			//			new ItemDisplayRule
			//			{
			//				ruleType = ItemDisplayRuleType.ParentedPrefab,
			//				followerPrefab = ItemDisplays.LoadDisplay("DisplayGlasses"),
			//				childName = "Head",
			//				localPos = new Vector3(0f, 0.25f, -0.2f),
			//				localAngles = new Vector3(320f, 180f, 0f),
			//				localScale = new Vector3(0.75f, 0.8f, 0.5f),
			//				limbMask = LimbFlags.None
			//			}
			//		}
			//	}
			//});
		}
    }
}