using Godot;
using System;

namespace LayerPresets;
public partial class ResetButton : Button
{
    public ResetButton() { }

    public ResetButton(Action onPressed)
    {
        TooltipText = "Reset to default";
        // https://godotengine.github.io/editor-icons/
        Icon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon("Reload", "EditorIcons");
        Pressed += onPressed;
    }
}
