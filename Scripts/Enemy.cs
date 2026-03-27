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
	int Offset;
	
    public override void _Ready()
	{
		Spawn =new Vector2(rnd.Next(700, 800), -10);
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animation.Play("Walking");
		animation.FlipH = true;
		GlobalPosition = Spawn;
		Offset = rnd.Next(0, 100);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Global global = Global.GetInstance();
        if (global.GetState<float>("ShieldHealth") > 0.0)
		{
			EnemyDistance = 500+Offset;
		}
		else
		{
			EnemyDistance =350+Offset;
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
            Velocity = new Vector2(-Speed, 0);
            MoveAndSlide();
        }
        if (GlobalPosition.DistanceTo(Origo) < EnemyDistance && Velocity != new Vector2 (0,0))
		{
            animation.Play("Run&Gun");
            if (animation.Frame == 1)
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
		var scene=GD.Load<PackedScene>("res://Scenes/bullet.tscn");
		var node = scene.Instantiate();
		AddChild(node);
    }

	void EnemyEntered(Node2D body)
	{
		//if (body is Enemy)
		//{
		//	AddCollisionExceptionWith(body);
		//}
	}

}
