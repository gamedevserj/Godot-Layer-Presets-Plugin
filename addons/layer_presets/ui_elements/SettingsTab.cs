using Godot;
using System.Collections.Generic;

namespace LayerPresets;
public partial class SettingsTab : VBoxContainer
{
    private readonly PropertyHint _propertyHint;
    private readonly Dictionary<string, HBoxContainer> _presetsBindings = [];
    private readonly VBoxContainer _container;
    private readonly Label _noPresetsLabel;

    public SettingsTab() { }

    public SettingsTab(PropertyHint propertyHint) 
    {
        _propertyHint = propertyHint;
        var layerName = SettingsConstants.GetFormattedPropertyName(propertyHint);
        var presets = PresetsController.GetAllPresets(propertyHint);
        _noPresetsLabel = new Label
        {
            Text = $"No {layerName} presets created.",
            HorizontalAlignment = HorizontalAlignment.Center
        };

        if (presets.Count == 0)
        {
            AddChild(_noPresetsLabel);
            return;
        }

        _container = new VBoxContainer();
        _container.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        _container.SizeFlagsVertical = SizeFlags.ExpandFill;

        var buttonsContainer = new VBoxContainer();
        buttonsContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;

        var scrollContainer = CreateScrollContainer();
        var presetsContainer = CreateVBoxContainer(0);

        foreach (var preset in presets)
        {
            var presetElement = CreatePresetElement(preset.Value);
            presetsContainer.AddChild(presetElement);
            _presetsBindings.Add(preset.Key, presetElement);
        }

        var deleteAllButton = new DeleteAllLayerPresetsButton(propertyHint);

        var restoreFromPresetsContainer = CreateHBoxContainer();
        var folderPath = new LineEdit
        {
            Text = "res://",
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        var restorePresetsFromMetaButton = new Button();
        restorePresetsFromMetaButton.Text = "Restore from presets";
        restorePresetsFromMetaButton.TooltipText = 
            $"Go over every scene in specified folder and restore layers on objects that have presets.\n" +
            $"Use it if you edited preset in this menu.";
        restorePresetsFromMetaButton.Pressed += () => MetaRestoreController.RestoreLayersFromMeta(propertyHint, folderPath.Text);
        
        restoreFromPresetsContainer.AddChild(restorePresetsFromMetaButton);
        restoreFromPresetsContainer.AddChild(folderPath);

        buttonsContainer.AddChild(deleteAllButton);
        buttonsContainer.AddChild(restoreFromPresetsContainer);

        scrollContainer.AddChild(presetsContainer);
        _container.AddChild(buttonsContainer);
        _container.AddChild(scrollContainer);
        AddChild(_container);
    }

    public override void _EnterTree()
    {
        PresetsController.OnPresetDeleted += OnPresetDeleted;
        PresetsController.OnAllPresetsDeleted += OnAllPresetsDeleted;
    }

    public override void _ExitTree()
    {
        PresetsController.OnPresetDeleted -= OnPresetDeleted;
        PresetsController.OnAllPresetsDeleted -= OnAllPresetsDeleted;
    }

    private void OnPresetDeleted(string id)
    {
        if (_presetsBindings.TryGetValue(id, out HBoxContainer value))
        {
            value.QueueFree();
            _presetsBindings.Remove(id);
        }
    }

    private void OnAllPresetsDeleted(PropertyHint propertyHint)
    {
        if (_propertyHint == propertyHint)
        {
            _container.QueueFree();
            _presetsBindings.Clear();
            AddChild(_noPresetsLabel);
        }
    }

    private HBoxContainer CreatePresetElement(PresetData preset)
    {
        var baseControl = EditorInterface.Singleton.GetBaseControl();
        var margin = SettingsConstants.Margin;

        var mainContainer = new HBoxContainer();
        mainContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        mainContainer.AddThemeConstantOverride("separation", margin);
        var presetAndDeleteButtonContainer = CreateHBoxContainer(margin);

        var panelContainer = new PanelContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };

        var cornerRadius = 10;
        var style = new StyleBoxFlat
        {
            BgColor = baseControl.GetThemeColor("normal", "Editor"),
            CornerRadiusBottomLeft = cornerRadius,
            CornerRadiusBottomRight = cornerRadius,
            CornerRadiusTopLeft = cornerRadius,
            CornerRadiusTopRight = cornerRadius
        };
        panelContainer.AddThemeStyleboxOverride("panel", style);

        var innerMargin = CreateMarginContainer(margin);
        var presetContainer = CreateVBoxContainer(margin);

        var presetNameInput = new LineEdit
        {
            Text = preset.Name,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        presetNameInput.TextChanged += preset.SetName;

        var expandLayerSectionsButton = new ExpandLayerSectionsButton();
        var presetNameContainer = new HBoxContainer();
        presetNameContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;

        var layerPicker = new LayerPicker(preset.Layer, expandLayerSectionsButton);
        layerPicker.OnLayerUpdatedManually += preset.SetLayer;

        var deleteButton = new DeletePresetButton(preset.Id, preset.LayerType);
        deleteButton.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
        deleteButton.SizeFlagsVertical = SizeFlags.ShrinkCenter;

        presetNameContainer.AddChild(presetNameInput);
        presetNameContainer.AddChild(expandLayerSectionsButton);

        presetContainer.AddChild(presetNameContainer);
        presetContainer.AddChild(layerPicker);

        innerMargin.AddChild(presetContainer);
        panelContainer.AddChild(innerMargin);
        presetAndDeleteButtonContainer.AddChild(panelContainer);
        presetAndDeleteButtonContainer.AddChild(deleteButton);
        mainContainer.AddChild(presetAndDeleteButtonContainer);

        return mainContainer;
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

    private static HBoxContainer CreateHBoxContainer(int separation = 4)
    {
        var container = new HBoxContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };
        container.AddThemeConstantOverride("separation", separation);

        return container;
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
