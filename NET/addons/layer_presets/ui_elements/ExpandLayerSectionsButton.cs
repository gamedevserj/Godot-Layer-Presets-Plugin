using Godot;
using System;

namespace LayerPresets;
public partial class ExpandLayerSectionsButton : Button
{
    public Action OnButtonPressed { get; set; }
    public ExpandLayerSectionsButton() 
    {
        TooltipText = "Show all layers"; 
        // https://godotengine.github.io/editor-icons/
        Icon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon("Stretch", "EditorIcons");
        // doing it this way prevents editor from throwing the following error when rebuilding
        // ERROR: Attempt to disconnect a nonexistent connection... Signal: 'pressed'
        // the error does not affect the functionality, but I just don't want to see it
        Pressed += () => { OnButtonPressed?.Invoke(); };
    }
}
