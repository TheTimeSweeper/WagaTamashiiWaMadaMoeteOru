using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using System.Security;
using System.Security.Permissions;
using UnityEngine.AddressableAssets;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace SillyItemCensor
{
    [BepInPlugin("com.TheTimeSweeper.SillyItemCensor", "SillyItemCensor", "1.0.0")]
    public class SillyItemCensorPlugin : BaseUnityPlugin
    {
        private static string itemList;
        private string[] censoredItems;

        private Mesh cubeMesh;
        private AssetBundle bundle;

        private Dictionary<string, Sprite> cachedSprites = new Dictionary<string, Sprite>();

        void Awake()
        {
            On.RoR2.ItemCatalog.SetItemDefs += ItemCatalog_SetItemDefs;

            cubeMesh = Addressables.LoadAssetAsync<Mesh>("Decalicious/DecalCube.asset").WaitForCompletion();
            bundle = 
                AssetBundle.LoadFromFile(
                    System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName(Info.Location), 
                        "itemcensorer"));

            ReadConfig();
        }

        private string getItemTierSpriteName(ItemTier tier)
        {
            switch (tier)
            {
                case ItemTier.Tier1:
                case ItemTier.Tier2:
                case ItemTier.Tier3:
                case ItemTier.Lunar:
                case ItemTier.Boss:
                    return tier.ToString().ToLowerInvariant();
                case ItemTier.VoidTier1:
                case ItemTier.VoidTier2:
                case ItemTier.VoidTier3:
                case ItemTier.VoidBoss:
                    return "void";
                default:
                case ItemTier.NoTier:
                    return "consumed";
                case ItemTier.AssignedAtRuntime:
                    return "default";
            }
        }

        private Sprite getIconSprite(ItemTier tier)
        {
            string tierSprite = getItemTierSpriteName(tier);

            if (cachedSprites.ContainsKey(tierSprite))
            {
                return cachedSprites[tierSprite];
            }

            return cachedSprites[tierSprite] = bundle.LoadAsset<Sprite>(tierSprite);
        }

        private void ReadConfig()
        {
            itemList = Config.Bind("Hello", "CensoredItemsList", "Syringe", "List of Items to be censored, comma separated").Value;

            censoredItems = itemList.Replace(", ", ",").Split(',');
        }

        private void ItemCatalog_SetItemDefs(On.RoR2.ItemCatalog.orig_SetItemDefs orig, ItemDef[] newItemDefs)
        {
            for (int i = 0; i < newItemDefs.Length; i++)
            {
                ItemDef itemDef = newItemDefs[i];
                if (ItemIsCensored(itemDef))
                {
                    itemDef.pickupIconSprite = getIconSprite(itemDef.tier);

                    if (itemDef.pickupModelPrefab == null)
                        continue;

                    SkinnedMeshRenderer[] renderers = itemDef.pickupModelPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
                    for (int j = 0; j < renderers.Length; j++)
                    {
                        renderers[j].sharedMesh = cubeMesh;
                    }

                    MeshFilter[] filters = itemDef.pickupModelPrefab.GetComponentsInChildren<MeshFilter>();
                    for (int j = 0; j < filters.Length; j++)
                    {
                        filters[j].mesh = cubeMesh;
                    }
                }
            }

            orig(newItemDefs);
        }

        private bool ItemIsCensored(ItemDef itemDef)
        {
            for (int i = 0; i < censoredItems.Length; i++)
            {
                bool found = false;

                string itemEntry = censoredItems[i].ToLowerInvariant();

                if (itemDef.name.ToLowerInvariant().Contains(itemEntry))
                {
                    found = true;
                }

                string englishName = RoR2.Language.GetString(itemDef.nameToken, "en").ToLowerInvariant();
                if (englishName.Contains(itemEntry))
                {
                    found = true;
                }

                if (englishName.Replace(" ", "").Contains(itemEntry.Replace(" ", "")))
                {
                    found = true;
                }

                if (found)
                {
                    Logger.LogMessage($"Found item for entry {censoredItems[i]}, censoring {itemDef.name}");
                    return true;
                } 
            }
            return false;
        }
    }
}
