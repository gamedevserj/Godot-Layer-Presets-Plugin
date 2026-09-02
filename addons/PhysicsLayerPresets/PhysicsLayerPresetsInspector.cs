#if TOOLS

using Godot;
using Godot.Collections;
using System;
using System.Xml.Linq;
using Utilities;

namespace PhysicsLayerPresets;
public partial class PhysicsLayerPresetsInspector : EditorInspectorPlugin
{
    public static Action OnSettingsButtonClicked { get; set; }

    public override bool _CanHandle(GodotObject @object)
    {
        return FindPhysicsLayerProperty(@object) != null;
    }

    private static void OnSettingsButtonPressed() => OnSettingsButtonClicked?.Invoke();

    //private void Test() => GD.Print("test");

    public override bool _ParseProperty(GodotObject @object, Variant.Type type, string name, PropertyHint hintType, string hintString, PropertyUsageFlags usageFlags, bool wide)
    {
        bool isPhysics3DLayer = (type == Variant.Type.Int && hintType == PropertyHint.Layers3DPhysics);
        if (isPhysics3DLayer)
        {
            Debug.Log($"name = {@object}");
            //var pickerRow2 = new LayerPickerRow2((uint)@object.Get(name));
            //pickerRow2.Visible = false;
            //AddCustomControl(pickerRow2);
            //return true;
            //GD.Print("type = " + @object.GetType());
            //GD.Print("prop connected = " + @object.HasConnections("property_list_changed"));
            //var connections = @object.GetSignalConnectionList("property_list_changed");

            //if (!@object.IsConnected("property_list_changed", Callable.From(Test)))
            //{
            //    @object.Connect("property_list_changed", Callable.From(Test));
            //}
            //foreach (var c in connections)
            //{
            //    foreach (var item in c)
            //    {
            //        GD.Print($"key = { item.Key}, value = {item.Value}");
            //    }
            //}
            var presets = PresetsController.GetPresets();
            var container = new HBoxContainer();
            var newPresetButton = new Button
            {
                Text = "New preset from current",
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };

            // https://godotengine.github.io/editor-icons/
            var icon = EditorInterface.Singleton.GetBaseControl().GetThemeIcon("GDScript", "EditorIcons");
            var settingsButton = new Button();
            settingsButton.Icon = icon;
            settingsButton.Pressed += OnSettingsButtonPressed;

            newPresetButton.Pressed += () =>
            {
                var currentLayer = (uint)@object.Get(name);
                var editorWindow = EditorInterface.Singleton.GetBaseControl();

                var dialog = new CreateLayerDialog(@object, presets, currentLayer);
                editorWindow.AddChild(dialog);
                dialog.ShowDialog();
            };

            if (presets.Length > 0)
            {
                var dropdown = new PresetsDropdown(@object, name, presets);
                container.AddChild(dropdown);
            }
            
            container.AddChild(newPresetButton);
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

            if ((Variant.Type)typeInt == Variant.Type.Int && (PropertyHint)hintInt == PropertyHint.Layers3DPhysics)
            {
                GD.Print($"prop[name] = {prop["name"]}");
                //GD.Print($"prop[name] = {(uint)@object.Get(name)}");
                return (string)prop["name"];
            }
        }
        return null;
    }
}
#endif