using Godot;
using System;
using System.Text.RegularExpressions;

public partial class Mineral : Node2D
{
    [Export] Sprite2D Right  = null;
    [Export] Sprite2D Left   = null;
    [Export] Sprite2D Top    = null;
    [Export] Sprite2D Bottom = null;
    public enum MineralType
    {
        Red,
        Purple,
        Yellow,
    }
    private MineralType Type = MineralType.Red;

    public void SetMineralType(MineralType mineralType)
    {
        Type = mineralType;
        
        switch (Type) {
            case MineralType.Red:
                Rect2 rect1 = new Rect2(10, 00, new Vector2(32, 32)); 
                Right.RegionRect  = rect1;
                Left.RegionRect   = rect1;
                Top.RegionRect    = rect1;
                Bottom.RegionRect = rect1;
                break;
            case MineralType.Purple:
                Rect2 rect2 = new Rect2(10, 32, new Vector2(32, 32)); 
                Right.RegionRect  = rect2;
                Left.RegionRect   = rect2;
                Top.RegionRect    = rect2;
                Bottom.RegionRect = rect2;
                break;
            case MineralType.Yellow:
                Rect2 rect3 = new Rect2(10, 64, new Vector2(32, 32)); 
                Right.RegionRect  = rect3;
                Left.RegionRect   = rect3;
                Top.RegionRect    = rect3;
                Bottom.RegionRect = rect3;
                break;
        }
    }

    public void ShowSide(Vector2I side)
    {
        switch (side) {
            case Vector2I(1, 0):
                Right.Show();
                break;
            case Vector2I(-1, 0):
                Left.Show();
                break;
            case Vector2I(0, -1):
                Top.Show();
                break;
            case Vector2I(0, 1):
                Bottom.Show();
                break;
        }
    }


    public void HideSide(Vector2I side)
    {
        switch (side) {
            case Vector2I(1, 0):
                Right.Hide();
                break;
            case Vector2I(-1, 0):
                Left.Hide();
                break;
            case Vector2I(0, -1):
                Top.Hide();
                break;
            case Vector2I(0, 1):
                Bottom.Hide();
                break;
        }
    }
}
