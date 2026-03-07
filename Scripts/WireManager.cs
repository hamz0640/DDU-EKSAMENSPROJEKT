using Godot;
using System;

public partial class WireManager : Node2D
{
    [Export] private Node2D player;
    [Export] private Area2D wireTemplate;

    private float nextSpawnY;
    private float spawnInterval = 32f;

    public override void _Ready()
    {
        nextSpawnY = GlobalPosition.Y - spawnInterval;
    }

    public override void _Process(double delta)
    {
        if (player.GlobalPosition.Y > nextSpawnY)
        {
            CreateNew(new Vector2(GlobalPosition.X, nextSpawnY));

            nextSpawnY += spawnInterval;
        }
    }

    public void CreateNew(Vector2 spawnPos)
    {

        Area2D newArea = (Area2D)wireTemplate.Duplicate();
        AddChild(newArea);
        newArea.GlobalPosition = spawnPos;

        GD.Print($"New wire created at: {spawnPos}");
    }
}