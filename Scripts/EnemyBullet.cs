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
		if (body is PlayerController)
		{
			Tracker tracker = Tracker.GetInstance();
			tracker.IncrementTracking("Wave:ShieldDamageTaken", Damage);

			animation.Play("Hit");

			global.SetState<float>("CurrentEnergy",PlayerEnergy()-Damage);

			GD.Print(ShipHealth());
			QueueFree();
				
		}
	}

	void OnAreaEntered(Area2D area)
	{
		if (area.Name == "ChargingZone" && ShieldEnergy() > 0)
		{
			global.SetState<float>("CurrentShieldHealth", ShieldEnergy() - Damage);
			QueueFree();
		}
		else if (area.Name == "ShipHitbox")
		{
			global.SetState<float>("CurrentShipHealth", ShipHealth() - Damage);
			GD.Print("Ship hit");
			QueueFree();
		}
	}
	float ShipHealth()
	{
		return global.GetState<float>("CurrentShipHealth");
	}

	float PlayerEnergy()
	{
		return global.GetState<float>("CurrentEnergy");
	}

	float ShieldEnergy()
	{
		return global.GetState<float>("CurrentShieldHealth");
	}

	void CheckDistance()
	{
		if (GlobalPosition.DistanceTo(Spawn) > 2000)
		{
			QueueFree();
		}
	}

	
}
