using Godot;
using System;

[GlobalClass]
public partial class SceneSaver : Node
{
    // DATA //
    [Export] private string savedSceneName;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        InstructionsUI.instance.AddInstruction(this, "Press S to save the scene.");
    }

    public override void _UnhandledInput(InputEvent receivedEvent)
    {
        if (receivedEvent is InputEventKey keyboardInput)
        {
            if (keyboardInput.Keycode == Key.S && keyboardInput.IsPressed())
            {
                PackedScene savedScene = new PackedScene();
                savedScene.Pack(GetTree().CurrentScene);
                ResourceSaver.Save(savedScene, $"res://NavigationPrototype/Scenes/{savedSceneName}.tscn");
            }
        }

        base._UnhandledInput(receivedEvent);
    }
}
