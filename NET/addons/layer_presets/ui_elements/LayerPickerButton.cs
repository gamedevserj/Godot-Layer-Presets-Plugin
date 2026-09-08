using Godot;
using System;

namespace LayerPresets;
public partial class LayerPickerButton : Button
{
    private const string NormalColor = "263548";
    private const string HoverColor = "34496a";
    private const string HoverPressedColor = "477dc6";
    private const string PressedColor = "386ca6";
    private const int FontSize = 16;

    public LayerPickerButton() { }

    public LayerPickerButton(int bit, string tooltipText, Vector2 size)
    {
        Bit = bit;
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

        TooltipText = tooltipText;
        ToggleMode = true;
        SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
        SizeFlagsVertical = SizeFlags.ShrinkCenter;
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

        // doing it this way prevents editor from throwing error when rebuilding (hot reload issue)
        // similar to ButtonBase Pressed
        Toggled += (toggledOn) => { OnButtonToggled?.Invoke(Bit); };
    }

    public Action<int> OnButtonToggled { get; set; }

    public int Bit { get; private set; }
}
