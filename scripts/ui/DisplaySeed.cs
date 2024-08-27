using Godot;
using System;

public partial class DisplaySeed : Label
{
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        ParticleManager manager = NodeFinder.FindNodesOfType<ParticleManager>(GetTree().Root)[0];
        this.Text = "Seed: " + manager.Seed;
	}
}
