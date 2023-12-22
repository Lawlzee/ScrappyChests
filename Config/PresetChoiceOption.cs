using BepInEx.Configuration;
using RoR2.UI;
using RiskOfOptions.Options;
using UnityEngine;
using RiskOfOptions.Components.Options;
using RiskOfOptions.Components.Panel;
using System.Reflection;
using RiskOfOptions.Components.RuntimePrefabs;
using RiskOfOptions.OptionConfigs;

namespace ScrappyChests;

public class PresetChoiceOption : ChoiceOption
{
    private DropDownController _dropdownController;

    public PresetChoiceOption(ConfigEntryBase configEntry)
        : base(configEntry, new ChoiceConfig { description = configEntry.BoxedValue.ToString() })
    {
    }

    public override GameObject CreateOptionGameObject(GameObject prefab, Transform parent)
    {
        GameObject button = base.CreateOptionGameObject(prefab, parent);

        _dropdownController = button.GetComponentInChildren<DropDownController>();
        button.GetComponentInChildren<HGButton>().onSelect.AddListener(() => UpdateDescription(Value.ToString()));

        return button;
    }

    public void UpdateDescription(string description)
    {
        var panelField = typeof(ModOptionPanelController).GetField("_panel", BindingFlags.NonPublic | BindingFlags.Instance);
        ModOptionsPanelPrefab panel = (ModOptionsPanelPrefab)panelField.GetValue(_dropdownController.optionController);

        SetDescription(description, new BaseOptionConfig());
        panel.ModOptionsDescriptionPanel.GetComponentInChildren<HGTextMeshProUGUI>().SetText(description);
    }
}
