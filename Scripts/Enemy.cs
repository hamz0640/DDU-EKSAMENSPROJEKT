using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	Random rnd=new Random();
    AnimatedSprite2D animation;
	public float Speed = 30.0f;
	public int EnemyDistance;
	Vector2 Origo = new Vector2(0, 0);
	Vector2 Spawn;
	private bool hasShot = false;
	[Export] public double MaxHealth = 3;
	private double currentHealth;
	int Offset;
	
    public override void _Ready()
	{
		currentHealth = MaxHealth;
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animation.Play("Walking");
		animation.FlipH = Speed < 0;
		// Spawn = new Vector2(rnd.Next(700, 800), -10);
		// GlobalPosition = Spawn;
		// Offset = rnd.Next(0, 60);
	}

	public override void _Process(double delta)
	{
        Global global = Global.GetInstance();
        if (global.GetState<float>("ShieldHealth") > 0.0)
		{
			EnemyDistance = 400+Offset;
		}
		else
		{
			EnemyDistance =330+Offset;
		}
		
		if (GlobalPosition.DistanceTo(Origo) < EnemyDistance)
		{
			Velocity = new Vector2( 0 , 0 );
			animation.Play("Shooting");
			if (animation.Frame == 1)
			{
                if (!hasShot)
				{
					BulletSpawn();
					hasShot = true;
				}
            } else
			{
				hasShot = false;
			}
		}
		else
		{
            Velocity = new Vector2(Speed, 0);
            MoveAndSlide();
        }
        if (GlobalPosition.DistanceTo(Origo) > EnemyDistance)
		{
            animation.Play("Run&Gun");
            if (animation.Frame == 1 || animation.Frame == 5)
            {
                if (!hasShot)
                {
                    BulletSpawn();
                    hasShot = true;
                }
            }
            else
            {
                hasShot = false;
            }
        }
    }

	void BulletSpawn()
	{
		var scene=GD.Load<PackedScene>("res://Scenes/enemy_bullet.tscn");
		var node = scene.Instantiate<EnemyBullet>();
		if (Speed > 0)
        	node.Direction = Vector2.Right;
    	else
        	node.Direction = Vector2.Left;

		GetParent().AddChild(node);
    	node.GlobalPosition = GlobalPosition;
    }

	public void TakeDamage(double damage)
	{
		currentHealth -= damage;

		if (currentHealth <= 0)
		{
			QueueFree();
		}
	}
	public void SetSpawnSide(bool spawnRight)
	{
		if (spawnRight)
		{
			Spawn = new Vector2(rnd.Next(700, 800), -10);
			Speed = -Math.Abs(Speed); // move left
		}
		else
		{
			Spawn = new Vector2(rnd.Next(-800, -700), -10);
			Speed = Math.Abs(Speed); // move right
		}
	}

}
