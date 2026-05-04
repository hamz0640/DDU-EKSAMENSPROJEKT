using Godot;
using System;

[GlobalClass]
public partial class ShieldHealthUpgrade : Upgrade
{
    public override void OnBuy(SceneTree _tree)
    {
        Global global = Global.GetInstance();
        float shieldHealth = global.GetState<float>("CurrentShieldHealth") + 200.0f;
        global.SetState("CurrentShieldHealth", shieldHealth);
        AmountBought += 1;
    }
}
