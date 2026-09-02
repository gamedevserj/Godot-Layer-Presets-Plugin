using Godot;
using System;

namespace PhysicsLayerPresets;

public partial class LayerPicker : GridContainer
{
    private readonly Vector2 ButtonSize = new(32, 32);

    public LayerPicker(uint layer, Action<uint> onLayerChanged)
    {
        Columns = 4;
        SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
        AddThemeConstantOverride("h_separation", 20);
        AddThemeConstantOverride("v_separation", 20);
        var container = CreateInnerGridContainer();

        for (int i = 0; i < 32; i++)
        {
            int bit = i;
            var name = (string)ProjectSettings.GetSetting($"layer_names/3d_physics/layer_{bit + 1}", $"Layer {bit + 1}");
            var button = new LayerPickerButton(bit, name, layer, ButtonSize);

            button.LayerChanged += onLayerChanged;
            container.AddChild(button);
            if (i > 0 && (i + 1) % 8 == 0 && i < 31)
            {
                container = CreateInnerGridContainer();
            }
        }
    }

    private GridContainer CreateInnerGridContainer()
    {
        var container = new GridContainer
        {
            Columns = 4,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };

        AddChild(container);
        return container;
    }
}