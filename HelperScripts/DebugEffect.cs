using Godot;
using System;

public partial class DebugEffect : RefCounted
{
    public Node EffectNode { get; private set; }
    public double StartTime { get; private set; }
    public double Duration { get; private set; }

    public DebugEffect(Node effectNode, double startTime, double duration)
    {
        EffectNode = effectNode;
        StartTime = startTime;
        Duration = duration;
    }
}
