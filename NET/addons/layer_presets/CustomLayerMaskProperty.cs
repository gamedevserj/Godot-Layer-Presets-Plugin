using Godot;

namespace LayerPresets;

[Tool]
public partial class CustomLayerMaskProperty : EditorProperty
{
    private const uint DefaultLayerValue = 1;

    private readonly PropertyHint _propertyHint;
    private readonly GodotObject _object;
    private readonly LayerPicker _layerPicker;
    private readonly Button _resetButton;

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
        
        var addNewPresetButton = new AddNewPresetButton(@object, propertyName, propertyHint);

        var buttonsContainer = new HBoxContainer();
        buttonsContainer.Alignment = BoxContainer.AlignmentMode.End;
        var hasPresets = presets.Count > 0;
        if (hasPresets)
        {
            var dropdown = new PresetsDropdown(@object, propertyName, propertyHint, SelectFromPreset);
            buttonsContainer.AddChild(dropdown);
        }
        var expandLayerSectionsButton = new ExpandLayerSectionsButton();
        buttonsContainer.AddChild(expandLayerSectionsButton);
        buttonsContainer.AddChild(addNewPresetButton);

        _resetButton = new ResetButton(() => UpdateLayer(DefaultLayerValue));
        buttonsContainer.AddChild(_resetButton);

        if (hasPresets)
        {
            var settingsButton = new OpenPresetSettingsWindowButton(propertyHint);
            buttonsContainer.AddChild(settingsButton);
        }

        _layerPicker = new LayerPicker((uint)@object.Get(propertyName), expandLayerSectionsButton);
        _layerPicker.OnLayerUpdatedManually += OnLayerUpdatedManually;

        verticalContainer.AddChild(buttonsContainer);
        verticalContainer.AddChild(_layerPicker);
        marginContainer.AddChild(verticalContainer);
        AddChild(marginContainer);
        SetBottomEditor(marginContainer);
    }

    public override void _EnterTree()
    {
        SwitchResetButtonVisibility();
        PresetsController.OnPresetDeleted += OnPresetDeleted;
        PresetsController.OnAllPresetsDeleted += OnAllPresetsDeleted;
    }

    public override void _ExitTree()
    {
        PresetsController.OnPresetDeleted -= OnPresetDeleted;
        PresetsController.OnAllPresetsDeleted -= OnAllPresetsDeleted;
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
        SwitchResetButtonVisibility();
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

    private void OnPresetDeleted(string id) => _object.NotifyPropertyListChanged();

    private void OnAllPresetsDeleted(PropertyHint propertyHint)
    {
        if (_propertyHint == propertyHint)
        {
            _object.NotifyPropertyListChanged();
        }
    }

    private void SwitchResetButtonVisibility()
    {
        _resetButton.Visible = (uint)_object.Get(GetEditedProperty()) != DefaultLayerValue;
    }
}