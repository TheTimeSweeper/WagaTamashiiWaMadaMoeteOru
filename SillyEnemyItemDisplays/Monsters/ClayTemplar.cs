using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace SillyEnemyItemDisplays.Monsters {
    public class ClayTemplar : ItemDisplaysBase {

		protected override string bodyName => "ClayBruiserBody";

		protected override void SetLegacyItemDisplays(List<ItemDisplayRuleSet.NamedRuleGroup> list, List<ItemDisplayRuleSet.NamedRuleGroup> list2) {

			ItemDisplayRuleSet itemDisplayRuleSet3 = Resources.Load<GameObject>("Prefabs/CharacterBodies/LoaderBody").GetComponentInChildren<CharacterModel>().itemDisplayRuleSet;

            #region fromLoader
            list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "IncreaseHealing",
				displayRuleGroup = itemDisplayRuleSet3.FindDisplayRuleGroup(ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("IncreaseHealing")))
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "NovaOnHeal",
				displayRuleGroup = itemDisplayRuleSet3.FindDisplayRuleGroup(ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("NovaOnHeal")))
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "KillEliteFrenzy",
				displayRuleGroup = itemDisplayRuleSet3.FindDisplayRuleGroup(ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("KillEliteFrenzy")))
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Clover",
				displayRuleGroup = itemDisplayRuleSet3.FindDisplayRuleGroup(ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("Clover")))
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "GoldOnHit",
				displayRuleGroup = itemDisplayRuleSet3.FindDisplayRuleGroup(ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("GoldOnHit")))
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "ShieldOnly",
				displayRuleGroup = itemDisplayRuleSet3.FindDisplayRuleGroup(ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("ShieldOnly")))
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "HeadHunter",
				displayRuleGroup = itemDisplayRuleSet3.FindDisplayRuleGroup(ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("HeadHunter")))
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "GhostOnKill",
				displayRuleGroup = itemDisplayRuleSet3.FindDisplayRuleGroup(ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex("GhostOnKill")))
			});
            #endregion loader

            #region items
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
							localPos = new Vector3(0f, 0.4f, 0.25f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(0.5f, 0.5f, 0.5f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "AttackSpeedOnCrit",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayWolfPelt"),
							childName = "Head",
							localPos = new Vector3(0f, 0.5f, 0.067f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(1f, 1f, 1f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "JumpBoost",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayWaxBird"),
							childName = "Head",
							localPos = new Vector3(0f, -0.7f, -0.05f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(2f, 2f, 2f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Bandolier",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBandolier"),
							childName = "Muzzle",
							localPos = new Vector3(0.05f, -0.05f, -1.75f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(0.75f, 1f, 1f),
							limbMask = LimbFlags.None
						},
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBandolier"),
							childName = "Head",
							localPos = new Vector3(-0.2f, -0.5f, 0.25f),
							localAngles = new Vector3(-45f, 180f, 0f),
							localScale = new Vector3(1f, 2f, 1f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "DeathMark",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathMark"),
							childName = "Muzzle",
							localPos = new Vector3(0f, 0f, -0.25f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(0.1f, 0.1f, 0.1f),
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
							childName = "Head",
							localPos = new Vector3(0f, 0.1f, -0.25f),
							localAngles = new Vector3(-90f, 0f, 0f),
							localScale = new Vector3(1.5f, 1.5f, 1.5f),
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
							childName = "Head",
							localPos = new Vector3(-0.25f, 0f, 0f),
							localAngles = new Vector3(0f, 0f, 45f),
							localScale = new Vector3(0.25f, 0.25f, 0.25f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "BarrierOnOverHeal",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayAegis"),
							childName = "Muzzle",
							localPos = new Vector3(0f, 0.25f, -1f),
							localAngles = new Vector3(180f, 180f, 180f),
							localScale = new Vector3(0.75f, 0.75f, 0.75f),
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
							childName = "Muzzle",
							localPos = new Vector3(0f, 0f, -0.9f),
							localAngles = new Vector3(90f, 0f, 0f),
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
							childName = "Muzzle",
							localPos = new Vector3(0f, 0f, -2.75f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(0.7f, 0.7f, 0.7f),
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
							childName = "Muzzle",
							localPos = new Vector3(0f, 0f, -1f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(3.5f, 3.5f, 3.5f),
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
							childName = "Muzzle",
							localPos = new Vector3(0f, 0f, -1.2f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(3.5f, 3.5f, 3.5f),
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
							localPos = new Vector3(-0.4f, -0.5f, 0.15f),
							localAngles = new Vector3(-90f, 260f, 45f),
							localScale = new Vector3(0.8f, 0.6f, 0.6f),
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
							childName = "Head",
							localPos = new Vector3(0f, 0f, -0.4f),
							localAngles = new Vector3(0f, 180f, 0f),
							localScale = new Vector3(0.6f, 0.6f, 0.6f),
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
							childName = "Head",
							localPos = new Vector3(-0.4f, -0.5f, 0.15f),
							localAngles = new Vector3(-90f, -90f, 0f),
							localScale = new Vector3(1.5f, 1.5f, 1.5f),
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
							childName = "Head",
							localPos = new Vector3(0f, 0f, 0f),
							localAngles = new Vector3(-90f, -90f, 0f),
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
							childName = "Muzzle",
							localPos = new Vector3(0.4f, 0f, -1.75f),
							localAngles = new Vector3(0f, 85f, 90f),
							localScale = new Vector3(0.8f, 0.8f, 0.8f),
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
							childName = "Head",
							localPos = new Vector3(0f, 0f, -0.15f),
							localAngles = new Vector3(-60f, 0f, 0f),
							localScale = new Vector3(0.5f, 0.5f, 0.5f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "ArmorReductionOnHit",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayWarhammer"),
							childName = "Muzzle",
							localPos = new Vector3(0f, 0.25f, 0f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(1f, 1f, 1f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "FallBoots",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
							childName = "FootL",
							localPos = new Vector3(0f, -0.05f, 0f),
							localAngles = new Vector3(45f, 0f, 0f),
							localScale = new Vector3(0.25f, 0.25f, 0.25f),
							limbMask = LimbFlags.None
						},
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
							childName = "FootR",
							localPos = new Vector3(0f, -0.05f, 0f),
							localAngles = new Vector3(70f, 0f, 0f),
							localScale = new Vector3(0.25f, 0.25f, 0.25f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "BounceNearby",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayHook"),
							childName = "Muzzle",
							localPos = new Vector3(0f, 0f, -0.25f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(1f, 1f, 1f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "UtilitySkillMagazine",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
							childName = "Muzzle",
							localPos = new Vector3(0f, 0f, -0.75f),
							localAngles = new Vector3(0f, -90f, 90f),
							localScale = new Vector3(2f, 2f, 2f),
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
							childName = "FootL",
							localPos = new Vector3(0f, 0.05f, -0.15f),
							localAngles = new Vector3(50f, 180f, 180f),
							localScale = new Vector3(0.15f, 0.15f, 0.075f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "HealWhileSafe",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplaySnail"),
							childName = "FootR",
							localPos = new Vector3(0f, -0.05f, 0f),
							localAngles = new Vector3(0f, 0f, 180f),
							localScale = new Vector3(0.1f, 0.1f, 0.1f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "LunarPrimaryReplacement",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdEye"),
							childName = "Muzzle",
							localPos = new Vector3(0f, -0.1f, -0.25f),
							localAngles = new Vector3(-90f, 0f, 0f),
							localScale = new Vector3(1f, 1f, 1f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "SecondarySkillMagazine",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayDoubleMag"),
							childName = "Muzzle",
							localPos = new Vector3(0f, -0.8f, -2f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(-0.2f, -0.2f, -0.2f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Pearl",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayPearl"),
							childName = "Head",
							localPos = new Vector3(-1f, 0f, 0f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(0.2f, 0.2f, 0.2f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "ShinyPearl",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayShinyPearl"),
							childName = "Head",
							localPos = new Vector3(1f, 0f, 0f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(0.2f, 0.2f, 0.2f),
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
							childName = "Head",
							localPos = new Vector3(-0.5f, -1.2f, 0.25f),
							localAngles = new Vector3(0f, 0f, -20f),
							localScale = new Vector3(1f, 1f, 1f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Icicle",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayFrostRelic"),
							childName = "Root",
							localPos = new Vector3(-1f, 2f, -1f),
							localAngles = new Vector3(90f, 0f, 0f),
							localScale = new Vector3(2f, 2f, 2f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Talisman",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayTalisman"),
							childName = "Root",
							localPos = new Vector3(1f, 2f, -1f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(1f, 1f, 1f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "FocusConvergence",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayFocusedConvergence"),
							childName = "Root",
							localPos = new Vector3(0.5f, 2.5f, -3f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(0.2f, 0.2f, 0.2f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
            #endregion items

            #region quips
            list2.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Fruit",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayFruit"),
							childName = "Head",
							localPos = new Vector3(0.6f, -1.2f, 0f),
							localAngles = new Vector3(0f, -45f, 45f),
							localScale = new Vector3(0.75f, 0.75f, 0.75f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list2.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "BFG",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBFG"),
							childName = "Muzzle",
							localPos = new Vector3(0f, 0.2f, -0.25f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(0.75f, 0.75f, 0.75f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list2.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Meteor",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayMeteor"),
							childName = "Root",
							localPos = new Vector3(0f, 2f, -0.75f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(2f, 2f, 2f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list2.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Blackhole",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayGravCube"),
							childName = "Root",
							localPos = new Vector3(0f, 2f, -0.75f),
							localAngles = new Vector3(0f, 0f, 0f),
							localScale = new Vector3(1f, 1f, 1f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list2.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "Jetpack",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayBugWings"),
							childName = "Head",
							localPos = new Vector3(-0.15f, -0.25f, 0f),
							localAngles = new Vector3(0f, 45f, 0f),
							localScale = new Vector3(0.25f, 0.25f, 0.25f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
			list2.Add(new ItemDisplayRuleSet.NamedRuleGroup {
				name = "GoldGat",
				displayRuleGroup = new DisplayRuleGroup {
					rules = new ItemDisplayRule[]
					{
						new ItemDisplayRule
						{
							ruleType = ItemDisplayRuleType.ParentedPrefab,
							followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldGat"),
							childName = "Muzzle",
							localPos = new Vector3(1f, 0.8f, -1f),
							localAngles = new Vector3(45f, 90f, 0f),
							localScale = new Vector3(0.5f, 0.5f, 0.5f),
							limbMask = LimbFlags.None
						}
					}
				}
			});
#endregion quips
        }

        protected override void SetItemDisplayRules(List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules) {

		}
	}
}