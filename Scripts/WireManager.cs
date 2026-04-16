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
    }

    public override void _Process(double delta)
    {
        if (player.GlobalPosition.Y - 10 > nextSpawn.Y)
        {
            HandleMine((float)delta, wireArea.GlobalPosition);
            CreateNew();
        }
    }

    public void CreateNew()
    {
        bool touchingGround = false;
        var bodies = wireArea.GetOverlappingBodies();
        foreach (var body in bodies)
        {
            if (body.Name == "GroundLayer" || body.IsInGroup("Ground"))
                touchingGround = true;
        }

        if (!touchingGround)
        {
            nextSpawn.Y += spawnInterval;
            Area2D newArea = (Area2D)wireArea.Duplicate();
            newArea.AddToGroup("wire");
            AddChild(newArea);
            newArea.GlobalPosition = nextSpawn;

            // Flyt detektoren til den nye spids
            wireArea.GlobalPosition = nextSpawn;
        }
    }

    private void HandleMine(float delta, Vector2 checkPosition)
    {
        Global global = Global.GetInstance();
        Ground ground = (Ground)GetTree().GetFirstNodeInGroup("Ground");

        if (ground == null) 
            return;

        float miningSpeed = global.GetState<float>("MiningSpeed");
        Vector2I tilePosition = ground.ToTilePosition(checkPosition + new Vector2(0, -10));
        Vector2I miningTilePosition = tilePosition + Vector2I.Down;

        TileData tileData = ground.GroundLayer.GetCellTileData(miningTilePosition);

        if (tileData == null)
        {
            miningTilePosition = tilePosition;
            tileData = ground.GroundLayer.GetCellTileData(miningTilePosition);
        }

        if (tileData == null) 
            return;

        if (!ground.TileHealth.ContainsKey(miningTilePosition))
        {
            ground.TileHealth[miningTilePosition] = 10.0f;
        }

        float tileHealth = ground.TileHealth[miningTilePosition];
        float damage = miningSpeed * delta;
        float newTileHealth = tileHealth - damage;

        if (newTileHealth <= 0.0f)
        {
            ground.BreakTile(miningTilePosition);
            ground.TileHealth.Remove(miningTilePosition);
        }
        else
        {
            ground.TileHealth[miningTilePosition] = newTileHealth;
        }
    }
}