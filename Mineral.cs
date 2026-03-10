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
                Right.RegionRect = new Rect2(00, 00, new Vector2(32, 32));
                break;
            case MineralType.Purple:
                Right.RegionRect = new Rect2(00, 32, new Vector2(32, 32));
                break;
            case MineralType.Yellow:
                Right.RegionRect = new Rect2(00, 64, new Vector2(32, 32));
                break;
        }
    }

    public void ShowSide(string side)
    {
        switch (side) {
            case "Right":
                Right.Show();
                break;
            case "Left":
                Left.Show();
                break;
            case "Top":
                Top.Show();
                break;
            case "Bottom":
                Bottom.Show();
                break;
        }
    }
}
