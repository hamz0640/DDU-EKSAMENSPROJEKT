using Godot;
using System;
using System.Collections.Generic;

public partial class WireManager : Node2D
{
    [Export] private Node2D player;
    [Export] private Area2D wireArea;
    [Export] private Sprite2D wireSegmentSprite; // Assign your wire sprite template in the editor

    private Vector2 nextSpawn;
    private int startYpos = -45;
    private float spawnInterval = 4f;

    private List<Sprite2D> spawnedSegments = new();
    private List<CollisionShape2D> spawnedCollisions = new();

    public override void _Ready()
    {
        nextSpawn.Y = startYpos + spawnInterval;
        nextSpawn.X = wireArea.GlobalPosition.X;
    }

    public void CheckIfNewNeeded()
    {
        var character = player.GetNode<CharacterBody2D>("CharacterBody2D");
        if (character.GlobalPosition.Y + 10 > nextSpawn.Y)
        {
            ExtendWire();
        }
    }

    public void ExtendWire()
    {
        bool touchingGround = false;
        var bodies = wireArea.GetOverlappingBodies();
        foreach (var body in bodies)
        {
            if (body.Name == "GroundLayer")
                touchingGround = true;
        }

        if (!touchingGround)
        {
            nextSpawn.Y += spawnInterval;

            // --- Add a new visual sprite segment ---
            Sprite2D newSegment = new Sprite2D();
            newSegment.Texture = wireSegmentSprite.Texture;
            newSegment.GlobalPosition = nextSpawn;
            AddChild(newSegment);
            spawnedSegments.Add(newSegment);

            // --- Add a matching collision segment to the wireArea ---
            CollisionShape2D newCollision = new CollisionShape2D();
            RectangleShape2D shape = new RectangleShape2D();
            shape.Size = new Vector2(4, spawnInterval); // Match your wire width
            newCollision.Shape = shape;
            newCollision.Position = wireArea.ToLocal(nextSpawn);
            wireArea.AddChild(newCollision);
            spawnedCollisions.Add(newCollision);

            // Move the detection tip to the new bottom
            wireArea.GlobalPosition = nextSpawn;
        }
    }
}