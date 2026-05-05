using Godot;
using System;

[GlobalClass]
public partial class ShieldHealthUpgrade : Upgrade
{
    public override void OnBuy(SceneTree _tree)
    {
        Global global = Global.GetInstance();
        float max = global.GetState<float>("ShieldHealth") + 200f;
        global.SetState("ShieldHealth", max);
        global.SetState("CurrentShieldHealth", max);
        AmountBought += 1;
    }
}
