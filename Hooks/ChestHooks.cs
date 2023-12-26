using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrappyChests;

public static class ChestHooks
{
    public static void Init()
    {
        On.RoR2.ChestBehavior.Roll += ChestBehavior_Roll;
        On.RoR2.ShopTerminalBehavior.GenerateNewPickupServer_bool += ShopTerminalBehavior_GenerateNewPickupServer_bool;
        On.RoR2.RouletteChestController.GenerateEntriesServer += RouletteChestController_GenerateEntriesServer;
        On.RoR2.ShrineChanceBehavior.AddShrineStack += ShrineChanceBehavior_AddShrineStack;
        On.RoR2.FreeChestDropTable.GenerateDropPreReplacement += FreeChestDropTable_GenerateDropPreReplacement;
        On.RoR2.OptionChestBehavior.Roll += OptionChestBehavior_Roll;
    }

    private static void ChestBehavior_Roll(On.RoR2.ChestBehavior.orig_Roll orig, ChestBehavior self)
    {
        if (Configuration.Instance.ModEnabled.Value && self.dropTable)
        {
            if (self.dropTable.name == "dtGoldChest")
            {
                if (Configuration.Instance.ReplaceLegendaryChestDropTable.Value)
                {
                    using var _ = self.dropTable.ReplaceDropTable(nameof(ChestBehavior_Roll));
                    orig(self);
                    return;
                }
            }
            else if (self.dropTable.name == "dtLockbox")
            {
                if (Configuration.Instance.ReplaceLockboxDropTable.Value)
                {
                    using var _ = self.dropTable.ReplaceDropTable(nameof(ChestBehavior_Roll));
                    orig(self);
                    return;
                }
            }
            else if (self.dropTable.name == "dtLunarChest")
            {
                if (Configuration.Instance.ReplaceLunarPodDropTable.Value)
                {
                    using var _ = self.dropTable.ReplaceDropTable(nameof(ChestBehavior_Roll));
                    orig(self);
                    return;
                }
            }
            else if (self.dropTable.name == "dtVoidChest")
            {
                if (Configuration.Instance.ReplaceVoidCradleDropTable.Value)
                {
                    using var _ = self.dropTable.ReplaceDropTable(nameof(ChestBehavior_Roll));
                    orig(self);
                    return;
                }
            }
            else if (Configuration.Instance.ReplaceChestDropTable.Value)
            {
                using var _ = self.dropTable.ReplaceDropTable(nameof(ChestBehavior_Roll));
                orig(self);
                return;
            }
        }
        orig(self);
    }

    private static void ShopTerminalBehavior_GenerateNewPickupServer_bool(On.RoR2.ShopTerminalBehavior.orig_GenerateNewPickupServer_bool orig, ShopTerminalBehavior self, bool newHidden)
    {
        if (Configuration.Instance.ModEnabled.Value)
        {
            if (self.serverMultiShopController == null)
            {
                if (Configuration.Instance.ReplaceLunarBudsDropTable.Value && self.dropTable.name == "dtLunarChest")
                {
                    using var _ = self.dropTable.ReplaceDropTable(nameof(ShopTerminalBehavior_GenerateNewPickupServer_bool));
                    orig(self, newHidden);
                    return;
                }

                if ((Configuration.Instance.AddVoidItemsToPrinters.Value && self.dropTable.name.StartsWith("dtDuplicator"))
                    || (Configuration.Instance.AddVoidItemsToCauldrons.Value && self.dropTable.name is "dtTier1Item" or "dtTier2Item" or "dtTier3Item")
                    || self.name == "VoidShopTerminal")
                {
                    BasicPickupDropTable basicDropTable = (BasicPickupDropTable)self.dropTable;

                    using IDisposable disposable = DropTableHelpers.CreateSelectorCopy(basicDropTable.selector, x => basicDropTable.selector = x);

                    if (self.name == "VoidShopTerminal")
                    {
                        basicDropTable.selector.Clear();
                    }

                    basicDropTable.Add(Run.instance.availableVoidTier1DropList, basicDropTable.tier1Weight);
                    basicDropTable.Add(Run.instance.availableVoidTier2DropList, basicDropTable.tier2Weight);
                    basicDropTable.Add(Run.instance.availableVoidTier3DropList, basicDropTable.tier3Weight);
                    basicDropTable.Add(Run.instance.availableVoidBossDropList, basicDropTable.bossWeight);

                    DropTableHelpers.UpdateSpeedItemsSpawnRate(basicDropTable.selector);

                    Log.Debug($"{nameof(ShopTerminalBehavior_GenerateNewPickupServer_bool)} {basicDropTable.GetType().Name} replaced");
                    orig(self, newHidden);
                    return;
                }

            }
            else if (Configuration.Instance.ReplaceMultiShopDropTable.Value)
            {
                using var _ = self.dropTable.ReplaceDropTable(nameof(ShopTerminalBehavior_GenerateNewPickupServer_bool));
                orig(self, newHidden);
                return;
            }
        }

        orig(self, newHidden);
    }

    private static void RouletteChestController_GenerateEntriesServer(On.RoR2.RouletteChestController.orig_GenerateEntriesServer orig, RouletteChestController self, Run.FixedTimeStamp startTime)
    {
        if (Configuration.Instance.ModEnabled.Value && Configuration.Instance.ReplaceAdaptiveChestDropTable.Value)
        {
            using var _ = self.dropTable.ReplaceDropTable(nameof(RouletteChestController_GenerateEntriesServer));
            orig(self, startTime);
            return;
        }

        orig(self, startTime);
    }

    private static void ShrineChanceBehavior_AddShrineStack(On.RoR2.ShrineChanceBehavior.orig_AddShrineStack orig, ShrineChanceBehavior self, Interactor activator)
    {
        if (Configuration.Instance.ModEnabled.Value && Configuration.Instance.ReplaceChanceShrineDropTable.Value)
        {
            using var _ = self.dropTable.ReplaceDropTable(nameof(ShrineChanceBehavior_AddShrineStack));
            orig(self, activator);
            return;
        }
        orig(self, activator);
    }

    private static PickupIndex FreeChestDropTable_GenerateDropPreReplacement(On.RoR2.FreeChestDropTable.orig_GenerateDropPreReplacement orig, FreeChestDropTable self, Xoroshiro128Plus rng)
    {
        if (Configuration.Instance.ModEnabled.Value && Configuration.Instance.ReplaceCrashedMultishopDropTable.Value)
        {
            orig(self, rng);
            using var _ = self.ReplaceDropTable(nameof(FreeChestDropTable_GenerateDropPreReplacement));
            return PickupDropTable.GenerateDropFromWeightedSelection(rng, self.selector);
        }

        return orig(self, rng);
    }

    private static void OptionChestBehavior_Roll(On.RoR2.OptionChestBehavior.orig_Roll orig, OptionChestBehavior self)
    {
        if (Configuration.Instance.ModEnabled.Value)
        {
            if (Configuration.Instance.ReplaceVoidPotentialDropTable.Value && self.dropTable.name == "dtVoidTriple")
            {
                using var _ = self.dropTable.ReplaceDropTable(nameof(OptionChestBehavior_Roll));
                orig(self);
                return;
            }

            if (Configuration.Instance.ReplaceEncrustedCacheDropTable.Value && self.dropTable.name == "dtVoidLockbox")
            {
                using var _ = self.dropTable.ReplaceDropTable(nameof(OptionChestBehavior_Roll));
                orig(self);
                return;
            }
        }

        orig(self);
    }
}
