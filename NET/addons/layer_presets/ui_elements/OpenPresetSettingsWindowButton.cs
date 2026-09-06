using Godot;

namespace LayerPresets;
public partial class OpenPresetSettingsWindowButton : Button
{
    public OpenPresetSettingsWindowButton() { }

    public OpenPresetSettingsWindowButton(PropertyHint propertyHint) 
    {
        TooltipText = "Open settings";
        // https://godotengine.github.io/editor-icons/
        Icon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon("GDScript", "EditorIcons");
        Pressed += () => OpenSettingsWindow(propertyHint);
    }

    private static void OpenSettingsWindow(PropertyHint propertyHint)
    {
        var window = new SettingsWindow(propertyHint);
        EditorInterface.Singleton.GetBaseControl().AddChild(window);
        window.PopupCentered();
    }
}
