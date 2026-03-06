using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

public partial class Ground : Node2D
{
    private FastNoiseLite GroundNoise = new();
    private Vector2I lastTilePosition = Vector2I.MaxValue;
    private HashSet<Vector2I> loadedTiles = new();
    private HashSet<Vector2I> UnbreakableTiles = new();
    [ExportGroup("Configurations")]
    [Export]
    public float CaveThreshold = 0.2f;
    [Export]
    public int TileMargin = 1;
    [ExportGroup("References")]
    [Export]
    public TileMapLayer BackgroundLayer = null;
    [Export]
    public TileMapLayer GroundLayer = null;
    [Export]
    public TileMapLayer MineralLayer = null;


    public override void _Ready()
    {
        Random random = new();
        GroundNoise.Seed = random.Next();
        GroundNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        GroundNoise.Frequency = 0.05f;

        for (int x = 0; x < 10; x++)
        {
            for (int y = -3; y < 1; y++)
            {
                loadedTiles.Add(new Vector2I(x, y));
            }
        }

        GroundLayer.SetCellsTerrainConnect([
            new Vector2I(0, 0),
            new Vector2I(2, 0),
            new Vector2I(3, 0),
            new Vector2I(4, 0),
            new Vector2I(5, 0),
            new Vector2I(6, 0),
            new Vector2I(7, 0),
            new Vector2I(8, 0),
            new Vector2I(9, 0),
            new Vector2I(10, 0),
            new Vector2I(11, 0)
        ], 0, 0);

        BackgroundLayer.SetCellsTerrainConnect([
            new Vector2I(0, 0),
            new Vector2I(1, 0),
            new Vector2I(2, 0),
            new Vector2I(3, 0),
            new Vector2I(4, 0),
            new Vector2I(5, 0),
            new Vector2I(6, 0),
            new Vector2I(7, 0),
            new Vector2I(8, 0),
            new Vector2I(9, 0),
            new Vector2I(10, 0),
            new Vector2I(11, 0)
        ], 0, 0);
    }


    public override void _Process(double delta)
    {
        Camera2D camera = (Camera2D)GetTree().GetFirstNodeInGroup("Camera");
        Vector2 cameraPosition = camera.GlobalPosition;

        float screenToTileFactor = 32.0f;
        Vector2I tilePosition = new Vector2I(
            (int)(cameraPosition.X / screenToTileFactor),
            (int)(cameraPosition.Y / screenToTileFactor)
        );
        
        if (tilePosition == lastTilePosition)
        {
            return;
        }
        else
        {
            lastTilePosition = tilePosition;
        }

        int horizontalTileExtent = (int)Mathf.Ceil(960.0 / camera.Zoom.X / screenToTileFactor);
        int leftMostTile   = tilePosition.X - horizontalTileExtent - TileMargin;
        int rightMostTile  = tilePosition.X + horizontalTileExtent + TileMargin;
        
        int verticalTileExtent = (int)Mathf.Ceil(540.0 / camera.Zoom.Y / screenToTileFactor);
        // int topMostTile    = tilePosition.Y - verticalTileExtent + TileMargin;
        int bottomMostTile = tilePosition.Y + verticalTileExtent + TileMargin;

        for (int x = leftMostTile; x <= rightMostTile; x++)
        {
            for (int y = bottomMostTile; y >= 0; y--)
            {
                Vector2I localTilePosition = new Vector2I(x, y);
                // GD.Print(localTilePosition);
                if (loadedTiles.Contains(localTilePosition))
                {
                    continue;
                }

                if (y <= 1)
                {
                    BackgroundLayer.SetCellsTerrainConnect(
                        [localTilePosition], 
                        0, 
                        0
                    );
                    GroundLayer.SetCellsTerrainConnect(
                        [localTilePosition], 
                        0, 
                        0
                    );
                    UnbreakableTiles.Add(localTilePosition);
                    continue;
                }

                loadedTiles.Add(localTilePosition);
                BackgroundLayer.SetCellsTerrainConnect(
                    [localTilePosition], 
                    0, 
                    0
                );

                float value = GroundNoise.GetNoise2D(x, y);

                if (value > CaveThreshold)
                    continue;
                
                GroundLayer.SetCellsTerrainConnect(
                    [localTilePosition], 
                    0, 
                    0
                );
            }
        }
    }


    public Vector2I toTilePosition(Vector2 position)
    {
        float screenToTileFactor = 32.0f;
        Vector2I tilePosition = new Vector2I(
            (int)(position.X / screenToTileFactor),
            (int)(position.Y / screenToTileFactor)
        );

        return tilePosition;
    }


    public bool IsBreakable(Vector2I tilePosition)
    {
        return !UnbreakableTiles.Contains(tilePosition);
    }


    public bool BreakTile(Vector2I tilePosition)
    {
        if (UnbreakableTiles.Contains(tilePosition))
            return false;

        GroundLayer.SetCellsTerrainConnect(
            [tilePosition], 0, 0
        );

        return true;
    }
}
