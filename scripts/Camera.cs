using System;
using System.Diagnostics;
using Godot;

public partial class Camera : Camera2D
{
    // Speed at which the camera moves
    [Export] public float PanSpeed = 200f;
    [Export] public float ZoomSpeed = 0.2f;
    [Export] public float ShiftMultiplier = 10f;
    [Export(PropertyHint.ExpEasing)] public float easingCurve = -5f;
    public Vector2 defaultZoom = Vector2.One;
    public Vector2 defaultPosition = Vector2.Zero;
    public bool isInTransition = false;
    public float timePassed = 0f;
    public float transitionProcess = 0f;
    public Vector2 transitionPositionStart = Vector2.Zero;
    public Vector2 transitionZoomStart = Vector2.One;

    // public override void _PhysicsProcess(double delta)
    // {
    //     float panSpeed = Input.IsKeyPressed(Key.Shift) ? PanSpeed * ShiftMultiplier : PanSpeed;
    //     Vector2 cameraMovement = Vector2.Zero;

    //     cameraMovement.X = Input.GetAxis("move_left", "move_right") * panSpeed * (float)delta / Zoom.Length();
    //     cameraMovement.Y = -Input.GetAxis("move_down", "move_up") * panSpeed * (float)delta / Zoom.Length();


    //     // Apply the camera movement
    //     GlobalPosition += cameraMovement;
    //     // Zoom the camera with the mouse wheel
    //     if (Input.IsActionJustPressed("zoom_in"))
    //     {
    //         Zoom *= ZoomSpeed + 1;
    //     }
    //     if (Input.IsActionJustPressed("zoom_out"))
    //     {
    //         Zoom /= ZoomSpeed + 1;
    //     }
    // }
    public override void _Process(double delta)
    {
        ProcessTransition((float)delta);
    }
    public void ProcessTransition(float delta) {
        const float targetTime = 1f;
        if (!isInTransition) return;
    
        timePassed+=delta;
        float newProcess = transitionProcess + delta/targetTime;

        this.GlobalPosition = TransitionStep(transitionPositionStart, defaultPosition, newProcess+0.2f, easingCurve);
        this.Zoom = TransitionStep(transitionZoomStart, defaultZoom, newProcess+0.2f, easingCurve);
        transitionProcess = newProcess;
        if (newProcess >= 1f) {
            isInTransition = false;
            transitionProcess = 0f;
            transitionPositionStart = Vector2.Zero;
            transitionZoomStart = Vector2.One;
            timePassed = 0f;
        }
    }

    // public override void _Input(InputEvent @event)
    // {
    //     if (@event.IsActionPressed("reset"))
    //     {
    //         if (isInTransition) return;
    //         isInTransition = true;
    //         transitionPositionStart = GlobalPosition;
    //         transitionZoomStart = Zoom;
    //     }
    // }
    private Vector2 TransitionStep(Vector2 start, Vector2 end, float t, float easingCurve)
    {
        return start.Lerp(end, Mathf.Ease(t, easingCurve));
    }

    
    public void SetCameraPosition(Vector2 center, float width, float height) {
        Vector2 resolution = GetViewportRect().Size.Normalized();
        float w = resolution.X * width;
        float h = resolution.Y * height;
        float max = Math.Max(w, h);
        float fix = max == w ? w / resolution.X : h / resolution.Y;

        this.Position = center;
        float zoomFactor = GetViewportRect().Size.X / fix;
        this.Zoom = new Vector2(zoomFactor, zoomFactor);
        this.defaultPosition = center;
        this.defaultZoom = Zoom;
    }
}
