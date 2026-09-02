using Godot;
using Utilities;
using static Godot.Control;

namespace PhysicsLayerPresets;
public partial class SettingsWindow : Window
{
    public SettingsWindow() { }

    public SettingsWindow(string title, PresetData[] presets)
    {
        Title = title;
        Size = SettingsConstants.SettingsWindowSize;
        var backgroundPanel = CreateBackgroundPanel();
        AddChild(backgroundPanel);

        var scrollContainer = CreateScrollContainer();
        AddChild(scrollContainer);

        var marginContainer = CreateMarginContainer();
        scrollContainer.AddChild(marginContainer);

        var verticalContainer = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        verticalContainer.AddThemeConstantOverride("separation", 30);

        for (int i = 0; i < presets.Length; i++)
        {
            verticalContainer.AddChild(CreatePresetContainer(presets[i]));
        }

        marginContainer.AddChild(verticalContainer);

        FocusExited += Close;
        CloseRequested += Close;
    }

    public void Close() => QueueFree();

    private static VBoxContainer CreatePresetContainer(PresetData preset)
    {
        var container = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ShrinkCenter,
            SizeFlagsVertical = SizeFlags.ShrinkBegin,
        };
        var presetNameInput = new LineEdit { Text = preset.Name };
        presetNameInput.TextChanged += (text) => { 
            preset.Name = text;
            PresetsController.EditPreset(preset);
        };

        container.AddChild(presetNameInput);
        container.AddChild(new LayerPicker(preset.Layer, (layer) => {
            preset.Layer = layer;
            PresetsController.EditPreset(preset);
        }));
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
