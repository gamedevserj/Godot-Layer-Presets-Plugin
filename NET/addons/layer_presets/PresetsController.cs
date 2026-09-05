using Godot;
using Godot.Collections;

namespace LayerPresets;
internal static class PresetsController
{
    public delegate void PresetEdited(PresetData preset);
    public static event PresetEdited OnPresetEdited;

    public delegate void PresetDeleted(string id);
    public static event PresetDeleted OnPresetDeleted;

    public delegate void AllPresetsDeleted(PropertyHint propertyHint);
    public static event AllPresetsDeleted OnAllPresetsDeleted;

    private static Dictionary<string, PresetData> _physics2dPresets = [];
    private static Dictionary<string, PresetData> _physics3dPresets = [];
    private static Dictionary<string, PresetData> _render2dPresets = [];
    private static Dictionary<string, PresetData> _render3dPresets = [];
    private static Dictionary<string, PresetData> _navigation2dPresets = [];
    private static Dictionary<string, PresetData> _navigation3dPresets = [];
    private static Dictionary<string, PresetData> _avoidance = [];

    private static Dictionary<PropertyHint, Dictionary<string, PresetData>> _presetTypeBindings = [];

    public static PresetData GetPreset(string id, PropertyHint propertyHint) => GetAllPresets(propertyHint)[id];

    public static Dictionary<string, PresetData> GetAllPresets(PropertyHint propertyHint)
    {
        if (_presetTypeBindings.Count == 0)
        {
            _presetTypeBindings = new()
            {
                { PropertyHint.Layers2DRender, _render2dPresets },
                { PropertyHint.Layers2DPhysics, _physics2dPresets },
                { PropertyHint.Layers2DNavigation, _navigation2dPresets },
                { PropertyHint.Layers3DRender, _render3dPresets },
                { PropertyHint.Layers3DPhysics, _physics3dPresets },
                { PropertyHint.Layers3DNavigation, _navigation3dPresets },
                { PropertyHint.LayersAvoidance, _avoidance },
            };
        }
        var presets = _presetTypeBindings[propertyHint];

        if (ProjectSettings.HasSetting(GetLayersSetting(propertyHint)) && presets.Count == 0)
        {
            presets = LoadPresets(propertyHint);
        }

        return presets;
    }

    public static string GetPresetMetaName(GodotObject @object, string property, PropertyHint propertyHint)
    {
        return $"{@object.GetType().Name}_{propertyHint}_{property}";
    }

    public static void SaveEditedPreset(PresetData preset, PropertyHint propertyHint)
    {
        Variant savedVariant = ProjectSettings.GetSetting(GetLayersSetting(propertyHint));
        var allDictionaryPresets = savedVariant.As<Dictionary<string, Dictionary>>();
        allDictionaryPresets[preset.Id] = preset.ToDictionary();
        ProjectSettings.SetSetting(GetLayersSetting(propertyHint), allDictionaryPresets);
        Save(propertyHint);
        OnPresetEdited?.Invoke(preset);
    }

    public static void AddPreset(PresetData preset)
    {
        var allPresets = GetAllPresets(preset.LayerType);
        allPresets.Add(preset.Id, preset);
        SaveAddedPreset(preset, preset.LayerType);
    }

    public static void DeletePreset(string id, PropertyHint propertyHint)
    {
        var allPresets = GetAllPresets(propertyHint);
        allPresets.Remove(id);
        DeleteSavedPreset(id, propertyHint);
        OnPresetDeleted?.Invoke(id);
    }

    public static void DeleteAll(PropertyHint propertyHint)
    {
        var allPresets = GetAllPresets(propertyHint);
        allPresets.Clear();
        if (ProjectSettings.HasSetting(GetLayersSetting(propertyHint)))
        {
            ProjectSettings.SetSetting(GetLayersSetting(propertyHint), new Dictionary<string, Dictionary>());
            Save(propertyHint);
        }
        OnAllPresetsDeleted?.Invoke(propertyHint);
    }

    private static void DeleteSavedPreset(string id, PropertyHint propertyHint)
    {
        Variant savedVariant = ProjectSettings.GetSetting(GetLayersSetting(propertyHint));
        var dictionary = savedVariant.As<Dictionary<string, Dictionary>>();
        dictionary.Remove(id);
        ProjectSettings.SetSetting(GetLayersSetting(propertyHint), dictionary);
        Save(propertyHint);
    }

    private static void SaveAddedPreset(PresetData preset, PropertyHint propertyHint)
    {
        if (ProjectSettings.HasSetting(GetLayersSetting(propertyHint)))
        {
            Variant savedVariant = ProjectSettings.GetSetting(GetLayersSetting(propertyHint));
            var allPresets = savedVariant.As<Dictionary<string, Dictionary>>();
            var dictionaryPreset = preset.ToDictionary();
            allPresets.Add(preset.Id, dictionaryPreset);
            ProjectSettings.SetSetting(GetLayersSetting(propertyHint), allPresets);
        }
        else
        {
            var allPresets = new Dictionary<string, Dictionary>();
            var presetDictionary = preset.ToDictionary();
            allPresets.Add(preset.Id, presetDictionary);
            ProjectSettings.SetSetting(GetLayersSetting(propertyHint), allPresets);
        }

        Save(propertyHint);
    }

    private static Dictionary<string, PresetData> LoadPresets(PropertyHint propertyHint)
    {
        Variant savedVariant = ProjectSettings.GetSetting(GetLayersSetting(propertyHint));
        var dictionaryPresets = savedVariant.As<Dictionary<string, Dictionary>>();
        Dictionary<string, PresetData> presets = [];
        foreach (var presetDictionary in dictionaryPresets.Values)
        {
            var preset = new PresetData(presetDictionary);
            presets.Add(preset.Id, preset);
        }
        return presets;
    }

    private static void Save(PropertyHint propertyHint)
    {
        ProjectSettings.SetAsInternal(GetLayersSetting(propertyHint), true);
        ProjectSettings.Save();
    }

    private static string GetLayersSetting(PropertyHint propertyHint) => SettingsConstants.GetLayersSetting(propertyHint);
}
