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
    V1_2,
    Vanilla,
    Custom
}

public record ConfigPreset
{
    public ConfigPresetMoniker Moniker { get; init; }
    public string Description { get; init; }

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
    public int MinimumStageForRedPrinters { get; init; } = 2;
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
    public bool ReplaceLunarCoinDrops { get; init; } = true;

    public bool ReplaceNewtAltarsCost { get; init; } = true;
    public bool ReplaceLunarSeerCost { get; init; } = true;
    public bool ReplaceLunarPodCost { get; init; } = true;
    public bool ReplaceLunarBudCost { get; init; } = true;
    public bool ReplaceSlabCost { get; init; } = true;
    public bool ReplaceMageCost { get; init; } = true;
    public bool ReplaceFrogCost { get; init; } = true;
    public bool ReplaceShrineOfOrderCost { get; init; } = false;

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
