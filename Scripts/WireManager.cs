using Godot;
using System;

public partial class WireManager : Node2D
{
    [Export] private Node2D player;
    [Export] private Area2D wireTemplate;
    [Export] private int startYpos = -21;

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
            CreateNew(new Vector2(GlobalPosition.X, nextSpawnY));
            nextSpawnY += spawnInterval;
        }
    }

    public void CreateNew(Vector2 spawnPos)
    {
        //Lav ny wire i forlængelse af den gamle HVIS IKKE den ville ramme en ground block
        Area2D newArea = (Area2D)wireTemplate.Duplicate();
        AddChild(newArea);
        newArea.GlobalPosition = spawnPos;
    }
}