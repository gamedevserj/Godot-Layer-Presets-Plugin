#if TOOLS

using Godot;

namespace PhysicsLayerPresets;
[Tool]
public partial class PhysicsLayerPresetsInspectorPlugin : EditorPlugin
{
    private PhysicsLayerPresetsInspector _inspector;
    private SettingsWindowController _settingsWindowController;

    public override void _EnterTree()
    {
        CreateColorSettings();
        _inspector = new PhysicsLayerPresetsInspector();
        _settingsWindowController = new SettingsWindowController();
        AddInspectorPlugin(_settingsWindowController);
        AddInspectorPlugin(_inspector);
    }

    public override void _ExitTree()
    {
        if (_inspector != null)
        {
            RemoveInspectorPlugin(_inspector);
            _inspector = null;
        }

        if (_settingsWindowController != null)
        {
            _settingsWindowController.CloseSettingsWindow();
            RemoveInspectorPlugin(_settingsWindowController);
            _settingsWindowController = null;
        }
    }

    private void CreateColorSettings()
    {

    }
}
#endif