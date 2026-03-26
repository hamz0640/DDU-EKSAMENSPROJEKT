using Godot;
using System;

public partial class Bullet : CharacterBody2D
{
	Vector2 Offset = new Vector2(15,5);
    AnimatedSprite2D animation;
	bool IsHit = false;
    public override void _Ready()
	{
		GlobalPosition = GlobalPosition-Offset;
        animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animation.Play("Bullet");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Velocity = new Vector2(-100, 0);
        MoveAndSlide();
    }

	void OnBodyEntered(Node2D body)
	{
		if (body.Name == "ChargingZone" || body is PlayerController)
		{
			QueueFree();
				
		}
	}

	void OnAreaEntered(Area2D body)
	{
		if (body is ChargingZone)
		{
			QueueFree();
            
            GD.Print("ChargingZone Hit");
            
        }
	}
	
}
