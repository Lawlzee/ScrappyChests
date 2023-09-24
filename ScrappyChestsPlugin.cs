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

        private static WeightedSelection<PickupIndex> _originalSacrificeArtifactSelector;

        public static ConfigEntry<bool> ModEnabled;
        public static ConfigEntry<bool> ReplaceChestDropTable;
        public static ConfigEntry<bool> ReplaceMultiShopDropTable;
        public static ConfigEntry<bool> ReplaceAdaptiveChestDropTable;
        public static ConfigEntry<bool> ReplaceChanceShrineDropTable;
        public static ConfigEntry<bool> ReplaceBossDropTable;
        public static ConfigEntry<bool> ReplaceBossHunterDropTable;
        public static ConfigEntry<bool> ReplaceScavengerDropTable;
        public static ConfigEntry<bool> ReplaceCrashedMultishopDropTable;
        public static ConfigEntry<bool> ReplaceDoppelgangerDropTable;
        public static ConfigEntry<bool> ReplaceSacrificeArtifactDropTable;
        public static ConfigEntry<bool> ReplaceElderLemurianDropTable;
        public static ConfigEntry<bool> ReplaceVoidPotentialDropTable;
        public static ConfigEntry<bool> ReplaceSimulacrumOrbDropTable;
        public static ConfigEntry<bool> ReplaceVoidFieldsOrbDropTable;
        //todo: legendaryChest
        //todo: lockboc
        //todo: white
        //todo: green
        //todo: yellow
        //todo: red

        public void Awake()
        {
            Log.Init(Logger);

            On.RoR2.ChestBehavior.Roll += ChestBehavior_Roll;
            On.RoR2.ShopTerminalBehavior.GenerateNewPickupServer_bool += ShopTerminalBehavior_GenerateNewPickupServer_bool;
            On.RoR2.RouletteChestController.GenerateEntriesServer += RouletteChestController_GenerateEntriesServer;
            On.RoR2.ShrineChanceBehavior.AddShrineStack += ShrineChanceBehavior_AddShrineStack;
            On.RoR2.BossGroup.DropRewards += BossGroup_DropRewards;
            On.RoR2.EquipmentSlot.FireBossHunter += EquipmentSlot_FireBossHunter;
            On.EntityStates.ScavBackpack.Opening.FixedUpdate += Opening_FixedUpdate;
            On.RoR2.ChestBehavior.RollItem += ChestBehavior_RollItem;
            On.RoR2.FreeChestDropTable.GenerateDropPreReplacement += FreeChestDropTable_GenerateDropPreReplacement;
            On.RoR2.DoppelgangerDropTable.GenerateDropPreReplacement += DoppelgangerDropTable_GenerateDropPreReplacement;
            On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath += SacrificeArtifactManager_OnServerCharacterDeath;
            On.RoR2.MasterDropDroplet.DropItems += MasterDropDroplet_DropItems;
            On.RoR2.OptionChestBehavior.Roll += OptionChestBehavior_Roll;
            On.RoR2.InfiniteTowerWaveController.DropRewards += InfiniteTowerWaveController_DropRewards;
            On.RoR2.ArenaMonsterItemDropTable.GenerateUniqueDropsPreReplacement += ArenaMonsterItemDropTable_GenerateUniqueDropsPreReplacement;

            ModEnabled = Config.Bind<bool>("Configuration", "Mod enabled", true, "Mod enabled");
            ReplaceChestDropTable = Config.Bind<bool>("Configuration", "Replace Chest drops", true, "Chest will drop scrap instead of items");
            ReplaceMultiShopDropTable = Config.Bind<bool>("Configuration", "Replace Multishop Terminal drops", true, "Multishop Terminal will drop scrap instead of items");
            ReplaceAdaptiveChestDropTable = Config.Bind<bool>("Configuration", "Replace Adaptive Chest drops", true, "Adaptive Chest will drop scrap instead of items");
            ReplaceChanceShrineDropTable = Config.Bind<bool>("Configuration", "Replace Shrine of Chance drops", true, "Shrine of Chance will drop scrap instead of items");
            ReplaceBossDropTable = Config.Bind<bool>("Configuration", "Replace Boss drops", true, "Defeating a Boss will drop scrap instead of items");
            ReplaceBossHunterDropTable = Config.Bind<bool>("Configuration", "Replace Trophy Hunters Tricorn drops", true, "Trophy Hunter's Tricorn will drop scrap instead of items");
            ReplaceScavengerDropTable = Config.Bind<bool>("Configuration", "Replace Scavenger drops", true, "Scavenger will drop scrap instead of items");
            ReplaceCrashedMultishopDropTable = Config.Bind<bool>("Configuration", "Replace Crashed Multishop drops", true, "Crashed Multishop will drop scrap instead of items");
            ReplaceDoppelgangerDropTable = Config.Bind<bool>("Configuration", "Replace the Relentless Doppelganger drops", false, "The Relentless Doppelganger from the Artifact of Vengeance will drop scrap instead of items");
            ReplaceSacrificeArtifactDropTable = Config.Bind<bool>("Configuration", "Replace the Artifact of Sacrifice mob drops", true, "When using the Artifact of Sacrifice, the mobs will drop scrap instead of items");
            ReplaceElderLemurianDropTable = Config.Bind<bool>("Configuration", "Replace the Elite Elder Lemurian mob drops", false, "The Elite Elder Lemurian in the hidden chamber of Abandoned Aqueduct will drop scrap instead of bands");
            ReplaceVoidPotentialDropTable = Config.Bind<bool>("Configuration", "Replace Void Potential drops", true, "Void Potential will drop scrap instead of items");
            ReplaceSimulacrumOrbDropTable = Config.Bind<bool>("Configuration", "Replace the Simulacrum wave reward drops", true, "The orb reward after each wave of Simulacrum will drop scrap instead of items");
            ReplaceVoidFieldsOrbDropTable = Config.Bind<bool>("Configuration", "Replace the Void Fields wave reward drops", true, "The orb reward after each wave of the Void Fields will drop scrap instead of items");

            ModSettingsManager.AddOption(new CheckBoxOption(ModEnabled));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceChestDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceMultiShopDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceAdaptiveChestDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceChanceShrineDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceBossDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceBossHunterDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceScavengerDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceCrashedMultishopDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceDoppelgangerDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceSacrificeArtifactDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceElderLemurianDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceVoidPotentialDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceSimulacrumOrbDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceVoidFieldsOrbDropTable));

            //todo: add RiskOfOptions icon
        }

        private void ChestBehavior_Roll(On.RoR2.ChestBehavior.orig_Roll orig, ChestBehavior self)
        {
            if (ModEnabled.Value && ReplaceChestDropTable.Value)
            {
                if (self.dropTable)
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
            if (ModEnabled.Value && ReplaceMultiShopDropTable.Value)
            {
                if (self.serverMultiShopController != null)
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
            if (ModEnabled.Value && ReplaceBossDropTable.Value)
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
                return;
            }

            orig(self);
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

                List<PickupIndex> whiteScrap = new List<PickupIndex>
                {
                    PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex(RoR2Content.Items.ScrapWhite.name))
                };

                List<PickupIndex> greenScrap = new List<PickupIndex>
                {
                    PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex(RoR2Content.Items.ScrapGreen.name))
                };

                List<PickupIndex> redScrap = new List<PickupIndex>
                {
                    PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex(RoR2Content.Items.ScrapRed.name))
                };

                List<PickupIndex> lunarCoin = new List<PickupIndex>
                {
                    PickupCatalog.FindPickupIndex(RoR2Content.MiscPickups.LunarCoin.miscPickupIndex)
                };

                dropTable.Add(whiteScrap, chestBehavior.tier1Chance);
                dropTable.Add(greenScrap, chestBehavior.tier2Chance);
                dropTable.Add(redScrap, chestBehavior.tier3Chance);
                dropTable.Add(Run.instance.availableLunarCombinedDropList, chestBehavior.lunarChance / Run.instance.availableLunarCombinedDropList.Count);
                dropTable.Add(lunarCoin, chestBehavior.lunarCoinChance);


                Log.Debug("Opening_FixedUpdate BasicPickupDropTable replaced");

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
                using var  _ = ReplaceDropTable(self, nameof(FreeChestDropTable_GenerateDropPreReplacement));
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
                    string scrapPickupName = choice.value.pickupDef.itemTier switch
                    {
                        ItemTier.Tier1 => "ItemIndex.ScrapWhite",
                        ItemTier.Tier2 => "ItemIndex.ScrapGreen",
                        ItemTier.Tier3 => "ItemIndex.ScrapRed",
                        ItemTier.Boss => "ItemIndex.ScrapYellow",
                        _ => null
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
