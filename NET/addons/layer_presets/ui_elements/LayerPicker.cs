using Godot;
using System;
using System.Collections.Generic;

namespace LayerPresets;

public partial class LayerPicker : GridContainer
{
    private const int SizeUpdatePadding = 5;
    private readonly ExpandLayerSectionsButton _expandButton;
    
    private int[] _limits = new int[4];
    private readonly List<GridContainer> _sections = [];
    private bool _expand;
    private LayerPickerButton[] _buttons = new LayerPickerButton[32];

    public LayerPicker() { } 

    public LayerPicker(uint layer, ExpandLayerSectionsButton expandButton) 
    {
        _expandButton = expandButton;
        _expandButton.OnButtonPressed += OnExpand;
        Init(layer);
    }

    public Action<uint> OnLayerUpdatedManually { get; set; }

    public override void _Notification(int what)
    {
        if (what == NotificationResized)
        {
            CalculateLimits();
            UpdateColumns();
        }
    }

    public void UpdateOnReset()
    {
        for (int i = 0; i < 32; i++)
        {
            int bit = i;
            _buttons[i].ButtonPressed = (1 & (1u << bit)) != 0;
        }
    }

    private void OnExpand()
    {
        _expand = true;
        UpdateColumns();
    }

    private void Init(uint layer)
    {
        Columns = 4;
        SizeFlagsHorizontal = SizeFlags.ExpandFill;
        AddThemeConstantOverride("h_separation", SettingsConstants.LayerPickerSectionSeparation);
        var container = CreateInnerGridContainer();

        for (int i = 0; i < 32; i++)
        {
            int bit = i;
            var tooltipText = (string)ProjectSettings.GetSetting($"layer_names/3d_physics/layer_{bit + 1}", $"Layer {bit + 1}");
            if (tooltipText == string.Empty)
            {
                tooltipText = $"Layer {bit + 1}";
            }
            tooltipText += $"\nBit {bit}, value {Mathf.Pow(2, bit)}";
            var button = new LayerPickerButton(bit, tooltipText, SettingsConstants.ButtonSize);
            button.ButtonPressed = (layer & (1u << bit)) != 0;
            _buttons[i] = button;
            button.Toggled += pressed =>
            {
                UpdateLayer();
            };
            container.AddChild(button);
            if (i > 0 && (i + 1) % 8 == 0 && i < 31)
            {
                container = CreateInnerGridContainer();
                _sections.Add(container);
            }
        }
    }

    private void UpdateColumns()
    {
        var columns = 0;
        for (int i = 0; i < _limits.Length; i++)
        {
            if (Size.X > _limits[i] + SizeUpdatePadding)
            {
                columns++;
            }
        }
        if (columns == 0)
        {
            columns = 1;
        }

        Columns = columns;

        for (int i = 0; i < _sections.Count; i++)
        {
            // showing if there is enough place of if expand button pressed
            _sections[i].Visible = Columns > i + 1 || _expand;
        }

        _expandButton.Visible = !_expand && Columns < 4;
    }

    private void CalculateLimits()
    {
        var sectionSize = (int)SettingsConstants.ButtonSize.X * 4 + SettingsConstants.LayerPickerButtonSeparation * 3;
        for (int i = 0; i < _limits.Length; i++)
        {
            _limits[i] = sectionSize * (i + 1) + SettingsConstants.LayerPickerSectionSeparation * (i);
        }
    }

    private uint UpdateLayer()
    {
        uint layer = 0;

        for (int i = 0; i < _buttons.Length; i++)
        {
            if (_buttons[i].ButtonPressed)
            {
                layer |= (1U << _buttons[i].Bit);
            }
        }

        OnLayerUpdatedManually?.Invoke(layer);
        return layer;
    }

    private GridContainer CreateInnerGridContainer()
    {
        var container = new GridContainer
        {
            Columns = 4,
            SizeFlagsHorizontal = SizeFlags.ShrinkEnd,
        };
        container.AddThemeConstantOverride("h_separation", SettingsConstants.LayerPickerButtonSeparation);
        container.AddThemeConstantOverride("v_separation", SettingsConstants.LayerPickerButtonSeparation);

        AddChild(container);
        return container;
    }
}