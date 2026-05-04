using Godot;
using System;

[GlobalClass]
public partial class TurretEfficiency : Upgrade
{
    public override void OnBuy(SceneTree _tree)
    {
        Global global = Global.GetInstance();
        float shieldHealth = global.GetState<float>("TurretEfficiency") * 0.9f;
        global.SetState("TurretEfficiency", shieldHealth);
        AmountBought += 1;
        GD.Print("purchased");
    }
}
