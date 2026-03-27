using Godot;
using System;

[GlobalClass]
public partial class TurretUpgrade : Upgrade
{
    public override void OnBuy(SceneTree tree)
    {
        Window root = tree.Root;

        PackedScene turretScene = GD.Load<PackedScene>("res://Scenes/Turret.tscn");
        Turret turret = (Turret)turretScene.Instantiate();
        turret.GlobalPosition = new Vector2(0, -1000);
        root.AddChild(turret);
    }

    public override bool CanBuy(SceneTree tree)
    {
        return tree.GetNodeCountInGroup("Turret") < 2;
    }
}
