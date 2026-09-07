using Godot;
using Godot.Collections;
using System;

namespace LayerPresets;
public partial class PresetData : GodotObject
{
    public static event Action<string, string> OnPresetNameUpdated;
    public static event Action<PropertyHint, uint, uint> OnPresetLayerUpdated;

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

    public string Id { get; private set; }
    public string Name { get; private set; }
    public uint Layer { get; private set; }
    public PropertyHint LayerType { get; private set; }


    public void SetName(string newName)
    {
        OnPresetNameUpdated?.Invoke(Id, newName);
        Name = newName;
        PresetsController.SaveEditedPreset(this, LayerType);
    }

    public void SetLayer(uint newLayer)
    {
        OnPresetLayerUpdated?.Invoke(LayerType, Layer, newLayer);
        Layer = newLayer;
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
