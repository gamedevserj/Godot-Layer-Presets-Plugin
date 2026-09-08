using Godot;

namespace LayerPresets;

[Tool]
public partial class CustomLayerMaskProperty : EditorProperty
{
    private readonly PropertyHint _propertyHint;
    private readonly GodotObject _object;
    private readonly LayerPicker _layerPicker;
    private readonly PresetsDropdown _dropdown;
    private readonly AddNewPresetButton _addNewButton;

    public CustomLayerMaskProperty()
    {}

    public CustomLayerMaskProperty(GodotObject @object, string propertyName, PropertyHint propertyHint)
    {
        _object = @object;
        _propertyHint = propertyHint;
        var presets = PresetsController.GetAllPresets(propertyHint);
        var marginContainer = new MarginContainer();
        var margin = SettingsConstants.Margin;
        marginContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        marginContainer.AddThemeConstantOverride("margin_left", margin);
        marginContainer.AddThemeConstantOverride("margin_right", margin);
        marginContainer.AddThemeConstantOverride("margin_top", margin);
        marginContainer.AddThemeConstantOverride("margin_bottom", margin);

        var verticalContainer = new VBoxContainer();
        verticalContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        verticalContainer.AddThemeConstantOverride("separation", 10); 
        
        var buttonsContainer = new HBoxContainer();
        buttonsContainer.Alignment = BoxContainer.AlignmentMode.End;
        
        if (presets.Count > 0)
        {
            _dropdown = new PresetsDropdown(@object, propertyName, propertyHint, SelectFromPreset);
            buttonsContainer.AddChild(_dropdown);
        }

        var expandLayerSectionsButton = new ExpandLayerSectionsButton();
        _addNewButton = new AddNewPresetButton(@object, propertyName, propertyHint);
        buttonsContainer.AddChild(expandLayerSectionsButton);
        buttonsContainer.AddChild(_addNewButton);

        var settingsButton = new OpenSettingsButton(propertyHint);
        buttonsContainer.AddChild(settingsButton);

        _layerPicker = new LayerPicker((uint)@object.Get(propertyName), expandLayerSectionsButton, _propertyHint);
        _layerPicker.OnLayerUpdatedManually += OnLayerUpdatedManually;

        verticalContainer.AddChild(buttonsContainer);
        verticalContainer.AddChild(_layerPicker);
        marginContainer.AddChild(verticalContainer);
        AddChild(marginContainer);
        SetBottomEditor(marginContainer);
    }

    private uint CurrentLayer => (uint)_object.Get(GetEditedProperty());

    public override void _EnterTree()
    {
        PresetData.OnPresetLayerUpdated += OnPresetLayerUpdated;

        PresetsController.OnPresetCreated += OnPresetCreated;
        PresetsController.OnPresetDeleted += OnPresetDeleted;
        PresetsController.OnAllPresetsDeleted += OnAllPresetsDeleted;
    }

    private void OnPresetCreated(PresetData preset)
    {
        _object.NotifyPropertyListChanged();
    }

    public override void _ExitTree()
    {
        PresetData.OnPresetLayerUpdated -= OnPresetLayerUpdated;

        PresetsController.OnPresetCreated -= OnPresetCreated;
        PresetsController.OnPresetDeleted -= OnPresetDeleted;
        PresetsController.OnAllPresetsDeleted -= OnAllPresetsDeleted;
    }

    public override void _UpdateProperty()
    {
        // pressing the revert button does not call NotifyPropertyListChanged
        // so the layerpicker and dropdown are not being updated, this is a way around it
        if (CurrentLayer == SettingsConstants.DefaultLayerValue)
        {
            if (_dropdown != null)
            {
                _dropdown.UpdateVisual(SettingsConstants.DefaultLayerValue);
            }
            _layerPicker.UpdateOnReset();
        }
        CheckAddButtonVisibility();
    }

    private void CheckAddButtonVisibility()
    {
        _addNewButton.Visible = _dropdown == null || _dropdown.Text == SettingsConstants.NoPresetText;
    }

    private void SelectFromPreset(string presetId)
    {
        var presets = PresetsController.GetAllPresets(_propertyHint);
        var preset = presets[presetId];
        _object.SetMeta(GetMetaName(), presetId);
        UpdateLayer(preset.Layer);
    }

    private void UpdateLayer(uint layer)
    {
        _object.Set(GetEditedProperty(), layer);
        _object.NotifyPropertyListChanged(); // updates the dropdown/layerpicker
        EmitChanged(GetEditedProperty(), layer); // makes the scene dirty
    }

    private void OnLayerUpdatedManually(uint layer)
    {
        var presets = PresetsController.GetAllPresets(_propertyHint);
        var id = "";
        foreach (var preset in presets.Values)
        {
            if (preset.Layer == layer)
            {
                id = preset.Id;
                break;
            }
        }

        if (id != "")
        {
            _object.SetMeta(GetMetaName(), id);
        }
        else
        {
            _object.RemoveMeta(GetMetaName());
        }

        UpdateLayer(layer);
    }

    private string GetMetaName() => PresetsController.GetPresetMetaName(_object, GetEditedProperty(), _propertyHint);

    private void OnPresetDeleted(string id)
    {
        if (_object.HasMeta(GetMetaName()) && (string)_object.GetMeta(GetMetaName()) == id)
        {
            _object.RemoveMeta(GetMetaName());
        }
        _object.NotifyPropertyListChanged();
    }

    private void OnAllPresetsDeleted(PropertyHint propertyHint)
    {
        if (_propertyHint == propertyHint)
        {
            _object.NotifyPropertyListChanged();
        }
    }

    private void OnPresetLayerUpdated(PropertyHint layerType, uint oldLayer, uint newLayer)
    {
        if (layerType == _propertyHint && CurrentLayer == oldLayer)
        {
            _object.Set(GetEditedProperty(), newLayer);
            _object.NotifyPropertyListChanged();
        }
    }
}