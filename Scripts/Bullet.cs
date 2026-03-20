using Godot;
using System;

public partial class Bullet : CharacterBody2D
{
	Vector2 Offset = new Vector2(15,5);
	public override void _Ready()
	{
		GlobalPosition = GlobalPosition-Offset;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Velocity = new Vector2(-100, 0);
        MoveAndSlide();
    }
}
