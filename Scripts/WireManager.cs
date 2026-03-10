using Godot;
using System;

public partial class WireManager : Node2D
{
    [Export] private Node2D player;
    [Export] private Area2D wireTemplate;
    [Export] private int startYpos = -21;
    [Export] private Area2D wireArea;
    private float nextSpawnY;
    private float spawnInterval = 4f;

    public override void _Ready()
    {
        //Original spawn Y koordinat
        nextSpawnY = startYpos + spawnInterval;
    }

    public void CheckIfNewNeeded()
    {
        var character = player.GetNode<CharacterBody2D>("CharacterBody2D");
        //Hvis spilleren er længere nede end wire, lav wiren længere
        if (character.GlobalPosition.Y + 0 > nextSpawnY)
        {
            CreateNew();
        }
    }

    public void CreateNew()
    {
        nextSpawnY += spawnInterval;
        //Lav ny wire i forlængelse af den gamle HVIS IKKE den ville ramme en ground block
        bool touchingGround = false;
        var bodies = wireArea.GetOverlappingBodies();
        foreach (var body in bodies) {
            if (body.Name == "GroundLayer")
            {
                touchingGround = true;
            }
        }
        if (!touchingGround)
        {
            Area2D newArea = (Area2D)wireTemplate.Duplicate();
            AddChild(newArea);
            newArea.GlobalPosition = spawnPos;
            //Flyt den detekterene del af wiren til spidsen
            wireArea.GlobalPosition = spawnPos;
        }
        else
            GD.Print("WIRE BLOCKED, the wire is touching the ground, so it is not to be extended");
    }
}