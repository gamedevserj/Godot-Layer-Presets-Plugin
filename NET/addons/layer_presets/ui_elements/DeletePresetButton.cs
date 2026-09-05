using Godot;

namespace LayerPresets;
public partial class DeletePresetButton : Button
{
    public DeletePresetButton(string id, PropertyHint propertyHint) 
    {
        TooltipText = "Delete preset";
        // https://godotengine.github.io/editor-icons/
        Icon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon("KeyInvalid", "EditorIcons");
        Pressed += () => { PresetsController.DeletePreset(id, propertyHint); };
    }
}
