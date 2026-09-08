#if TOOLS
using Godot;

namespace LayerPresets;
[Tool]
public partial class LayerPresetsInspectorPlugin : EditorPlugin
{
    private LayerPresetsInspector _inspector;
    private Control _control;

    public override void _EnterTree()
    {
        _inspector = new LayerPresetsInspector();
        AddInspectorPlugin(_inspector);

        _control = new SettingsViewControl(PropertyHint.Layers2DPhysics);
        _control.Name = "Layer Presets";
        AddControlToContainer(CustomControlContainer.ProjectSettingTabRight, _control);
    }

    public override void _ExitTree()
    {
        if (_inspector != null)
        {
            RemoveInspectorPlugin(_inspector);
            _inspector = null;
        }

        if (_control != null)
        {
            _control.QueueFree();
        }
    }
}
#endif