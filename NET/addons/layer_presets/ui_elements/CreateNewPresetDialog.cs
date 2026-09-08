using Godot;
using System.Collections.Generic;

namespace LayerPresets;
public partial class CreateNewPresetDialog : ConfirmationDialog
{
    private readonly LineEdit _inputField;
    private readonly PropertyHint _propertyHint;

    public CreateNewPresetDialog() { }

    public CreateNewPresetDialog(GodotObject @object, string propertyName, uint currentLayer, PropertyHint propertyHint)
    {
        _propertyHint = propertyHint;
        var presets = PresetsController.GetAllPresets(propertyHint);
        Title = $"Adding new {SettingsConstants.GetFormattedPropertyHintName(propertyHint)} preset";
        DialogCloseOnEscape = true;

        var okButton = GetOkButton();
        var verticalContainer = new VBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        var nameIsValidLabel = new Label { Text = "Enter preset name" };
        _inputField = new LineEdit { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };

        verticalContainer.AddChild(_inputField);
        verticalContainer.AddChild(nameIsValidLabel);
        AddChild(verticalContainer);

        HashSet<string> presetNames = [];
        foreach (var preset in presets)
        {
            presetNames.Add(preset.Value.Name);
        }

        _inputField.TextChanged += (newText) =>
        {
            var status = NameValidator.IsNameValid(newText, presetNames);
            nameIsValidLabel.Text = NameValidator.GetStatusMessage(status);
            var isValid = status == NameValidityStatus.Valid;
            okButton.Disabled = !isValid;
            var color = isValid ? Colors.Green : Colors.Red;
            nameIsValidLabel.AddThemeColorOverride("font_color", color);
        };

        _inputField.TextSubmitted += (newText) =>
        {
            var status = NameValidator.IsNameValid(newText, presetNames);
            if (status == NameValidityStatus.Valid)
            {
                EmitSignalConfirmed();
            }
        };

        Confirmed += () =>
        {
            var name = _inputField.Text;
            var id = PresetsController.AddPreset(name, currentLayer, _propertyHint);
            @object.SetMeta(PresetsController.GetPresetMetaName(@object, propertyName, propertyHint), id);
            @object.NotifyPropertyListChanged();
            QueueFree();
        };

        okButton.Disabled = true;

        FocusExited += QueueFree;
        Canceled += QueueFree;
        CloseRequested += QueueFree;
    }

    public void ShowDialog()
    {
        PopupCentered();
        _inputField.GrabFocus();
    }
}
