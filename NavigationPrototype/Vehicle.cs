using Godot;
using System;

[GlobalClass]
public partial class Vehicle : Node3D
{
    // DATA //
    // Instance Configs
    [Export] private float acceleration;
    [Export] private float braking;

    // Cached Data
    private float currentVelocity;
    private float targetVelocity;


    // FUNCTIONS //
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
