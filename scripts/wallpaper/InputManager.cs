using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

public partial class InputManager : Node2D
{
    private List<(WallpaperInputEvent, Action)> inputSubscribers = new List<(WallpaperInputEvent, Action)>();
    private List<(WallpaperMouseEvent, Action)> mouseSubscribers = new List<(WallpaperMouseEvent, Action)>();

    public static InputManager Instance;


    [DllImport("user32.dll")]
    public static extern int GetAsyncKeyState(Int32 i);
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    private static extern IntPtr GetDesktopWindow();

    public override void _Ready()
    {
        Instance = this;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {

        if (!WindowFocus.IsDesktopActive()) return;
        foreach ((WallpaperInputEvent inputEvent, Action func) in inputSubscribers) {

            int isPressed = GetAsyncKeyState(inputEvent.code);

            int required = inputEvent.justPressed ? 32769 : 32768;
            if (isPressed == required) {
                func();
            }
        }
        foreach ((WallpaperMouseEvent mouseEvent, Action func) in mouseSubscribers) {

            int isPressed = GetAsyncKeyState(mouseEvent.click);

            int required = 32768;
            
            if (isPressed != required) continue;
            Vector2 mousePos = GetViewport().GetMousePosition();

            if (!mouseEvent.area.HasPoint(mousePos)) continue;
            func();
        }
	}

    public void SubscribeInput(WallpaperInputEvent inputEvent, Action action) {
        this.inputSubscribers.Add((inputEvent, action));
    }
    public void SubscribeMouse(WallpaperMouseEvent mouseEvent, Action action) {
        this.mouseSubscribers.Add((mouseEvent, action));
    }
}

public class WallpaperInputEvent {
    public int code;
    public bool justPressed;
    public WallpaperInputEvent(int code, bool justPressed = true) {
        this.code = code;
        this.justPressed = justPressed;
    }
}

public class WallpaperMouseEvent {
    public int click; // 1 for left click and 2 for right click
    public Rect2 area;
    public WallpaperMouseEvent(int click, Rect2 area) {
        this.click = click;
        this.area = area;
    }
}
