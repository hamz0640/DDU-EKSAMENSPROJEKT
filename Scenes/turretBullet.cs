using Godot;
using System;

public partial class turretBullet : CharacterBody2D
{
    Vector2 Offset = new Vector2(-6, -0.5f);
    AnimatedSprite2D animation;
    bool IsHit = false;
    Global global = Global.GetInstance();
    float Damage = 5;
    Vector2 Spawn;

    public override void _Ready()
    {
        GlobalPosition = GlobalPosition - Offset;
        animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animation.Play("Bullet");
        Spawn = GlobalPosition;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Velocity = new Vector2(20, 0);
        MoveAndSlide();
        CheckDistance();
    }

    void OnBodyEntered(Node2D body)
    {
        animation.Play("Hit");
        QueueFree();
    }

    void OnAreaEntered(Area2D body)
    {

    }
    float PlayerEnergy()
    {
        return global.GetState<float>("CurrentEnergy");
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
