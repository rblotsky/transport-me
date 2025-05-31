using Godot;
using System;
using System.ComponentModel.Design;

[GlobalClass]
[Tool]
public partial class NavSegment : Node3D
{
    // DATA
    // Serializable Properties
    private Vector3 _start = Vector3.Zero;
    [Export] public Vector3 Start { get { return _start; } set { _start = value; UpdateVisualization(); } }
    private Vector3 _end = Vector3.Zero;
    [Export] public Vector3 End { get { return _end; } set { _end = value; UpdateVisualization(); } }
    private Vector3 _control = Vector3.Zero;
    [Export] public Vector3 Control { get { return _control; } set { _control = value; UpdateVisualization(); } }
    [Export] public float MaxSpeed = 30f;
    
    // Readonly Properties
    public Vector3 GlobalStart { get { return ToGlobal(Start); } }
    public Vector3 GlobalEnd { get { return ToGlobal(End); } }
    public Vector3 GlobalControl { get { return ToGlobal(Control); } }
    public Vector3[] Endpoints { get { return new Vector3[2] { Start, End}; } }
    public Vector3[] GlobalEndpoints { get { return new Vector3[2] { GlobalStart, GlobalEnd }; } }
    public Vector3 DirectionalLine { get { return End - Start; }}
    public float SimpleLength { get { return DirectionalLine.Length(); } }
    public float Length { get { return SimpleLength; } }

    // Runtime only properties
    public NavConnection EndConnection { get; set; }
    public NavConnection StartConnection { get; set; }

    // Editor Cached Data
    private MeshInstance3D curveVisualizer;
    private MeshInstance3D endpointVisualizer;
    private MeshInstance3D endpointDirectionVisualizer;
    private MeshInstance3D directionVisualizer;
    private MeshInstance3D controlVisualizer;


    // FUNCTIONS //
    // Godot Defaults
    public override void _Ready()
    {
        // In editor, run visualization
        if (Engine.IsEditorHint())
        {
            UpdateVisualization();
        }
        base._EnterTree();
    }

    public override void _ExitTree()
    {
        if(Engine.IsEditorHint())
        {
            RemoveVisualizers();
        }

        RequestReady();

        base._ExitTree();
    }


    // Data Retrieval
    public Vector3 GetOtherEndLocal(Vector3 oneEnd)
    {
        if (Start == oneEnd) return End;
        else if (End == oneEnd) return Start;
        else return Vector3.Zero ;
    }

    public Vector3 GetOtherEndGlobal(Vector3 oneEnd)
    {
        if (GlobalStart == oneEnd) return GlobalEnd;
        else if (GlobalEnd == oneEnd) return GlobalStart;
        else return Vector3.Zero;
    }

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
        if (index == 0) Start = value;
        else if (index == 1) Control = value;
        else if (index == 2) End = value;
    }

    public Vector3 GetPositionOnSegment(float percentOfSegment, bool globalCoordinates = true)
    {
        Vector3 localPos = Curves.BezierQuadratic3D(
            Start,
            Curves.Vec3RemoveHeight(Control),
            End,
            percentOfSegment
            );
        return globalCoordinates ? ToGlobal(localPos) : localPos;
    }


    // Extra Setters
    public void SetEndpoint(int endpoint, Vector3 value)
    {
        if(endpoint == 0)
        {
            Start = value;
        }
        else if(endpoint == 1)
        {
            End = value;
        }
        else
        {
            throw new ArgumentException($"Expected an endpoint of 0 or 1 but got {endpoint}!");
        }
    }


    // Visualization
    private void RemoveVisualizers()
    {
        if (curveVisualizer != null)
        {
            curveVisualizer.Free();
            curveVisualizer = null;
        }

        if (endpointVisualizer != null)
        {
            endpointVisualizer.Free();
            endpointVisualizer = null;
        }

        if(endpointDirectionVisualizer != null)
        {
            endpointDirectionVisualizer.Free();
            endpointDirectionVisualizer = null;
        }

        if(directionVisualizer != null)
        {
            directionVisualizer.Free();
            directionVisualizer = null;
        }

        if(controlVisualizer != null)
        {
            controlVisualizer.Free();
            controlVisualizer = null;
        }
    }

    private void UpdateVisualization()
    {
        // Only runs in editor
        if(Engine.IsEditorHint() && IsNodeReady())
        {
            // Removes and creates new visualizers
            RemoveVisualizers();
            curveVisualizer = new MeshInstance3D();
            AddChild(curveVisualizer);
            endpointVisualizer = new MeshInstance3D();
            AddChild(endpointVisualizer);
            endpointDirectionVisualizer = new MeshInstance3D();
            AddChild(endpointDirectionVisualizer);
            directionVisualizer = new MeshInstance3D();
            AddChild(directionVisualizer);
            controlVisualizer = new MeshInstance3D();
            AddChild(controlVisualizer);

            
        }
    }

    public void DebugPrint()
    {
        GD.PrintT("START", GlobalStart.ToString(), "END", GlobalEnd.ToString());
    }
}
