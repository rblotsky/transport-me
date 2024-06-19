using Godot;
using System;

[GlobalClass]
public partial class PrototypeEditor : Node
{
    // DATA //
    // Scene References
    [Export] private Camera3D camera;
    [Export] private Cursor cursor;
    [Export] private NavGraph navGraph;

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
            // If scroll wheel, zoom
            if (mouseButtonInput.Pressed)
            {
                if (mouseButtonInput.ButtonIndex == MouseButton.Left)
                {
                    GD.Print("Started");
                    NavNode newNode = new NavNode(GetMousePosition());
                    GD.Print("Instantiated");
                    AddChild(newNode);
                    GD.Print("Added");
                    navGraph.AddNode(newNode);
                    GD.Print("Added to NavGraph");
                }
            }
        }

        // Call baseclass input
        base._Input(receivedEvent);
    }


    // FUNCTIONS //
    // Cursor
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
