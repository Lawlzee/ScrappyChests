namespace ScrappyChests;

public static class ConfigPresets
{
    public static readonly ConfigPreset Default = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.Default,
        Description = ConfigPresetDescriptions.Default
    };

    public static readonly ConfigPreset Easy = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.Easy,
        Description = ConfigPresetDescriptions.Easy,
        ReplaceAdaptiveChestDropTable = false,
        ReplaceRedItems = false,
        ReplaceLegendaryChestDropTable = false,
        ReplaceVoidPotentialDropTable = false,
        ReplaceVoidCradleDropTable = false,
        ReplaceBossDropTable = false,
        ReplaceAWUDropTable = false,
        ReplaceNewtAltarsCost = false,
        ReplaceLunarSeerCost = false,
        ReplaceLunarPodCost = false,
        ReplaceLunarBudCost = false,
        ReplaceSlabCost = false,
        ReplaceMageCost = false,
        ReplaceFrogCost = false,
        ReplaceSimulacrumOrbDropTable = false,
        ReplaceVoidFieldsOrbDropTable = false
    };

    public static readonly ConfigPreset AllTheChoices = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.AllTheChoices,
        Description = ConfigPresetDescriptions.AllTheChoices,
        ReplaceMultiShopDropTable = false,
        ReplaceAdaptiveChestDropTable = false,
        ReplaceVoidPotentialDropTable = false,
        ReplaceLunarPodDropTable = true,
        ReplaceSimulacrumOrbDropTable = false,
        ReplaceVoidFieldsOrbDropTable = false
    };

    public static readonly ConfigPreset DefaultPrinterSpawnRate = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.DefaultPrinterSpawnRate,
        Description = ConfigPresetDescriptions.DefaultPrinterSpawnRate,
        WhitePrinterSpawnMultiplier = 1,
        GreenPrinterSpawnMultiplier = 1,
        RedPrinterSpawnMultiplier = 1,
        YellowPrinterSpawnMultiplier = 1
    };

    public static readonly ConfigPreset NoPrinters = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.NoPrinters,
        Description = ConfigPresetDescriptions.NoPrinters,
        WhitePrinterSpawnMultiplier = 0,
        GreenPrinterSpawnMultiplier = 0,
        RedPrinterSpawnMultiplier = 0,
        YellowPrinterSpawnMultiplier = 0,
        AddVoidPrintersToVoidSeeds = false,
    };

    public static readonly ConfigPreset Hardcore = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.Hardcore,
        Description = ConfigPresetDescriptions.Hardcore,
        ReplaceLunarPodDropTable = true,
        ReplaceLunarBudsDropTable = true,
        WhitePrinterSpawnMultiplier = 1,
        GreenPrinterSpawnMultiplier = 1,
        RedPrinterSpawnMultiplier = 1,
        YellowPrinterSpawnMultiplier = 1,
        AddVoidPrintersToVoidSeeds = false,
        MinimumStageForRedPrinters = 5,
        AddWhiteCauldronToBazaar = false,
        AddYellowCauldronToBazaar = false,
        AddYellowCauldronToMoon = false,
        SpeedItemSpawnMultiplier = 1,
        ReplaceLockboxDropTable = true,
        ReplaceEncrustedCacheDropTable = true,
        ReplaceCrashedMultishopDropTable = true,
        ReplaceBossHunterDropTable = true,
        ReplaceScavengerDropTable = true,
        ReplaceElderLemurianDropTable = true,
        ReplaceShrineOfOrderCost = true,
        ReplaceLunarCoinDrops = false
    };

    public static readonly ConfigPreset V1_2 = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.V1_2,
        Description = ConfigPresetDescriptions.V1_2,
        MinimumStageForRedPrinters = 5,
        ReplaceLunarPodCost = false,
        ReplaceLunarBudCost = false,
        ReplaceSlabCost = false,
        ReplaceMageCost = false,
        ReplaceFrogCost = false,
        ReplaceLunarCoinDrops = false
    };

    public static readonly ConfigPreset V1_1 = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.V1_1,
        Description = ConfigPresetDescriptions.V1_1,
        AddYellowCauldronToBazaar = false,
        AddYellowCauldronToMoon = false,
        AddVoidPrintersToVoidSeeds = false,
        MinimumStageForRedPrinters = 5,
        ReplaceNewtAltarsCost = false,
        ReplaceLunarSeerCost = false,
        AddWhiteCauldronToBazaar = false,
        ReplaceLunarPodCost = false,
        ReplaceLunarBudCost = false,
        ReplaceSlabCost = false,
        ReplaceMageCost = false,
        ReplaceFrogCost = false,
        ReplaceLunarCoinDrops = false
    };

    public static readonly ConfigPreset V1 = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.V1,
        Description = ConfigPresetDescriptions.V1,
        WhitePrinterSpawnMultiplier = 1,
        GreenPrinterSpawnMultiplier = 1,
        RedPrinterSpawnMultiplier = 1,
        YellowPrinterSpawnMultiplier = 1,
        AddVoidItemsToPrinters = false,
        AddVoidItemsToCauldrons = false,
        AddYellowCauldronToBazaar = false,
        AddYellowCauldronToMoon = false,
        AddVoidPrintersToVoidSeeds = false,
        MinimumStageForRedPrinters = 5,
        ReplaceNewtAltarsCost = false,
        ReplaceLunarSeerCost = false,
        SpeedItemSpawnMultiplier = 1.0f,
        ReplaceVoidCradleDropTable = false,
        AddWhiteCauldronToBazaar = false,
        ReplaceLunarPodCost = false,
        ReplaceLunarBudCost = false,
        ReplaceSlabCost = false,
        ReplaceMageCost = false,
        ReplaceFrogCost = false,
        ReplaceLunarCoinDrops = false
    };

    public static readonly ConfigPreset Vanilla = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.Vanilla,
        Description = ConfigPresetDescriptions.Vanilla,
        ReplaceChestDropTable = false,
        ReplaceMultiShopDropTable = false,
        ReplaceAdaptiveChestDropTable = false,
        ReplaceChanceShrineDropTable = false,
        ReplaceLegendaryChestDropTable = false,
        ReplaceVoidPotentialDropTable = false,
        ReplaceVoidCradleDropTable = false,
        ReplaceLunarPodDropTable = false,
        ReplaceLunarBudsDropTable = false,
        WhitePrinterSpawnMultiplier = 1.0f,
        GreenPrinterSpawnMultiplier = 1.0f,
        RedPrinterSpawnMultiplier = 1.0f,
        YellowPrinterSpawnMultiplier = 1.0f,
        AddVoidItemsToPrinters = false,
        AddVoidPrintersToVoidSeeds = false,
        MinimumStageForRedPrinters = 5,
        AddVoidItemsToCauldrons = false,
        AddWhiteCauldronToBazaar = false,
        AddYellowCauldronToBazaar = false,
        AddYellowCauldronToMoon = false,
        ReplaceLockboxDropTable = false,
        ReplaceEncrustedCacheDropTable = false,
        ReplaceCrashedMultishopDropTable = false,
        ReplaceBossHunterDropTable = false,
        SpeedItemSpawnMultiplier = 1.0f,
        ReplaceBossDropTable = false,
        ReplaceAWUDropTable = false,
        ReplaceScavengerDropTable = false,
        ReplaceElderLemurianDropTable = false,
        ReplaceLunarCoinDrops = false,
        ReplaceNewtAltarsCost = false,
        ReplaceLunarSeerCost = false,
        ReplaceLunarPodCost = false,
        ReplaceLunarBudCost = false,
        ReplaceSlabCost = false,
        ReplaceMageCost = false,
        ReplaceFrogCost = false,
        ReplaceDoppelgangerDropTable = false,
        ReplaceSacrificeArtifactDropTable = false,
        ReplaceSimulacrumOrbDropTable = false,
        ReplaceVoidFieldsOrbDropTable = false,
        ReplaceWhiteItems = false,
        ReplaceGreenItems = false,
        ReplaceRedItems = false,
        ReplaceYellowItems = false,
        ReplaceBlueItems = false,
        ReplaceVoidTier1Items = false,
        ReplaceVoidTier2Items = false,
        ReplaceVoidTier3Items = false
    };

    public static readonly ConfigPreset[] All = [
        Default,
        Easy,
        AllTheChoices,
        DefaultPrinterSpawnRate,
        NoPrinters,
        Hardcore,
        V1_2,
        V1_1,
        V1,
        Vanilla
    ];
}
