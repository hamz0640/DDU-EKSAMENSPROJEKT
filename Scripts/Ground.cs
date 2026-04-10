using Godot;
using System;
using System.Collections.Generic;

public partial class Ground : Node2D
{
    private FastNoiseLite GroundNoise = new();
    private Vector2I lastTilePosition = Vector2I.MaxValue;
    private HashSet<Vector2I> LoadedTiles = new();
    private HashSet<Vector2I> PlacedTiles = new();
    private HashSet<Vector2I> UnbreakableTiles = new();
    private Dictionary<Vector2I, Mineral.MineralType> Minerals = new();
    private Dictionary<Vector2I, Mineral> MineralNodeLUT = new();
    private Random rand = new();
    public Dictionary<Vector2I, float> TileHealth = new();
    [ExportGroup("Configurations")]
    [Export]
    public float CaveThreshold = 0.25f;
    [Export]
    public int TileMargin = 1;
    [Export]
    public float RedProbability = 0.1f;
    [Export]
    public float PurpleProbability = 0.1f;
    [Export]
    public float YellowProbability = 0.1f;
    [ExportGroup("References")]
    [Export]
    public TileMapLayer BackgroundLayer = null;
    [Export]
    public TileMapLayer GroundLayer = null;


    public override void _Ready()
    {
        GroundNoise.Seed = rand.Next();
        GroundNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        GroundNoise.Frequency = 0.2f;

        for (int x = 0; x < 10; x++)
        {
            for (int y = -3; y < 1; y++)
            {
                LoadedTiles.Add(new Vector2I(x, y));
            }
        }

        PlaceGroundTile(new Vector2I(0, 0), true);
        PlaceGroundTile(new Vector2I(2, 0), true);
        PlaceGroundTile(new Vector2I(3, 0), true);
        PlaceGroundTile(new Vector2I(4, 0), true);
        PlaceGroundTile(new Vector2I(5, 0), true);
        PlaceGroundTile(new Vector2I(6, 0), true);
        PlaceGroundTile(new Vector2I(7, 0), true);
        PlaceGroundTile(new Vector2I(8, 0), true);
        PlaceGroundTile(new Vector2I(9, 0), true);
        PlaceGroundTile(new Vector2I(10, 0), true);
        PlaceGroundTile(new Vector2I(11, 0), true);

        PlaceBackgroundTile(new Vector2I(0, 0));
        PlaceBackgroundTile(new Vector2I(1, 0));
        PlaceBackgroundTile(new Vector2I(2, 0));
        PlaceBackgroundTile(new Vector2I(3, 0));
        PlaceBackgroundTile(new Vector2I(4, 0));
        PlaceBackgroundTile(new Vector2I(5, 0));
        PlaceBackgroundTile(new Vector2I(6, 0));
        PlaceBackgroundTile(new Vector2I(7, 0));
        PlaceBackgroundTile(new Vector2I(8, 0));
        PlaceBackgroundTile(new Vector2I(9, 0));
        PlaceBackgroundTile(new Vector2I(10, 0));
        PlaceBackgroundTile(new Vector2I(11, 0));
    }


    public override void _Process(double delta)
    {
        LoadNewTiles();
    }

    private void LoadNewTiles()
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
        int bottomMostTile = tilePosition.Y + verticalTileExtent + TileMargin;
        int topMostTile    = tilePosition.Y - verticalTileExtent - TileMargin;
        topMostTile = Math.Max(0, topMostTile);

        for (int x = leftMostTile; x <= rightMostTile; x++)
        {
            for (int y = bottomMostTile; y >= topMostTile; y--)
            {
                Vector2I localTilePosition = new Vector2I(x, y);
                if (LoadedTiles.Contains(localTilePosition))
                    continue;

                PlaceBackgroundTile(localTilePosition);

                if (y < 1 || (GroundNoise.GetNoise2D(x / 3.0f, y) < CaveThreshold))
                {
                    PlaceGroundTile(localTilePosition, y < 1);
                }
            }
        }

        for (int x = leftMostTile; x <= rightMostTile; x++)
        {
            for (int y = bottomMostTile; y >= topMostTile; y--)
            {
                Vector2I localTilePosition = new Vector2I(x, y);
                if (LoadedTiles.Contains(localTilePosition))
                    continue;
                
                if (!PlacedTiles.Contains(localTilePosition))
                    continue;

                if (y >= 1)
                    SpawnMinerals(localTilePosition);
            }
        }

