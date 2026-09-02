using Godot;

namespace PhysicsLayerPresets;
public partial class SettingsWindowController : EditorInspectorPlugin
{
    private SettingsWindow _window;

    public SettingsWindowController()
    {
        PhysicsLayerPresetsInspector.OnSettingsButtonClicked += OpenSettingsWindow;
    }

    ~SettingsWindowController()
    {
        PhysicsLayerPresetsInspector.OnSettingsButtonClicked -= OpenSettingsWindow;
    }

    public void OpenSettingsWindow()
    {
        var presets = PresetsController.GetPresets();
        _window = new SettingsWindow("Layer Presets Settings", presets);
        EditorInterface.Singleton.GetBaseControl().AddChild(_window); 
        _window.PopupCentered();
    }
}
