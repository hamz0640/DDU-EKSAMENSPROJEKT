using Godot;
using System;

public partial class Turret : CharacterBody2D
{
	[Export] int SpawnHeight = 150;
	[Export] AnimatedSprite2D animator;
	[Export] int Shootspeed = 1;
	bool deployed = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animator.Pause();
		GD.Print("Turret spawned");
		this.Position = new Vector2(this.Position.X, -SpawnHeight);
		//Screenshake

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
		if (deployed)
		{
            animator.Play("shoot");
			// Shoot logic spawn bullet and smth
			BulletSpawn();
            // End shoot logic
            GetTree().CreateTimer(Shootspeed).Timeout += _TimeOutShoot;
        }
	}
    
	static void _on_area_2d_area_entered(Area2D body)
	{
		GD.Print("Body entered! " + body.Name);
	}

    static void _on_area_2d_area_exited(Area2D body)
    {
        GD.Print("Body exited! " + body.Name);
    }

    void BulletSpawn()
    {
        var scene = GD.Load<PackedScene>("res://Scenes/turretBullet.tscn");
        var node = scene.Instantiate();
        AddChild(node);
    }
}
