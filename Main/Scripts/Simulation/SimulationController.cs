using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class SimulationController : Node
{
    // DATA //
    [Export] private float simulationSecondsBetweenSteps = 1;

    [Export] private int stepsPerSecond;

    private List<ISimulatedEntity> simulatedEntities;


    // METHODS //
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public void RegisterEntity(ISimulatedEntity entity)
    {
        simulatedEntities.Add(entity);
    }

    public void DeregisterEntity(ISimulatedEntity entity)
    {
        simulatedEntities.Remove(entity);
    }
}
