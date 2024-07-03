using Godot;
using System;

[GlobalClass]
public partial class InstructionsUI : Node
{
    // DATA //
    // Scene References
    [Export] public Container instructionsContainer;
    [Export] public FlowContainer instructions;

    // Singleton Pattern
    public static InstructionsUI instance;


    // FUNCTIONS //
    // Godot Defaults
    public override void _EnterTree()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            QueueFree();
        }

        base._EnterTree();
    }


    // Usage
    public void AddInstruction(string text)
    {
        Label newLabel = new Label();
        newLabel.Text = text;

        instructions.AddChild(newLabel);
    }

    // Signals
    // This is connected to a Signal from the menu button. It runs when the menu button is pressed.
    private void OnButtonPressed()
    {
        if(instructionsContainer.Visible)
        {
            instructionsContainer.Hide();
        }
        else
        {
            instructionsContainer.Show();
        }
    }
}
