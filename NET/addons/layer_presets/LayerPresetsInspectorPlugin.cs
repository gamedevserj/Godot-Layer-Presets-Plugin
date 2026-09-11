#if TOOLS
using Godot;

namespace LayerPresets;
[Tool]
public partial class LayerPresetsInspectorPlugin : EditorPlugin
{
    private LayerPresetsInspector _inspector;
    private Button _settingsButtonScreen2d;
    private Button _settingsButtonScreen3d;

    public override void _EnterTree()
    {
        _inspector = new LayerPresetsInspector();
        AddInspectorPlugin(_inspector);

        _settingsButtonScreen2d = new OpenSettingsButton(PropertyHint.Layers2DPhysics);
        _settingsButtonScreen3d = new OpenSettingsButton(PropertyHint.Layers3DPhysics);
        AddControlToContainer(CustomControlContainer.CanvasEditorMenu, _settingsButtonScreen2d);
        AddControlToContainer(CustomControlContainer.SpatialEditorMenu, _settingsButtonScreen3d);
    }

    public override void _ExitTree()
    {
        if (_inspector != null)
        {
            RemoveInspectorPlugin(_inspector);
            _inspector = null;
        }

        if (_settingsButtonScreen2d != null)
        {
            _settingsButtonScreen2d.QueueFree();
        }

        if (_settingsButtonScreen3d != null)
        {
            _settingsButtonScreen3d.QueueFree();
        }
    }
}
#endif