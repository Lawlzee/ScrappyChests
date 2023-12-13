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
using UnityEngine;
using System.IO;
using RiskOfOptions.OptionConfigs;
using UnityEngine.AddressableAssets;

namespace ScrappyChests
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ScrappyChestsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "Lawlzee.ScrappyChests";
        public const string PluginAuthor = "Lawlzee";
        public const string PluginName = "Scrappy Chests";
        public const string PluginVersion = "1.2";

        private Dictionary<string, (ConfigEntry<float> Config, GameObject SoupPrefab)> _printerConfigBySpawnCard;

        public static ConfigEntry<bool> ModEnabled;

        public static ConfigEntry<bool> ReplaceChestDropTable;
        public static ConfigEntry<bool> ReplaceMultiShopDropTable;
        public static ConfigEntry<bool> ReplaceAdaptiveChestDropTable;
        public static ConfigEntry<bool> ReplaceChanceShrineDropTable;
        public static ConfigEntry<bool> ReplaceLegendaryChestDropTable;
        public static ConfigEntry<bool> ReplaceVoidPotentialDropTable;
        public static ConfigEntry<bool> ReplaceVoidCradleDropTable;
        public static ConfigEntry<bool> ReplaceLunarPodDropTable;
        public static ConfigEntry<bool> ReplaceLunarBudsDropTable;

        public static ConfigEntry<float> WhitePrinterSpawnMultiplier;
        public static ConfigEntry<float> GreenPrinterSpawnMultiplier;
        public static ConfigEntry<float> RedPrinterSpawnMultiplier;
        public static ConfigEntry<float> YellowPrinterSpawnMultiplier;
        public static ConfigEntry<bool> AddVoidItemsToPrinters;
        public static ConfigEntry<bool> AddVoidItemsToCauldrons;

        public static ConfigEntry<bool> ReplaceLockboxDropTable;
        public static ConfigEntry<bool> ReplaceEncrustedCacheDropTable;
        public static ConfigEntry<bool> ReplaceCrashedMultishopDropTable;
        public static ConfigEntry<bool> ReplaceBossHunterDropTable;
        public static ConfigEntry<float> SpeedItemSpawnMultiplier;

        public static ConfigEntry<bool> ReplaceBossDropTable;
        public static ConfigEntry<bool> ReplaceAWUDropTable;
        public static ConfigEntry<bool> ReplaceScavengerDropTable;
        public static ConfigEntry<bool> ReplaceElderLemurianDropTable;
        
        public static ConfigEntry<bool> ReplaceNewtAltarsCost;

        public static ConfigEntry<bool> ReplaceDoppelgangerDropTable;
        public static ConfigEntry<bool> ReplaceSacrificeArtifactDropTable;

        public static ConfigEntry<bool> ReplaceSimulacrumOrbDropTable;
        public static ConfigEntry<bool> ReplaceVoidFieldsOrbDropTable;

        public static ConfigEntry<bool> ReplaceWhiteItems;
        public static ConfigEntry<bool> ReplaceGreenItems;
        public static ConfigEntry<bool> ReplaceRedItems;
        public static ConfigEntry<bool> ReplaceYellowItems;
        public static ConfigEntry<bool> ReplaceBlueItems;
        public static ConfigEntry<bool> ReplaceVoidTier1Items;
        public static ConfigEntry<bool> ReplaceVoidTier2Items;
        public static ConfigEntry<bool> ReplaceVoidTier3Items;
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

            On.RoR2.SceneDirector.GenerateInteractableCardSelection += SceneDirector_GenerateInteractableCardSelection;

            On.RoR2.EquipmentSlot.FireBossHunter += EquipmentSlot_FireBossHunter;

            On.RoR2.BossGroup.DropRewards += BossGroup_DropRewards;
            On.EntityStates.ScavBackpack.Opening.FixedUpdate += Opening_FixedUpdate;
            On.RoR2.MasterDropDroplet.DropItems += MasterDropDroplet_DropItems;
            On.RoR2.ChestBehavior.RollItem += ChestBehavior_RollItem;

            On.RoR2.PurchaseInteraction.GetContextString += PurchaseInteraction_GetContextString;
            On.RoR2.PurchaseInteraction.CanBeAffordedByInteractor += PurchaseInteraction_CanBeAffordedByInteractor;
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin; ;
            On.RoR2.PurchaseInteraction.UpdateHologramContent += PurchaseInteraction_UpdateHologramContent;
            On.RoR2.UI.PingIndicator.RebuildPing += PingIndicator_RebuildPing;

            On.RoR2.DoppelgangerDropTable.GenerateDropPreReplacement += DoppelgangerDropTable_GenerateDropPreReplacement;
            On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath += SacrificeArtifactManager_OnServerCharacterDeath;

            On.RoR2.InfiniteTowerWaveController.DropRewards += InfiniteTowerWaveController_DropRewards;
            On.RoR2.ArenaMonsterItemDropTable.GenerateUniqueDropsPreReplacement += ArenaMonsterItemDropTable_GenerateUniqueDropsPreReplacement;
            On.RoR2.ArenaMissionController.EndRound += ArenaMissionController_EndRound;

            ModEnabled = Config.Bind("Configuration", "Mod enabled", true, "Mod enabled");

            ReplaceChestDropTable = Config.Bind("Chests", "Chest", true, "Chests will drop scrap instead of items");
            ReplaceMultiShopDropTable = Config.Bind("Chests", "Multishop Terminal", true, "Multishop Terminals will drop scrap instead of items");
            ReplaceAdaptiveChestDropTable = Config.Bind("Chests", "Adaptive Chest", true, "Adaptive Chests will drop scrap instead of items");
            ReplaceChanceShrineDropTable = Config.Bind("Chests", "Shrine of Chance", true, "Shrine of Chance will drop scrap instead of items");
            ReplaceLegendaryChestDropTable = Config.Bind("Chests", "Legendary Chest", true, "Legendary Chests will drop scrap instead of items");
            ReplaceVoidPotentialDropTable = Config.Bind("Chests", "Void Potential", true, "Void Potential will drop scrap instead of items");
            ReplaceVoidCradleDropTable = Config.Bind("Chests", "Void Cradle", true, "Void Cradle will drop scrap instead of items");
            ReplaceLunarPodDropTable = Config.Bind("Chests", "Lunar Pod", false, "Lunar Pods will drop Beads of Fealty instead of items");
            ReplaceLunarBudsDropTable = Config.Bind("Chests", "Lunar Bud", false, "Lunar Buds in the Bazaar Between Time will always sell Beads of Fealty");

            WhitePrinterSpawnMultiplier = Config.Bind("Printers", "White printer spawn multiplier", 1.5f, "Controls the spawn rate of white printers. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn printers.");
            GreenPrinterSpawnMultiplier = Config.Bind("Printers", "Green printer spawn multiplier", 2.5f, "Controls the spawn rate of green printers. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn printers.");
            RedPrinterSpawnMultiplier = Config.Bind("Printers", "Red printer spawn multiplier", 3f, "Controls the spawn rate of ref printers. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn printers.");
            YellowPrinterSpawnMultiplier = Config.Bind("Printers", "Yellow printer spawn multiplier", 3f, "Controls the spawn rate of yellow printers. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn printers.");
            AddVoidItemsToPrinters = Config.Bind("Printers", "Add void items to Printers", true, "Add void items to Printers");
            AddVoidItemsToCauldrons = Config.Bind("Printers", "Add void items to Cauldrons", true, "Add void items to Cauldrons");

            ReplaceLockboxDropTable = Config.Bind("Items", "Rusted Key", false, "Lockboxes will drop scrap instead of items");
            ReplaceEncrustedCacheDropTable = Config.Bind("Items", "Encrusted Key", false, "Encrusted Cache will drop scrap instead of items");
            ReplaceCrashedMultishopDropTable = Config.Bind("Items", "Crashed Multishop", false, "Crashed Multishop will drop scrap instead of items");
            ReplaceBossHunterDropTable = Config.Bind("Items", "Trophy Hunters Tricorn", false, "Trophy Hunter's Tricorn will drop scrap instead of items");
            SpeedItemSpawnMultiplier = Config.Bind("Items", "Speed items spawn multiplier", 1.25f, "Controls the spawn rate of speed items. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn speed items.");

            ReplaceBossDropTable = Config.Bind("Mobs", "Boss", true, "Defeating a Boss will drop scrap instead of items");
            ReplaceAWUDropTable = Config.Bind("Mobs", "Alloy Worship Unit", true, "Alloy Worship Unit will drop scrap instead of items");
            ReplaceScavengerDropTable = Config.Bind("Mobs", "Scavenger", false, "Scavenger will drop scrap instead of items");
            ReplaceElderLemurianDropTable = Config.Bind("Mobs", "Elite Elder Lemurian", false, "The Elite Elder Lemurian in the hidden chamber of Abandoned Aqueduct will drop scrap instead of bands");

            ReplaceNewtAltarsCost = Config.Bind("Costs", "Newt Altars uses white items", true, "Newt Altar uses white items as the activation cost instead of lunar coins");
            
            ReplaceDoppelgangerDropTable = Config.Bind("Artifacts", "Relentless Doppelganger", false, "The Relentless Doppelganger from the Artifact of Vengeance will drop scrap instead of items");
            ReplaceSacrificeArtifactDropTable = Config.Bind("Artifacts", "Artifact of Sacrifice", true, "When using the Artifact of Sacrifice, mobs will drop scrap instead of items");

            ReplaceSimulacrumOrbDropTable = Config.Bind("Waves", "Simulacrum", true, "The orb reward after each wave of Simulacrum will drop scrap instead of items");
            ReplaceVoidFieldsOrbDropTable = Config.Bind("Waves", "Void Fields", true, "The orb reward after each wave of the Void Fields will drop scrap instead of items");

            ReplaceWhiteItems = Config.Bind("Tiers", "White item", true, "Replace white item drops with white scrap");
            ReplaceGreenItems = Config.Bind("Tiers", "Green item", true, "Replace green item drops with green scrap");
            ReplaceRedItems = Config.Bind("Tiers", "Red item", true, "Replace red item drops with red scrap");
            ReplaceYellowItems = Config.Bind("Tiers", "Yellow item", true, "Replace yellow item drops with yellow scrap");
            ReplaceBlueItems = Config.Bind("Tiers", "Blue item", true, "Replace blue item drops with Beads of Fealty");
            ReplaceVoidTier1Items = Config.Bind("Tiers", "Void tier 1 item", true, "Replace void tier 1 drops with white scrap");
            ReplaceVoidTier2Items = Config.Bind("Tiers", "Void tier 2 item", true, "Replace void tier 2 drops with green scrap");
            ReplaceVoidTier3Items = Config.Bind("Tiers", "Void tier 3 item", true, "Replace void tier 3 drops with red scrap");

            ModSettingsManager.AddOption(new CheckBoxOption(ModEnabled));

            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceChestDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceMultiShopDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceAdaptiveChestDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceChanceShrineDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceLegendaryChestDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceVoidPotentialDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceVoidCradleDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceLunarPodDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceLunarBudsDropTable));

            ModSettingsManager.AddOption(new StepSliderOption(WhitePrinterSpawnMultiplier, new StepSliderConfig() { min = 0, max = 5, increment = 0.05f, formatString = "{0:0.##}x" }));
            ModSettingsManager.AddOption(new StepSliderOption(GreenPrinterSpawnMultiplier, new StepSliderConfig() { min = 0, max = 5, increment = 0.05f, formatString = "{0:0.##}x" }));
            ModSettingsManager.AddOption(new StepSliderOption(RedPrinterSpawnMultiplier, new StepSliderConfig() { min = 0, max = 5, increment = 0.05f, formatString = "{0:0.##}x" }));
            ModSettingsManager.AddOption(new StepSliderOption(YellowPrinterSpawnMultiplier, new StepSliderConfig() { min = 0, max = 5, increment = 0.05f, formatString = "{0:0.##}x" }));
            ModSettingsManager.AddOption(new CheckBoxOption(AddVoidItemsToPrinters));
            ModSettingsManager.AddOption(new CheckBoxOption(AddVoidItemsToCauldrons));

            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceLockboxDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceEncrustedCacheDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceCrashedMultishopDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceBossHunterDropTable));
            ModSettingsManager.AddOption(new StepSliderOption(SpeedItemSpawnMultiplier, new StepSliderConfig() { min = 0, max = 5, increment = 0.05f, formatString = "{0:0.##}x" }));

            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceBossDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceAWUDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceScavengerDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceElderLemurianDropTable));

            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceNewtAltarsCost));

            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceDoppelgangerDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceSacrificeArtifactDropTable));

            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceSimulacrumOrbDropTable));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceVoidFieldsOrbDropTable));

            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceWhiteItems));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceGreenItems));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceRedItems));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceYellowItems));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceBlueItems));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceVoidTier1Items));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceVoidTier2Items));
            ModSettingsManager.AddOption(new CheckBoxOption(ReplaceVoidTier3Items));

            ModSettingsManager.SetModIcon(LoadIconSprite());
        }

        Sprite LoadIconSprite()
        {
            Texture2D texture = new Texture2D(2, 2); 
            texture.LoadImage(File.ReadAllBytes(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), "icon.png")));
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));

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
                else if (self.dropTable.name == "dtVoidChest")
                {
                    if (ReplaceVoidCradleDropTable.Value)
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
                if (self.serverMultiShopController == null)
                {
                    if (ReplaceLunarBudsDropTable.Value && self.dropTable.name == "dtLunarChest")
                    {
                        using var _ = ReplaceDropTable(self.dropTable, nameof(ShopTerminalBehavior_GenerateNewPickupServer_bool));
                        orig(self, newHidden);
                        return;
                    }
                    
                    if ((AddVoidItemsToPrinters.Value && self.dropTable.name.StartsWith("dtDuplicator"))
                        || (AddVoidItemsToCauldrons.Value && self.dropTable.name is "dtTier1Item" or "dtTier2Item" or "dtTier3Item"))
                    {
                        BasicPickupDropTable basicDropTable = (BasicPickupDropTable)self.dropTable;

                        using IDisposable disposable = CreateSelectorCopy(basicDropTable.selector, x => basicDropTable.selector = x);

                        basicDropTable.Add(Run.instance.availableVoidTier1DropList, basicDropTable.tier1Weight);
                        basicDropTable.Add(Run.instance.availableVoidTier2DropList, basicDropTable.tier2Weight);
                        basicDropTable.Add(Run.instance.availableVoidTier3DropList, basicDropTable.tier3Weight);
                        basicDropTable.Add(Run.instance.availableVoidBossDropList, basicDropTable.bossWeight);

                        UpdateSpeedItemsSpawnRate(basicDropTable.selector);

                        Log.Debug($"{nameof(ShopTerminalBehavior_GenerateNewPickupServer_bool)} {basicDropTable.GetType().Name} replaced");
                        orig(self, newHidden);
                        return;
                    }

                }
                else if (ReplaceMultiShopDropTable.Value)
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

        private string PurchaseInteraction_GetContextString(On.RoR2.PurchaseInteraction.orig_GetContextString orig, PurchaseInteraction self, Interactor activator)
        {
            return ReplacePurchaseInteraction(self, () => orig(self, activator));
        }
        private bool PurchaseInteraction_CanBeAffordedByInteractor(On.RoR2.PurchaseInteraction.orig_CanBeAffordedByInteractor orig, PurchaseInteraction self, Interactor activator)
        {
            return ReplacePurchaseInteraction(self, () => orig(self, activator));
        }

        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            ReplacePurchaseInteraction(self, () => { orig(self, activator); return true; });
        }

        private void PurchaseInteraction_UpdateHologramContent(On.RoR2.PurchaseInteraction.orig_UpdateHologramContent orig, PurchaseInteraction self, GameObject hologramContentObject)
        {
            ReplacePurchaseInteraction(self, () => { orig(self, hologramContentObject); return true; });
        }

        private void PingIndicator_RebuildPing(On.RoR2.UI.PingIndicator.orig_RebuildPing orig, RoR2.UI.PingIndicator self)
        {
            if (self.pingTarget)
            {
                PurchaseInteraction purchaseInteraction = self.pingTarget.GetComponent<PurchaseInteraction>();
                if (purchaseInteraction?.displayNameToken == "NEWT_STATUE_NAME")
                {
                    ReplacePurchaseInteraction(purchaseInteraction, () => { orig(self); return true; });
                    return;
                }
            }

            orig(self);
        }

        private T ReplacePurchaseInteraction<T>(PurchaseInteraction self, Func<T> orig)
        {
            if (ModEnabled.Value && ReplaceNewtAltarsCost.Value && self.displayNameToken == "NEWT_STATUE_NAME")
            {
                var oldCostType = self.costType;
                using var disposable = new Disposable(() => self.costType = oldCostType);
                self.costType = CostTypeIndex.WhiteItem;
                return orig();
            }

            return orig();
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
            if (ModEnabled.Value)
            {
                if (ReplaceVoidPotentialDropTable.Value && self.dropTable.name == "dtVoidTriple")
                {
                    using var _ = ReplaceDropTable(self.dropTable, nameof(OptionChestBehavior_Roll));
                    orig(self);
                    return;
                }
                
                if (ReplaceEncrustedCacheDropTable.Value && self.dropTable.name == "dtVoidLockbox")
                {
                    using var _ = ReplaceDropTable(self.dropTable, nameof(OptionChestBehavior_Roll));
                    orig(self);
                    return;
                }
            }

            orig(self);
        }

        private WeightedSelection<DirectorCard> SceneDirector_GenerateInteractableCardSelection(On.RoR2.SceneDirector.orig_GenerateInteractableCardSelection orig, SceneDirector self)
        {
            var weightedSelection = orig(self);

            if (ModEnabled.Value)
            {
                _printerConfigBySpawnCard ??= new Dictionary<string, (ConfigEntry<float> Config, GameObject SoupPrefab)>
                {
                    ["iscDuplicator"] = (WhitePrinterSpawnMultiplier, Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarCauldrons/LunarCauldron, WhiteToGreen.prefab").WaitForCompletion()),
                    ["iscDuplicatorLarge"] = (GreenPrinterSpawnMultiplier, Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarCauldrons/LunarCauldron, GreenToRed Variant.prefab").WaitForCompletion()),
                    ["iscDuplicatorMilitary"] = (RedPrinterSpawnMultiplier, Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarCauldrons/LunarCauldron, RedToWhite Variant.prefab").WaitForCompletion()),
                    ["iscDuplicatorWild"] = (YellowPrinterSpawnMultiplier, null)
                };

                for (int i = 0; i < weightedSelection.choices.Length; i++)
                {
                    ref WeightedSelection<DirectorCard>.ChoiceInfo choice = ref weightedSelection.choices[i];

                    if (choice.value != null && _printerConfigBySpawnCard.TryGetValue(choice.value.spawnCard.name, out var printerMultiplierConfig))
                    {
                        if (printerMultiplierConfig.Config.Value != 1)
                        {
                            var oldWeigth = choice.weight;
                            weightedSelection.ModifyChoiceWeight(i, choice.weight * printerMultiplierConfig.Config.Value);
                            Log.Debug($"iscDuplicator weight changed from {oldWeigth} to {choice.weight}");
                        }

                        if (printerMultiplierConfig.SoupPrefab != null)
                        {
                            var newCard = new DirectorCard
                            {
                                forbiddenUnlockable = choice.value.forbiddenUnlockable,
                                forbiddenUnlockableDef = choice.value.forbiddenUnlockableDef,
                                minimumStageCompletions = choice.value.minimumStageCompletions,
                                preventOverhead = choice.value.preventOverhead,
                                requiredUnlockable = choice.value.requiredUnlockable,
                                requiredUnlockableDef = choice.value.requiredUnlockableDef,
                                selectionWeight = choice.value.selectionWeight,
                                spawnCard = Instantiate(choice.value.spawnCard),
                                spawnDistance = choice.value.spawnDistance,
                            };

                            newCard.spawnCard.prefab = printerMultiplierConfig.SoupPrefab;
                            choice.value = newCard;
                            Log.Debug($"iscDuplicator replaced with soup");
                        }
                    }
                }
            }

            return weightedSelection;
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

        private void ArenaMissionController_EndRound(On.RoR2.ArenaMissionController.orig_EndRound orig, ArenaMissionController self)
        {
            if (ModEnabled.Value && ReplaceVoidFieldsOrbDropTable.Value)
            {
                using var disposables = new CompositeDisposable();

                foreach (var rewardOrder in self.playerRewardOrder)
                {
                    disposables.Add(ReplaceDropTable(rewardOrder, nameof(ArenaMissionController_EndRound)));
                }

                orig(self);
                return;
            }

            orig(self);
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
                        ItemTier.VoidTier1 => ReplaceVoidTier1Items.Value,
                        ItemTier.VoidTier2 => ReplaceVoidTier2Items.Value,
                        ItemTier.VoidTier3 => ReplaceVoidTier3Items.Value,
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
                        ItemTier.VoidTier1 => "ItemIndex.ScrapWhite",
                        ItemTier.VoidTier2 => "ItemIndex.ScrapGreen",
                        ItemTier.VoidTier3 => "ItemIndex.ScrapRed",
                        _ => throw new Exception("Unreachable")
                    };

                    if (scrapPickupName != null)
                    {
                        choice.value = PickupCatalog.FindPickupIndex(scrapPickupName);
                    }
                }

                UpdateSpeedItemsSpawnRate(selector);

                Log.Debug($"{caller} {dropTable.GetType().Name} ({dropTable.name}) replaced");
            }
        }

        private void UpdateSpeedItemsSpawnRate(WeightedSelection<PickupIndex> selector)
        {
            if (SpeedItemSpawnMultiplier.Value == 1)
            {
                return;
            }

            var speedItems = new (ItemIndex Item, float Weigth)[]
            {
                (RoR2Content.Items.Hoof.itemIndex, 1),//Paul's Goat Hoof
                (RoR2Content.Items.SprintBonus.itemIndex, 1),//Energy Drink
                (DLC1Content.Items.AttackSpeedAndMoveSpeed.itemIndex, 1),//Mocha
                (RoR2Content.Items.SprintOutOfCombat.itemIndex, 1),//Red Whip
                (RoR2Content.Items.JumpBoost.itemIndex, 1),//Wax Quail
                (DLC1Content.Items.MoveSpeedOnKill.itemIndex, 1),//Hunter's Harpoon
                (RoR2Content.Items.WardOnLevel.itemIndex, 0.33f),//Warbanner
                (RoR2Content.Items.WarCryOnMultiKill.itemIndex, 0.33f),//Berzerker's Pauldron
                (RoR2Content.Items.LunarBadLuck.itemIndex, 0.33f),//Purity
                (RoR2Content.Items.AlienHead.itemIndex, 0.33f),//Alien Head
                (RoR2Content.Items.UtilitySkillMagazine.itemIndex, 0.33f),//Hardlight Afterburner
                (DLC1Content.Items.HalfAttackSpeedHalfCooldowns.itemIndex, 0.33f),//Light Flux Pauldron
                (RoR2Content.Items.Phasing.itemIndex, 0.25f),//Old War Stealthkit
                (RoR2Content.Items.Bandolier.itemIndex, 0.25f),//Bandolier
            };

            var speedEquipements = new (EquipmentIndex Equipement, float Weigth)[]
            {
                (RoR2Content.Equipment.TeamWarCry.equipmentIndex, 0.5f),//Gorag's Opus
                (RoR2Content.Equipment.Gateway.equipmentIndex, 0.5f),//Eccentric Vase
                (RoR2Content.Equipment.Jetpack.equipmentIndex, 0.5f),//Milky Chrysalis
                (RoR2Content.Equipment.FireBallDash.equipmentIndex, 0.5f),//Volcanic Egg
                (RoR2Content.Equipment.Tonic.equipmentIndex, 0.5f)//Spinel Tonic
            };

            for (int i = 0; i < selector.choices.Length; i++)
            {
                ref var choice = ref selector.choices[i];
                var pickupDef = choice.value.pickupDef;

                var itemWeigth = speedItems
                    .Where(x => x.Item == pickupDef.itemIndex)
                    .Select(x => x.Weigth)
                    .Concat(speedEquipements
                        .Where(x => x.Equipement == pickupDef.equipmentIndex)
                        .Select(x => x.Weigth))
                    .FirstOrDefault();

                if (itemWeigth > 0)
                {
                    var oldWeight = choice.weight;
                    selector.ModifyChoiceWeight(i, choice.weight + choice.weight * (SpeedItemSpawnMultiplier.Value - 1) * itemWeigth);
                    Log.Debug($"{choice.value.pickupDef.nameToken} weight changed from {oldWeight} to {choice.weight}");
                }
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
