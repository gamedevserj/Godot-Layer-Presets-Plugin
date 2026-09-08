using Godot;

namespace LayerPresets;
public partial class SettingsViewControl : Control
{
    private const int Margin = 20;
    private const int TabContentMargin = 10;

    public SettingsViewControl() { }

    public SettingsViewControl(PropertyHint propertyHint) 
    {
        SetAnchorsPreset(LayoutPreset.FullRect);
        var backgroundPanel = CreateBackgroundPanel();
        AddChild(backgroundPanel);

        var mainContainer = CreateVBoxContainer();
        AddChild(mainContainer);

        var marginContainer = CreateMarginContainer(Margin);
        var tabContainer = new TabContainer();
        tabContainer.TabAlignment = TabBar.AlignmentMode.Center;
        var panelStylebox = new StyleBoxFlat
        {
            BgColor = new Color(0, 0, 0, 0),
            ContentMarginLeft = TabContentMargin,
            ContentMarginRight = TabContentMargin,
            ContentMarginTop = TabContentMargin,
            BorderWidthLeft = TabContentMargin / 2,
            BorderWidthRight = TabContentMargin / 2,
            BorderWidthBottom = TabContentMargin / 2,
            BorderWidthTop = TabContentMargin / 2,
            BorderColor = EditorInterface.Singleton.GetBaseControl().GetThemeColor("normal", "Editor"),
        };

        tabContainer.AddThemeStyleboxOverride("panel", panelStylebox);
        int tabIndex = 0;
        foreach (var layerType in SettingsConstants.HandledProperties)
        {
            tabContainer.AddChild(new SettingsTab(layerType));
            tabContainer.SetTabTitle(tabIndex, SettingsConstants.GetFormattedPropertyHintName(layerType));
            if (layerType == propertyHint)
            {
                tabContainer.CurrentTab = tabIndex;
            }
            tabIndex++;
        }
        
        marginContainer.AddChild(tabContainer);
        mainContainer.AddChild(marginContainer);
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

    private static MarginContainer CreateMarginContainer(int margin)
    {
        var marginContainer = new MarginContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };
        marginContainer.SetAnchorsPreset(LayoutPreset.FullRect);
        marginContainer.AddThemeConstantOverride("margin_left", margin);
        marginContainer.AddThemeConstantOverride("margin_right", margin);
        marginContainer.AddThemeConstantOverride("margin_top", margin);
        marginContainer.AddThemeConstantOverride("margin_bottom", margin);

        return marginContainer;
    }

    private static VBoxContainer CreateVBoxContainer(int separation = 4)
    {
        var container = new VBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };
        container.SetAnchorsPreset(LayoutPreset.FullRect);
        container.AddThemeConstantOverride("separation", separation);

        return container;
    }
}
