using Godot;
using Godot.Collections;

namespace LayerPresets;
public partial class PresetData : GodotObject
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public uint Layer {  get; private set; }
    public PropertyHint LayerType { get; private set; }

    public PresetData() { }

    public PresetData(string id, string name, uint layer, PropertyHint layerType)
    {
        Id = id;
        Name = name;
        Layer = layer;
        LayerType = layerType;
    }

    public PresetData(Dictionary dictionary)
    {
        Id = dictionary["id"].As<string>();
        Name = dictionary["name"].As<string>();
        Layer = dictionary["layer"].As<uint>();
        LayerType = dictionary["layerType"].As<PropertyHint>();
    }

    public void SetName(string name)
    {
        Name = name;
        PresetsController.SaveEditedPreset(this, LayerType);
    }

    public void SetLayer(uint layer)
    {
        Layer = layer;
        PresetsController.SaveEditedPreset(this, LayerType);
    }

    public Dictionary ToDictionary()
    {
        var dictionary = new Dictionary
        {
            { "id", Id },
            { "name",  Name },
            { "layer" , Layer },
            { "layerType", (int)LayerType },
        };

        return dictionary;
    }
}
