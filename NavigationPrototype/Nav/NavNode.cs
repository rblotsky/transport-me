using Godot;
using Godot.Collections;
using System.Linq;

[GlobalClass]
public partial class NavNode : Node3D
{
    // DATA //
    [Export] public float NodeRadius { get; set; }

    [Export] public Array<NavSegment> attachedSegments = new Array<NavSegment>();
    public NavSegment[] StartingSegments
    {
        get
        {
            return attachedSegments.Where((NavSegment segment) => { return segment.Start == this; }).ToArray();
        }
    }
    public NavSegment[] EndingSegments
    {
        get
        {
            return attachedSegments.Where((NavSegment segment) => { return segment.End == this; }).ToArray();
        }
    }


    // FUNCTIONS //
    // Checks
    public bool ConnectedTo(NavNode otherNode, bool considerDirection = true)
    {
        // Returns whether any segments go from this node to the other node
        if(considerDirection)
        {
            return StartingSegments.Where((NavSegment segment) => { return segment.End == otherNode; }).Count() > 0;
        }
        else
        {
            return attachedSegments.Where((NavSegment segment) => { return segment.Endpoints.Contains(otherNode); }).Count() > 0;
        }
    }


    // Managing Structure
    public void ConnectSegment(NavSegment segment)
    {
        attachedSegments.Add(segment);
    }

    public void DisconnectSegment(NavSegment segment)
    {
        attachedSegments.Remove(segment);
    }

    public void DisconnectFromSegments()
    {
        attachedSegments.Clear();
    }


    // Managing Display
    public void CreatePhysicalRepresentation()
    {
        // Gives itself a mesh and collider
        EasyShapes.AddGenericShapeMesh(this, EasyShapes.CreateSphereMesh(NodeRadius), true);
        EasyShapes.AddGenericShapeCollider(this, EasyShapes.CreateSphereShape(NodeRadius), true);
    }

    public void RemovePhysicalRepresentation()
    {
        Simplifications.FreeOwnedNode(Simplifications.GetFirstChildOfType<MeshInstance3D>(this));
        Simplifications.FreeOwnedNode(Simplifications.GetFirstChildOfType<StaticBody3D>(this));
    }
}
