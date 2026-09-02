using Godot;
using System;

namespace PhysicsLayerPresets;
public partial class LayerPickerButton : Button
{
    private const string NormalColor = "263548";
    private const string HoverColor = "34496a";
    private const string HoverPressedColor = "477dc6";
    private const string PressedColor = "386ca6";
    private const int FontSize = 16;
    
    public event Action<uint> LayerChanged;

    public LayerPickerButton() { }

    public LayerPickerButton(int bit, string name, uint layer, Vector2 size)
    {
        var label = new Label
        {
            Text = (bit + 1).ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        var labelStylebox = new StyleBoxEmpty();
        label.AddThemeStyleboxOverride("normal", labelStylebox);
        label.SetAnchorsPreset(LayoutPreset.FullRect);
        label.AddThemeFontSizeOverride("font_size", FontSize);
        AddChild(label);

        TooltipText = name;
        ToggleMode = true;
        ButtonPressed = (layer & (1u << bit)) != 0;
        SizeFlagsHorizontal = SizeFlags.ExpandFill;
        SizeFlagsVertical = SizeFlags.ExpandFill;
        CustomMinimumSize = size;

        var normalStyle = new StyleBoxFlat();
        normalStyle.BgColor = new Color(NormalColor);

        var hoverStyle = new StyleBoxFlat();
        hoverStyle.BgColor = new Color(HoverColor);

        var hoverPressedStyle = new StyleBoxFlat();
        hoverPressedStyle.BgColor = new Color(HoverPressedColor);

        var pressedStyle = new StyleBoxFlat();
        pressedStyle.BgColor = new Color(PressedColor);

        AddThemeStyleboxOverride("normal", normalStyle);
        AddThemeStyleboxOverride("hover", hoverStyle);
        AddThemeStyleboxOverride("hover_pressed", hoverPressedStyle);
        AddThemeStyleboxOverride("pressed", pressedStyle);

        Toggled += pressed =>
        {
            layer = pressed ? layer | (1u << bit) : layer & ~(1u << bit);
            LayerChanged?.Invoke(layer);
        };
    }
}
