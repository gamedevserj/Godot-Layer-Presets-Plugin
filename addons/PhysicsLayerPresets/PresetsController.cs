using Godot;
using Godot.Collections;

namespace PhysicsLayerPresets;
internal static class PresetsController
{
    public static Dictionary<string, uint> GetPresets()
    {
        if (ProjectSettings.HasSetting(SettingsConstants.PhysicsLayers3DPresetsSetting))
        {
            Variant savedVariant = ProjectSettings.GetSetting(SettingsConstants.PhysicsLayers3DPresetsSetting);
            var dictionary = savedVariant.As<Dictionary<string, uint>>();
            return dictionary;
        }

        return [];
    }

    public static void AddPreset(string name, uint value)
    {
        if (ProjectSettings.HasSetting(SettingsConstants.PhysicsLayers3DPresetsSetting))
        {
            Variant savedVariant = ProjectSettings.GetSetting(SettingsConstants.PhysicsLayers3DPresetsSetting);
            var dictionary = savedVariant.As<Dictionary<string, uint>>();
            dictionary.Add(name, value);
            ProjectSettings.SetSetting(SettingsConstants.PhysicsLayers3DPresetsSetting, dictionary);
        }
        else
        {
            var dictionary = new Dictionary<string, uint>() { { name, value } };
            ProjectSettings.SetSetting(SettingsConstants.PhysicsLayers3DPresetsSetting, dictionary);
        }

        var propertyInfo = new Dictionary
            {
                { "name", SettingsConstants.PhysicsLayers3DPresetsSetting },
                { "type", (int)Variant.Type.Dictionary },
                { "hint", (int)PropertyHint.DictionaryType }, 
                // https://docs.godotengine.org/en/stable/classes/class_%40globalscope.html#enum-globalscope-propertyhint
                // 11 is for the 3D Physics layer
                { "hint_string", $"{(int)Variant.Type.String}:;{(int)Variant.Type.Int}/11:" }
            };
        ProjectSettings.AddPropertyInfo(propertyInfo);
        ProjectSettings.Save();
    }
}
