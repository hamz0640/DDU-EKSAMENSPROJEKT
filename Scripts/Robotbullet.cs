using Godot;
using System;

public partial class Robotbullet : CharacterBody2D
{
    [Export] public float Speed = 100f;
    [Export] public int Damage = 1;
    public float Direction; // 1 = højre, -1 = venstre

    public override void _PhysicsProcess(double delta)
    {
        Velocity = new Vector2(Speed * Direction, 0);
        MoveAndSlide();
    }

    public override void _Ready()
    {
        var area = GetNode<Area2D>("Area2D");
        area.BodyEntered += OnArea2DBodyEntered;
        
        GD.Print("=== ROBOTBULLET DEBUG ===");
        GD.Print("Robotbullet Layer: " + CollisionLayer);
        GD.Print("Robotbullet Mask: " + CollisionMask);
        GD.Print("Area2D Layer: " + area.CollisionLayer);
        GD.Print("Area2D Mask: " + area.CollisionMask);
    }

    private void OnArea2DBodyEntered(Node2D body)
    {
        GD.Print("Hit: " + body.Name + " type: " + body.GetType());

        if (body is Enemy enemy)
        {
            GD.Print("Enemy hit!");
            enemy.TakeDamage(Damage);
            QueueFree();
        }
    }
}