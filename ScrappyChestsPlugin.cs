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
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using HarmonyLib;

namespace ScrappyChests
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ScrappyChestsPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "Lawlzee.ScrappyChests";
        public const string PluginAuthor = "Lawlzee";
        public const string PluginName = "Scrappy Chests";
        public const string PluginVersion = "1.2.2";

        private Configuration _config;

        private static readonly CostTypeIndex _yellowSoupCostIndex = (CostTypeIndex)81273;
        private CostTypeDef _yellowSoupCostTypeDef;

        public void Awake()
        {
            Log.Init(Logger);

            _config = new Configuration(Config);
            _config.InitUI(Info);

            On.RoR2.ChestBehavior.Roll += ChestBehavior_Roll;
            On.RoR2.ShopTerminalBehavior.GenerateNewPickupServer_bool += ShopTerminalBehavior_GenerateNewPickupServer_bool;
            On.RoR2.RouletteChestController.GenerateEntriesServer += RouletteChestController_GenerateEntriesServer;
            On.RoR2.ShrineChanceBehavior.AddShrineStack += ShrineChanceBehavior_AddShrineStack;
            On.RoR2.FreeChestDropTable.GenerateDropPreReplacement += FreeChestDropTable_GenerateDropPreReplacement;
            On.RoR2.OptionChestBehavior.Roll += OptionChestBehavior_Roll;

            On.RoR2.SceneDirector.GenerateInteractableCardSelection += SceneDirector_GenerateInteractableCardSelection;

            On.RoR2.CampDirector.GenerateInteractableCardSelection += CampDirector_GenerateInteractableCardSelection;
            On.RoR2.DirectorCore.TrySpawnObject += DirectorCore_TrySpawnObject;

            On.RoR2.EquipmentSlot.FireBossHunter += EquipmentSlot_FireBossHunter;

            On.RoR2.BossGroup.DropRewards += BossGroup_DropRewards;
            On.EntityStates.ScavBackpack.Opening.FixedUpdate += Opening_FixedUpdate;
            On.RoR2.MasterDropDroplet.DropItems += MasterDropDroplet_DropItems;
            On.RoR2.ChestBehavior.RollItem += ChestBehavior_RollItem;

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            On.RoR2.CostTypeCatalog.GetCostTypeDef += CostTypeCatalog_GetCostTypeDef;

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

            On.RoR2.SceneObjectToggleGroup.Awake += SceneObjectToggleGroup_Awake;

            _yellowSoupCostTypeDef = new CostTypeDef()
            {
                costStringFormatToken = "COST_ITEM_FORMAT",
                buildCostString = (def, context) =>
                {
                    var cost = GetYellowCauldronCost();
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
                            var cost = GetYellowCauldronCost();
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
                    var cost = GetYellowCauldronCost();

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

        private void ChestBehavior_Roll(On.RoR2.ChestBehavior.orig_Roll orig, ChestBehavior self)
        {
            if (_config.ModEnabled.Value && self.dropTable)
            {
                if (self.dropTable.name == "dtGoldChest")
                {
                    if (_config.ReplaceLegendaryChestDropTable.Value)
                    {
                        using var _ = ReplaceDropTable(self.dropTable, nameof(ChestBehavior_Roll));
                        orig(self);
                        return;
                    }
                }
                else if (self.dropTable.name == "dtLockbox")
                {
                    if (_config.ReplaceLockboxDropTable.Value)
                    {
                        using var _ = ReplaceDropTable(self.dropTable, nameof(ChestBehavior_Roll));
                        orig(self);
                        return;
                    }
                }
                else if (self.dropTable.name == "dtLunarChest")
                {
                    if (_config.ReplaceLunarPodDropTable.Value)
                    {
                        using var _ = ReplaceDropTable(self.dropTable, nameof(ChestBehavior_Roll));
                        orig(self);
                        return;
                    }
                }
                else if (self.dropTable.name == "dtVoidChest")
                {
                    if (_config.ReplaceVoidCradleDropTable.Value)
                    {
                        using var _ = ReplaceDropTable(self.dropTable, nameof(ChestBehavior_Roll));
                        orig(self);
                        return;
                    }
                }
                else if (_config.ReplaceChestDropTable.Value)
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
            if (_config.ModEnabled.Value)
            {
                if (self.serverMultiShopController == null)
                {
                    if (_config.ReplaceLunarBudsDropTable.Value && self.dropTable.name == "dtLunarChest")
                    {
                        using var _ = ReplaceDropTable(self.dropTable, nameof(ShopTerminalBehavior_GenerateNewPickupServer_bool));
                        orig(self, newHidden);
                        return;
                    }

                    if ((_config.AddVoidItemsToPrinters.Value && self.dropTable.name.StartsWith("dtDuplicator"))
                        || (_config.AddVoidItemsToCauldrons.Value && self.dropTable.name is "dtTier1Item" or "dtTier2Item" or "dtTier3Item")
                        || self.name == "VoidShopTerminal")
                    {
                        BasicPickupDropTable basicDropTable = (BasicPickupDropTable)self.dropTable;

                        using IDisposable disposable = CreateSelectorCopy(basicDropTable.selector, x => basicDropTable.selector = x);

                        if (self.name == "VoidShopTerminal")
                        {
                            basicDropTable.selector.Clear();
                        }

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
                else if (_config.ReplaceMultiShopDropTable.Value)
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
            if (_config.ModEnabled.Value && _config.ReplaceAdaptiveChestDropTable.Value)
            {
                using var _ = ReplaceDropTable(self.dropTable, nameof(RouletteChestController_GenerateEntriesServer));
                orig(self, startTime);
                return;
            }

            orig(self, startTime);
        }

        private void ShrineChanceBehavior_AddShrineStack(On.RoR2.ShrineChanceBehavior.orig_AddShrineStack orig, ShrineChanceBehavior self, Interactor activator)
        {
            if (_config.ModEnabled.Value && _config.ReplaceChanceShrineDropTable.Value)
            {
                using var _ = ReplaceDropTable(self.dropTable, nameof(ShrineChanceBehavior_AddShrineStack));
                orig(self, activator);
                return;
            }
            orig(self, activator);
        }

        private void BossGroup_DropRewards(On.RoR2.BossGroup.orig_DropRewards orig, BossGroup self)
        {
            if (_config.ModEnabled.Value)
            {
                if (self.bossDropChance == 0 && self.dropTable.name == "dtTier3Item" && self.bossDropTables.Count == 1 && self.bossDropTables[0].name == "dtBossRoboBallBoss")
                {
                    if (_config.ReplaceAWUDropTable.Value)
                    {
                        Replace();
                        return;
                    }
                }
                else if (_config.ReplaceBossDropTable.Value)
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
            if (_config.ModEnabled.Value && _config.ReplaceBossHunterDropTable.Value)
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
            if (_config.ModEnabled.Value && _config.ReplaceScavengerDropTable.Value)
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
            if (_config.ModEnabled.Value && _config.ReplaceScavengerDropTable.Value)
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
                if (purchaseInteraction?.displayNameToken == "NEWT_STATUE_NAME" || purchaseInteraction?.displayNameToken == "BAZAAR_SEER_NAME")
                {
                    ReplacePurchaseInteraction(purchaseInteraction, () => { orig(self); return true; });
                    return;
                }
            }

            orig(self);
        }

        private T ReplacePurchaseInteraction<T>(PurchaseInteraction self, Func<T> orig)
        {
            if (_config.ModEnabled.Value
                && ((_config.ReplaceNewtAltarsCost.Value && self.displayNameToken == "NEWT_STATUE_NAME")
                    || (_config.ReplaceLunarSeerCost.Value && self.displayNameToken == "BAZAAR_SEER_NAME")))
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
            if (_config.ModEnabled.Value && _config.ReplaceCrashedMultishopDropTable.Value)
            {
                orig(self, rng);
                using var _ = ReplaceDropTable(self, nameof(FreeChestDropTable_GenerateDropPreReplacement));
                return PickupDropTable.GenerateDropFromWeightedSelection(rng, self.selector);
            }

            return orig(self, rng);
        }

        private PickupIndex DoppelgangerDropTable_GenerateDropPreReplacement(On.RoR2.DoppelgangerDropTable.orig_GenerateDropPreReplacement orig, DoppelgangerDropTable self, Xoroshiro128Plus rng)
        {
            if (_config.ModEnabled.Value && _config.ReplaceDoppelgangerDropTable.Value)
            {
                using var _ = ReplaceDropTable(self, nameof(DoppelgangerDropTable_GenerateDropPreReplacement));
                return orig(self, rng);
            }

            return orig(self, rng);
        }

        private void SacrificeArtifactManager_OnServerCharacterDeath(On.RoR2.Artifacts.SacrificeArtifactManager.orig_OnServerCharacterDeath orig, DamageReport damageReport)
        {
            if (_config.ModEnabled.Value && _config.ReplaceSacrificeArtifactDropTable.Value)
            {
                using var _ = ReplaceDropTable(SacrificeArtifactManager.dropTable, nameof(SacrificeArtifactManager_OnServerCharacterDeath));
                orig(damageReport);
                return;
            }

            orig(damageReport);
        }

        private void MasterDropDroplet_DropItems(On.RoR2.MasterDropDroplet.orig_DropItems orig, MasterDropDroplet self)
        {
            if (_config.ModEnabled.Value && _config.ReplaceElderLemurianDropTable.Value)
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
            if (_config.ModEnabled.Value)
            {
                if (_config.ReplaceVoidPotentialDropTable.Value && self.dropTable.name == "dtVoidTriple")
                {
                    using var _ = ReplaceDropTable(self.dropTable, nameof(OptionChestBehavior_Roll));
                    orig(self);
                    return;
                }

                if (_config.ReplaceEncrustedCacheDropTable.Value && self.dropTable.name == "dtVoidLockbox")
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

            if (_config.ModEnabled.Value)
            {
                Dictionary<string, ConfigEntry<float>> configBySpawnCard = new Dictionary<string, ConfigEntry<float>>
                {
                    ["iscDuplicator"] = _config.WhitePrinterSpawnMultiplier,
                    ["iscDuplicatorLarge"] = _config.GreenPrinterSpawnMultiplier,
                    ["iscDuplicatorMilitary"] = _config.RedPrinterSpawnMultiplier,
                    ["iscDuplicatorWild"] = _config.YellowPrinterSpawnMultiplier
                };

                for (int i = 0; i < weightedSelection.choices.Length; i++)
                {
                    ref WeightedSelection<DirectorCard>.ChoiceInfo choice = ref weightedSelection.choices[i];

                    if (choice.value != null && configBySpawnCard.TryGetValue(choice.value.spawnCard.name, out var printerMultiplierConfig))
                    {
                        if (printerMultiplierConfig.Value == 1)
                        {
                            continue;
                        }

                        var oldWeigth = choice.weight;
                        weightedSelection.ModifyChoiceWeight(i, choice.weight * printerMultiplierConfig.Value);
                        Log.Debug($"iscDuplicator weight changed from {oldWeigth} to {choice.weight}");
                    }
                }
            }

            return weightedSelection;
        }

        private WeightedSelection<DirectorCard> CampDirector_GenerateInteractableCardSelection(On.RoR2.CampDirector.orig_GenerateInteractableCardSelection orig, CampDirector self)
        {
            if (_config.ModEnabled.Value && _config.AddVoidPrintersToVoidSeeds.Value && self.interactableDirectorCards.name == "dccsVoidCampInteractables")
            {
                DirectorCardCategorySelection.Category printerCategory = new DirectorCardCategorySelection.Category
                {
                    name = "Printers",
                    selectionWeight = _config.VoidSeedsPrinterWeight.Value,
                    cards = [
                        CreateDirectorCard("RoR2/Base/Duplicator/iscDuplicator.asset", _config.VoidSeedsPrinterWhiteWeight.Value, _config.VoidSeedsPrinterWhiteCreditCost.Value),
                        CreateDirectorCard("RoR2/Base/DuplicatorLarge/iscDuplicatorLarge.asset", _config.VoidSeedsPrinterGreenWeight.Value, _config.VoidSeedsPrinterGreenCreditCost.Value),
                        CreateDirectorCard("RoR2/Base/DuplicatorMilitary/iscDuplicatorMilitary.asset", _config.VoidSeedsPrinterRedWeight.Value, _config.VoidSeedsPrinterRedCreditCost.Value)
                    ]
                };

                DirectorCard CreateDirectorCard(string assetKey, int selectionWeight, int directorCreditCost)
                {
                    var spawnCard = Addressables.LoadAssetAsync<InteractableSpawnCard>(assetKey).WaitForCompletion();
                    InteractableSpawnCard newSpawnCard = ScriptableObject.CreateInstance<InteractableSpawnCard>();

                    newSpawnCard.name = "iscVoidPrinter";
                    newSpawnCard.sendOverNetwork = spawnCard.sendOverNetwork;
                    newSpawnCard.hullSize = spawnCard.hullSize;
                    newSpawnCard.nodeGraphType = spawnCard.nodeGraphType;
                    newSpawnCard.requiredFlags = spawnCard.requiredFlags;
                    newSpawnCard.forbiddenFlags = spawnCard.forbiddenFlags;
                    newSpawnCard.directorCreditCost = directorCreditCost;
                    newSpawnCard.occupyPosition = spawnCard.occupyPosition;
                    newSpawnCard.eliteRules = spawnCard.eliteRules;
                    newSpawnCard.orientToFloor = spawnCard.orientToFloor;
                    newSpawnCard.slightlyRandomizeOrientation = spawnCard.slightlyRandomizeOrientation;
                    newSpawnCard.skipSpawnWhenSacrificeArtifactEnabled = spawnCard.skipSpawnWhenSacrificeArtifactEnabled;
                    newSpawnCard.weightScalarWhenSacrificeArtifactEnabled = spawnCard.weightScalarWhenSacrificeArtifactEnabled;
                    newSpawnCard.maxSpawnsPerStage = spawnCard.maxSpawnsPerStage;

                    newSpawnCard.prefab = spawnCard.prefab;

                    return new DirectorCard
                    {
                        spawnCard = newSpawnCard,
                        selectionWeight = selectionWeight
                    };
                }

                var oldCategories = self.interactableDirectorCards.categories;
                using var disposable = new Disposable(() => self.interactableDirectorCards.categories = oldCategories);

                self.interactableDirectorCards.categories = self.interactableDirectorCards.categories.AddToArray(printerCategory);

                return orig(self);
            }

            return orig(self);
        }

        private GameObject DirectorCore_TrySpawnObject(On.RoR2.DirectorCore.orig_TrySpawnObject orig, DirectorCore self, DirectorSpawnRequest directorSpawnRequest)
        {
            GameObject gameObject = orig(self, directorSpawnRequest);
            if (gameObject)
            {
                if (directorSpawnRequest.spawnCard.name == "iscVoidPrinter")
                {
                    ShopTerminalBehavior shopTerminal = gameObject.GetComponent<ShopTerminalBehavior>();
                    shopTerminal.name = "VoidShopTerminal";
                }
            }

            return gameObject;
        }

        private CostTypeDef CostTypeCatalog_GetCostTypeDef(On.RoR2.CostTypeCatalog.orig_GetCostTypeDef orig, CostTypeIndex costTypeIndex)
        {
            if (costTypeIndex == _yellowSoupCostIndex)
            {
                return _yellowSoupCostTypeDef;
            }

            return orig(costTypeIndex);
        }

        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (!_config.ModEnabled.Value)
            {
                return;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name != "bazaar")
            {
                return;
            }

            if (_config.AddWhiteCauldronToBazaar.Value)
            {
                GameObject whiteSoup = Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarCauldrons/LunarCauldron, RedToWhite Variant.prefab").WaitForCompletion());
                whiteSoup.transform.position = new Vector3(-95.98551f, -24.7408f, -43.01519f);
                whiteSoup.transform.rotation = Quaternion.Euler(0, -45f, 0);

                NetworkServer.Spawn(whiteSoup);
            }

            if (_config.AddYellowCauldronToBazaar.Value)
            {
                GameObject yellowSoup = CreateYellowSoup(new Vector3(-103.5473f, -25.05961f, -40.06012f), Quaternion.Euler(0, -10f, 0));
                NetworkServer.Spawn(yellowSoup);
            }
        }

        private void SceneObjectToggleGroup_Awake(On.RoR2.SceneObjectToggleGroup.orig_Awake orig, SceneObjectToggleGroup self)
        {
            if (_config.ModEnabled.Value && _config.AddYellowCauldronToMoon.Value)
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
                        minEnabled = 3,
                        maxEnabled = 3
                    });
                }
            }

            orig(self);
        }

        private GameObject CreateYellowSoup(Vector3 position, Quaternion rotation)
        {
            GameObject yellowSoup = Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarCauldrons/LunarCauldron, WhiteToGreen.prefab").WaitForCompletion());
            yellowSoup.transform.position = position;
            yellowSoup.transform.rotation = rotation;

            PurchaseInteraction purchaseInteraction = yellowSoup.GetComponent<PurchaseInteraction>();
            purchaseInteraction.costType = _yellowSoupCostIndex;
            (int White, int Green, int Red, int Yellow) cost = GetYellowCauldronCost();
            purchaseInteraction.cost = cost.White + cost.Green + cost.Red + cost.Yellow;

            ShopTerminalBehavior terminalBehavior = yellowSoup.GetComponent<ShopTerminalBehavior>();
            terminalBehavior.dropTable = Addressables.LoadAssetAsync<BasicPickupDropTable>("RoR2/Base/DuplicatorWild/dtDuplicatorWild.asset").WaitForCompletion();
            terminalBehavior.itemTier = ItemTier.Boss;

            return yellowSoup;
        }

        private (int White, int Green, int Red, int Yellow) GetYellowCauldronCost()
        {
            int white = 0;
            int green = 0;
            int red = 0;
            int yellow = 0;

            foreach (char c in _config.YellowCauldronCost.Value)
            {
                switch (c)
                {
                    case 'w':
                    case 'W':
                        white++;
                        break;
                    case 'g':
                    case 'G':
                        green++;
                        break;
                    case 'r':
                    case 'R':
                        red++;
                        break;
                    case 'y':
                    case 'Y':
                        yellow++;
                        break;
                }
            }

            return (white, green, red, yellow);
        }

        private void InfiniteTowerWaveController_DropRewards(On.RoR2.InfiniteTowerWaveController.orig_DropRewards orig, InfiniteTowerWaveController self)
        {
            if (_config.ModEnabled.Value && _config.ReplaceSimulacrumOrbDropTable.Value)
            {
                using var _ = ReplaceDropTable(self.rewardDropTable, nameof(InfiniteTowerWaveController_DropRewards));
                orig(self);
                return;
            }

            orig(self);
        }

        private PickupIndex[] ArenaMonsterItemDropTable_GenerateUniqueDropsPreReplacement(On.RoR2.ArenaMonsterItemDropTable.orig_GenerateUniqueDropsPreReplacement orig, ArenaMonsterItemDropTable self, int maxDrops, Xoroshiro128Plus rng)
        {
            if (_config.ModEnabled.Value && _config.ReplaceSimulacrumOrbDropTable.Value)
            {
                using var _ = ReplaceDropTable(self, nameof(ArenaMonsterItemDropTable_GenerateUniqueDropsPreReplacement));
                return orig(self, maxDrops, rng);
            }

            return orig(self, maxDrops, rng);
        }

        private void ArenaMissionController_EndRound(On.RoR2.ArenaMissionController.orig_EndRound orig, ArenaMissionController self)
        {
            if (_config.ModEnabled.Value && _config.ReplaceVoidFieldsOrbDropTable.Value)
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
                        ItemTier.Tier1 => _config.ReplaceWhiteItems.Value,
                        ItemTier.Tier2 => _config.ReplaceGreenItems.Value,
                        ItemTier.Tier3 => _config.ReplaceRedItems.Value,
                        ItemTier.Boss => _config.ReplaceYellowItems.Value,
                        ItemTier.Lunar => _config.ReplaceBlueItems.Value,
                        ItemTier.VoidTier1 => _config.ReplaceVoidTier1Items.Value,
                        ItemTier.VoidTier2 => _config.ReplaceVoidTier2Items.Value,
                        ItemTier.VoidTier3 => _config.ReplaceVoidTier3Items.Value,
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
            if (_config.SpeedItemSpawnMultiplier.Value == 1)
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
                    selector.ModifyChoiceWeight(i, choice.weight + choice.weight * (_config.SpeedItemSpawnMultiplier.Value - 1) * itemWeigth);
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
