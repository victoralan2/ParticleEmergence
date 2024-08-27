using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;

public partial class OptionsMenu : Control
{
    [Export] public ParticleManager particleManager;
    [Export] public LineEdit ParticleColors;
    [Export] public SpinBox NumberOfParticles;
    [Export] public SpinBox SpaceSizeX;
    [Export] public SpinBox SpaceSizeY;
    [Export] public SpinBox Seed;
    [Export] public SpinBox TimeScale;
    [Export] public Button menuButton;
    public Window parent;

    public Configuration currentConfig = new Configuration();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        parent = (Window)this.GetParent();

        Rect2 rect = menuButton.GetGlobalRect();
        InputManager.Instance.SubscribeMouse(new WallpaperMouseEvent(1, rect), _Popup);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (parent.Visible) {
            currentConfig.NumberOfParticles = (uint)NumberOfParticles.Value;
            currentConfig.SpaceDimensions = new Vector2((float)SpaceSizeX.Value, (float)SpaceSizeY.Value);
            currentConfig.TimeScale = (float)TimeScale.Value;
            List<Color> colors = new List<Color>();
            string[] particleColors = ParticleColors.Text.Split(",");
            foreach (string color in particleColors) {
                colors.Add(Color.FromString(color, Colors.Black));
            }
            currentConfig.ParticleColors = colors.ToArray();
            currentConfig.Seed = (int)Seed.Value;
        }
	}

    public void _OnApplyPressed() {
        particleManager.Reset(currentConfig);
    }
    public void _Popup() {
        parent.Visible = true;
    }
    
    public void _OnWindowClose() {
        parent.Visible = false;
    }
}
