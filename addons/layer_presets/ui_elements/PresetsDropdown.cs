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
        SizeFlagsVertical = SizeFlags.ExpandFill;
        ClipText = true;

        var presets = GetPresets();
        var indexesToIdsBind = new Dictionary<int, string>();
        // dropdown selects by indexes, so I store them, can't use dictionary because layers can be non-unique
        var layersToIndexesBind = new List<(uint layer, int index)>();
        for (int i = 0; i < presets.Length; i++)
        {
            AddItem(presets[i].Name);
            layersToIndexesBind.Add((presets[i].Layer, i));
            indexesToIdsBind.Add(i, presets[i].Id);
        }

        // selecting initial value
        uint currentLayer = (uint)@object.Get(propertyName);
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
            Text = "No preset";
        }

        ItemSelected += (index) => { onPresetSelected?.Invoke(indexesToIdsBind[(int)index]); };
    }

    public override void _EnterTree() => PresetsController.OnPresetEdited += OnPresetEdited;

    public override void _ExitTree() => PresetsController.OnPresetEdited -= OnPresetEdited;

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
}
