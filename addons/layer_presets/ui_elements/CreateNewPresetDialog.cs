using Godot;
using System;
using System.Collections.Generic;

namespace LayerPresets;
public partial class CreateNewPresetDialog : ConfirmationDialog
{
    private enum NameValidityStatus { Valid, Empty, Duplicate };

    private readonly LineEdit _inputField;
    private readonly PropertyHint _propertyHint;

    public CreateNewPresetDialog()
    {}

    public CreateNewPresetDialog(GodotObject @object, string propertyName, uint currentLayer, PropertyHint propertyHint)
    {
        _propertyHint = propertyHint;
        var presets = PresetsController.GetAllPresets(propertyHint);
        Title = $"Adding new {propertyHint} preset";
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
            var status = IsNameValid(newText, presetNames);
            nameIsValidLabel.Text = GetStatusMessage(status);
            okButton.Disabled = status != NameValidityStatus.Valid;
        };

        _inputField.TextSubmitted += (newText) =>
        {
            var status = IsNameValid(newText, presetNames);
            if (status == NameValidityStatus.Valid)
            {
                EmitSignalConfirmed();
            }
        };

        Confirmed += () =>
        {
            var name = _inputField.Text;
            var id = Guid.NewGuid().ToString();
            PresetsController.AddPreset(new PresetData(id, name, currentLayer, _propertyHint));
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

    private static NameValidityStatus IsNameValid(string newPresetName, HashSet<string> presetNames)
    {
        if (string.IsNullOrWhiteSpace(newPresetName))
            return NameValidityStatus.Empty;

        if (presetNames.Contains(newPresetName))
            return NameValidityStatus.Duplicate;

        return NameValidityStatus.Valid;
    }

    private static string GetStatusMessage(NameValidityStatus status) =>
        status switch
        {
            NameValidityStatus.Valid => "Name is valid",
            NameValidityStatus.Empty => "Name can not be empty",
            NameValidityStatus.Duplicate => "Preset with this name already exists",
            _ => "Name can not be empty",
        };
}
