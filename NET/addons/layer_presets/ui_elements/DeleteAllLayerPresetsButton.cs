using Godot;

namespace LayerPresets;
public partial class DeleteAllLayerPresetsButton : Button
{
    private const string DeleteButtonNormalColor = "b01c2a";
    private const string DeleteButtonHoverColor = "c82333";
    private const string DeleteButtonPressedColor = "bd2130";

    public DeleteAllLayerPresetsButton() { }

    public DeleteAllLayerPresetsButton(PropertyHint propertyHint) 
    {
        Text = $"Delete all '{SettingsConstants.GetFormattedPropertyHintName(propertyHint)}' presets".ToUpper();
        var normalStyle = new StyleBoxFlat();
        normalStyle.CornerRadiusBottomLeft =
            normalStyle.CornerRadiusBottomRight =
            normalStyle.CornerRadiusTopLeft =
            normalStyle.CornerRadiusTopRight = 5;
        normalStyle.BgColor = new Color(DeleteButtonNormalColor);

        var hoverStyle = (StyleBoxFlat)normalStyle.Duplicate();
        hoverStyle.BgColor = new Color(DeleteButtonHoverColor);

        var pressedStyle = (StyleBoxFlat)normalStyle.Duplicate();
        pressedStyle.BgColor = new Color(DeleteButtonPressedColor);

        AddThemeStyleboxOverride("normal", normalStyle);
        AddThemeStyleboxOverride("hover", hoverStyle);
        AddThemeStyleboxOverride("pressed", pressedStyle);
    }
}
