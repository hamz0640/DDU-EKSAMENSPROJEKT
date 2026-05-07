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
    }

    private void OnArea2DBodyEntered(Node2D body)
    {
        if (body is Enemy enemy)
        {
            enemy.TakeDamage(Damage);
            QueueFree();
        }
    }
}