using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;
using BepInEx.Configuration;
using HarmonyLib;

namespace ScrappyChests;

public static class PrinterHooks
{
    public static void Init()
    {
        On.RoR2.SceneDirector.GenerateInteractableCardSelection += SceneDirector_GenerateInteractableCardSelection;
        On.RoR2.CampDirector.GenerateInteractableCardSelection += CampDirector_GenerateInteractableCardSelection;
        On.RoR2.DirectorCore.TrySpawnObject += DirectorCore_TrySpawnObject;
        On.RoR2.DirectorCard.IsAvailable += DirectorCard_IsAvailable;
    }

    private static WeightedSelection<DirectorCard> CampDirector_GenerateInteractableCardSelection(On.RoR2.CampDirector.orig_GenerateInteractableCardSelection orig, CampDirector self)
    {
        if (Configuration.Instance.ModEnabled.Value && Configuration.Instance.AddVoidPrintersToVoidSeeds.Value && self.interactableDirectorCards.name == "dccsVoidCampInteractables")
        {
            DirectorCardCategorySelection.Category printerCategory = new DirectorCardCategorySelection.Category
            {
                name = "Printers",
                selectionWeight = Configuration.Instance.VoidSeedsPrinterWeight.Value,
                cards = [
                    CreateDirectorCard("RoR2/Base/Duplicator/iscDuplicator.asset", Configuration.Instance.VoidSeedsPrinterWhiteWeight.Value, Configuration.Instance.VoidSeedsPrinterWhiteCreditCost.Value),
                    CreateDirectorCard("RoR2/Base/DuplicatorLarge/iscDuplicatorLarge.asset", Configuration.Instance.VoidSeedsPrinterGreenWeight.Value, Configuration.Instance.VoidSeedsPrinterGreenCreditCost.Value),
                    CreateDirectorCard("RoR2/Base/DuplicatorMilitary/iscDuplicatorMilitary.asset", Configuration.Instance.VoidSeedsPrinterRedWeight.Value, Configuration.Instance.VoidSeedsPrinterRedCreditCost.Value)
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

    private static GameObject DirectorCore_TrySpawnObject(On.RoR2.DirectorCore.orig_TrySpawnObject orig, DirectorCore self, DirectorSpawnRequest directorSpawnRequest)
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

    private static bool DirectorCard_IsAvailable(On.RoR2.DirectorCard.orig_IsAvailable orig, DirectorCard self)
    {
        if (Configuration.Instance.ModEnabled.Value && self.spawnCard.name == "iscDuplicatorMilitary")
        {
            int defaultValue = self.minimumStageCompletions;

            using var disposable = new Disposable(() => self.minimumStageCompletions = defaultValue);
            self.minimumStageCompletions = Configuration.Instance.MinimumStageForRedPrinters.Value - 1;

            return orig(self);
        }

        return orig(self);
    }

    private static WeightedSelection<DirectorCard> SceneDirector_GenerateInteractableCardSelection(On.RoR2.SceneDirector.orig_GenerateInteractableCardSelection orig, SceneDirector self)
    {
        var weightedSelection = orig(self);

        if (Configuration.Instance.ModEnabled.Value)
        {
            Dictionary<string, ConfigEntry<float>> configBySpawnCard = new Dictionary<string, ConfigEntry<float>>
            {
                ["iscDuplicator"] = Configuration.Instance.WhitePrinterSpawnMultiplier,
                ["iscDuplicatorLarge"] = Configuration.Instance.GreenPrinterSpawnMultiplier,
                ["iscDuplicatorMilitary"] = Configuration.Instance.RedPrinterSpawnMultiplier,
                ["iscDuplicatorWild"] = Configuration.Instance.YellowPrinterSpawnMultiplier
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
}
