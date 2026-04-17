using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Turret : CharacterBody2D
{
	[Export] int SpawnHeight = 150;
	[Export] AnimatedSprite2D animator;
	[Export] int Shootspeed = 1;
	bool deployed = false;
	int EnemiesWithinR = 0;
	int EnemiesWithinL = 0;
	bool FacingR = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animator.Pause();
		GD.Print("Turret spawned");
		this.Position = new Vector2(100, -SpawnHeight);
		//Screenshake

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch (FacingR)
		{
			case true:
				animator.FlipH = false;
				break;
			case false:
				animator.FlipV = true;
				break;
		} // Fix turret orientation

        Vector2 velocity = Velocity;
        if (!IsOnFloor())
		{
            velocity += GetGravity() * (float)delta;
            Velocity = velocity;
        }
		else
		{
			if (!deployed)
			{
				deployed = true;
				GetTree().CreateTimer(3).Timeout += _DeployAnimation;
            }
		}
        MoveAndSlide();
    }

	private void _DeployAnimation()
	{
        animator.Play("deploy");
        GetTree().CreateTimer(2).Timeout += _TimeOutShoot;
    }
	
	private void _TimeOutShoot()
	{
		if(!FacingR && EnemiesWithinR > 0 && EnemiesWithinL <= 0)
			FacingR = true;
		if (FacingR && EnemiesWithinL > 0 && EnemiesWithinR <= 0)
			FacingR = false;

		if (EnemiesWithinR > 0 || EnemiesWithinL > 0)
		{
            animator.Play("shoot");
			// Shoot logic spawn bullet and smth
			BulletSpawn();
            // End shoot logic
            GetTree().CreateTimer(Shootspeed).Timeout += _TimeOutShoot;
        }
		else
		{
			animator.Play("idle");
            GetTree().CreateTimer(Shootspeed).Timeout += _TimeOutShoot;
        }
	}
    
	void _on_area_2d_body_entered(Node2D body)
	{
		if (body is CharacterBody2D character)
		{
			if (body.Position.X < 0)
				EnemiesWithinR++;
			else
				EnemiesWithinL++;
		}
	}

    void _on_area_2d_body_exited(Node2D body)
    {
        if (body is CharacterBody2D character)
        {
            if (body.Position.X < 0)
                EnemiesWithinR--;
            else
                EnemiesWithinL--;
        }
    }

    void BulletSpawn()
    {
        var scene = GD.Load<PackedScene>("res://Scenes/turretBullet.tscn");
        var node = scene.Instantiate();
        AddChild(node);
    }
}
