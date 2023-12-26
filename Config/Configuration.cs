using BepInEx.Configuration;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RiskOfOptions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using BepInEx;
using System.Linq;

namespace ScrappyChests;

internal class Configuration
{
    public static Configuration Instance { get; set; }

    private bool _presetIsChanging;
    private bool _settingIsChanging;

    public ConfigEntry<bool> ModEnabled { get; }
    public ConfigEntry<ConfigPresetMoniker> Preset { get; }
    public PresetChoiceOption PresetChoiceOption { get; }

    public ConfigEntry<bool> ReplaceChestDropTable { get; }
    public ConfigEntry<bool> ReplaceMultiShopDropTable { get; }
    public ConfigEntry<bool> ReplaceAdaptiveChestDropTable { get; }
    public ConfigEntry<bool> ReplaceChanceShrineDropTable { get; }
    public ConfigEntry<bool> ReplaceLegendaryChestDropTable { get; }
    public ConfigEntry<bool> ReplaceVoidPotentialDropTable { get; }
    public ConfigEntry<bool> ReplaceVoidCradleDropTable { get; }
    public ConfigEntry<bool> ReplaceLunarPodDropTable { get; }
    public ConfigEntry<bool> ReplaceLunarBudsDropTable { get; }

    public ConfigEntry<float> WhitePrinterSpawnMultiplier { get; }
    public ConfigEntry<float> GreenPrinterSpawnMultiplier { get; }
    public ConfigEntry<float> RedPrinterSpawnMultiplier { get; }
    public ConfigEntry<float> YellowPrinterSpawnMultiplier { get; }
    public ConfigEntry<int> MinimumStageForRedPrinters { get; }
    public ConfigEntry<bool> AddVoidItemsToPrinters { get; }

    public ConfigEntry<bool> AddVoidPrintersToVoidSeeds { get; }
    public ConfigEntry<float> VoidSeedsPrinterWeight { get; }
    public ConfigEntry<int> VoidSeedsPrinterWhiteWeight { get; }
    public ConfigEntry<int> VoidSeedsPrinterWhiteCreditCost { get; }
    public ConfigEntry<int> VoidSeedsPrinterGreenWeight { get; }
    public ConfigEntry<int> VoidSeedsPrinterGreenCreditCost { get; }
    public ConfigEntry<int> VoidSeedsPrinterRedWeight { get; }
    public ConfigEntry<int> VoidSeedsPrinterRedCreditCost { get; }

    public ConfigEntry<bool> AddVoidItemsToCauldrons { get; }
    public ConfigEntry<bool> AddWhiteCauldronToBazaar { get; }
    public ConfigEntry<bool> AddYellowCauldronToBazaar { get; }
    public ConfigEntry<bool> AddYellowCauldronToMoon { get; }
    public ConfigEntry<string> _yellowCauldronCost { get; }

    public ConfigEntry<bool> ReplaceLockboxDropTable { get; }
    public ConfigEntry<bool> ReplaceEncrustedCacheDropTable { get; }
    public ConfigEntry<bool> ReplaceCrashedMultishopDropTable { get; }
    public ConfigEntry<bool> ReplaceBossHunterDropTable { get; }
    public ConfigEntry<float> SpeedItemSpawnMultiplier { get; }

    public ConfigEntry<bool> ReplaceBossDropTable { get; }
    public ConfigEntry<bool> ReplaceAWUDropTable { get; }
    public ConfigEntry<bool> ReplaceScavengerDropTable { get; }
    public ConfigEntry<bool> ReplaceElderLemurianDropTable { get; }
    public ConfigEntry<bool> ReplaceLunarCoinDrops { get; }

    public ConfigEntry<bool> ReplaceNewtAltarsCost { get; }
    public ConfigEntry<bool> ReplaceLunarSeerCost { get; }
    public ConfigEntry<bool> ReplaceLunarPodCost { get; }
    public ConfigEntry<bool> ReplaceLunarBudCost { get; }
    public ConfigEntry<bool> ReplaceSlabCost { get; }
    public ConfigEntry<bool> ReplaceMageCost { get; }
    public ConfigEntry<bool> ReplaceFrogCost { get; }
    public ConfigEntry<bool> ReplaceShrineOfOrderCost { get; }

