using Godot;

namespace PhysicsLayerPresets;
internal static class SettingsConstants
{
    public static readonly Vector2I SettingsWindowSize = DisplayServer.ScreenGetSize() / 2;
    public const string PhysicsLayers3DPresetsSetting = "LayerPresets/PhysicsLayers/PhysicsLayer3DPresets";    
}
