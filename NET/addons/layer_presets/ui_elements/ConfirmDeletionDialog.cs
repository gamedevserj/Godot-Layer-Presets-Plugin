using Godot;

namespace LayerPresets;
public partial class ConfirmDeletionDialog : ConfirmationDialog
{
    public ConfirmDeletionDialog() { }

    public ConfirmDeletionDialog(string title, string text)
    {
        Title = title;
        var label = new Label
        {
            Text = text,
        };
        AddChild(label);

        FocusEntered += () => { GetCancelButton().GrabFocus(); };
        FocusExited += QueueFree;
        Canceled += QueueFree;
        CloseRequested += QueueFree;
    }
}