    public ConfigEntry<bool> ReplaceDoppelgangerDropTable { get; }
    public ConfigEntry<bool> ReplaceSacrificeArtifactDropTable { get; }

    public ConfigEntry<bool> ReplaceSimulacrumOrbDropTable { get; }
    public ConfigEntry<bool> ReplaceVoidFieldsOrbDropTable { get; }

    public ConfigEntry<bool> ReplaceWhiteItems { get; }
    public ConfigEntry<bool> ReplaceGreenItems { get; }
    public ConfigEntry<bool> ReplaceRedItems { get; }
    public ConfigEntry<bool> ReplaceYellowItems { get; }
    public ConfigEntry<bool> ReplaceBlueItems { get; }
    public ConfigEntry<bool> ReplaceVoidTier1Items { get; }
    public ConfigEntry<bool> ReplaceVoidTier2Items { get; }
    public ConfigEntry<bool> ReplaceVoidTier3Items { get; }

    public ConfigEntry<string> _maxCompletedEclipseLevels { get; }

    public Configuration(ConfigFile config)
    {
        var defaultConfig = ConfigPresets.Default;
        ModEnabled = config.Bind("Configuration", "Mod enabled", true, "Mod enabled");
        Preset = config.Bind("Configuration", "Preset", ConfigPresetMoniker.Default, "Configuration preset");
        PresetChoiceOption = new PresetChoiceOption(Preset);

        ReplaceChestDropTable = config.Bind("Chests", "Chest", defaultConfig.ReplaceChestDropTable, "Chests will drop scrap instead of items");
        ReplaceMultiShopDropTable = config.Bind("Chests", "Multishop Terminal", defaultConfig.ReplaceMultiShopDropTable, "Multishop Terminals will drop scrap instead of items");
        ReplaceAdaptiveChestDropTable = config.Bind("Chests", "Adaptive Chest", defaultConfig.ReplaceAdaptiveChestDropTable, "Adaptive Chests will drop scrap instead of items");
        ReplaceChanceShrineDropTable = config.Bind("Chests", "Shrine of Chance", defaultConfig.ReplaceChanceShrineDropTable, "Shrine of Chance will drop scrap instead of items");
        ReplaceLegendaryChestDropTable = config.Bind("Chests", "Legendary Chest", defaultConfig.ReplaceLegendaryChestDropTable, "Legendary Chests will drop scrap instead of items");
        ReplaceVoidPotentialDropTable = config.Bind("Chests", "Void Potential", defaultConfig.ReplaceVoidPotentialDropTable, "Void Potential will drop scrap instead of items");
        ReplaceVoidCradleDropTable = config.Bind("Chests", "Void Cradle", defaultConfig.ReplaceVoidCradleDropTable, "Void Cradle will drop scrap instead of items");
        ReplaceLunarPodDropTable = config.Bind("Chests", "Lunar Pod", defaultConfig.ReplaceLunarPodDropTable, "Lunar Pods will drop Beads of Fealty instead of items");
        ReplaceLunarBudsDropTable = config.Bind("Chests", "Lunar Bud", defaultConfig.ReplaceLunarBudsDropTable, "Lunar Buds in the Bazaar Between Time will always sell Beads of Fealty");

        WhitePrinterSpawnMultiplier = config.Bind("Printers", "White printer spawn multiplier", defaultConfig.WhitePrinterSpawnMultiplier, "Controls the spawn rate of white printers. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn printers.");
        GreenPrinterSpawnMultiplier = config.Bind("Printers", "Green printer spawn multiplier", defaultConfig.GreenPrinterSpawnMultiplier, "Controls the spawn rate of green printers. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn printers.");
        RedPrinterSpawnMultiplier = config.Bind("Printers", "Red printer spawn multiplier", defaultConfig.RedPrinterSpawnMultiplier, "Controls the spawn rate of ref printers. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn printers.");
        YellowPrinterSpawnMultiplier = config.Bind("Printers", "Yellow printer spawn multiplier", defaultConfig.YellowPrinterSpawnMultiplier, "Controls the spawn rate of yellow printers. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn printers.");
        MinimumStageForRedPrinters = config.Bind("Printers", "Minimum stage for red printers to spawn", defaultConfig.MinimumStageForRedPrinters, "Minimum stage for red printers to spawn. The vanilla value is 5.");
        AddVoidItemsToPrinters = config.Bind("Printers", "Add void items to Printers", defaultConfig.AddVoidItemsToPrinters, "Add void items to Printers");

        AddVoidPrintersToVoidSeeds = config.Bind("Printers", "Add void printers to void seeds", defaultConfig.AddVoidPrintersToVoidSeeds, "Add void printers to void seeds");
        VoidSeedsPrinterWeight = config.Bind("Printers", "Void seeds void printers weigth", defaultConfig.VoidSeedsPrinterWeight, "Void seeds void printers weigth");
        VoidSeedsPrinterWhiteWeight = config.Bind("Printers", "Void seeds white void printers weigth", defaultConfig.VoidSeedsPrinterWhiteWeight, "Void seeds white void printers weigth");
        VoidSeedsPrinterWhiteCreditCost = config.Bind("Printers", "Void seeds white void printers credit cost", defaultConfig.VoidSeedsPrinterWhiteCreditCost, "Void seeds white void printers credit cost");
        VoidSeedsPrinterGreenWeight = config.Bind("Printers", "Void seeds green void printers weigth", defaultConfig.VoidSeedsPrinterGreenWeight, "Void seeds green void printers weigth");
        VoidSeedsPrinterGreenCreditCost = config.Bind("Printers", "Void seeds green void printers credit cost", defaultConfig.VoidSeedsPrinterGreenCreditCost, "Void seeds green void printers credit cost");
        VoidSeedsPrinterRedWeight = config.Bind("Printers", "Void seeds red void printers weigth", defaultConfig.VoidSeedsPrinterRedWeight, "Void seeds red void printers weigth");
        VoidSeedsPrinterRedCreditCost = config.Bind("Printers", "Void seeds red void printers credit cost", defaultConfig.VoidSeedsPrinterRedCreditCost, "Void seeds red void printers credit cost");

        AddVoidItemsToCauldrons = config.Bind("Cauldrons", "Add void items to Cauldrons", defaultConfig.AddVoidItemsToCauldrons, "Add void items to Cauldrons");
        AddWhiteCauldronToBazaar = config.Bind("Cauldrons", "Add white Cauldrons to the Bazaar", defaultConfig.AddWhiteCauldronToBazaar, "Add a white Cauldrons to the Bazaar");
        AddYellowCauldronToBazaar = config.Bind("Cauldrons", "Add yellow Cauldrons to the Bazaar", defaultConfig.AddYellowCauldronToBazaar, "Add a yellow Cauldrons to the Bazaar");
        AddYellowCauldronToMoon = config.Bind("Cauldrons", "Add yellow Cauldrons to the Moon", defaultConfig.AddYellowCauldronToMoon, "Add a yellow Cauldrons to the Moon");
        _yellowCauldronCost = config.Bind("Cauldrons", "Yellow Cauldrons Cost", defaultConfig.YellowCauldronCost, """
                Cost to use the yellow Cauldrons
                
                w: white
                g: green
                r: red
                y: yellow (boss)

                Examples:
                ywg: 1 yellow, 1 white, 1 green
                wwwrr: 3 white, 2 red
                """);

        ReplaceLockboxDropTable = config.Bind("Items", "Rusted Key", defaultConfig.ReplaceLockboxDropTable, "Lockboxes will drop scrap instead of items");
        ReplaceEncrustedCacheDropTable = config.Bind("Items", "Encrusted Key", defaultConfig.ReplaceEncrustedCacheDropTable, "Encrusted Cache will drop scrap instead of items");
        ReplaceCrashedMultishopDropTable = config.Bind("Items", "Crashed Multishop", defaultConfig.ReplaceCrashedMultishopDropTable, "Crashed Multishop will drop scrap instead of items");
        ReplaceBossHunterDropTable = config.Bind("Items", "Trophy Hunters Tricorn", defaultConfig.ReplaceBossHunterDropTable, "Trophy Hunter's Tricorn will drop scrap instead of items");
        SpeedItemSpawnMultiplier = config.Bind("Items", "Speed items spawn multiplier", defaultConfig.SpeedItemSpawnMultiplier, "Controls the spawn rate of speed items. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn speed items.");

        ReplaceBossDropTable = config.Bind("Mobs", "Boss", defaultConfig.ReplaceBossDropTable, "Defeating a Boss will drop scrap instead of items");
        ReplaceAWUDropTable = config.Bind("Mobs", "Alloy Worship Unit", defaultConfig.ReplaceAWUDropTable, "Alloy Worship Unit will drop scrap instead of items");
        ReplaceScavengerDropTable = config.Bind("Mobs", "Scavenger", defaultConfig.ReplaceScavengerDropTable, "Scavenger will drop scrap instead of items");
        ReplaceElderLemurianDropTable = config.Bind("Mobs", "Elite Elder Lemurian", defaultConfig.ReplaceElderLemurianDropTable, "The Elite Elder Lemurian in the hidden chamber of Abandoned Aqueduct will drop scrap instead of bands");
        ReplaceLunarCoinDrops = config.Bind("Mobs", "Lunar coins drops", defaultConfig.ReplaceLunarCoinDrops, "Mobs will drop white scrap instead of Lunar coins");

        ReplaceNewtAltarsCost = config.Bind("Costs", "Newt Altars uses white items", defaultConfig.ReplaceNewtAltarsCost, "Newt Altar uses white items as the activation cost instead of lunar coins");
        ReplaceLunarSeerCost = config.Bind("Costs", "Lunar Seer uses white items", defaultConfig.ReplaceLunarSeerCost, "Lunar Seer (dream) uses white items as the activation cost instead of lunar coins");
        ReplaceLunarPodCost = config.Bind("Costs", "Lunar pods uses white items", defaultConfig.ReplaceLunarPodCost, "Lunar pods uses white items as the activation cost instead of lunar coins");
        ReplaceLunarBudCost = config.Bind("Costs", "Lunar buds uses white items", defaultConfig.ReplaceLunarBudCost, "Lunar buds uses white items as the activation cost instead of lunar coins");
        ReplaceSlabCost = config.Bind("Costs", "Slab uses white items", defaultConfig.ReplaceSlabCost, "The Slab in the Bazaar Between Time uses white items as the activation cost instead of lunar coins");
        ReplaceMageCost = config.Bind("Costs", "Unlocking artificier uses white items", defaultConfig.ReplaceMageCost, "Unlocking artificier uses white items as the activation cost instead of lunar coins");
        ReplaceFrogCost = config.Bind("Costs", "Petting the frog uses white items", defaultConfig.ReplaceFrogCost, "Petting the frog uses white items as the activation cost instead of lunar coins");
        ReplaceShrineOfOrderCost = config.Bind("Costs", "Shrine of order uses white items", defaultConfig.ReplaceShrineOfOrderCost, "Shrine of order uses white items as the activation cost instead of lunar coins");

        ReplaceDoppelgangerDropTable = config.Bind("Artifacts", "Relentless Doppelganger", defaultConfig.ReplaceDoppelgangerDropTable, "The Relentless Doppelganger from the Artifact of Vengeance will drop scrap instead of items");
        ReplaceSacrificeArtifactDropTable = config.Bind("Artifacts", "Artifact of Sacrifice", defaultConfig.ReplaceSacrificeArtifactDropTable, "When using the Artifact of Sacrifice, mobs will drop scrap instead of items");

        ReplaceSimulacrumOrbDropTable = config.Bind("Waves", "Simulacrum", defaultConfig.ReplaceSimulacrumOrbDropTable, "The orb reward after each wave of Simulacrum will drop scrap instead of items");
        ReplaceVoidFieldsOrbDropTable = config.Bind("Waves", "Void Fields", defaultConfig.ReplaceVoidFieldsOrbDropTable, "The orb reward after each wave of the Void Fields will drop scrap instead of items");

        ReplaceWhiteItems = config.Bind("Tiers", "White item", defaultConfig.ReplaceWhiteItems, "Replace white item drops with white scrap");
        ReplaceGreenItems = config.Bind("Tiers", "Green item", defaultConfig.ReplaceGreenItems, "Replace green item drops with green scrap");
        ReplaceRedItems = config.Bind("Tiers", "Red item", defaultConfig.ReplaceRedItems, "Replace red item drops with red scrap");
        ReplaceYellowItems = config.Bind("Tiers", "Yellow item", defaultConfig.ReplaceYellowItems, "Replace yellow item drops with yellow scrap");
        ReplaceBlueItems = config.Bind("Tiers", "Blue item", defaultConfig.ReplaceBlueItems, "Replace blue item drops with Beads of Fealty");
        ReplaceVoidTier1Items = config.Bind("Tiers", "Void tier 1 item", defaultConfig.ReplaceVoidTier1Items, "Replace void tier 1 drops with white scrap");
        ReplaceVoidTier2Items = config.Bind("Tiers", "Void tier 2 item", defaultConfig.ReplaceVoidTier2Items, "Replace void tier 2 drops with green scrap");
        ReplaceVoidTier3Items = config.Bind("Tiers", "Void tier 3 item", defaultConfig.ReplaceVoidTier3Items, "Replace void tier 3 drops with red scrap");

        _maxCompletedEclipseLevels = config.Bind("Eclipse", "Max completed Eclipse levels", "Commando=0,Huntress=0,Bandit2=0,Toolbot=0,Engi=0,Mage=0,Merc=0,Treebot=0,Loader=0,Croco=0,Captain=0,Heretic=0,Railgunner=0,VoidSurvivor=0", "Max eclipse level per survivor reached with Scappy Chests enabled");

        Preset.SettingChanged += OnPresetChanged;

        foreach ((ConfigEntryBase c, object presetValue) in GetAllConfigs(ConfigPresets.Default))
        {
            c.AddSettingChanged(OnSettingChanged);
        }
    }

