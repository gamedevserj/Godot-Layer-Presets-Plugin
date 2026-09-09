# Godot-Layer-Presets-Plugin

Inspired by https://github.com/Adrien-Lucas/godot-collision-presets  
This plugin allows creating presets for Physics2D/3D, Render2D/3D, Navigation2D/3D, and Avoidance layers.  

## How to use
- Copy layer_presets folder into your addons folder
- Go to Project Settings -> Plugins and enable the plugin

#### Properties having the following attributes (both built-in and created by you) will be handled by this plugin:  
PropertyHint.Layers2DPhysics, PropertyHint.Layers3DPhysics, PropertyHint.Layers2DRender, PropertyHint.Layers3DRender, PropertyHint.Layers2DNavigation, PropertyHint.Layers3DNavigation, PropertyHint.LayersAvoidance

There are two ways of creating a preset:
1. In inspector
  - Go to the node that contains type (Physics/Render etc.) you want to create preset for
  - Set it up by clicking which layers should be enabled/disabled for that preset
  - Click the '+' button to add new preset from the current setup
2. In the settings window
  - The plugin adds a tab to project settings called 'Layer presets', so you can go Project Settings -> Layer presets
  - Or if you have a node selected that has a property with one of the handled attributes - there is a button with cog icon which opens the settings window

In the settings you can create/edit/delete presets.  

When you select preset on a node the information about it will be saved to meta. If you then go to settings and change the preset's name or layers you can update the nodes that use that preset to use new values by providing the path to the folder with scenes that you want to update in the filed under 'Delete all' button and then click 'Update metas' button. It will go over every scene in the folder and check whether node has a meta the preset can be recovered from and if so, it will update the property on the node. It will also get rid of the old metas, if you deleted a preset, but information about it was saved in a node.

## Limitations
The _ParseProperty method accepts GodotObject as parameter, which means there is no information about which class the property belongs to. Here's and example:  
```
public partial class BaseClass : Control
{
  [Export(PropertyHint.Layers3DPhysics)]
  private uint _layer = 1;

  public override void _Ready()
  {
      GD.Print($"BaseClass = {_layer}");
  }
}

public partial class ChildClass : BaseClass
{
  [Export(PropertyHint.Layers3DPhysics)]
  private uint _layer = 1;

  public override void _Ready()
  {
      base._Ready();
      GD.Print($"ChildClass = {_layer}");
  }
}
```

When you change the layers both properties will update, but if you press play you will see that only the child class has correct layer value. So avoid naming the fields/properties the same way in your child classes as your parent class.
