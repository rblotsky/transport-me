using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
[Tool]
public partial class GraphDisplayer : Node
{
    // DATA //
    // Configs
    [ExportCategory("Configuration")]
    [Export] private Color dataNodeColour = Colors.Blue;
    [Export] private Color connectNodeColour = Colors.Red;
    [Export] private Color segmentColour = Colors.RebeccaPurple;
    [Export] private int segmentResolution = 5;
    [Export] private float nodeRadius = 0.2f;

    // Toggle
    [ExportCategory("Toggle Display")]
    private bool _displayToggle = false;
    [Export] public bool DisplayToggle { get { return _displayToggle; } set { _displayToggle = value; ToggleDisplay(value); } }


    // FUNCTIONS //
    private void ToggleDisplay(bool status)
    {
        if(status)
        {
            CreateDisplay();
        }
        else
        {
            DeleteDisplay();
        }
    }

    private void CreateDisplay()
    {
        // First, gets all available PloppableGraphs to display.
        PloppableGraph[] containers = Simplifications.GetChildrenOfType<PloppableGraph>(GetParent(), true).ToArray();

        foreach(PloppableGraph container in containers)
        {
            // Adds each container's nodes to the world.
            foreach(NavSegment segment in container.segments)
            {
                NodeMesh(segment.Start, container);
                NodeMesh(segment.End, container);
            }
        }
    }

    private void NodeMesh(Vector3 position, PloppableGraph ploppable)
    {
        MeshInstance3D mesh = EasyShapes.AddShapeMesh(
            this, 
            EasyShapes.SphereMesh(
                nodeRadius, 
                EasyShapes.ColouredMaterial(connectNodeColour, 1)
                ), 
            false
            );
        mesh.GlobalPosition = ploppable.GlobalPosition + position;
    }

    private void DeleteDisplay()
    {
        // Deletes all its children as they are meshes to display the graph.
        // This is NOT RECURSIVE - There should be no recursive children.
        List<Node> children = Simplifications.GetChildrenOfNode(this, false);
        foreach(Node child in children)
        {
            child.QueueFree();
        }
    }
}
