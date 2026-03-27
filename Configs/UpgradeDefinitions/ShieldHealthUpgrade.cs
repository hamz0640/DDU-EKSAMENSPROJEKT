using Godot;
using System;

[GlobalClass]
public partial class ShieldHealthUpgrade : Upgrade
{
    public override void OnBuy(SceneTree _tree)
    {
        Global global = Global.GetInstance();
        float shieldHealth = global.GetState<float>("ShieldHealth") + 20.0f;
        global.SetState("ShieldHealth", shieldHealth);
        AmountBought += 1;
    }
}
