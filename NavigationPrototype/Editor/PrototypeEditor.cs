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
    [Export] private WorldGrid grid;
    [Export] private Label hologramModeLabel;

    // Editor Configs
    [ExportCategory("Configs")]
    [Export] private int maxPlacementDistance = 100;

    // Cached Data
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

        grid.SetNextPos(GetRaycastMousePosition());
        base._Process(delta);
    }

    public override void _UnhandledInput(InputEvent receivedEvent)
    {
        if(receivedEvent is InputEventKey keyInput)
        {
            if(keyInput.Keycode == Key.H && keyInput.IsPressed())
            {
                ToggleHologramMode();
            }
        }

        // Call baseclass input
        base._UnhandledInput(receivedEvent);
    }

    private void ToggleHologramMode()
    {
        if(hologramModeOn)
        {
            GD.Print("Disabling Hologram!");
            EnableCursor();
            hologramModeOn = false;
            hologramModeLabel.Text = "Placement: Single Node";
        }
        else
        {
            GD.Print("Enabling Hologram!");
            DisableCursor();
            hologramModeOn = true;
            hologramModeLabel.Text = "Placement: Hologram";
        }
    }


    // Switching hologram and cursor

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

        if (raycastResults.Values.Count > 0)
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
