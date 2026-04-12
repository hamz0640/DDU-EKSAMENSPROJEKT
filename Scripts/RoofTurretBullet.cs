using Godot;
using System;
using System.Collections;

public partial class RoofTurretBullet : Bullet
{
    public override void OnHit(Node2D hit)
    {
        if (hit is TileMapLayer)
        {
            QueueFree();
        }

        if (hit.GetParent() is Enemy)
        {
            hit.GetParent().QueueFree();
            QueueFree();
        }
    }


    public override void _PhysicsProcess(double delta)
    {
        if (DistanceTravelled() > 2000.0)
        {
            QueueFree();
        }

        MoveAndSlide();
    }
}
