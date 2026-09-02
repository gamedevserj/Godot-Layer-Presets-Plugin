using Godot;
using Godot.Collections;

namespace PhysicsLayerPresets;
public partial class PresetData : GodotObject
{
    public string Id { get; set; }
    public string Name { get; set; }
    public uint Layer {  get; set; }

    public PresetData() { }

    public PresetData(string id, string name, uint layer)
    {
        Id = id;
        Name = name;
        Layer = layer;
    }

    public PresetData(System.Collections.Generic.KeyValuePair<string, Dictionary<string, uint>> dictionary)
    {
        Id = dictionary.Key;

        foreach (var item in dictionary.Value)
        {
            Name = item.Key;
            Layer = item.Value;
        }
    }

    public Dictionary<string, Dictionary<string, uint>> ToDictionary()
    {
        var innerDictionary = new Dictionary<string, uint> { { Name, Layer } };
        var dictionary = new Dictionary<string, Dictionary<string, uint>>
        {
            { Id, innerDictionary }
        };

        return dictionary;
    }
}
