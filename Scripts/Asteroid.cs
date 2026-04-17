using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class Asteroid : Bullet
{
    [Export] Vector2 BasePosition = new Vector2(105, -70);
    [Export] Sprite2D Sprite;
    int RotationSpeed;
    float Speed;
    Global global = Global.GetInstance();
    Random rnd = new Random();
    int Damage = 20;

    public override void _Ready()
    {
        // Dette kører koden inde i Bullet scriptet
        base._Ready();
        GD.Print("Asteroid spawned");

        Tracker tracker = Tracker.GetInstance();
        tracker.IncrementTracking("Wave:AsteroidsSpawned", 1u);

        this.Position = new Vector2(rnd.Next(-1200, 1200), -1000);

        RotationSpeed = rnd.Next(2, 10);
        Speed = rnd.Next(40, 100);

    }

    public override void OnHit(Node2D hit)
    {
        if (hit is TileMapLayer)
        {
            GD.Print("Asteroid hit ground");
            QueueFree();
        }

        if (hit is ChargingZone)
        {
            Tracker tracker = Tracker.GetInstance();
            tracker.IncrementTracking("Wave:AsteroidsHitShield", 1u);
            tracker.IncrementTracking("Wave:ShieldDamageTaken",  Damage);

            GD.Print("Asteroid hit Charging Zone");
            global.SetState<float>("ShieldHealth", ShieldEnergy() - Damage);

            QueueFree();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Sprite.Rotation += RotationSpeed * (float)delta;
        this.GlobalPosition = this.GlobalPosition.MoveToward(BasePosition, Speed*(float)delta);

        MoveAndSlide();
    }

    float PlayerEnergy()
    {
        return global.GetState<float>("CurrentEnergy");
    }

    float ShieldEnergy()
    {
        return global.GetState<float>("ShieldHealth");
    }

}