        for (int x = leftMostTile; x <= rightMostTile; x++)
        {
            for (int y = bottomMostTile; y >= topMostTile; y--)
            {
                Vector2I localTilePosition = new Vector2I(x, y);
                LoadedTiles.Add(localTilePosition);
            }
        }
    }

    private void SpawnMinerals(Vector2I tilePosition)
    {
        float mineralValue = rand.NextSingle();
        Mineral.MineralType mineralType = Mineral.MineralType.Red;

        float loRed = 0.0f;
        float hiRed = loRed + RedProbability;
        float loPurple = hiRed;
        float hiPurple = loPurple + PurpleProbability;
        float loYellow = hiPurple;
        float hiYellow = loYellow + YellowProbability;


        bool shouldInstantiate = false;
        if (mineralValue > loRed && mineralValue < hiRed)
        {
            mineralType = Mineral.MineralType.Red;
            shouldInstantiate = true;
        }
            
        if (mineralValue > loPurple && mineralValue < hiPurple)
        {
            mineralType = Mineral.MineralType.Purple;
            shouldInstantiate = true;
        }
        if (mineralValue > loYellow && mineralValue < hiYellow)
        {
            mineralType = Mineral.MineralType.Yellow;
            shouldInstantiate = true;
        }

        if (!shouldInstantiate)
            return;
            
        
        PackedScene mineralScene = GD.Load<PackedScene>("res://Scenes/mineral.tscn");
        Mineral mineral = mineralScene.Instantiate<Mineral>();
        AddChild(mineral);
        
        Minerals[tilePosition] = mineralType;
        MineralNodeLUT[tilePosition] = mineral;
        
        mineral.SetMineralType(mineralType);
        mineral.Position = tilePosition * 32 + new Vector2(16, 16);

        if (!IsTileAt(tilePosition + new Vector2I(0, -1)))
            mineral.ShowSide(Vector2I.Up);
        if (!IsTileAt(tilePosition + new Vector2I(1, 0)))
            mineral.ShowSide(Vector2I.Right);
        if (!IsTileAt(tilePosition + new Vector2I(0, 1)))
            mineral.ShowSide(Vector2I.Down);
        if (!IsTileAt(tilePosition + new Vector2I(-1, 0)))
            mineral.ShowSide(Vector2I.Left);
    } 


    public bool IsTileAt(Vector2I tilePosition)
    {
        return PlacedTiles.Contains(tilePosition);
    }


    public Vector2I ToTilePosition(Vector2 position)
    {
        return GroundLayer.LocalToMap(position);
    }


    public bool IsBreakable(Vector2I tilePosition)
    {
        return !UnbreakableTiles.Contains(tilePosition);
    }


    public bool BreakTile(Vector2I tilePosition)
    {        
        if (UnbreakableTiles.Contains(tilePosition))
            return false;
        
        MineralNodeLUT.TryGetValue(tilePosition, out Mineral mineralCenter);
        if (mineralCenter != null)
        {
            Mineral.MineralType mineralType = Minerals[tilePosition];
            Global global = (Global)GetTree().Root.GetNode("Global");
            global.EmitSignal("MineralCountUpdated", [(int)mineralType, true]);

            float fortune = global.GetState<float>("Fortune");
            Random random = new Random();
            while (random.NextSingle() * fortune > 1.0)
            {
                global.EmitSignal("MineralCountUpdated", [(int)mineralType, true]);
            }

            RemoveChild(mineralCenter);
            Minerals.Remove(tilePosition);
            MineralNodeLUT.Remove(tilePosition);
        }

        MineralNodeLUT.TryGetValue(tilePosition + Vector2I.Up,    out Mineral mineralUp);
        MineralNodeLUT.TryGetValue(tilePosition + Vector2I.Down,  out Mineral mineralDown);
        MineralNodeLUT.TryGetValue(tilePosition + Vector2I.Right, out Mineral mineralRight);
        MineralNodeLUT.TryGetValue(tilePosition + Vector2I.Left,  out Mineral mineralLeft);

        if (mineralUp    != null) mineralUp.ShowSide(Vector2I.Down);
        if (mineralDown  != null) mineralDown.ShowSide(Vector2I.Up);
        if (mineralRight != null) mineralRight.ShowSide(Vector2I.Left);
        if (mineralLeft  != null) mineralLeft.ShowSide(Vector2I.Right);

        GroundLayer.SetCellsTerrainConnect([tilePosition], 0, -1);

        return true;
    }

    public void PlaceGroundTile(Vector2I tilePosition, bool isUnbreakable)
    {
        MineralNodeLUT.TryGetValue(tilePosition + Vector2I.Up,    out Mineral mineralUp);
        MineralNodeLUT.TryGetValue(tilePosition + Vector2I.Down,  out Mineral mineralDown);
        MineralNodeLUT.TryGetValue(tilePosition + Vector2I.Right, out Mineral mineralRight);
        MineralNodeLUT.TryGetValue(tilePosition + Vector2I.Left,  out Mineral mineralLeft);

        if (mineralUp    != null) mineralUp.HideSide(Vector2I.Down);
        if (mineralDown  != null) mineralDown.HideSide(Vector2I.Up);
        if (mineralRight != null) mineralRight.HideSide(Vector2I.Left);
        if (mineralLeft  != null) mineralLeft.HideSide(Vector2I.Right);

        PlacedTiles.Add(tilePosition);

        if (isUnbreakable)
            UnbreakableTiles.Add(tilePosition);
        
        GroundLayer.SetCellsTerrainConnect([tilePosition], 0, 0);
        TileHealth[tilePosition] = 1.0f;
    }


    public void PlaceBackgroundTile(Vector2I tilePosition)
    {     
        BackgroundLayer.SetCellsTerrainConnect([tilePosition], 0, 0);
    }
}
