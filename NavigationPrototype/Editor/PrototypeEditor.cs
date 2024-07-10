using Godot;
using Godot.Collections;
using System;
using System.Diagnostics.Tracing;

[GlobalClass]
public partial class PrototypeEditor : Node
{
    // DATA //
    // Scene References
    [ExportCategory("References")]
    [Export] private Camera3D camera;
    [Export] private Cursor cursor;
    [Export] private NavGraph navGraph;
    [Export] private Label hologramModeLabel;
    [Export] private WorldGrid grid;

    // Editor Configs
    [ExportCategory("Configs")]
    [Export] private int maxPlacementDistance = 100;
    [Export] private float hologramCatchmentRadius = 1;
    [Export] private Color hologramColour = Colors.Red;
    [Export] private float hologramAlpha = 0.5f;
    [Export] private float rotationIncrement = 15f;

    // Cached Data
    private NavNode lastClickedNode = null;
    private RoadHologram roadHologram;
    private bool hologramModeOn = false;

    // FUNCTIONS //
    // Godot Defaults
    public override void _Ready()
    {
        InstructionsUI.instance.AddInstruction(this, "---------SINGLE NODE MODE--------------");
        InstructionsUI.instance.AddInstruction(this, "Click on an empty space to add a node.");
        InstructionsUI.instance.AddInstruction(this, "Right click on a node to remove it.");
        InstructionsUI.instance.AddInstruction(this, "Click on two nodes in a row to make a segment between them.");
        InstructionsUI.instance.AddInstruction(this, "Right click to cancel making a segment.");
        InstructionsUI.instance.AddInstruction(this, "---------HOLOGRAM MODE--------------");
        InstructionsUI.instance.AddInstruction(this, "Press Q and E to rotate the hologram.");
        InstructionsUI.instance.AddInstruction(this, "Click on an empty space to place nodes according to the hologram.");
        InstructionsUI.instance.AddInstruction(this, "---------GENERIC----------------");
        InstructionsUI.instance.AddInstruction(this, "Press H to toggle between hologram and single-node modes.");
        base._Ready();
    }

    public override void _Process(double delta)
	{
        // If in single node mode, we move the cursor
        if (!hologramModeOn)
        {
            cursor.SetNextPos(GetRaycastMousePosition());
        }
        else
        {
            roadHologram.GlobalPosition = GetRaycastMousePosition();
        }

        grid.SetNextPos(GetRaycastMousePosition());
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
                    if (!hologramModeOn)
                    {
                        HandleNodeOrSegmentPlacement();
                    }
                    else
                    {
                        HandleHologramPlacement();
                    }
                }

                else if (mouseButtonInput.ButtonIndex == MouseButton.Right)
                {
                    if (!hologramModeOn)
                    {
                        HandleNodeRemoval();
                    }
                }
            }
        }

        else if(receivedEvent is InputEventKey keyInput)
        {
            if(keyInput.Keycode == Key.H && keyInput.IsPressed())
            {
                ToggleHologramMode();
            }
            else if(hologramModeOn)
            {
                HandleHologramRotate(keyInput);
            }
        }

        // Call baseclass input
        base._UnhandledInput(receivedEvent);
    }


    // Input Handling
    private void HandleNodeOrSegmentPlacement()
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
                    Simplifications.AddOwnedChild(this, newSegment);
                    newSegment.SetEndpoints(lastClickedNode, clickedNode);
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
            NavNode newNode = new NavNode();
            Simplifications.AddOwnedChild(this, newNode);
            navGraph.AddNode(newNode, SnapPositionToGrid(Simplifications.GetWorldMousePosition(camera)));
        }
    }

    private void HandleNodeRemoval()
    {
        lastClickedNode = null;

        // Check if clicking ON an existing node - if so, remove it
        PhysicsRayQueryParameters3D mouseRaycast = Simplifications.CreateMousePosRaycastQuery(camera, maxPlacementDistance);

        Dictionary raycastResults = camera.GetWorld3D().DirectSpaceState.IntersectRay(mouseRaycast);

        // If we clicked a NavNode, we try to delete it
        if (raycastResults.Values.Count > 0 && Simplifications.IsParentOfType<NavNode>((Node)raycastResults["collider"]))
        {
            NavNode clickedNode = (NavNode)((Node)raycastResults["collider"]).GetParent();
            navGraph.RemoveAndFreeNodeWithConnections(clickedNode);
        }
    }

    private void HandleHologramPlacement()
    {
        roadHologram.InstantiateRealRoad(this, navGraph);
    }

    private void ToggleHologramMode()
    {
        if(hologramModeOn)
        {
            GD.Print("Disabling Hologram!");
            roadHologram.Free();
            EnableCursor();
            hologramModeOn = false;
            hologramModeLabel.Text = "Placement: Single Node";
        }
        else
        {
            GD.Print("Enabling Hologram!");
            CreateRoadHologram();
            DisableCursor();
            hologramModeOn = true;
            hologramModeLabel.Text = "Placement: Hologram";
        }
    }

    private void HandleHologramRotate(InputEventKey keyInput)
    {
        if(keyInput.Keycode == Key.Q && keyInput.IsPressed())
        {
            roadHologram.RotateY(Mathf.DegToRad(rotationIncrement));
        }
        else if (keyInput.Keycode == Key.E && keyInput.IsPressed())
        {
            roadHologram.RotateY(Mathf.DegToRad(-rotationIncrement));
        }
    }


    // Switching hologram and cursor
    private void CreateRoadHologram()
    {
        roadHologram = new RoadHologram();
        roadHologram.CatchmentRadius = hologramCatchmentRadius;
        roadHologram.Colour = hologramColour;
        roadHologram.Alpha = hologramAlpha;
        AddChild(roadHologram);
    }

    private void EnableCursor()
    {
        cursor.SetNextPos(GetRaycastMousePosition());
    }

    private void DisableCursor()
    {
        cursor.SetNextPos(new Vector3(0, -999, 0));
    }


    // Mouse Position 
    private Vector3 GetRaycastMousePosition()
    {
        PhysicsRayQueryParameters3D mouseRaycast = Simplifications.CreateMousePosRaycastQuery(camera, maxPlacementDistance);

        Dictionary raycastResults = camera.GetWorld3D().DirectSpaceState.IntersectRay(mouseRaycast);

        if (raycastResults.Values.Count > 0 && Simplifications.IsParentOfType<NavNode>((Node)raycastResults["collider"]))
        {
            return SnapPositionToGrid((Vector3)raycastResults["position"]);
        }
        else
        {
            return SnapPositionToGrid(Simplifications.GetWorldMousePosition(camera));
        }
    }

    private Vector3 SnapPositionToGrid(Vector3 position)
    {
        return position.Snapped(new Vector3(1, 1, 1));
    }
}
