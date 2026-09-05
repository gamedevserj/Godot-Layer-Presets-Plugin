using Godot;
using System.Collections.Generic;

namespace LayerPresets;
internal static class SettingsConstants
{
    public static readonly Vector2I SettingsWindowSize = DisplayServer.ScreenGetSize() / 2;
    public static readonly HashSet<PropertyHint> HandledProperties =
        [
            PropertyHint.Layers2DPhysics, PropertyHint.Layers3DPhysics,
            PropertyHint.Layers2DRender, PropertyHint.Layers3DRender,
            PropertyHint.Layers2DNavigation, PropertyHint.Layers3DNavigation,
            PropertyHint.LayersAvoidance
        ];

    public static readonly Vector2 ButtonSize = new(32, 32);
    public const int LayerPickerSectionSeparation = 10;
    public const int LayerPickerButtonSeparation = 4;
    public const int Margin = 10;

    public static string GetLayersSetting(PropertyHint type) => $"LayerPresets/{type}";

    public static string GetFormattedPropertyName(PropertyHint propertyHint)
    {
        return propertyHint switch
        {
            PropertyHint.Layers2DRender => "2D Render",
            PropertyHint.Layers2DPhysics => "2D Physics",
            PropertyHint.Layers2DNavigation => "2D Navigation",
            PropertyHint.Layers3DRender => "3D Render",
            PropertyHint.Layers3DPhysics => "3D Physics",
            PropertyHint.Layers3DNavigation => "3D Navigation",
            PropertyHint.LayersAvoidance => "Avoidance",
            _ => "",
        };
    }
}
