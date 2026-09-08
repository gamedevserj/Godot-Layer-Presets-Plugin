using Godot;

namespace LayerPresets;
public partial class ExpandLayerSectionsButton : ExtendedButton
{
    public ExpandLayerSectionsButton()
    {
        TooltipText = "Show all layers";
        // https://godotengine.github.io/editor-icons/
        Icon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon("Stretch", "EditorIcons");
    }
}
