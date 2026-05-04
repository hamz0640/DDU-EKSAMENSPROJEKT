using Godot;
using System;

[GlobalClass]
public partial class TurretEfficiency : Upgrade
{
    public override void OnBuy(SceneTree _tree)
    {
        Global global = Global.GetInstance();
        float shieldHealth = global.GetState<float>("ShieldHealth") + 200.0f;
        global.SetState("ShieldHealth", shieldHealth);
        AmountBought += 1;
    }
}
