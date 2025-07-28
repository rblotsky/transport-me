using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class SimulationController : Node
{
    // DATA //
    [Export] private double simulationSecondsBetweenSteps = 0.1f;
    [Export] private double realSecondsBetweenSteps = 0.1f;

    private double secondsSinceStart = 0.0f;
    private double timeOfLastStep = 0.0f;
    private List<ISimulatedEntity> simulatedEntities = new List<ISimulatedEntity>();
    private List<ISimulatedEntity> entitiesToRemoveAtNextOpportunity = new List<ISimulatedEntity>();
    private List<ISimulatedEntity> entitiesToAddAtNextOpportunity = new List<ISimulatedEntity>();

    public static SimulationController instance = null;


    // METHODS //
    public override void _EnterTree()
    {
        instance = this;
        base._EnterTree();
    }

    public override void _Ready()
    {
        secondsSinceStart = 0.0f;
        timeOfLastStep = 0.0f;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Determine how many steps to run, then runs those steps
        secondsSinceStart += delta;

        double timeSinceLastStep = secondsSinceStart - timeOfLastStep;
        int newSteps = (int)(timeSinceLastStep / realSecondsBetweenSteps);

        for(int i = 0; i < newSteps; i++)
        {
            RunSimulationStep();
            timeOfLastStep = secondsSinceStart;
        }

        base._PhysicsProcess(delta);
    }

    private void RunSimulationStep()
    {
        foreach(ISimulatedEntity entity in simulatedEntities)
        {
            entity.BeforeSimulationStep(simulationSecondsBetweenSteps);
        }
        foreach (ISimulatedEntity entity in simulatedEntities)
        {
            entity.SimulationStep(simulationSecondsBetweenSteps);
        }
        foreach (ISimulatedEntity entity in simulatedEntities)
        {
            entity.AfterSimulationStep(simulationSecondsBetweenSteps);
        }

        // Adds new entities and removes deregistered ones at end of the current step
        // to ensure the list of simulated entities doesn't change in the middle of
        // a step.
        AddNewlyRegisteredEntities();
        RemoveDeregisteredEntities();
    }

    // Registration of Entities
    public void RegisterEntity(ISimulatedEntity entity)
    {
        entitiesToAddAtNextOpportunity.Add(entity);
    }

    public void DeregisterEntity(ISimulatedEntity entity)
    {
        entitiesToRemoveAtNextOpportunity.Add(entity);
    }

    private void AddNewlyRegisteredEntities()
    {
        foreach (ISimulatedEntity entity in entitiesToAddAtNextOpportunity)
        {
            simulatedEntities.Add(entity);
        }

        entitiesToAddAtNextOpportunity.Clear();
    }

    private void RemoveDeregisteredEntities()
    {
        foreach (ISimulatedEntity entity in entitiesToRemoveAtNextOpportunity)
        {
            simulatedEntities.Remove(entity);
        }

        entitiesToRemoveAtNextOpportunity.Clear();
    }
}
