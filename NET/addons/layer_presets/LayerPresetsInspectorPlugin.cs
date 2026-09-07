#if TOOLS
using Godot;

namespace LayerPresets;
[Tool]
public partial class LayerPresetsInspectorPlugin : EditorPlugin
{
    private LayerPresetsInspector _inspector;
    private Button _button;

    public override void _EnterTree()
    {
        _inspector = new LayerPresetsInspector();
        AddInspectorPlugin(_inspector);
        _button = new Button
        {
            Text = "Button",
            SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
            SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin
        };

        AddControlToContainer(CustomControlContainer.ProjectSettingTabRight, _button);
    }

    public override void _ExitTree()
    {
        if (_inspector != null)
        {
            RemoveInspectorPlugin(_inspector);
            _inspector = null;
        }

        if (_button != null)
        {
            _button.QueueFree();
        }
    }
}
#endif