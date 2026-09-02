using Godot;
using Godot.Collections;
using System;

namespace PhysicsLayerPresets;
public partial class CreateLayerDialog : ConfirmationDialog
{
    private enum NameValidityStatus { Valid, Empty, Duplicate };

    private LineEdit _inputField;

    public CreateLayerDialog()
    {}

    public CreateLayerDialog(GodotObject @object, PresetData[] presets, uint currentLayer)
    {
        Title = "Adding new preset";
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

        System.Collections.Generic.HashSet<string> presetNames = [];
        for (int i = 0; i < presets.Length; i++)
        {
            presetNames.Add(presets[i].Name);
        }

        _inputField.TextChanged += (string newText) =>
        {
            var status = IsNameValid(newText, presetNames);
            nameIsValidLabel.Text = GetStatusMessage(status);
            okButton.Disabled = status != NameValidityStatus.Valid;
        };

        Confirmed += () =>
        {
            var name = _inputField.Text;
            var id = Guid.NewGuid().ToString();
            PresetsController.AddPreset(id, name, currentLayer);
            QueueFree();
            @object.NotifyPropertyListChanged();
        };

        okButton.Disabled = true;
        Canceled += () => QueueFree();
        CloseRequested += () => QueueFree();
    }

    public void ShowDialog()
    {
        PopupCentered();
        _inputField.GrabFocus();
    }

    private static NameValidityStatus IsNameValid(string newText, System.Collections.Generic.HashSet<string> presets)
    {
        if (string.IsNullOrWhiteSpace(newText))
            return NameValidityStatus.Empty;

        if (presets.Contains(newText))
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
