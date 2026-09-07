using Godot;

namespace LayerPresets;
public partial class DeletePresetButton : Button
{
    public DeletePresetButton() 
    {
        TooltipText = "Delete preset";
        // https://godotengine.github.io/editor-icons/
        Icon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon("KeyInvalid", "EditorIcons");
        SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
        SizeFlagsVertical = SizeFlags.ShrinkCenter;
    }
}
