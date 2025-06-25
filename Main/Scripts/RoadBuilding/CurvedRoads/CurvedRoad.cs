using Godot;
using Godot.Collections;
using System;

[GlobalClass]
[Tool]
public partial class CurvedRoad : MeshInstance3D
{
    // DATA //
    // Graph data
    [ExportCategory("Road Shape")]
    private Vector3 _start = new Vector3(1,0,0);
    [Export] public Vector3 Start { get { return _start; } set { _start = value; } }
    private Vector3 _end = new Vector3(-1, 0, 0);
    [Export] public Vector3 End { get { return _end; } set { _end = value; } }
    private Vector3 _control = Vector3.Zero;
    [Export] public Vector3 Control { get { return _control; } set { _control = value ; } }

    // Properties
    public Vector3 ControlAtStartHeight { get { return new Vector3(Control.X, Start.Y, Control.Z); } }
    public Vector3 ControlAtEndHeight { get { return new Vector3(Control.X, End.Y, Control.Z); } }
    public Vector3 ControlAtAvgHeight { get { return new Vector3(Control.X, (Start.Y + End.Y) / 2, Control.Z);  } }
    public Vector3 DirectionalLine { get { return End - Start; } }
    /// <summary>
    /// A transform of the Start. Origin = Start, Basis = facing Control from Start
    /// </summary>
    public Transform3D StartTransform { get { return new Transform3D(Basis.LookingAt(Start - ControlAtStartHeight), Start); } }

    /// <summary>
    /// A transform of the End. Origin = End, Basis = facing End from Control
    /// </summary>
    public Transform3D EndTransform { get { return new Transform3D(Basis.LookingAt(ControlAtEndHeight - End), End); } }


    // Road Mesh Data
    [ExportCategory("Mesh")]
    [Export] private RoadMesh roadMesh;
    [Export] private bool debugNormals = false;


    // FUNCTIONS //
    public override void _EnterTree()
    {
        if (!Engine.IsEditorHint())
        {
            Mesh = GetDisplayMesh();
        }
    }


    // Controlling
    /// <summary>
    /// Gets one of the start, control, or end points by its index.
    /// </summary>
    /// <param name="index">0 = Start, 1 = Control, 2 = End</param>
    /// <returns>The local value of the requested point</returns>
    public Vector3 GetPointByIndex(int index)
    {
        if (index == 0) return Start;
        else if (index == 1) return Control;
        else if (index == 2) return End;
        else return Vector3.Zero;
    }

    /// <summary>
    /// Sets one of the start, control, or end points by its index.
    /// </summary>
    /// <param name="index">0 = Start, 1 = Control, 2 = End</param>
    /// <param name="value">The Vector3 value of the point</param>
    public void SetPointByIndex(int index, Vector3 value)
    {
        if (index == 0) Start = value ;
        else if (index == 1) Control = value;
        else if (index == 2) End = value;
    }

    // Visualization
    public Mesh GetDisplayMesh()
    {
        if (roadMesh != null)
        {
            // If debug normals enabled, uses the normals mesh instead
            if (debugNormals)
            {
                return roadMesh.GenerateRoadNormalsMesh(this);
            }
            else 
            { 
                return roadMesh.GenerateRoadMesh(this); 
            }
        }

        return null;
    }
}
