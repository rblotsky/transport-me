using Godot;
using System;

[GlobalClass]
public partial class CameraController : Node
{
    // DATA //
    // Scene References 
    [Export] private Camera3D cameraRef;
    [Export] private float zoomSpeed = 5;
    [Export] private float maxZoomDistance = 50;
    [Export] private float minZoomDistance = -50;
    [Export] private float lerpWeight = 2;

    // Cached Data
    private float currentZoomDistance = 0;
    private Vector3 nextCameraPos = Vector3.Zero;
    private Vector3 initialCameraPos = Vector3.Zero;


    // FUNCTIONS //
    // Godot Defaults
    public override void _Process(double delta)
    {
        if(initialCameraPos == Vector3.Zero)
        {
            initialCameraPos = cameraRef.GlobalPosition;
            nextCameraPos = initialCameraPos;
        }

        cameraRef.GlobalPosition = cameraRef.GlobalPosition.Lerp(nextCameraPos, (float)delta * lerpWeight);
        base._Process(delta);
    }

    public override void _UnhandledInput(InputEvent receivedEvent)
    {
        if(receivedEvent is InputEventMouseMotion mouseMotionInput)
        {
            // If middle clicking, pan camera
            if(mouseMotionInput.ButtonMask.HasFlag(MouseButtonMask.Middle))
            {
                cameraRef.RotateY(-0.01f * mouseMotionInput.Relative.X);
                cameraRef.RotateObjectLocal(Vector3.Right, -0.01f * mouseMotionInput.Relative.Y);
            }
        }

        else if (receivedEvent is InputEventMouseButton mouseButtonInput)
        {
            // If scroll wheel, zoom
            if (mouseButtonInput.Pressed)
            {
                if (mouseButtonInput.ButtonIndex == MouseButton.WheelUp)
                {
                    currentZoomDistance -= zoomSpeed;
                }

                else if (mouseButtonInput.ButtonIndex == MouseButton.WheelDown)
                {
                    currentZoomDistance += zoomSpeed;
                }

                currentZoomDistance = Mathf.Clamp(currentZoomDistance, minZoomDistance, maxZoomDistance);
                GoToNextZoomPosition();
            }
        }

        // Call baseclass input
        base._UnhandledInput(receivedEvent);
    }


    // Zoom Functions
    private void GoToNextZoomPosition()
    {
        nextCameraPos = initialCameraPos + cameraRef.GlobalTransform.Basis.Z*currentZoomDistance;
    }
}
