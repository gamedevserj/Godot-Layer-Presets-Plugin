#if TOOLS

using Godot;
using Godot.Collections;
using System;

namespace PhysicsLayerPresets;
public partial class PhysicsLayerPresetsInspector : EditorInspectorPlugin
{
    public static Action OnSettingsButtonClicked { get; set; }

    public override bool _CanHandle(GodotObject @object)
    {
        return FindPhysicsLayerProperty(@object) != null;
    }

    private static void OnSettingsButtonPressed() => OnSettingsButtonClicked?.Invoke();

    public override bool _ParseProperty(GodotObject @object, Variant.Type type, string name, PropertyHint hintType, string hintString, PropertyUsageFlags usageFlags, bool wide)
    {
        bool isPhysics3DLayer = (type == Variant.Type.Int && hintType == PropertyHint.Layers3DPhysics);
        if (isPhysics3DLayer)
        {
            var presets = PresetsController.GetPresets();
            var container = new HBoxContainer();
            var createPresetButton = new Button
            {
                Text = "Create preset from current",
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };
            // https://godotengine.github.io/editor-icons/
            var icon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon("GDScript", "EditorIcons");
            var settingsButton = new Button
            {
                Icon = icon,
                CustomMaximumSize = new Vector2(50, 50),
            };
            settingsButton.Pressed += OnSettingsButtonPressed;

            createPresetButton.Pressed += () =>
            {
                var currentLayer = (uint)@object.Get(name);
                var editorWindow = EditorInterface.Singleton.GetBaseControl();

                var dialog = new CreateLayerDialog(@object, presets, currentLayer);
                editorWindow.AddChild(dialog);
                dialog.ShowDialog();
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
            container.AddChild(settingsButton);
            AddCustomControl(container);

            // returning fales makes Godot render the rest as is
            return false;
        }
        return false;
    }    

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