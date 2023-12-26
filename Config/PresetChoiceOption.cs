using BepInEx.Configuration;
using RoR2.UI;
using RiskOfOptions.Options;
using UnityEngine;
using RiskOfOptions.Components.Options;
using RiskOfOptions.Components.Panel;
using System.Reflection;
using RiskOfOptions.Components.RuntimePrefabs;
using RiskOfOptions.OptionConfigs;
using System.Linq;

namespace ScrappyChests;

public class PresetChoiceOption : ChoiceOption
{
    private DropDownController _dropdownController;

    public PresetChoiceOption(ConfigEntryBase configEntry)
        : base(configEntry, new ChoiceConfig { description = GetCurrentDescription(configEntry.BoxedValue) })
    {
    }

    public override GameObject CreateOptionGameObject(GameObject prefab, Transform parent)
    {
        GameObject button = base.CreateOptionGameObject(prefab, parent);

        _dropdownController = button.GetComponentInChildren<DropDownController>();

        button.GetComponentInChildren<HGButton>().onSelect.AddListener(() =>
        {
            string description = GetCurrentDescription(Value);
            UpdateDescription(description, true);
        });

        return button;
    }

    private static string GetCurrentDescription(object value)
    {
        var moniker = (ConfigPresetMoniker)value;
        var current = ConfigPresets.All
            .Where(x => x.Moniker == moniker)
            .FirstOrDefault();

        return current?.Description ?? ConfigPresetDescriptions.Custom;
    }

    public void UpdateDescription(string description, bool updateDescriptionPanel)
    {
        SetDescription(description, new BaseOptionConfig());

        if (!updateDescriptionPanel)
        {
            return;
        }

        var panelField = typeof(ModOptionPanelController).GetField("_panel", BindingFlags.NonPublic | BindingFlags.Instance);
        ModOptionsPanelPrefab panel = (ModOptionsPanelPrefab)panelField.GetValue(_dropdownController.optionController);

        panel.ModOptionsDescriptionPanel.GetComponentInChildren<HGTextMeshProUGUI>().SetText(description);
    }
}
