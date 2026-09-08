using System.Collections.Generic;

namespace LayerPresets;
internal static class NameValidator
{
    public static NameValidityStatus IsNameValid(string newPresetName, HashSet<string> presetNames)
    {
        if (string.IsNullOrWhiteSpace(newPresetName))
            return NameValidityStatus.Empty;

        if (presetNames.Contains(newPresetName))
            return NameValidityStatus.Duplicate;

        return NameValidityStatus.Valid;
    }

    public static string GetStatusMessage(NameValidityStatus status) =>
        status switch
        {
            NameValidityStatus.Valid => "Name is valid",
            NameValidityStatus.Empty => "Name can not be empty",
            NameValidityStatus.Duplicate => "Preset with this name already exists",
            _ => "Name can not be empty",
        };
}
