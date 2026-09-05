#if TOOLS
using Godot;

namespace LayerPresets;
public partial class LayerPresetsInspector : EditorInspectorPlugin
{
    public override bool _CanHandle(GodotObject @object)
    {
        if (@object == null) return false;

        foreach (var property in @object.GetPropertyList())
        {
            var type = property["type"].As<Variant.Type>();
            var proprtyHint = property["hint"].As<PropertyHint>();

            if (type == Variant.Type.Int && SettingsConstants.HandledProperties.Contains(proprtyHint))
            {
                return true;
            }
        }
        return false;
    }

    public override bool _ParseProperty(GodotObject @object, Variant.Type type, string name, PropertyHint hintType, string hintString, PropertyUsageFlags usageFlags, bool wide)
    {
        bool canParse = (type == Variant.Type.Int && SettingsConstants.HandledProperties.Contains(hintType));
        if (canParse)
        {
            AddPropertyEditor(name, new CustomLayerMaskProperty(@object, name, hintType));
            return true;
        }
        return false;
    }
}
#endif