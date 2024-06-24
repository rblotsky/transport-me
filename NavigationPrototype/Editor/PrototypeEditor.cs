using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class PrototypeEditor : Node
{
    // DATA //
    // Scene References
    [Export] private Camera3D camera;
    [Export] private Cursor cursor;
    [Export] private NavGraph navGraph;

    // Placement Configs
    [Export] private int maxPlacementDistance = 100;

    // Cached Data
    private NavNode lastClickedNode = null;


    // FUNCTIONS //
    // Godot Defaults
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        cursor.SetNextPos(GetMousePosition());
        base._Process(delta);
    }

    public override void _Input(InputEvent receivedEvent)
    {
        if (receivedEvent is InputEventMouseButton mouseButtonInput)
        {
            if (mouseButtonInput.Pressed)
            {
                // Handle click events - creating new nodes or segments.
                if (mouseButtonInput.ButtonIndex == MouseButton.Left)
                {
                    // Check if clicking ON an existing node - if so, handle segment creation
                    Vector2 mousePosScreen = GetViewport().GetMousePosition();
                    PhysicsRayQueryParameters3D mouseRaycast = PhysicsRayQueryParameters3D.Create(
                        camera.ProjectRayOrigin(mousePosScreen),
                        camera.ProjectRayOrigin(mousePosScreen)+camera.ProjectRayNormal(mousePosScreen) * maxPlacementDistance);

                    Dictionary raycastResults = camera.GetWorld3D().DirectSpaceState.IntersectRay(mouseRaycast);
                    
                    GD.Print("Raycast results:" + raycastResults.ToString());
                    // If we clicked a NavNode, we try to create a segment
                    if(raycastResults.Values.Count > 0)
                    {
                        Debugging.InstantiateCollisionMarkerAsChild(this, Colors.DarkRed, 0.2f, (Vector3)raycastResults["position"]);
                    }

                    if (raycastResults.Values.Count > 0 && Simplifications.IsParentOfType<NavNode>((Node)raycastResults["collider"]))
                    {
                        NavNode clickedNode = (NavNode)((Node)raycastResults["collider"]).GetParent();
                        Debugging.ColourNode(clickedNode, Colors.Red);

                        // If we clicked a node before, create a segment between these. Otherwise, cache.
                        if (lastClickedNode != null)
                        {
                            if (!navGraph.ExistsSegment(lastClickedNode, clickedNode))
                            {
                                NavSegment newSegment = new NavSegment(lastClickedNode, clickedNode);
                                AddChild(newSegment);
                                navGraph.AddSegment(newSegment);
                            }
                            lastClickedNode = null;
                        }
                        else
                        {
                            lastClickedNode = clickedNode;
                        }
                    }

                    // If we didn't click an existing node, we create a new node.
                    else
                    {
                        NavNode newNode = new NavNode(GetMousePosition());
                        AddChild(newNode);
                        navGraph.AddNode(newNode);
                    }
                }
            }
        }

        // Call baseclass input
        base._Input(receivedEvent);
    }


    // Mouse Position 
    private Vector3 GetMousePosition()
    {
        // Gets mouse position in the world
        Vector2 mousePosScreen = GetViewport().GetMousePosition();
        Plane placementPlane = Plane.PlaneXZ;
        Vector3? mousePosWorld =
            placementPlane.IntersectsRay(
            camera.ProjectRayOrigin(mousePosScreen),
            camera.ProjectRayNormal(mousePosScreen));


        // Returns the placement point (at height 0) or a zero vector if mouse position wasn't found
        if (mousePosWorld != null)
        {
            return new Vector3(mousePosWorld.Value.X, 0, mousePosWorld.Value.Z);
        }
        else
        {
            return Vector3.Zero;
        }
    }
}
