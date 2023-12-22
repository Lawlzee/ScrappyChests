using System;
using System.Collections.Generic;
using System.Text;

namespace ScrappyChests;

public enum ConfigPresetMoniker
{
    Default,
    Easy,
    AllTheChoices,
    DefaultPrinterSpawnRate,
    NoPrinters,
    Hardcore,
    V1,
    V1_1,
    Vanilla,
    Custom
}

public record ConfigPreset
{
    public static readonly ConfigPreset Default = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.Default
    };

    public static readonly ConfigPreset Easy = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.Easy,
        ReplaceAdaptiveChestDropTable = false,
        ReplaceRedItems = false,
        ReplaceLegendaryChestDropTable = false,
        ReplaceVoidPotentialDropTable = false,
        ReplaceVoidCradleDropTable = false,
        ReplaceBossDropTable = false,
        ReplaceAWUDropTable = false,
        ReplaceNewtAltarsCost = false,
        ReplaceLunarSeerCost = false
    };

    public static readonly ConfigPreset AllTheChoices = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.AllTheChoices,
        ReplaceMultiShopDropTable = false,
        ReplaceAdaptiveChestDropTable = false,
        ReplaceVoidPotentialDropTable = false,
        ReplaceLunarPodDropTable = true
    };

    public static readonly ConfigPreset DefaultPrinterSpawnRate = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.DefaultPrinterSpawnRate,
        WhitePrinterSpawnMultiplier = 1,
        GreenPrinterSpawnMultiplier = 1,
        RedPrinterSpawnMultiplier = 1,
        YellowPrinterSpawnMultiplier = 1
    };

    public static readonly ConfigPreset NoPrinters = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.NoPrinters,
        WhitePrinterSpawnMultiplier = 0,
        GreenPrinterSpawnMultiplier = 0,
        RedPrinterSpawnMultiplier = 0,
        YellowPrinterSpawnMultiplier = 0,
        AddVoidPrintersToVoidSeeds = false,
    };

    public static readonly ConfigPreset Hardcore = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.Hardcore,
        ReplaceLunarPodDropTable = true,
        ReplaceLunarBudsDropTable = true,
        WhitePrinterSpawnMultiplier = 1,
        GreenPrinterSpawnMultiplier = 1,
        RedPrinterSpawnMultiplier = 1,
        YellowPrinterSpawnMultiplier = 1,
        AddVoidPrintersToVoidSeeds = false,
        AddWhiteCauldronToBazaar = false,
        AddYellowCauldronToBazaar = false,
        AddYellowCauldronToMoon = false,
        SpeedItemSpawnMultiplier = 1,
        ReplaceLockboxDropTable = true,
        ReplaceEncrustedCacheDropTable = true,
        ReplaceCrashedMultishopDropTable = true,
        ReplaceBossHunterDropTable = true,
        ReplaceScavengerDropTable = true,
        ReplaceElderLemurianDropTable = true
    };

    public static readonly ConfigPreset V1_1 = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.V1_1,
        WhitePrinterSpawnMultiplier = 1,
        GreenPrinterSpawnMultiplier = 1,
        RedPrinterSpawnMultiplier = 1,
        YellowPrinterSpawnMultiplier = 1,
        AddYellowCauldronToBazaar = false,
        AddYellowCauldronToMoon = false,
        AddVoidPrintersToVoidSeeds = false,
        ReplaceNewtAltarsCost = false,
        ReplaceLunarSeerCost = false,
        AddWhiteCauldronToBazaar = false
    };

    public static readonly ConfigPreset V1 = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.V1,
        WhitePrinterSpawnMultiplier = 1,
        GreenPrinterSpawnMultiplier = 1,
        RedPrinterSpawnMultiplier = 1,
        YellowPrinterSpawnMultiplier = 1,
        AddVoidItemsToPrinters = false,
        AddVoidItemsToCauldrons = false,
        AddYellowCauldronToBazaar = false,
        AddYellowCauldronToMoon = false,
        AddVoidPrintersToVoidSeeds = false,
        ReplaceNewtAltarsCost = false,
        ReplaceLunarSeerCost = false,
        SpeedItemSpawnMultiplier = 1.0f,
        ReplaceVoidCradleDropTable = false,
        AddWhiteCauldronToBazaar = false
    };

    public static readonly ConfigPreset Vanilla = new ConfigPreset
    {
        Moniker = ConfigPresetMoniker.Vanilla,
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
        ReplaceNewtAltarsCost = false,
        ReplaceLunarSeerCost = false,
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

    public static readonly ConfigPreset[] AllPresets = [
        Default,
        Easy,
        AllTheChoices,
        DefaultPrinterSpawnRate,
        NoPrinters,
        Hardcore,
        V1_1,
        V1,
        Vanilla
    ];

    public ConfigPresetMoniker Moniker { get; init; }

    public bool ReplaceChestDropTable { get; init; } = true;
    public bool ReplaceMultiShopDropTable { get; init; } = true;
    public bool ReplaceAdaptiveChestDropTable { get; init; } = true;
    public bool ReplaceChanceShrineDropTable { get; init; } = true;
    public bool ReplaceLegendaryChestDropTable { get; init; } = true;
    public bool ReplaceVoidPotentialDropTable { get; init; } = true;
    public bool ReplaceVoidCradleDropTable { get; init; } = true;
    public bool ReplaceLunarPodDropTable { get; init; } = false;
    public bool ReplaceLunarBudsDropTable { get; init; } = false;

    public float WhitePrinterSpawnMultiplier { get; init; } = 1.5f;
    public float GreenPrinterSpawnMultiplier { get; init; } = 2.5f;
    public float RedPrinterSpawnMultiplier { get; init; } = 3.0f;
    public float YellowPrinterSpawnMultiplier { get; init; } = 3.0f;
    public bool AddVoidItemsToPrinters { get; init; } = true;

    public bool AddVoidPrintersToVoidSeeds { get; init; } = true;
    public float VoidSeedsPrinterWeight { get; init; } = 0.2f;
    public int VoidSeedsPrinterWhiteWeight { get; init; } = 60;
    public int VoidSeedsPrinterWhiteCreditCost { get; init; } = 25;
    public int VoidSeedsPrinterGreenWeight { get; init; } = 36;
    public int VoidSeedsPrinterGreenCreditCost { get; init; } = 40;
    public int VoidSeedsPrinterRedWeight { get; init; } = 4;
    public int VoidSeedsPrinterRedCreditCost { get; init; } = 50;

    public bool AddVoidItemsToCauldrons { get; init; } = true;
    public bool AddWhiteCauldronToBazaar { get; init; } = true;
    public bool AddYellowCauldronToBazaar { get; init; } = true;
    public bool AddYellowCauldronToMoon { get; init; } = true;
    public string YellowCauldronCost { get; init; } = "wwgy";

    public bool ReplaceLockboxDropTable { get; init; } = false;
    public bool ReplaceEncrustedCacheDropTable { get; init; } = false;
    public bool ReplaceCrashedMultishopDropTable { get; init; } = false;
    public bool ReplaceBossHunterDropTable { get; init; } = false;
    public float SpeedItemSpawnMultiplier { get; init; } = 1.25f;

    public bool ReplaceBossDropTable { get; init; } = true;
    public bool ReplaceAWUDropTable { get; init; } = true;
    public bool ReplaceScavengerDropTable { get; init; } = false;
    public bool ReplaceElderLemurianDropTable { get; init; } = false;

    public bool ReplaceNewtAltarsCost { get; init; } = true;
    public bool ReplaceLunarSeerCost { get; init; } = true;

    public bool ReplaceDoppelgangerDropTable { get; init; } = false;
    public bool ReplaceSacrificeArtifactDropTable { get; init; } = true;

    public bool ReplaceSimulacrumOrbDropTable { get; init; } = true;
    public bool ReplaceVoidFieldsOrbDropTable { get; init; } = true;

    public bool ReplaceWhiteItems { get; init; } = true;
    public bool ReplaceGreenItems { get; init; } = true;
    public bool ReplaceRedItems { get; init; } = true;
    public bool ReplaceYellowItems { get; init; } = true;
    public bool ReplaceBlueItems { get; init; } = true;
    public bool ReplaceVoidTier1Items { get; init; } = true;
    public bool ReplaceVoidTier2Items { get; init; } = true;
    public bool ReplaceVoidTier3Items { get; init; } = true;
}
