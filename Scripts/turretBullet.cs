using Godot;
using System;
using System.Linq;

public partial class turretBullet : CharacterBody2D
{
    Vector2 OffsetR = new Vector2(-6, -0.5f);
    Vector2 OffsetL = new Vector2(6, 0.5f);
    AnimatedSprite2D animation;
    bool IsHit = false;
    Global global = Global.GetInstance();
    Vector2 Spawn;
    bool FacingR;

    public override void _Ready()
    {
        animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animation.Play("Bullet");
        GD.Print(FacingR);
        if (FacingR)
        {
            animation.FlipH = false;
            GlobalPosition = GlobalPosition - OffsetR;
        }
        else
        {
            animation.FlipH = true;
            GlobalPosition = GlobalPosition + OffsetL;
        }
        Spawn = GlobalPosition;
    }

    public void CheckFacing(bool FacingR)
    {
        this.FacingR = FacingR;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (FacingR)
            Velocity = new Vector2(220, 0);
        else
            Velocity = new Vector2(-220, 0);
        MoveAndSlide();
        CheckDistance();
    }

    void OnBodyEntered(Node2D body)
    {
        animation.Play("Hit");
        GD.Print($"Bullet hit: {body.Name} | Type: {body.GetType()} | Groups: {string.Join(", ", body.GetGroups())}");
        

        if (body is Enemy enemy)
        {
            GD.Print("Enemy hit!");
            enemy.TakeDamage(1);
            QueueFree();
        }
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
            QueueFree();
    }
}
