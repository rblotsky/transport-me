using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class InstructionsUI : Node
{
    // DATA //
    // Scene References
    [Export] public Control panel;
    [Export] public Container instructions;

    // Cached Data
    private List<(Node nodeUsed, string instruction)> instructionsByNode = new List<(Node nodeUsed, string instruction)>();

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
            Free();
        }

        base._EnterTree();
    }


    // Usage
    public void AddInstruction(Node nodeUsed, string text)
    {
        Label newLabel = new Label();
        newLabel.Text = text;

        instructions.AddChild(newLabel);
        instructionsByNode.Add((nodeUsed, text));
    }

    public void RemoveInstructionsForNode(Node nodeUsed)
    {
        instructionsByNode.RemoveAll(item => { return item.nodeUsed == nodeUsed; });
    }


    // Signals
    // This is connected to a Signal from the menu button. It runs when the menu button is pressed.
    private void OnButtonPressed()
    {
        if(panel.Visible)
        {
            panel.Hide();
        }
        else
        {
            panel.Show();
        }
    }
}
