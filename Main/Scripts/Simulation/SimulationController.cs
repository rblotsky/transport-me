using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class SimulationController : Node
{
    // DATA //
    [Export] private double simulationTimeMultiplier = 60.0f;

    // METHODS //
    public override void _PhysicsProcess(double delta)
    {
        GD.Print(CurrentSimulationTimeSeconds() + "seconds");
    }

    public double CurrentSimulationTimeSeconds()
    {
        return (double)((Time.GetTicksMsec() / 1000.0d) * simulationTimeMultiplier);
    }

}
