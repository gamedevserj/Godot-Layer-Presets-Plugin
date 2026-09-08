using Godot;
using System;

namespace LayerPresets;
// a helper class for line edits that cause errors when subscribing to built-in TextChanged delegate
public partial class ExtendedLineEdit : LineEdit
{
    public ExtendedLineEdit()
    {
        TextChanged += (text) => { OnTextChanged?.Invoke(text); };
    }

    // custom action to subscribe to in order to avoid non-critical errors during recompilation
    // (hot reload issues with closures)
    public Action<string> OnTextChanged { get; set; }
}
