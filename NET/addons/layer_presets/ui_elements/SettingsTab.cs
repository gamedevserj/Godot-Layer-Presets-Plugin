using Godot;
using System.Collections.Generic;

namespace LayerPresets;
public partial class SettingsTab : VBoxContainer
{
    private const int PresetsListTitleFontSize = 22;

    private readonly PropertyHint _propertyHint;
    private readonly Dictionary<string, HBoxContainer> _presetsBindings = [];
    private readonly VBoxContainer _container;
    private readonly DeleteAllLayerPresetsButton _deleteAllButton;
    private readonly VBoxContainer _presetsContainer;
    private readonly VBoxContainer _presetsArea;

    public SettingsTab() { }

    public SettingsTab(PropertyHint propertyHint) 
    {
        _propertyHint = propertyHint;
        var presets = PresetsController.GetAllPresets(propertyHint);

        _container = CreateVBoxContainer(0);

        var buttonsContainer = new VBoxContainer();
        buttonsContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;

        var scrollContainer = CreateScrollContainer();
        _presetsContainer = CreateVBoxContainer(10);

        foreach (var preset in presets)
        {
            var presetElement = CreatePresetElement(preset.Value);
            _presetsContainer.AddChild(presetElement);
            _presetsBindings.Add(preset.Key, presetElement);
        }

        _deleteAllButton = new DeleteAllLayerPresetsButton(propertyHint);
        _deleteAllButton.Pressed += () =>
        {
            var confirmDialog = new ConfirmDeletionDialog(
                $"Confirm deleting {propertyHint} presets", 
                $"Delete all {SettingsConstants.GetFormattedPropertyHintName(propertyHint)} presets?");
            confirmDialog.Confirmed += () => { PresetsController.DeleteAll(propertyHint); };
            AddChild(confirmDialog);
            confirmDialog.PopupCentered();
        };
        

        var restoreFromPresetsContainer = CreateHBoxContainer();
        var folderPath = new LineEdit
        {
            Text = "res://",
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        var restorePresetsFromMetaButton = new Button();
        restorePresetsFromMetaButton.Text = "Update metas";
        restorePresetsFromMetaButton.TooltipText = 
            $"Go over every scene in specified folder and restore layers on objects that have presets.\n" +
            $"Remove all meta ids for presets that were deleted.\n" +
            $"Use it if you edited preset in this menu and want nodes in scenes update their layer according to preset.";
        restorePresetsFromMetaButton.Pressed += () => MetaRestoreController.RestoreLayersFromMeta(propertyHint, folderPath.Text);
        
        restoreFromPresetsContainer.AddChild(restorePresetsFromMetaButton);
        restoreFromPresetsContainer.AddChild(folderPath);

        buttonsContainer.AddChild(_deleteAllButton);
        buttonsContainer.AddChild(restoreFromPresetsContainer);
        buttonsContainer.AddChild(CreateAddNewPresetContainer());

        scrollContainer.AddChild(_presetsContainer);

        _container.AddChild(buttonsContainer);

        _presetsArea = new VBoxContainer();
        var separator = new HSeparator();
        var style = new StyleBoxLine();
        style.Thickness = 5;
        separator.AddThemeStyleboxOverride("normal", style);
        separator.AddThemeConstantOverride("separation", 20);
        _presetsArea.AddChild(separator);

        var presetsListTitle = new Label();
        presetsListTitle.Text = "PRESETS";
        presetsListTitle.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        presetsListTitle.HorizontalAlignment = HorizontalAlignment.Center;
        presetsListTitle.AddThemeFontSizeOverride("font_size", PresetsListTitleFontSize);
        _presetsArea.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        _presetsArea.SizeFlagsVertical = SizeFlags.ExpandFill;
        _presetsArea.AddChild(presetsListTitle);
        _presetsArea.AddChild(scrollContainer);

        _container.AddChild(_presetsArea);
        UpdateElementsVisibilityOnPresetsAmountChanged();
        AddChild(_container);
    }

    public override void _EnterTree()
    {
        PresetsController.OnPresetCreated += OnPresetCreated;
        PresetsController.OnPresetDeleted += OnPresetDeleted;
        PresetsController.OnAllPresetsDeleted += OnAllPresetsDeleted;
    }

    public override void _ExitTree()
    {
        PresetsController.OnPresetCreated -= OnPresetCreated;
        PresetsController.OnPresetDeleted -= OnPresetDeleted;
        PresetsController.OnAllPresetsDeleted -= OnAllPresetsDeleted;
    }

    private void OnPresetCreated(PresetData preset)
    {
        var presetElement = CreatePresetElement(preset);
        _presetsContainer.AddChild(presetElement);
        _presetsBindings.Add(preset.Id, presetElement);
        UpdateElementsVisibilityOnPresetsAmountChanged();
    }

    private void OnPresetDeleted(string id)
    {
        if (_presetsBindings.TryGetValue(id, out HBoxContainer value))
        {
            value.QueueFree();
            _presetsBindings.Remove(id);
        }

        UpdateElementsVisibilityOnPresetsAmountChanged();
    }

    private void OnAllPresetsDeleted(PropertyHint propertyHint)
    {
        if (_propertyHint == propertyHint)
        {
            foreach (var node in _presetsContainer.GetChildren())
            {
                node.QueueFree();
            }

            _presetsBindings.Clear();
            UpdateElementsVisibilityOnPresetsAmountChanged();
        }
    }

    private void UpdateElementsVisibilityOnPresetsAmountChanged()
    {
        var hasPresets = _presetsBindings.Count != 0;
        _deleteAllButton.Visible = hasPresets;
        _presetsArea.Visible = hasPresets;
    }

    private VBoxContainer CreateAddNewPresetContainer()
    {
        var presets = PresetsController.GetAllPresets(_propertyHint);
        var container = new VBoxContainer();
        
        var baseControl = EditorInterface.Singleton.GetBaseControl();
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
            CornerRadiusTopRight = cornerRadius,
        };
        panelContainer.AddThemeStyleboxOverride("panel", style);

        var innerMargin = CreateMarginContainer(SettingsConstants.Margin);

        var innerContainer = new VBoxContainer();
        var createButton = new ExtendedButton();
        createButton.Text = "Create new";
        createButton.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        createButton.Disabled = true;
        
        var duplicateNameLabel = new Label();
        duplicateNameLabel.Visible = false;
        duplicateNameLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        duplicateNameLabel.HorizontalAlignment = HorizontalAlignment.Center;
        duplicateNameLabel.AddThemeColorOverride("font_color", Colors.Red);
        var presetNameInput = new ExtendedLineEdit
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        };
        presetNameInput.OnTextChanged += (text) =>
        {
            HashSet<string> presetNames = [];
            foreach (var preset in presets.Values)
            {
                presetNames.Add(preset.Name);
            }
            var status = NameValidator.IsNameValid(text, presetNames);
            if (status != NameValidityStatus.Valid)
            {
                var message = NameValidator.GetStatusMessage(status);
                duplicateNameLabel.Text = message;
            }

            duplicateNameLabel.Visible = status != NameValidityStatus.Valid;
            createButton.Disabled = status != NameValidityStatus.Valid;
        };

