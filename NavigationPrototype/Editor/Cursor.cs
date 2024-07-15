using Godot;
using System;

[GlobalClass]

public partial class Cursor : Node3D
{
    // DATA //
    private Vector3 nextPos = Vector3.Zero;

    // Exported
    [Export] private float lerpWeight = 15f;
    [Export] private bool useSmoothing = true;


    // FUNCTIONS //
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (useSmoothing) GlobalPosition = GlobalPosition.Lerp(nextPos, (float)delta * lerpWeight);
        else GlobalPosition = nextPos;

        base._Process(delta);
    }


    // External Usage
    public void SetNextPos(Vector3 newPos)
    {
        nextPos = newPos;

        if (!useSmoothing)
        {
            GlobalPosition = nextPos;
        }
    }
}