    public void InitUI(PluginInfo info)
    {
        ModSettingsManager.AddOption(new CheckBoxOption(ModEnabled));
        ModSettingsManager.AddOption(PresetChoiceOption);

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
        ModSettingsManager.AddOption(new IntSliderOption(MinimumStageForRedPrinters, new IntSliderConfig() { min = 1, max = 10 }));
        ModSettingsManager.AddOption(new CheckBoxOption(AddVoidItemsToPrinters));

        ModSettingsManager.AddOption(new CheckBoxOption(AddVoidPrintersToVoidSeeds));
        ModSettingsManager.AddOption(new StepSliderOption(VoidSeedsPrinterWeight, new StepSliderConfig() { min = 0, max = 5, increment = 0.025f, formatString = "{0:0.###}" }));
        ModSettingsManager.AddOption(new IntSliderOption(VoidSeedsPrinterWhiteWeight, new IntSliderConfig() { min = 0, max = 100 }));
        ModSettingsManager.AddOption(new IntSliderOption(VoidSeedsPrinterWhiteCreditCost, new IntSliderConfig() { min = 0, max = 100 }));
        ModSettingsManager.AddOption(new IntSliderOption(VoidSeedsPrinterGreenWeight, new IntSliderConfig() { min = 0, max = 100 }));
        ModSettingsManager.AddOption(new IntSliderOption(VoidSeedsPrinterGreenCreditCost, new IntSliderConfig() { min = 0, max = 100 }));
        ModSettingsManager.AddOption(new IntSliderOption(VoidSeedsPrinterRedWeight, new IntSliderConfig() { min = 0, max = 100 }));
        ModSettingsManager.AddOption(new IntSliderOption(VoidSeedsPrinterRedCreditCost, new IntSliderConfig() { min = 0, max = 100 }));

        ModSettingsManager.AddOption(new CheckBoxOption(AddVoidItemsToCauldrons));
        ModSettingsManager.AddOption(new CheckBoxOption(AddWhiteCauldronToBazaar));
        ModSettingsManager.AddOption(new CheckBoxOption(AddYellowCauldronToBazaar));
        ModSettingsManager.AddOption(new CheckBoxOption(AddYellowCauldronToMoon));
        ModSettingsManager.AddOption(new StringInputFieldOption(_yellowCauldronCost));

        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceLockboxDropTable));
        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceEncrustedCacheDropTable));
        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceCrashedMultishopDropTable));
        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceBossHunterDropTable));
        ModSettingsManager.AddOption(new StepSliderOption(SpeedItemSpawnMultiplier, new StepSliderConfig() { min = 0, max = 5, increment = 0.05f, formatString = "{0:0.##}x" }));

        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceBossDropTable));
        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceAWUDropTable));
        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceScavengerDropTable));
        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceElderLemurianDropTable));
        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceLunarCoinDrops));

        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceNewtAltarsCost));
        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceLunarSeerCost));
        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceLunarPodCost));
        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceLunarBudCost));
        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceSlabCost));
        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceMageCost));
        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceFrogCost));
        ModSettingsManager.AddOption(new CheckBoxOption(ReplaceShrineOfOrderCost));

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

        ModSettingsManager.AddOption(new StringInputFieldOption(_maxCompletedEclipseLevels));

        ModSettingsManager.SetModIcon(LoadIconSprite());

        Sprite LoadIconSprite()
        {
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(info.Location), "icon.png")));
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        }
    }

    public (int White, int Green, int Red, int Yellow) YellowCauldronCost
    {
        get
        {
            int white = 0;
            int green = 0;
            int red = 0;
            int yellow = 0;

            foreach (char c in _yellowCauldronCost.Value)
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
    }

    public Dictionary<string, int> MaxCompletedEclipseLevels
    {
        get
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            string[] configs = _maxCompletedEclipseLevels.Value.Split(',');
            foreach (string config in configs)
            {
                string[] parts = config.Split('=');
                if (parts.Length == 2 && int.TryParse(parts[1], out int level))
                {
                    result[parts[0]] = level;
                }
            }

            return result;
        }
        set
        {
            _maxCompletedEclipseLevels.Value = string.Join(
                ",", 
                value
                    .OrderBy(x => x.Key)
                    .Select(x => $"{x.Key}={x.Value}"));
        }
    }

    private void OnSettingChanged(object sender, EventArgs e)
    {
        if (_presetIsChanging)
        {
            return;
        }

        _settingIsChanging = true;

        var preset = GetCurrentPreset();
        Preset.Value = preset;

        _settingIsChanging = false;
    }

    private IEnumerable<(ConfigEntryBase Config, object PresetValue)> GetAllConfigs(ConfigPreset preset)
    {
        yield return (ReplaceChestDropTable, preset.ReplaceChestDropTable);
        yield return (ReplaceMultiShopDropTable, preset.ReplaceMultiShopDropTable);
        yield return (ReplaceAdaptiveChestDropTable, preset.ReplaceAdaptiveChestDropTable);
        yield return (ReplaceChanceShrineDropTable, preset.ReplaceChanceShrineDropTable);
        yield return (ReplaceLegendaryChestDropTable, preset.ReplaceLegendaryChestDropTable);
        yield return (ReplaceVoidPotentialDropTable, preset.ReplaceVoidPotentialDropTable);
        yield return (ReplaceVoidCradleDropTable, preset.ReplaceVoidCradleDropTable);
        yield return (ReplaceLunarPodDropTable, preset.ReplaceLunarPodDropTable);
        yield return (ReplaceLunarBudsDropTable, preset.ReplaceLunarBudsDropTable);
        yield return (WhitePrinterSpawnMultiplier, preset.WhitePrinterSpawnMultiplier);
        yield return (GreenPrinterSpawnMultiplier, preset.GreenPrinterSpawnMultiplier);
        yield return (RedPrinterSpawnMultiplier, preset.RedPrinterSpawnMultiplier);
        yield return (YellowPrinterSpawnMultiplier, preset.YellowPrinterSpawnMultiplier);
        yield return (MinimumStageForRedPrinters, preset.MinimumStageForRedPrinters);
        yield return (AddVoidItemsToPrinters, preset.AddVoidItemsToPrinters);
        yield return (AddVoidPrintersToVoidSeeds, preset.AddVoidPrintersToVoidSeeds);
        yield return (VoidSeedsPrinterWeight, preset.VoidSeedsPrinterWeight);
        yield return (VoidSeedsPrinterWhiteWeight, preset.VoidSeedsPrinterWhiteWeight);
        yield return (VoidSeedsPrinterWhiteCreditCost, preset.VoidSeedsPrinterWhiteCreditCost);
        yield return (VoidSeedsPrinterGreenWeight, preset.VoidSeedsPrinterGreenWeight);
        yield return (VoidSeedsPrinterGreenCreditCost, preset.VoidSeedsPrinterGreenCreditCost);
        yield return (VoidSeedsPrinterRedWeight, preset.VoidSeedsPrinterRedWeight);
        yield return (VoidSeedsPrinterRedCreditCost, preset.VoidSeedsPrinterRedCreditCost);
        yield return (AddVoidItemsToCauldrons, preset.AddVoidItemsToCauldrons);
        yield return (AddWhiteCauldronToBazaar, preset.AddWhiteCauldronToBazaar);
        yield return (AddYellowCauldronToBazaar, preset.AddYellowCauldronToBazaar);
        yield return (AddYellowCauldronToMoon, preset.AddYellowCauldronToMoon);
        yield return (_yellowCauldronCost, preset.YellowCauldronCost);
        yield return (ReplaceLockboxDropTable, preset.ReplaceLockboxDropTable);
        yield return (ReplaceEncrustedCacheDropTable, preset.ReplaceEncrustedCacheDropTable);
        yield return (ReplaceCrashedMultishopDropTable, preset.ReplaceCrashedMultishopDropTable);
        yield return (ReplaceBossHunterDropTable, preset.ReplaceBossHunterDropTable);
        yield return (SpeedItemSpawnMultiplier, preset.SpeedItemSpawnMultiplier);
        yield return (ReplaceBossDropTable, preset.ReplaceBossDropTable);
        yield return (ReplaceAWUDropTable, preset.ReplaceAWUDropTable);
        yield return (ReplaceScavengerDropTable, preset.ReplaceScavengerDropTable);
        yield return (ReplaceElderLemurianDropTable, preset.ReplaceElderLemurianDropTable);
        yield return (ReplaceLunarCoinDrops, preset.ReplaceLunarCoinDrops);
        yield return (ReplaceNewtAltarsCost, preset.ReplaceNewtAltarsCost);
        yield return (ReplaceLunarSeerCost, preset.ReplaceLunarSeerCost);
        yield return (ReplaceLunarPodCost, preset.ReplaceLunarPodCost);
        yield return (ReplaceLunarBudCost, preset.ReplaceLunarBudCost);
        yield return (ReplaceSlabCost, preset.ReplaceSlabCost);
        yield return (ReplaceMageCost, preset.ReplaceMageCost);
        yield return (ReplaceFrogCost, preset.ReplaceFrogCost);
        yield return (ReplaceShrineOfOrderCost, preset.ReplaceShrineOfOrderCost);
        yield return (ReplaceDoppelgangerDropTable, preset.ReplaceDoppelgangerDropTable);
        yield return (ReplaceSacrificeArtifactDropTable, preset.ReplaceSacrificeArtifactDropTable);
        yield return (ReplaceSimulacrumOrbDropTable, preset.ReplaceSimulacrumOrbDropTable);
        yield return (ReplaceVoidFieldsOrbDropTable, preset.ReplaceVoidFieldsOrbDropTable);
        yield return (ReplaceWhiteItems, preset.ReplaceWhiteItems);
        yield return (ReplaceGreenItems, preset.ReplaceGreenItems);
        yield return (ReplaceRedItems, preset.ReplaceRedItems);
        yield return (ReplaceYellowItems, preset.ReplaceYellowItems);
        yield return (ReplaceBlueItems, preset.ReplaceBlueItems);
        yield return (ReplaceVoidTier1Items, preset.ReplaceVoidTier1Items);
        yield return (ReplaceVoidTier2Items, preset.ReplaceVoidTier2Items);
        yield return (ReplaceVoidTier3Items, preset.ReplaceVoidTier3Items);
    }

    public ConfigPresetMoniker GetCurrentPreset()
    {
        foreach (var preset in ConfigPresets.All)
        {
            bool match = GetAllConfigs(preset)
                .All(x => EqualityComparer<object>.Default.Equals(x.Config.BoxedValue, x.PresetValue));

            if (match)
            {
                return preset.Moniker;
            }
        }

        return ConfigPresetMoniker.Custom;
    }

    private void OnPresetChanged(object sender, EventArgs e)
    {
        if (Preset.Value == ConfigPresetMoniker.Custom)
        {
            PresetChoiceOption.UpdateDescription(ConfigPresetDescriptions.Custom, !_settingIsChanging);
            return;
        }

        ConfigPreset preset = ConfigPresets.All
            .First(x => x.Moniker == Preset.Value);

        PresetChoiceOption.UpdateDescription(preset.Description, !_settingIsChanging);

        _presetIsChanging = true;
        foreach ((ConfigEntryBase config, object presetValue) in GetAllConfigs(preset))
        {
            config.BoxedValue = presetValue;
        }

        _presetIsChanging = false;
    }
}
