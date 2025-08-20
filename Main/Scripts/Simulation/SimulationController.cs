using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Transportme.Main.Core;
using Transportme.Main.Scripts.Simulation;

[GlobalClass]
public partial class SimulationController : Node, IMyObservable<ISimulatable>
{
    // DATA //
    private double _multiplier = 1.0f;
    [Export] public double simulationTimeMultiplier { get { return _multiplier; } set { UpdateSimulationStep(value); } }
    // the base we use as modifiers. Represents the amount of seconds per (in game) minute
    private static double baseSimulationSpeed = 0.2d;
    private double cachedSimulationStep = 0.2d;
    // Usually gets re-freshed and goes down
    private double _timeUntilNextSimTick = 0f;
    [Export] private bool _isSimulationRunning = false;
    public bool IsSimulationRunning { get { return _isSimulationRunning; } }

    private List<ISimulatable> subscribers;
    private List<ISimulatable> pendingAddition;
    private List<ISimulatable> pendingRemoval;

    public override void _EnterTree()
    {
        subscribers = new List<ISimulatable>();
        pendingRemoval = new List<ISimulatable>();
        pendingAddition = new List<ISimulatable>();
        base._EnterTree();
    }
    // METHODS //
    public override void _PhysicsProcess(double delta)
    {
        if(!_isSimulationRunning) return;
        _timeUntilNextSimTick -= delta;
        while (_timeUntilNextSimTick < 0f)
        {
            _timeUntilNextSimTick += cachedSimulationStep;
            SimulationStep();
        }   
    }

    private void SimulationStep()
    {
        foreach (ISimulatable toAdd in pendingAddition)
        {
            subscribers.Add(toAdd);
        }
        //todo pending additons
        subscribers.ForEach(s => s.BeforeSimulationStep());
        subscribers.ForEach((s) => s.Simulate(0.02));
        subscribers.ForEach(s => s.AfterSimulationStep());

        foreach (ISimulatable toRemove in pendingRemoval)
        {
            subscribers.Remove(toRemove);
        }
        pendingRemoval.Clear();
        pendingAddition.Clear();

    }

    public void ToggleSimulation()
    {
        _isSimulationRunning = !IsSimulationRunning;
    }

    public void SimulateSteps(int stepsCount)
    {
        for (int i = 0; i < stepsCount; i++)
        {
            SimulationStep();
        }
    }

    private void UpdateSimulationStep(double newMultiplier)
    {
        GD.Print("Simulation Controller > Updated multiplier to ", newMultiplier);
        _multiplier = newMultiplier;
        cachedSimulationStep = baseSimulationSpeed / newMultiplier;
    }

    public double CurrentSimulationTimeSeconds()
    {
        return (double)((Time.GetTicksMsec() / 1000.0d) * simulationTimeMultiplier);
    }

    public void Register(ISimulatable item)
    {
        if (!subscribers.Contains(item))
        {
            GD.Print("SimController > Registered", item);
            pendingAddition.Add(item);
        }
    }

    public void Unregister(ISimulatable item)
    {
        if (subscribers.Contains(item))
        {
            pendingRemoval.Add(item);
        }
    }
}