        var expandLayerSectionsButton = new ExpandLayerSectionsButton();
        var presetNameContainer = new HBoxContainer();
        presetNameContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        presetNameContainer.AddChild(presetNameInput);
        presetNameContainer.AddChild(expandLayerSectionsButton);

        var layerPicker = new LayerPicker(0, expandLayerSectionsButton, _propertyHint);

        createButton.OnButtonPressed += () =>
        {
            // not creating UI presets here because preset can be created via button in the inspector while the settings menu is open
            // so subscribing to event is simpler to handle both cases in terms of updating UI
            PresetsController.AddPreset(presetNameInput.Text, layerPicker.GetLayerFromButtons(), _propertyHint);
            presetNameInput.Text = string.Empty;
            createButton.Disabled = true;

        };
        innerContainer.AddChild(presetNameContainer);
        innerContainer.AddChild(layerPicker);

        innerContainer.AddChild(createButton);
        innerContainer.AddChild(duplicateNameLabel);

        innerMargin.AddChild(innerContainer);
        panelContainer.AddChild(innerMargin);
        container.AddChild(panelContainer);
        return container;
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

        var layerPicker = new LayerPicker(preset.Layer, expandLayerSectionsButton, _propertyHint);
        layerPicker.OnLayerUpdatedManually += (bit, layer) => 
        {
            CheckIfLayerIsDuplicate(layerPicker, preset, bit, layer);
        };

        var deleteButton = new DeletePresetButton();
        deleteButton.Pressed += () =>
        {
            var confirmDialog = new ConfirmDeletionDialog(
                $"Confirm deleting {preset.Name} presets", 
                $"Delete {preset.Name} preset?");

            confirmDialog.Confirmed += () => { PresetsController.DeletePreset(preset.Id, preset.LayerType); };
            AddChild(confirmDialog);
            confirmDialog.PopupCentered();
        };

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

    private void CheckIfLayerIsDuplicate(LayerPicker layerPicker, PresetData preset, int bitChanged, uint layer)
    {
        var isLayerDefined = PresetsController.IsLayerDuplicate(_propertyHint, layer, out (string name, uint layer) presetDuplicate);
        if (!isLayerDefined)
        {
            preset.SetLayer(layer);
        }
        else
        {
            GD.PrintErr($"Preset '{presetDuplicate.name}' already has the same layer!");
            layerPicker.RevertButtonIfLayerIsDefined(bitChanged, presetDuplicate.layer);
        }
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
