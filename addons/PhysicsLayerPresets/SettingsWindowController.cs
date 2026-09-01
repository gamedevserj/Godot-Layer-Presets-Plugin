using Godot;
using static Godot.Control;

namespace PhysicsLayerPresets;
public partial class SettingsWindowController : EditorInspectorPlugin
{
    private Window _window; 

    public SettingsWindowController()
    {
        PhysicsLayerPresetsInspector.OnSettingsButtonClicked += OpenSettingsWindow;
    }

    ~SettingsWindowController()
    {
        PhysicsLayerPresetsInspector.OnSettingsButtonClicked -= OpenSettingsWindow;
    }

    public void OpenSettingsWindow()
    {
        _window = new Window
        {
            Title = "My Plugin Settings",
            Size = SettingsConstants.SettingsWindowSize,
        };

        var backgroundPanel = CreateBackgroundPanel();
        _window.AddChild(backgroundPanel);

        var scrollContainer = CreateScrollContainer();
        _window.AddChild(scrollContainer);

        var marginContainer = CreateMarginContainer();
        scrollContainer.AddChild(marginContainer);

        var verticalContainer = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        verticalContainer.AddThemeConstantOverride("separation", 30);

        var presets = PresetsController.GetPresets();
        foreach (var preset in presets)
        {
            //var presetName = new Label { Text = preset.Key };
            //var presetValue = new Label { Text = preset.Value.ToString() };
            
            //verticalContainer.AddChild(presetName);
            //verticalContainer.AddChild(presetValue);
            //verticalContainer.AddChild(new LayerPickerRow(preset.Value));
            verticalContainer.AddChild(CreatePresetContainer(preset));
        }

        marginContainer.AddChild(verticalContainer);

        _window.CloseRequested += CloseSettingsWindow;
        EditorInterface.Singleton.GetBaseControl().AddChild(_window); 
        _window.PopupCentered();
    }

    public void CloseSettingsWindow()
    {
        if (_window != null && IsInstanceValid(_window))
        {
            _window.QueueFree();
            _window = null;
        }
    }

    private static VBoxContainer CreatePresetContainer(System.Collections.Generic.KeyValuePair<string, uint> preset)
    {
        var container = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ShrinkCenter,
            SizeFlagsVertical = SizeFlags.ShrinkBegin,
        };
        var presetName = new LineEdit { Text = preset.Key };

        container.AddChild(presetName);
        container.AddChild(new LayerPickerRow(preset.Value));
        return container;
    }

    private static Panel CreateBackgroundPanel()
    {
        var backgroundPanel = new Panel();
        backgroundPanel.SetAnchorsPreset(LayoutPreset.FullRect);

        var styleBox = new StyleBoxFlat
        {
            BgColor = EditorInterface.Singleton.GetBaseControl().GetThemeColor("base_color", "Editor"),
        };
        backgroundPanel.AddThemeStyleboxOverride("panel", styleBox);

        return backgroundPanel;
    }

    private static ScrollContainer CreateScrollContainer()
    {
        var scrollContainer = new ScrollContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };
        scrollContainer.SetAnchorsPreset(LayoutPreset.FullRect);

        return scrollContainer;
    }

    private static MarginContainer CreateMarginContainer()
    {
        var marginContainer = new MarginContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        marginContainer.AddThemeConstantOverride("margin_left", 50);
        marginContainer.AddThemeConstantOverride("margin_right", 50);
        marginContainer.AddThemeConstantOverride("margin_top", 50);
        marginContainer.AddThemeConstantOverride("margin_bottom", 50);

        return marginContainer;
    }
}
