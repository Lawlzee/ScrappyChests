using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine;
using HarmonyLib;

namespace ScrappyChests;

public static class CauldronHooks
{
    private static readonly CostTypeIndex _yellowSoupCostIndex = (CostTypeIndex)81273;
    private static CostTypeDef _yellowSoupCostTypeDef;

    public static void Init()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        On.RoR2.CostTypeCatalog.GetCostTypeDef += CostTypeCatalog_GetCostTypeDef;
        On.RoR2.SceneObjectToggleGroup.Awake += SceneObjectToggleGroup_Awake;

        _yellowSoupCostTypeDef = new CostTypeDef()
        {
            costStringFormatToken = "COST_ITEM_FORMAT",
            buildCostString = (def, context) =>
            {
                var cost = Configuration.Instance.YellowCauldronCost;
                List<string> costParts = new List<string>();

                AppendCost(cost.White, "white");
                AppendCost(cost.Green, "green");
                AppendCost(cost.Red, "red");
                AppendCost(cost.Yellow, "yellow");

                if (costParts.Count == 0)
                {
                    context.stringBuilder.Append("free");
                    return;
                }

                context.stringBuilder.Append(string.Join(", ", costParts));

                void AppendCost(int count, string tier)
                {
                    if (count > 0)
                    {
                        string plural = count > 1 ? "s" : "";
                        costParts.Add($"{count} {tier}{plural}");
                    }
                }
            },
            saturateWorldStyledCostString = true,
            isAffordable = (CostTypeDef costTypeDef, CostTypeDef.IsAffordableContext context) =>
            {
                CharacterBody component = context.activator.GetComponent<CharacterBody>();
                if (component)
                {
                    Inventory inventory = component.inventory;
                    if (inventory)
                    {
                        var cost = Configuration.Instance.YellowCauldronCost;
                        return HasAtLeastXTotalItemsOfTier(ItemTier.Tier1, cost.White)
                            && HasAtLeastXTotalItemsOfTier(ItemTier.Tier2, cost.Green)
                            && HasAtLeastXTotalItemsOfTier(ItemTier.Tier3, cost.Red)
                            && HasAtLeastXTotalItemsOfTier(ItemTier.Boss, cost.Yellow);
                    }
                }
                return false;

                bool HasAtLeastXTotalItemsOfTier(ItemTier itemTier, int x)
                {
                    if (x == 0)
                    {
                        return true;
                    }

                    return component.inventory.HasAtLeastXTotalItemsOfTier(itemTier, x);
                }
            },
            payCost = (CostTypeDef costTypeDef, CostTypeDef.PayCostContext context) =>
            {
                var cost = Configuration.Instance.YellowCauldronCost;

                PayCost(CostTypeIndex.WhiteItem, cost.White);
                PayCost(CostTypeIndex.GreenItem, cost.Green);
                PayCost(CostTypeIndex.RedItem, cost.Red);
                PayCost(CostTypeIndex.BossItem, cost.Yellow);

                context.cost = cost.White + cost.Green + cost.Red + cost.Yellow;

                void PayCost(CostTypeIndex costTypeIndex, int cost)
                {
                    if (cost == 0)
                    {
                        return;
                    }

                    context.cost = cost;

                    var costTypeDef = CostTypeCatalog.GetCostTypeDef(costTypeIndex);
                    costTypeDef.payCost(costTypeDef, context);
                }
            },
            colorIndex = ColorCatalog.ColorIndex.BossItem,
            itemTier = ItemTier.Boss
        };
    }

    private static CostTypeDef CostTypeCatalog_GetCostTypeDef(On.RoR2.CostTypeCatalog.orig_GetCostTypeDef orig, CostTypeIndex costTypeIndex)
    {
        if (costTypeIndex == _yellowSoupCostIndex)
        {
            return _yellowSoupCostTypeDef;
        }

        return orig(costTypeIndex);
    }

    private static void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (!Configuration.Instance.ModEnabled.Value)
        {
            return;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name != "bazaar")
        {
            return;
        }

        if (Configuration.Instance.AddWhiteCauldronToBazaar.Value)
        {
            GameObject whiteSoup = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarCauldrons/LunarCauldron, RedToWhite Variant.prefab").WaitForCompletion());
            whiteSoup.transform.position = new Vector3(-95.98551f, -24.7408f, -43.01519f);
            whiteSoup.transform.rotation = Quaternion.Euler(0, -45f, 0);

            NetworkServer.Spawn(whiteSoup);
        }

        if (Configuration.Instance.AddYellowCauldronToBazaar.Value)
        {
            GameObject yellowSoup = CreateYellowSoup(new Vector3(-103.5473f, -25.05961f, -40.06012f), Quaternion.Euler(0, -10f, 0));
            NetworkServer.Spawn(yellowSoup);
        }
    }

    private static void SceneObjectToggleGroup_Awake(On.RoR2.SceneObjectToggleGroup.orig_Awake orig, SceneObjectToggleGroup self)
    {
        if (Configuration.Instance.ModEnabled.Value && Configuration.Instance.AddYellowCauldronToMoon.Value)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name == "moon2")
            {
                self.toggleGroups = self.toggleGroups.AddToArray(new GameObjectToggleGroup
                {
                    objects = [
                        CreateYellowSoup(new Vector3(-175.1659f, -190.8372f, -348.236f), Quaternion.Euler(0, 180, 0)),
                        CreateYellowSoup(new Vector3(-211.1335f, -142.8229f, -327.5427f), Quaternion.Euler(0, 45, 0)),
                        CreateYellowSoup(new Vector3(-280.2209f, -189.0418f, -324.9852f), Quaternion.Euler(0, 0, 0))
                    ],
                    minEnabled = 0,
                    maxEnabled = 2
                });
            }
        }

        orig(self);
    }

    private static GameObject CreateYellowSoup(Vector3 position, Quaternion rotation)
    {
        GameObject yellowSoup = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarCauldrons/LunarCauldron, WhiteToGreen.prefab").WaitForCompletion());
        yellowSoup.transform.position = position;
        yellowSoup.transform.rotation = rotation;

        PurchaseInteraction purchaseInteraction = yellowSoup.GetComponent<PurchaseInteraction>();
        purchaseInteraction.costType = _yellowSoupCostIndex;
        (int White, int Green, int Red, int Yellow) cost = Configuration.Instance.YellowCauldronCost;
        purchaseInteraction.cost = cost.White + cost.Green + cost.Red + cost.Yellow;

        ShopTerminalBehavior terminalBehavior = yellowSoup.GetComponent<ShopTerminalBehavior>();
        terminalBehavior.dropTable = Addressables.LoadAssetAsync<BasicPickupDropTable>("RoR2/Base/DuplicatorWild/dtDuplicatorWild.asset").WaitForCompletion();
        terminalBehavior.itemTier = ItemTier.Boss;

        return yellowSoup;
    }
}
