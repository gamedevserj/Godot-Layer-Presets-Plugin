#if TOOLS
using Godot;

namespace LayerPresets;
[Tool]
public partial class LayerPresetsInspectorPlugin : EditorPlugin
{
    private LayerPresetsInspector _inspector;

    public override void _EnterTree()
    {
        _inspector = new LayerPresetsInspector();
        AddInspectorPlugin(_inspector);
    }

    public override void _ExitTree()
    {
        if (_inspector != null)
        {
            RemoveInspectorPlugin(_inspector);
            _inspector = null;
        }
    }
}
#endif