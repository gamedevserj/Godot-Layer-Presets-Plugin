using Godot;
using System;

namespace PhysicsLayerPresets;

public partial class LayerPickerRow : GridContainer
{
    private const string NormalColor = "263548";
    private const string HoverColor = "34496a";
    private const string HoverPressedColor = "477dc6";
    private const string PressedColor = "386ca6";
    private const int FontSize = 20;
    private readonly Vector2 ButtonSize = new(50, 50);

    public uint Mask;
    public event Action<uint> MaskChanged;

    public LayerPickerRow(uint initialMask)
    {
        Columns = 4;
        SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
        AddThemeConstantOverride("h_separation", 20);
        AddThemeConstantOverride("v_separation", 20);
        Mask = initialMask;
        var container = CreateInnerGridContainer();

        for (int i = 0; i < 32; i++)
        {
            int bit = i;
            var name = (string)ProjectSettings.GetSetting($"layer_names/3d_physics/layer_{bit + 1}", $"Layer {bit + 1}");
            var button = new Button
            {
                Text = (bit + 1).ToString(),
                TooltipText = name,
                ToggleMode = true,
                ButtonPressed = (Mask & (1u << bit)) != 0,
                SizeFlagsHorizontal = SizeFlags.ExpandFill,
                SizeFlagsVertical = SizeFlags.ExpandFill,
                CustomMinimumSize = ButtonSize,
            };

            var normalStyle = new StyleBoxFlat();
            normalStyle.BgColor = new Color(NormalColor);

            var hoverStyle = new StyleBoxFlat();
            hoverStyle.BgColor = new Color(HoverColor);

            var hoverPressedStyle = new StyleBoxFlat();
            hoverPressedStyle.BgColor = new Color(HoverPressedColor);

            var pressedStyle = new StyleBoxFlat();
            pressedStyle.BgColor = new Color(PressedColor);

            button.AddThemeStyleboxOverride("normal", normalStyle);
            button.AddThemeStyleboxOverride("hover", hoverStyle);
            button.AddThemeStyleboxOverride("hover_pressed", hoverPressedStyle);
            button.AddThemeStyleboxOverride("pressed", pressedStyle);
            button.AddThemeFontSizeOverride("font_size", FontSize);

            button.Toggled += pressed =>
            {
                Mask = pressed ? Mask | (1u << bit) : Mask & ~(1u << bit);
                MaskChanged?.Invoke(Mask);
            };

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