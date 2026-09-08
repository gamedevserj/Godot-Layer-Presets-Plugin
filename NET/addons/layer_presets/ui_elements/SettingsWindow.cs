using Godot;

namespace LayerPresets;
public partial class SettingsWindow : Window
{
    public SettingsWindow() { }

    public SettingsWindow(PropertyHint propertyHint) 
    {
        Title = "Layer Presets Settings";
        Size = SettingsConstants.SettingsWindowSize;
        var control = new SettingsViewControl(propertyHint);
        AddChild(control);

        CloseRequested += Close;
    }

    public void Close() => QueueFree();

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            Close();
        }
    }
}
