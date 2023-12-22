using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrappyChests;

public static class ConfigEntryBaseExtensions
{
    public static void AddSettingChanged(this ConfigEntryBase config, EventHandler settingChanged)
    {
        if (config is ConfigEntry<bool> boolConfig)
        {
            boolConfig.SettingChanged += settingChanged;
        }
        else if (config is ConfigEntry<float> floatConfig)
        {
            floatConfig.SettingChanged += settingChanged;
        }
        else if (config is ConfigEntry<int> intConfig)
        {
            intConfig.SettingChanged += settingChanged;
        }
        else if (config is ConfigEntry<string> stringConfig)
        {
            stringConfig.SettingChanged += settingChanged;
        }
        else
        {
            throw new NotSupportedException(config.GetType().Name);
        }
    }
}
