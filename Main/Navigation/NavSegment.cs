using Godot;
using System;

[GlobalClass]
[Tool]
public partial class NavSegment : Node3D
{
    // TODO refactor curved segments into a subclass
    // DATA
    // Serializable Properties
    private Vector3 _start = Vector3.Zero;
    [Export] private Vector3 Start { get { return _start; } set { _start = value; UpdateVisualization(); } }
    private Vector3 _end = Vector3.Zero;
    [Export] private Vector3 End { get { return _end; } set { _end = value; UpdateVisualization(); } }
    private Vector3 _control = Vector3.Zero;
    [Export] private Vector3 Control { get { return _control; } set { _control = value; UpdateVisualization(); } }
    [Export(PropertyHint.Flags, "Pedestrian,Automobile,Tram,Bus,Train")] private int allowedVehicleTypes;
    
    // Readonly Properties
    public Vector3 GlobalStart { get { return ToGlobal(Start); } }
    public Vector3 GlobalEnd { get { return ToGlobal(End); } }
    public Vector3 GlobalControl { get { return ToGlobal(Control); } }
    public Vector3[] Endpoints { get { return new Vector3[2] { Start, End}; } }
    public Vector3[] GlobalEndpoints { get { return new Vector3[2] { GlobalStart, GlobalEnd }; } }
    public Vector3 DirectionalLine { get { return End - Start; }}
    public float SimpleLength { get { return DirectionalLine.Length(); } }
    public float Length { get { return SimpleLength; } }

    public NavConnection InboundConnection { get; set; }
    public NavConnection OutboundConnection { get; set; }

    // Editor Cached Data
    private MeshInstance3D curveVisualizer;
    private MeshInstance3D endpointVisualizer;
    private MeshInstance3D endpointDirectionVisualizer;
    private MeshInstance3D directionVisualizer;


    // FUNCTIONS //
    // Godot Defaults
    public override void _EnterTree()
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

    public Vector3 GetPositionOnSegment(float percentOfSegment, bool globalCoordinates = true)
    {
        Vector3 localPos = Curves.CalculateBezierQuadraticWithHeight(
            Start,
            Control,
            End,
            percentOfSegment
            );
        return globalCoordinates ? ToGlobal(localPos) : localPos;
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
    }

    private void UpdateVisualization()
    {
        // Only runs in editor
        if(Engine.IsEditorHint())
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
            AddChild (directionVisualizer);
            


            curveVisualizer.Mesh = EasyShapes.CurveMesh(Start, End, Control, Colors.LightBlue, 10);
            endpointVisualizer.Position = End;
            endpointVisualizer.Mesh = EasyShapes.SphereMesh(0.1f, EasyShapes.ColouredMaterial(Colors.Red, 0.5f));
            endpointDirectionVisualizer.Mesh = EasyShapes.SphereMesh(0.08f, EasyShapes.ColouredMaterial(Colors.HotPink, 0.5f));
            endpointDirectionVisualizer.Position = Curves.CalculateBezierQuadraticWithHeight(Start, Control, End, 0.99f);
            directionVisualizer.Mesh = EasyShapes.TrianglePointerMesh(Colors.Red, 0.2f);
            directionVisualizer.LookAtFromPosition(GlobalStart, GlobalEnd);
            directionVisualizer.Position = GetPositionOnSegment(0.5f, false);


        }
    }

    public void DebugPrint()
    {
        GD.PrintT("START", GlobalStart.ToString(), "END", GlobalEnd.ToString());
    }
}
