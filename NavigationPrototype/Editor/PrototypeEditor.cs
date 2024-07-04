using Godot;
using Godot.Collections;
using System;
using System.Diagnostics.Tracing;

[GlobalClass]
public partial class PrototypeEditor : Node
{
    // DATA //
    // Scene References
    [Export] private Camera3D camera;
    [Export] private Cursor cursor;
    [Export] private NavGraph navGraph;

    // Editor Configs
    [Export] private int maxPlacementDistance = 100;
    [Export] private string savedSceneName = "GraphScene";

    // Cached Data
    private NavNode lastClickedNode = null;


    // FUNCTIONS //
    // Godot Defaults
    public override void _Ready()
    {
        InstructionsUI.instance.AddInstruction("Click on an empty space to add a node.");
        InstructionsUI.instance.AddInstruction("Right click on a node to remove it.");
        InstructionsUI.instance.AddInstruction("Click on two nodes in a row to make a segment between them.");
        InstructionsUI.instance.AddInstruction("Right click to cancel making a segment.");
        InstructionsUI.instance.AddInstruction("Press S to save the scene.");
        base._Ready();
    }

    public override void _Process(double delta)
	{
        cursor.SetNextPos(GetRaycastMousePosition());
        base._Process(delta);
    }

    public override void _UnhandledInput(InputEvent receivedEvent)
    {
        if (receivedEvent is InputEventMouseButton mouseButtonInput)
        {
            if (mouseButtonInput.Pressed)
            {
                // Handle click events - creating new nodes or segments.
                if (mouseButtonInput.ButtonIndex == MouseButton.Left)
                {
                    // Check if clicking ON an existing node - if so, handle segment creation
                    PhysicsRayQueryParameters3D mouseRaycast = Simplifications.CreateMousePosRaycastQuery(camera, maxPlacementDistance);

                    Dictionary raycastResults = camera.GetWorld3D().DirectSpaceState.IntersectRay(mouseRaycast);


                    // If we clicked a NavNode, we try to create a segment
                    if (raycastResults.Values.Count > 0 && Simplifications.IsParentOfType<NavNode>((Node)raycastResults["collider"]))
                    {
                        NavNode clickedNode = (NavNode)((Node)raycastResults["collider"]).GetParent();

                        // If we clicked a node before, create a segment between these. Otherwise, cache.
                        if (lastClickedNode != null)
                        {
                            if (!navGraph.ExistsSegment(lastClickedNode, clickedNode) && lastClickedNode != clickedNode)
                            {
                                NavSegment newSegment = new NavSegment();
                                newSegment.SetStartEnd(lastClickedNode, clickedNode);
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
                        GD.Print("Adding node!");
                        NavNode newNode = new NavNode();
                        newNode.NodePosition = GetMousePosition();
                        navGraph.AddNode(newNode);

                    }
                }

                else if (mouseButtonInput.ButtonIndex == MouseButton.Right)
                {
                    lastClickedNode = null;

                    // Check if clicking ON an existing node - if so, remove it
                    Vector2 mousePosScreen = Simplifications.GetMousePosScreen(this);
                    PhysicsRayQueryParameters3D mouseRaycast = Simplifications.CreateMousePosRaycastQuery(camera, maxPlacementDistance);

                    Dictionary raycastResults = camera.GetWorld3D().DirectSpaceState.IntersectRay(mouseRaycast);


                    // If we clicked a NavNode, we try to create a segment
                    if (raycastResults.Values.Count > 0 && Simplifications.IsParentOfType<NavNode>((Node)raycastResults["collider"]))
                    {
                        NavNode clickedNode = (NavNode)((Node)raycastResults["collider"]).GetParent();
                        navGraph.RemoveNodeAndConnections(clickedNode);
                    }
                }
            }
        }

        else if (receivedEvent is InputEventKey keyboardInput)
        {
            if (keyboardInput.Keycode == Key.S && keyboardInput.IsPressed())
            {
                PackedScene savedScene = new PackedScene();
                savedScene.Pack(GetTree().CurrentScene);
                ResourceSaver.Save(savedScene, $"res://NavigationPrototype/Scenes/{savedSceneName}.tscn");
            }
        }

        // Call baseclass input
        base._UnhandledInput(receivedEvent);
    }


    // Mouse Position 
    private Vector3 GetMousePosition()
    {
        // Gets mouse position in the world
        Vector2 mousePosScreen = Simplifications.GetMousePosScreen(this);
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

    private Vector3 GetRaycastMousePosition()
    {
        Vector2 mousePosScreen = Simplifications.GetMousePosScreen(this);

        PhysicsRayQueryParameters3D mouseRaycast = Simplifications.CreateMousePosRaycastQuery(camera, maxPlacementDistance);

        Dictionary raycastResults = camera.GetWorld3D().DirectSpaceState.IntersectRay(mouseRaycast);

        if (raycastResults.Values.Count > 0 && Simplifications.IsParentOfType<NavNode>((Node)raycastResults["collider"]))
        {
            return (Vector3)raycastResults["position"];
        }
        else
        {
            return GetMousePosition();
        }
    }
}
