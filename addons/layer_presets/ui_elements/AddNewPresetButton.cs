using Godot;

namespace LayerPresets;
public partial class AddNewPresetButton : Button
{
    public AddNewPresetButton() { }

    public AddNewPresetButton(GodotObject @object, string propertyName, PropertyHint propertyHint)
    {
        TooltipText = "Create new preset";
        // https://godotengine.github.io/editor-icons/
        Icon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon("Add", "EditorIcons");

        Pressed += () => OpenAddNewPresetDialog(@object, propertyName, propertyHint);
    }

    private static void OpenAddNewPresetDialog(GodotObject @object, string propertyName, PropertyHint propertyHint)
    {
        var currentLayer = (uint)@object.Get(propertyName);
        var editorWindow = EditorInterface.Singleton.GetBaseControl();

        var dialog = new CreateNewPresetDialog(@object, propertyName, currentLayer, propertyHint);
        editorWindow.AddChild(dialog);
        dialog.ShowDialog();
    }
}
