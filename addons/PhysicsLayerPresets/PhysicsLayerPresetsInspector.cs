#if TOOLS

using Godot;
using Godot.Collections;

namespace PhysicsLayerPresets;
public partial class PhysicsLayerPresetsInspector : EditorInspectorPlugin
{
    private enum NameValidityStatus { Valid, Empty, Duplicate };

    public override bool _CanHandle(GodotObject @object)
    {
        return FindPhysicsLayerProperty(@object) != null;
    }

    public override bool _ParseProperty(GodotObject @object, Variant.Type type, string name, PropertyHint hintType, string hintString, PropertyUsageFlags usageFlags, bool wide)
    {
        bool isPhysics3DLayer = (type == Variant.Type.Int && hintType == PropertyHint.Layers3DPhysics);
        if (isPhysics3DLayer)
        {
            var presets = PhysicsLayerPresetsInspectorPlugin.GetPresets();
            var container = new HBoxContainer();
            var createPresetButton = new Button
            {
                Text = "Create preset from current",
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };

            createPresetButton.Pressed += () =>
            {
                var currentLayer = (uint)@object.Get(name);
                var editorWindow = EditorInterface.Singleton.GetBaseControl();

                var dialog = new ConfirmationDialog
                {
                    Title = "Adding new preset",
                };
                var okButton = dialog.GetOkButton();
                var verticalContainer = new VBoxContainer
                {
                    SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                    SizeFlagsVertical = Control.SizeFlags.ExpandFill
                };
                var nameIsValidLabel = new Label { Text = "Enter preset name" };
                var inputField = new LineEdit { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };

                verticalContainer.AddChild(inputField);
                verticalContainer.AddChild(nameIsValidLabel);
                dialog.AddChild(verticalContainer);
                editorWindow.AddChild(dialog);

                inputField.TextChanged += (string newText) =>
                {
                    var status = IsNameValid(newText, presets);
                    nameIsValidLabel.Text = GetStatusMessage(status);
                    okButton.Disabled = status != NameValidityStatus.Valid;
                };

                dialog.Confirmed += () =>
                {
                    var name = inputField.Text;
                    PhysicsLayerPresetsInspectorPlugin.AddPreset(name, currentLayer);
                    dialog.QueueFree();
                    @object.NotifyPropertyListChanged();
                };

                okButton.Disabled = true;
                dialog.Canceled += () => dialog.QueueFree();
                dialog.CloseRequested += () => dialog.QueueFree();
                
                dialog.PopupCentered();
                inputField.GrabFocus();
            };

            if (presets.Count > 0)
            {
                var dropdown = new OptionButton
                {
                    SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
                };
                container.AddChild(dropdown);
                
                int index = 0;
                // dropdown selects by indexes, so I store them
                var masks = new System.Collections.Generic.List<(uint mask, int index)>();
                foreach (var kvp in presets)
                {
                    AddItem(kvp);
                }

                void AddItem(System.Collections.Generic.KeyValuePair<string, uint> kvp)
                {
                    dropdown.AddItem(kvp.Key);
                    masks.Add((kvp.Value, index));
                    index++;
                }

                // selecting initial value
                uint currentMask = (uint)@object.Get(name);
                var match = -1;
                for (int i = 0; i < masks.Count; i++)
                {
                    if (masks[i].mask == currentMask)
                    {
                        match = masks[i].index;
                        break;
                    }
                }
                dropdown.Select(match);
                if (match == -1)
                {
                    dropdown.Text = "No preset";
                }

                dropdown.ItemSelected += (long index) =>
                {
                    var presetName = dropdown.GetItemText((int)index);
                    var preset = presets[presetName];

                    @object.Set(name, preset);
                    @object.NotifyPropertyListChanged();
                };
            }

            container.AddChild(createPresetButton);
            AddCustomControl(container);

            // returning fales makes Godot render the rest as is
            return false;
        }
        return false;
    }

    private static NameValidityStatus IsNameValid(string newText, Dictionary<string, uint> presets)
    {
        if (string.IsNullOrWhiteSpace(newText))
            return NameValidityStatus.Empty;

        if (presets.ContainsKey(newText))
            return NameValidityStatus.Duplicate;

        return NameValidityStatus.Valid;
    }

    private static string GetStatusMessage(NameValidityStatus status) => 
        status switch
        {
            NameValidityStatus.Valid => "Name is valid",
            NameValidityStatus.Empty => "Name can not be empty",
            NameValidityStatus.Duplicate => "Preset with this name already exists",
            _ => "Name can not be empty",
        };
    

    private static string FindPhysicsLayerProperty(GodotObject @object)
    {
        if (@object == null) return null;

        foreach (Dictionary prop in @object.GetPropertyList())
        {
            long typeInt = (long)prop["type"];
            long hintInt = (long)prop["hint"];

            if ((Variant.Type)typeInt == Variant.Type.Int &&
                ((PropertyHint)hintInt == PropertyHint.Layers2DPhysics ||
                 (PropertyHint)hintInt == PropertyHint.Layers3DPhysics))
            {
                return (string)prop["name"];
            }
        }
        return null;
    }
}
#endif