using Godot;
using System;

public partial class EnemyBullet : CharacterBody2D
{
	Vector2 Offset = new Vector2(15,5);
    AnimatedSprite2D animation;
	bool IsHit = false;
	Global global = Global.GetInstance();
	float Damage = 5;
	Vector2 Spawn;

	public Vector2 Direction = Vector2.Left;

    public override void _Ready()
	{
		GlobalPosition = GlobalPosition-Offset;
        animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animation.Play("Bullet");
		Spawn = GlobalPosition;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    	Velocity = Direction * 200;
        MoveAndSlide();
		CheckDistance();
    }

	void OnBodyEntered(Node2D body)
	{
		if (body.Name == "ChargingZone" || body is PlayerController)
		{
			animation.Play("Hit");

			global.SetState<float>("CurrentShipHealth",ShipHealth()-Damage);

			GD.Print(ShipHealth());
			QueueFree();
				
		}
	}

	void OnAreaEntered(Area2D body)
	{
		if (body is ChargingZone && ShieldEnergy() > 0.0)
		{
            global.SetState<float>("ShieldHealth", ShieldEnergy() - Damage);
			GD.Print(ShieldEnergy());

            QueueFree();
            
            GD.Print("ChargingZone Hit");
            
        }
	}
	float ShipHealth()
	{
		return global.GetState<float>("CurrentShipHealth");
	}

	float ShieldEnergy()
	{
		return global.GetState<float>("ShieldHealth");
	}

	void CheckDistance()
	{
		if (GlobalPosition.DistanceTo(Spawn) > 2000)
		{
			QueueFree();
		}
	}

	
}
