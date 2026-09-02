using Godot;
using Godot.Collections;

namespace PhysicsLayerPresets;
internal static class PresetsController
{
    public delegate void PresetEdited(PresetData preset);
    public static event PresetEdited OnPresetEdited;

    public static PresetData[] GetPresets()
    {
        if (ProjectSettings.HasSetting(SettingsConstants.PhysicsLayers3DPresetsSetting))
        {
            Variant savedVariant = ProjectSettings.GetSetting(SettingsConstants.PhysicsLayers3DPresetsSetting);
            var dictionaryPresets = savedVariant.As<Dictionary<string, Dictionary<string, uint>>>();
            System.Collections.Generic.List<PresetData> presets = [];
            foreach (var preset in dictionaryPresets)
            {
                presets.Add(new PresetData(preset));
            }
            return [.. presets];
        }

        return [];
    }

    public static void EditPreset(PresetData preset)
    {
        Variant savedVariant = ProjectSettings.GetSetting(SettingsConstants.PhysicsLayers3DPresetsSetting);
        var dictionaryPresets = savedVariant.As<Dictionary<string, Dictionary<string, uint>>>();
        dictionaryPresets[preset.Id] = new Dictionary<string, uint> { { preset.Name, preset.Layer} };
        ProjectSettings.SetSetting(SettingsConstants.PhysicsLayers3DPresetsSetting, dictionaryPresets);
        Save();
        OnPresetEdited?.Invoke(preset);
    }

    public static void AddPreset(string id, string name, uint value)
    {
        if (ProjectSettings.HasSetting(SettingsConstants.PhysicsLayers3DPresetsSetting))
        {
            Variant savedVariant = ProjectSettings.GetSetting(SettingsConstants.PhysicsLayers3DPresetsSetting);
            var dictionary = savedVariant.As<Dictionary<string, Dictionary<string, uint>>>();
            dictionary.Add(id, new Dictionary<string, uint> { { name, value} });
            ProjectSettings.SetSetting(SettingsConstants.PhysicsLayers3DPresetsSetting, dictionary);
        }
        else
        {
            var dictionary = new Dictionary<string, Dictionary<string, uint>>
            {
                { id, new Dictionary<string, uint>() { { name, value } } }
            };
            ProjectSettings.SetSetting(SettingsConstants.PhysicsLayers3DPresetsSetting, dictionary);
        }

        //var propertyInfo = new Dictionary
        //    {
        //        { "name", SettingsConstants.PhysicsLayers3DPresetsSetting },
        //        { "type", (int)Variant.Type.Dictionary },
        //        { "hint", (int)PropertyHint.DictionaryType }, 
        //        // https://docs.godotengine.org/en/stable/classes/class_%40globalscope.html#enum-globalscope-propertyhint
        //        // 11 is for the 3D Physics layer
        //        { "hint_string", $"{(int)Variant.Type.String}:;{(int)Variant.Type.Int}/11:" }
        //    };
        //ProjectSettings.AddPropertyInfo(propertyInfo);
        Save();
    }

    private static void Save()
    {
        ProjectSettings.SetAsInternal(SettingsConstants.PhysicsLayers3DPresetsSetting, true);
        ProjectSettings.Save();
    }
}
