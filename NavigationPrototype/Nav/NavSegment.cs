using Godot;
using System;

[GlobalClass]

public partial class NavSegment : Resource
{
    // DATA
    // Serializable Properties
    [Export] public Vector3I Start { get; set; }
    [Export] public Vector3I End { get; set; }

    /// <summary>
    /// Gives the endpoints in an array: [startNode, endNode]
    /// </summary>
    public Vector3I[] Endpoints { get { return new Vector3I[2] { Start, End}; } }
    public Vector3 DirectionalLine { get { return End - Start; }}


    // FUNCTIONS //
    // Data Retrieval
    public Vector3I GetOtherEnd(Vector3I oneEnd)
    {
        if (Start == oneEnd) return End;
        else if (End == oneEnd) return Start;
        else return Vector3I.Zero ;
    }
}
