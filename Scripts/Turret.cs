using Godot;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class Turret : CharacterBody2D
{
	[Export] int SpawnHeight = 150;
	[Export] AnimatedSprite2D animator;
	[Export] int Shootspeed = 1;
	[Export] RichTextLabel Label;
	[Export] int Bullets = 300;
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
		Label.Visible = false;
		Label.Text = Bullets.ToString();

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
				animator.FlipH = true;
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
		Label.Visible = true;
        UpdateEnemieDirection(); // Tjek om enemies er til højre eller venstre
		GD.Print($"Enemies left:{EnemiesWithinL}, Enemies right: {EnemiesWithinR}");
		if (!FacingR && EnemiesWithinR >= 1 && EnemiesWithinL < 1)
		{
			FacingR = true;
            GetTree().CreateTimer(Shootspeed).Timeout += _TimeOutShoot;
			goto Earlyexit;
        }
		if (FacingR && EnemiesWithinR < 1 && EnemiesWithinL >= 1)
		{
			FacingR = false;
            GetTree().CreateTimer(Shootspeed).Timeout += _TimeOutShoot;
            goto Earlyexit;
        }

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
	Earlyexit:;
		Label.Text = Bullets.ToString();
	}

    private void UpdateEnemieDirection()
    {
        var Enemies = GetTree().GetNodesInGroup("Enemy").OfType<Enemy>();
		EnemiesWithinR = 0;
		EnemiesWithinL = 0;
        foreach (Enemy enmy in Enemies)
        {
			if (enmy.GlobalPosition.X > this.GlobalPosition.X)
				EnemiesWithinR++; // På højre side
			else
				EnemiesWithinL++;
        }
    }

    void BulletSpawn()
    {
		Bullets--;
        var scene = GD.Load<PackedScene>("res://Scenes/turretBullet.tscn");
        var node = scene.Instantiate();
		if(node is turretBullet bl && Bullets > 0)
		{
			bl.CheckFacing(FacingR);
		}
		else
		{
			QueueFree();
		}
			AddChild(node);
    }
}
