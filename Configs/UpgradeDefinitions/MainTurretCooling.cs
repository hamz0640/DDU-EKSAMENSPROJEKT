using Godot;
using System;

[GlobalClass]
public partial class MainTurretCooling : Upgrade
{
    public override void OnBuy(SceneTree _tree)
    {
        Global global = Global.GetInstance();
        float shieldHealth = global.GetState<float>("MainTurretCooling") * 0.9f;
        global.SetState("MainTurretCooling", shieldHealth);
        AmountBought += 1;
        GD.Print("purchased");
    }
}
