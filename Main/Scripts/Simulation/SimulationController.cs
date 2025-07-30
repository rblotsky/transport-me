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
    [Export] public double simulationTimeMultiplier { get { return _multiplier; } set { UpdateSimulationStep(value); _multiplier = value; } }
    // the base we use as modifiers. Represents the amount of seconds per (in game) minute
    private static double baseSimulationSpeed = 0.2d;
    private double cachedSimulationStep = 0.2f;
    // Usually gets re-freshed and goes down
    private double _timeUntilNextSimTick = 0f;

    private List<ISimulatable> subscribers;

    public override void _EnterTree()
    {
        subscribers = new List<ISimulatable>();
        base._EnterTree();
    }
    // METHODS //
    public override void _PhysicsProcess(double delta)
    {
        _timeUntilNextSimTick -= delta;
        while (_timeUntilNextSimTick < 0f)
        {
            _timeUntilNextSimTick += cachedSimulationStep;
            subscribers.ForEach(s => s.BeforeSimulationStep());
            subscribers.ForEach((s) => s.Simulate(0.02));
            subscribers.ForEach(s => s.AfterSimulationStep());
        }
    }

    private void UpdateSimulationStep(double newMultiplier)
    {
        GD.Print("Simulation Controller > Updated multiplier to ", newMultiplier);
        cachedSimulationStep = baseSimulationSpeed / simulationTimeMultiplier;
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
            subscribers.Add(item);
        }
    }

    public void Unregister(ISimulatable item)
    {
        if (subscribers.Contains(item)) { 
            subscribers.Remove(item);
        }
    }
}
