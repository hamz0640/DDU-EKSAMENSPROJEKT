using Godot;
using System;
public partial class WireManager : Node2D
{
    [Export] private CharacterBody2D player;
    [Export] private Area2D wireArea;
    [Export] private Sprite2D wireSprite;
    private Vector2 nextSpawn;
    private int startYpos = -45;
    private float spawnInterval = 4f;

    public override void _Ready()
    {
        nextSpawn.Y = startYpos + spawnInterval;
        nextSpawn.X = wireArea.GlobalPosition.X;
    } // Original spawn position
    
    public override void _Process(double delta)
    {
        if (player.GlobalPosition.Y + 10 > nextSpawn.Y)
        {
            HandleMine((float)delta);
            CreateNew();
        }
    }

    public void CreateNew()
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
            Area2D newArea = (Area2D)wireArea.Duplicate();
            newArea.AddToGroup("wire");
            AddChild(newArea);
            newArea.GlobalPosition = nextSpawn;
            // Flyt den detekterene del af wiren til spidsen
            wireArea.GlobalPosition = nextSpawn;

        }
    } // Lav ny wire i forlængelse af den gamle HVIS IKKE den ville ramme en ground block

    private void HandleMine(float delta)
    {
        Global global = Global.GetInstance();
        Ground ground = (Ground)GetTree().GetFirstNodeInGroup("Ground");

        float miningSpeed = global.GetState<float>("MiningSpeed");

        Vector2I tileDirection = (Vector2I)Vector2.Down;
        Vector2I tilePosition = ground.ToTilePosition(GlobalPosition);
        Vector2I miningTilePosition = tilePosition + tileDirection;

        if (!ground.TileHealth.ContainsKey(miningTilePosition))
        {
            GD.Print("bjjksgnjsnjsgjssgesjgeks");
            return;
        }

        float tileHealth = ground.TileHealth[miningTilePosition];
        float newTileHealth = tileHealth - miningSpeed * (float)delta;

        GD.Print($"Graver i: {miningTilePosition} - HP tilbage: {newTileHealth}");

        if (newTileHealth <= 0.0)
            ground.BreakTile(miningTilePosition);
        else
            ground.TileHealth[miningTilePosition] = newTileHealth;
    }
}