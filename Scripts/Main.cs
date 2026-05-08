using Godot;
using System;

public partial class Main : Node2D
{
	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
        var scene = GD.Load<PackedScene>("res://Tutorial/tutorial_overlay.tscn");
        if (scene == null)
        {
            return;
        }
        
        var instance = scene.Instantiate();
        GetTree().CurrentScene.AddChild(instance);
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        
    }
}

