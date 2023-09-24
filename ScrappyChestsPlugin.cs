using BepInEx;
using BepInEx.Configuration;
using RiskOfOptions.Options;
using RiskOfOptions;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityStates.ScavBackpack;
using RoR2.Artifacts;

namespace ScrappyChests
{
    [BepInDependency("bubbet.zioriskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ScrappyChestsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Lawlzee";
        public const string PluginName = "ScrappyChests";
        public const string PluginVersion = "1.0.0";

        public static ConfigEntry<bool> ModEnabled;

        public static ConfigEntry<bool> ReplaceChestDropTable;
        public static ConfigEntry<bool> ReplaceMultiShopDropTable;
        public static ConfigEntry<bool> ReplaceAdaptiveChestDropTable;
        public static ConfigEntry<bool> ReplaceChanceShrineDropTable;
        public static ConfigEntry<bool> ReplaceLockboxDropTable;
        public static ConfigEntry<bool> ReplaceCrashedMultishopDropTable;
        public static ConfigEntry<bool> ReplaceLegendaryChestDropTable;
        public static ConfigEntry<bool> ReplaceVoidPotentialDropTable;
        public static ConfigEntry<bool> ReplaceLunarPodDropTable;
        public static ConfigEntry<bool> ReplaceLunarBudsDropTable;

        public static ConfigEntry<bool> ReplaceBossDropTable;
        public static ConfigEntry<bool> ReplaceBossHunterDropTable;
        public static ConfigEntry<bool> ReplaceAWUDropTable;
        public static ConfigEntry<bool> ReplaceScavengerDropTable;
        public static ConfigEntry<bool> ReplaceElderLemurianDropTable;

        public static ConfigEntry<bool> ReplaceDoppelgangerDropTable;
        public static ConfigEntry<bool> ReplaceSacrificeArtifactDropTable;

        public static ConfigEntry<bool> ReplaceSimulacrumOrbDropTable;
        public static ConfigEntry<bool> ReplaceVoidFieldsOrbDropTable;

        public static ConfigEntry<bool> ReplaceWhiteItems;
        public static ConfigEntry<bool> ReplaceGreenItems;
        public static ConfigEntry<bool> ReplaceRedItems;
        public static ConfigEntry<bool> ReplaceYellowItems;
        public static ConfigEntry<bool> ReplaceBlueItems;
        //todo: hidden chest

        public void Awake()
        {
            Log.Init(Logger);

            On.RoR2.ChestBehavior.Roll += ChestBehavior_Roll;
            On.RoR2.ShopTerminalBehavior.GenerateNewPickupServer_bool += ShopTerminalBehavior_GenerateNewPickupServer_bool;
            On.RoR2.RouletteChestController.GenerateEntriesServer += RouletteChestController_GenerateEntriesServer;
            On.RoR2.ShrineChanceBehavior.AddShrineStack += ShrineChanceBehavior_AddShrineStack;
            On.RoR2.FreeChestDropTable.GenerateDropPreReplacement += FreeChestDropTable_GenerateDropPreReplacement;
            On.RoR2.OptionChestBehavior.Roll += OptionChestBehavior_Roll;

            On.RoR2.BossGroup.DropRewards += BossGroup_DropRewards;
            On.RoR2.EquipmentSlot.FireBossHunter += EquipmentSlot_FireBossHunter;
            On.EntityStates.ScavBackpack.Opening.FixedUpdate += Opening_FixedUpdate;
            On.RoR2.MasterDropDroplet.DropItems += MasterDropDroplet_DropItems;
            On.RoR2.ChestBehavior.RollItem += ChestBehavior_RollItem;

            On.RoR2.DoppelgangerDropTable.GenerateDropPreReplacement += DoppelgangerDropTable_GenerateDropPreReplacement;
            On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath += SacrificeArtifactManager_OnServerCharacterDeath;

            On.RoR2.InfiniteTowerWaveController.DropRewards += InfiniteTowerWaveController_DropRewards;
            On.RoR2.ArenaMonsterItemDropTable.GenerateUniqueDropsPreReplacement += ArenaMonsterItemDropTable_GenerateUniqueDropsPreReplacement;

            ModEnabled = Config.Bind<bool>("Configuration", "Mod enabled", true, "Mod enabled");

            ReplaceChestDropTable = Config.Bind<bool>("Chests", "Chest", true, "Chest will drop scrap instead of items");
            ReplaceMultiShopDropTable = Config.Bind<bool>("Chests", "Multishop Terminal", true, "Multishop Terminal will drop scrap instead of items");
            ReplaceAdaptiveChestDropTable = Config.Bind<bool>("Chests", "Adaptive Chest", true, "Adaptive Chest will drop scrap instead of items");
            ReplaceChanceShrineDropTable = Config.Bind<bool>("Chests", "Shrine of Chance", true, "Shrine of Chance will drop scrap instead of items");
            ReplaceLegendaryChestDropTable = Config.Bind<bool>("Chests", "Legendary Chest", true, "Legendary Chest will drop scrap instead of items");
            ReplaceLockboxDropTable = Config.Bind<bool>("Items", "Rusted Key", false, "Lockboxs will drop scrap instead of items");
            ReplaceCrashedMultishopDropTable = Config.Bind<bool>("Items", "Crashed Multishop", false, "Crashed Multishop will drop scrap instead of items");
            ReplaceVoidPotentialDropTable = Config.Bind<bool>("Chests", "Void Potential", true, "Void Potential will drop scrap instead of items");
            ReplaceLunarPodDropTable = Config.Bind<bool>("Chests", "Lunar Pod", false, "Lunar Pod will drop Beads of Fealty instead of items");
            ReplaceLunarBudsDropTable = Config.Bind<bool>("Chests", "Lunar Bud", false, "Lunar Bud in the Bazaar Between Time will always sell Beads of Fealty");

            ReplaceBossDropTable = Config.Bind<bool>("Mobs", "Boss", true, "Defeating a Boss will drop scrap instead of items");
            ReplaceBossHunterDropTable = Config.Bind<bool>("Mobs", "Trophy Hunters Tricorn", false, "Trophy Hunter's Tricorn will drop scrap instead of items");
            ReplaceAWUDropTable = Config.Bind<bool>("Mobs", "Alloy Worship Unit", true, "Alloy Worship Unit will drop scrap instead of items");
            ReplaceScavengerDropTable = Config.Bind<bool>("Mobs", "Scavenger", false, "Scavenger will drop scrap instead of items");
            ReplaceElderLemurianDropTable = Config.Bind<bool>("Mobs", "Elite Elder Lemurian", false, "The Elite Elder Lemurian in the hidden chamber of Abandoned Aqueduct will drop scrap instead of bands");

            ReplaceDoppelgangerDropTable = Config.Bind<bool>("Artifacts", "Relentless Doppelganger", false, "The Relentless Doppelganger from the Artifact of Vengeance will drop scrap instead of items");
            ReplaceSacrificeArtifactDropTable = Config.Bind<bool>("Artifacts", "Artifact of Sacrifice", true, "When using the Artifact of Sacrifice, the mobs will drop scrap instead of items");

            ReplaceSimulacrumOrbDropTable = Config.Bind<bool>("Waves", "Simulacrum", true, "The orb reward after each wave of Simulacrum will drop scrap instead of items");
            ReplaceVoidFieldsOrbDropTable = Config.Bind<bool>("Waves", "Void Fields", true, "The orb reward after each wave of the Void Fields will drop scrap instead of items");

            ReplaceWhiteItems = Config.Bind<bool>("Tiers", "White item", true, "Replace white item drops with white scrap");
            ReplaceGreenItems = Config.Bind<bool>("Tiers", "Green item", true, "Replace green item drops with green scrap");
            ReplaceRedItems = Config.Bind<bool>("Tiers", "Red item", true, "Replace red item drops with red scrap");
            ReplaceYellowItems = Config.Bind<bool>("Tiers", "Yellow item", true, "Replace yellow item drops with yellow scrap");
            ReplaceBlueItems = Config.Bind<bool>("Tiers", "Blue item", true, "Replace blue item drops with Beads of Fealty");

            ModSettingsManager.AddOption(new CheckBoxOption(ModEnabled));

            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceChestDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceMultiShopDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceAdaptiveChestDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceChanceShrineDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceLockboxDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceCrashedMultishopDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceLegendaryChestDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceVoidPotentialDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceLunarPodDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceLunarBudsDropTable));

            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceBossDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceBossHunterDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceAWUDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceScavengerDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceElderLemurianDropTable));

            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceDoppelgangerDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceSacrificeArtifactDropTable));

            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceSimulacrumOrbDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceVoidFieldsOrbDropTable));

            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceWhiteItems));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceGreenItems));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceRedItems));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceYellowItems));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceBlueItems));

            //todo: add RiskOfOptions icon
        }

        private void ChestBehavior_Roll(On.RoR2.ChestBehavior.orig_Roll orig, ChestBehavior self)
        {
            if (ModEnabled.Value && self.dropTable)
            {
                if (self.dropTable.name == "dtGoldChest")
                {
                    if (ReplaceLegendaryChestDropTable.Value)
                    {
                        using var _ = ReplaceDropTable(self.dropTable, nameof(ChestBehavior_Roll));
                        orig(self);
                        return;
                    }
                }
                else if (self.dropTable.name == "dtLockbox")
                {
                    if (ReplaceLockboxDropTable.Value)
                    {
                        using var _ = ReplaceDropTable(self.dropTable, nameof(ChestBehavior_Roll));
                        orig(self);
                        return;
                    }
                }
                else if (self.dropTable.name == "dtLunarChest")
                {
                    if (ReplaceLunarPodDropTable.Value)
                    {
                        using var _ = ReplaceDropTable(self.dropTable, nameof(ChestBehavior_Roll));
                        orig(self);
                        return;
                    }
                }
                else if (ReplaceChestDropTable.Value)
                {
                    using var _ = ReplaceDropTable(self.dropTable, nameof(ChestBehavior_Roll));
                    orig(self);
                    return;
                }
            }
            orig(self);
        }

        private void ShopTerminalBehavior_GenerateNewPickupServer_bool(On.RoR2.ShopTerminalBehavior.orig_GenerateNewPickupServer_bool orig, ShopTerminalBehavior self, bool newHidden)
        {
            if (ModEnabled.Value)
            {
                if (ReplaceLunarBudsDropTable.Value && self.dropTable.name == "dtLunarChest")
                {
                    using var _ = ReplaceDropTable(self.dropTable, nameof(ShopTerminalBehavior_GenerateNewPickupServer_bool));
                    orig(self, newHidden);
                    return;
                }
                else if (ReplaceMultiShopDropTable.Value && self.serverMultiShopController != null)
                {
                    using var _ = ReplaceDropTable(self.dropTable, nameof(ShopTerminalBehavior_GenerateNewPickupServer_bool));
                    orig(self, newHidden);
                    return;
                }
            }

            orig(self, newHidden);
        }

        private void RouletteChestController_GenerateEntriesServer(On.RoR2.RouletteChestController.orig_GenerateEntriesServer orig, RouletteChestController self, Run.FixedTimeStamp startTime)
        {
            if (ModEnabled.Value && ReplaceAdaptiveChestDropTable.Value)
            {
                using var _ = ReplaceDropTable(self.dropTable, nameof(RouletteChestController_GenerateEntriesServer));
                orig(self, startTime);
                return;
            }

            orig(self, startTime);
        }

        private void ShrineChanceBehavior_AddShrineStack(On.RoR2.ShrineChanceBehavior.orig_AddShrineStack orig, ShrineChanceBehavior self, Interactor activator)
        {
            if (ModEnabled.Value && ReplaceChanceShrineDropTable.Value)
            {
                using var _ = ReplaceDropTable(self.dropTable, nameof(ShrineChanceBehavior_AddShrineStack));
                orig(self, activator);
                return;
            }
            orig(self, activator);
        }

        private void BossGroup_DropRewards(On.RoR2.BossGroup.orig_DropRewards orig, BossGroup self)
        {
            if (ModEnabled.Value)
            {
                if (self.bossDropChance == 0 && self.dropTable.name == "dtTier3Item" && self.bossDropTables.Count == 1 && self.bossDropTables[0].name == "dtBossRoboBallBoss")
                {
                    if (ReplaceAWUDropTable.Value)
                    {
                        Replace();
                        return;
                    }
                }
                else if (ReplaceBossDropTable.Value)
                {
                    Replace();
                    return;
                }
            }

            orig(self);

            void Replace()
            {
                using var disposables = new CompositeDisposable();
                if (self.dropTable)
                {
                    disposables.Add(ReplaceDropTable(self.dropTable, nameof(BossGroup_DropRewards)));
                }
                else
                {
                    Log.Info($"dropTable is {self.dropTable?.GetType().Name ?? "null"}");
                }

                foreach (PickupDropTable dropTable in self.bossDropTables.Distinct())
                {
                    disposables.Add(ReplaceDropTable(dropTable, nameof(BossGroup_DropRewards)));
                }

                orig(self);
            }
        }

        private bool EquipmentSlot_FireBossHunter(On.RoR2.EquipmentSlot.orig_FireBossHunter orig, EquipmentSlot self)
        {
            if (ModEnabled.Value && ReplaceBossHunterDropTable.Value)
            {
                self.UpdateTargets(DLC1Content.Equipment.BossHunter.equipmentIndex, true);
                HurtBox hurtBox = self.currentTarget.hurtBox;
                DeathRewards deathRewards = hurtBox?.healthComponent?.body?.gameObject?.GetComponent<DeathRewards>();
                if (!(bool)hurtBox || !(bool)deathRewards)
                    return false;

                using var _ = ReplaceDropTable(deathRewards.bossDropTable, nameof(EquipmentSlot_FireBossHunter));
                return orig(self);
            }

            return orig(self);
        }


        private void Opening_FixedUpdate(On.EntityStates.ScavBackpack.Opening.orig_FixedUpdate orig, Opening self)
        {
            if (ModEnabled.Value && ReplaceScavengerDropTable.Value)
            {
                var chestBehavior = self.GetComponent<ChestBehavior>();
                var dropTable = (BasicPickupDropTable)chestBehavior.dropTable;

                using var _ = CreateSelectorCopy(dropTable.selector, x => dropTable.selector = x);

                dropTable.selector.Clear();

                List<PickupIndex> lunarCoin = new List<PickupIndex>
                {
                    PickupCatalog.FindPickupIndex(RoR2Content.MiscPickups.LunarCoin.miscPickupIndex)
                };

                dropTable.Add(Run.instance.availableTier1DropList, chestBehavior.tier1Chance / Run.instance.availableTier1DropList.Count);
                dropTable.Add(Run.instance.availableTier2DropList, chestBehavior.tier2Chance / Run.instance.availableTier2DropList.Count);
                dropTable.Add(Run.instance.availableTier3DropList, chestBehavior.tier3Chance / Run.instance.availableTier3DropList.Count);
                dropTable.Add(Run.instance.availableLunarCombinedDropList, chestBehavior.lunarChance / Run.instance.availableLunarCombinedDropList.Count);
                dropTable.Add(lunarCoin, chestBehavior.lunarCoinChance);

                using var __ = ReplaceDropTable(dropTable, nameof(Opening_FixedUpdate));
                orig(self);
                return;
            }

            orig(self);
        }

        private void ChestBehavior_RollItem(On.RoR2.ChestBehavior.orig_RollItem orig, ChestBehavior self)
        {
            if (ModEnabled.Value && ReplaceScavengerDropTable.Value)
            {
                self.Roll();
                return;
            }
            orig(self);
        }

        private PickupIndex FreeChestDropTable_GenerateDropPreReplacement(On.RoR2.FreeChestDropTable.orig_GenerateDropPreReplacement orig, FreeChestDropTable self, Xoroshiro128Plus rng)
        {
            if (ModEnabled.Value && ReplaceCrashedMultishopDropTable.Value)
            {
                orig(self, rng);
                using var _ = ReplaceDropTable(self, nameof(FreeChestDropTable_GenerateDropPreReplacement));
                return PickupDropTable.GenerateDropFromWeightedSelection(rng, self.selector);
            }

            return orig(self, rng);
        }

        private PickupIndex DoppelgangerDropTable_GenerateDropPreReplacement(On.RoR2.DoppelgangerDropTable.orig_GenerateDropPreReplacement orig, DoppelgangerDropTable self, Xoroshiro128Plus rng)
        {
            if (ModEnabled.Value && ReplaceDoppelgangerDropTable.Value)
            {
                using var _ = ReplaceDropTable(self, nameof(DoppelgangerDropTable_GenerateDropPreReplacement));
                return orig(self, rng);
            }

            return orig(self, rng);
        }

        private void SacrificeArtifactManager_OnServerCharacterDeath(On.RoR2.Artifacts.SacrificeArtifactManager.orig_OnServerCharacterDeath orig, DamageReport damageReport)
        {
            if (ModEnabled.Value && ReplaceSacrificeArtifactDropTable.Value)
            {
                using var _ = ReplaceDropTable(SacrificeArtifactManager.dropTable, nameof(SacrificeArtifactManager_OnServerCharacterDeath));
                orig(damageReport);
                return;
            }

            orig(damageReport);
        }

        private void MasterDropDroplet_DropItems(On.RoR2.MasterDropDroplet.orig_DropItems orig, MasterDropDroplet self)
        {
            if (ModEnabled.Value && ReplaceElderLemurianDropTable.Value)
            {
                using CompositeDisposable disposable = new CompositeDisposable();
                foreach (var dropTable in self.dropTables)
                {
                    disposable.Add(ReplaceDropTable(dropTable, nameof(MasterDropDroplet_DropItems)));
                }
                orig(self);
                return;
            }

            orig(self);
        }

        private void OptionChestBehavior_Roll(On.RoR2.OptionChestBehavior.orig_Roll orig, OptionChestBehavior self)
        {
            if (ModEnabled.Value && ReplaceVoidPotentialDropTable.Value)
            {
                using var _ = ReplaceDropTable(self.dropTable, nameof(OptionChestBehavior_Roll));
                orig(self);
                return;
            }

            orig(self);
        }

        private void InfiniteTowerWaveController_DropRewards(On.RoR2.InfiniteTowerWaveController.orig_DropRewards orig, InfiniteTowerWaveController self)
        {
            if (ModEnabled.Value && ReplaceSimulacrumOrbDropTable.Value)
            {
                using var _ = ReplaceDropTable(self.rewardDropTable, nameof(InfiniteTowerWaveController_DropRewards));
                orig(self);
                return;
            }

            orig(self);
        }

        private PickupIndex[] ArenaMonsterItemDropTable_GenerateUniqueDropsPreReplacement(On.RoR2.ArenaMonsterItemDropTable.orig_GenerateUniqueDropsPreReplacement orig, ArenaMonsterItemDropTable self, int maxDrops, Xoroshiro128Plus rng)
        {
            if (ModEnabled.Value && ReplaceSimulacrumOrbDropTable.Value)
            {
                using var _ = ReplaceDropTable(self, nameof(ArenaMonsterItemDropTable_GenerateUniqueDropsPreReplacement));
                return orig(self, maxDrops, rng);
            }

            return orig(self, maxDrops, rng);
        }

        private IDisposable ReplaceDropTable(PickupDropTable dropTable, string caller)
        {
            if (dropTable is BasicPickupDropTable basicDropTable)
            {
                IDisposable disposable = CreateSelectorCopy(basicDropTable.selector, x => basicDropTable.selector = x);
                ReplaceDropTableSelector(basicDropTable.selector);
                return disposable;
            }

            if (dropTable is ExplicitPickupDropTable explicitDropTable)
            {
                IDisposable disposable = CreateSelectorCopy(explicitDropTable.weightedSelection, x => explicitDropTable.weightedSelection = x);
                ReplaceDropTableSelector(explicitDropTable.weightedSelection);
                return disposable;
            }

            if (dropTable is FreeChestDropTable freeChestDropTable)
            {
                IDisposable disposable = CreateSelectorCopy(freeChestDropTable.selector, x => freeChestDropTable.selector = x);
                ReplaceDropTableSelector(freeChestDropTable.selector);
                return disposable;
            }

            if (dropTable is DoppelgangerDropTable doppelgangerDropTable)
            {
                IDisposable disposable = CreateSelectorCopy(doppelgangerDropTable.selector, x => doppelgangerDropTable.selector = x);
                ReplaceDropTableSelector(doppelgangerDropTable.selector);
                return disposable;
            }

            if (dropTable is ArenaMonsterItemDropTable arenaMonsterItemDropTable)
            {
                IDisposable disposable = CreateSelectorCopy(arenaMonsterItemDropTable.selector, x => arenaMonsterItemDropTable.selector = x);
                ReplaceDropTableSelector(arenaMonsterItemDropTable.selector);
                return disposable;
            }

            Log.Warning($"Failed to override {caller} dropTable");
            Log.Warning($"{caller} dropTable is of type {dropTable?.GetType().FullName ?? "null"}");

            return Disposable.Empty;

            void ReplaceDropTableSelector(WeightedSelection<PickupIndex> selector)
            {
                for (int i = 0; i < selector.Count; i++)
                {
                    ref var choice = ref selector.choices[i];

                    bool replaceItem = choice.value.pickupDef.itemTier switch
                    {
                        ItemTier.Tier1 => ReplaceWhiteItems.Value,
                        ItemTier.Tier2 => ReplaceGreenItems.Value,
                        ItemTier.Tier3 => ReplaceRedItems.Value,
                        ItemTier.Boss => ReplaceYellowItems.Value,
                        ItemTier.Lunar => ReplaceBlueItems.Value,
                        _ => false
                    };

                    if (!replaceItem)
                    {
                        continue;
                    }

                    string scrapPickupName = choice.value.pickupDef.itemTier switch
                    {
                        ItemTier.Tier1 => "ItemIndex.ScrapWhite",
                        ItemTier.Tier2 => "ItemIndex.ScrapGreen",
                        ItemTier.Tier3 => "ItemIndex.ScrapRed",
                        ItemTier.Boss => "ItemIndex.ScrapYellow",
                        ItemTier.Lunar => "ItemIndex.LunarTrinket",
                        _ => throw new Exception("Unreachable")
                    };

                    if (scrapPickupName != null)
                    {
                        choice.value = PickupCatalog.FindPickupIndex(scrapPickupName);
                    }
                }

                Log.Debug($"{caller} {dropTable.GetType().Name} replaced");
            }
        }

        private IDisposable CreateSelectorCopy(WeightedSelection<PickupIndex> selector, Action<WeightedSelection<PickupIndex>> setSelector)
        {
            WeightedSelection<PickupIndex> oldSelector = selector;

            setSelector(new WeightedSelection<PickupIndex>
            {
                Capacity = selector.Capacity,
                choices = selector.choices.ToArray(),
                totalWeight = selector.totalWeight,
                Count = selector.Count
            });

            return new Disposable(() =>
            {
                setSelector(oldSelector);
            });
        }
    }
}
