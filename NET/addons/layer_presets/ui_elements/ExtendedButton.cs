using Godot;
using System;

namespace LayerPresets;
// a helper class for buttons that cause errors when subscribing to built-in Pressed delegate
public partial class ExtendedButton : Button
{
    public ExtendedButton()
    {
        Pressed += () => { OnButtonPressed?.Invoke(); };
    }

    // custom action to subscribe to in order to avoid non-critical errors during recompilation
    // (hot reload issues with closures)
    public Action OnButtonPressed { get; set; }
}
