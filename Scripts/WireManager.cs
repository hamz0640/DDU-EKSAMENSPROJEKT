using Godot;
using System;

public partial class WireManager : Node2D
{
    [Export] private Node2D player;
    [Export] private Area2D wireTemplate;
    [Export] private int startYpos = -30;
    [Export] private Area2D wireArea;
    private Vector2 nextSpawn;
    private float spawnInterval = 4f;

    public override void _Ready()
    {
        nextSpawn.Y = startYpos + spawnInterval;
        nextSpawn.X = wireTemplate.GlobalPosition.X;
    } // Original spawn position

    public void CheckIfNewNeeded()
    {
        var character = player.GetNode<CharacterBody2D>("CharacterBody2D");
        if (character.GlobalPosition.Y + 10 > nextSpawn.Y)
        {
            CreateNew();
        }
    } // Hvis spilleren er længere nede end wire, lav wiren længere

    public void CreateNew()
    {
        bool touchingGround = false;
        var bodies = wireArea.GetOverlappingBodies();
        foreach (var body in bodies) {
            if (body.Name == "GroundLayer")
                touchingGround = true;
        }
        if (!touchingGround)
        {
            nextSpawn.Y += spawnInterval;
            Area2D newArea = (Area2D)wireTemplate.Duplicate();
            AddChild(newArea);
            newArea.GlobalPosition = nextSpawn;
            //Flyt den detekterene del af wiren til spidsen
            wireArea.GlobalPosition = nextSpawn;
        }
    } // Lav ny wire i forlængelse af den gamle HVIS IKKE den ville ramme en ground block
}