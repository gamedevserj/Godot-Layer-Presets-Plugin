using Godot;
using System;
using System.Collections.Generic;

namespace LayerPresets;
public partial class PresetsDropdown : OptionButton
{
    private readonly PropertyHint _propertyHint;

    public PresetsDropdown() { }

    public PresetsDropdown(GodotObject @object, string propertyName, PropertyHint propertyHint, Action<string> onPresetSelected)
    {
        _propertyHint = propertyHint;
        SizeFlagsHorizontal = SizeFlags.ExpandFill;
        ClipText = true;

        UpdateVisual((uint)@object.Get(propertyName));
        var presets = GetPresets();
        var indexesToIdsBind = new Dictionary<int, string>();
        for (int i = 0; i < presets.Length; i++)
        {
            indexesToIdsBind.Add(i, presets[i].Id);
        }        

        ItemSelected += (index) => { onPresetSelected?.Invoke(indexesToIdsBind[(int)index]); };
    }

    public override void _EnterTree() => PresetData.OnPresetNameUpdated += OnPresetNameUpdated;

    public override void _ExitTree() => PresetData.OnPresetNameUpdated -= OnPresetNameUpdated;

    public void UpdateVisual(uint currentLayer)
    {
        var presets = GetPresets();
        Clear();
        // dropdown selects by indexes, so I store them
        // not using dictionary because layers can be non-unique if user edits one in settings
        var layersToIndexesBind = new List<(uint layer, int index)>();
        for (int i = 0; i < presets.Length; i++)
        {
            AddItem(presets[i].Name);
            layersToIndexesBind.Add((presets[i].Layer, i));
        }

        var match = -1;
        for (int i = 0; i < layersToIndexesBind.Count; i++)
        {
            if (layersToIndexesBind[i].layer == currentLayer)
            {
                match = layersToIndexesBind[i].index;
                break;
            }
        }

        Select(match);
        if (match == -1)
        {
            Text = SettingsConstants.NoPresetText;
        }
    }

    private uint GetSelectedLayer(long index)
    {
        var presets = GetPresets();
        var preset = presets[index];
        return preset.Layer;
    }

    private PresetData[] GetPresets()
    {
        var presetsDictionary = PresetsController.GetAllPresets(_propertyHint);
        var presets = new PresetData[presetsDictionary.Count];
        int index = 0;
        foreach (var preset in presetsDictionary)
        {
            presets[index] = preset.Value;
            index++;
        }
        return presets;
    }

    private void OnPresetEdited(PresetData preset)
    {
        var presets = GetPresets();
        for (int i = 0; i < presets.Length; i++)
        {
            if (presets[i].Id == preset.Id)
            {
                SetItemText(i, preset.Name);
                return;
            }
        }
    }

    private void OnPresetNameUpdated(string id, string newName)
    {
        var presets = GetPresets();
        for (int i = 0; i < presets.Length; i++)
        {
            if (presets[i].Id == id)
            {
                SetItemText(i, newName);
                return;
            }
        }
    }
}
