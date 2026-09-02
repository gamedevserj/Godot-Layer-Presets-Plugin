using Godot;

namespace PhysicsLayerPresets;
public partial class PresetsDropdown : OptionButton
{
    private PresetData[] _presets;

    public PresetsDropdown() { }

    public PresetsDropdown(GodotObject @object, string name, PresetData[] presets)
    {
        SizeFlagsHorizontal = SizeFlags.ExpandFill;
        _presets = presets;

        // dropdown selects by indexes, so I store them
        var masks = new System.Collections.Generic.List<(uint mask, int index)>();
        for (int i = 0; i < _presets.Length; i++)
        {
            AddItem(_presets[i].Name);
            masks.Add((_presets[i].Layer, i));
        }

        // selecting initial value
        uint currentMask = (uint)@object.Get(name);
        var match = -1;
        for (int i = 0; i < masks.Count; i++)
        {
            if (masks[i].mask == currentMask)
            {
                match = masks[i].index;
                break;
            }
        }
        Select(match);
        if (match == -1)
        {
            Text = "No preset";
        }

        ItemSelected += (long index) =>
        {
            var preset = _presets[index];

            @object.Set(name, preset.Layer);
            @object.NotifyPropertyListChanged();
        };
    }

    public override void _EnterTree()
    {
        PresetsController.OnPresetEdited += OnPresetEdited;
    }

    public override void _ExitTree()
    {
        PresetsController.OnPresetEdited -= OnPresetEdited;
    }

    private void OnPresetEdited(PresetData preset)
    {
        for (int i = 0; i < _presets.Length; i++)
        {
            if (_presets[i].Id == preset.Id)
            {
                SetItemText(i, preset.Name);
                return;
            }
        }
    }
}
