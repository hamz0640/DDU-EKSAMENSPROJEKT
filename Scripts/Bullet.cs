using Godot;
using System;


// The `Bullet` class, is meant to be inherited from, not instanced directly. It
// serves as scaffolding for any type of bullet, and provides helper methods to 
// help program the behaviour of the bullet
//
// IMPORTANT:
// It is expected that the bullet has an Area2D, called "Area2D" as a child. If
// this is not the case, it will probably crash, who know? 
//
// IMPORTANT:
// Do not override the _Ready function when inheriting from this, otherwise, it
// will not work. If you need to override the _Ready function, please copy the
// existing _Ready function into the new one, as otherwise the helper methods
// will not work :(
//
// Helper methods:
// - Vector2 SpawnPoint()
//     Returns a global `Vector2` that describes where the bullet was spawned, i.e.
//     where it was shot from
// - float DistanceTravelled()
//     Returns the distance from the original point where it was shot from.
//     Useful for despawning bullets when they get too far away from the origin
//     without hitting anything
// - abstract (in spirit) void OnHit(Node2D hit)
//     Called whenever the bullet hits *something*. This "something", can either
//     be anything that inherits from `PhysicsBody2D`, (i.e. Rigidbodies or 
//     Characterbodies), or it can be an Area2D. Maybe it can be TilemapLayers
//     too?
//     This is meant to be overriden, by the inherited class (Hence, the abstract
//     marker), so that the bullet can handle whatever it is. An implementation
//     of an enemy bullet, would for example check if the hit entity is a
//     player, a forcefield, a ship or an turret, and if it is not any of those,
//     just ignore it.
[GlobalClass]
public partial class Bullet : CharacterBody2D
{
    public Vector2 SpawnPoint { get; set; }= Vector2.Inf;
    public virtual void OnHit(Node2D hit)
    {
        throw new NotImplementedException();
    }

    public override void _Ready()
    {
        Area2D area = (Area2D)GetNode("Area2D");
        area.AreaEntered += OnHit;
        area.BodyEntered += OnHit;

        SpawnPoint = GlobalPosition;
    }

    public float DistanceTravelled()
    {
        return SpawnPoint.DistanceTo(GlobalPosition);
    }
}
