using Godot;
using System;

[GlobalClass]
public partial class SceneSaver : Node
{
    // DATA //
    [Export] private string savedSceneName;


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
