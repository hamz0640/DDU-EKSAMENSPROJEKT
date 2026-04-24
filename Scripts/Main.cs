using Godot;
using System;

public partial class Main : Node2D
{
	// Called when the node enters the scene tree for the first time.
public override void _Ready()
{
    GD.Print("Tutorial check running");
    
    var scene = GD.Load<PackedScene>("res://Tutorial/TutorialOverlay.tscn");
    if (scene == null)
    {
        GD.PrintErr("TUTORIAL: Could not load scene!");
        return;
    }
    
    var instance = scene.Instantiate();
    GetTree().CurrentScene.AddChild(instance);
    GD.Print("Tutorial added to scene tree");
}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	
}
	
	}

