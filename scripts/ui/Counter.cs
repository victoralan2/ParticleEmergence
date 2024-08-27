using Godot;
using System;

public partial class Counter : Label
{
    double time_elapsed = 0;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        time_elapsed += delta;
        this.Text = "Uptime:\n" + ConvertSecondsToTimeString(time_elapsed);
	}


    public static string ConvertSecondsToTimeString(double seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return timeSpan.ToString(@"hh\:mm\:ss");
    }
}
